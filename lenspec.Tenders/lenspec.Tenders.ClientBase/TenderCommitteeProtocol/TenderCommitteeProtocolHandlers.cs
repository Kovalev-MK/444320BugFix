using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderCommitteeProtocol;

namespace lenspec.Tenders
{
  partial class TenderCommitteeProtocolAddresseesClientHandlers
  {

    public virtual void AddresseesNumberValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      
    }
  }

  partial class TenderCommitteeProtocolClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var qualificationSelectionProtocolKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.QualificationSelectionProtocolKind);
      
      var isQualificationSelectionProtocol = _obj.BasisDocument?.DocumentKind == qualificationSelectionProtocolKind;
      
      _obj.State.Properties.DateQualificationSelection.IsVisible = isQualificationSelectionProtocol;
      _obj.State.Properties.TenderWinner.IsRequired = true;
      _obj.State.Properties.CostFinalOffer.IsRequired = true;
      _obj.State.Properties.TenderWinnerReserve.IsEnabled = _obj.TenderWinner != null;
      
      var basisDocumentIsChanged = false;
      e.Params.TryGetValue(lenspec.Tenders.TenderCommitteeProtocols.Resources.BasisDocumentIsChanged, out basisDocumentIsChanged);
      
      if (basisDocumentIsChanged)
      {
        DateTime? dateQualificationSelection = isQualificationSelectionProtocol ?
          _obj.BasisDocument?.DateQualificationSelection :
          null;
        
        if (!Equals(_obj.DateQualificationSelection, dateQualificationSelection))
          _obj.DateQualificationSelection = dateQualificationSelection;
      }
    }
    
  }
}