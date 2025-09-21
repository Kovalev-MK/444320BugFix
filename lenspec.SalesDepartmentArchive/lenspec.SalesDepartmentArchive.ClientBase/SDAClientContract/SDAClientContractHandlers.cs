using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAClientContract;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAClientContractClientHandlers
  {

    //Добавлено Avis Expert
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      _obj.State.Properties.RealEstateDocumentKind.IsRequired = true;
      _obj.State.Properties.InvestContractCode.IsEnabled = false;
      // КД создан без возможности регистрации. Доступность поля переопределена для функционала по Выдаче документа.
      _obj.State.Properties.Tracking.IsEnabled = true;
    }
    //конец Добавлено Avis Expert
  }

}