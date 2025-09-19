using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.ProtocolCollegialBody;

namespace lenspec.ProtocolsCollegialBodies.Client
{
  partial class ProtocolCollegialBodyFunctions
  {

    /// <summary>
    /// Проверим, есть ли тело документа, если нет то выведем диалог.
    /// </summary>
    [Public]
    public bool NeedSendForApproval()
    {
      var result = true;
      
      if (!_obj.HasVersions)
      {
        var dialog = Dialogs.CreateTaskDialog("Для старта необходимо вложить текст протокола по кнопке Создать из шаблона",
                                              MessageType.Information);
        dialog.Buttons.AddOk();
        dialog.Show();
        result = false;
      }
      
      return result;
    }

  }
}