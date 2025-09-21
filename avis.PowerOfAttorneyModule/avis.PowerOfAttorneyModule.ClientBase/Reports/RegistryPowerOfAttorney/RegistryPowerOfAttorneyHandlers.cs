using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PowerOfAttorneyModule
{
  partial class RegistryPowerOfAttorneyClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      if(!Sungero.Company.Employees.Current.IncludedIn(Roles.GetAll(x => x.Name.Equals(Constants.Module.RoleNameClerks)).FirstOrDefault()))
      {
        Dialogs.ShowMessage(avis.PowerOfAttorneyModule.Reports.Resources.RegistryPowerOfAttorney.ErrorMessageRights, MessageType.Error);
        e.Cancel = true;
        return;
      }
      var dialog = Dialogs.CreateInputDialog(avis.PowerOfAttorneyModule.Reports.Resources.RegistryPowerOfAttorney.DialogTitle);
      var companies = dialog.AddSelectMany("Наши организации-доверители", false, Sungero.Company.BusinessUnits.Null);
      var validFrom = dialog.AddDate("Действует с", false);
      var validTo = dialog.AddDate("Действует по", false);
      //      var responsible = dialog.AddSelectMany("Ответственный инициатор", false, Sungero.Company.Employees.Null);
      //      var attorney = dialog.AddSelectMany("Поверенный", false, Sungero.Parties.Counterparties.Null);
      //      attorney.From(Sungero.Parties.Counterparties.GetAll(x => Sungero.Parties.People.Is(x) || Sungero.Parties.Companies.Is(x)));
      var responsibleDefault = new List<Sungero.Company.IEmployee>();
      var responsible = dialog.AddSelectMany("Ответственный инициатор", false, responsibleDefault.ToArray());
      responsible.IsEnabled = false;
      responsible.IsVisible = false;
      var selectedResponsibleText = dialog.AddMultilineString("Ответственный инициатор", false, string.Empty).WithRowsCount(3);
      selectedResponsibleText.IsEnabled = false;

      #region Ответственный инициатор
      
      var addResponsibleLink = dialog.AddHyperlink("Добавить ответственного иницатора");
      addResponsibleLink.SetOnExecute(
        () =>
        {
          var selectedResponsible = Sungero.Company.Employees.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ShowSelectMany().AsEnumerable();
          if (selectedResponsible != null && selectedResponsible.Any())
          {
            var sourceResponsible = responsible.Value.ToList();
            sourceResponsible.AddRange(selectedResponsible);
            responsible.Value = sourceResponsible.Distinct();
            
            selectedResponsibleText.Value = string.Join("; ", responsible.Value.Select(x => x.Name));
          }
        });
      var deleteResponsibleLink = dialog.AddHyperlink("Исключить ответственного иницатора");
      deleteResponsibleLink.SetOnExecute(
        () =>
        {
          var selectedResponsible = responsible.Value.ShowSelectMany("Выберите ответственного иницатора для исключения");
          if (selectedResponsible != null && selectedResponsible.Any())
          {
            var currentResponsible = responsible.Value.ToList();
            foreach (var item in selectedResponsible)
            {
              currentResponsible.Remove(item);
            }
            responsible.Value = currentResponsible;
            selectedResponsibleText.Value = string.Join("; ", responsible.Value.Select(x => x.Name));
          }
        });
      
      #endregion
      
      var attorneyDefault = new List<Sungero.Parties.ICounterparty>();
      var attorney = dialog.AddSelectMany("Поверенный", false, attorneyDefault.ToArray());
      attorney.IsEnabled = false;
      attorney.IsVisible = false;
      var selectedAttorneyText = dialog.AddMultilineString("Поверенный", false, string.Empty).WithRowsCount(3);
      selectedAttorneyText.IsEnabled = false;

      #region Поверенный
      
      var addAttorneyLink = dialog.AddHyperlink("Добавить поверенного");
      addAttorneyLink.SetOnExecute(
        () =>
        {
          var selectedAttorney = Sungero.Parties.Counterparties.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
            .Where(x => Sungero.Parties.People.Is(x) || Sungero.Parties.Companies.Is(x))
            .ShowSelectMany().AsEnumerable();
          if (selectedAttorney != null && selectedAttorney.Any())
          {
            var sourceAttorney = attorney.Value.ToList();
            sourceAttorney.AddRange(selectedAttorney);
            attorney.Value = sourceAttorney.Distinct();
            
            selectedAttorneyText.Value = string.Join("; ", attorney.Value.Select(x => x.Name));
          }
        });
      var deleteAttorneyLink = dialog.AddHyperlink("Исключить поверенного");
      deleteAttorneyLink.SetOnExecute(
        () =>
        {
          var selectedAttorney = attorney.Value.ShowSelectMany("Выберите поверенного для исключения");
          if (selectedAttorney != null && selectedAttorney.Any())
          {
            var currentAttorney = attorney.Value.ToList();
            foreach (var item in selectedAttorney)
            {
              currentAttorney.Remove(item);
            }
            attorney.Value = currentAttorney;
            selectedAttorneyText.Value = string.Join("; ", attorney.Value.Select(x => x.Name));
          }
        });
      
      #endregion
      

      if(dialog.Show() != DialogButtons.Ok)
      {
        e.Cancel = true;
        return;
      }
      
      RegistryPowerOfAttorney.Companies.AddRange(companies.Value);
      RegistryPowerOfAttorney.ValidFrom = validFrom.Value;
      RegistryPowerOfAttorney.ValidTo = validTo.Value;
      if(responsible.Value.Any())
        RegistryPowerOfAttorney.Responsible.AddRange(responsible.Value);
      if(attorney.Value.Any())
        RegistryPowerOfAttorney.Attorney.AddRange(attorney.Value);
      
    }

  }
}