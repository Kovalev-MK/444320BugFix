using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PostalItems.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Список компонентов письма по критериям поиска.
    /// </summary>
    [LocalizeFunction("SearchLetterComponentsByAuthor_ResourceKey", "SearchLetterComponentsByAuthor_DescriptionResourceKey")]
    public virtual IQueryable<ILetterComponentDocument> SearchLetterComponentsByAuthor()
    {
      var dialog = Dialogs.CreateInputDialog("Поиск компонентов письма");
      var componentType = dialog.AddSelect("Компонент письма", true).From("Уведомление", "Чек", "Конверт");
      var dateFrom = dialog.AddDate("Дата создания с", false);
      var dateTo = dialog.AddDate("Дата создания по", false);
      var author = dialog.AddSelect("Автор", false, Sungero.Company.Employees.Current);
      dialog.SetOnButtonClick((args) =>
                              {
                                Sungero.Docflow.PublicFunctions.Module.CheckDialogPeriod(args, dateFrom, dateTo);
                              });
      
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok;
      if (dialog.Show() == DialogButtons.Ok)
        return Functions.Module.Remote.SearchLetterComponents(componentType.ValueIndex, dateFrom.Value, dateTo.Value, author.Value);
      return null;
    }

    /// <summary>
    /// Список почтовых отправлений по критериям поиска.
    /// </summary>
    [LocalizeFunction("SearchPostalItemsByAuthor_ResourceKey", "SearchPostalItemsByAuthor_DescriptionResourceKey")]
    public virtual IQueryable<IPostalItem> SearchPostalItemsByAuthor()
    {
      var dialog = Dialogs.CreateInputDialog("Почтовые отправления");
      var dateFrom = dialog.AddDate("Дата создания с", false);
      var dateTo = dialog.AddDate("Дата создания по", false);
      var author = dialog.AddSelect("Отправитель", false, Sungero.Company.Employees.Current);
      dialog.SetOnButtonClick((args) =>
                              {
                                Sungero.Docflow.PublicFunctions.Module.CheckDialogPeriod(args, dateFrom, dateTo);
                              });
      
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok;
      if (dialog.Show() == DialogButtons.Ok)
        return Functions.Module.Remote.SearchPostalItems(dateFrom.Value, dateTo.Value, author.Value);
      return null;
    }

    /// <summary>
    /// Открыть список почтовых отправлений.
    /// </summary>
    [LocalizeFunction("PostalItemsShow_ResourceKey", "PostalItemsShow_DescriptionResourceKey")]
    public virtual void PostalItemsShow()
    {
      // Проверяем что пользователь входит в роль администртор и делопроизводитель.
      var clerks = Roles.GetAll(r => r.Sid == Sungero.Docflow.PublicConstants.Module.RoleGuid.ClerksRole).FirstOrDefault();
      if (Sungero.Company.Employees.Current.IncludedIn(clerks) || Sungero.Company.Employees.Current.IncludedIn(Roles.Administrators))
      {
        PostalItems.GetAll().Show();
        return;
      }
      
      // Выводим диалог с ошибкой.
      var dialog = Dialogs.CreateTaskDialog("Вы должны входить в роль Делопроизводители.");
      dialog.Buttons.AddOk();
      dialog.Show();
    }

    #region [Отчёты]
    
    /// <summary>
    /// Отчёт "Описи".
    /// </summary>
    [LocalizeFunction("ReportInventories_ResourceKey", "ReportInventories_DescriptionResourceKey")]
    public virtual void ReportInventories()
    {
      var dialog = Dialogs.CreateInputDialog(avis.PostalItems.Resources.InternalMailInvenory);
      var beginDateCreate = dialog.AddDate(avis.PostalItems.Resources.CreationDateFrom, true, Calendar.Today.BeginningOfMonth());
      var endDateCreate = dialog.AddDate(avis.PostalItems.Resources.CreationDateTill, true, Calendar.Today);
      var mailDeliveryMethod = dialog.AddSelect(avis.PostalItems.Resources.DeliveryMethod, false, Sungero.Docflow.MailDeliveryMethods.Null);
      var senders = dialog.AddSelectMany(avis.PostalItems.Resources.Sender, false, lenspec.Etalon.BusinessUnits.Null);
        
      dialog.SetOnButtonClick((args) =>
                              {
                                // Проверка, что "Дата с" не больше "Даты по".
                                if (beginDateCreate.Value > endDateCreate.Value)
                                {
                                  args.AddError(avis.PostalItems.Resources.CreationDateFromTillVerificationError);
                                  return;
                                }
                              });
      
      if (dialog.Show() == DialogButtons.Ok)
      { 
        // Создаём отчёт.
        var report = avis.PostalItems.Reports.GetInventoryReport();
        report.BeginDateCreate = beginDateCreate.Value;
        report.EndDateCreate = endDateCreate.Value;
        report.MailDeliveryMethod = mailDeliveryMethod.Value;
        report.Senders.AddRange(senders.Value);
        report.Open();
      }
    }

    /// <summary>
    /// Отчёт "Конверты".
    /// </summary>
    [LocalizeFunction("ReportEnvelopes_ResourceKey", "ReportEnvelopes_DescriptionResourceKey")]
    public virtual void ReportEnvelopes()
    {
      var dialog = Dialogs.CreateInputDialog(avis.PostalItems.Resources.InternalMailEnvelopes);
      
      dialog.SetOnRefresh((x) =>
                          {
                            x.AddInformation(avis.PostalItems.Resources.ChooseEnvelopeSize);
                          });

      var beginDateCreate = dialog.AddDate(avis.PostalItems.Resources.CreationDateFrom, true, Calendar.Today.BeginningOfMonth());
      var endDateCreate = dialog.AddDate(avis.PostalItems.Resources.CreationDateTill, true, Calendar.Today);
      var mailDeliveryMethod = dialog.AddSelect(avis.PostalItems.Resources.DeliveryMethod, false, Sungero.Docflow.MailDeliveryMethods.Null);
      var senders = dialog.AddSelectMany(avis.PostalItems.Resources.Sender, false, lenspec.Etalon.BusinessUnits.Null);
      
      dialog.SetOnButtonClick((args) =>
                              {
                                // Проверка, что "Дата с" не больше "Даты по".
                                if (beginDateCreate.Value > endDateCreate.Value)
                                {
                                  args.AddError(avis.PostalItems.Resources.CreationDateFromTillVerificationError);
                                  return;
                                }
                              });
      
      var buttonC4 = dialog.Buttons.AddCustom(avis.PostalItems.Resources.EnvelopeSizeA4);
      var buttonC5 = dialog.Buttons.AddCustom(avis.PostalItems.Resources.EnvelopeSizeA5);
      
      // Отобразить диалог.
      var result = dialog.Show();
      
      if (result == buttonC4)
      {
        // Проверка, что "Дата с" не больше "Даты по".
        if (beginDateCreate.Value > endDateCreate.Value)
        {
          Dialogs.CreateConfirmDialog(avis.PostalItems.Resources.CreationDateFromTillVerificationError).Show();
          return;
        }
        
        // Создаём отчёт.
        var report = avis.PostalItems.Reports.GetEnvelopeC4Report();
        report.BeginDateCreate = beginDateCreate.Value;
        report.EndDateCreate = endDateCreate.Value;
        report.MailDeliveryMethod = mailDeliveryMethod.Value;
        report.Senders.AddRange(senders.Value);
        report.Open();
      }
      
      if (result == buttonC5)
      {
        // Создаём отчёт.
        var report = avis.PostalItems.Reports.GetEnvelopeC5Report();
        report.BeginDateCreate = beginDateCreate.Value;
        report.EndDateCreate = endDateCreate.Value;
        report.MailDeliveryMethod = mailDeliveryMethod.Value;
        report.Senders.AddRange(senders.Value);
        report.Open();
      }
    }
    
    /// <summary>
    /// Отчёт "Уведомления по внутренним отправлениям".
    /// </summary>
    [LocalizeFunction("ReportNotifications_ResourceKey", "ReportNotifications_DescriptionResourceKey")]
    public virtual void ReportNotifications()
    {
      var dialog = Dialogs.CreateInputDialog(avis.PostalItems.Resources.InternalPostalItemsNotifications);
      var beginDateCreate = dialog.AddDate(avis.PostalItems.Resources.CreationDateFrom, true, Calendar.Today.BeginningOfMonth());
      var endDateCreate = dialog.AddDate(avis.PostalItems.Resources.CreationDateTill, true, Calendar.Today);
      var mailDeliveryMethod = dialog.AddSelect(avis.PostalItems.Resources.DeliveryMethod, false, lenspec.Etalon.MailDeliveryMethods.Null).Where(m => m.NotificationTypeavis != null);
      var senders = dialog.AddSelectMany(avis.PostalItems.Resources.Sender, false, lenspec.Etalon.BusinessUnits.Null);
        
      dialog.SetOnButtonClick((args) =>
                              {
                                // Проверка, что "Дата с" не больше "Даты по".
                                if (beginDateCreate.Value > endDateCreate.Value)
                                {
                                  args.AddError(avis.PostalItems.Resources.CreationDateFromTillVerificationError);
                                  return;
                                }
                              });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        // Проверка, что "Дата с" не больше "Даты по".
        if (beginDateCreate.Value > endDateCreate.Value)
        {
          Dialogs.CreateConfirmDialog(avis.PostalItems.Resources.CreationDateFromTillVerificationError).Show();
          return;
        }
        
        // Создаём отчёт.
        var report = avis.PostalItems.Reports.GetNotificationReport();
        report.BeginDateCreate = beginDateCreate.Value;
        report.EndDateCreate = endDateCreate.Value;
        report.MailDeliveryMethod = mailDeliveryMethod.Value;
        report.Senders.AddRange(senders.Value);
        report.Open();
      }
    }
    
    #endregion [Отчёты]
    
    /// <summary>
    /// Создать новое почтовое отправление.
    /// </summary>
    [LocalizeFunction("CreatePostalItemShow_ResourceKey", "CreatePostalItemShow_DescriptionResourceKey")]
    public virtual void CreatePostalItemShow()
    {
      PostalItems.Create().Show();
    }
  }
}