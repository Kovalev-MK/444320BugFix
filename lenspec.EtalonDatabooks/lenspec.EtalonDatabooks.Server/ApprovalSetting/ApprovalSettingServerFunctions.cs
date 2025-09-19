using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ApprovalSetting;

namespace lenspec.EtalonDatabooks.Server
{
  partial class ApprovalSettingFunctions
  {
    /// <summary>
    /// Получить дубли записей.
    /// </summary>
    /// <param name="name">Наименование.</param>
    /// <param name="index">Индекс.</param>
    /// <param name="excludedId">ИД для исключения.</param>
    /// <returns>Список дублей.</returns>
    [Remote(IsPure = true)]
    public IQueryable<IApprovalSetting> GetDuplicates()
    {
      var duplicates = ApprovalSettings.GetAll(x => !Equals(x, _obj)
                                               && Equals(x.GroupKind, _obj.GroupKind)
                                               && Equals(x.ContractKind, _obj.ContractKind)
                                               && Equals(x.BusinessUnit, _obj.BusinessUnit)
                                               && Equals(x.ApprovalRule, _obj.ApprovalRule));
      var duplicatesByContractCategory = new List<IApprovalSetting>();
      foreach (var item in _obj.ContractCategories)
      {
        duplicatesByContractCategory.AddRange(duplicates.Where(x => x.ContractCategories.Any(c => Equals(item.ContractCategory, c.ContractCategory))).ToList());
      }
      duplicates = duplicates.Where(x => duplicatesByContractCategory.Contains(x));
      
      return duplicates;
    }
  }
}