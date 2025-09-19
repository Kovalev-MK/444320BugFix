using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonContracts.AttachmentContractDocument;

namespace avis.EtalonContracts
{
  partial class AttachmentContractDocumentSharedHandlers
  {

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      
      // Установка связи.
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.Constants.Module.AddendumRelationName, e.OldValue, e.NewValue);
      
      if (_obj.LeadingDocument == null)
        return;
      
      // Заполняем поля из ведущего договора.
      _obj.Counterparty = _obj.LeadingDocument.Counterparty;
      _obj.ThirdSideavis = _obj.LeadingDocument.ThirdSideavis;
      _obj.OurSignatory = _obj.LeadingDocument.OurSignatory;
      _obj.BusinessUnit = _obj.LeadingDocument.BusinessUnit;
    }

  }
}