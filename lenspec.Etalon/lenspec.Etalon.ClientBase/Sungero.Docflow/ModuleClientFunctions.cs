using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Docflow.Client
{
  partial class ModuleFunctions
  {
    
    /// <summary>
    /// Утвердить документ с приложениями.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="addenda">Приложения.</param>
    /// <param name="certificate">Сертификат (не передавать, чтобы оставить выбор пользователю).</param>
    /// <param name="substituted">За кого выполняется утверждение (не передавать, чтобы утвердить под текущим пользователем).</param>
    /// <param name="endorseWhenApproveFailed">Согласовать документ, если не удается выполнить утверждение.</param>
    /// <param name="needStrongSign">Требуется квалифицированная электронная подпись.</param>
    /// <param name="comment">Комментарий.</param>
    /// <returns>True, если сам документ был утверждён или не имеет версий. Факт подписания приложений неважен.</returns>
    [Public]
    public override bool ApproveWithAddenda(Sungero.Docflow.IOfficialDocument document, List<Sungero.Docflow.IOfficialDocument> addenda,
                                           Sungero.CoreEntities.ICertificate certificate, Sungero.Company.IEmployee substituted,
                                           bool endorseWhenApproveFailed, bool needStrongSign, string comment)
    {
      if (!lenspec.Etalon.FormalizedPowerOfAttorneys.Is(document))
      {
        return base.ApproveWithAddenda(document, addenda, certificate, substituted, endorseWhenApproveFailed, needStrongSign, comment);
      }
      
      var addendaHaveVersions = addenda != null && addenda.Any(a => a.HasVersions);
      if (!document.HasVersions && !addendaHaveVersions)
        return true;
      
      if (certificate == null && needStrongSign)
      {
        if (!this.TryGetUserCertificateAvis(document, out certificate))
          return false;
      }
      
      try
      {
        var result = !document.HasVersions;
        if (document.HasVersions)
        {
          var canApprove = !Sungero.Docflow.PublicFunctions.OfficialDocument.Remote.GetApprovalValidationErrors(document, true).Any();
          if (canApprove)
          {
            result = Signatures.Approve(document.LastVersion, certificate, comment, substituted);
          }
          else if (endorseWhenApproveFailed)
            result = Signatures.Endorse(document.LastVersion, certificate, comment, substituted);
        }
        
        // Если не удалось утвердить основной документ или приложений нет - приложения не трогаем.
        if (!result || addenda == null || !addenda.Any())
          return result;
        
        var addendaWithVersions = addenda.Where(a => a.HasVersions).ToList();
        if (!addendaWithVersions.Any())
          return result;
        
        var canBeApproved = new List<Sungero.Docflow.IOfficialDocument>();
        var canBeEndorsed = new List<Sungero.Docflow.IOfficialDocument>();
        foreach (var addendumDocument in addendaWithVersions)
        {
          var canApprove = !Sungero.Docflow.PublicFunctions.OfficialDocument.Remote.GetApprovalValidationErrors(addendumDocument, true).Any();
          if (canApprove)
            canBeApproved.Add(addendumDocument);
          else
            canBeEndorsed.Add(addendumDocument);
        }
        /*
        foreach (var addendumDocument in canBeApproved)
        {
          var addendumAccountingDocument = Sungero.Docflow.AccountingDocumentBases.As(addendumDocument);
          if (addendumAccountingDocument != null && addendumAccountingDocument.IsFormalized == true)
          {
            Functions.AccountingDocumentBase.GenerateDefaultSellerTitle(addendumAccountingDocument);
            Functions.AccountingDocumentBase.GenerateDefaultBuyerTitle(addendumAccountingDocument);
          }
        }
        */
        if (canBeApproved.Any())
          Signatures.Approve(canBeApproved.Select(a => a.LastVersion), certificate, comment, substituted);
        if (canBeEndorsed.Any())
          Signatures.Endorse(canBeEndorsed.Select(a => a.LastVersion), certificate, comment, substituted);
        return result;
      }
      catch (Sungero.Domain.Shared.Exceptions.ChildEntityNotFoundException ex)
      {
        throw AppliedCodeException.Create(Sungero.Docflow.OfficialDocuments.Resources.SigningVersionWasDeleted, ex);
      }
    }
    
    /// <summary>
    /// Согласовать документ с приложениями.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="addenda">Приложения.</param>
    /// <param name="certificate">Сертификат (не передавать, чтобы оставить выбор пользователю).</param>
    /// <param name="substituted">За кого выполняется утверждение (не передавать, чтобы утвердить под текущим пользователем).</param>
    /// <param name="needStrongSign">Требуется квалифицированная электронная подпись.</param>
    /// <param name="comment">Комментарий.</param>
    /// <returns>True, если сам документ был согласован или не имеет версий. Факт согласования приложений неважен.</returns>
    [Public]
    public override bool EndorseWithAddenda(Sungero.Content.IElectronicDocument document, List<Sungero.Content.IElectronicDocument> addenda,
                                           Sungero.CoreEntities.ICertificate certificate, Sungero.CoreEntities.IUser substituted,
                                           bool needStrongSign, string comment)
    {
      if (!lenspec.Etalon.FormalizedPowerOfAttorneys.Is(document))
      {
        return base.EndorseWithAddenda(document, addenda, certificate, substituted, needStrongSign, comment);
      }
      
      var addendaHasVersions = addenda != null && addenda.Any(a => a.HasVersions);
      if (!document.HasVersions && !addendaHasVersions)
        return true;
      
      if (certificate == null && needStrongSign)
      {
        var officialDocument = Sungero.Docflow.OfficialDocuments.As(document);
        
        if (!this.TryGetUserCertificateAvis(officialDocument, out certificate))
          return false;
      }
      
      try
      {
        var result = !document.HasVersions;
        if (document.HasVersions)
        {
          result = Signatures.Endorse(document.LastVersion, certificate, comment, substituted);
        }
        
        // Если не удалось согласовать основной документ или приложений нет - приложения не трогаем.
        if (!result || addenda == null || !addenda.Any())
          return result;
        
        var addendaWithVersions = addenda.Where(a => a.HasVersions).ToList();
        if (!addendaWithVersions.Any())
          return result;
        
        Signatures.Endorse(addendaWithVersions.Select(a => a.LastVersion), certificate, comment, substituted);
        return result;
      }
      catch (Sungero.Domain.Shared.Exceptions.ChildEntityNotFoundException ex)
      {
        throw AppliedCodeException.Create(Sungero.Docflow.OfficialDocuments.Resources.SigningVersionWasDeleted, ex);
      }
    }
    
    /// <summary>
    /// Получить сертификат пользователя для подписания.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="certificate">Сертификат для подписания.</param>
    /// <returns>True, если выбор произведен, false в случае отмены.</returns>
    private bool TryGetUserCertificateAvis(Sungero.Docflow.IOfficialDocument document, out Sungero.CoreEntities.ICertificate certificate)
    {
      var businessUnit = document.BusinessUnit;
      certificate = null;
      var certificates = Sungero.Docflow.PublicFunctions.Module.Remote.GetCertificates(document);
      var ourSigningReason = document.OurSigningReason;
      
      // Взять сертификат из основания подписания, если он подходит по критериям.
      // При рассмотрении адресатом поле "Основание" вернет сертификат подписавшего, а не рассматривающего, поэтому для рассмотрения автовыбор сертификата из "Основания" не делать.
      if (!CallContext.CalledFrom(ApprovalReviewAssignments.Info) &&
          ourSigningReason != null &&
          ourSigningReason.Certificate != null &&
          certificates.Contains(ourSigningReason.Certificate) &&
          Equals(ourSigningReason.Certificate.Owner, Employees.Current) &&
          !Sungero.Docflow.PublicFunctions.SignatureSetting.Remote.FormalizedPowerOfAttorneyIsExpired(ourSigningReason))
      {
        certificate = document.OurSigningReason.Certificate;
        // Сверить НОР в Праве подписи и в карточке документа.
        if (document.OurSigningReason != null && !document.OurSigningReason.BusinessUnits.Any(x => Equals(x.BusinessUnit, businessUnit)))
        {
          Dialogs.ShowMessage(lenspec.Etalon.Module.Docflow.Resources.CertificateDoesNotMatchToBusinessUnitFormat(businessUnit.Name), MessageType.Error);
          return false;
        }
        // Сверить сертификат в Праве подписи и выбранный.
        if (document.OurSigningReason != null && !Equals(document.OurSigningReason.Certificate, certificate))
        {
          Dialogs.ShowMessage(lenspec.Etalon.Module.Docflow.Resources.CertificateDoesNotMatchBySignatureSetting, MessageType.Error);
          return false;
        }
        return true;
      }
      
      if (certificates.Any())
      {
        var selectedCertificate = certificates.Count() > 1 ?
          certificates.ShowSelectCertificate() :
          certificates.First();
        if (selectedCertificate == null)
          return false;
        certificate = selectedCertificate;
        // Сверить НОР в Праве подписи и в карточке документа.
        if (document.OurSigningReason != null && document.OurSigningReason.BusinessUnits.Any(x => x.BusinessUnit != null) &&
            !document.OurSigningReason.BusinessUnits.Any(x => Equals(x.BusinessUnit, businessUnit)))
        {
          Dialogs.ShowMessage(lenspec.Etalon.Module.Docflow.Resources.CertificateDoesNotMatchToBusinessUnitFormat(businessUnit.Name), MessageType.Error);
          return false;
        }
        // Сверить сертификат в Праве подписи и выбранный.
        if (document.OurSigningReason != null && !Equals(document.OurSigningReason.Certificate, certificate))
        {
          Dialogs.ShowMessage(lenspec.Etalon.Module.Docflow.Resources.CertificateDoesNotMatchBySignatureSetting, MessageType.Error);
          return false;
        }
      }
      return true;
    }
    
    /// <summary>
    /// Утвердить документ.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="addenda">Приложения.</param>
    /// <param name="substituted">За кого выполняется утверждение.</param>
    /// <param name="needStrongSign">Требуется квалифицированная электронная подпись.</param>
    /// <param name="comment">Комментарий.</param>
    /// <param name="eventArgs">Аргумент обработчика вызова.</param>
    public override void ApproveDocument(Sungero.Docflow.IOfficialDocument document, List<Sungero.Docflow.IOfficialDocument> addenda,
                                         Sungero.Company.IEmployee substituted, bool needStrongSign, string comment,
                                         Sungero.Domain.Client.ExecuteActionArgs eventArgs)
    {
      base.ApproveDocument(document,addenda,substituted,needStrongSign,comment,eventArgs);
    }
  }
}