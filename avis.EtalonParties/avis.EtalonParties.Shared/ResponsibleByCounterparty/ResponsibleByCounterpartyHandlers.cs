using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.ResponsibleByCounterparty;

namespace avis.EtalonParties
{
  partial class ResponsibleByCounterpartySharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void BusinessUnitChanged(avis.EtalonParties.Shared.ResponsibleByCounterpartyBusinessUnitChangedEventArgs e)
    {
      Functions.ResponsibleByCounterparty.FillName(_obj);
    }

    public virtual void CounterpartyChanged(avis.EtalonParties.Shared.ResponsibleByCounterpartyCounterpartyChangedEventArgs e)
    {
      Functions.ResponsibleByCounterparty.FillName(_obj);
    }
    //конец Добавлено Avis Expert

  }
}