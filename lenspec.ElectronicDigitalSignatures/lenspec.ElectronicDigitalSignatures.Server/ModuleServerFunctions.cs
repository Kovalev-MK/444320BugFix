using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ElectronicDigitalSignatures.Server
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Создать Заявку УКЭП на носителе.
    /// </summary>
    /// <returns>Заявка УКЭП на носителе.</returns>
    [Remote]
    public static lenspec.ElectronicDigitalSignatures.IEDSApplication CreateEDSApplication()
    {
      return lenspec.ElectronicDigitalSignatures.EDSApplications.Create();
    }
  }
}