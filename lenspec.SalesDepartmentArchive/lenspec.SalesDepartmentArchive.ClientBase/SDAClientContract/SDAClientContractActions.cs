using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientContract;

namespace lenspec.SalesDepartmentArchive.Client
{
  partial class SDAClientContractActions
  {
    
    //Добавлено Avis Expert
    public override void DeliverDocument(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.DeliverDocument(e);
    }

    public override bool CanDeliverDocument(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // КД создан без возможности регистрации. Доступность действия переопределена для функционала по Выдаче документа.
      return _obj.AccessRights.CanUpdate();
    }
    //конец Добавлено Avis Expert

  }


}