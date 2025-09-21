using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentFlowTask;

namespace lenspec.Etalon.Client
{
  partial class DocumentFlowTaskFunctions
  {

    /// <summary>
    /// Проверить возможность старта задачи.
    /// </summary>
    /// <param name="eventArgs">Аргумент обработчика вызова.</param>
    /// <returns>True - разрешить старт задачи, иначе false.</returns>
    public override bool ValidateDocumentFlowTaskStart(Sungero.Domain.Client.ExecuteActionArgs eventArgs)
    {
      var isValid   = base.ValidateDocumentFlowTaskStart(eventArgs);
      var document  = _obj.DocumentGroup.ElectronicDocuments.SingleOrDefault();
      if (Sungero.FinancialArchive.UniversalTransferDocuments.Is(document) || Sungero.FinancialArchive.ContractStatements.Is(document) ||
          Sungero.FinancialArchive.Waybills.Is(document) || Sungero.FinancialArchive.IncomingTaxInvoices.Is(document))
      {
        if(lenspec.Etalon.AccountingDocumentBases.As(document).ResponsibleEmployee == null)
        {
          eventArgs.AddError(lenspec.Etalon.DocumentFlowTasks.Resources.ErrorRespBeforeStart);
          isValid = false;
        }
      }
      return isValid;
    }
  }
}