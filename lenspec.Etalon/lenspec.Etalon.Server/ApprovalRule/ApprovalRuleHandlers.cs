using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalRule;

namespace lenspec.Etalon
{
  partial class ApprovalRuleServerHandlers
  {

    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.CheckApprovalSettinglenspec = false;
    }
    //конец Добавлено Avis Expert
  }

}