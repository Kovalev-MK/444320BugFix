using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.ApproveCounterpartyAssignment;

namespace avis.EtalonParties
{
  // Добавлено avis.
  
  partial class ApproveCounterpartyAssignmentClientHandlers
  {
    /// <summary>
    /// Показ формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      _obj.ActiveText = string.Empty;
      
      _obj.State.Properties.CompanyName.IsVisible = false;
      _obj.State.Properties.Company.IsVisible = false;
      _obj.State.Properties.TIN.IsVisible = false;
      _obj.State.Properties.SpecifyNeedToBeMade.IsVisible = false;
      _obj.State.Properties.Person.IsVisible = false;
      _obj.State.Properties.FIOPerson.IsVisible = false;
      _obj.State.Properties.DateOfBirth.IsVisible = false;
      _obj.State.Properties.DatabookActionType.IsVisible = false;
      _obj.State.Properties.ResponsibleEmployee.IsVisible = false;
      _obj.State.Properties.BusinessUnit.IsVisible = false;
      _obj.State.Properties.ResponsibleByCounterparty.IsVisible = false;
      _obj.State.Properties.Comment.IsVisible = false;
      
      if (_obj.CreateCompanyTask == null)
        return;
      
      if (_obj.CreateCompanyTask.TypeObject == CreateCompanyTask.TypeObject.Counterparty && _obj.CreateCompanyTask.TypeRequest == CreateCompanyTask.TypeRequest.NewEntry)
      {
        _obj.State.Properties.CompanyName.IsVisible = true;
        _obj.State.Properties.TIN.IsVisible = true;
        _obj.State.Properties.Comment.IsVisible = true;
      }
      
      if (_obj.CreateCompanyTask.TypeObject == CreateCompanyTask.TypeObject.Counterparty && _obj.CreateCompanyTask.TypeRequest == CreateCompanyTask.TypeRequest.EditEntry)
      {
        _obj.State.Properties.Company.IsVisible = true;
        _obj.State.Properties.TIN.IsVisible = true;
        _obj.State.Properties.SpecifyNeedToBeMade.IsVisible = true;
      }
      
      if (_obj.CreateCompanyTask.TypeObject == CreateCompanyTask.TypeObject.Person && _obj.CreateCompanyTask.TypeRequest == CreateCompanyTask.TypeRequest.NewEntry)
      {
        _obj.State.Properties.FIOPerson.IsVisible = true;
        _obj.State.Properties.DateOfBirth.IsVisible = true;
        _obj.State.Properties.Comment.IsVisible = true;
      }
      
      if (_obj.CreateCompanyTask.TypeObject == CreateCompanyTask.TypeObject.Person && _obj.CreateCompanyTask.TypeRequest == CreateCompanyTask.TypeRequest.EditEntry)
      {
        _obj.State.Properties.Person.IsVisible = true;
        _obj.State.Properties.SpecifyNeedToBeMade.IsVisible = true;
      }
      
      if (_obj.CreateCompanyTask.TypeRequest == avis.EtalonParties.CreateCompanyTask.TypeRequest.ResponsibleByCounterparty)
      {
        e.Instruction = avis.EtalonParties.ApproveCounterpartyAssignments.Resources.ResponsibleByCounterpartyInstruction;
        _obj.State.Properties.DatabookActionType.IsVisible = true;
        
        if (_obj.CreateCompanyTask.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.NewEntry)
        {
          if (_obj.CreateCompanyTask.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty)
            _obj.State.Properties.Company.IsVisible = true;
          if (_obj.CreateCompanyTask.TypeObject == avis.EtalonParties.CreateCompanyTask.TypeObject.Person)
            _obj.State.Properties.Person.IsVisible = true;
          
          _obj.State.Properties.ResponsibleEmployee.IsVisible = true;
          _obj.State.Properties.BusinessUnit.IsVisible = true;
          _obj.State.Properties.Comment.IsVisible = true;
        }
        
        if (_obj.CreateCompanyTask.DatabookActionType == avis.EtalonParties.CreateCompanyTask.DatabookActionType.EditEntry)
        {
          _obj.State.Properties.ResponsibleByCounterparty.IsVisible = true;
          _obj.State.Properties.SpecifyNeedToBeMade.IsVisible = true;
        }
      }
    }
  }
  
  // Конец добавлено avis.
}