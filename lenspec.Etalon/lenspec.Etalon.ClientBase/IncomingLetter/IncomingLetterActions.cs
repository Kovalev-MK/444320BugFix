using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingLetter;

namespace lenspec.Etalon.Client
{
  partial class IncomingLetterActions
  {
    public override void AddRegistrationStamp(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.AddRegistrationStamp(e);
    }

    public override bool CanAddRegistrationStamp(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanAddRegistrationStamp(e);
    }



    
    //Добавлено Avis Expert
    public override void UpdateTemplatelenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.UpdateTemplatelenspec(e);
    }

    public override bool CanUpdateTemplatelenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
    }

    public virtual void SendForSimpleTaskavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var simpleTask = Sungero.Workflow.SimpleTasks.Create();
      simpleTask.Attachments.Add(_obj);
      simpleTask.Subject = _obj.Name;
      
      var addendas = _obj.Relations.GetRelatedDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName);
      foreach(var addenda in addendas)
      {
        simpleTask.Attachments.Add(addenda);
      }
      simpleTask.NeedsReview = false;
      simpleTask.Show();
    }

    public virtual bool CanSendForSimpleTaskavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
    //конец Добавлено Avis Expert

  }

}