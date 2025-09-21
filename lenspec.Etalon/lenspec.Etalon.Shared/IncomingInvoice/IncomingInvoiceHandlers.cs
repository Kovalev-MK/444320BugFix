using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingInvoice;

namespace lenspec.Etalon
{
  partial class IncomingInvoiceSharedHandlers
  {

    public override void MediumChanged(Sungero.Docflow.Shared.OfficialDocumentMediumChangedEventArgs e)
    {
      base.MediumChanged(e);
    }

    public virtual void SetOfDocumentslenspecChanged(lenspec.Etalon.Shared.IncomingInvoiceSetOfDocumentslenspecChangedEventArgs e)
    {
      
    }
    
    public virtual void SetLenspeclenspecChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      
    }

  }
}