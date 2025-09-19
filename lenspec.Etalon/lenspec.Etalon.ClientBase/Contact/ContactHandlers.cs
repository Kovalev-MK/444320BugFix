using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Contact;

namespace lenspec.Etalon
{
  partial class ContactClientHandlers
  {
    // Добавлено avis.

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      // Если администратор или Канцелярия ГК. Даём доступ к полю емейл.
      var officeGKRoleGuid = lenspec.EtalonDatabooks.PublicConstants.Module.OfficeGK;
      var responsibleForCounterpartyQualificationRole = lenspec.Tenders.PublicConstants.Module.ResponsibleForCounterpartyQualificationRole;
      if (Employees.Current.IncludedIn(Roles.Administrators) || Employees.Current.IncludedIn(officeGKRoleGuid) 
          || Employees.Current.IncludedIn(responsibleForCounterpartyQualificationRole))
        _obj.State.Properties.Email.IsEnabled = true;
    }
    
    // Конец добавлено avis.
  }
}