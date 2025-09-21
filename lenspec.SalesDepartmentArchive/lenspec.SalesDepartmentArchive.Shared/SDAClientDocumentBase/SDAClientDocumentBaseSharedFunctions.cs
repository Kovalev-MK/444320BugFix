using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientDocumentBase;

namespace lenspec.SalesDepartmentArchive.Shared
{
  partial class SDAClientDocumentBaseFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      _obj.State.Properties.Subject.IsRequired = false;
    }
    //конец Добавлено Avis Expert
  }
}