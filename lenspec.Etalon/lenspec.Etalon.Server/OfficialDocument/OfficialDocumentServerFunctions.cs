using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OfficialDocument;
using lenspec.Etalon.DocumentRegister;

namespace lenspec.Etalon.Server
{
  partial class OfficialDocumentFunctions
  {
    
    //Добавлено Avis Expert
    
    /// <summary>
    /// Создать документ из вычисляемой папки "Документы по квалификации".
    /// </summary>   
    /// <param name="documentKind">Вид документа.</param>
    [Remote]
    public static IOfficialDocument CreateQualificationDocument(Sungero.Docflow.IDocumentKind documentKind)
    {
      IOfficialDocument document;
      
      if (documentKind.DocumentType.DocumentTypeGuid == lenspec.Tenders.PublicConstants.Module.TenderCommitteeProtocolTypeGuid)
        document = Tenders.TenderCommitteeProtocols.Create();
      else if (documentKind.DocumentType.DocumentTypeGuid == lenspec.Tenders.PublicConstants.Module.TenderDocumentTypeGuid)
        document = Tenders.TenderDocuments.Create();
      else if (documentKind.DocumentType.DocumentTypeGuid == lenspec.Etalon.PublicConstants.Docflow.CounterpartyDocument.CouterpartyDocumentTypeGuid)
        document = Sungero.Docflow.CounterpartyDocuments.Create();
      else if (documentKind.DocumentType.DocumentTypeGuid == lenspec.Tenders.PublicConstants.Module.TenderQualificationFormTypeGuid)
        document = Tenders.TenderAccreditationForms.Create();
      else
        throw new ArgumentException(lenspec.Etalon.OfficialDocuments.Resources.NoHandlerForDocumentKind);
      
      document.IsQualificationDocumentlenspec = true;
      document.DocumentKind = documentKind;
      return document;
    }
    
    /// <summary>
    /// Преобразовать версию документа в PDF и поставить отметку об ЭП.
    /// </summary>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Пустая строка при успешном преобразовании; Иначе – текст ошибки.</returns>
    [Remote]
    public virtual string ConvertToPdfAndAddSignatureMarkAvis(long versionId)
    {
      var versionIdSignature = 0L;
      
      foreach (var version in _obj.Versions.OrderByDescending(v => v.Number))
      {
        var signature = this.GetSignatureForMark(version.Id, false);
        if (signature == null)
          continue;
        
        versionIdSignature = version.Id;
        break;
      }
      
      if (versionIdSignature == 0)
        return "Не найдено утверждающей подписи не на одной из версий.";
      
      var signatureMark = this.GetSignatureMarkAsHtml(versionIdSignature);
      this.GeneratePublicBodyWithSignatureMark(versionId, signatureMark);
      
      return string.Empty;
    }
    
    /// <summary>
    /// Получить максимальный тип прав на документ, которые текущий пользователь может выдать.
    /// </summary>
    /// <returns>Guid типа прав. Guid.Empty, если текущий пользователь не может выдавать права на документ.</returns>
    [Public]
    public override Guid GetAvailableAccessRights()
    {
      return base.GetAvailableAccessRights();
    }
    
    /// <summary>
    /// Скопировать права из текущего документа в указанный.
    /// </summary>
    /// <param name="document">Документ, в который копируются права.</param>
    /// <param name="accessRightsLimit">Максимальный тип прав, который может быть выдан. Guid.Empty, если устанавливать максимальный уровень прав не требуется.</param>
    [Public]
    public override void CopyAccessRightsToDocument(Sungero.Docflow.IOfficialDocument document, Guid accessRightsLimit)
    {
      base.CopyAccessRightsToDocument(document, accessRightsLimit);
    }
    
    /// <summary>
    /// Признак того, что формат номера не надо валидировать.
    /// </summary>
    /// <returns>True, если формат номера неважен.</returns>
    [Remote(IsPure = true)]
    public override bool IsNumberValidationDisabled()
    {
      return base.IsNumberValidationDisabled();
    }
    
    /// <summary>
    /// Получить все данные для отображения диалога регистрации.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="operation">Операция.</param>
    /// <returns>Параметры диалога.</returns>
    [Remote(IsPure = true)]
    public static Sungero.Docflow.Structures.OfficialDocument.IDialogParamsLite GetRegistrationDialogParamsAvis(Sungero.Docflow.IOfficialDocument document, Enumeration operation)
    {
      return Sungero.Docflow.Server.OfficialDocumentFunctions.GetRegistrationDialogParams(document, operation);
    }
    
