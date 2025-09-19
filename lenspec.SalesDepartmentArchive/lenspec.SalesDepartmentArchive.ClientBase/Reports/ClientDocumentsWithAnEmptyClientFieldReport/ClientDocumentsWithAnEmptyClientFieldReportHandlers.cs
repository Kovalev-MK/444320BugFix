using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.SalesDepartmentArchive
{
  partial class ClientDocumentsWithAnEmptyClientFieldReportClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Параметры отчета");
      var businessUnits = dialog.AddSelectMany("Наши организации", false,
                                               Etalon.BusinessUnits.Null);
      businessUnits.IsEnabled = false;
      businessUnits.IsVisible = false;
      
      var selectedBusinessUnitText = dialog.AddMultilineString("Наши организации", false,
                                                               string.Empty).WithRowsCount(3);
      selectedBusinessUnitText.IsEnabled = false;
      
      var addBusinessUnitLink = dialog.AddHyperlink("Добавить наши организации");
      var deleteBusinessUnitLink = dialog.AddHyperlink("Исключить наши организации");
      
      addBusinessUnitLink.SetOnExecute(
        () =>
        {
          var selectedBusinessUnits = Etalon.BusinessUnits.GetAll().ShowSelectMany().AsEnumerable();
          if (selectedBusinessUnits != null && selectedBusinessUnits.Any())
          {
            var sourceBusinessUnits = businessUnits.Value.ToList();
            sourceBusinessUnits.AddRange(selectedBusinessUnits);
            businessUnits.Value = sourceBusinessUnits.Distinct();
            
            selectedBusinessUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
          }
        });
      deleteBusinessUnitLink.SetOnExecute(
        () =>
        {
          var selectedBusinessUnits = businessUnits.Value.ShowSelectMany("Выберите наши организации для исключения");
          if (selectedBusinessUnits != null && selectedBusinessUnits.Any())
          {
            var currentBusinessUnits = businessUnits.Value.ToList();
            foreach (var businessUnit in selectedBusinessUnits)
            {
              currentBusinessUnits.Remove(businessUnit);
            }
            businessUnits.Value = currentBusinessUnits;
            selectedBusinessUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
          }
        });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        
        ClientDocumentsWithAnEmptyClientFieldReport.BusinessUnit.AddRange(businessUnits.Value.ToList());
        
      }
      else
        e.Cancel = true;
    }
  }
}