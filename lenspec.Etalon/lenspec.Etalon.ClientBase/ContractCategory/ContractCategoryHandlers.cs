using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractCategory;

namespace lenspec.Etalon
{
  partial class ContractCategoryClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Устанавливаем доступность поля сумма.
      Functions.ContractCategory.CheckSumRequired(_obj);
    }

  }
}