    /// <summary>
    /// Преобразовать документ в PDF с наложением отметки об ЭП.
    /// </summary>
    /// <returns>Результат преобразования.</returns>
    [Remote]
    public virtual lenspec.Etalon.Structures.Docflow.OfficialDocument.СonversionToPdfResultAvis ConvertToPdfWithSignatureMarkAvis()
    {
      var versionId = _obj.LastVersion.Id;
      var result = lenspec.Etalon.Structures.Docflow.OfficialDocument.СonversionToPdfResultAvis.Create();
      var info = this.ValidateDocumentBeforeConversion(versionId);
      
      result.HasErrors = info.HasErrors;
      result.ErrorTitle = info.ErrorTitle;
      result.ErrorMessage = info.ErrorMessage;
      
      if (result.HasErrors)
        return result;// info;
      
      // Документ МКДО.
      if (this.IsExchangeDocument(versionId))
      {
        Sungero.Exchange.PublicFunctions.Module.Remote.GeneratePublicBody(_obj.Id);
        result.IsOnConvertion = true;
        result.HasErrors = false;
      }
      else if (this.CanConvertToPdfInteractively())
      {
        // Способ преобразования: интерактивно.
        var test = lenspec.Etalon.Module.Docflow.PublicFunctions.Module.Remote.SearchAndReplaceAnchor(_obj);

        result.IsFastConvertion = true;
        result.ErrorTitle = OfficialDocuments.Resources.ConvertionErrorTitleBase;
      }
      else
      {
        // Способ преобразования: асинхронно.
        var asyncConvertToPdf = Sungero.Docflow.AsyncHandlers.ConvertDocumentToPdf.Create();
        asyncConvertToPdf.DocumentId = _obj.Id;
        asyncConvertToPdf.VersionId = versionId;
        asyncConvertToPdf.UserId = Users.Current.Id;
        asyncConvertToPdf.ExecuteAsync();
        
        result.IsOnConvertion = true;
        result.HasErrors = false;
      }
      
      return result;// info;
    }
    
    
    /// <summary>
    /// Преобразовать документ в PDF с наложением отметки об ЭП.
    /// </summary>
    /// <returns>Пустая строка при успешном преобразовании; Иначе – текст ошибки.</returns>
    [Remote]
    public virtual string ConvertToPdfWithSignatureMarkAvis(long versionId)
    {
      var error = this.ConvertToPdfAndAddSignatureMarkAvis(versionId);

      return error;
    }
    
    /// <summary>
    /// Преобразовать версию документа в PDF со штампом.
    /// </summary>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Пустая строка при успешном преобразовании; Иначе – текст ошибки.</returns>
    [Public, Remote(IsPure = false)]
    public virtual string ConvertToPdfWithStamp(long versionId)
    {
      string stampHtml = lenspec.Etalon.Module.Docflow.Resources.HtmlStampForAutoGenerated;
      string image = lenspec.Etalon.Module.Docflow.Resources.RxLogoBase64;
      
      stampHtml = stampHtml.Replace("{Image}", image);
      var conversionResult = this.GeneratePublicBodyWithSignatureMark(versionId, stampHtml);
      
      if (!conversionResult.HasErrors)
        return string.Empty;
      
      return lenspec.Etalon.OfficialDocuments.Resources.ConversionToPdfErrorFormat(conversionResult.ErrorMessage);
    }
    
    /// <summary>
    /// Преобразовать версию документа в PDF и проставить штамп.
    /// </summary>
    /// <param name="versionId">ИД версии.</param>
    /// <returns>Результат преобразования в PDF.</returns>
    [Remote]
    public virtual string ConvertToPdfAndAddStamp(long versionId)
    {
      var signatureMark = this.GetSignatureMarkAsHtml(versionId);
      this.GeneratePublicBodyWithSignatureMark(versionId, signatureMark);
      
      return string.Empty;
    }
    
