using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PostalItems.LetterComponentDocument;

namespace avis.PostalItems
{
  partial class LetterComponentDocumentSharedHandlers
  {
    /// <summary>
    /// Изменение значения свойства "Почтовое отправление".
    /// </summary>
    /// <param name="e"></param>
    public virtual void PostalItemChanged(avis.PostalItems.Shared.LetterComponentDocumentPostalItemChangedEventArgs e)
    {
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.Constants.Module.AddendumRelationName, e.OldValue?.OutgoingLetter, e.NewValue?.OutgoingLetter);
    }
    
    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
    }
  }
}