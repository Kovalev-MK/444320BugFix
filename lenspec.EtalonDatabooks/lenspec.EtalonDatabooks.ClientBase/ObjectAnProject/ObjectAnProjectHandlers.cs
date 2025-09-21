using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnProject;

namespace lenspec.EtalonDatabooks
{
  partial class ObjectAnProjectClientHandlers
  {
    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      // Измененить доступность свойств "Разрешение на стройстельство" и "Разрешение на ввод".
      Functions.ObjectAnProject.Remote.PermitPropertiesIsEnable(_obj);
      
      // Далаем доступным поле "Договор управления (Девелопмент)".
      if (_obj.DevelopmentCompany == null)
        _obj.State.Properties.DevelopmentContract.IsEnabled = false;
      else
        _obj.State.Properties.DevelopmentContract.IsEnabled = true;
      
      _obj.State.Properties.OurCF.IsRequired = true;
    }
  }
}