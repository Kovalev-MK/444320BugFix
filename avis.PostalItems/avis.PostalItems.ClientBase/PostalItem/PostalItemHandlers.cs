using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PostalItems.PostalItem;
using System.Text.RegularExpressions;

namespace avis.PostalItems
{
  // Добавлено avis.
  
  partial class PostalItemClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      var error = string.Empty;
      e.Params.TryGetValue(Constants.PostalItem.Params.PostalCodeCalculationError, out error);
      
      if (!string.IsNullOrEmpty(error))
      {
        e.AddError(error);
        e.Params.Remove(Constants.PostalItem.Params.PostalCodeCalculationError);
      }
    }
    
    /// <summary>
    /// Проверить строку на наличие только цифр в строке.
    /// </summary>
    /// <param name="text">Текст для проверки.</param>
    /// <returns>true-только цифры, false-символ кроме числа.</returns>
    private bool IsNumber(string text)
    {
      Regex regex = new Regex(@"^[0-9]+$");
      return regex.IsMatch(text);
    }

    public virtual void BarcodeValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (string.IsNullOrEmpty(e.NewValue))
        return;
      
      // Если ввели что-то кроме цифр. Выводи ошибку. 
      if (IsNumber(e.NewValue) == false)
      {
        e.NewValue = "";
        var dialog = Dialogs.CreateTaskDialog("Штрих-код может содержать только цифры.");
        dialog.Buttons.AddCancel();
        dialog.Show();
      }
    }
    
    /// <summary>
    /// Показ формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      // Активируем поле "Кому".
      avis.PostalItems.PublicFunctions.PostalItem.EnableTo(_obj);

      if (string.IsNullOrEmpty(_obj.Name))
        _obj.Name = "<Наименование будет сформированно автоматически при сохранении>";
    }
  }
  
  // Конец добавлено avis.
}