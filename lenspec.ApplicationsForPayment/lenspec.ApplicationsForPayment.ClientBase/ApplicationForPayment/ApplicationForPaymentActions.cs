using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForPayment.ApplicationForPayment;

namespace lenspec.ApplicationsForPayment.Client
{
  partial class ApplicationForPaymentActions
  {
    public virtual void Export1C(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var lockInfo = Locks.GetLockInfo(_obj);
      if (lockInfo.IsLockedByOther)
      {
        ApplicationsForPayment.PublicFunctions.ApplicationForPayment.GetLockErrorMessage(_obj);
        return;
      }
      
      var errors = new List<string>();
      
      if (_obj.Counterparty != null && _obj.CounterpartyBankDetail != null &&
          (string.IsNullOrEmpty(_obj.CounterpartyBankDetail.CorrespondentAccount) || string.IsNullOrWhiteSpace(_obj.CounterpartyBankDetail.CorrespondentAccount)))
        errors.Add(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.CounterpartyAccountIsEmpty);
      
      var thirdSide = _obj.ThirdSide;
      if (thirdSide != null && _obj.ThirdSideBankDetail != null &&
          (string.IsNullOrEmpty(_obj.ThirdSideBankDetail.CorrespondentAccount) || string.IsNullOrWhiteSpace(_obj.ThirdSideBankDetail.CorrespondentAccount)))
        errors.Add(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.ThirdSideAccountIsEmpty);
      
      var errorMessage = Functions.ApplicationForPayment.UnloadMissingDataByCounterparty(_obj);
      if (!string.IsNullOrEmpty(errorMessage))
        errors.Add(errorMessage);
      
      if (errors.Any())
      {
        e.AddError(string.Join(Environment.NewLine, errors));
        return;
      }
      
      PublicFunctions.ApplicationForPayment.Export1C(_obj);
    }

    public virtual bool CanExport1C(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var includeInRole = Users.Current.IncludedIn(Roles.Administrators) || Users.Current.IncludedIn(lenspec.ApplicationsForPayment.Constants.Module.UploadApplicationForPaymentRoleGuid);
      return !_obj.State.IsInserted && !_obj.State.IsChanged && includeInRole &&
        _obj.LifeCycleState == lenspec.ApplicationsForPayment.ApplicationForPayment.LifeCycleState.Active &&
        _obj.InternalApprovalState == lenspec.ApplicationsForPayment.ApplicationForPayment.InternalApprovalState.Approved;
    }

    public virtual void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.ApplicationForPayment.Remote.GetDuplicates(_obj);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(Sungero.Commons.Resources.DuplicateNotFound);
    }

    public virtual bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var hasErrors = false;
      
      if (_obj.PaymentType == ApplicationForPayment.PaymentType.Refunds &&
          _obj.CustomerRequest == null && _obj.IncomingLetter == null && _obj.Memo == null && _obj.Contract == null)
      {
        e.AddError(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.NeedFillBasisDocumentForRefunds);
        hasErrors = true;
      }
      
      if (_obj.PaymentType == ApplicationForPayment.PaymentType.Postpay &&
          (_obj.Contract == null || !_obj.ContractStatements.Any() && !_obj.UTDs.Any() && !_obj.Waybills.Any()))
      {
        e.AddError(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.NeedFillBasisDocumentForPostpay);
        hasErrors = true;
      }
      
      if (_obj.PaymentType == ApplicationForPayment.PaymentType.Regulatory &&
          _obj.SimpleDocument == null && _obj.Memo == null)
      {
        e.AddError(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.NeedFillBasisDocumentForRegulary);
        hasErrors = true;
      }
      
      if (_obj.PaymentType == ApplicationForPayment.PaymentType.Sanction &&
          _obj.IncomingLetter == null && _obj.Memo == null)
      {
        e.AddError(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.NeedFillBasisDocumentForSanction);
        hasErrors = true;
      }
      
      if (_obj.Counterparty != null)
      {
        var counterparty = lenspec.Etalon.Counterparties.As(_obj.Counterparty);
        if (counterparty.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.NotAssessed ||
            counterparty.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopNotRecomend)
        {
          e.AddError(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.NeedApproveCounterparty);
          hasErrors = true;
        }
      }
      
      if (_obj.ThirdSide != null)
      {
        var counterparty = lenspec.Etalon.Counterparties.As(_obj.ThirdSide);
        if (counterparty.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.NotAssessed ||
            counterparty.ResultApprovalDEBavis == lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopNotRecomend)
        {
          e.AddError(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.NeedApproveThirdSide);
          hasErrors = true;
        }
      }
      
      if (hasErrors)
        return;
      
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

  }

}