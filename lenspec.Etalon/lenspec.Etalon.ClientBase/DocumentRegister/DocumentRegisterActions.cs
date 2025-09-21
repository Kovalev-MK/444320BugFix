using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentRegister;

namespace lenspec.Etalon.Client
{
  partial class DocumentRegisterActions
  {
    
    //Добавлено Avis Expert
    public virtual void ShowDuplicatesavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.DocumentRegister.Remote.GetDuplicates(_obj);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(Sungero.Commons.Resources.DuplicateNotFound);
    }

    public virtual bool CanShowDuplicatesavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
    //конец Добавлено Avis Expert

  }

}