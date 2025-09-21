using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.CollegialBody;

namespace lenspec.ProtocolsCollegialBodies
{

  partial class CollegialBodyClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      _obj.State.Properties.IsAutomaticApproval.IsEnabled = false;
      // Если входим в роль "Полные права на справочник «Коллегиальные органы»" или «Администраторы», делаем доступной кнопку Автоматическое согласование по истечении срока
      if (Sungero.Company.Employees.Current.IncludedIn(ProtocolsCollegialBodies.Constants.Module.ClerksGKResponsible) || Sungero.Company.Employees.Current.IncludedIn(Roles.Administrators))
        _obj.State.Properties.IsAutomaticApproval.IsEnabled = true;
    }

    //Добавлено Avis Expert
    public virtual void ChairmanValueInput(lenspec.ProtocolsCollegialBodies.Client.CollegialBodyChairmanValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
    }
    //конец Добавлено Avis Expert

    public virtual void IndexValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.NewValue))
        e.NewValue = e.NewValue.Trim();
    }

    public virtual void NameValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.NewValue))
        e.NewValue = e.NewValue.Trim();
    }

  }
}