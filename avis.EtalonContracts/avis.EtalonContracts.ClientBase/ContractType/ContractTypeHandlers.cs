using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonContracts.ContractType;

namespace avis.EtalonContracts
{
  partial class ContractTypeClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      var rightOnContractDatabooksRole = Roles.GetAll(x => x.Sid == avis.EtalonContracts.Constants.Module.ResponsibleContracDocuments).SingleOrDefault();
      _obj.State.Properties.Code.IsEnabled = Sungero.Company.Employees.Current.IncludedIn(rightOnContractDatabooksRole);
    }

  }
}