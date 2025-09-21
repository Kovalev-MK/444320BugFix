using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.HidingVersionsFunction;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class HidingVersionsFunctionFunctions
  {

    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result =  base.Execute(approvalTask);
      var addendums = approvalTask.AddendaGroup.OfficialDocuments.ToList();
      
      if (!addendums.Any())
        return this.GetErrorResult("Отстутствуют документы в группе Приложения");
      
      if (!HideVersionsDocuments(addendums))
      {
        Logger.Debug("Avis - HidingVersionsFunction: Не все версии документов были скрыты.");
        result = this.GetRetryResult(string.Empty);
      }
      return result;
    }
    
    /// <summary>
    /// Скрыть все версии кроме последней у документов
    /// </summary>
    /// <param name="documents">Список документов</param>
    /// <returns>true - все документы успешно обработанны, false - если хотябы одна версия кроме последней не скрыта</returns>
    [Public]
    public static bool HideVersionsDocuments(List<Sungero.Docflow.IOfficialDocument> documents)
    {
      var isAllSuccess = true;
      foreach (var document in documents)
      {
        try
        {
          var locksDocument = Locks.GetLockInfo(document);
          if (locksDocument.IsLocked)
            throw AppliedCodeException.Create(string.Format("Документ с ИД={0} заблокирован пользователем {1}", document.Id, locksDocument.OwnerName));
          foreach (var version in document.Versions)
          {
            var locksVersion = Locks.GetLockInfo(version);
            if (locksVersion.IsLocked)
              throw AppliedCodeException.Create(string.Format("Версия с ИД={0} документа с ИД={1} заблокирована пользователем {2}", document.Id, version.Id, locksVersion.OwnerName));
            if (!version.Equals(document.LastVersion))
            {
              var canHide = !Sungero.Docflow.PublicFunctions.OfficialDocument.CanHideVersion(document, version.Number); //Возвращает false, если скрытие возможно
              if (!canHide)
                throw AppliedCodeException.Create("Версию с ИД={0} документа с ИД={1} скрыть не возможно,т.к. она учавствует в задаче на ознакомление");
              version.IsHidden = true;
            }
          }
          document.Save();
        }
        catch (AppliedCodeException ex)
        {
          Logger.DebugFormat("Avis - HideVersionsDocuments: {0}", ex.Message);
          isAllSuccess = false;
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Avis - HideVersionsDocuments: Ошибка обработки документа с ИД={0}: {1}", document.Id, ex.Message);
          isAllSuccess = false;
        }
      }
      return isAllSuccess;
    }
  }
}