using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.EtalonMemos.Client
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Диалог поиска Служебных записок.
    /// </summary>
    [LocalizeFunction("ShowMemos_ResourceKey", "ShowMemos_DescriptionResourceKey")]
    public virtual System.Linq.IQueryable<lenspec.Etalon.IMemo> SearchMemos()
    {
      // Создать диалог ввода.
      var searchDialog = Dialogs.CreateSearchDialog<lenspec.Etalon.IMemo>("Поиск служебных записок");
      
      if (searchDialog.Show())
      {
        var query = searchDialog.GetQuery();
        query.Show();
      }
      return null;
    }
    
    /// <summary>
    /// Поиск служебных записок текущего пользователя.
    /// </summary>
    [LocalizeFunction("ShowMyMemos_ResourceKey", "ShowMyMemos_DescriptionResourceKey")]
    public virtual System.Linq.IQueryable<lenspec.Etalon.IMemo> ShowMyMemos()
    {
      // Создать диалог ввода.
      var dialog = Dialogs.CreateInputDialog("Поиск моих служебных записок");
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok;
      
      // Добавить дату создания с
      var dateFrom = dialog.AddDate("Дата создания с", false);
      
      // Добавить дату создания по
      var dateTo = dialog.AddDate("Дата создания по", false);
      
      // Добавить выбор автора.
      var userEmployee = dialog.AddSelect("Автор", false, Sungero.CoreEntities.Users.Current);
      
      // Отобразить диалог и проверить, что нажата кнопка «ОК».
      if (dialog.Show() == DialogButtons.Ok)
        // Получить служебные записки по криетриям
        Functions.Module.Remote.GetAllMemos(dateFrom.Value,
                                            dateTo.Value,
                                            userEmployee.Value).Show();
      return null;
    }

    /// <summary>
    /// Создать новую служебную записку.
    /// </summary>
    [LocalizeFunction("CreateMemos_ResourceKey", "CreateMemos_DescriptionResourceKey")]
    public virtual void CreateMemos()
    {
      Functions.Module.Remote.CreateMemos().Show();
    }

  }
}