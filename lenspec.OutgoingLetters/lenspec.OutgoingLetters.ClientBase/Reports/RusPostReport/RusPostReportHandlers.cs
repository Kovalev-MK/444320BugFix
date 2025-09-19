using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters
{
  partial class RusPostReportClientHandlers
  {

    //Добавлено Avis Expert
    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      RusPostReport.BeginDate = Calendar.Today.BeginningOfMonth();
      RusPostReport.EndDate = Calendar.Today.EndOfMonth();
      
      var dialog = Dialogs.CreateInputDialog("Отчет Реестр для Руспоста");
      var beginDate = dialog.AddDate("Дата письма с", true, RusPostReport.BeginDate.Value);
      var endDate = dialog.AddDate("Дата письма по", true, RusPostReport.EndDate.Value);
      
      var addressesDefault = new List<Etalon.ICounterparty>();
      var addressees = dialog.AddSelectMany("Адресаты", false, addressesDefault.ToArray());
      addressees.IsEnabled = false;
      addressees.IsVisible = false;
      var selectedAddresseeText = dialog.AddMultilineString("Адресаты", false, string.Empty).WithRowsCount(3);
      selectedAddresseeText.IsEnabled = false;
      
      var addAddresseeLink = dialog.AddHyperlink("Добавить адресатов");
      addAddresseeLink.SetOnExecute(
        () =>
        {
          var selectedAddresses = Etalon.Counterparties.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ShowSelectMany().AsEnumerable();
          if (selectedAddresses != null && selectedAddresses.Any())
          {
            var sourceAddressees = addressees.Value.ToList();
            sourceAddressees.AddRange(selectedAddresses);
            addressees.Value = sourceAddressees.Distinct();
            
            selectedAddresseeText.Value = string.Join("; ", addressees.Value.Select(x => x.Name));
          }
        });
      var deleteAddresseeLink = dialog.AddHyperlink("Исключить адресатов");
      deleteAddresseeLink.SetOnExecute(
        () =>
        {
          var selectedAddresses = addressees.Value.ShowSelectMany("Выберите адресатов для исключения");
          if (selectedAddresses != null && selectedAddresses.Any())
          {
            var currentAdressees = addressees.Value.ToList();
            foreach (var addressee in selectedAddresses)
            {
              currentAdressees.Remove(addressee);
            }
            addressees.Value = currentAdressees;
            selectedAddresseeText.Value = string.Join("; ", addressees.Value.Select(x => x.Name));
          }
        });
      
      var businessUnitsDefault = new List<Etalon.IBusinessUnit>();
      var businessUnits = dialog.AddSelectMany("Наши организации", false, businessUnitsDefault.ToArray());
      businessUnits.IsEnabled = false;
      businessUnits.IsVisible = false;
      var selectedBussinesUnitText = dialog.AddMultilineString("Наши организации", false, string.Empty).WithRowsCount(3);
      selectedBussinesUnitText.IsEnabled = false;
      
      var addBussinesUnitLink = dialog.AddHyperlink("Добавить наши организации");
      addBussinesUnitLink.SetOnExecute(
        () =>
        {
          var selectedBussinesUnits = Etalon.BusinessUnits.GetAll().ShowSelectMany().AsEnumerable();
          if (selectedBussinesUnits != null && selectedBussinesUnits.Any())
          {
            var sourceBussinesUnits = businessUnits.Value.ToList();
            sourceBussinesUnits.AddRange(selectedBussinesUnits);
            businessUnits.Value = sourceBussinesUnits.Distinct();
            
            selectedBussinesUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
          }
        });
      var deleteBussinesUnitLink = dialog.AddHyperlink("Исключить наши организации");
      deleteBussinesUnitLink.SetOnExecute(
        () =>
        {
          var selectedBussinesUnits = businessUnits.Value.ShowSelectMany("Выберите наши организации для исключения");
          if (selectedBussinesUnits != null && selectedBussinesUnits.Any())
          {
            var currentBussinesUnits = businessUnits.Value.ToList();
            foreach (var bussinesUnit in selectedBussinesUnits)
            {
              currentBussinesUnits.Remove(bussinesUnit);
            }
            businessUnits.Value = currentBussinesUnits;
            selectedBussinesUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
          }
        });
      
      dialog.SetOnButtonClick((args) =>
                              {
                                Sungero.Docflow.PublicFunctions.Module.CheckReportDialogPeriod(args, beginDate, endDate);
                              });
      
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok;
      
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        RusPostReport.BeginDate = beginDate.Value;
        RusPostReport.EndDate = endDate.Value;
        RusPostReport.Addressee.AddRange(addressees.Value.ToList());
        RusPostReport.BusinessUnit.AddRange(businessUnits.Value.ToList());
      }
      else
      {
        e.Cancel = true;
      }
    }
    //конец Добавлено Avis Expert

  }
}