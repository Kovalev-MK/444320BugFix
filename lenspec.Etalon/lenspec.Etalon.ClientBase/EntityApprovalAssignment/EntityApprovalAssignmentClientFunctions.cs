using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.EntityApprovalAssignment;

namespace lenspec.Etalon.Client
{
  partial class EntityApprovalAssignmentFunctions
  {

    /// <summary>
    /// Валидация задания перед согласованием.
    /// </summary>
    /// <param name="eventArgs">Аргументы действия.</param>
    /// <returns>True - если ошибок нет, иначе - False.</returns>
    public override bool ValidateBeforeApproval(Sungero.Domain.Client.ExecuteActionArgs eventArgs)
    {
      var isValid = base.ValidateBeforeApproval(eventArgs);
      var document = _obj.DocumentGroup.ElectronicDocuments.SingleOrDefault();
      if (Sungero.FinancialArchive.UniversalTransferDocuments.Is(document) || Sungero.FinancialArchive.ContractStatements.Is(document) ||
          Sungero.FinancialArchive.Waybills.Is(document))
      {
        if(lenspec.Etalon.AccountingDocumentBases.As(document).OurSignatory == null)
        {
          eventArgs.AddError(lenspec.Etalon.EntityApprovalAssignments.Resources.ErrorOurSigBeforeComplete);
          isValid = false;
        }
      }
      return isValid;
    }
  }
}