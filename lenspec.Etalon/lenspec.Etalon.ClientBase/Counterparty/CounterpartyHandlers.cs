using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Counterparty;

namespace lenspec.Etalon
{
  partial class CounterpartyClientHandlers
  {

    public virtual void ResponsibleDEBavisValueInput(lenspec.Etalon.Client.CounterpartyResponsibleDEBavisValueInputEventArgs e)
    {
      if (e.NewValue != null && !EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
    }

    public virtual void ApprovalPeriodavisValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (_obj.InspectionDateDEBavis == null || e.NewValue == null)
        return;
      
      if (_obj.InspectionDateDEBavis > e.NewValue)
      {
        e.AddError(lenspec.Etalon.Counterparties.Resources.DBInspectionDateMustBeEarlierThanApprovement);
        _obj.ApprovalPeriodavis = null;
      }
    }

    public virtual void InspectionDateDEBavisValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (_obj.ApprovalPeriodavis == null || e.NewValue == null)
        return;
      
      if (_obj.ApprovalPeriodavis < e.NewValue)
      {
        e.AddError(lenspec.Etalon.Counterparties.Resources.DBInspectionDateMustBeEarlierThanApprovement);
        _obj.InspectionDateDEBavis = null;
      }
    }

    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.ResultApprovalDEBavis.IsRequired = true;
      
      var isAdmin = Users.Current.IncludedIn(Roles.Administrators);
      _obj.State.Properties.ResultApprovalDEBavis.IsEnabled = isAdmin;
      _obj.State.Properties.InspectionDateDEBavis.IsEnabled = isAdmin;
      _obj.State.Properties.ApprovalPeriodavis.IsEnabled = isAdmin && _obj.ResultApprovalDEBavis != lenspec.Etalon.Counterparty.ResultApprovalDEBavis.DoesNotReqAppr;
      _obj.State.Properties.ResponsibleDEBavis.IsEnabled = isAdmin;
      _obj.State.Properties.LimitPerCounterpartyavis.IsEnabled = isAdmin;
      
      //var roleCounterParti = Roles.GetAll(r => r.Sid == avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid).FirstOrDefault();
      _obj.State.Properties.SalesAgentlenspec.IsEnabled = isAdmin || Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid);
    }
  }
}