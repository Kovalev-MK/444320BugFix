using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.RecordManagementUI.Client
{
  partial class ModuleFunctions
  {

    /// <summary>
    /// 
    /// </summary>
    [LocalizeFunction("CallRusPostMailList_ResourceKey", "CallRusPostMailList_DescriptionResourceKey")]
    public virtual void CallRusPostMailList()
    {
      var dialog = Dialogs.CreateInputDialog("Параметры отчета");
      
      var vid = dialog.AddSelect("Вид отправки письма", true).From("Ценные письма","Заказные письма");
      
      var datastart = dialog.AddDate("Дата с", true);
      var dataend = dialog.AddDate("Дата по", true);
      dataend.Value = Calendar.Today;
      
      dialog.Buttons.AddOkCancel();
      
      dialog.SetOnRefresh((d) => {

                            if (datastart.Value > dataend.Value && datastart.Value != null )
                            {
                              d.AddError("Дата в поле \"Дата с\" не может быть больше даты в поле \"Дата по\"");
                            }
                          });
      if (dialog.Show() == DialogButtons.Ok)
      {
        try
        {
          if( vid.Value == "Ценные письма")
          {
            var report = lenspec.OutgoingLetters.Reports.GetRusPostMailListNew();
            report.Vid = vid.Value;
            report.DateStart = datastart.Value;
            report.DateEnd = dataend.Value;
            
            report.Open();
          }
          else
          {
            var report = lenspec.OutgoingLetters.Reports.GetRusPostMailListShort();
            report.Vid = vid.Value;
            report.DateStart = datastart.Value;
            report.DateEnd = dataend.Value;
            
            report.Open();
          }
        }
        catch (Exception ex)
        {
          Dialogs.NotifyMessage(ex.Message);
        }
      }
    }
  }
}