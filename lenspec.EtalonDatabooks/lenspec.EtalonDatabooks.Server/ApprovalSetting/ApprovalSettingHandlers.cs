using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApprovalSetting;

namespace lenspec.EtalonDatabooks
{
  partial class ApprovalSettingContractKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContractKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.GroupKind != null)
        query = query.Where(x => Equals(x.GroupContractType, _obj.GroupKind));
      
      return query;
    }
  }

  partial class ApprovalSettingGroupKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> GroupKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.ContractKind != null)
        query = query.Where(x => Equals(x, _obj.ContractKind.GroupContractType));
      
      return query;
    }
  }

  partial class ApprovalSettingCategoryPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CategoryFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query;
    }
  }

  partial class ApprovalSettingServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var duplicates = Functions.ApprovalSetting.GetDuplicates(_obj);
      if (duplicates.Any())
      {
        var contractCategories = new List<string>();
        foreach (var item in _obj.ContractCategories)
        {
          if (duplicates.Any(x => x.ContractCategories.Any(c => Equals(item.ContractCategory, c.ContractCategory))))
            contractCategories.Add(item.ContractCategory?.Name);
        }
        
        e.AddError(lenspec.EtalonDatabooks.ApprovalSettings.Resources.DuplicatesDetectedFormat(contractCategories.Any()
                                                                                               ? lenspec.EtalonDatabooks.ApprovalSettings.Resources.DuplicateContractCategoriesFormat(string.Join("; ", contractCategories))
                                                                                               : string.Empty),
                   _obj.Info.Actions.ShowDuplicates);
        return;
      }
      
      if (_obj.ContractCategories != null && _obj.ContractCategories.Any())
      {
        var contractCategories = string.Join(", ", _obj.ContractCategories.Where(x => x.ContractCategory != null).Select(x => x.ContractCategory.Name).ToList());
        if (contractCategories.Length > _obj.Info.Properties.ContractCategoriesNames.Length)
          _obj.ContractCategoriesNames = contractCategories.Substring(0, _obj.Info.Properties.ContractCategoriesNames.Length);
        else
          _obj.ContractCategoriesNames = contractCategories;
      }
      else
        _obj.ContractCategoriesNames = string.Empty;
    }
  }

}