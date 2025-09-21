using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractCategory;

namespace lenspec.Etalon
{
  partial class ContractCategorySharedHandlers
  {

    public virtual void CheckSumavisChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      // Устанавливаем доступность поля сумма.
      PublicFunctions.ContractCategory.CheckSumRequired(_obj);
    }

    public virtual void GroupContractTypeavisChanged(lenspec.Etalon.Shared.ContractCategoryGroupContractTypeavisChangedEventArgs e)
    {
      _obj.ContractKindavis = null;
      
      // ���� � ��������� ������ ������� ��� ��.
      if (_obj.GroupContractTypeavis != null && _obj.GroupContractTypeavis.RequireOurCF == avis.EtalonContracts.GroupContractType.RequireOurCF.Yes)
        _obj.IsOurCFavis = true;
      
       if (_obj.GroupContractTypeavis != null && _obj.GroupContractTypeavis.RequireOurCF == avis.EtalonContracts.GroupContractType.RequireOurCF.No)
        _obj.IsOurCFavis = false;
    }

  }
}