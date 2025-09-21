using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalTask;

namespace lenspec.Etalon
{

  partial class ApprovalTaskSharedHandlers
  {

    public override void AuthorChanged(Sungero.Workflow.Shared.TaskAuthorChangedEventArgs e)
    {
      base.AuthorChanged(e);
      
      if (e.NewValue != null && !Equals(e.NewValue, e.OldValue))
        Functions.ApprovalTask.SetHeadOfTheInitiator(_obj);
    }

    //Добавлено Avis Expert
    public virtual void HeadOfTheInitiatorlenspecChanged(lenspec.Etalon.Shared.ApprovalTaskHeadOfTheInitiatorlenspecChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        if (!_obj.ReqApprovers.Where(x => x.Approver.Equals(e.NewValue)).Any())
        {
          _obj.ReqApprovers.AddNew().Approver = e.NewValue;
          e.Params.AddOrUpdate("allowRemoveHeadOfTheInitiator", true);
        }
        else
        {
          e.Params.AddOrUpdate("allowRemoveHeadOfTheInitiator", false);
        }
      }
      
      bool allowRemoveHeadOfTheInitiator;
      if (e.Params.TryGetValue("allowRemoveHeadOfTheInitiator", out allowRemoveHeadOfTheInitiator) && allowRemoveHeadOfTheInitiator)
      {
        var approverToRemove = _obj.ReqApprovers.FirstOrDefault(x => x.Approver.Equals(e.OldValue));
        if (approverToRemove != null)
        {
          _obj.ReqApprovers.Remove(approverToRemove);
        }
      }
    }

    public override void DocumentGroupAdded(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      base.DocumentGroupAdded(e);
      
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document == null)
        return;
      
      // Документ типа "Заявка на сдачу в архив".
      if (SalesDepartmentArchive.SDARequestSubmissionToArchives.Is(document))
        this.SDARequestSubmissionToArchiveAdded(document);
      
      // Документ вида "Заявка на выдачу из архива".
      else if (SalesDepartmentArchive.SDARequestIssuanceFromArchives.Is(document))
        this.SDARequestIssuanceFromArchiveAdded();
      
      // Документ типа "Доверенность".
      else if (lenspec.Etalon.PowerOfAttorneys.Is(document))
        this.PowerOfAttorneyAdded(document);
      
      // Вложить сертификаты получателя УКЭП.
      if (lenspec.ElectronicDigitalSignatures.EDSApplications.Is(document))
      {
        var edsApplication = lenspec.ElectronicDigitalSignatures.EDSApplications.As(document);
        if (edsApplication.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Cancellation)
          lenspec.ElectronicDigitalSignatures.PublicFunctions.EDSApplication.AddCertificatesToAttachmentGroup(edsApplication, _obj.OtherGroup);
      }
      
