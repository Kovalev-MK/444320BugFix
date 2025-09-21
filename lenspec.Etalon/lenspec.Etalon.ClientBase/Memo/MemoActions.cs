using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Memo;

namespace lenspec.Etalon.Client
{
  internal static class MemoAddresseesStaticActions
  {

    public static bool CanClearAddresseeslenspec(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = Sungero.Docflow.Memos.As(e.Entity);
      return obj.Addressees.Any() && obj.IsManyAddressees == true && obj.InternalApprovalState == null;
    }

    public static void ClearAddresseeslenspec(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = Sungero.Docflow.Memos.As(e.Entity);
      obj.Addressees.Clear();
    }
  }


  partial class MemoActions
  {



    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Если тела документа нет, то открывать сообщение “Создайте служебную записку по кнопке “Создать из шаблона“
      if (!_obj.HasVersions)
      {
        Dialogs.ShowMessage(string.Format("Создайте хотя бы одну версию для документа {0}.", _obj.Name));
        return;
      }
      
      // Если по документу ранее были запущены задачи, то вывести соответствующий диалог.
      if (Functions.ApprovalTask.CheckDuplicates(_obj, false))
        return;
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

  }


}