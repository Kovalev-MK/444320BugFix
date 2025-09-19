using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.OutgoingLetters.MassDocumentRegistration;

namespace lenspec.OutgoingLetters.Server
{
  partial class MassDocumentRegistrationFunctions
  {

    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      Logger.DebugFormat("Avis - MassDocumentRegistration - массовая регистрация документов, задача {0}.", approvalTask.Id);
      var result = base.Execute(approvalTask);
      
      var document = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      if (document == null)
        return this.GetErrorResult("Не найден документ.");

      try
      {
        var documentsToRegister = new List<Sungero.Docflow.IOfficialDocument>();
        
        // Регистрация приложений
        if (lenspec.OutgoingLetters.MassMailingApplications.Is(document) || Sungero.Docflow.PowerOfAttorneys.Is(document))
        {
          //Исходящие письма
          documentsToRegister.AddRange(approvalTask.AddendaGroup.OfficialDocuments
                                       .Where(x => Sungero.RecordManagement.OutgoingLetters.Is(x) && Sungero.RecordManagement.OutgoingLetters.As(x).RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered)
                                       .ToList());
          //Доверенности
          documentsToRegister.AddRange(approvalTask.AddendaGroup.OfficialDocuments
                                       .Where(x => Sungero.Docflow.PowerOfAttorneys.Is(x) && Sungero.Docflow.PowerOfAttorneys.As(x).RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered)
                                       .ToList());
        }
        
        if (!documentsToRegister.Any())
          return result;
        
        var errors = PublicFunctions.Module.RegisterListOfDocumentsBySystemUser(documentsToRegister);
        if (!string.IsNullOrEmpty(errors))
        {
          Logger.ErrorFormat("Avis - MassDocumentRegistration - ошибка массовой регистрации документов, задача {0}: {1}", approvalTask.Id, errors);
          result = this.GetRetryResult(string.Empty);
        }
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Avis - MassDocumentRegistration - ошибка массовой регистрации документов, задача {0}: ", ex, approvalTask.Id);
        result = this.GetRetryResult(string.Empty);
      }
      return result;
    }
  }
}