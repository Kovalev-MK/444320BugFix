using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnSale;

namespace lenspec.EtalonDatabooks
{
  partial class ObjectAnSaleSharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void PurposeOfPremisesChanged(lenspec.EtalonDatabooks.Shared.ObjectAnSalePurposeOfPremisesChangedEventArgs e)
    {
      lenspec.EtalonDatabooks.PublicFunctions.ObjectAnSale.FillAddress(_obj);
      Functions.ObjectAnSale.FillName(_obj);
    }

    public virtual void EditPriceChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.SurchargeAmount = null;
        _obj.RefundAmount = null;
        return;
      }
      
      var amountAbsoluteValue = Math.Abs(e.NewValue.Value);
      if (e.NewValue.Value >= 0)
      {
        _obj.SurchargeAmount = amountAbsoluteValue;
        _obj.RefundAmount = null;
      }
      else
      {
        _obj.RefundAmount = amountAbsoluteValue;
        _obj.SurchargeAmount = null;
      }
      var currency = Sungero.Commons.Currencies.GetAll(x => x.NumericCode.Equals("643")).FirstOrDefault();
      _obj.AmountInWords = EtalonDatabooks.PublicFunctions.Module.AmountInCurrencyToWords(amountAbsoluteValue, currency);
    }

    public virtual void EditSquereChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.ZoomSizeS = null;
        _obj.ReductionSizeS = null;
        return;
      }
      
      if (e.NewValue.Value >= 0)
      {
        _obj.ZoomSizeS = Math.Abs(e.NewValue.Value);
        _obj.ReductionSizeS = null;
      }
      else
      {
        _obj.ReductionSizeS = Math.Abs(e.NewValue.Value);
        _obj.ZoomSizeS = null;
      }
    }
    
    /// <summary>
    /// Изменение свойства "№ помещения".
    /// </summary>
    /// <param name="e"></param>
    public virtual void NumberRoomMailChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      lenspec.EtalonDatabooks.PublicFunctions.ObjectAnSale.FillAddress(_obj);
      Functions.ObjectAnSale.FillName(_obj);
    }
    
    /// <summary>
    /// Изменение свойства "Объект".
    /// </summary>
    /// <param name="e"></param>
    public virtual void ObjectAnProjectChanged(lenspec.EtalonDatabooks.Shared.ObjectAnSaleObjectAnProjectChangedEventArgs e)
    {
      // Заполняем ИСП из объекта проекта.
      _obj.OurCF = e.NewValue?.OurCF;
      
      lenspec.EtalonDatabooks.PublicFunctions.ObjectAnSale.FillAddress(_obj);
      Functions.ObjectAnSale.FillName(_obj);
    }
    //конец Добавлено Avis Expert
  }
}