using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.AccountingDocumentBase;

namespace lenspec.Etalon.Shared
{
  partial class AccountingDocumentBaseFunctions
  {
    public override bool CheckVatAmount(Nullable<double> vatAmount)
    {
      if (!_obj.TotalAmount.HasValue || _obj.VatRate == null)
        return true;
      
      if (_obj.VatRate != null && vatAmount == null)
        return false;
      
      if (_obj.VatRate.Sid == lenspec.Etalon.Module.Docflow.PublicConstants.Module.VatRateMixedPercentSid)
        return true;
      
      var expectedVatAmount = Sungero.Commons.PublicFunctions.Module.GetVatAmountFromTotal(_obj.TotalAmount.Value, _obj.VatRate);
      return vatAmount.Value == expectedVatAmount;
    }
  }
}