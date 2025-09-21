using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace lenspec.AutomatedSupportTickets.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Загрузка пользователей из файла
    /// </summary>
    [LocalizeFunction("FillingUserGroupsFromFile", "SettingUpUserGroups")]
    public virtual void FillinGroupsTest()
    {
      // Диалог выбора файла
      var FileDialog = Dialogs.CreateInputDialog("Выберите файл");
      var roles = FileDialog.AddSelect("Группа пользователей", true, Sungero.CoreEntities.Roles.Null);
      var File = FileDialog.AddFileSelect("Файл", true);
      File.WithFilter("Excel файлы", ".xlsx", ".xls");
      if (FileDialog.Show() != DialogButtons.Ok)
        return;
      
      // Получить содержимое файла.
      var fileContent = File.Value.Content;
      
      // Передать поток в функцию
      string result = "";
      try
      {
        result = lenspec.AutomatedSupportTickets.PublicFunctions.Module.TestParseExcel(fileContent, File.Value.Name, roles.Value.Id);
        Dialogs.NotifyMessage(lenspec.AutomatedSupportTickets.Resources.GoodCheck);
      }
      catch (Exception ex)
      {
        Dialogs.NotifyMessage(ex.Message);
        return;
      }
    }
    
    /// <summary>
    /// Проверить даты диалога.
    /// </summary>
    /// <param name="args">Аргументы события нажатия на кнопку диалога.</param>
    /// <param name="dialogPeriodBegin">Параметр даты начала.</param>
    /// <param name="dialogPeriodEnd">Параметр даты конца.</param>
    [Public]
    public static void CheckDialogPeriodBetween(CommonLibrary.InputDialogButtonClickEventArgs args,
                                                CommonLibrary.IDateDialogValue dialogPeriodBegin,
                                                CommonLibrary.IDateDialogValue dialogPeriodEnd)
    {
      CheckDialogPeriodBetween(args, dialogPeriodBegin, dialogPeriodEnd, lenspec.AutomatedSupportTickets.Resources.CheckWrong);
    }
    
    /// <summary>
    /// Проверить даты диалога.
    /// </summary>
    /// <param name="args">Аргументы события нажатия на кнопку диалога.</param>
    /// <param name="dialogPeriodBegin">Параметр даты начала.</param>
    /// <param name="dialogPeriodEnd">Параметр даты конца.</param>
    /// <param name="wrongPeriodError">Текст ошибки о неверной дате.</param>
    private static void CheckDialogPeriodBetween(CommonLibrary.InputDialogButtonClickEventArgs args,
                                                 CommonLibrary.IDateDialogValue dialogPeriodBegin,
                                                 CommonLibrary.IDateDialogValue dialogPeriodEnd,
                                                 CommonLibrary.LocalizedString wrongPeriodError)
    {
      var periodBegin = dialogPeriodBegin.Value;
      var periodEnd = dialogPeriodEnd.Value;
      var truedate = periodBegin.Value.AddMonths(2);
      
      if (periodBegin.HasValue && periodEnd.HasValue &&  truedate < periodEnd.Value)
      {
        args.AddError(wrongPeriodError);
      }
    }

    //Добавлено Avis Expert
    /// <summary>
    /// Создать заявку на формирование замещения.
    /// </summary>
    [LocalizeFunction("CreateSubstitutionRequest_ResourceKey", "CreateSubstitutionRequest_DescriptionResourceKey")]
    public virtual void CreateSubstitutionRequest()
    {
      AutomatedSupportTickets.PublicFunctions.Module.Remote.CreateSubstitutionRequest().Show();
    }
    //конец Добавлено Avis Expert

  }
}