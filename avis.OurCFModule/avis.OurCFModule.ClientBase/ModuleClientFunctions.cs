using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.OurCFModule.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Создание разрешения на строительство.
    /// </summary>
    [LocalizeFunction("CreateRNS_ResourceKey", "CreateRNS_DescriptionResourceKey")]
    public virtual void CreateRNS()
    {
      lenspec.EtalonDatabooks.ProjectBuildingPermitDocuments.Create().Show();
    }
    
    /// <summary>
    /// Создание разрешения на ввод объекта.
    /// </summary>
    [LocalizeFunction("CreateRNV_ResourceKey", "CreateRNV_DescriptionResourceKey")]
    public virtual void CreateRNV()
    {
      lenspec.EtalonDatabooks.ProjectObjectPermitDocuments.Create().Show();
    }
    
    /// <summary>
    /// Список документов вида «Разрешение на строительство».
    /// </summary>
    [LocalizeFunction("SearchRNS_ResourceKey", "SearchRNS_DescriptionResourceKey")]
    public virtual void SearchRNS()
    {
      lenspec.EtalonDatabooks.ProjectBuildingPermitDocuments.GetAll().Show();
    }
    
    /// <summary>
    /// Список документов вида «Разрешение на ввод объекта».
    /// </summary>
    [LocalizeFunction("SearchRNV_ResourceKey", "SearchRNV_DescriptionResourceKey")]
    public virtual void SearchRNV()
    {
      lenspec.EtalonDatabooks.ProjectObjectPermitDocuments.GetAll().Show();
    }
  }
}