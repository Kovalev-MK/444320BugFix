using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Bank;

namespace lenspec.Etalon
{
  partial class BankClientHandlers
  {
    // Добавлено avis.
    
    public override void ResultApprovalDEBavisValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      base.ResultApprovalDEBavisValueInput(e);
      _obj.State.Properties.ApprovalPeriodavis.IsEnabled = _obj.ResultApprovalDEBavis != lenspec.Etalon.Bank.ResultApprovalDEBavis.DoesNotReqAppr;
    }
    
    /// <summary>
    /// Показ формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      // Если входит в админа, не блокируем поля.
      if (Employees.Current.IncludedIn(Roles.Administrators))
      {
        // Разблокируем поле.
        _obj.State.Properties.Email.IsEnabled = true;
        
        return;
      }
      
      // Блокируем все поля кроме емейла для делопроизводителей, только если они не входят в роль "Права на создание всех видов контрагентов"
      var clerkRole = Sungero.Docflow.PublicFunctions.DocumentRegister.Remote.GetClerks();
      if (Employees.Current.IncludedIn(clerkRole) && !Employees.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid))
      {
        // Разблокируем поле.
        _obj.State.Properties.Email.IsEnabled = true;
        
        // Блокируем поля.
        _obj.State.Properties.Name.IsEnabled = false;
        _obj.State.Properties.LegalName.IsEnabled = false;
        _obj.State.Properties.Nonresident.IsEnabled = false;
        _obj.State.Properties.BIC.IsEnabled = false;
        _obj.State.Properties.SWIFT.IsEnabled = false;
        _obj.State.Properties.TIN.IsEnabled = false;
        _obj.State.Properties.Status.IsEnabled = false;
        _obj.State.Properties.CorrespondentAccount.IsEnabled = false;
        _obj.State.Properties.TRRC.IsEnabled = false;
        _obj.State.Properties.Code.IsEnabled = false;
        _obj.State.Properties.City.IsEnabled = false;
        _obj.State.Properties.Region.IsEnabled = false;
        _obj.State.Properties.LegalAddress.IsEnabled = false;
        _obj.State.Properties.PostalAddress.IsEnabled = false;
        _obj.State.Properties.Phones.IsEnabled = false;
        _obj.State.Properties.Homepage.IsEnabled = false;
      }
    }
    
    // Конец добавлено avis.
  }
}