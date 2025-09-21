using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.FillingDocumentKindsOfRealEstateSale;

namespace lenspec.SalesDepartmentArchive.Server
{
  partial class FillingDocumentKindsOfRealEstateSaleFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = base.Execute(approvalTask);
      
      try
      {
        var document = approvalTask.DocumentGroup.OfficialDocuments.FirstOrDefault();
        if (document != null && SalesDepartmentArchive.SDARequestSubmissionToArchives.Is(document))
        {
          var listOfDocument = SalesDepartmentArchive.SDARequestSubmissionToArchives.As(document).ListOfDocument;
          
          var clientContractDocumentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.SDAClientContractKind);
          if (clientContractDocumentKind != null)
          {
            Transactions.Execute(
              () =>
              {
                foreach(var item in listOfDocument)
                {
                  if (item.RealEstateDocumentKind.DocumentKind != null && item.RealEstateDocumentKind.DocumentKind.Equals(clientContractDocumentKind) &&
                      (item.ClientContract.RealEstateDocumentKind == null || !item.ClientContract.RealEstateDocumentKind.Equals(item.RealEstateDocumentKind)))
                  {
                    var lockInfo = Locks.GetLockInfo(item.ClientContract);
                    if (lockInfo != null && lockInfo.IsLocked)
                    {
                      result = this.GetRetryResult(string.Format("Карточка Клиентского договора ИД = {0} заблокирована пользователем {1}.", item.ClientContract.Id, lockInfo.OwnerName));
                      break;
                    }
                    item.ClientContract.RealEstateDocumentKind = item.RealEstateDocumentKind;
                    item.ClientContract.Save();
                  }
                }
                result = this.GetSuccessResult();
              });
          }
        }
      }
      catch(Exception ex)
      {
        result = this.GetErrorResult(ex.Message);
        Logger.Error("Avis - FillingDocumentKindsOfRealEstateSale - ", ex);
      }
      return result;
    }
    //конец Добавлено Avis Expert

  }
}