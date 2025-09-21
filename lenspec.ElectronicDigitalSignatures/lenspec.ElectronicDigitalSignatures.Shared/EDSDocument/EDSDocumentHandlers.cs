using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.EDSDocument;

namespace lenspec.ElectronicDigitalSignatures
{
  partial class EDSDocumentSharedHandlers
  {

    public virtual void PersonChanged(lenspec.ElectronicDigitalSignatures.Shared.EDSDocumentPersonChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (!string.IsNullOrEmpty(e.NewValue.INILA))
          _obj.INILA = e.NewValue.INILA;
        
        if (!string.IsNullOrEmpty(e.NewValue.TIN))
          _obj.TIN = e.NewValue.TIN;
      }
      
      FillName();
    }

  }
}