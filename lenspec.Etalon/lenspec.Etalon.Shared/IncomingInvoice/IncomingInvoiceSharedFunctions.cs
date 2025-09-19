using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingInvoice;

namespace lenspec.Etalon.Shared
{
  partial class IncomingInvoiceFunctions
  {

    /// <summary>
    /// Установить обязательность свойств в визуальном режиме в зависимости от заполненных данных.
    /// </summary>
    public virtual void SetRequiredPropertiesOnVisualMode()
    {      
      // Вид носителя = Электронный.
      var isElectronicMediumType = _obj.Medium != null && _obj.Medium.Sid == Sungero.Docflow.PublicConstants.MediumType.ElectronicMediumTypeSid;
      _obj.State.Properties.VatRate.IsRequired = !isElectronicMediumType;
    }
    
    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
    }
  }
}