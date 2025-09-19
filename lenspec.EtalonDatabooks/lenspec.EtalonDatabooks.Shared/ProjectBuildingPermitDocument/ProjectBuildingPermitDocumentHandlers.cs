using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectBuildingPermitDocument;

namespace lenspec.EtalonDatabooks
{
  partial class ProjectBuildingPermitDocumentSharedHandlers
  {

    public virtual void CollectionAccountingObjectChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      FillName();
    }

    public virtual void DateRNSChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      FillName();
    }

    public virtual void SpecDeveloperChanged(lenspec.EtalonDatabooks.Shared.ProjectBuildingPermitDocumentSpecDeveloperChangedEventArgs e)
    {
      FillName();
    }

    public virtual void NumberRNSChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      FillName();
    }

    public override void OurCFChanged(lenspec.EtalonDatabooks.Shared.ProjectDocumentBaseOurCFChangedEventArgs e)
    {
      base.OurCFChanged(e);
      
      if (e.NewValue == null)
        _obj.CollectionObjectAnProject.Clear();
      
      PublicFunctions.ProjectBuildingPermitDocument.EnableObjectAnSale(_obj);
      
      FillName();
    }

  }
}