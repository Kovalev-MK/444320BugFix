using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.CreateCompanyTask;

namespace avis.EtalonParties
{
  partial class CreateCompanyTaskClientHandlers
  {

    public virtual void ResponsibleEmployeeValueInput(avis.EtalonParties.Client.CreateCompanyTaskResponsibleEmployeeValueInputEventArgs e)
    {
      if (_obj.TypeRequest == CreateCompanyTask.TypeRequest.ResponsibleByCounterparty &&
         e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(avis.EtalonParties.CreateCompanyTasks.Resources.EnterEmployeeValidAccountErrorMessage);
      }
    }

    public virtual void TypeObjectValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      if (e.NewValue == CreateCompanyTask.TypeObject.Person && _obj.TypeRequest == CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
        e.AddError(avis.EtalonParties.CreateCompanyTasks.Resources.CreateOrChangeResponsublePersonErrorMessage);
    }
    
    /// <summary>
    /// Показ формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      // Отображаем поля в зависимости от выбранных значений.
      PublicFunctions.CreateCompanyTask.ShowProperties(_obj);
    }
  }
}