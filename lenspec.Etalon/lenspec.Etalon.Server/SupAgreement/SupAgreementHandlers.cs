using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.SupAgreement;

namespace lenspec.Etalon
{
  partial class SupAgreementLeadingDocumentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> LeadingDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.LeadingDocumentFiltering(query, e);
      
      query = query.Where(q => lenspec.Etalon.Contracts.Is(q) &&
                          q.InternalApprovalState != null &&
                          q.InternalApprovalState.Value == Sungero.Docflow.OfficialDocument.InternalApprovalState.Signed);
      
      if (_obj.BusinessUnit != null)
        query = query.Where(q => q.BusinessUnit == _obj.BusinessUnit);
      
      if (_obj.Counterparty != null)
        query = query.Where(q => q.Counterparty == _obj.Counterparty);
      
      return query;
    }
  }

  partial class SupAgreementServerHandlers
  {

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      //Functions.SupAgreement.CalculateRemainingAmountLeadingContract(_obj);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      PublicFunctions.SupAgreement.TerminatedLeadingContract(_obj);
    }

    /// <summary>
    /// ��������.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsTerminationavis = false;
      _obj.TotalAmount = 0;
      
      _obj.LeadingDocumentCommentavis = lenspec.Etalon.SupAgreements.Resources.LeadingDocumentComment;
    }
  }

}