using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.CategoryCounterparty;

namespace avis.EtalonParties
{
  partial class CategoryCounterpartyServerHandlers
  {
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Проверка уникальности поля ид директум5, и разрешение сохранять запись без ид директум5.
      if (_obj.IdDirectum5 == null)
        return;
      
      if (EtalonParties.CategoryCounterparties.GetAll(c => c.IdDirectum5 == _obj.IdDirectum5 && c.Id != _obj.Id).Any())
      {
        e.AddError(avis.EtalonParties.CategoryCounterparties.Resources.IdDirectum5ErrorMessage);
        return;
      }
      
      if (EtalonParties.CategoryCounterparties.GetAll(c => c.Name == _obj.Name && c.Id != _obj.Id).Any())
      {
        e.AddError(avis.EtalonParties.CategoryCounterparties.Resources.NameErrorMessage);
        return;
      }
    }
  }
}