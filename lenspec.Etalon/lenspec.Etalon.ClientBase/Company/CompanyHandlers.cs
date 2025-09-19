using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Company;

namespace lenspec.Etalon
{
  // Добавлено avis
  
  partial class CompanyClientHandlers
  {

    public override void ResultApprovalDEBavisValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      base.ResultApprovalDEBavisValueInput(e);
      _obj.State.Properties.ApprovalPeriodavis.IsEnabled = e.NewValue != lenspec.Etalon.Company.ResultApprovalDEBavis.DoesNotReqAppr;
    }
    
    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      // Обязательность полей
      PublicFunctions.Company.ChangeRequiredProp(_obj);
      PublicFunctions.Company.IsChangeGroupCounterparty(_obj);
      
      base.Refresh(e);
      
      //!!!Внимание на порядок выдачи доступности!!!
      
      var isAdmin = false;
      var isClerk = false;
      var isCreateCounterpartyRole = false;
      var isResponsibleForCounterpartyQualification = false;
      var isResponsibleForMonopolists = false;
      
      e.Params.TryGetValue(Constants.EtalonIntergation.Company.Params.isAdmin, out isAdmin);
      e.Params.TryGetValue(Constants.EtalonIntergation.Company.Params.isClerk, out isClerk);
      e.Params.TryGetValue(Constants.EtalonIntergation.Company.Params.isCreateCounterpartyRole, out isCreateCounterpartyRole);
      e.Params.TryGetValue(Constants.EtalonIntergation.Company.Params.isResponsibleForCounterpartyQualification, out isResponsibleForCounterpartyQualification);
      e.Params.TryGetValue(Constants.EtalonIntergation.Company.Params.isResponsibleForMonopolists, out isResponsibleForMonopolists);
      
      // Запрещаем изменение свойств всем, кроме ролей
      // "Администраторы" и "Права на создание всех видов КА".
      if (!isAdmin && !isCreateCounterpartyRole)
        foreach (var prop in _obj.State.Properties)
          prop.IsEnabled = false;
      
      // Доступность вкладки "Тендеры" для ролей
      // "Администраторы", "Права на создание всех видов КА", "Ответственные за квалификацию и дисквалификацию контрагентов".
      var tenderTabEnabled = isAdmin || isCreateCounterpartyRole || isResponsibleForCounterpartyQualification;
      _obj.State.Properties.IsProvideravis.IsEnabled = tenderTabEnabled;
      _obj.State.Properties.IsContractoravis.IsEnabled = tenderTabEnabled;
      
      // Доступность свойства "Эл. почта" для роли "Делопроизводители".
      _obj.State.Properties.Email.IsEnabled = isClerk;
      
      // Доступность свойства "Монополист" для роли "Ответственные за признак монополистов КА".
      _obj.State.Properties.IsMonopolistlenspec.IsEnabled = isResponsibleForMonopolists;
    }
    
    /// <summary>
    /// Показ формы.
    /// </summary>
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      // Делаем поля группы и категории контрагентов обязательными.
      _obj.State.Properties.GroupCounterpartyavis.IsRequired = true;
      _obj.State.Properties.CategoryCounterpartyavis.IsRequired = true;
      
      var isAdmin = Users.Current.IncludedIn(Roles.Administrators);
      var isClerk = Users.Current.IncludedIn(Sungero.Docflow.PublicConstants.Module.RoleGuid.ClerksRole);
      var isCreateCounterpartyRole = Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid);
      var isResponsibleForCounterpartyQualification = Users.Current.IncludedIn(lenspec.Tenders.PublicConstants.Module.ResponsibleForCounterpartyQualificationRole);
      var isResponsibleForMonopolists = Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleResponsibleForMonopolistsGuid);
      
      e.Params.AddOrUpdate(Constants.EtalonIntergation.Company.Params.isAdmin, isAdmin);
      e.Params.AddOrUpdate(Constants.EtalonIntergation.Company.Params.isClerk, isClerk);
      e.Params.AddOrUpdate(Constants.EtalonIntergation.Company.Params.isCreateCounterpartyRole, isCreateCounterpartyRole);
      e.Params.AddOrUpdate(Constants.EtalonIntergation.Company.Params.isResponsibleForCounterpartyQualification, isResponsibleForCounterpartyQualification);
      e.Params.AddOrUpdate(Constants.EtalonIntergation.Company.Params.isResponsibleForMonopolists, isResponsibleForMonopolists);
    }
  }
  
  // Конец добавлено avis
}