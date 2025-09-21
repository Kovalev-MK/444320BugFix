using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientContract;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAClientContractSharedHandlers
  {

    public override void CounterpartyClientChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.CounterpartyClientChanged(e);
      if (_obj.CounterpartyClient != null)
        _obj.ClientNames = string.Join("; ", _obj.CounterpartyClient.Where(c => c.ClientItem != null).Select(c => c.ClientItem.Name));
    }

    //Добавлено Avis Expert
    public virtual void PremisesChanged(lenspec.SalesDepartmentArchive.Shared.SDAClientContractPremisesChangedEventArgs e)
    {
      if (e.NewValue != null && e.NewValue.ObjectAnProject != null)
      {
        _obj.ObjectAnProject = e.NewValue.ObjectAnProject;
      }
      else
      {
        _obj.ObjectAnProject = null;
      }
    }

    public override void ClientDocumentDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.ClientDocumentDateChanged(e);
      FillName();
    }

    public override void ClientDocumentNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.ClientDocumentNumberChanged(e);
      FillName();
    }

    public override void RealEstateDocumentKindChanged(lenspec.SalesDepartmentArchive.Shared.SDAClientDocumentBaseRealEstateDocumentKindChangedEventArgs e)
    {
      base.RealEstateDocumentKindChanged(e);
      FillName();
    }
    
    //конец Добавлено Avis Expert
  }
}