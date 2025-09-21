using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.CounterpartyRegisterBase;

namespace lenspec.Tenders
{
  partial class CounterpartyRegisterBaseSharedHandlers
  {

    public virtual void QCDecisionDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      Functions.CounterpartyRegisterBase.FillName(_obj);
    }

    public virtual void PresenceRegionChanged(lenspec.Tenders.Shared.CounterpartyRegisterBasePresenceRegionChangedEventArgs e)
    {
      Functions.CounterpartyRegisterBase.FillName(_obj);
    }

    public virtual void QCDecisionChanged(lenspec.Tenders.Shared.CounterpartyRegisterBaseQCDecisionChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (e.NewValue != null)
      {
        _obj.QCDecisionDate = e.NewValue.QCDecisionDate;
      }
      else
      {
        _obj.QCDecisionDate = null;
      }
    }

    public virtual void InitiatorOfInclusionInRegisterChanged(lenspec.Tenders.Shared.CounterpartyRegisterBaseInitiatorOfInclusionInRegisterChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (e.NewValue != null)
      {
        _obj.InitiatorOfInclusionJobTitle = e.NewValue.JobTitle;
        _obj.InitiatorOfInclusionDepartment = e.NewValue.Department;
      }
      else
      {
        _obj.InitiatorOfInclusionJobTitle = null;
        _obj.InitiatorOfInclusionDepartment = null;
      }
    }

    public virtual void CounterpartyChanged(lenspec.Tenders.Shared.CounterpartyRegisterBaseCounterpartyChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      if (e.NewValue != null)
      {
        _obj.TIN = e.NewValue.TIN;
        _obj.ResultApprovalDEB = e.NewValue.ResultApprovalDEBavis;
        _obj.ResponsibleDEB = e.NewValue.ResponsibleDEBavis;
        _obj.InspectionDateDEB = e.NewValue.InspectionDateDEBavis;
        _obj.ApprovalDeadline = e.NewValue.ApprovalPeriodavis;
        // Если нового КА нет в решении КК, очищаем поле.
        if (_obj.QCDecision != null && !_obj.QCDecision.Counterparties.Any(c => Equals(c.Counterparty, e.NewValue)))
          _obj.QCDecision = null;
      }
      else
      {
        _obj.TIN = null;
        _obj.ResultApprovalDEB = null;
        _obj.ResponsibleDEB = null;
        _obj.InspectionDateDEB = null;
        _obj.ApprovalDeadline = null;
        _obj.QCDecision = null;
      }
      
      Functions.CounterpartyRegisterBase.FillName(_obj);
    }

  }
}