using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.ResponsibleByCounterparty;

namespace avis.EtalonParties
{
  partial class ResponsibleByCounterpartyClientHandlers
  {

    //Добавлено Avis Expert
    public virtual void ResponsibleValueInput(avis.EtalonParties.Client.ResponsibleByCounterpartyResponsibleValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
        e.AddError(avis.EtalonParties.ResponsibleByCounterparties.Resources.EmployeeValidAccountErrorMessage);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      if (_obj.BusinessUnit != null)
      {
        var allPropertiesIsEnabled = false;
        var substitutedBUIds = Functions.ResponsibleByCounterparty.Remote.GetSubstitutedBusinessUnitIds(_obj);
        allPropertiesIsEnabled = substitutedBUIds.Contains(_obj.BusinessUnit.Id);
        
        foreach(var property in _obj.State.Properties)
        {
          property.IsEnabled = allPropertiesIsEnabled;
        }
        
        _obj.State.Properties.Name.IsEnabled = false;
        _obj.State.Properties.CreatedAt.IsEnabled = false;
      }
    }
    //конец Добавлено Avis Expert

  }
}