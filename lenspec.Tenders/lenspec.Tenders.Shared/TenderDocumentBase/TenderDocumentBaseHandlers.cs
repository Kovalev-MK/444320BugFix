using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderDocumentBase;

namespace lenspec.Tenders
{
  partial class TenderDocumentBaseSharedHandlers
  {

    //Добавлено Avis Expert
    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      // Очистить поля оргструктуры, которые автоматически заполнились при создании внутреннего документа.
      Functions.TenderDocumentBase.UpdateOrganizationStructure(_obj);
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      FillName();
    }

    public virtual void OurCFChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      FillName();
      
      if (_obj.OurCF.Any(x => x.OurCF != null) && _obj.ObjectAnProjects.Any(x => x.ObjectAnProject != null))
      {
        var ourCFIds = _obj.OurCF.Where(x => x.OurCF != null).Select(x => x.OurCF.Id).ToList();
        var objectAnProjects = _obj.ObjectAnProjects.Where(x => x.ObjectAnProject != null && x.ObjectAnProject.OurCF != null);
        if (objectAnProjects.Any(x => !ourCFIds.Contains(x.ObjectAnProject.OurCF.Id)))
          _obj.ObjectAnProjects.Clear();
      }
    }

    public virtual void CounterpartiesChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      FillName();
    }

    public virtual void TenderSelectionSubjectChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      FillName();
    }
    //конец Добавлено Avis Expert

  }
}