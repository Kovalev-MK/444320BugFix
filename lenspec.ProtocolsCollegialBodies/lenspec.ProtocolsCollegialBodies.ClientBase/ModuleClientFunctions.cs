using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ProtocolsCollegialBodies.Client
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Список прокололов коллегиальных органов по критериям поиска.
    /// </summary>
    [LocalizeFunction("SearchProtocolsByAuthor_ResourceKey", "SearchProtocolsByAuthor_DescriptionResourceKey")]
    public virtual IQueryable<IProtocolCollegialBody> SearchProtocolsByAuthor()
    {
      var dialog = Dialogs.CreateInputDialog("Протоколы коллегиальных органов");
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
        return Functions.Module.Remote.SearchProtocol(dateFrom.Value, dateTo.Value, author.Value);
      return null;
    }

    /// <summary>
    /// Создать протокол.
    /// </summary>
    [LocalizeFunction("CreateProtocol_ResourceKey", "CreateProtocol_DescriptionResourceKey")]
    public virtual void CreateProtocol()
    {
      ProtocolsCollegialBodies.PublicFunctions.Module.Remote.CreateProtocol().Show();
    }
  }
}