using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.AccountingDocumentBase;

namespace lenspec.Etalon
{
  partial class AccountingDocumentBaseSharedHandlers
  {

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (e.NewValue == null)
      {
        _obj.OurSignatory         = null;
        _obj.ResponsibleEmployee  = null;
      }    
      else
      {
        var contract              = lenspec.Etalon.ContractualDocuments.As(e.NewValue);
        _obj.OurSignatory         = contract.OurSignatory;
        _obj.ResponsibleEmployee  = contract.ResponsibleEmployee;
      }

    }

  }
}