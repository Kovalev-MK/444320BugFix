using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonContracts.ContractType;

namespace avis.EtalonContracts.Server
{
  partial class ContractTypeFunctions
  {
    /// <summary>
    /// Получить тип договора по коду константы.
    /// </summary>
    /// <param name="constantCode">Код константы.</param>
    /// <returns>Найденный тип договора.</returns>
    [Public, Remote(IsPure = true)]
    public static IContractType GetContractTypeByConstantCode(string constantCode)
    {
      var constantValue = lenspec.EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(constantCode);
      
      long contractTypeId = 0;
      if (long.TryParse(constantValue, out contractTypeId))
        return ContractTypes.GetAll(x => x.Id == contractTypeId).FirstOrDefault();
      
      return null;
    }
  }
}