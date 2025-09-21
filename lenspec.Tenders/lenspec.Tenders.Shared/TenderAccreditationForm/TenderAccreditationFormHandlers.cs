using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderAccreditationForm;

namespace lenspec.Tenders
{
  partial class TenderAccreditationFormSharedHandlers
  {

    public override void RegistrationDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.RegistrationDateChanged(e);
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      FillName();
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      FillName();
    }

    public virtual void AccreditationFormDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      // DIRRXMIGR-862: Используется коробочное поле "Дата документа".
    }

    public virtual void CounterpartyChanged(lenspec.Tenders.Shared.TenderAccreditationFormCounterpartyChangedEventArgs e)
    {
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      FillName();
    }

  }
}