    // TODO: Закомментировано простановка штампа в новую версию!!!
    /// <summary>
    /// Преобразовать в PDF с отметкой о регистрации в новую версию документа.
    /// </summary>
    /// <param name="versionId">ИД преобразуемой версии.</param>
    /// <param name="registrationStamp">Отметка о регистрации (html).</param>
    /// <param name="rightIndent">Значение отступа справа.</param>
    /// <param name="bottomIndent">Значение отступа снизу.</param>
    /// <returns>Информация о результате создания новой версии документа в PDF.</returns>
    /*
    public override Sungero.Docflow.Structures.OfficialDocument.ConversionToPdfResult ConvertToPdfAndAddRegistrationStamp(long versionId, string registrationStamp, double rightIndent, double bottomIndent)
    {
      // Предпроверки.
      var result = Sungero.Docflow.Structures.OfficialDocument.СonversionToPdfResult.Create();
      result.HasErrors = true;
      var version = _obj.Versions.SingleOrDefault(v => v.Id == versionId);
      if (version == null)
      {
        result.HasConvertionError = true;
        result.ErrorMessage = OfficialDocuments.Resources.NoVersionWithNumberErrorFormat(versionId);
        return result;
      }
      
      Logger.DebugFormat("Start Convert to PDF: document id {0}, version id {1}: application - {2}, original application - {3}.",
                         _obj.Id, version.Id, _obj.AssociatedApplication, version.BodyAssociatedApplication);
      
      // Чтобы не потерять текстовый слой в pdf документе, который может находиться в публичном теле после интеллектуальной обработки.
      // Отметку о поступлении проставлять на публичное тело последней версии документа, если оно есть.
      var documentBody = version.Body;
      var extension = version.BodyAssociatedApplication.Extension;
      
      System.IO.Stream pdfDocumentStream = null;
      using (var inputStream = new System.IO.MemoryStream())
      {
        documentBody.Read().CopyTo(inputStream);
        try
        {
          var pdfConverter = Sungero.AsposeExtensions.Converter.Create();
          pdfDocumentStream = pdfConverter.GeneratePdf(inputStream, extension);
          if (!string.IsNullOrEmpty(registrationStamp))
          {
            pdfDocumentStream = pdfConverter.AddRegistrationStamp(pdfDocumentStream, registrationStamp, 1, rightIndent, bottomIndent);
          }
        }
        catch (Exception e)
        {
          if (e is Sungero.AsposeExtensions.PdfConvertException)
            Logger.Error(Sungero.Docflow.Resources.PdfConvertErrorFormat(_obj.Id), e.InnerException);
          else
            Logger.Error(string.Format("{0} {1}", Sungero.Docflow.Resources.PdfConvertErrorFormat(_obj.Id), e.Message));
          
          result.HasConvertionError = true;
          result.HasLockError = false;
          result.ErrorMessage = Sungero.Docflow.Resources.DocumentBodyNeedsRepair;
        }
      }
      
      if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
        return result;
      
      
      version.PublicBody.Write(pdfDocumentStream);
      version.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension("pdf");
      
      pdfDocumentStream.Close();
      Logger.DebugFormat("{4}: document id {0}, version id {1}: application - {2}, original application - {3}.",
                         _obj.Id, version.Id, _obj.AssociatedApplication, version.BodyAssociatedApplication,
                         "Generate public body");
      
      try
      {
        var paramToWriteInHistory = Sungero.Docflow.PublicConstants.OfficialDocument.AddHistoryCommentAboutPDFConvert;
        ((Sungero.Domain.Shared.IExtendedEntity)_obj).Params[paramToWriteInHistory] = true;
        _obj.Save();
        ((Sungero.Domain.Shared.IExtendedEntity)_obj).Params.Remove(paramToWriteInHistory);
        
        Sungero.Docflow.Functions.OfficialDocument.PreparePreview(_obj);
        
        result.HasErrors = false;
      }
      catch (Sungero.Domain.Shared.Exceptions.RepeatedLockException e)
      {
        Logger.Error(e.Message);
        result.HasConvertionError = false;
        result.HasLockError = true;
        result.ErrorMessage = e.Message;
      }
      catch (Exception e)
      {
        Logger.Error(e.Message);
        result.HasConvertionError = true;
        result.HasLockError = false;
        result.ErrorMessage = e.Message;
      }

      Logger.DebugFormat("End Convert to PDF document id {0} version id {1}: application - {2}, original application - {3}.",
                         _obj.Id, version.Id, _obj.AssociatedApplication, version.BodyAssociatedApplication);
      
      return result;
    }*/
    
    //конец Добавлено Avis Expert
  }
}