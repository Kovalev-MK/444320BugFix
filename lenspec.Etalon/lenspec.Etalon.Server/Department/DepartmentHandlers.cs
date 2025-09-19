using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Department;

namespace lenspec.Etalon
{
  partial class DepartmentServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.IsBudgetFormedlenspec = Department.IsBudgetFormedlenspec.No;
    }
  }

}