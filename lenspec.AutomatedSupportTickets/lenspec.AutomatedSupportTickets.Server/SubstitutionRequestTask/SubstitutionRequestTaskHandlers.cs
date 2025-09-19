using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.SubstitutionRequestTask;

namespace lenspec.AutomatedSupportTickets
{

  partial class SubstitutionRequestTaskServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Prolongation = false;
    }
    //конец Добавлено Avis Expert
  }

}