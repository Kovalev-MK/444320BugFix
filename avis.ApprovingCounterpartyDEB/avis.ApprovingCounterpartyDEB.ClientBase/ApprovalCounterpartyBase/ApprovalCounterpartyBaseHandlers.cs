using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBase;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalCounterpartyBaseClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    { 
      base.Showing(e);   
      if (_obj.Counterparty != null)
      {      
        var lastTask = Functions.ApprovalCounterpartyBase.Remote.GetLastTask(_obj);
        if (lastTask == null)
          e.HideAction(_obj.Info.Actions.GetApprovalList);        
      }
    }
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Properties.Subject.IsRequired = false;
      _obj.State.Properties.Department.IsEnabled = false;
      _obj.State.Properties.Cities.IsEnabled = _obj.PresenceRegion.Any(x => x.Region != null);
    }

    public virtual void CounterpartyValueInput(avis.ApprovingCounterpartyDEB.Client.ApprovalCounterpartyBaseCounterpartyValueInputEventArgs e)
    {
      if (e.NewValue != null && e.NewValue.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.DoesNotReqAppr)
        e.AddWarning(avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBases.Resources.DoesNotRequirApprovalMessage);
    }
  }
}