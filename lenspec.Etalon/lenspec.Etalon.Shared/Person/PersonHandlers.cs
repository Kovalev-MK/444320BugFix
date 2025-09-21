using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Person;

namespace lenspec.Etalon
{
  // Добавлено avis.
  partial class PersonSharedHandlers
  {

    public override void SexChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      base.SexChanged(e);
      if (e.NewValue != null)
      {
        // Устанавливаем поле "обращение" в зависимости от пола.
        if (e.NewValue == Person.Sex.Male)
          _obj.Appealavis = "ый";
        
        if (e.NewValue == Person.Sex.Female)
          _obj.Appealavis = "ая";
      }
      else
      {
        _obj.Appealavis = null;
      }
    }
    private void UpdateForm()
    {
      #region Устаревшее
      /*
      // Блокируем все поля.
      Etalon.PublicFunctions.Person.BlockAllProperties(_obj);
      // Разрешаем редактирование полей для роли "Права на создание всех контрагентов".
      var createCounterpartyRoleGuid = avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid;
      var createCounterpartyRole = Roles.GetAll(r => Equals(r.Sid, createCounterpartyRoleGuid)).FirstOrDefault();
      Etalon.PublicFunctions.Person.IsCounterpartyRole(_obj, createCounterpartyRole);
      */
      #endregion
    }

    public virtual void IsOtheravisChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      Functions.Person.SetEnabledAndRequiredProperties(_obj);
    }

    public virtual void IsEmployeeGKavisChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      Functions.Person.SetEnabledAndRequiredProperties(_obj);
    }

    public virtual void IsClientOwnersavisChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      Functions.Person.SetEnabledAndRequiredProperties(_obj);
    }

    public virtual void IsClientBuyersavisChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      Functions.Person.SetEnabledAndRequiredProperties(_obj);
    }

    public virtual void IsLawyeravisChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      Functions.Person.SetEnabledAndRequiredProperties(_obj);
    }
  }

  // Конец добавлено avis.
}