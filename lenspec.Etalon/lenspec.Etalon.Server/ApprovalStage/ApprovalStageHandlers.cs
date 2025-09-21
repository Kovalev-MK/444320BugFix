using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalStage;

namespace lenspec.Etalon
{
  partial class ApprovalStageServerHandlers
  {
    //Добавлено Avis Expert
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.WithInterruptionlenspec = false;
      _obj.CheckReservationlenspec = false;
      _obj.UpdateTemplateBeforeExecuteavis = false;
      _obj.BulkEmaillenspec = false;
      _obj.UpdateTemplateNumberAndDateBeforeExecuteavis = false;
      _obj.DeliveryMethodIsEditableavis = false;
      _obj.DoNotSignApplicationslenspec = false;
      _obj.IsProhibitExtensionOfTimeavis = false;
      _obj.IsProhibitChangeSigneravis = false;
      _obj.IsCheckVersionlenspec = false;
      _obj.IsCheckExport1CApplicationForPaymentlenspec = false;
    }
    //конец Добавлено Avis Expert
  }
}