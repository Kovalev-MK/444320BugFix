using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDARequestToArchiveBase;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDARequestToArchiveBaseServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      var author = Sungero.Company.Employees.As(_obj.Author);
      if (author != null)
      {
        _obj.Department = author.Department;
      }
    }
    //конец Добавлено Avis Expert
  }

}