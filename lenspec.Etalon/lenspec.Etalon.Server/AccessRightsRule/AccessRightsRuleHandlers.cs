using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.AccessRightsRule;

namespace lenspec.Etalon
{
  partial class AccessRightsRuleServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      if (_obj.IsGrantOnTasksavis == null)
        _obj.IsGrantOnTasksavis = false;
      
      _obj.IsGrantOnOurCompanyDocumentslenspec = false;
    }
  }

}