using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ManagementCompanyJKHArhive.ArhiveJKHDocumentBase;

namespace avis.ManagementCompanyJKHArhive.Client
{
  partial class ArhiveJKHDocumentBaseActions
  {
    public override void CreateFromTemplate(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateFromTemplate(e);
    }

    /// <summary>
    /// Возможность выполнения "из шаблона".
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public override bool CanCreateFromTemplate(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
      //return base.CanCreateFromTemplate(e);
    }

  }

}