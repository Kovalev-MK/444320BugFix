using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ApplicationsForPayment.ApplicationForPayment;

namespace lenspec.ApplicationsForPayment
{
  partial class ApplicationForPaymentClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      var isUploadApplicationRole = Users.Current.IncludedIn(Constants.Module.UploadApplicationForPaymentRoleGuid);
      e.Params.AddOrUpdate(Constants.Module.Params.IsUploadApplicationRole, isUploadApplicationRole);
    }

    public override void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      base.TotalAmountValueInput(e);
      
      if (e.NewValue < 0)
        e.AddError(Sungero.Docflow.Resources.TotalAmountMustBePositive);
    }

    public virtual void PlannedPaymentDateValueInput(Sungero.Presentation.DateTimeValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue.Value < Calendar.Today)
      {
        e.AddError(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.PlannedDateMustBeGreaterCurrentate);
        return;
      }
    }

    public virtual void UINValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.NewValue) && !System.Text.RegularExpressions.Regex.IsMatch(e.NewValue, @"^\d*$"))
      {
        e.AddError(lenspec.ApplicationsForPayment.ApplicationForPayments.Resources.UINMustConsistOfNumbersOnly);
        return;
      }
    }

    public virtual IEnumerable<Enumeration> PaymentTypeFiltering(IEnumerable<Enumeration> query)
    {
      if (_obj.Category == lenspec.ApplicationsForPayment.ApplicationForPayment.Category.Standard)
        query = query.Where(x => x == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Postpay || x == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Prepayment ||
                            x == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.NoContract || x == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Regulatory);
      else if (_obj.Category == lenspec.ApplicationsForPayment.ApplicationForPayment.Category.ForClients)
        query = query.Where(x => x == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Refunds ||
                            x == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.TransitPayment ||
                            x == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.SubsidyDownPaym);
      else if (_obj.Category == lenspec.ApplicationsForPayment.ApplicationForPayment.Category.ForBailiffs)
        query = query.Where(x => x == lenspec.ApplicationsForPayment.ApplicationForPayment.PaymentType.Sanction);
      
      return query;
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var isBusinessUnitMain = false;
      e.Params.TryGetValue(Constants.ApplicationForPayment.IsBusinessUnitMainParam, out isBusinessUnitMain);
      _obj.State.Properties.DecodingBudgetItem.IsRequired = isBusinessUnitMain;
      _obj.State.Properties.Directorate.IsEnabled = isBusinessUnitMain;
      _obj.State.Properties.Directorate.IsRequired = isBusinessUnitMain;
      _obj.State.Properties.DirectorateRegion.IsEnabled = isBusinessUnitMain;
      _obj.State.Properties.DirectorateRegion.IsRequired = isBusinessUnitMain;
      _obj.State.Properties.DepartmentByDirectorate.IsEnabled = isBusinessUnitMain;
      _obj.State.Properties.DepartmentByDirectorate.IsRequired = isBusinessUnitMain;
      
      var message = string.Empty;
      e.Params.TryGetValue(Constants.ApplicationForPayment.IsSafeCounterpartyParam, out message);
      if (!string.IsNullOrEmpty(message))
        e.AddError(message);
      
      message = string.Empty;
      e.Params.TryGetValue(Constants.ApplicationForPayment.IsSignedContractParam, out message);
      if (!string.IsNullOrEmpty(message))
        e.AddError(message);
      
      message = string.Empty;
      e.Params.TryGetValue(Constants.ApplicationForPayment.IsSafeThirdSideParam, out message);
      if (!string.IsNullOrEmpty(message))
        e.AddError(message);
      
      message = string.Empty;
      e.Params.TryGetValue(Constants.ApplicationForPayment.IsCounterpartyWithEmptyFields, out message);
      if (!string.IsNullOrEmpty(message))
      {
        if (Etalon.People.Is(_obj.Counterparty))
          e.AddWarning(message);
        else
          e.AddError(message);
      }
      
      message = string.Empty;
      e.Params.TryGetValue(Constants.ApplicationForPayment.IsThirdSideWithEmptyFields, out message);
      if (!string.IsNullOrEmpty(message))
      {
        if (Etalon.People.Is(_obj.ThirdSide))
          e.AddWarning(message);
        else
          e.AddError(message);
      }

    }

  }
}