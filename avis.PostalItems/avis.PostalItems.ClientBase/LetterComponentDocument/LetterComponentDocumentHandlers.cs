using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PostalItems.LetterComponentDocument;

namespace avis.PostalItems
{
  partial class LetterComponentDocumentClientHandlers
  {

    public virtual void PostalItemValueInput(avis.PostalItems.Client.LetterComponentDocumentPostalItemValueInputEventArgs e)
    {
      if (Equals(e.OldValue, e.NewValue))
        return;
      
      // Обновляем связь между документами.
      if (e.OldValue != null && e.OldValue.OutgoingLetter != null)
      {
        if (e.OldValue.OutgoingLetter.AccessRights.CanUpdate(Users.Current))
        {
          _obj.Relations.RemoveFrom(Sungero.Docflow.Constants.Module.AddendumRelationName, e.OldValue.OutgoingLetter);
        }
        else
        {
          e.AddWarning("Не удалось удалить связь между Исх. письмом и Компонентом письма: нет прав на изменение.");
        }
      }
      if (e.NewValue != null && e.NewValue.OutgoingLetter != null)
      {
        if (e.NewValue.OutgoingLetter.AccessRights.CanUpdate(Users.Current))
        {
          _obj.Relations.AddFrom(Sungero.Docflow.Constants.Module.AddendumRelationName, e.NewValue.OutgoingLetter);
        }
        else
        {
          e.AddWarning("Не удалось добавить связь между Исх. письмом и Компонентом письма: нет прав на изменение.");
        }
      }
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      //base.Refresh(e);
      _obj.State.Properties.Subject.IsRequired = false;
    }
    
    /// <summary>
    /// Показ формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      if (string.IsNullOrEmpty(_obj.Name))
        _obj.Name = "<Имя будет сформировано автоматически по содержанию и другим реквизитам документа>";
      
      //base.Showing(e);
    }
  }
}