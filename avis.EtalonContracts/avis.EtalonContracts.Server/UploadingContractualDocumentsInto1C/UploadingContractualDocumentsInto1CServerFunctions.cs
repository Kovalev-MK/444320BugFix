using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonContracts.UploadingContractualDocumentsInto1C;

namespace avis.EtalonContracts.Server
{
  partial class UploadingContractualDocumentsInto1CFunctions
  {
    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = base.Execute(approvalTask);
      bool contractUloaded    = true;
      bool contractOsUloaded  = true;
      var document = approvalTask.DocumentGroup.OfficialDocuments.SingleOrDefault();
      var contract = lenspec.Etalon.ContractualDocuments.Get(document.Id);
      contractUloaded = lenspec.Etalon.PublicFunctions.ContractualDocument.UnloadingContracts(contract);
      if (contractUloaded)
      {
        //Если есть ОС то и их выгрузим
        if (contract.ConstructionObjectsavis.Count() > 0)
        {
          contractOsUloaded = lenspec.Etalon.PublicFunctions.ContractualDocument.UnloadingContractsDetails(contract);
        }
      }
      if (contractUloaded && contractOsUloaded)
      {
        lenspec.Etalon.PublicFunctions.ContractualDocument.UpdateContractSyns(contract);
      }
      return result;
    }

  }
}