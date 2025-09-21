using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingLetter;

namespace lenspec.Etalon
{
  partial class IncomingLetterClientHandlers
  {

    //Добавлено Avis Expert
    public override void AssigneeValueInput(Sungero.Docflow.Client.OfficialDocumentAssigneeValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.AssigneeValueInput(e);
    }

    public override void AddresseeValueInput(Sungero.Docflow.Client.IncomingDocumentBaseAddresseeValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.AddresseeValueInput(e);      
    }
    //конец Добавлено Avis Expert
  }


}