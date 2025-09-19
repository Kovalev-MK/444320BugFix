using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.GroupCounterparty;

namespace avis.EtalonParties
{
  // Добавлено avis.
  partial class GroupCounterpartyServerHandlers
  {
    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsTINRequired = false;
      _obj.IsTRRCRequired = false;
      _obj.IsNonresidentRequired = false;
      _obj.IsLegalNameRequired = false;
    }
    
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Проверка уникальности поля ид директум5, и разрешение сохранять запись без ид директум5.
      if (_obj.IdDirectum5 == null)
        return;
      
      if (EtalonParties.GroupCounterparties.GetAll(g => g.IdDirectum5 == _obj.IdDirectum5 && g.Id != _obj.Id).Any())
      {
        e.AddError(avis.EtalonParties.CategoryCounterparties.Resources.IdDirectum5ErrorMessage);
        return;
      }
      
      if (EtalonParties.GroupCounterparties.GetAll(g => g.Name == _obj.Name && g.Id != _obj.Id).Any())
      {
        e.AddError(avis.EtalonParties.CategoryCounterparties.Resources.NameErrorMessage);
        return;
      }
    }
  }
  // Конец добавлено avis.
}