using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.RemovingBodyFunction;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class RemovingBodyFunctionFunctions
  {

    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = base.Execute(approvalTask);
      var documents = new List<Sungero.Docflow.IOfficialDocument>();
      
      // Документы поверенных
      var documentAttorneyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DocumentAttorneyKindGuid);
      var documentsAttorney = approvalTask.OtherGroup.All.Where(x => Sungero.Docflow.OfficialDocuments.Is(x) && documentAttorneyKind.Equals(Sungero.Docflow.OfficialDocuments.As(x).DocumentKind))
        .Select(x => Sungero.Docflow.OfficialDocuments.As(x)).ToList();
      if (documentsAttorney.Any())
        documents.AddRange(documentsAttorney);
      
      // Главный документ (Заявка на оформление доверенности или Заявка на оформление нотариальной доверенности)
      var requestNpoaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
      var requestPoaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.RequestToCreatePowerOfAttorneyKindGuid);
      var mainDoc = approvalTask.DocumentGroup.OfficialDocuments.FirstOrDefault(x => requestPoaKind.Equals(x.DocumentKind) || requestNpoaKind.Equals(x.DocumentKind));
      if (mainDoc != null)
        documents.Add(mainDoc);
      
      if (!documents.Any())
        return this.GetErrorResult("Отсутствуют документы поверенных и заявка на оформление доверенности");
      
      if (!RemoveBodyDocuments(documents))
        result = this.GetRetryResult(string.Empty);
      
      return result;
    }
    
    /// <summary>
    /// Удалить версии документов, изъять права на просмотр и изменение у всех пользователей на эти документы и перевести в состояние Устаревший
    /// </summary>
    /// <param name="documents">Список документов</param>
    /// <returns>true - если все документы успешно обработанны, если хотябы 1 с ошибкой - false</returns>
    [Public]
    public static bool RemoveBodyDocuments(List<Sungero.Docflow.IOfficialDocument> documents)
    {
      var isAllSuccess = true;
      foreach (var document in documents)
      {
        try
        {
          var lockInfo = Locks.GetLockInfo(document);
          if (lockInfo.IsLocked)
            throw AppliedCodeException.Create(string.Format("Документ с ИД={0} заблокирован пользователем {1}", document.Id, lockInfo.OwnerName));

          // Удалить версии
          if (document.HasVersions)
            document.Versions.Clear();
          
          // Состояние = устаревший
          document.LifeCycleState = Sungero.Docflow.OfficialDocument.LifeCycleState.Obsolete;
          
          // Изъять права у субъектов
          var recipients = Sungero.CoreEntities.Recipients.GetAll();
          foreach (var recipient in recipients)
          {
            if (recipient.IsSystem == true)
              continue;
            if (document.AccessRights.CanRead(recipient))
              document.AccessRights.RevokeAll(recipient);
          }
          document.Save();
        }
        catch (AppliedCodeException ex)
        {
          Logger.Debug("Avis - RemoveBodyDocuments. {0}", ex.Message);
          isAllSuccess = false;
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Avis - RemoveBodyDocuments. Ошибка обработки документа с ИД={0}: {1}.", document.Id, ex.Message);
          isAllSuccess = false;
        }
      }
      return isAllSuccess;
    }

  }
}