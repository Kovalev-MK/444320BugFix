using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PostalItems.PostalItem;

namespace avis.PostalItems
{
  partial class PostalItemOutgoingLetterPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> OutgoingLetterFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Counterparty != null)
        return query.Where(x => Equals(x.Correspondent, _obj.Counterparty));
      
      return query;
    }
  }

  partial class PostalItemToPropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация поля "Кому".
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual IQueryable<T> ToFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Если Адресат пуст, или поле Кому заблокировано то стандартная фильтрация.
      if (_obj.Counterparty == null || _obj.State.Properties.To.IsEnabled == false)
        return query;
      
      // Если поле доступно и укащан Адресат, фильтруем по компании.
      query = query.Where(q => q.Company == _obj.Counterparty);
      return query;
    }
  }

  partial class PostalItemServerHandlers
  {
    // Добавлено avis.
    
    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      // Заполняем дату создания.
      _obj.DateCreated = Calendar.Now;
    }
    
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Проверяем, есть ли номер.
      if (string.IsNullOrEmpty(_obj.Number))
      {
        // Получаем очередной номер.
        _obj.Number = lenspec.EtalonDatabooks.PublicFunctions.Module.GetNextValueConstant(lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.AutonumerationAttachmentPostalItemCode).ToString();
        
        if (_obj.Number == "-1")
        {
          _obj.Number = "";
          return;
        }
        
        // Добавляем 0 в начале числа, до 5 символов.
        while (_obj.Number.Length < 5)
          _obj.Number = _obj.Number.Insert(0, "0");
        
        // Записываем номер.
        _obj.Number += $"-{Calendar.Now.ToString("yyyy")}";
      }
      
      // Заполняем название
      _obj.Name = $"Отправление №{_obj.Number} для - {_obj.Counterparty.Name}";
      
      // Заполняем данные для отчёта описи.
      var sumPages = 0;
      var sumLetters = Math.Round(0.00, 2);
      
      foreach(var attachment in _obj.AttachmentCollection)
      {
        if (attachment.ValuePages.HasValue)
          sumPages += attachment.ValuePages.Value;
        
        if (attachment.ValueLetter.HasValue)
          sumLetters += Math.Round(attachment.ValueLetter.Value, 2);
      }
      
      _obj.SumPages = sumPages;
      // Формируем ценность, что бы нули появились после запятой, сделал этот костыль.
      var specifier = "N";
      var culture = System.Globalization.CultureInfo.CreateSpecificCulture("fr-CA");
      _obj.SumLetter = sumLetters.ToString(specifier, culture);
    }
    
    // Конец добавлено avis.
  }

}