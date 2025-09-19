using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorney;

namespace lenspec.Etalon
{
  partial class PowerOfAttorneyDocumentKindPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> DocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.DocumentKindFiltering(query, e);
      
      var isInRoleOfficeGK = false;
      e.Params.TryGetValue(lenspec.Etalon.Constants.Docflow.PowerOfAttorney.Params.IsInRoleOfficeGK, out isInRoleOfficeGK);
      if (isInRoleOfficeGK)
        return query;
      
      var poaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Sungero.Docflow.PublicConstants.Module.Initialize.PowerOfAttorneyKind);
      var nPoaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.DocumentNotarialKindGuid);
      query = query.Where(x => !Equals(poaKind, x) && !Equals(nPoaKind, x));
      return query;
    }
  }

  partial class PowerOfAttorneyContractKindslenspecContractKindPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> ContractKindslenspecContractKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_root.ContractKindGroupslenspec.Any())
      {
        var kindGroups = _root.ContractKindGroupslenspec.Select(x => x.GroupContractType);
        query = query.Where(x => kindGroups.Contains(x.GroupContractType));
      }
      return query;
    }
  }

  partial class PowerOfAttorneyDocKindsavisKindPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> DocKindsavisKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => !Equals(x.DocumentType.DocumentTypeGuid, "be859f9b-7a04-4f07-82bc-441352bce627"));
      return query;
    }
  }

  partial class PowerOfAttorneyContractCategoryavisCategoryPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> ContractCategoryavisCategoryFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_root.ContractKindGroupslenspec.Any())
      {
        var kindGroups = _root.ContractKindGroupslenspec.Select(x => x.GroupContractType);
        query = query.Where(x => kindGroups.Contains(lenspec.Etalon.ContractCategories.As(x).GroupContractTypeavis));
      }
      if (_root.ContractKindslenspec.Any())
      {
        var kinds = _root.ContractKindslenspec.Select(x => x.ContractKind);
        query = query.Where(x => kinds.Contains(lenspec.Etalon.ContractCategories.As(x).ContractKindavis));
      }
      return query;
    }
  }


  partial class PowerOfAttorneyServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      long businessUnitId;
      var businessUnit = Sungero.Company.BusinessUnits.Null;
      if (_obj.BusinessUnit != null)
        businessUnit = _obj.BusinessUnit;
      else if (e.Params.TryGetValue("BusinessUnitId", out businessUnitId))
        businessUnit = Sungero.Company.BusinessUnits.Get(businessUnitId);
      
      base.BeforeSave(e);
      // Проверить совпадение делопроизводителей
      if (!Functions.PowerOfAttorney.CheckResponsibleKindRoles(_obj))
      {
        e.AddError(lenspec.Etalon.PowerOfAttorneys.Resources.ErrorMessageDifferentResponsible);
        return;
      }
      
      // Проверить заполненность полномочий
      if (string.IsNullOrEmpty(_obj.FreePowerslenspec) && !_obj.PowerListlenspec.Any())
      {
        e.AddError(lenspec.Etalon.PowerOfAttorneys.Resources.ErrorMessageEmptyPowers);
        return;
      }
      
      _obj.BusinessUnit = businessUnit;
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.IsRevokedavis = false;
      _obj.IsProjectPOAavis = false;
      _obj.DateAbortLablelenspec = lenspec.Etalon.PowerOfAttorneys.Resources.DateAbortLableData;
    }
    //<<Avis-Expert
  }

}