using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectObjectPermitDocument;

namespace lenspec.EtalonDatabooks
{
  partial class ProjectObjectPermitDocumentSharedHandlers
  {

    public virtual void CollectionAccountingProjectChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      FillName();
    }

    public virtual void DateRNVChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      FillName();
    }

    public virtual void SpecDeveloperChanged(lenspec.EtalonDatabooks.Shared.ProjectObjectPermitDocumentSpecDeveloperChangedEventArgs e)
    {
      FillName();
    }

    public virtual void NumberRNVChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      FillName();
    }

    public override void OurCFChanged(lenspec.EtalonDatabooks.Shared.ProjectDocumentBaseOurCFChangedEventArgs e)
    {
      base.OurCFChanged(e);
      
      if (e.NewValue == null)
        _obj.CollectionObjectAnProject.Clear();
      
      PublicFunctions.ProjectObjectPermitDocument.EnableObjectAnSale(_obj);
      
      FillName();
    }

  }
}