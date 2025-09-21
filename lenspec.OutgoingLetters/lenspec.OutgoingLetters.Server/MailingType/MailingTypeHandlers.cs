using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.OutgoingLetters.MailingType;

namespace lenspec.OutgoingLetters
{
  partial class MailingTypeServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.BankDetailsAreRequired = false;
      _obj.ChangeOfMeasurements = false;
    }
    //конец Добавлено Avis Expert
  }

}