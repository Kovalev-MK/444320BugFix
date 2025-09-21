using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.BusinessUnit;

namespace lenspec.Etalon.Client
{
  partial class BusinessUnitFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Загрузить Нашу организацию из 1С.
    /// </summary>
    public static void DownloadBusinessUnitFrom1C()
    {
      var tin = string.Empty;
      
      var dialog = Dialogs.CreateInputDialog("Загрузка компании из 1С", "Введите ИНН нашей организации");
      var tinParameter = dialog.AddString("ИНН", true);
      var downloadButton = dialog.Buttons.AddCustom("Загрузить");
      dialog.Buttons.AddCancel();
      
      dialog.SetOnButtonClick(
        (args) =>
        {
          #region Загрузить
          
          if (Equals(args.Button, downloadButton))
          {
            // Проверить заполненность обязательных параметров.
            if (string.IsNullOrWhiteSpace(tinParameter.Value))
              return;
            
            // Проверить валидность ИНН.
            var result = Sungero.Parties.PublicFunctions.Counterparty.CheckTin(tin, true);
            if (!string.IsNullOrEmpty(result))
            {
              args.AddError(result, tinParameter);
              return;
            }
          }
          
          #endregion
        });
      
      #region События
      
      // Прокинуть параметры для сохранения значений при перестроении диалогов.
      tinParameter.SetOnValueChanged(
        (args) =>
        {
          tin = tinParameter.Value;
        });
      
      #endregion
      
      if (dialog.Show() == downloadButton)
      {
        try
        {
          Dialogs.ShowMessage(PublicFunctions.BusinessUnit.GetBusinessUnitFrom1C(tin));
        }
        catch (Exception ex)
        {
          Dialogs.ShowMessage(ex.Message, Sungero.Parties.Resources.ContactAdministrator, MessageType.Error);
          Logger.ErrorFormat("Error on find Business Unit in 1C. {0}", ex.Message);
        }
      }
    }
    //конец Добавлено Avis Expert
  }
}