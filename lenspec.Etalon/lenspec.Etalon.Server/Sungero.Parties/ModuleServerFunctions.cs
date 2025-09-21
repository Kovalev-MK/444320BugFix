using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Parties.Server
{
  partial class ModuleFunctions
  {
    #region Выгрузка КА в 1С
    
    /// <summary>
    /// Выгрузка контрагента в 1С.
    /// </summary>
    /// <param name="company">Контрагент.</param>
    /// <returns>Сообщение об ошибке.</returns>
    [Public]
    public string UnloadCounterpartyInto1C(Sungero.Parties.ICounterparty counterparty)
    {
      var errorMessage = string.Empty;
      if (counterparty == null)
        return errorMessage;
      
      var settings = Integrationses.GetAll(s => s.Code == avis.Integration.Constants.Module.OrgSructureImportRecordCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        return lenspec.Etalon.Module.Parties.Resources.ConnectionStringNotFound;
      
      string connectionString = Encryption.Decrypt(settings.ConnectionParams);
      
      var person = lenspec.Etalon.People.As(counterparty);
      var company = lenspec.Etalon.Companies.As(counterparty);
      var bank = lenspec.Etalon.Banks.As(counterparty);
      
      if (person != null)
        errorMessage = UnloadPersonInto1C(person, connectionString);
      else if (company != null)
        errorMessage = UnloadCompanyInto1C(company, connectionString);
      else if (bank != null)
        errorMessage = UnloadBankInto1C(bank, connectionString);
      
      return errorMessage;
    }
    
    /// <summary>
    /// Выгрузка контрагента-персону в 1С.
    /// </summary>
    /// <param name="company">Контрагент-персона.</param>
    /// <returns>Сообщение об ошибке.</returns>
    [Public]
    public string UnloadPersonInto1C(lenspec.Etalon.IPerson company, string connectionString)
    {
      var errorMessage = string.Empty;
      if (company == null)
        return errorMessage;
      
      try
      {
        int action          = 2;
        string directumID   = company.ExternalCodeavis;
        if (string.IsNullOrEmpty(directumID))
          directumID         = "-";
        string inn          = company.TIN;
        if (string.IsNullOrEmpty(inn))
          inn         = "-";
        string legAddress   = company.LegalAddress;
        if (string.IsNullOrEmpty(legAddress))
          legAddress      = "-";
        string stPostalAddress = company.PostalAddress;
        if (string.IsNullOrEmpty(stPostalAddress))
          stPostalAddress = "-";
        DateTime modtime    = Calendar.Now;
        var stOrgCode = String.Empty;
        int phys = 1;
        string fam  = company.LastName;
        if(string.IsNullOrEmpty(fam))
          fam =  "-";
        string name = company.FirstName;
        if(string.IsNullOrEmpty(name))
          name =  "-";
        string otch = company.MiddleName;
        if(string.IsNullOrEmpty(otch))
          otch = "-";
        DateTime dateReg =  new DateTime(1900, 1, 1);
        string series = company.IdentitySeries;
        if(string.IsNullOrEmpty(series))
          series = "-";
        string number = company.IdentityNumber;
        if(string.IsNullOrEmpty(number))
          number = "-";
        DateTime? dateVid = new DateTime(1900, 1, 1);
        if (company.IdentityDateOfIssue != null)
          dateVid = company.IdentityDateOfIssue;
        string placeVid = company.IdentityAuthority;
        if(string.IsNullOrEmpty(placeVid))
          placeVid = "-";
        string codePodr = company.IdentityAuthorityCode;
        if(string.IsNullOrEmpty(codePodr))
          codePodr  = "-";
        string snils = company.INILA;
        if(string.IsNullOrEmpty(snils))
          snils = "-";
        DateTime? dateOfBirth = new DateTime(1900, 1, 1);
        if (company.DateOfBirth != null)
          dateOfBirth = company.DateOfBirth;
        var gender = 1;
        if ( company.Sex.Value != lenspec.Etalon.Person.Sex.Male)
          gender = 0;
        long dirRXID  = company.Id;
        DateTime? validity = new DateTime(1900, 1, 1);
        if (company.IdentityExpirationDate != null)
          validity = company.IdentityExpirationDate;
        string nameAn         = string.Join(" ", ((IEnumerable<string>) new[] {company.LastName, company.FirstName, company.MiddleName}).Where(s => !string.IsNullOrEmpty(s)));
        string legNameAn      = nameAn;
        string stTelephone    = "-";
        string ogrn           = "-";
        string orgGroup       = "-";
        string orgType        = "-";
        int resident          = 0;
        string residentCode   = "-";
        string placeOfBirth   = "-";
        long? headOrg         = 0;
        string kpp            = "-";
        string region         = "-";
        string rx             = "TRUE";
        errorMessage = AvisIntegrationHelper.DataBaseHelper.UnloadingCompanies(connectionString, directumID, nameAn, legNameAn, inn, kpp, ogrn, orgGroup, orgType,
                                                                               resident, residentCode, region, legAddress, stPostalAddress, headOrg, modtime, stOrgCode, stTelephone,
                                                                               phys, fam, name, otch, dateReg, series, number, dateVid, placeVid, codePodr, gender, snils,
                                                                               dateOfBirth, placeOfBirth, validity, dirRXID, rx, action);
        if (!string.IsNullOrEmpty(errorMessage))
        {
          Logger.ErrorFormat("UnloadPersonInto1C - Ошибка при выгрузке персоны (ИД {0}) - {1}", company.Id, errorMessage);
          errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageUnloadPersonInto1C;
        }
        else
        {
          var asyncRightsHandler = lenspec.Etalon.Module.Parties.AsyncHandlers.AsyncCounterpartyChangeExportStatuslenspec.Create();
          asyncRightsHandler.CounterpartyId = company.Id;
          asyncRightsHandler.ExecuteAsync();
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("UnloadPersonInto1C - Ошибка при выгрузке персоны (ИД {0}) - ", ex, company.Id);
        errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageUnloadPersonInto1C;
      }
      
      return errorMessage;
    }
    
    /// <summary>
    /// Выгрузка контрагента-организации в 1С.
    /// </summary>
    /// <param name="company">Контрагент-организация.</param>
    /// <returns>Сообщение об ошибке.</returns>
    [Public]
    public string UnloadCompanyInto1C(lenspec.Etalon.ICompany company, string connectionString)
    {
      var errorMessage = string.Empty;
      if (company == null)
        return errorMessage;
      
      try
      {
        int action          = 2;
        string directumID   = company.ExternalCodeavis;
        if (string.IsNullOrEmpty(directumID))
          directumID  =  "-";
        string nameAn       = company.Name;
        if (string.IsNullOrEmpty(nameAn))
          nameAn      =  "-";
        string legNameAn    = company.LegalName;
        if (string.IsNullOrEmpty(legNameAn))
          legNameAn   = "-";
        string inn          = company.TIN;
        if (string.IsNullOrEmpty(inn))
          inn         = "-";
        string kpp          = company.TRRC;
        if (string.IsNullOrEmpty(kpp))
          kpp         = "-";
        string ogrn         = company.PSRN;
        if (string.IsNullOrEmpty(ogrn))
          ogrn        = "-";
        string orgGroup     = "-";
        if(company.GroupCounterpartyavis != null)
          orgGroup = company.GroupCounterpartyavis.Name;
        string orgType      = "-";
        if(company.CategoryCounterpartyavis != null)
          orgType = company.CategoryCounterpartyavis.Name;
        int resident        = 0;
        if (company.Nonresident != null)
          if (company.Nonresident.Value == true)
            resident = 1;
        string residentCode    = company.ForeignCompanyCodeavis;
        if(string.IsNullOrEmpty(residentCode))
          residentCode = "-";
        string region       = "-";
        if(company.Region != null)
          region = company.Region.Name;
        string legAddress   = company.LegalAddress;
        if (string.IsNullOrEmpty(legAddress))
          legAddress      = "-";
        string stPostalAddress = company.PostalAddress;
        if (string.IsNullOrEmpty(stPostalAddress))
          stPostalAddress = "-";
        long? headOrg       = 0;
        if (company.HeadCompany != null)
          headOrg       = company.HeadCompany.Id;
        DateTime modtime    = Calendar.Now;
        //          var stOrgCode       = args.businesUnitCode;
        var stOrgCode = String.Empty;
        string stTelephone  = company.Phones;
        if (string.IsNullOrEmpty(stTelephone))
          stTelephone =  "-";
        int phys = 0;
        string fam            = "-";
        string name           = "-";
        string otch           = "-";
        DateTime dateReg      = new DateTime(1900, 1, 1);
        string series         = "-";
        string number         = "-";
        DateTime dateVid      = new DateTime(1900, 1, 1);
        string placeVid       = "-";
        string codePodr       = "-";
        int gender            = 0;
        string snils          =  "-";
        DateTime? dateOfBirth = new DateTime(1900, 1, 1);
        string placeOfBirth   =  "-";
        DateTime? validity    = new DateTime(1900, 1, 1);
        long dirRXID          = company.Id;
        string rx             = "TRUE";
        errorMessage = AvisIntegrationHelper.DataBaseHelper.UnloadingCompanies(connectionString, directumID, nameAn, legNameAn, inn, kpp, ogrn, orgGroup, orgType,
                                                                               resident, residentCode, region, legAddress, stPostalAddress, headOrg, modtime, stOrgCode, stTelephone,
                                                                               phys, fam, name, otch, dateReg, series, number, dateVid, placeVid, codePodr, gender, snils,
                                                                               dateOfBirth, placeOfBirth, validity, dirRXID, rx, action);
        if (!string.IsNullOrEmpty(errorMessage))
        {
          Logger.ErrorFormat("UnloadPersonInto1C - Ошибка при выгрузке организации (ИД {0}) - {1}", company.Id, errorMessage);
          errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageUnloadCompanyInto1C;
        }
        else
        {
          var asyncRightsHandler = lenspec.Etalon.Module.Parties.AsyncHandlers.AsyncCounterpartyChangeExportStatuslenspec.Create();
          asyncRightsHandler.CounterpartyId = company.Id;
          asyncRightsHandler.ExecuteAsync();
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("UnloadPersonInto1C - Ошибка при выгрузке организации (ИД {0}) - ", ex, company.Id);
        errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageUnloadCompanyInto1C;
      }
      
      return errorMessage;
    }
    
    /// <summary>
    /// Выгрузка контрагента-банка в 1С.
    /// </summary>
    /// <param name="company">Контрагент-банк.</param>
    /// <returns>Сообщение об ошибке.</returns>
    [Public]
    public string UnloadBankInto1C(lenspec.Etalon.IBank company, string connectionString)
    {
      var errorMessage = string.Empty;
      if (company == null)
        return errorMessage;
      
      try
      {
        int action          = 2;
        string directumID   = "-";
        string nameAn       = company.Name;
        if (string.IsNullOrEmpty(nameAn))
          nameAn      =  "-";
        string legNameAn    = company.LegalName;
        if (string.IsNullOrEmpty(legNameAn))
          legNameAn   = "-";
        string inn          = company.TIN;
        if (string.IsNullOrEmpty(inn))
          inn         = "-";
        string kpp          = company.TRRC;
        if (string.IsNullOrEmpty(kpp))
          kpp         = "-";
        int resident        = 0;
        if (company.Nonresident != null)
          if (company.Nonresident.Value == true)
            resident  = 1;
        string region = "-";
        if(company.Region != null)
          region = company.Region.Name;
        string legAddress   = company.LegalAddress;
        if (string.IsNullOrEmpty(legAddress))
          legAddress      = "-";
        string stPostalAddress = company.PostalAddress;
        if (string.IsNullOrEmpty(stPostalAddress))
          stPostalAddress = "-";
        long? headOrg       = 0;
        if (company.HeadBankavis != null)
          headOrg = company.HeadBankavis.Id;
        DateTime modtime      = Calendar.Now;
        var stOrgCode = String.Empty;
        int phys = 0;
        string fam            = "-";
        string name           = "-";
        string otch           = "-";
        DateTime dateReg      =  new DateTime(1900, 1, 1);
        string series         = "-";
        string number         = "-";
        DateTime dateVid      = new DateTime(1900, 1, 1);
        string placeVid       = "-";
        string codePodr       = "-";
        long dirRXID          = company.Id;
        string stTelephone    = company.Phones;
        if (string.IsNullOrEmpty(stTelephone))
          stTelephone =  "-";
        int gender            = 0;
        string snils          = "-";
        DateTime? dateOfBirth = new DateTime(1900, 1, 1);
        string ogrn           = "-";
        string orgGroup       = "-";
        string orgType        = "-";
        string residentCode   = "-";
        string placeOfBirth   = "-";
        DateTime? validity    = new DateTime(1900, 1, 1);
        string rx             = "TRUE";
        errorMessage = AvisIntegrationHelper.DataBaseHelper.UnloadingCompanies(connectionString, directumID, nameAn, legNameAn, inn, kpp, ogrn, orgGroup, orgType,
                                                                               resident, residentCode, region, legAddress, stPostalAddress, headOrg, modtime, stOrgCode, stTelephone,
                                                                               phys, fam, name, otch, dateReg, series, number, dateVid, placeVid, codePodr, gender, snils,
                                                                               dateOfBirth, placeOfBirth, validity, dirRXID, rx, action);
        if (!string.IsNullOrEmpty(errorMessage))
        {
          Logger.ErrorFormat("UnloadPersonInto1C - Ошибка при выгрузке банка (ИД {0}) - {1}", company.Id, errorMessage);
          errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageUnloadBankInto1C;
        }
        else
        {
          var asyncRightsHandler = lenspec.Etalon.Module.Parties.AsyncHandlers.AsyncCounterpartyChangeExportStatuslenspec.Create();
          asyncRightsHandler.CounterpartyId = company.Id;
          asyncRightsHandler.ExecuteAsync();
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("UnloadPersonInto1C - Ошибка при выгрузке банка (ИД {0}) - ", ex, company.Id);
        errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageUnloadBankInto1C;
      }
      
      return errorMessage;
    }
    
    #endregion
    
    #region Заполнить компанию из KonturFocus
    
    /// <summary>
    /// Заполнить организацию из КонтурФокус
    /// </summary>
    /// <param name="company">Компания</param>
    /// <returns>Сообщение с ошибкой, если процесс неуспешен</returns>
    [Remote]
    public string FillCompanyFromKontur(lenspec.Etalon.ICompanyBase company)
    {
      var errorMessage = string.Empty;
      try
      {
        var kontur = GetRestApiConnectionKontur();
        if (Equals("9909", company.TIN.Substring(0, 4)))
          errorMessage = TryFillForeignCompanyFromKontur(company, kontur);
        else
          errorMessage = TryFillRFCompanyFromKontur(company, kontur);
      }
      catch (AppliedCodeException ex)
      {
        errorMessage = ex.Message;
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Avis - TryFillRFCompanyFromKontur - Ошибка обработки организации с ИНН {0}: {1}. {2}", company.TIN, ex.Message, innerMessage);
        errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageFailFillFromKontur;
      }
      return errorMessage;
    }
    
    /// <summary>
    /// Получить АПИ для запросов в КонтурФокус
    /// </summary>
    /// <returns>АПИ</returns>
    private KonturFocusIntegration.Rest GetRestApiConnectionKontur()
    {
      KonturFocusIntegration.Rest kontur = null;
      var konturSetting = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.Constants.Module.KonturFocusEGRULRecordCode).FirstOrDefault();
      if(konturSetting == null)
        throw AppliedCodeException.Create(lenspec.Etalon.Module.Parties.Resources.ErrorMessageFailConnectiongToKonturFocus);
      
      var konturApiKey = Encryption.Decrypt(konturSetting.ConnectionParams);
      kontur = new KonturFocusIntegration.Rest(konturApiKey);
      
      return kontur;
    }
    
    /// <summary>
    /// Получить модель данных организации РФ из КонтурФокус
    /// </summary>
    /// <param name="tin">ИНН</param>
    /// <param name="kontur">АПИ для запросов в КонтурФокус</param>
    /// <returns>Модель</returns>
    private KonturFocusIntegration.Models.CompanyInfo.CompanyInfoModel GetCompanyInfoModel(string tin, KonturFocusIntegration.Rest kontur)
    {
      var konturCompany = kontur.GetCompanyInfo(tin, false) as KonturFocusIntegration.Models.CompanyInfo.CompanyInfoModel;
      if (konturCompany == null)
        throw AppliedCodeException.Create(lenspec.Etalon.Module.Parties.Resources.ErrorMessageNotFoundCompanyFormat(tin));
      
      return konturCompany;
    }
    
    /// <summary>
    /// Заполнить организацию РФ
    /// </summary>
    /// <param name="company">Компания</param>
    /// <param name="kontur">API из dll для функционала контура</param>
    /// <returns>Сообщение с ошибкой, если процесс неуспешен</returns>
    private string TryFillRFCompanyFromKontur(lenspec.Etalon.ICompanyBase company, KonturFocusIntegration.Rest kontur)
    {
      var errorMessage = string.Empty;
      try
      {
        var konturCompany = GetCompanyInfoModel(company.TIN, kontur);
        if (konturCompany.IP != null)
        {
          FillIndividualEntrepreneur(konturCompany, company);
          FillAdditionlDataForIndividualEntrepreneur(kontur, company);
        }
        if (konturCompany.UL != null)
        {
          if (company.TRRC == konturCompany.UL.Kpp)
          {
            FillLegalEntityLikeHeadCompany(konturCompany, company);
            FillAdditionlDataForLegalEntity(kontur, company);
          }
          else if (konturCompany.UL.Branches.Any(x => x.Kpp == company.TRRC))
          {
            FillLegalEntityLikeBranch(konturCompany, company, company.TRRC);
            FillAdditionlDataForLegalEntity(kontur, company);
          }
          else
            throw AppliedCodeException.Create(lenspec.Etalon.Banks.Resources.NoCompanyTINandTRRCErrorMessage);
        }
      }
      catch (AppliedCodeException ex)
      {
        errorMessage = ex.Message;
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Avis - TryFillRFCompanyFromKontur - Ошибка обработки организации с ИНН {0}: {1}. {2}", company.TIN, ex.Message, innerMessage);
        errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageFailFillFromKontur;
      }
      return errorMessage;
    }
    
    /// <summary>
    /// Заполнить иностранную организацию
    /// </summary>
    /// <param name="company">Организация</param>
    /// <returns>Сообщение с ошибкой, если процесс неуспешен</returns>
    private string TryFillForeignCompanyFromKontur(lenspec.Etalon.ICompanyBase companyBase, KonturFocusIntegration.Rest kontur)
    {
      var errorMessage = string.Empty;
      try
      {
        var companyInfo = kontur.GetCompanyInfo(companyBase.TIN, false) as KonturFocusIntegration.Models.CompanyInfo.ForeignCompanyInfoModel;
        if (companyInfo == null)
          throw AppliedCodeException.Create(lenspec.Etalon.Module.Parties.Resources.ErrorMessageNotFoundCompanyFormat(companyBase.TIN));
        var company = lenspec.Etalon.Companies.As(companyBase);
        company.Name = companyInfo.ShortName;
        company.LegalName = companyInfo.FullName;
        company.TRRC = companyInfo.Kpp;
        company.Nonresident = true;
        company.ForeignCompanyCodeavis = companyInfo.Nza;
        if (companyInfo.AddressFias != null)
          FillAddress(companyInfo.AddressFias, company);
        company.PostalAddress = company.LegalAddress;
      }
      catch (AppliedCodeException ex)
      {
        errorMessage = ex.Message;
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Avis - TryFillForeignCompanyFromKontur - Ошибка обработки организации с ИНН {0}: {1}. {2}", companyBase.TIN, ex.Message, innerMessage);
        errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageFailFillFromKontur;
      }
      return errorMessage;
    }
    
    /// <summary>
    /// Заполнить ИП
    /// </summary>
    /// <param name="konturCompany">Модель данных компании из контура</param>
    /// <param name="company">Компания</param>
    private void FillIndividualEntrepreneur(KonturFocusIntegration.Models.CompanyInfo.CompanyInfoModel konturCompany, lenspec.Etalon.ICompanyBase company)
    {
      company.HeadCompany = null;
      company.LegalAddress = null;
      company.PostalAddress = null;
      company.Name = $"ИП {konturCompany.IP.Fio}";
      company.LegalName = company.Name;
      company.NCEO = konturCompany.IP.Okpo;
      company.TIN = konturCompany.Inn;
      company.PSRN = konturCompany.Ogrn;
    }
    
    /// <summary>
    /// Заполнить дополнительные данные ИП из отдельных ресурсов КонтурФокус
    /// </summary>
    /// <param name="kontur">API из dll для функционала контура</param>
    /// <param name="company">Компания</param>
    private void FillAdditionlDataForIndividualEntrepreneur(KonturFocusIntegration.Rest kontur, lenspec.Etalon.ICompanyBase company)
    {
      var ergDetailsModel = kontur.GetCompanyEgrDetail(company.TIN);
      FillNCEA(ergDetailsModel, company);
    }
    
    /// <summary>
    /// Заполнить организацию как головную
    /// </summary>
    /// <param name="konturCompany">Модель данных компании из контура</param>
    /// <param name="company">Компания</param>
    public void FillLegalEntityLikeHeadCompany(KonturFocusIntegration.Models.CompanyInfo.CompanyInfoModel konturCompany, lenspec.Etalon.ICompanyBase company)
    {
      company.Name = !string.IsNullOrEmpty(konturCompany.UL.LegalName.Short) ? konturCompany.UL.LegalName.Short : konturCompany.UL.LegalName.Full;
      company.LegalName = konturCompany.UL.LegalName.Full;
      company.NCEO = konturCompany.UL.Okpo;
      company.PSRN = konturCompany.Ogrn;
      company.TRRC = konturCompany.UL.Kpp;

      var head = konturCompany.UL.Heads.FirstOrDefault();
      if (head != null)
      {
        company.FIOContactavis = head.Fio;
        company.JobTitleContactavis = head.Position;
      }
      
      if (konturCompany.UL.LegalAddress?.ParsedAddressRF != null)
        FillAddress(konturCompany.UL.LegalAddress.ParsedAddressRF, company);
      else if (konturCompany.UL.LegalAddress?.ParsedAddressRFFias != null)
        FillAddress(konturCompany.UL.LegalAddress.ParsedAddressRFFias, company);
    }
    
    /// <summary>
    /// Заполнить дополнительные данные компании из отдельных ресурсов КонтурФокус
    /// </summary>
    /// <param name="kontur">API из dll для функционала контура</param>
    /// <param name="company">Компания</param>
    private void FillAdditionlDataForLegalEntity(KonturFocusIntegration.Rest kontur, lenspec.Etalon.ICompanyBase company)
    {
      var siteModel = kontur.GetCompanySite(company.TIN);
      company.Homepage = CheckValueAndAdd(siteModel?.Sites, company.Homepage, 15);
      
      var phoneModel = kontur.GetCompanyPhone(company.TIN);
      company.Phones = CheckValueAndAdd(phoneModel?.ContactPhones?.Phones, company.Phones, 20);
      
      var ergDetailsModel = kontur.GetCompanyEgrDetail(company.TIN);
      FillNCEA(ergDetailsModel, company);
    }
    
    /// <summary>
    /// Заполнить ОКВЭД в организации
    /// </summary>
    /// <param name="egrDetailModel">Модель расширенных данных из КонтурФокус</param>
    /// <param name="company">Компания</param>
    private void FillNCEA(KonturFocusIntegration.Models.CompanyEgrDetail.CompanyEgrDetailModel egrDetailModel, lenspec.Etalon.ICompanyBase company)
    {
      if (egrDetailModel == null)
        return;
      
      var ncea = string.Empty;
      if (egrDetailModel.IP != null && egrDetailModel.IP.Activities != null)
      {
        ncea = egrDetailModel.IP.Activities.PrincipalActivity?.Code;
        foreach (var activity in egrDetailModel.IP.Activities.ComplementaryActivities)
        {
          if (activity != null && !string.IsNullOrEmpty(activity.Code))
            ncea = FormConcatination(ncea, activity.Code, ";");
        }
      }
      else if (egrDetailModel.UL != null && egrDetailModel.UL.Activities != null)
      {
        ncea = egrDetailModel.UL.Activities.PrincipalActivity?.Code;
        if (egrDetailModel.UL.Activities.ComplementaryActivities != null)
        {
          foreach (var activity in egrDetailModel.UL.Activities.ComplementaryActivities)
          {
            if (activity != null && !string.IsNullOrEmpty(activity.Code))
              ncea = FormConcatination(ncea, activity.Code, ";");
          }
        }
      }
      company.NCEA = ncea;
    }
    
    /// <summary>
    /// Заполнить организацию как филиал
    /// </summary>
    /// <param name="konturCompany">Модель данных компании из контура</param>
    /// <param name="company">Компания</param>
    /// <param name="trrc">КПП филиала</param>
    public void FillLegalEntityLikeBranch(KonturFocusIntegration.Models.CompanyInfo.CompanyInfoModel konturCompany, lenspec.Etalon.ICompanyBase company, string trrc)
    {
      var companyModel = konturCompany.UL.Branches.FirstOrDefault(x => x.Kpp == trrc);

      if (!string.IsNullOrEmpty(companyModel.Name))
        company.Name = companyModel.Name;
      else
      {
        var mainCompanyName = !string.IsNullOrEmpty(konturCompany.UL.LegalName.Short) ? konturCompany.UL.LegalName.Short : konturCompany.UL.LegalName.Full;
        company.Name = string.Format("Филиал {0} (КПП: {1})", mainCompanyName, companyModel.Kpp);
      }
      company.LegalName = company.Name;
      company.TRRC = companyModel.Kpp;
      FillAddress(companyModel.ParsedAddressRF, company);
      company.PostalAddress = company.LegalAddress;
      
      if (lenspec.Etalon.Banks.Is(company))
        company.HeadBankavis = lenspec.Etalon.Banks.GetAll(x => x.TIN == konturCompany.Inn && x.TRRC == konturCompany.UL.Kpp).FirstOrDefault();
      else
        company.HeadCompany = lenspec.Etalon.Companies.GetAll(x => x.TIN == konturCompany.Inn && x.TRRC == konturCompany.UL.Kpp).FirstOrDefault();
    }
    
    /// <summary>
    /// Заполнить юридический адрес в компании (для стандартных адресов)
    /// </summary>
    /// <param name="addressModel">Модель адреса</param>
    /// <param name="company">Компания</param>
    private void FillAddress(KonturFocusIntegration.Models.CompanyInfo.ParsedAddressRFModel addressModel, lenspec.Etalon.ICompanyBase company)
    {
      var address = addressModel.ZipCode;
      if (addressModel.RegionName != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.RegionName.TopoShortName, addressModel.RegionName.TopoValue), ",");
      if (addressModel.City != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.City.TopoShortName, addressModel.City.TopoValue), ",");
      if (addressModel.Settlement != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.Settlement.TopoShortName, addressModel.Settlement.TopoValue), ",");
      if (addressModel.District != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.District.TopoShortName, addressModel.District.TopoValue), ",");
      if (addressModel.Street != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.Street.TopoShortName, addressModel.Street.TopoValue), ",");
      if (addressModel.House != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.House.TopoShortName, addressModel.House.TopoValue), ",");
      if (!string.IsNullOrEmpty(addressModel.BulkRaw))
        address = FormConcatination(address, addressModel.BulkRaw, ",");
      if (addressModel.Flat != null)
      {
        if (!string.IsNullOrEmpty(addressModel.Flat.TopoShortName))
          address = FormConcatination(address, string.Format("{0} {1}", addressModel.Flat.TopoShortName, addressModel.Flat.TopoValue), ",");
        else
          address = FormConcatination(address, addressModel.Flat.TopoValue, ",");
      }
      company.LegalAddress = address;
    }
    
    /// <summary>
    /// Заполнить юридический адрес в компании (для адреса новых территорий)
    /// </summary>
    /// <param name="addressModel">Модель адреса</param>
    /// <param name="company">Компания</param>
    private void FillAddress(KonturFocusIntegration.Models.CompanyInfo.ParsedAddressRFFiasModel addressModel, lenspec.Etalon.ICompanyBase company)
    {
      var address = addressModel.ZipCode;
      if (addressModel.RegionName != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.RegionName.TopoShortName.Trim('.'), addressModel.RegionName.TopoValue), ",");
      if (addressModel.City != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.City.TopoShortName.Trim('.'), addressModel.City.TopoValue), ",");
      if (addressModel.MunicipalDistrict != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.MunicipalDistrict.TopoShortName.Trim('.'), addressModel.MunicipalDistrict.TopoValue), ",");
      if (addressModel.Settlement != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.Settlement.TopoShortName.Trim('.'), addressModel.Settlement.TopoValue), ",");
      if (addressModel.Street != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.Street.TopoShortName.Trim('.'), addressModel.Street.TopoValue), ",");
      if (addressModel.House != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.House.TopoShortName.Trim('.'), addressModel.House.TopoValue), ",");
      if (!string.IsNullOrEmpty(addressModel.BulkRaw))
        address = FormConcatination(address, addressModel.BulkRaw, ",");
      else if (addressModel.Buildings.FirstOrDefault() != null)
        address = FormConcatination(address, string.Format("{0} {1}", addressModel.Buildings.First().TopoFullName, addressModel.Buildings.First().TopoValue), ",");
      if (addressModel.Flat != null)
      {
        if (!string.IsNullOrEmpty(addressModel.Flat.TopoShortName))
          address = FormConcatination(address, string.Format("{0} {1}", addressModel.Flat.TopoShortName.Trim('.'), addressModel.Flat.TopoValue), ",");
        else
          address = FormConcatination(address, addressModel.Flat.TopoValue, ",");
      }
      company.LegalAddress = address;
    }
    
    /// <summary>
    /// Проверяет имеются ли данные значения в строке и добавляет не хватающих.
    /// </summary>
    /// <param name="newValues">Список значений для добавления.</param>
    /// <param name="oldValue">Строка в которую будут добавляться значения через запятую.</param>
    /// <param name="oldValue">Максимальное количество сайтов/телефонов для вывода.</param>
    /// <returns>Отформатированная строка</returns>
    private string CheckValueAndAdd(List<string> newValues, string oldValue, int maxCount)
    {
      if (newValues == null)
        return oldValue;
      
      var stringBuilder = new System.Text.StringBuilder(oldValue, 250);
      
      // Преобразовываем в список значений котоыре уже записаны.
      var oldValues = new List<string>();
      if (!string.IsNullOrEmpty(oldValue))
        oldValues = oldValue.Replace(" ", "").Split(',').ToList();
      
      // Если старых значений больше чем максимальное количество, то оставляем только их.
      if (oldValues.Count >= maxCount)
        return oldValue;
      
      foreach (var newValue in newValues.Take(maxCount))
      {
        // Если значение имеется в списке, то пропускаем.
        if (oldValues != null && oldValues.Contains(newValue))
          continue;
        
        // Добавляем значение если оно не обнаружено в строке.
        if (stringBuilder.Length > 0)
          stringBuilder.Append($", {newValue}");
        else
          stringBuilder.Append($"{newValue}");
      }
      var result = stringBuilder.ToString();
      
      if (result.Length <= 250)
        return result;
      
      // Обрезаем длину телефона до 250 символов.
      return result.Substring(0,249);
    }
    
    #region Функции для диалогов
    
    // FIXME т.к. нельзя использовать out ref и внешние структуры в remote функциях, приходится 2 раза делать один и тот же запрос (для предпроверок отображаемых пользователю и для заполнения филиала)
    
    /// <summary>
    /// Проверить возможность формирования списка наименований филиалов для отображения в диалоге
    /// </summary>
    /// <param name="tin">ИНН</param>
    /// <returns>Сообщение с ошибкой (если есть)</returns>
    [Remote(IsPure = true)]
    public string CheckOpportunitySelectBranch(string tin)
    {
      var errorMessage = string.Empty;
      try
      {
        var kontur = GetRestApiConnectionKontur();
        var companyModel = GetCompanyInfoModel(tin, kontur);
        var isExistenceHeadCompany = Sungero.Parties.CompanyBases.GetAll().Any(x => !string.IsNullOrEmpty(tin) && !string.IsNullOrEmpty(companyModel.UL.Kpp) &&
                                                                               Equals(tin, x.TIN) && Equals(companyModel.UL.Kpp, x.TRRC));
        if (!isExistenceHeadCompany)
          throw AppliedCodeException.Create("Перед созданием филиала необходимо создать головную организацию.");
        
        if (companyModel.UL.Branches == null)
          throw AppliedCodeException.Create("Филиалы с таким ИНН не найдены в KonturFocus.");
      }
      catch (AppliedCodeException ex)
      {
        errorMessage = ex.Message;
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Avis - TryFillRFCompanyFromKontur - Ошибка обработки организации с ИНН {0}: {1}. {2}", tin, ex.Message, innerMessage);
        errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageFailFillFromKontur;
      }
      return errorMessage;
    }
    
    /// <summary>
    /// Сформировать список наименований филиалов + КПП (для диалога выбора филиала)
    /// </summary>
    /// <param name="tin">ИНН</param>
    /// <param name="errorMessage">Строка для записи ошибки (если ошибка есть)</param>
    /// <returns>Список наименований филиалов</returns>
    [Remote(IsPure = true)]
    public List<string> TryFormBranchNames(string tin)
    {
      var branchNames = new List<string>();
      try
      {
        var kontur = GetRestApiConnectionKontur();
        var companyModel = GetCompanyInfoModel(tin, kontur);
        
        foreach (var branch in companyModel.UL.Branches)
        {
          if (branch != null)
          {
            var headCompanyName = companyModel.UL.LegalName != null && !string.IsNullOrEmpty(companyModel.UL.LegalName.Short) ? companyModel.UL.LegalName.Short :
              companyModel.UL.LegalName != null && !string.IsNullOrEmpty(companyModel.UL.LegalName.Full) ? companyModel.UL.LegalName.Full : string.Format("ИНН: {0}", companyModel.Inn);
            var branchName = !string.IsNullOrEmpty(branch.Name) ? string.Format("{0} (КПП: {1})", branch.Name, branch.Kpp) : string.Format("Филиал: {0} (КПП: {1})", headCompanyName, branch.Kpp);
            branchNames.Add(branchName);
          }
        }
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Avis - TryFillRFCompanyFromKontur - Ошибка обработки организации с ИНН {0}: {1}. {2}", tin, ex.Message, innerMessage);
      }
      return branchNames;
    }
    
    /// <summary>
    /// Заполнить организацию как головную из диалога
    /// </summary>
    /// <param name="company">Компания</param>
    /// <returns>Сообщение об ошибке если есть</returns>
    [Remote]
    public string TryFillLegalEntityLikeHeadCompanyFromDialog(lenspec.Etalon.ICompanyBase company)
    {
      var errorMessage = string.Empty;
      try
      {
        var kontur = GetRestApiConnectionKontur();
        var companyModel = GetCompanyInfoModel(company.TIN, kontur);
        FillLegalEntityLikeHeadCompany(companyModel, company);
        FillAdditionlDataForLegalEntity(kontur, company);
      }
      catch (AppliedCodeException ex)
      {
        errorMessage = ex.Message;
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Avis - TryFillRFCompanyFromKontur - Ошибка обработки организации с ИНН {0}: {1}. {2}", company.TIN, ex.Message, innerMessage);
        errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageFailFillFromKontur;
      }
      return errorMessage;
    }
    
    /// <summary>
    /// Заполнить организацию как филиал по выбранному филиалу из диалога
    /// </summary>
    /// <param name="company">Компания</param>
    /// <param name="selectedBranchName">Выбранный филиал</param>
    /// <returns>Сообщение об ошибке если есть</returns>
    [Remote]
    public string TryFillLegalEntityLikeBranchFromDialog(lenspec.Etalon.ICompanyBase company, string selectedBranchName)
    {
      var errorMessage = string.Empty;
      try
      {
        var kontur = GetRestApiConnectionKontur();
        var companyModel = GetCompanyInfoModel(company.TIN, kontur);
        var expression = new System.Text.RegularExpressions.Regex(@lenspec.Etalon.Module.Parties.Resources.RegexForGettingTRRC);
        var matches = expression.Matches(selectedBranchName)[0].Groups[1];
        var trrc = matches.Value;
        FillLegalEntityLikeBranch(companyModel, company, trrc);
        FillAdditionlDataForLegalEntity(kontur, company);
      }
      catch (AppliedCodeException ex)
      {
        errorMessage = ex.Message;
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Avis - TryFillRFCompanyFromKontur - Ошибка обработки организации с ИНН {0}: {1}. {2}", company.TIN, ex.Message, innerMessage);
        errorMessage = lenspec.Etalon.Module.Parties.Resources.ErrorMessageFailFillFromKontur;
      }
      return errorMessage;
    }
    
    #endregion
    
    #endregion
    
    /// <summary>
    /// Получить индивидуальнцую ссылку на эксперсс-отчет из КФ.
    /// </summary>
    [Public, Remote(IsPure = true)]
    public string GetCompanyReportFromKontur(string inn)
    {
      var konturSetting = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.Constants.Module.KonturFocusEGRULRecordCode).FirstOrDefault();
      if(konturSetting == null)
        throw AppliedCodeException.Create("Не найден ключ Контур.Фокуса.");

      var konturApiKey = Encryption.Decrypt(konturSetting.ConnectionParams);
      var kontur = new KonturFocusIntegration.Rest(konturApiKey);
      return kontur.GetExpressReportFromKF(inn);
    }
    
    /// <summary>
    /// Объединить строки используя разделитель
    /// </summary>
    /// <param name="mainString">Основная строка</param>
    /// <param name="addedString">Добавляемая подстрока</param>
    /// <param name="separator">Разделитель</param>
    /// <returns></returns>
    /// <remarks>Паттерн: Основная строка + разделитель + пробел + добавляемая подстрока</remarks>
    [Public]
    public string FormConcatination(string mainString, string addedString, string separator)
    {
      if (string.IsNullOrEmpty(mainString))
        return addedString;

      return string.Format("{0}{1} {2}", mainString, separator, addedString);
    }
    
    // <summary>
    /// Вызываем асинхронный обработчик Выгрузка Контрагента в 1С
    /// </summary>
    /// <param name="counterPartiesid">ИД выгружаемых контрагентов</param>
    /// <param name="businesUnitCode">Код НоР в рамках которого выгружается контрагент</param>
    [Public]
    public static void UnloadingKA(List<long> counterPartiesid, string businesUnitCode)
    {
      foreach (var counterPartyId in counterPartiesid)
      {
        // Создать асинхронный обработчик DisablingAccounts.
        var asyncRightsHandler = lenspec.Etalon.Module.Parties.AsyncHandlers.UnloadingContractorslenspec.Create();
        // Заполнить параметры асинхронного обработчика.
        asyncRightsHandler.counterPartiesid = counterPartyId;
        asyncRightsHandler.businesUnitCode  = businesUnitCode;
        // Вызвать асинхронный обработчик.
        asyncRightsHandler.ExecuteAsync();
      }
    }
    
    // <summary>
    /// Вызываем асинхронный обработчик Выгрузка Контрагента в 1С
    /// </summary>
    /// <param name="counterPartiesid">ИД выгружаемого контрагента</param>
    /// <param name="businesUnitCode">Код НоР в рамках которого выгружается контрагент</param>
    [Public]
    public static void UnloadingKAFromReference(long counterPartiesid, string businesUnitCode)
    {
      // Создать асинхронный обработчик DisablingAccounts.
      var asyncRightsHandler = lenspec.Etalon.Module.Parties.AsyncHandlers.UnloadingContractorslenspec.Create();
      // Заполнить параметры асинхронного обработчика.
      asyncRightsHandler.counterPartiesid = counterPartiesid;
      asyncRightsHandler.businesUnitCode  = businesUnitCode;
      // Вызвать асинхронный обработчик.
      asyncRightsHandler.ExecuteAsync();
    }
  }
}