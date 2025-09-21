using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OutgoingLetter;
using Sungero.Domain.Shared;
using Sungero.Metadata;

namespace lenspec.Etalon.Server
{
  partial class OutgoingLetterFunctions
  {
    //Добавлено Avis Expert
    
    /// <summary>
    /// Создать архив с телом и подписями
    /// </summary>
    /// <param name="document">Документ</param>
    /// <param name="signFormat">Формат подписи без точки (sig или sgn)</param>
    /// <returns>zip</returns>
    [Remote, Public]
    public static IZip GetZipWithSignaturies(Sungero.Docflow.IOfficialDocument document, string signFormat)
    {
      try
      {
        var zip = Zip.Create();
        var version = document.LastVersion;
        
        var body = version.PublicBody != null && version.PublicBody.Size != 0 ? version.PublicBody : version.Body;
        var extension = version.PublicBody != null && version.PublicBody.Size != 0 ? version.AssociatedApplication.Extension : version.BodyAssociatedApplication.Extension;
        var fileName = string.Format("{0}.{1}", document.Name, extension);
        zip.Add(body, fileName, string.Empty);
        
        var signature = Signatures.Get(version).FirstOrDefault(x => x.SignCertificate != null);
        var dataSign = signature.GetDataSignature();
        zip.Add(dataSign, signature.Signatory.Name, signFormat, string.Empty);
        
        zip.Save(document.Name);
        return zip;
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Ошибка экспорта исходящего письма и подписи - {0}. {1}", ex.Message, innerMessage);
        return null;
      }
    }
    
    /// <summary>
    /// Получить связанные документы по типу связи.
    /// </summary>
    /// <param name="document">Документ, для которого получаются связанные документы.</param>
    /// <param name="relationTypeName">Наименование типа связи.</param>
    /// <param name="withVersion">Учитывать только документы с версиями.</param>
    /// <returns>Связанные документы.</returns>
    [Remote]
    public static List<Sungero.Docflow.IOfficialDocument> GetRelatedDocumentsByRelationType(Sungero.Docflow.IOfficialDocument document, string relationTypeName, bool withVersion)
    {
      return Sungero.Docflow.Server.OfficialDocumentFunctions.GetRelatedDocumentsByRelationType(document, relationTypeName, withVersion);
    }
    
    /// <summary>
    /// Получить значение эл. почты из карточки Контактного лица или его Персоны (при наличии).
    /// </summary>
    /// <param name="contact">Контактное лицо.</param>
    /// <returns>Значение поля Эл. почта.</returns>
    [Remote]
    public static string GetContactEmail(Sungero.Parties.IContact contact)
    {
      if (contact == null)
        return string.Empty;
      
      var email = string.Empty;
      if (!string.IsNullOrEmpty(contact.Email))
      {
        email = contact.Email;
      }
      else if (contact.Person != null && !string.IsNullOrEmpty(contact.Person.Email))
      {
        email = contact.Person.Email;
      }
      return email;
    }
    
