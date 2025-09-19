using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientDocument;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAClientDocumentSharedHandlers
  {

    //Добавлено Avis Expert
    public override void RealEstateDocumentKindChanged(lenspec.SalesDepartmentArchive.Shared.SDAClientDocumentBaseRealEstateDocumentKindChangedEventArgs e)
    {
      base.RealEstateDocumentKindChanged(e);
      FillName();
    }

    public virtual void ClientContractChanged(lenspec.SalesDepartmentArchive.Shared.SDAClientDocumentClientContractChangedEventArgs e)
    {
      FillName();
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.Constants.Module.AddendumRelationName, e.OldValue, e.NewValue);
      
      if (e.NewValue != null)
      {
        _obj.BusinessUnit = e.NewValue.BusinessUnit;
        _obj.CounterpartyClient.Clear();
        foreach(var counterparty in e.NewValue.CounterpartyClient)
        {
          _obj.CounterpartyClient.AddNew().ClientItem = counterparty.ClientItem;
        }
      }
    }
    //конец Добавлено Avis Expert

  }
}