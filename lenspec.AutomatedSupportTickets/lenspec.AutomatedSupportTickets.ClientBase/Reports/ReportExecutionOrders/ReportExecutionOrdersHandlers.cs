using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets
{
  partial class ReportExecutionOrdersClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      ReportExecutionOrders.beginDate  = Calendar.Today.BeginningOfMonth();
      ReportExecutionOrders.endDate = Calendar.Now;
      var dialog = Dialogs.CreateInputDialog("Отчет об исполнении поручений");
      var beginDate = dialog.AddDate("Дата документа с", false, ReportExecutionOrders.beginDate);
      var endDate = dialog.AddDate("Дата документа по", false, ReportExecutionOrders.endDate);
      var executor = dialog.AddSelect("Исполнитель", false, ReportExecutionOrders.executor);
      var controller = dialog.AddSelect("Контролёр", false, ReportExecutionOrders.controller);
      var term = dialog.AddDate("Срок исполнения поручения", false, ReportExecutionOrders.term);
      var ourCF = dialog.AddSelect("ИСП", false, ReportExecutionOrders.ourCF);
      var correspondent = dialog.AddSelect("Корреспондент", false, ReportExecutionOrders.correspondent);
      var actionStatus = dialog.AddSelect("Состояние поручения", true, 0).From("В работе", "На контроле", "Исполнено");
      
      #region НОР
      var businessUnitsDefault = new List<Etalon.IBusinessUnit>();
      var businessUnits = dialog.AddSelectMany("Наша организация", false, businessUnitsDefault.ToArray());
      businessUnits.IsEnabled = false;
      businessUnits.IsVisible = false;
      var selectedBussinesUnitText = dialog.AddMultilineString("Наша организация", false, string.Empty).WithRowsCount(3);
      selectedBussinesUnitText.IsEnabled = false;
      
      var addBussinesUnitLink = dialog.AddHyperlink("Добавить Наши организации");
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
      var deleteBussinesUnitLink = dialog.AddHyperlink("Исключить Наши организации");
      deleteBussinesUnitLink.SetOnExecute(
        () =>
        {
          var selectedBussinesUnits = businessUnits.Value.ShowSelectMany("Выберите Наши организации для исключения");
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
      
      dialog.SetOnRefresh((d) => {

                            if (beginDate.Value > endDate.Value && beginDate.Value != null )
                            {
                              d.AddError("Дата в поле \"Дата документа с\" не может быть больше даты в поле \"Дата документа по\"");
                            }
                          });
      if (dialog.Show() == DialogButtons.Ok)
      {
        if (beginDate.Value != null)
        {
          ReportExecutionOrders.beginDate = beginDate.Value;
        }
        else
        {
          ReportExecutionOrders.beginDate = null;
        }
        if (endDate.Value != null)
        {
          ReportExecutionOrders.endDate = endDate.Value;
        }
        else
        {
          ReportExecutionOrders.endDate = null;
        }
        ReportExecutionOrders.executor = executor.Value;
        ReportExecutionOrders.controller = controller.Value;
        ReportExecutionOrders.term = term.Value;
        ReportExecutionOrders.ourCF = ourCF.Value;
        ReportExecutionOrders.correspondent = correspondent.Value;
        ReportExecutionOrders.BusinessUnit.AddRange(businessUnits.Value.ToList());
        ReportExecutionOrders.CurrentUser = Users.Current;
        ReportExecutionOrders.status = actionStatus.Value;
      }
      else
      {
        e.Cancel = true;
      }
    }
  }
}
