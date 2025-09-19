using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OutgoingLetter;

namespace lenspec.Etalon.Client
{
  internal static class OutgoingLetterAddresseesStaticActions
  {
    //Добавлено Avis Expert
    public static void ClearAddresseeslenspec(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = Sungero.RecordManagement.OutgoingLetters.As(e.Entity);
      obj.Addressees.Clear();
    }

    public static bool CanClearAddresseeslenspec(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = Sungero.RecordManagement.OutgoingLetters.As(e.Entity);
      return obj.Addressees.Any() && obj.IsManyAddressees == true && obj.InternalApprovalState == null;
    }

    public static void FillFromDistributionList(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = Sungero.Docflow.OutgoingDocumentBases.As(e.Entity);
      var distributionLists = Etalon.Module.Docflow.Functions.Module.Remote.GetDistributionLists();
      var distributionList = distributionLists.ShowSelect();
      if (distributionList == null)
        return;
      
      foreach (var addressee in distributionList.Addressees.OrderBy(a => a.Number))
      {
        var newAddressee = obj.Addressees.AddNew();
        newAddressee.Correspondent = addressee.Correspondent;
        newAddressee.Addressee = addressee.Addressee;
        newAddressee.DeliveryMethod = obj.DeliveryMethod;
      }
    }

    public static bool CanFillFromDistributionList(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = Sungero.Docflow.OutgoingDocumentBases.As(e.Entity);
      return obj.IsManyAddressees == true && !Sungero.Docflow.PublicFunctions.OutgoingDocumentBase.DisableAddresseesOnRegistration(obj, e);
    }
    //конец Добавлено Avis Expert

  }


  partial class OutgoingLetterActions
  {


    //Добавлено Avis Expert
    public virtual void ExportVersionWithSignatoryavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.HasVersions)
      {
        e.AddError("Необходимо добавить версию документа.");
        return;
      }

      if (!Signatures.Get(_obj.LastVersion).Any(x => x.SignatureType == SignatureType.Approval))
      {
        e.AddError("Для отправки с открепленным сертификатом документ должен быть подписан ЭП.");
        return;
      }
      
      var zip = lenspec.Etalon.PublicFunctions.OutgoingLetter.Remote.GetZipWithSignaturies(_obj, "sig");
      if (zip == null)
      {
        e.AddError("Ошибка выгрузки документа с подписью. Обратитесь к администратору.");
        return;
      }
      zip.Export();
    }

    public virtual bool CanExportVersionWithSignatoryavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return _obj.HasVersions;
    }

    public virtual void CreatePostalItemavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      var postalItem = avis.PostalItems.PostalItems.Create();
      postalItem.OutgoingLetter = _obj;
      postalItem.Counterparty = _obj.Correspondent;
      postalItem.MailDeliveryMethod = lenspec.Etalon.MailDeliveryMethods.As(_obj.DeliveryMethod);
      postalItem.Show();
    }

    public virtual bool CanCreatePostalItemavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void UpdateTemplatelenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.UpdateTemplatelenspec(e);
    }

    public override bool CanUpdateTemplatelenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
    }


    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.HasVersions)
      {
        Dialogs.ShowMessage(lenspec.Etalon.OutgoingLetters.Resources.AttachTextOfTheLetter, MessageType.Warning);
        return;
      }
      
      if (Functions.ApprovalTask.CheckDuplicates(_obj, true))
        return;
      
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }
    //конец Добавлено Avis Expert

  }

}