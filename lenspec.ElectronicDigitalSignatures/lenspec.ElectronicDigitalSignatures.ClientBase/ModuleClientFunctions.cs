using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ElectronicDigitalSignatures.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Создать Заявку УКЭП на носителе.
    /// </summary>
    [LocalizeFunction("CreateEDSApplication_ResourceKey", "CreateEDSApplication_DescriptionResourceKey")]
    public virtual void CreateEDSApplication()
    {
      ElectronicDigitalSignatures.Functions.Module.Remote.CreateEDSApplication().Show();
    }
    
    /// <summary>
    /// Открыть гиперссылку Инструкция для продления УКЭП.
    /// </summary>
    [LocalizeFunction("OpenRenewalInstruction_ResourceKey", "OpenRenewalInstruction_DescriptionResourceKey")]
    public void OpenRenewalInstruction()
    {
      Functions.EDSApplication.ShowRenewalInstruction();
    }
    
    /// <summary>
    /// Открыть гиперссылку Инструкция для самостоятельного аннулирования УКЭП.
    /// </summary>
    [LocalizeFunction("OpenCancellationInstruction_ResourceKey", "OpenCancellationInstruction_DescriptionResourceKey")]
    public void OpenCancellationInstruction()
    {
      Functions.EDSApplication.ShowCancellationInstruction();
    }
    
    /// <summary>
    /// Открыть гиперссылку Инструкция для настройки УКЭП на рабочем ПК.
    /// </summary>
    [LocalizeFunction("OpenSettingInstruction_ResourceKey", "OpenSettingInstruction_DescriptionResourceKey")]
    public void OpenSettingInstruction()
    {
      Functions.EDSApplication.ShowSettingInstruction();
    }
  }
}