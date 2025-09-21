using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectBuildingPermitDocument;

namespace lenspec.EtalonDatabooks.Client
{
  partial class ProjectBuildingPermitDocumentFunctions
  {
    /// <summary>
    /// Доступность поля Объекты продаж, в зависимости от ИСП.
    /// </summary>
    [Public]
    public void EnableObjectAnSale()
    {
      // Доступность поля Объекты продаж, в зависимости от ИСП.
      _obj.State.Properties.CollectionObjectAnProject.IsEnabled = _obj.OurCF != null;
    }
  }
}