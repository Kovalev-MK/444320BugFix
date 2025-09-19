using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OrderBase;

namespace lenspec.Etalon
{
  partial class OrderBaseAddresseeslenspecClientHandlers
  {

    public virtual void AddresseeslenspecNumberValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue < 1)
        e.AddError(Sungero.Docflow.OfficialDocuments.Resources.NumberAddresseeListIsNotPositive);
    }
  }

  partial class OrderBaseClientHandlers
  {

    //Добавлено Avis Expert
    public override void AssigneeValueInput(Sungero.Docflow.Client.OfficialDocumentAssigneeValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.AssigneeValueInput(e);
    }

    public override void PreparedByValueInput(Sungero.Docflow.Client.OfficialDocumentPreparedByValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.PreparedByValueInput(e);
    }
    //конец Добавлено Avis Expert

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      if (!Users.Current.IncludedIn(Roles.Administrators))
      {
        _obj.State.Properties.LifeCycleState.IsEnabled = false;
        _obj.State.Properties.RegistrationState.IsEnabled = false;
        _obj.State.Properties.VerificationState.IsEnabled = false;
        _obj.State.Properties.InternalApprovalState.IsEnabled = false;
        _obj.State.Properties.ExternalApprovalState.IsEnabled = false;
        _obj.State.Properties.ExchangeState.IsEnabled = false;
        _obj.State.Properties.ExecutionState.IsEnabled = false;
        _obj.State.Properties.ControlExecutionState.IsEnabled = false;
      }
      if (_obj.FillEmpDeplenspec == true)
      {
        _obj.State.Properties.Departmenslenspec.IsEnabled = true;
        _obj.State.Properties.Departmenslenspec.IsRequired = true;
        _obj.State.Properties.FillOptlenspec.IsEnabled = true;
        _obj.State.Properties.FillOptlenspec.IsRequired = true;
      }
      else
      {
        _obj.State.Properties.Departmenslenspec.IsEnabled = false;
        _obj.State.Properties.Departmenslenspec.IsRequired = false;
        _obj.State.Properties.FillOptlenspec.IsEnabled = false;
        _obj.State.Properties.FillOptlenspec.IsRequired = false;
      }
      
      #region Доработка отменена ТЗ "ЛНА. Вернуть возможность изменения карточки, если полные права"
      /*
      var addresseesIsEnabled = false;
      // Делопроизводители всегда могут редактировать список Адресатов
      if (Users.Current.IncludedIn(Sungero.Docflow.Constants.Module.RoleGuid.ClerksRole))
      {
        addresseesIsEnabled = true;
      }
      else
      {
        addresseesIsEnabled = Users.Current.Equals(_obj.Author);
      }
      _obj.State.Properties.Addresseeslenspec.IsEnabled = addresseesIsEnabled;
      var addresseesProperties = _obj.State.Properties.Addresseeslenspec.Properties;
      foreach(var property in addresseesProperties)
      {
        property.IsEnabled = addresseesIsEnabled;
      }
      */
      #endregion
    }

  }
}