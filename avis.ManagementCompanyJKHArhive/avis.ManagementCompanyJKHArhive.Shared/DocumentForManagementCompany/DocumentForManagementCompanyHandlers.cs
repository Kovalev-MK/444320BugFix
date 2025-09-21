using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ManagementCompanyJKHArhive.DocumentForManagementCompany;

namespace avis.ManagementCompanyJKHArhive
{
  partial class DocumentForManagementCompanySharedHandlers
  {

    // Добавлено avis.
    public override void ObjectAnSaleChanged(avis.ManagementCompanyJKHArhive.Shared.ArhiveJKHDocumentBaseObjectAnSaleChangedEventArgs e)
    {
      base.ObjectAnSaleChanged(e);
      if (e.NewValue != null)
      {
        _obj.Address = e.NewValue.Address;
      }
    }

    public virtual void MKDChanged(avis.ManagementCompanyJKHArhive.Shared.DocumentForManagementCompanyMKDChangedEventArgs e)
    {
      // Создамём связь.
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.Constants.Module.AddendumRelationName, e.OldValue, e.NewValue);
      
      if(e.NewValue == null)
        return;
      
      // Если изменился МКД. То заполняем карточку документа УК.
      if (e.OldValue != e.NewValue)
      {
        _obj.Client = e.NewValue.Client;
        _obj.BusinessUnit = e.NewValue.BusinessUnit;
        _obj.ObjectAnSale = e.NewValue.ObjectAnSale;
        _obj.OurCF = e.NewValue.OurCF;
      }
    }
    
    // Конец добавлено avis.
  }
}