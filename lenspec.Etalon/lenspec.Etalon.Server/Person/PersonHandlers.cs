using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Person;

namespace lenspec.Etalon
{
  // Добавлено avis.
  partial class PersonServerHandlers
  {

    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      // выставляем false на чекбоксы.
      _obj.IsLawyeravis = false;
      _obj.IsClientBuyersavis = false;
      _obj.IsClientOwnersavis = false;
      _obj.IsEmployeeGKavis = false;
      _obj.IsOtheravis = false;
      _obj.IsPersonalDataavis = false;
      
      if (_obj.State.IsCopied)
      {
        _obj.ExternalCodeavis = null;
        _obj.CodeInvestavis = null;
      }
    }
    
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (!string.IsNullOrEmpty(_obj.MiddleName))
      {
        _obj.MiddleName = _obj.MiddleName.Trim();
        if (!System.Text.RegularExpressions.Regex.IsMatch(_obj.MiddleName, "^[A-Za-zА-я]"))
          _obj.MiddleName = null;
      }
      
      // Заполняем почтовый адрес, если он не был заполнен инвестом.
      if (string.IsNullOrEmpty(_obj.PostalAddress))
        _obj.PostalAddress = _obj.LegalAddress;
      
      base.BeforeSave(e);
      
      //Обязательно для заполнения хотя бы одно из 5 значений: Адвокаты/нотариусы/совет дома, Клиенты (покупатели, дольщики), Клиенты (собственники), Сотрудник ГК, Прочие
      if (_obj.IsLawyeravis != true && _obj.IsClientBuyersavis != true &&  _obj.IsClientOwnersavis != true && _obj.IsEmployeeGKavis != true && _obj.IsOtheravis != true)
        e.AddError(lenspec.Etalon.People.Resources.RequiredFillIn);
    }
    
  }
  // Конец добавлено avis.
}