      this.SetSignatoryProtocol();
      this.SetDeliveryMethod();
    }

    /// <summary>
    /// Вложение документа типа "Заявка на сдачу в архив".
    /// </summary>
    /// <param name="document">Документ.</param>
    private void SDARequestSubmissionToArchiveAdded(Sungero.Docflow.IOfficialDocument document)
    {
      var requestSubmissionToArchive = SalesDepartmentArchive.SDARequestSubmissionToArchives.As(document);
      
      if (requestSubmissionToArchive.UrgentRequest == SalesDepartmentArchive.SDARequestSubmissionToArchive.UrgentRequest.Yes)
        _obj.Importance = Sungero.Docflow.ApprovalTask.Importance.High;
      
      if (!_obj.State.IsCopied)
        using (TenantInfo.Culture.SwitchTo())
          _obj.ActiveText = lenspec.Etalon.ApprovalTasks.Resources.RequestApprovalText;
    }
    
    /// <summary>
    /// Вложение документа типа "Заявка на выдачу из архива".
    /// </summary>
    /// <param name="document">Документ.</param>
    private void SDARequestIssuanceFromArchiveAdded()
    {
      if (_obj.State.IsCopied)
        return;
      
      using (TenantInfo.Culture.SwitchTo())
        _obj.ActiveText = lenspec.Etalon.ApprovalTasks.Resources.RequestApprovalText;
    }
    
    /// <summary>
    /// Вложение документа типа "Доверенность".
    /// </summary>
    /// <param name="document">Документ.</param>
    private void PowerOfAttorneyAdded(Sungero.Docflow.IOfficialDocument document)
    {
      var powerOfAttorney = lenspec.Etalon.PowerOfAttorneys.As(document);
      // Вложить документы поверенных в задачу при согласовании доверенности.
      lenspec.Etalon.Functions.PowerOfAttorney.AddRelatedDocumentsToAttachmentGroup(powerOfAttorney, _obj.OtherGroup);
    }
    
    public override void ApprovalRuleChanged(Sungero.Docflow.Shared.ApprovalTaskApprovalRuleChangedEventArgs e)
    {
      base.ApprovalRuleChanged(e);
      
      if (e.NewValue != null)
      {
        Functions.ApprovalTask.SetHeadOfTheInitiator(_obj);
      }
      else
      {
        _obj.HeadOfTheInitiatorlenspec = null;
        _obj.State.Properties.HeadOfTheInitiatorlenspec.IsVisible = _obj.State.Properties.HeadOfTheInitiatorlenspec.IsRequired = false;
      }
      
      this.SetSignatoryProtocol();
      this.SetDeliveryMethod();
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void SetSignatoryProtocol()
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document == null)
        return;
      
      if (ProtocolsCollegialBodies.ProtocolCollegialBodies.Is(document) && _obj.ApprovalRule != null)
      {
        var protocol = ProtocolsCollegialBodies.ProtocolCollegialBodies.As(document);
        if (protocol.CollegialBody != null && protocol.CollegialBody.Chairman != null)
        {
          var signStage = _obj.ApprovalRule.Stages
            .Where(x => x.Stage != null && x.Stage.StageType == Sungero.Docflow.ApprovalStage.StageType.Sign && x.Stage.ApprovalRole != null)
            .Select(x => x.Stage)
            .FirstOrDefault();
          if (signStage != null && signStage.ApprovalRole.Type == ProtocolsCollegialBodies.CompRoleCollegialBody.Type.Chairman)
          {
            _obj.Signatory = protocol.CollegialBody.Chairman;
            _obj.State.Properties.Signatory.IsEnabled = false;
          }
        }
      }
      
      if (Tenders.TenderCommitteeProtocols.Is(document) && _obj.ApprovalRule != null)
      {
        var protocol = Tenders.TenderCommitteeProtocols.As(document);
        if (protocol.TenderCommittee != null && protocol.TenderCommittee.Chairman != null)
        {
          var signStage = _obj.ApprovalRule.Stages
            .Where(x => x.Stage != null && x.Stage.StageType == Sungero.Docflow.ApprovalStage.StageType.Sign && x.Stage.ApprovalRole != null)
            .Select(x => x.Stage)
            .FirstOrDefault();
          if (signStage != null && signStage.ApprovalRole.Type == lenspec.EtalonDatabooks.ComputedRole.Type.TenderChairman)
          {
            _obj.Signatory = protocol.TenderCommittee.Chairman;
            _obj.State.Properties.Signatory.IsEnabled = false;
          }
        }
      }
      
      if (Tenders.AccreditationCommitteeProtocols.Is(document) && _obj.ApprovalRule != null)
      {
        var protocol = Tenders.AccreditationCommitteeProtocols.As(document);
        if (protocol.AccreditationCommittee != null && protocol.AccreditationCommittee.Chairman != null)
        {
          var signStage = _obj.ApprovalRule.Stages
            .Where(x => x.Stage != null && x.Stage.StageType == Sungero.Docflow.ApprovalStage.StageType.Sign && x.Stage.ApprovalRole != null)
            .Select(x => x.Stage)
            .FirstOrDefault();
          if (signStage != null && signStage.ApprovalRole.Type == lenspec.EtalonDatabooks.ComputedRole.Type.AccreditationChairman)
          {
            _obj.Signatory = protocol.AccreditationCommittee.Chairman;
            _obj.State.Properties.Signatory.IsEnabled = false;
          }
        }
      }
    }
    
    /// <summary>
    /// Заполнить поле Способ доставки, если вложена Заявка на рассылку массовых уведомлений.
    /// </summary>
    private void SetDeliveryMethod()
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document == null)
        return;
      
      if (lenspec.OutgoingLetters.MassMailingApplications.Is(document))
      {
        if (_obj.State.Properties.DeliveryMethod.IsVisible)
        {
          _obj.DeliveryMethod = lenspec.OutgoingLetters.MassMailingApplications.As(document).DeliveryMethod;
        }
        else
        {
          _obj.DeliveryMethod = null;
        }
      }
      if (Sungero.RecordManagement.OutgoingLetters.Is(document))
      {
        if (_obj.State.Properties.DeliveryMethod.IsVisible)
        {
          _obj.DeliveryMethod = Sungero.RecordManagement.OutgoingLetters.As(document).DeliveryMethod;
        }
      }
    }
    //конец Добавлено Avis Expert
  }

}