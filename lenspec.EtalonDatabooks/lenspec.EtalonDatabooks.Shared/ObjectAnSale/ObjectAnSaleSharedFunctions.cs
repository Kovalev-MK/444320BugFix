using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnSale;

namespace lenspec.EtalonDatabooks.Shared
{
  partial class ObjectAnSaleFunctions
  {

    /// <summary>
    /// Заполнить имя записи.
    /// </summary>
    [Public]
    public virtual void FillName()
    {
      _obj.Name = string.Empty;
      if (!string.IsNullOrEmpty(_obj.NumberRoomMail))
      {
        _obj.Name = $"Помещение №{_obj.NumberRoomMail}";
      }
      if (_obj.PurposeOfPremises != null)
      {
        _obj.Name += $", назначение {_obj.PurposeOfPremises.Name}";
      }
      if (_obj.ObjectAnProject != null)
      {
        _obj.Name += $", объект {_obj.ObjectAnProject.Name}";
      }
    }
  }
}