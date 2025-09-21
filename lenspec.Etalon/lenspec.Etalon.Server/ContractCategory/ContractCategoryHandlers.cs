using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractCategory;

namespace lenspec.Etalon
{
  partial class ContractCategoryContractKindavisPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContractKindavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.GroupContractTypeavis != null)
        query = query.Where(q => q.GroupContractType == _obj.GroupContractTypeavis);
      
      return query;
    }
  }

  partial class ContractCategoryServerHandlers
  {
    /// <summary>
    /// До сохранения.
    /// </summary>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
    }
    
    /// <summary>
    /// Создание.
    /// </summary>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsOurCFavis = false;
      _obj.IsSMRavis = false;
      _obj.IsTenderProtocolNotRequiredlenspec = false;
      
      base.Created(e);
    }
  }

}