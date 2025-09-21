using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.OutgoingLetters.FormationIncomingLettersWithinGroup;

namespace lenspec.OutgoingLetters.Server
{
  partial class FormationIncomingLettersWithinGroupFunctions
  {
    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)

    {
      var result = base.Execute(approvalTask);
      var document = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (document == null)
      {
        return this.GetErrorResult("Не найден документ.");
      }
      // Отобрали корреспондентов-зеркала.
      var correspondents = GetDocumentCorrespondent(document);
      if (!correspondents.Any())
      {
        return this.GetSuccessResult();
      }
      var businessUnit = lenspec.Etalon.BusinessUnits.GetAll();
      
      #region Проверить, чтобы у всех используемых НОР был заполнен Ответственный делопроизводитель - он будет автором задачи.
      
      var emptyResponsibleClerk = new List<Sungero.Company.IBusinessUnit>();
      foreach(var correspondent in correspondents)
      {
        var companyBU = businessUnit.Where(x => correspondent.Equals(x.Company)).Select(x => lenspec.Etalon.BusinessUnits.As(x)).SingleOrDefault();
        if (!companyBU.RoleKindEmployeelenspec.Any(x => x.RoleKind.Name.Equals(lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk)))
          emptyResponsibleClerk.Add(companyBU);
      }
      if (emptyResponsibleClerk.Any())
        return this.GetErrorResult(string.Format("В НОР {0} не заполнен {1}.",
                                                 string.Join(", ", emptyResponsibleClerk.Select(x => x.Name).ToList()),
                                                 lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk));
      
      #endregion
      
      var organization = Sungero.Parties.Counterparties.GetAll();
      var contact = Sungero.Parties.Contacts.GetAll();
      foreach(var correspondent in correspondents)
      {
        //Создадим документ и заполним поля.
        var incomingLetter = lenspec.Etalon.IncomingLetters.Create();
        // Признак для снятия логики коробочной обработки поля Корреспондент (очищает поле В ответ на).
        incomingLetter.IsCorrespondenceWithinGrouplenspec = true;
        //Заполним НОР.
        var nor = businessUnit.Where(x => x.Company.Equals(correspondent)).SingleOrDefault();
        incomingLetter.BusinessUnit = nor;
        // Вычислим подразделение инициатора задачи.
        var author = nor.RoleKindEmployeelenspec.Where(x => x.RoleKind.Name.Equals(lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk)).FirstOrDefault();
        var department = author.Employee.Department;
        //Заполним ИСП.
        incomingLetter.OurCFlenspec = Etalon.OutgoingLetters.As(document).OurCFlenspec;
        //Заполним Содержание.
        incomingLetter.Subject = Etalon.OutgoingLetters.As(document).Subject;
        // Автоматически создастся связь с типом "Ответное письмо".
        incomingLetter.InResponseTo = Sungero.Docflow.OutgoingDocumentBases.As(document);
        //Заполним Корреспондента.
        incomingLetter.Correspondent = document.BusinessUnit.Company;
        //Заполним "Подписал".
        if (document.OurSignatory != null)
        {
          if (contact.Where(x => x.Company != null && x.Company.Equals(incomingLetter.Correspondent) && x.Name == document.OurSignatory.Name && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).Count() == 1)
          {
            incomingLetter.SignedBy = contact.Where(x => x.Company != null && x.Company.Equals(incomingLetter.Correspondent) && x.Name == document.OurSignatory.Name && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).SingleOrDefault();
          }
        }
        //Заполним "Подготовил".
        if (document.PreparedBy != null)
        {
          if (contact.Where(x => x.Company != null && x.Company.Equals(incomingLetter.Correspondent) && x.Name == document.PreparedBy.Name && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).Count() == 1)
          {
            incomingLetter.Contact = contact.Where(x => x.Company != null && x.Company.Equals(incomingLetter.Correspondent) && x.Name == document.PreparedBy.Name && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).SingleOrDefault();
          }
        }

        //Переносим тело исходящего письма (последнюю версию) в карточку входящего письма.
        if(document.HasPublicBody == true && document.LastVersion.AssociatedApplication.Extension == "pdf")
        {
          var publicBody = document.LastVersion.PublicBody.Read();
          incomingLetter.CreateVersionFrom(publicBody, document. AssociatedApplication.Extension);
          publicBody.Close();
        }
        else
        {
          var body = document.LastVersion.Body.Read();
          incomingLetter.CreateVersionFrom(body, document.AssociatedApplication.Extension);
          body.Close();
        }
        
        incomingLetter.Department = department;
        incomingLetter.Save();

        //Получим из константы ид регламента.
        var regulationID = Convert.ToInt64(lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.FormationIncomingLettersWithinGroupFunctionsCode).FirstOrDefault().Value);
        var regulation = Sungero.Docflow.ApprovalRules.GetAll().Where(x => x.Id == regulationID).SingleOrDefault();
        if (regulation == null)
        {
          return this.GetErrorResult("Не найден Регламент");
        }
        
        //Создадим задачу по регламенту.
        try
        {
          var task = Sungero.Docflow.ApprovalTasks.Create();
          task.DocumentGroup.OfficialDocuments.Add(incomingLetter);
          task.ApprovalRule = regulation;
          
          // Синхронизируем вложения.
          foreach(var addenda in approvalTask.AddendaGroup.All.AsEnumerable())
          {
            if (!task.AddendaGroup.All.Contains(addenda))
              task.AddendaGroup.All.Add(addenda);
          }
          foreach(var other in approvalTask.OtherGroup.All.AsEnumerable())
          {
            if (!task.OtherGroup.All.Contains(other))
              task.OtherGroup.All.Add(other);
          }
          
          // Заполним поле Копия.
          if (document.BusinessUnit != null)
          {
            var responsibleClerk = lenspec.Etalon.BusinessUnits.As(document.BusinessUnit).RoleKindEmployeelenspec
              .Where(x => x.RoleKind.Name.Equals(lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk))
              .FirstOrDefault();
            if (responsibleClerk != null)
            {
              var observer = task.Observers.AddNew();
              observer.Observer = responsibleClerk.Employee;
            }
          }
          
          //Проверим заполнен ли автор.
          if (author != null)
          {
            task.Author = author.Employee;
            task.Start();
          }
          else
          {
            var user = Roles.GetAll(n => n.Sid == EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
            var subject = lenspec.OutgoingLetters.FormationIncomingLettersWithinGroups.Resources.ResponsibleClerkNotFound;
            var taskNotification = Sungero.Workflow.SimpleTasks.CreateWithNotices(subject, user);
            taskNotification.Attachments.Add(task);
            taskNotification.Start();
          }
        }
        //Выведем информацию об ошибке (если такая будет).
        catch(Exception ex)
        {
          Logger.ErrorFormat("FormationIncomingLettersWithinGroup - ", ex);
          return this.GetErrorResult(ex.Message);
        }
      }

      return result;

    }
    
