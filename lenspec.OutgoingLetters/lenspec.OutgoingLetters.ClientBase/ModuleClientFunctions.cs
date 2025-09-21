using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Запустить отчет Отчет по почтовым отправлениям в ЛК для Почты РФ.
    /// </summary>
    [LocalizeFunction("MailListReportV2_ResourceKey", "MailListReportV2_DescriptionResourceKey")]
    public virtual void MailListReportV2()
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
            var report = Reports.GetRusPostMailListNew();
            report.Vid = vid.Value;
            report.DateStart = datastart.Value;
            report.DateEnd = dataend.Value;
            report.Open();
          }
          else
          {
            var report = Reports.GetRusPostMailListShort();
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

    //Добавлено Avis Expert
    /// <summary>
    /// Создать Заявку на массовую рассылку уведомлений.
    /// </summary>
    [LocalizeFunction("CreateMassMailingApplication_ResourceKey", "CreateMassMailingApplication_DescriptionResourceKey")]
    public virtual void CreateMassMailingApplication()
    {
      OutgoingLetters.PublicFunctions.Module.Remote.CreateMassMailingApplication().Show();
    }

    /// <summary>
    /// Создать Исходящее письмо.
    /// </summary>
    [LocalizeFunction("CreateOutgoingLetter_ResourceKey", "CreateOutgoingLetter_DescriptionResourceKey")]
    public virtual void CreateOutgoingLetter()
    {
      OutgoingLetters.PublicFunctions.Module.Remote.CreateOutgoingLetter().Show();
    }
    //конец Добавлено Avis Expert

  }
}