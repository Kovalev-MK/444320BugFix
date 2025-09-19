using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PostalItems.LetterComponentDocument;

namespace avis.PostalItems
{
  partial class LetterComponentDocumentServerHandlers
  {
    // TODO: После установки на прод, можно будет удалить перекрытие метода.
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      /* Вынесено в FillName
      if (_obj.Archiveavis != true)
      {
        _obj.Name = $"{_obj.DocumentKind.Name} на {_obj.PostalItem.Name}";
        // base.BeforeSave(e);
      }
      */
    }
    
    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.DateCreated = Calendar.Now;
      // base.Created(e);
    }
  }

}