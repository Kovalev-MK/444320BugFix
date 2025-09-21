using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingInvoice;

namespace lenspec.Etalon
{
  partial class IncomingInvoiceSetLenspeclenspecDocumentPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> SetLenspeclenspecDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => Sungero.FinancialArchive.ContractStatements.Is(x) ||
                          Sungero.FinancialArchive.Waybills.Is(x) ||
                          Sungero.FinancialArchive.UniversalTransferDocuments.Is(x));
      
      if (_root.Counterparty != null)
        query = query.Where(x => x.Counterparty == _root.Counterparty);
      
      return query;
    }
  }

  partial class IncomingInvoiceSetOfDocumentslenspecPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> SetOfDocumentslenspecFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query;
    }
  }

  partial class IncomingInvoiceServerHandlers
  {

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      foreach(var line in _obj.SetLenspeclenspec)
      {
        if (line.Document != null && line.Document.AccessRights.CanRead() &&
            !_obj.Relations.GetRelatedFromDocuments(Sungero.Docflow.PublicConstants.Module.SimpleRelationName).Any(x => Equals(x, line.Document)))
          _obj.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, line.State.Properties.Document.OriginalValue, line.Document);
      }
    }
  }

}