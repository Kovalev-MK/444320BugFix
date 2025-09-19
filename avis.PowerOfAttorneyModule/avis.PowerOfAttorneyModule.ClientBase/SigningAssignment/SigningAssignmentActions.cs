using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.SigningAssignment;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class SigningAssignmentActions
  {
    public virtual void Denied(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanDenied(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

    public virtual void Sign(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var signatureSettingsByUser = Sungero.Docflow.SignatureSettings.GetAll(s => s.Recipient.Equals(Users.Current) &&
                                                                             s.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      if (!signatureSettingsByUser.Any())
      {
        e.AddError("Для подписания документа электронной подписью необходимо право подписи.");
        return;
      }
      
      var validSignatureSettings = signatureSettingsByUser.Where(x => x.Certificate != null && (x.Certificate.Enabled == true) &&
                                                                 (!x.Certificate.NotBefore.HasValue || x.Certificate.NotBefore <= Calendar.Now) &&
                                                                 (!x.Certificate.NotAfter.HasValue || x.Certificate.NotAfter >= Calendar.Now));
      
      var documentVersion = _obj.MainAttachment.ApplicationRelinquishmentAuthorities.SingleOrDefault().LastVersion;
      if (validSignatureSettings.Any())
      {
        var certificate = validSignatureSettings.Where(x => x.Priority == signatureSettingsByUser.Select(i => i.Priority).Max()).FirstOrDefault().Certificate;
        Signatures.Approve(documentVersion, certificate, "Утверждено");
      }
      else
        Signatures.Approve(documentVersion, null, "Подписан простой подписью");
    }

    public virtual bool CanSign(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }


}