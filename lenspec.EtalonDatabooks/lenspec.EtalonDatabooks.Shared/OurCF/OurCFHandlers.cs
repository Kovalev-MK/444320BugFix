using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.OurCF;

namespace lenspec.EtalonDatabooks
{


  partial class OurCFSharedHandlers
  {
    
    /// <summary>
    /// Изменение значения "Коммерческое наименование проекта".
    /// </summary>
    /// <param name="e"></param>
    public virtual void CommercialNameChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }

    public virtual void DevelopmentChanged(lenspec.EtalonDatabooks.Shared.OurCFDevelopmentChangedEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.State.Properties.AgrDevelopment.IsRequired = false;
        _obj.State.Properties.AgrDevelopment.IsEnabled = false;
        _obj.AgrDevelopment = null;
        return;
      }
      
      _obj.State.Properties.AgrDevelopment.IsRequired = true;
      _obj.State.Properties.AgrDevelopment.IsEnabled = true;
    }
    
    // Конец добавлено avis.
  }

}