    /// <summary>
    /// Проверим значение в поле "Корреспондент" карточки исходящего письма.
    /// </summary>
    /// <returns>Список организаций-адрессатов.</returns>
    private List<Sungero.Parties.ICounterparty> GetDocumentCorrespondent(Sungero.Docflow.IOfficialDocument document)
    {
      var correspondents = new List<Sungero.Parties.ICounterparty>();
      var counterpartyBU = new List<Sungero.Parties.ICounterparty>();
      
      if (Etalon.OutgoingLetters.Is(document))
      {
        if(Etalon.OutgoingLetters.As(document).IsManyAddressees == true)
        {
          var addresses = Etalon.OutgoingLetters.As(document).Addressees.Where(x => Sungero.Parties.Companies.Is(x.Correspondent)).Select(x => x.Correspondent);
          if (addresses != null && addresses.Any())
          {
            correspondents.AddRange(addresses);
          }
        }
        else
        {
          if (Sungero.Parties.Companies.Is(Etalon.OutgoingLetters.As(document).Correspondent))
            correspondents.Add(Etalon.OutgoingLetters.As(document).Correspondent);
        }
        if(correspondents.Count != 0)
        {
          var businessUnit = Sungero.Company.BusinessUnits.GetAll();
          foreach (var correspondent in correspondents)
          {
            //Проверка зеркала НОР
            if (businessUnit.Any(x => x.Company.Equals(correspondent)))
            {
              counterpartyBU.Add(correspondent);
            }
          }
        }
      }
      return counterpartyBU;
    }
    
  }
}