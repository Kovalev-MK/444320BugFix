using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.CounterpartyDocument;

namespace lenspec.Etalon
{
  partial class CounterpartyDocumentSharedHandlers
  {

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      
      var documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Etalon.Module.Parties.Constants.Module.ExtractFromEGRULKind);
      e.Params.AddOrUpdate(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, documentKind == e.NewValue);
      
      if (!Equals(e.NewValue, e.OldValue))
        _obj.Counterparty = null;
    }

    public virtual void CustomDocumentDatelenspecChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      Functions.CounterpartyDocument.FillName(_obj);
    }

  }
}