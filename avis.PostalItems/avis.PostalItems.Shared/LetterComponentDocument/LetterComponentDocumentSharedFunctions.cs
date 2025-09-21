using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PostalItems.LetterComponentDocument;

namespace avis.PostalItems.Shared
{
  partial class LetterComponentDocumentFunctions
  {
    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      if (_obj.Archiveavis != true)
        _obj.Name = $"{_obj.DocumentKind.Name} на {_obj.PostalItem.Name}";
    }
  }
}