using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets
{
  partial class CompanyStafWithHierarchyClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("По какой организации выгрузить отчёт?");

      var businessUnits = dialog.AddSelectMany("Наши организации", false, lenspec.Etalon.BusinessUnits.Null);
      businessUnits.IsEnabled = false;
      businessUnits.IsVisible = false;
      
      var selectedBussinesUnitText = dialog.AddMultilineString("Наши организации", false, string.Empty).WithRowsCount(3);
      selectedBussinesUnitText.IsEnabled = false;
      var addBussinesUnitLink = dialog.AddHyperlink("Добавить наши организации");
      var deleteBussinesUnitLink = dialog.AddHyperlink("Исключить наши организации");
      
      var today = dialog.AddDate("Текущая дата", false, Calendar.Now);
      today.IsEnabled = false;
      today.IsVisible = false;
      
      #region Наши организации
      
      addBussinesUnitLink.SetOnExecute(
        () =>
        {
          var selectedBussinesUnits = lenspec.Etalon.BusinessUnits.GetAll().Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ShowSelectMany().AsEnumerable();
          if (selectedBussinesUnits != null && selectedBussinesUnits.Any())
          {
            var sourceBussinesUnits = businessUnits.Value.ToList();
            sourceBussinesUnits.AddRange(selectedBussinesUnits);
            businessUnits.Value = sourceBussinesUnits.Distinct();
            
            selectedBussinesUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
          }
        });
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
      
      #endregion
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        CompanyStafWithHierarchy.Today = today.Value.Value;
        
        if (businessUnits.Value.Any())
        {
          CompanyStafWithHierarchy.Companies.AddRange(businessUnits.Value);          
          CompanyStafWithHierarchy.CompanyIDs = string.Join(",", businessUnits.Value.Select(x => x.Id).ToList());
        }
        else
        {
          var allbusinessUnit = lenspec.Etalon.BusinessUnits.GetAll().Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
          CompanyStafWithHierarchy.Companies.AddRange(allbusinessUnit);
          CompanyStafWithHierarchy.CompanyIDs = string.Join(",", allbusinessUnit.Select(x => x.Id).ToList());
        }
      }
      else
      {
        e.Cancel = true;
      }
    }
  }
}