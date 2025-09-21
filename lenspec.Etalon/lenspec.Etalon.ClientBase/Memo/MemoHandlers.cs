using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Memo;

namespace lenspec.Etalon
{
  partial class MemoAddresseesClientHandlers
  {

    public override void AddresseesAddresseeValueInput(Sungero.Docflow.Client.MemoAddresseesAddresseeValueInputEventArgs e)
    {
      // Проверка на автоматизированность сотрудника.
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      
      base.AddresseesAddresseeValueInput(e);
    }
  }


  partial class MemoClientHandlers
  {

    public override void OurSignatoryValueInput(Sungero.Docflow.Client.OfficialDocumentOurSignatoryValueInputEventArgs e)
    {
      // Проверка на автоматизированность сотрудника.
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.OurSignatoryValueInput(e);
    }
    
    /// <summary>
    /// Изменение значения контрола "Адресат".
    /// </summary>
    /// <param name="e"></param>
    public override void AddresseeValueInput(Sungero.Docflow.Client.MemoAddresseeValueInputEventArgs e)
    {
      // Проверка на автоматизированность сотрудника.
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      
      base.AddresseeValueInput(e);
    }

    //Добавлено Avis Expert
    public override void PreparedByValueInput(Sungero.Docflow.Client.OfficialDocumentPreparedByValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);

      base.PreparedByValueInput(e);
    }
    //конец Добавлено Avis Expert

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      e.HideAction(_obj.Info.Actions.SignVersion);
      
      // Проверка вхождения в роль "Пользователи с правами на указание в документах сотрудников из любых НОР".
      var roleSid = lenspec.EtalonDatabooks.PublicConstants.Module.RightsToSelectAnyEmployees;
      var hasRightsToSelectAnyEmployees = Users.Current.IncludedIn(roleSid);
      e.Params.AddOrUpdate(lenspec.Etalon.Constants.Docflow.Memo.Params.HasRightsToSelectAnyEmployees, hasRightsToSelectAnyEmployees);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      if (!Users.Current.IncludedIn(Roles.Administrators))
      { _obj.State.Properties.LifeCycleState.IsEnabled = false;
        _obj.State.Properties.RegistrationState.IsEnabled = false;
        _obj.State.Properties.VerificationState.IsEnabled = false;
        _obj.State.Properties.InternalApprovalState.IsEnabled = false;
        _obj.State.Properties.ExternalApprovalState.IsEnabled = false;
        _obj.State.Properties.ExchangeState.IsEnabled = false;
        _obj.State.Properties.ExecutionState.IsEnabled = false;
        _obj.State.Properties.ControlExecutionState.IsEnabled = false;}
      
      // Если зарегистрировали, то заблокировать Рег номер и Дату документа навсегда.
      if (_obj.RegistrationNumber != null)
      {
        _obj.State.Properties.RegistrationNumber.IsEnabled = false;
        _obj.State.Properties.RegistrationDate.IsEnabled = false;
      }
      
      var addresseesIsEnabled = false;
      // Делопроизводители всегда могут редактировать список Адресатов.
      if (Users.Current.IncludedIn(Sungero.Docflow.Constants.Module.RoleGuid.ClerksRole))
        addresseesIsEnabled = true;
      else
        addresseesIsEnabled = Users.Current.Equals(_obj.Author);
      
      _obj.State.Properties.Addressees.IsEnabled = addresseesIsEnabled;
      var addresseesProperties = _obj.State.Properties.Addressees.Properties;
      foreach(var property in addresseesProperties)
      {
        property.IsEnabled = addresseesIsEnabled;
      }
    }

  }
}