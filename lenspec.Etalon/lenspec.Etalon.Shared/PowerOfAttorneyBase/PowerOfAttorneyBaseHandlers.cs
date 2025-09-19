using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorneyBase;

namespace lenspec.Etalon
{
  partial class PowerOfAttorneyBaseRepresentativesSharedHandlers
  {

    public override void RepresentativesIssuedToChanged(Sungero.Docflow.Shared.PowerOfAttorneyBaseRepresentativesIssuedToChangedEventArgs e)
    {
      base.RepresentativesIssuedToChanged(e);
      if (e.NewValue != null)
      {
        if (e.NewValue != e.OldValue)
        {
          var employees = Sungero.Company.Employees.GetAll(x => Equals(x.Person, e.NewValue));
          if (employees.Count() == 1)
            _obj.Employeelenspec = employees.SingleOrDefault();
          else
            _obj.Employeelenspec = null;
        }
      }
      else
        _obj.Employeelenspec = null;
    }
  }

  partial class PowerOfAttorneyBaseSharedHandlers
  {

  }
}