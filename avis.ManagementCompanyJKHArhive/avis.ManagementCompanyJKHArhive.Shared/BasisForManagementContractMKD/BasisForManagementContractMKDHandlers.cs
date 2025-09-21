using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ManagementCompanyJKHArhive.BasisForManagementContractMKD;

namespace avis.ManagementCompanyJKHArhive
{
  partial class BasisForManagementContractMKDSharedHandlers
  {

    //Добавлено Avis Expert
    public override void ObjectAnSaleChanged(avis.ManagementCompanyJKHArhive.Shared.ArhiveJKHDocumentBaseObjectAnSaleChangedEventArgs e)
    {
      base.ObjectAnSaleChanged(e);
      if (e.NewValue != null)
      {
        _obj.Address = e.NewValue.Address;
      }
    }

    public virtual void MKDChanged(avis.ManagementCompanyJKHArhive.Shared.BasisForManagementContractMKDMKDChangedEventArgs e)
    {
      // Создамём связь.
      _obj.Relations.AddFromOrUpdate(Constants.Module.DocumentBasisToManagementAgreementMKDRelationName, e.OldValue, e.NewValue);
      
      if(e.NewValue == null)
        return;
      
      // Если изменился МКД. То заполняем карточку документа УК.
      if (e.OldValue != e.NewValue)
      {
        _obj.Client = e.NewValue.Client;
        _obj.BusinessUnit = e.NewValue.BusinessUnit;
        _obj.ObjectAnSale = e.NewValue.ObjectAnSale;
      }
    }
    //конец Добавлено Avis Expert

  }
}