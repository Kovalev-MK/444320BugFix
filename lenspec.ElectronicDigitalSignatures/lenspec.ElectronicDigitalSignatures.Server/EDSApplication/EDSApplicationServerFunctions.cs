using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.EDSApplication;

namespace lenspec.ElectronicDigitalSignatures.Server
{
  partial class EDSApplicationFunctions
  {
    
    /// <summary>
    /// Создать Документ УКЭП с видом Паспорт РФ.
    /// </summary>
    /// <returns>Документ УКЭП с видом Паспорт РФ.</returns>
    [Remote]
    public lenspec.ElectronicDigitalSignatures.IEDSDocument CreatePassport()
    {
      var documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.PassportKind);
      var passport = lenspec.ElectronicDigitalSignatures.EDSDocuments.Create();
      passport.DocumentKind = documentKind;
      passport.Person = _obj.PreparedBy?.Person;
      passport.Save();
      
      _obj.Relations.Add(Sungero.Docflow.PublicConstants.Module.AddendumRelationName, passport);
      passport.AccessRights.Grant(_obj.PreparedBy, DefaultAccessRightsTypes.Read);
      passport.AccessRights.Save();
      
      return passport;
    }

    /// <summary>
    /// Создать Согласие на обработку персональных данных.
    /// </summary>
    /// <returns>Согласие на обработку персональных данных.</returns>
    [Remote]
    public Sungero.Core.IZip CreateConsentToProcessing(Sungero.Docflow.IDocumentKind documentKind, Sungero.Docflow.IDocumentTemplate template)
    {
      try
      {
        var zip = Sungero.Core.Zip.Create();
        
        var document = ElectronicDigitalSignatures.EDSDocuments.CreateFrom(template);
        document.DocumentKind = documentKind;
        document.Person = _obj.PreparedBy?.Person;
        document.Save();
        
        _obj.Relations.Add(Sungero.Docflow.PublicConstants.Module.AddendumRelationName, document);
        document.AccessRights.Grant(_obj.PreparedBy, DefaultAccessRightsTypes.Read);
        document.AccessRights.Save();
        
        var version = document.LastVersion;
        var body = version.PublicBody != null && version.PublicBody.Size != 0 ? version.PublicBody : version.Body;
        var extension = version.PublicBody != null && version.PublicBody.Size != 0 ? version.AssociatedApplication.Extension : version.BodyAssociatedApplication.Extension;
        var fileName = string.Format("{0}.{1}", document.Name, extension);
        zip.Add(body, fileName, string.Empty);
        
        string zipName = lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.ConsentToProcessingArchiveNameFormat(_obj.PreparedBy?.Person?.ShortName);
        string tempFolderName = CommonLibrary.FileUtils.NormalizeFileName(zipName);
        zip.Save(tempFolderName);
        
        return zip;
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - CreateConsentToProcessing - ", ex);
        return null;
      }
    }
  }
}