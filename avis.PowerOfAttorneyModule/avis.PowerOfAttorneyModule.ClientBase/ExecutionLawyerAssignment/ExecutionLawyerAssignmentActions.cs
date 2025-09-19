using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ExecutionLawyerAssignment;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class ExecutionLawyerAssignmentActions
  {
    public virtual void Forward(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if(_obj.Forward == null)
      {
        e.AddError(avis.PowerOfAttorneyModule.ExecutionLawyerAssignments.Resources.ErrorMessageForwardNullForwardAction);
        e.Cancel();
      }
      if(_obj.Forward == _obj.Performer)
      {
        e.AddError(avis.PowerOfAttorneyModule.ExecutionLawyerAssignments.Resources.ErrorMessageForwardPerformerForwardAction);
        e.Cancel();
      }
    }

    public virtual bool CanForward(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Reject(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if(string.IsNullOrEmpty(_obj.ActiveText))
      {
        e.AddError(avis.PowerOfAttorneyModule.ApprovalManagerInitiatorAssignments.Resources.ErrorMessageCencelAction);
      }
    }

    public virtual bool CanReject(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }


    public virtual void Rework(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if(string.IsNullOrEmpty(_obj.ActiveText))
      {
        e.AddError(avis.PowerOfAttorneyModule.ApprovalManagerInitiatorAssignments.Resources.ErrorMessageReworkAction);
        return;
      }
    }

    public virtual bool CanRework(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void CreatePowerOfAttorneys(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.ExecutionLawyerAssignment.RefillPowerOffAttorney(_obj);
      var document = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      var businessUnits = document.OurBusinessUavis.Select(i => i.Company);
      foreach(var company in businessUnits)
      {
        var powerOfAttorney = lenspec.Etalon.PowerOfAttorneys.Copy(document);
        powerOfAttorney.DocumentKind = document.DocumentKind;
        powerOfAttorney.IsProjectPOAavis = false;
        powerOfAttorney.BusinessUnit = Sungero.Company.BusinessUnits.As(company);
        powerOfAttorney.OurSignatory = powerOfAttorney.BusinessUnit.CEO != null
          ? lenspec.Etalon.Employees.As(powerOfAttorney.BusinessUnit.CEO)
          : null;
        powerOfAttorney.ValidTill = document.ValidTill;
        powerOfAttorney.Save();
        _obj.POAs.PowerOfAttorneys.Add(powerOfAttorney);
      }
      _obj.Save();
    }

    public virtual bool CanCreatePowerOfAttorneys(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ReviewPowerOfAttorney(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var document = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      var businessUnits = document.OurBusinessUavis.Select(i => i.Company);
      var powerOfAttorneyList = Sungero.Docflow.PowerOfAttorneys.GetAll(d => businessUnits.Contains(d.BusinessUnit));
      
      if(powerOfAttorneyList.Any())
        powerOfAttorneyList.Show();
      else
        Dialogs.NotifyMessage(avis.PowerOfAttorneyModule.ExecutionLawyerAssignments.Resources.NotifyMessageRewiewPowerOfAttorneyAction);
    }

    public virtual bool CanReviewPowerOfAttorney(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      if (!_obj.POAs.All.Any())
      {
        e.AddError(avis.PowerOfAttorneyModule.ExecutionLawyerAssignments.Resources.ErrorMessageEmptyPoas);
        return;
      }
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}