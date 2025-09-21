using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentTemplate;

namespace lenspec.Etalon.Client
{
  partial class DocumentTemplateActions
  {
    public virtual void ShowDuplicateslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.DocumentTemplate.Remote.GetDuplicateDocumentTemplates(_obj.Name, _obj.Id);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(lenspec.Etalon.DocumentTemplates.Resources.DuplicatesNotFound);
    }

    public virtual bool CanShowDuplicateslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}