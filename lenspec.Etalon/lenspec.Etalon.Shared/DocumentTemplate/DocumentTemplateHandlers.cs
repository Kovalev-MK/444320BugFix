using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentTemplate;

namespace lenspec.Etalon
{
  partial class DocumentTemplateSharedHandlers
  {

    public virtual void GroupContractTypeslenspecChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      
    }

    public override void DocumentTypeChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.DocumentTypeChanged(e);
    }

  }
}