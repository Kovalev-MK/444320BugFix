using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.BusinessUnit;

namespace lenspec.Etalon
{
  partial class BusinessUnitServerHandlers
  {
    // Добавлено avis
    
    // До сохранения в (транзакции)
    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      base.Saving(e);
      
      var company = lenspec.Etalon.Companies.As(_obj.Company);
      company.GroupCounterpartyavis = avis.EtalonParties.GroupCounterparties.GetAll(g => g.IdDirectum5 == 6).FirstOrDefault();
      company.CategoryCounterpartyavis = avis.EtalonParties.CategoryCounterparties.GetAll(c => c.IdDirectum5 == 7).FirstOrDefault();
    }
    
    // Конец добавлено avis
  }
}