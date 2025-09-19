using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Employee;

namespace lenspec.Etalon
{
  partial class EmployeeServerHandlers
  {

    //Добавлено Avis Expert
    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      
      bool employeeClosedParam;
      if (e.Params.TryGetValue(Etalon.Constants.EtalonIntergation.Employee.EmployeeRecordHasBeenClosedParam, out employeeClosedParam) && employeeClosedParam)
      {
        try
        {
          var errors = string.Empty;
          // Удалить сотрудника из Списков ознакомления/рассмотрения.
          var acquaintanceLists = Sungero.RecordManagement.AcquaintanceLists.GetAll()
            .Where(x => x.Participants.Any(p => Equals(_obj, p.Participant)) || x.ExcludedParticipants.Any(p => Equals(_obj, p.ExcludedParticipant)));
          foreach(var acquaintanceList in acquaintanceLists)
          {
            try
            {
              var changedParticipants = Sungero.RecordManagement.AcquaintanceLists.Get(acquaintanceList.Id);
              var participantsToRemove = changedParticipants.Participants.Where(x => Equals(_obj, x.Participant)).ToList();
              foreach(var participant in participantsToRemove)
              {
                changedParticipants.Participants.Remove(participant);
              }
              
              var participants = changedParticipants.Participants.Select(p => p.Participant).ToList();
              var excludedParticipants = changedParticipants.ExcludedParticipants.Select(x => x.ExcludedParticipant).ToList();
              var performers = Sungero.RecordManagement.Server.AcquaintanceTaskFunctions.GetParticipants(participants, excludedParticipants);
              if (performers.Count == 0)
              {
                Sungero.RecordManagement.AcquaintanceLists.Delete(changedParticipants);
                continue;
              }
              
              var excludedParticipantsToRemove = changedParticipants.ExcludedParticipants.Where(x => Equals(_obj, x.ExcludedParticipant)).ToList();
              foreach(var excludedParticipant in excludedParticipantsToRemove)
              {
                changedParticipants.ExcludedParticipants.Remove(excludedParticipant);
              }
              if (changedParticipants.State.IsChanged)
              {
                changedParticipants.Save();
              }
            }
            catch(Exception ex)
            {
              errors += string.Format(" Список {0} - {1}.", acquaintanceList.Name, ex.Message);
            }
          }
          
          // Удалить сотрудника из групп, в которых он участвует.
          // Исключить роли по Виду должности, роли с одним участником, НОР и Подразделения
          var roleByJobTitleKindIds = EtalonDatabooks.JobTitleKinds.GetAll(x => x.Role != null).Select(x => x.Role.Id).ToList().Distinct();
          var groups = Sungero.CoreEntities.Groups.GetAll()
            .Where(x => !Sungero.Company.BusinessUnits.Is(x) && !Sungero.Company.Departments.Is(x) &&
                   !roleByJobTitleKindIds.Contains(x.Id) && x.RecipientLinks.Any(r => Equals(_obj, r.Member)) &&
                   (!Sungero.CoreEntities.Roles.Is(x) || Sungero.CoreEntities.Roles.Is(x) && Sungero.CoreEntities.Roles.As(x).IsSingleUser != true));
          foreach(var employeeGroup in groups)
          {
            try
            {
              var changedEmployeeGroup = Sungero.CoreEntities.Groups.Get(employeeGroup.Id);
              
              var recipientLinksToRemove = changedEmployeeGroup.RecipientLinks.Where(x => Equals(_obj, x.Member)).ToList();
              foreach(var recipientLink in recipientLinksToRemove)
              {
                changedEmployeeGroup.RecipientLinks.Remove(recipientLink);
              }
              if (changedEmployeeGroup.State.IsChanged)
              {
                changedEmployeeGroup.Save();
              }
            }
            catch(Exception ex)
            {
              errors += string.Format(" Группа {0} - {1}.", employeeGroup.Name, ex.Message);
            }
          }
          
          if (!string.IsNullOrEmpty(errors))
          {
            Logger.ErrorFormat("Удаление закрытого сотрудника {0} из списков ознакомления и групп.{1}", _obj.Id, errors);
          }
        }
        catch(Exception ex)
        {
          Logger.ErrorFormat("Avis Employee - AfterSave - {0} {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
        }
      }
    }
    
    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      base.Saving(e);
      
      // Добавить параметр о закрытии записи сотрудника.
      e.Params.AddOrUpdate(Etalon.Constants.EtalonIntergation.Employee.EmployeeRecordHasBeenClosedParam,
                           _obj.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed && _obj.Status != _obj.State.Properties.Status.OriginalValue);
      
      // Если изменилось состояние или изменился Вид должности.
      if (_obj.State.Properties.Status.OriginalValue != _obj.Status || !Equals(_obj.State.Properties.JobTitleKindlenspec.OriginalValue, _obj.JobTitleKindlenspec))
      {
        try
        {
          var asyncRoleHandler = Etalon.Module.Company.AsyncHandlers.SynchronizeJobTitleKindWithRolelenspec.Create();
          asyncRoleHandler.EmployeeId = _obj.Id;
          asyncRoleHandler.OldJobTitleId = _obj.State.Properties.JobTitleKindlenspec.OriginalValue == null ? 0 : _obj.State.Properties.JobTitleKindlenspec.OriginalValue.Id;
          asyncRoleHandler.NewJobTitleId = _obj.JobTitleKindlenspec == null ? 0 :_obj.JobTitleKindlenspec.Id;
          // Если старый статус был "Закрытая", то false; если пусто или "Действующая", то true.
          asyncRoleHandler.OldStatus = _obj.State.Properties.Status.OriginalValue == Sungero.CoreEntities.DatabookEntry.Status.Closed ? false : true;
          // Если новый статус - "Действующая", то true; если пуст или "Закрыта", то false.
          asyncRoleHandler.NewStatus = _obj.Status == Sungero.CoreEntities.DatabookEntry.Status.Active ? true : false;
          asyncRoleHandler.ExecuteAsync();
        }
        catch(Exception ex)
        {
          Logger.Error(ex.Message);
        }
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.CivilLawContractlenspec = false;
      _obj.IsGPHavis = false;
      _obj.IsMaternityLeavelenspec = Employee.IsMaternityLeavelenspec.No;
      
      if (_obj.State.IsCopied)
        _obj.ExternalCodeavis = null;
    }
    //конец Добавлено Avis Expert
  }

}