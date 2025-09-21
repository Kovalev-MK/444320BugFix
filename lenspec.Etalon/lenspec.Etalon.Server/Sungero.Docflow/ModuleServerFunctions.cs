using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using Sungero.Core;
using Sungero.CoreEntities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace lenspec.Etalon.Module.Docflow.Server
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// Получить из списка правил подходящие для документа.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Подходящие правила.</returns>
    public override IQueryable<Sungero.Docflow.IAccessRightsRule> GetAvailableRules(Sungero.Docflow.IOfficialDocument document)
    {
      if (!Etalon.ContractualDocuments.Is(document))
        return base.GetAvailableRules(document);
      
      var contractualDocument = Etalon.ContractualDocuments.As(document);
      var businessUnit = Sungero.Company.BusinessUnits.GetAll(x => Equals(contractualDocument.Counterparty, x.Company)).SingleOrDefault();
      if (businessUnit == null)
        return base.GetAvailableRules(document);
      
      var documentGroup = Sungero.Docflow.PublicFunctions.OfficialDocument.GetDocumentGroup(document);
      
      return Sungero.Docflow.AccessRightsRules.GetAll(s => s.Status == Sungero.Docflow.AccessRightsRule.Status.Active)
        .Where(s => !s.DocumentKinds.Any() || s.DocumentKinds.Any(k => Equals(k.DocumentKind, document.DocumentKind)))
        .Where(s => !s.BusinessUnits.Any() || s.BusinessUnits.Any(u => Equals(u.BusinessUnit, document.BusinessUnit) || Equals(u.BusinessUnit, businessUnit)))
        .Where(s => !s.Departments.Any() || s.Departments.Any(k => Equals(k.Department, document.Department)))
        .Where(s => !s.DocumentGroups.Any() || s.DocumentGroups.Any(k => Equals(k.DocumentGroup, documentGroup)));;
    }
    
    public override IZip CreateZipFromZipModel(List<Sungero.Docflow.Structures.Module.ZipModel> zipModels, List<Sungero.Docflow.Structures.Module.ExportedDocument> objs, string fileName)
    {
      var zip = Zip.Create();
      
      foreach (var zipModel in zipModels)
      {
        var document = Sungero.Docflow.OfficialDocuments.Get(zipModel.DocumentId);
        var version = document.Versions.Where(x => x.Id == zipModel.VersionId).FirstOrDefault();
        if (zipModel.SignatureId != null)
        {
          var signature = Signatures.Get(version).Where(x => x.Id == zipModel.SignatureId).SingleOrDefault();
          var signBody = signature.GetDataSignature();
          zip.Add(signBody, zipModel.FileName, "sig", zipModel.FolderRelativePath.ToArray());
          continue;
        }
        var body = zipModel.IsPublicBody ? version.PublicBody : version.Body;
        zip.Add(body, zipModel.FileName, zipModel.FolderRelativePath.ToArray());
        Logger.DebugFormat("Document with Id '{0}', version id '{1}', is PublicBody: '{2}' has been added to zip model",
                           zipModel.DocumentId, zipModel.VersionId, zipModel.IsPublicBody);
      }
      // Отчет
      var now = Sungero.Core.Calendar.UserNow;
      var generated = Sungero.Docflow.Shared.ModuleFunctions.GetFinArchiveExportReport(objs, now);
      zip.Add(generated, Sungero.FinancialArchive.Reports.Resources.FinArchiveExportReport.HeaderFormat(now.ToShortDateString() + " " + now.ToLongTimeString()));
      Logger.DebugFormat("Report has been added to zip model");
      
      zip.Save(fileName);
      Logger.DebugFormat("Zip model has been saved");
      return zip;
    }
    
    public override void ExportSignature(Sungero.Content.IElectronicDocumentVersions version, string fileName, Sungero.Docflow.Structures.Module.ExportedFolder folder, Sungero.Domain.Shared.ISignature signature, List<Sungero.Docflow.Structures.Module.ZipModel> zipModels, Sungero.Docflow.Structures.Module.ExportedFolder mainFolder)
    {
      if (signature != null)
      {
        var signData = signature.GetDataSignature();
        var signFullFileName = fileName + "SIG";
        
        if (zipModels != null)
        {
          var zipModel = Sungero.Docflow.Structures.Module.ZipModel.Create();
          zipModel.DocumentId = version.ElectronicDocument.Id;
          zipModel.VersionId = version.Id;
          zipModel.FileName = signFullFileName;
          zipModel.FolderRelativePath = base.GetFolderRelativePath(mainFolder);
          zipModel.SignatureId = signature.Id;
          zipModel.Size = signData.LongLength;
          zipModels.Add(zipModel);
          signData = null;
        }
        
        var file = Sungero.Docflow.Structures.Module.ExportedFile.Create(-1, signFullFileName + ".sig", signData, null, null);
        folder.Files.Add(file);
      }
    }
    
    /// <summary>
    /// Выдать права на документ по правилу назначения прав.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="ruleIds">ИД правил назначения прав.</param>
    /// <param name="grantAccessRightsToRelatedDocuments">Выдавать права связанным документам.</param>
    /// <returns>True, если права были успешно выданы.</returns>
    public override bool GrantAccessRightsToDocumentByRule(Sungero.Docflow.IOfficialDocument document, List<long> ruleIds, bool grantAccessRightsToRelatedDocuments)
    {
      var logMessagePrefix = string.Format("AccessRightsBulkProcessing. GrantAccessRightsToDocumentsByRule. Document(ID={0}).", document.Id);
      if (document == null)
      {
        Logger.DebugFormat("{0} No document with this id found", logMessagePrefix);
        return true;
      }
      
      var documentRules = Sungero.Docflow.AccessRightsRules.GetAll(r => ruleIds.Contains(r.Id)).ToList();
      if (!documentRules.Any())
      {
        Logger.DebugFormat("{0} No suitable rules found", logMessagePrefix);
        return true;
      }
      
      var needSave = false;
      var recipientsAccessRights = this.GetRecipientsAccessRights(document.Id, documentRules);
      foreach (var recipientAccessRights in recipientsAccessRights)
      {
        var recipientAccessRightsGuids = recipientAccessRights.Value.Select(x => Sungero.Docflow.PublicFunctions.Module.GetRightTypeGuid(x));
        var highestAccessRights = this.GetHighestInstanceAccessRights(recipientAccessRightsGuids);
        if (this.GrantAccessRightsOnEntity(document, recipientAccessRights.Key, highestAccessRights.Value))
          needSave = true;
      }
      
      if (needSave)
      {
        try
        {
          var etalonOfficialDocument = Etalon.OfficialDocuments.As(document);
          if (etalonOfficialDocument != null && etalonOfficialDocument.Archiveavis.HasValue && etalonOfficialDocument.Archiveavis.Value == true)
          {
            Logger.DebugFormat("Avis - GrantAccessRightsToDocumentByRule - снята обязательность для сохранения прав в архивном документе (ИД {0})", etalonOfficialDocument.Id);
            foreach(var property in etalonOfficialDocument.State.Properties)
            {
              property.IsRequired = false;
            }
          }
          document.AccessRights.Save();
          Logger.DebugFormat("{0} Rights to the document granted successfully", logMessagePrefix);
        }
        catch (Exception ex)
        {
          // В случае возникновения исключения документ всегда уходит на повторную обработку в рамках АО.
          // Ничего критичного не произойдет и поэтому в лог пишется сообщение типа Debug,
          // чтобы оно не светилось красным и не нервировало лишний раз администратора.
          Logger.DebugFormat("{0} Cannot grant rights to document", ex, logMessagePrefix);
          return false;
        }
      }
      else
      {
        Logger.DebugFormat("{0} No need grant rights to document", logMessagePrefix);
      }
      
      // Права на дочерние документы от ведущего.
      if (grantAccessRightsToRelatedDocuments)
        this.CreateGrantAccessRightsToChildDocumentAsyncHandler(document, documentRules);
      
      return true;
    }
    
    /// <summary>
    /// Создать роль.
    /// </summary>
    /// <param name="roleName">Название роли.</param>
    /// <param name="roleDescription">Описание роли.</param>
    /// <param name="roleGuid">Guid роли. Игнорирует имя.</param>
    /// <param name="isSingleUser">Роль с одним участником.</param>
    /// <returns>Новая роль.</returns>
    [Public]
    public static IRole CreateRoleAvis(string roleName, string roleDescription, Guid roleGuid, bool isSingleUser)
    {
      //InitializationLogger.DebugFormat("Init: Create Role {0}", roleName);
      var role = Roles.GetAll(r => r.Sid == roleGuid).FirstOrDefault();
      
      if (role == null)
      {
        role = Roles.Create();
        role.Name = roleName;
        role.Description = roleDescription;
        role.Sid = roleGuid;
        role.IsSystem = true;
        role.IsSingleUser = isSingleUser;
        
        if (isSingleUser)
        {
          var newLink = role.RecipientLinks.AddNew();
          newLink.Member = Users.Current;
        }
        
        role.Save();
      }
      else
      {
        if (role.Name != roleName)
        {
          //InitializationLogger.DebugFormat("Role '{0}'(Sid = {1}) renamed as '{2}'", role.Name, role.Sid, roleName);
          role.Name = roleName;
          role.Save();
        }
        if (role.Description != roleDescription)
        {
          //InitializationLogger.DebugFormat("Role '{0}'(Sid = {1}) update Description '{2}'", role.Name, role.Sid, roleDescription);
          role.Description = roleDescription;
          role.Save();
        }
        
        if (role.IsSingleUser != isSingleUser && isSingleUser == true)
        {
          role.RecipientLinks.Clear();
          
          role.IsSingleUser = isSingleUser;
          var newLink = role.RecipientLinks.AddNew();
          newLink.Member = Users.Current;
          role.Save();
        }
        
        if (role.IsSingleUser != isSingleUser && isSingleUser == false)
        {
          role.IsSingleUser = isSingleUser;
          role.Save();
        }
      }
      return role;
    }
    
    /// <summary>
    /// Получить списки рассылки.
    /// </summary>
    /// <returns>Списки рассылки.</returns>
    [Remote(IsPure = true)]
    public IQueryable<Sungero.Docflow.IDistributionList> GetDistributionLists()
    {
      return Sungero.Docflow.DistributionLists.GetAll().Where(a => a.Status == Sungero.RecordManagement.AcquaintanceList.Status.Active);
    }
    
    /// <summary>
    /// Получить количество якорей в word документе.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Количество якорей в документе.</returns>
    [Public, Remote]
    public static int GetCountAnchor(Sungero.Docflow.IOfficialDocument document)
    {
      var version = document.Versions.SingleOrDefault(v => v.Id == document.LastVersion.Id);
      
      using (var inputStream = new System.IO.MemoryStream())
      {
        version.Body.Read().CopyTo(inputStream);

        var wordDoc = WordprocessingDocument.Open(inputStream, true);
        
        var mainDocumentPart = wordDoc.MainDocumentPart;
        var documentBody = mainDocumentPart.Document.Body;
        
        // Находим параграфы с якорями.
        var count = documentBody.Descendants<Paragraph>().Count(x => x.InnerText.Equals("⚓^"));
        
        return count;
      }
    }
    
    /// <summary>
    /// Получить отметку об ЭП для сертификата из подписи.
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// Изменен текст, убран логотип, добавлен параметр OurFirmName
    /// <returns>Изображение отметки об ЭП для сертификата в виде html.</returns>
    [Public]
    public override string GetSignatureMarkForCertificateAsHtml(Sungero.Domain.Shared.ISignature signature, Sungero.Docflow.Structures.StampSetting.ISignatureStampParams signatureStampParams)
    {
      if (signature == null)
      {
        Logger.Debug("GetSignatureMarkForCertificateAsHtml - signature is null");
        return string.Empty;
      }
      
      var certificate = signature.SignCertificate;
      if (certificate == null)
      {
        Logger.Debug("GetSignatureMarkForCertificateAsHtml - certificate is null");
        return string.Empty;
      }
      
      var certificateSubject = this.GetCertificateSubject(signature);
      if (certificateSubject == null)
      {
        Logger.Debug("GetSignatureMarkForCertificateAsHtml - certificateSubject is null");
        return string.Empty;
      }
      try
      {
        Logger.DebugFormat("GetSignatureMarkForCertificateAsHtml - Surname={0}, GivenName={1}, CounterpartyName={2}, NotBefore={3}, NotAfter={4}, OrganizationName={5}",
                           certificateSubject.Surname,
                           certificateSubject.GivenName,
                           certificateSubject.CounterpartyName,
                           certificate.NotBefore == null ? "null" : certificate.NotBefore.Value.ToShortDateString(),
                           certificate.NotAfter == null ? "null" : certificate.NotAfter.Value.ToShortDateString(),
                           certificateSubject.OrganizationName);
        
        var signatoryFullName = string.Format("{0} {1}", certificateSubject.Surname, certificateSubject.GivenName).Trim();
        if (string.IsNullOrEmpty(signatoryFullName))
          signatoryFullName = certificateSubject.CounterpartyName;
        
        string html;
        string validity;
        using (Sungero.Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
        {
          html = Sungero.Docflow.Resources.HtmlStampTemplateForCertificate;
          html = html.Replace("{SignatoryFullName}", signatoryFullName);
          html = html.Replace("{Thumbprint}", certificate.Thumbprint.ToLower());
          html = html.Replace("{Logo}", signatureStampParams.Logo);
          html = html.Replace("{SigningDate}", signatureStampParams.SigningDate);
          html = html.Replace("{Title}", signatureStampParams.Title);
          validity = string.Format("{0} {1} {2} {3}",
                                   Company.Resources.From,
                                   certificate.NotBefore.Value.ToShortDateString(),
                                   Company.Resources.To,
                                   certificate.NotAfter.Value.ToShortDateString());
        }
        html = html.Replace("{Validity}", validity);
        //html = html.Replace("{OurFirmName}", certificateSubject.OrganizationName);
        return html;
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("GetSignatureMarkForCertificateAsHtml - {0}", ex.Message);
        return string.Empty;
      }
    }
    
    /// <summary>
    /// Получить отметку об ЭП.
    /// </summary>
    /// <param name="document">Документ для преобразования.</param>
    /// <param name="versionId">ИД версии для генерации.</param>
    /// <returns>Изображение отметки об ЭП в виде html.</returns>
    [Public]
    public override string GetSignatureMarkAsHtml(Sungero.Docflow.IOfficialDocument document, long versionId)
    {
      var signature = Sungero.Docflow.PublicFunctions.OfficialDocument.GetSignatureForMark(document, versionId);
      if (signature == null)
        throw new Exception(OfficialDocuments.Resources.LastVersionNotApproved);
      
      var stampSetting =  Sungero.Docflow.PublicFunctions.OfficialDocument.GetStampSettings(document).FirstOrDefault();
      var signatureStampParams = Sungero.Docflow.Structures.StampSetting.SignatureStampParams.Create();
      if (stampSetting != null)
        signatureStampParams = Sungero.Docflow.PublicFunctions.StampSetting.GetSignatureStampParams(stampSetting, signature.SigningDate, signature.SignCertificate != null);
      else
        signatureStampParams = this.GetDefaultSignatureStampParams(signature.SignCertificate != null);
      
      // В случае квалифицированной ЭП информацию для отметки брать из атрибутов субъекта сертификата.
      // В случае простой ЭП информацию для отметки брать из атрибутов подписи.
      if (signature.SignCertificate != null)
        return this.GetSignatureMarkForCertificateAsHtml(signature, signatureStampParams, document);
      else
        return this.GetSignatureMarkForSimpleSignatureAsHtml(signature, signatureStampParams, document);
    }

    /// <summary>
    /// Получить отметку об ЭП для сертификата из подписи.
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// Параметр OurFirmName заполняется из документа, поля Наша организация
    /// <returns>Изображение отметки об ЭП для сертификата в виде html.</returns>
    [Public]
    public string GetSignatureMarkForSimpleSignatureAsHtml(Sungero.Domain.Shared.ISignature signature, Sungero.Docflow.Structures.StampSetting.ISignatureStampParams signatureStampParams, Sungero.Docflow.IOfficialDocument document)
    {
      var html = lenspec.Etalon.Module.Docflow.PublicFunctions.Module.GetSignatureMarkForSimpleSignatureAsHtml(signature, signatureStampParams);
      
      // Проверяем что документ
      //      if ((Sungero.Docflow.Addendums.Is(document) || lenspec.LocalRegulations.DocumentApprovedByOrders.Is(document)) && document?.LeadingDocument?.BusinessUnit != null)
      //        html = html.Replace("{OurFirmName}", document.LeadingDocument.BusinessUnit.Name); //document.BusinessUnit.Name
      //      else
      html = html.Replace("{OurFirmName}", document?.BusinessUnit?.Name); //document.BusinessUnit.Name
      
      //html = html.Replace("{OurFirmName}", document.BusinessUnit.Name); //document.BusinessUnit.Name
      return html;
    }

    /// <summary>
    /// Получить отметку об ЭП для сертификата из подписи.
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// Параметр OurFirmName заполняется из документа, поля Наша организация
    /// <returns>Изображение отметки об ЭП для сертификата в виде html.</returns>
    [Public]
    public string GetSignatureMarkForCertificateAsHtml(Sungero.Domain.Shared.ISignature signature, Sungero.Docflow.Structures.StampSetting.ISignatureStampParams signatureStampParams, Sungero.Docflow.IOfficialDocument document)
    {
      var html = lenspec.Etalon.Module.Docflow.PublicFunctions.Module.GetSignatureMarkForCertificateAsHtml(signature, signatureStampParams);
      
      // Проверяем что документ
      //      if ((Sungero.Docflow.Addendums.Is(document) || lenspec.LocalRegulations.DocumentApprovedByOrders.Is(document)) && document?.LeadingDocument?.BusinessUnit != null)
      //        html = html.Replace("{OurFirmName}", document.LeadingDocument.BusinessUnit.Name); //document.BusinessUnit.Name
      //      else
      html = html.Replace("{OurFirmName}", document?.BusinessUnit?.Name); //document.BusinessUnit.Name
      return html;
    }
    
    /// <summary>
    /// Получить отметку об ЭП для подписи.
    /// </summary>
    /// <param name="signature">Подпись.</param>
    /// Наименование организации должно браться из поля Наша организация самого документа, а не из карточки работника подписанта
    /// <returns>Изображение отметки об ЭП для подписи в виде html.</returns>
    [Public]
    public override string GetSignatureMarkForSimpleSignatureAsHtml(Sungero.Domain.Shared.ISignature signature, Sungero.Docflow.Structures.StampSetting.ISignatureStampParams signatureStampParams)
    {
      if (signature == null)
        return string.Empty;
      
      var employeOurFirm = Sungero.Company.Employees.As(signature.Signatory);
      var ourFirmName = string.Empty;
      if (employeOurFirm != null && employeOurFirm.Department.BusinessUnit != null)
        ourFirmName = employeOurFirm.Department.BusinessUnit.Name;
      
      try
      {
        string html;
        
        using (Sungero.Core.CultureInfoExtensions.SwitchTo(TenantInfo.Culture))
        {
          html = Sungero.Docflow.Resources.HtmlStampTemplateForSignature;
          html = html.Replace("{SignatoryFullName}", signature.SignatoryFullName);
          html = html.Replace("{SignatoryId}", signature.Signatory.Id.ToString());
          html = html.Replace("{Logo}", signatureStampParams.Logo);
          html = html.Replace("{SigningDate}", signatureStampParams.SigningDate);
          //          html = html.Replace("{Title}", lenspec.Etalon.Module.Docflow.Resources.TitleForStamp);
          html = html.Replace("{Title}", Sungero.Docflow.Resources.SignatureStampSampleTitle);
        }
        return html;
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("GetSignatureMarkForSimpleSignatureAsHtml - {0}", ex.Message);
        return string.Empty;
      }
    }
    
    /// <summary>
    /// Заменяет якори в документе на подписи.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public, Remote]
    public static string SearchAndReplaceAnchor(Sungero.Docflow.IOfficialDocument document)
    {
      // получаем утверждающую подпись
      var approvalMark = GetSignatureForApprovalMark(document);
      
      if (approvalMark == null)
      {
        return "На документ не установлена утверждающая подпись.";
      }
      // Получаем согласующие подписи.
      var endorsingMarks = GetSignatureForEndorsingMarks(document);
      
      // Получаем количество якорей в документе.
      var anchorCounts = 0;
      try
      {
        anchorCounts = GetCountAnchor(document);
      }
      catch
      {
      }
      
      var stampSetting = Sungero.Docflow.PublicFunctions.OfficialDocument.GetStampSettings(document).FirstOrDefault();
      
      // Если в документе нету якорей, то стандартным методом преобразовываем в pdf.
      if (anchorCounts == 0)
      {
        // Получение html штампа.
        var signatureMark = lenspec.Etalon.Module.Docflow.PublicFunctions.Module.GetSignatureMarkForCertificateAsHtml(approvalMark, Sungero.Docflow.PublicFunctions.StampSetting.GetSignatureStampParams(stampSetting, approvalMark.SigningDate, false), document);
        if (string.IsNullOrEmpty(signatureMark))
        {
          signatureMark = lenspec.Etalon.Module.Docflow.PublicFunctions.Module.GetSignatureMarkForSimpleSignatureAsHtml(approvalMark, Sungero.Docflow.PublicFunctions.Module.GetDefaultSignatureStampParams(false), document);
        }

        ConvertToPdfAvis(document, signatureMark);
        return "";
      }
      
      // Получаем версию документа
      var version = document.Versions.SingleOrDefault(v => v.Id == document.LastVersion.Id);
      
      using (var inputStream = new System.IO.MemoryStream())
      {
        version.Body.Read().CopyTo(inputStream);
        
        using (var wordDoc = WordprocessingDocument.Open(inputStream, true))
        {
          var html = "";
          
          // Устанавливаем согласующие подписи
          if (endorsingMarks != null)
          {
            foreach(var signature in endorsingMarks)
            {
              // Устанавливаем очередную согласующую подпись
              if (anchorCounts > 1)
              {
                // Получение html штампа
                html = lenspec.Etalon.Module.Docflow.PublicFunctions.Module.GetSignatureMarkForCertificateAsHtml(signature, Sungero.Docflow.PublicFunctions.StampSetting.GetSignatureStampParams(stampSetting, signature.SigningDate, false), document);
                if (string.IsNullOrEmpty(html))
                {
                  html = lenspec.Etalon.Module.Docflow.PublicFunctions.Module.GetSignatureMarkForSimpleSignatureAsHtml(signature, Sungero.Docflow.PublicFunctions.Module.GetDefaultSignatureStampParams(false), document);
                }

                ReplaceAnchor(wordDoc, html);
                anchorCounts--;
              }
            }
          }
          
          // Удаляем лишнии якори, если согласующих подписей не хватило
          while (anchorCounts > 1)
          {
            html = "";
            ReplaceAnchor(wordDoc, html);
            anchorCounts--;
          }
          
          var getSignatureStampParams = Sungero.Docflow.PublicFunctions.StampSetting.GetSignatureStampParams(stampSetting, approvalMark.SigningDate, false);
          // Устанавливаем утверждающую подпись
          html = lenspec.Etalon.Module.Docflow.PublicFunctions.Module.GetSignatureMarkForCertificateAsHtml(approvalMark, getSignatureStampParams, document);
          if (string.IsNullOrEmpty(html))
          {
            html = lenspec.Etalon.Module.Docflow.PublicFunctions.Module.GetSignatureMarkForSimpleSignatureAsHtml(approvalMark, Sungero.Docflow.PublicFunctions.Module.GetDefaultSignatureStampParams(false), document);
          }

          ReplaceAnchor(wordDoc, html);
        }
        
        // Преобразование в pdf
        var pdfConverter = Sungero.AsposeExtensions.Converter.Create();
        var extension = version.BodyAssociatedApplication.Extension;
        var pdfDocumentStream = pdfConverter.GeneratePdf(inputStream, extension);
        
        version.PublicBody.Write(pdfDocumentStream);
        version.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension("pdf");
        document.Save();
      }
      
      return "";
    }
    
    /// <summary>
    /// Сгенерировать PublicBody документа с отметкой об ЭП.
    /// </summary>
    /// <param name="document">Документ для преобразования.</param>
    /// <param name="signatureMark">Отметка об ЭП (html).</param>
    /// <returns>Информация о результате генерации PublicBody для версии документа.</returns>
    public static void ConvertToPdfAvis(Sungero.Docflow.IOfficialDocument document, string signatureMark)
    {
      var versionId = document.LastVersion.Id;
      
      var version = document.Versions.SingleOrDefault(v => v.Id == versionId);
      if (version == null)
      {
        return;
      }
      
      Logger.DebugFormat("Start generate public body for document id {0} version id {1}: document application - {2}, version application - {3}.",
                         document.Id, version.Id, document.AssociatedApplication, version.BodyAssociatedApplication);
      
      System.IO.Stream pdfDocumentStream = null;
      using (var inputStream = new System.IO.MemoryStream())
      {
        version.Body.Read().CopyTo(inputStream);
        
        var pdfConverter = Sungero.AsposeExtensions.Converter.Create();
        var extension = version.BodyAssociatedApplication.Extension;
        pdfDocumentStream = pdfConverter.GeneratePdf(inputStream, extension);
        var htmlStampString = signatureMark;
        if (!string.IsNullOrEmpty(htmlStampString))
        {
          pdfDocumentStream = pdfConverter.AddSignatureMark(pdfDocumentStream, extension, htmlStampString, Docflow.Resources.SignatureMarkAnchorSymbol, 1);
        }
      }
      
      version.PublicBody.Write(pdfDocumentStream);
      version.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension("pdf");
      document.Save();
      pdfDocumentStream.Close();
    }
    
    /// <summary>
    /// Преобразовать в pdf.
    /// </summary>
    /// <param name="isRewriteBody">Перезаписывать оригинальное тело документа (LastVersion.Body)? false - означивается только PublicBody</param>
    /// <param name="document">Документ для преобразования.</param>
    [Public]
    public static void ConvertToPdfAvis(Sungero.Docflow.IOfficialDocument document, bool isRewriteBody)
    {
      // Предпроверки.
      var version = document.LastVersion;
      if (version == null)
      {
        Logger.Error($"ConvertToPdfAvis: У документа отсутствует версия. docId = {document.Id}");
        return;
      }
      
      // Получить тело версии для преобразования в PDF.
      var body = Sungero.Docflow.PublicFunctions.OfficialDocument.GetBodyToConvertToPdf(document, version, false);
      if (body == null || body.Body == null || body.Body.Length == 0)
      {
        Logger.Error($"ConvertToPdfAvis: Версия у документа пуста. doId = {document.Id}, verId = {version.Id}");
        return;
      }
      
      System.IO.Stream pdfDocumentStream = null;
      using (var inputStream = new System.IO.MemoryStream(body.Body))
      {
        try
        {
          pdfDocumentStream = Sungero.Docflow.IsolatedFunctions.PdfConverter.GeneratePdf(inputStream, body.Extension);
        }
        catch (Exception ex)
        {
          if (ex is AppliedCodeException)
            Logger.Error(Docflow.Resources.PdfConvertErrorFormat(document.Id), ex.InnerException);
          else
            Logger.Error(Docflow.Resources.PdfConvertErrorFormat(document.Id), ex);
          return;
        }
      }
      
      // Выключить error-логирование при доступе к зашифрованным бинарным данным/версии.
      AccessRights.SuppressSecurityEvents(
        () =>
        {
          if (isRewriteBody)
          {
            version.PublicBody.Write(pdfDocumentStream);
            version.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension(Sungero.Docflow.Constants.OfficialDocument.Extensions.Pdf);
          }
          else
          {
            document.CreateVersionFrom(pdfDocumentStream, Sungero.Docflow.Constants.OfficialDocument.Extensions.Pdf);
            var lastVersion = document.LastVersion;
          }
        });
      
      pdfDocumentStream.Close();
      document.Save();
      Logger.Debug($"ConvertToPdfAvis: End converting to PDF. docId = {document.Id}, verId = {version.Id}");
    }
    
    /// <summary>
    /// Преобразовать в pdf.
    /// </summary>
    /// <param name="document">Документ для преобразования.</param>
    /// <param name="versionId">Версию документа для преобразования.</param>
    [Public]
    public static void ConvertToPdfAvis(Sungero.Docflow.IOfficialDocument document, long versionId)
    {
      var version = document.Versions.SingleOrDefault(v => v.Id == versionId);
      if (version == null)
        return;
      
      Logger.DebugFormat("Start generate public body for document id {0} version id {1}: document application - {2}, version application - {3}.",
                         document.Id, version.Id, document.AssociatedApplication, version.BodyAssociatedApplication);
      
      System.IO.Stream pdfDocumentStream = null;
      using (var inputStream = new System.IO.MemoryStream())
      {
        version.Body.Read().CopyTo(inputStream);
        
        var pdfConverter = Sungero.AsposeExtensions.Converter.Create();
        var extension = version.BodyAssociatedApplication.Extension;
        pdfDocumentStream = pdfConverter.GeneratePdf(inputStream, extension);
      }
      
      version.PublicBody.Write(pdfDocumentStream);
      version.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension("pdf");
      document.Save();
      pdfDocumentStream.Close();
    }
    
    /// <summary>
    /// Заменяет 1 якорь, на html.
    /// </summary>
    /// <param name="wordDoc">word документ.</param>
    /// <param name="html">html код штампа.</param>
    private static void ReplaceAnchor(DocumentFormat.OpenXml.Packaging.WordprocessingDocument wordDoc, string html)
    {
      var mainDocumentPart = wordDoc.MainDocumentPart;
      var documentBody = mainDocumentPart.Document.Body;
      
      // Находим параграф с якорем.
      var paragraphToReplace = documentBody.Descendants<Paragraph>().FirstOrDefault(x => x.InnerText.Equals("⚓^"));

      // Проверяем что нашли параграф с якорем.
      if (paragraphToReplace == null)
      {
        return;
      }

      // Заменяем якорь на html элемент.
      var random = new Random(); // Требуется для установки рандомного имени для AltChunk, если будут 2 совпадающих названия, то выдаст ошибку.
      var partId = $"id_{random.Next()}";

      var formatImportPart = mainDocumentPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html, partId);
      var htmlAsUtf8Bytes = Encoding.UTF8.GetBytes(html);
      using (var htmlContentStream = new MemoryStream(htmlAsUtf8Bytes))
      {
        formatImportPart.FeedData(htmlContentStream);
      }
      var altChunk = new AltChunk();
      altChunk.Id = partId;

      paragraphToReplace.InsertBeforeSelf(altChunk);
      paragraphToReplace.Remove();
    }
    
    /// <summary>
    /// Получить утверждающую подпись.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Электронная подпись.</returns>
    private static Sungero.Domain.Shared.ISignature GetSignatureForApprovalMark(Sungero.Docflow.IOfficialDocument document)
    {
      var version = document.Versions.FirstOrDefault(x => x.Id == document.LastVersion.Id);
      if (version == null)
        return null;
      
      // Только утверждающие подписи.
      var versionSignatures = Signatures.Get(version)
        .Where(s => s.IsExternal != true && s.SignatureType == SignatureType.Approval)
        .ToList();
      if (!versionSignatures.Any())
        return null;
      
      // В приоритете подпись сотрудника из поля "Подписал". Квалифицированная ЭП приоритетнее простой.
      return versionSignatures
        .OrderByDescending(s => Equals(s.Signatory, document.OurSignatory))
        .ThenBy(s => s.SignCertificate == null)
        .ThenByDescending(s => s.SigningDate)
        .FirstOrDefault();
    }
    
    /// <summary>
    /// Получить согласующие подписи.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Электронные подписи.</returns>
    private static List<Sungero.Domain.Shared.ISignature> GetSignatureForEndorsingMarks(Sungero.Docflow.IOfficialDocument document)
    {
      var version = document.Versions.FirstOrDefault(x => x.Id == document.LastVersion.Id);
      if (version == null)
        return null;
      
      // Получаем утвердающую подпись.
      var approvalMark = GetSignatureForApprovalMark(document);
      
      // Только утверждающие подписи.
      var versionSignatures = Signatures.Get(version)
        .Where(s => s.IsExternal != true && s.SignatureType == SignatureType.Endorsing && s.Signatory != approvalMark.Signatory)
        .OrderByDescending(x=> x.SigningDate)
        .GroupBy(c=> c.Signatory)
        .Select(g => g.First())
        .ToList();
      if (!versionSignatures.Any())
        return null;
      
      return versionSignatures;
    }
    
    #region [Документооборот. Массовое назначение прав на задачи]
    
    /// <summary>
    /// Поставить АО выдачи прав на задачи по документу в очередь на выполнение.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="accessRightsRule">Правило назначения прав.</param>
    /// <remarks>Для обработки единичных документов в АО.</remarks>
    [Public]
    public static void EnqueueGrantAccessRightsToTasks(Sungero.Docflow.IOfficialDocument document, IAccessRightsRule accessRightsRule)
    {
      BulkProcessDocuments(
        new List<Sungero.Docflow.IOfficialDocument>(){ document },
        accessRightsRule,
        true,
        "avis - GrantAccessRightsToTasksAsync"
       );
    }
    
    /// <summary>
    /// Выдать права на задачи.
    /// </summary>
    /// <param name="accessRightsRule">Правило назначения прав.</param>
    /// <param name="executeAsync">Признак асинхронного выполнения.</param>
    /// <param name="prefix">Префикс логирования.</param>
    public static void GrantAccessRightsToTasks(IAccessRightsRule accessRightsRule, bool executeAsync, string prefix)
    {
      Logger.DebugFormat("{0} Started.", prefix);
      if (accessRightsRule == null)
        throw new ArgumentNullException(AccessRightsRules.Info.LocalizedName);
      
      var documents = lenspec.Etalon.PublicFunctions.AccessRightsRule.GetMatchingDocuments(accessRightsRule);
      BulkProcessAllDocuments(documents, accessRightsRule, executeAsync, prefix);
    }
    
    /// <summary>
    /// Отправить все документы на пакетную обработку в АО.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <param name="accessRightsRule">Правило назначения прав.</param>
    /// <param name="executeAsync">Признак асинхронного выполнения.</param>
    /// <param name="prefix">Префикс логирования.</param>
    public static void BulkProcessAllDocuments(IQueryable<Sungero.Docflow.IOfficialDocument> documents, IAccessRightsRule accessRightsRule, bool executeAsync, string prefix)
    {
      if (documents == null)
        return;
      
      var bulkSize = Docflow.Constants.Module.Jobs.DocumentsGrantingBulkSize;
      var bulk = new List<Sungero.Docflow.IOfficialDocument>(bulkSize);
      var bulkCount = 0;
      long totalDocuments = 0;
      // Разделение документов на пакеты.
      foreach(var document in documents)
      {
        totalDocuments++;
        bulk.Add(document);
        if (bulk.Count == bulkSize)
        {
          BulkProcessDocuments(bulk, accessRightsRule, executeAsync, prefix);
          bulk.Clear();
          bulkCount++;
        }
      }
      // "Хвост" оставшихся документов.
      if (bulk.Count > 0)
      {
        BulkProcessDocuments(bulk, accessRightsRule, executeAsync, prefix);
        bulkCount++;
      }

      var totalDocumentsInfo = string.Format("(totally {0} documents)", totalDocuments);
      if (executeAsync)
        Logger.DebugFormat("{0} Enqueued {1} bulks {2}.", prefix, bulkCount , totalDocumentsInfo);
      else
        Logger.DebugFormat("{0} Proceeded {1} bulks {2}.", prefix, bulkCount, totalDocumentsInfo);
    }
    
    /// <summary>
    /// Отправить пакет документов на обработку.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <param name="accessRightsRule">Правило назначения прав.</param>
    /// <param name="executeAsync">Признак асинхронного выполнения.</param>
    /// <param name="prefix">Префикс логирования.</param>
    private static void BulkProcessDocuments(List<Sungero.Docflow.IOfficialDocument> documents, IAccessRightsRule accessRightsRule, bool executeAsync, string prefix)
    {
      var documentIDs = string.Join(";", documents.Select(x => x.Id));
      var ruleID = accessRightsRule.Id;
      if (executeAsync)
        EnqueueBulkProcessDocuments(documentIDs, ruleID);
      else
        GrantAccessRightsToTasksByDocuments(documentIDs, ruleID, prefix);
    }
    
    /// <summary>
    /// Отправить пакет документов на обработку в АО.
    /// </summary>
    /// <param name="documentIDs">ИД Документов.</param>
    /// <param name="ruleID">ИД Правила назначения прав.</param>
    private static void EnqueueBulkProcessDocuments(string documentIDs, long ruleID)
    {
      var asyncHandler = Docflow.AsyncHandlers.GrantAccessRightsToTasksByDocumentsAsynclenspec.Create();
      asyncHandler.DocumentIDs = documentIDs;
      asyncHandler.RuleID = ruleID;
      asyncHandler.ExecuteAsync();
    }
    
    /// <summary>
    /// Выдача прав на задачи по документам.
    /// </summary>
    /// <param name="documentIDsString">ИД документов (в строке).</param>
    /// <param name="ruleID">ИД правила назначения прав.</param>
    /// <param name="prefix">Префикс логирования.</param>
    /// <returns>Признак необходимости повторного запуска (для АО).</returns>
    public static bool GrantAccessRightsToTasksByDocuments(string documentIDsString, long ruleID, string prefix)
    {
      var rule = lenspec.Etalon.AccessRightsRules.GetAll(x => x.Id == ruleID).SingleOrDefault();
      if (rule == null)
      {
        Logger.ErrorFormat("{0} {1} with ID {2} not found.", prefix, AccessRightsRules.Info.LocalizedName, ruleID);
        return false;
      }
      var documentIDs = documentIDsString.Split(';').Select(
        x =>
        {
          long id = -1;
          if (!long.TryParse(x, out id))
            Logger.ErrorFormat("{0} Failed to parse ID from string: {1}.", prefix, x);
          return id;
        }
       );
      var tasks = Sungero.Workflow.Tasks.GetAll();
      var needRetry = false;
      var proceededCount = 0;
      var errorsCount = 0;
      // Выдача прав на задачи.
      foreach (var task in tasks)
      {
        // Фильтруем задачи по наличию документов во вложениях.
        if (!task.AllAttachments.Where(a => documentIDs.Contains(a.Id)).Any(a => Sungero.Docflow.OfficialDocuments.Is(a)))
          continue;
        
        proceededCount++;
        var error = GrantRightsToTaskByDocument(task, rule.Members.ToList());
        if (string.IsNullOrEmpty(error))
          continue;
        
        errorsCount++;
        Logger.ErrorFormat("{0} {1}", prefix,  error);
        needRetry = true;
      }
      
      Logger.DebugFormat("{0} Chunk processing done: granted rights on {1}/{2} tasks. Errors: {3}.", prefix, proceededCount - errorsCount, proceededCount, errorsCount);
      return needRetry;
    }
    
    /// <summary>
    /// Выдача прав на задачу участникам из правила.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="members">Группы и сотрудники.</param>
    /// <returns>Null при успешной обработке; Иначе – текст ошибки.</returns>
    private static string GrantRightsToTaskByDocument(Sungero.Workflow.ITask task, List<Sungero.Docflow.IAccessRightsRuleMembers> members)
    {
      var isChanged = false;
      try
      {
        foreach (var member in members)
        {
          var isRead = task.AccessRights.CanRead(member.Recipient);
          var isForbidden = task.AccessRights.IsGranted(DefaultAccessRightsTypes.Forbidden, member.Recipient);
          if (isRead || isForbidden)
            continue;
          
          isChanged = true;
          task.AccessRights.Grant(member.Recipient, DefaultAccessRightsTypes.Read);
        }
        if (!isChanged)
          return null;

        foreach (var property in task.State.Properties)
          property.IsRequired = false;
        task.AccessRights.Save();
      }
      catch (Exception ex)
      {
        return string.Format("Failed to grant rights on {0} ID {1}: {2}", Sungero.Workflow.Tasks.Info.LocalizedName, task.Id, ex.Message);
      }
      return null;
    }
    
    #endregion [Документооборот. Массовое назначение прав на задачи]
  }
}