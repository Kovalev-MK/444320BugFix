using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnProject;

namespace lenspec.EtalonDatabooks.Server
{
  // Добавлено avis.
  
  partial class ObjectAnProjectFunctions
  {
    /// <summary>
    /// Измененить доступность свойств "Разрешение на стройстельство" и "Разрешение на ввод".
    /// </summary>
    [Remote]
    public void PermitPropertiesIsEnable()
    {
      if (_obj.SpecDeveloper == null)
      {
        // Если не выбран спец застройщика делаем поля недоступными для редактирования.
        _obj.State.Properties.BuildingPermit.IsEnabled = false;
        _obj.State.Properties.EnterAnObjectPermit.IsEnabled = false;
        return;
      }
      
      // После выбора спец застройщика делаем поля доступными для редактирования.
      _obj.State.Properties.BuildingPermit.IsEnabled = true;
      _obj.State.Properties.EnterAnObjectPermit.IsEnabled = true;
    }
  }
  
  // Конец добавлено avis.
}