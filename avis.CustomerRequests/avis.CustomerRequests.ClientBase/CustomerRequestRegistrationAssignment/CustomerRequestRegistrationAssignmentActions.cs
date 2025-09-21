using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustomerRequestRegistrationAssignment;

namespace avis.CustomerRequests.Client
{
  partial class CustomerRequestRegistrationAssignmentActions
  {
    // Кнопка выполнить.
    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      // Проверить регистрацию документа
      var document = _obj.AllAttachments.FirstOrDefault();
      var customerRequest = avis.CustomerRequests.CustomerRequests.As(document);
      
      var registrationState = customerRequest.RegistrationState;
      if (registrationState == null || (Enumeration)registrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered)
      {
        e.AddError(Sungero.Docflow.ApprovalTasks.Resources.ToPerformNeedRegisterDocument); //, _obj.Info.Actions.Re);
        return;
      }
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }
  }
}