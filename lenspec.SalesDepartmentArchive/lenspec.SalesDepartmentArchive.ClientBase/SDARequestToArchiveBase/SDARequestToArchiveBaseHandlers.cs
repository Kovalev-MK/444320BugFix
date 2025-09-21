using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDARequestToArchiveBase;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDARequestToArchiveBaseClientHandlers
  {

    //Добавлено Avis Expert
    public override void AuthorValueInput(Sungero.Content.Client.ElectronicDocumentAuthorValueInputEventArgs e)
    {
      if (e.NewValue != null && Sungero.Company.Employees.Is(e.NewValue) && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(Sungero.Company.Employees.As(e.NewValue)))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.AuthorValueInput(e);
    }
    //конец Добавлено Avis Expert

  }
}