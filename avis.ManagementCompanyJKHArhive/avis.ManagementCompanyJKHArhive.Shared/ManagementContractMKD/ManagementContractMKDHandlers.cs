using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ManagementCompanyJKHArhive.ManagementContractMKD;

namespace avis.ManagementCompanyJKHArhive
{
  partial class ManagementContractMKDSharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void BasisDocumentChanged(avis.ManagementCompanyJKHArhive.Shared.ManagementContractMKDBasisDocumentChangedEventArgs e)
    {
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.BasisRelationName, e.OldValue, e.NewValue);
      if (e.NewValue != null)
      {
        _obj.BusinessUnit = e.NewValue.BusinessUnit;
        _obj.Client = e.NewValue.Client;
        _obj.ObjectAnSale = e.NewValue.ObjectAnSale;
        _obj.OurCF = e.NewValue.OurCF;
        _obj.Address = e.NewValue.Address;
      }
    }
    //конец Добавлено Avis Expert

  }
}