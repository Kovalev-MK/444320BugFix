using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.PowerOfAttorneyBase;

namespace lenspec.Etalon.Server
{
  partial class PowerOfAttorneyBaseFunctions
  {

    [Public, Converter("PassportDetailsTemplateField")]
    public static string GetAttorneysPassportDetails(lenspec.Etalon.IPowerOfAttorneyBase document)
    {
      var passportDetails = string.Empty;
      var passportRFKind = Sungero.Parties.IdentityDocumentKinds.GetAll(x => Sungero.Parties.PublicConstants.Module.IdentityDocumentKindsGuid.CitizenPassport == x.SID).SingleOrDefault();
      var persons = document.Representatives.Where(x => x.IssuedTo != null && Sungero.Parties.People.Is(x.IssuedTo)).Select(x => Sungero.Parties.People.As(x.IssuedTo));
      foreach (var person in persons)
      {
        if (Equals(person.IdentityKind, passportRFKind))
        {
          var sex = Sungero.Parties.People.Info.Properties.Sex.GetLocalizedValue(person.Sex);
          var dateOfBirth = person.DateOfBirth.HasValue ? person.DateOfBirth.Value.ToShortDateString() : string.Empty;
          var identityDateOfIssue = person.IdentityDateOfIssue.HasValue ? person.IdentityDateOfIssue.Value.ToShortDateString() : string.Empty;
          var personFullName = CommonLibrary.PersonFullName.Create(person.LastName, person.FirstName, person.MiddleName, CommonLibrary.PersonFullNameDisplayFormat.Full);
          var personNameDeclension = CaseConverter.ConvertPersonFullNameToTargetDeclension(personFullName, DeclensionCase.Accusative);
          var passportDetailsRow = string.Format("{0}\n(пол: {1}, дата рождения: {2}, место рождения: {3}, паспорт гражданина Российской Федерации {4} {5}, паспорт выдан {6}, дата выдачи {7}, код подразделения: {8})",
                                                 personNameDeclension, sex, dateOfBirth, person.BirthPlace, person.IdentitySeries,
                                                 person.IdentityNumber, person.IdentityAuthority, identityDateOfIssue, person.IdentityAuthorityCode);
          
          passportDetails += string.IsNullOrEmpty(passportDetails) ? passportDetailsRow : System.Environment.NewLine + System.Environment.NewLine + passportDetailsRow;
        }
      }
      return passportDetails;
    }
  }
}