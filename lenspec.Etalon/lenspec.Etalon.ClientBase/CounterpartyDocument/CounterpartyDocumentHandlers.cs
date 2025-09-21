using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.CounterpartyDocument;

namespace lenspec.Etalon
{
  partial class CounterpartyDocumentClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Functions.CounterpartyDocument.UpdateFields(_obj, _obj.DocumentKind);
      
      if (!e.Params.Contains(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam))
      {
        var documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Etalon.Module.Parties.Constants.Module.ExtractFromEGRULKind);
        e.Params.AddOrUpdate(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, documentKind == _obj.DocumentKind);
      }
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      Functions.CounterpartyDocument.UpdateFields(_obj, _obj.DocumentKind);
      
      var instruction = Functions.CounterpartyDocument.Remote.GetInstruction(_obj.DocumentKind);
      if (!string.IsNullOrEmpty(instruction))
        e.AddInformation(_obj.Info.Properties.DocumentKind, instruction);
      
      var documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Etalon.Module.Parties.Constants.Module.ExtractFromEGRULKind);
      e.Params.AddOrUpdate(Etalon.Constants.Docflow.CounterpartyDocument.IsExtractFromEGRULKindParam, documentKind == _obj.DocumentKind);
    }

    public override void DocumentKindValueInput(Sungero.Docflow.Client.OfficialDocumentDocumentKindValueInputEventArgs e)
    {
      base.DocumentKindValueInput(e);
      
      var instruction = Functions.CounterpartyDocument.Remote.GetInstruction(e.NewValue);
      if (!string.IsNullOrEmpty(instruction))
        e.AddInformation(e.Property, instruction);
    }
  }

}