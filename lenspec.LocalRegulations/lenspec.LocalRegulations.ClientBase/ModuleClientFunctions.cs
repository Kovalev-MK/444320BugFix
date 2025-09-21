using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.LocalRegulations.Client
{
  public class ModuleFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Создать приказ.
    /// </summary>
    [LocalizeFunction("CreateOrder_ResourceKey", "CreateOrder_DescriptionResourceKey")]
    public virtual void CreateOrder()
    {
      LocalRegulations.PublicFunctions.Module.Remote.CreateOrder().Show();
    }
    
    /// <summary>
    /// Создать распоряжение.
    /// </summary>
    [LocalizeFunction("CreateCompanyDirective_ResourceKey", "CreateCompanyDirective_DescriptionResourceKey")]
    public virtual void CreateCompanyDirective()
    {
      LocalRegulations.PublicFunctions.Module.Remote.CreateCompanyDirective().Show();
    }
    //конец Добавлено Avis Expert
  }
}