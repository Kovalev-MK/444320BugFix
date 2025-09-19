using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ScanningAssignment;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class ScanningAssignmentActions
  {
    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var documents = _obj.POAs.PowerOfAttorneys.Where(i => i.DocumentKind.Id == Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DocumentNotarialKindGuid).Id);
      if(documents != null && documents.Any())
      {
        var dialog = Dialogs.CreateInputDialog("Введите реестровые номера доверенностей");
        var results = new Dictionary<long, CommonLibrary.IStringDialogValue>();
        foreach(var doc in documents)
        {
          results.Add(doc.Id, dialog.AddString(doc.Name, true));
        }
        if(dialog.Show() == DialogButtons.Ok)
        {
          foreach(var pair in results)
          {
            var poa = documents.Where(i => i.Id == pair.Key).Single();
            poa.RegistryNumavis = pair.Value.Value;
            poa.Save();
          }
        }
      }
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}