    /// <summary>
    /// Получить список корреспондентов и адресатов для шаблона исходящего письма.
    /// </summary>
    /// <param name="document">Исходящее письмо.</param>
    /// <returns>Список корреспондентов и адресатов.</returns>
    [Public, Sungero.Core.Converter("GetCorrespondentsAndAddressees")]
    public static string GetCorrespondentsAndAddressees(IOutgoingLetter document)
    {
      var result = string.Empty;
      // Если действие «Несколько адресатов» не активно.
      if (document.IsManyAddressees != true)
      {
        if (document.Addressee != null)
        {
          if (document.Addressee.JobTitle != null)
          {
            result += CaseConverter.ConvertJobTitleToTargetDeclension(document.Addressee.JobTitle, Sungero.Core.DeclensionCase.Dative);
            result += Environment.NewLine;
          }
          if (document.Addressee.Company != null)
          {
            result += document.Addressee.Company.Name;
            result += Environment.NewLine;
          }
          var personFullName = Sungero.Docflow.Server.OfficialDocumentFunctions.LastNameAndInitials(document.Addressee);
          result += CaseConverter.ConvertPersonFullNameToTargetDeclension(personFullName, Sungero.Core.DeclensionCase.Dative);
          result += Environment.NewLine;
        }
        else
        {
          if (Sungero.Parties.CompanyBases.Is(document.Correspondent))
          {
            result += document.Correspondent.Name;
            result += Environment.NewLine;
          }
          if (Sungero.Parties.People.Is(document.Correspondent))
          {
            result += Sungero.Parties.People.As(document.Correspondent).ShortName;
            result += Environment.NewLine;
          }
        }
        if (!string.IsNullOrEmpty(document.AddressOfRecipientlenspec) && document.DeliveryMethod != null &&
            !document.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) && !document.DeliveryMethod.Name.Trim().Equals("Электронная почта"))
        {
          result += document.AddressOfRecipientlenspec;
        }
      }
      // Если активно действие «Несколько адресатов».
      else
      {
        if (document.Addressees.Count() > Sungero.Docflow.Constants.Module.AddresseesShortListLimit)
        {
          result = lenspec.Etalon.OutgoingLetters.Resources.ToManyAddressees;
        }
        else
        {
          var addressees = document.GetChildCollectionPropertyValue(document.Info.Properties.Addressees.Name);
          foreach (Etalon.IOutgoingLetterAddressees addressee in addressees)
          {
            if (addressee.Addressee != null)
            {
              if (addressee.Addressee.JobTitle != null)
              {
                result += CaseConverter.ConvertJobTitleToTargetDeclension(addressee.Addressee.JobTitle, Sungero.Core.DeclensionCase.Dative);
                result += Environment.NewLine;
              }
              if (addressee.Addressee.Company != null)
              {
                result += addressee.Addressee.Company.Name;
                result += Environment.NewLine;
              }
              var personFullName = Sungero.Docflow.Server.OfficialDocumentFunctions.LastNameAndInitials(addressee.Addressee);
              result += CaseConverter.ConvertPersonFullNameToTargetDeclension(personFullName, Sungero.Core.DeclensionCase.Dative);
              result += Environment.NewLine;
            }
            else
            {
              if (Sungero.Parties.CompanyBases.Is(addressee.Correspondent))
              {
                result += addressee.Correspondent.Name;
                result += Environment.NewLine;
              }
              if (Sungero.Parties.People.Is(addressee.Correspondent))
              {
                var personFullName = Sungero.Docflow.Server.OfficialDocumentFunctions.LastNameAndInitials(Sungero.Parties.People.As(addressee.Correspondent));
                result += CaseConverter.ConvertPersonFullNameToTargetDeclension(personFullName, Sungero.Core.DeclensionCase.Dative);
                result += Environment.NewLine;
              }
            }
            if (!string.IsNullOrEmpty(addressee.PostalAddressavis) && addressee.DeliveryMethod != null &&
                !addressee.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) && !addressee.DeliveryMethod.Name.Trim().Equals("Электронная почта"))
            {
              result += addressee.PostalAddressavis;
              result += Environment.NewLine;
            }
            result += Environment.NewLine;
          }
        }
      }
      return result.Trim();
    }
    
    /// <summary>
    /// Получить Список адресов нескольких адресатов для шаблона исходящего письма.
    /// </summary>
    /// <param name="document">Исходящее письмо.</param>
    /// <returns>Список адресов нескольких адресатов.</returns>
    [Public, Sungero.Core.Converter("GetAddresseesForManyAddressees")]
    public static string GetAddressees(IOutgoingLetter document)
    {
      var result = string.Empty;
      if (document.DeliveryMethod != null &&
          !document.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) &&
          !document.DeliveryMethod.Name.Trim().Equals("Электронная почта"))
      {
        if (document.IsManyAddressees != true)
        {
          result = document.AddressOfRecipientlenspec;
        }
        else
        {
          if (document.Addressees.Count() > Sungero.Docflow.Constants.Module.AddresseesShortListLimit)
          {
            result = lenspec.Etalon.OutgoingLetters.Resources.ToManyAddressees;
          }
          else
          {
            var addressees = document.GetChildCollectionPropertyValue(document.Info.Properties.Addressees.Name);
            foreach (Etalon.IOutgoingLetterAddressees addressee in addressees)
            {
              //брать адрес доставки на вкладке Адресаты
              if (!string.IsNullOrEmpty(addressee.PostalAddressavis))
              {
                result += addressee.PostalAddressavis;
                result += Environment.NewLine;
              }
            }
          }
        }
      }
      return result.Trim();
    }
    
    /// <summary>
    /// Получить Список эл. адресов для шаблона исходящего письма.
    /// </summary>
    /// <param name="document">Исходящее письмо.</param>
    /// <returns>Список эл. адресов.</returns>
    [Public, Sungero.Core.Converter("GetEmailAddresses")]
    public static string GetEmailAddresses(IOutgoingLetter document)
    {
      var result = string.Empty;
      if (document.DeliveryMethod != null &&
          (document.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) ||
           document.DeliveryMethod.Name.Trim().Equals("Электронная почта")))
      {
        if (document.IsManyAddressees != true && !string.IsNullOrEmpty(document.Emaillenspec))
        {
          result = document.Emaillenspec;
        }
        else
        {
          if (document.Addressees.Count() > Sungero.Docflow.Constants.Module.AddresseesShortListLimit)
          {
            result = lenspec.Etalon.OutgoingLetters.Resources.ToManyAddressees;
          }
          else
          {
            var addressees = document.GetChildCollectionPropertyValue(document.Info.Properties.Addressees.Name);
            foreach (Etalon.IOutgoingLetterAddressees addressee in addressees)
            {
              //брать эл. почту на вкладке Адресаты
              if (!string.IsNullOrEmpty(addressee.AddresseeEmailavis))
              {
                result += addressee.AddresseeEmailavis;
                result += Environment.NewLine;
              }
            }
          }
        }
      }
      return result.Trim();
    }
    
    /// <summary>
    /// Заполнение поля "Долдность подписанта для шаблонов".
    /// </summary>
    [Public]
    public static void FieldJobTitle(lenspec.Etalon.IOutgoingLetter letter)
    {
      // Если поле Представитель заполнено, и оно еще не находится в поле Должность. То заполняем.
      if (string.IsNullOrEmpty(letter.RepresentativeByPowerOfAttorneyNumberlenspec) == false)
      {
        letter.JobTitleavis = $"Представитель по доверенности № {letter.RepresentativeByPowerOfAttorneyNumberlenspec}";
        if (letter.PowerOfAttorneyDatelenspec != null)
          letter.JobTitleavis += $" от {letter.PowerOfAttorneyDatelenspec.Value.ToString("dd.MM.yyyy")}";
        
        return;
      }
      
      if (letter.OurSignatory != null)
      {
        letter.JobTitleavis = letter?.OurSignatory?.JobTitle?.Name;
        return;
      }
      
      letter.JobTitleavis = "";
    }
    //конец Добавлено Avis Expert
  }
}