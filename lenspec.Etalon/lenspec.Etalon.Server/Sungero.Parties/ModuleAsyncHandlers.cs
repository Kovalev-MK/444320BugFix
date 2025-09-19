using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Parties.Server
{
  partial class ModuleAsyncHandlers
  {
    /// <summary>
    /// Изменение статуса Экспорт в 1С.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncCounterpartyChangeExportStatuslenspec(lenspec.Etalon.Module.Parties.Server.AsyncHandlerInvokeArgs.AsyncCounterpartyChangeExportStatuslenspecInvokeArgs args)
    {
      var isForcedLocked = false;
      Logger.DebugFormat("AsyncCounterpartyChangeExportStatuslenspec - обновление статуса синхронизации ИД = {0}.", args.CounterpartyId);
      
      // Получаем контрагента.
      var ka = lenspec.Etalon.Counterparties.GetAll().Where(x => x.Id == args.CounterpartyId).FirstOrDefault();
      try
      {
        var lockinfo = Locks.GetLockInfo(ka);
        if (lockinfo != null && lockinfo.IsLocked)
        {
          args.Retry = true;
          Logger.ErrorFormat("AsyncCounterpartyChangeExportStatuslenspec - контрагент с ИД = {0} - заблокирован пользователем {1}.", ka.Id, lockinfo.OwnerName);
          return;
        }
        else
        {
          isForcedLocked = Locks.TryLock(ka);
        }
        
        if (!isForcedLocked)
        {
          throw new Exception($"не удалось заблокировать карточку контрагента в Directum RX.");
        }
        
        // Проставляем статус "Выполнено" и дату выполнения, сохраняем контрагента.
        ka.Export1CStatelenspec = lenspec.Etalon.Counterparty.Export1CStatelenspec.Yes;
        ka.Export1CDatelenspec = Calendar.Now;
        ka.Save();
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("AsyncCounterpartyChangeExportStatuslenspec - не удалось сохранить изменения по контрагент с ИД = {0} - {1}.", ka.Id, ex.Message);
        args.Retry = false;
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(ka);
      }
    }

    public virtual void UpdateCompanyRegisterStatuslenspec(lenspec.Etalon.Module.Parties.Server.AsyncHandlerInvokeArgs.UpdateCompanyRegisterStatuslenspecInvokeArgs args)
    {
      var prefix = "avis - UpdateCompanyRegisterStatuslenspec -";
      Logger.DebugFormat("{0} Updating counterparty register status for company id={1}, set provider={2}, contractor={3}.", prefix, args.CompanyId, args.IsProvider, args.IsContractor);
      var company = lenspec.Etalon.Companies.Get(args.CompanyId);
      var isForceLocked = false;
      
      try
      {
        // Проверка аргументов.
        if (company == null)
        {
          args.Retry = false;
          throw new Exception($"Company not found.");
        }
        
        // Проверка необходимости внесения изменений.
        if (company.IsProvideravis == args.IsProvider && company.IsContractoravis == args.IsContractor)
        {
          args.Retry = false;
          Logger.DebugFormat("{0} Status is up to date.", prefix);
          return;
        }
        
        // Проверка наличия блокировок.
        var lockInfo = Locks.GetLockInfo(company);
        if (lockInfo == null || lockInfo.IsLocked)
        {
          args.Retry = true;
          throw new Exception($"Company is locked by user {lockInfo.OwnerName}.");
        }
        else
          isForceLocked = Locks.TryLock(company);
        
        // Проверка возможности блокировки.
        if (!isForceLocked)
          throw new Exception($"Unable to set lock on company.");
        
        // Изменения.
        company.IsContractoravis = args.IsContractor;
        company.IsProvideravis = args.IsProvider;
        
        // Снять обязательность полей перед сохранением.
        foreach (var property in company.State.Properties)
          property.IsRequired = false;
        
        company.Save();
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("{0} Error processing company id={1}: {2}", prefix, args.CompanyId, ex.Message);
      }
      finally
      {
        if (isForceLocked)
          Locks.Unlock(company);
      }
    }

    public virtual void UnloadingContractorslenspec(lenspec.Etalon.Module.Parties.Server.AsyncHandlerInvokeArgs.UnloadingContractorslenspecInvokeArgs args)
    {
      Logger.Debug($"Avis - UnloadingContractorslenspec. Выгрузка КА с ИД {args.counterPartiesid} в 1С. Старт.");
      var ka = lenspec.Etalon.Counterparties.GetAll().Where(x => x.Id == args.counterPartiesid).FirstOrDefault();
      
      //Проверим настройки подключения
      var settings = Integrationses.GetAll(s => s.Code == avis.Integration.Constants.Module.OrgSructureImportRecordCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      // инициализируем helper для получения данных из интеграционной базы
      string connectionString = Encryption.Decrypt(settings.ConnectionParams);
      
      // Проверим к какому типу относится контрагент и от этого уже будет выгружать
      var errorMessage = string.Empty;
      #region Организации
      if (lenspec.Etalon.Companies.Is(ka) == true)
      {
        try
        {
          var company = lenspec.Etalon.Companies.GetAll().Where(x => x.Id == ka.Id).FirstOrDefault();
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
        }
        catch(Exception ex)
        {
          Logger.Error("Ошибка при выгрузке организаций", ex);
        }
      }
      #endregion
      
      #region Банки
      if (lenspec.Etalon.Banks.Is(ka) == true)
      {
        try
        {
          int action          = 2;
          var company = lenspec.Etalon.Banks.GetAll().Where(x => x.Id == ka.Id).FirstOrDefault();
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
        }
        catch(Exception ex)
        {
          Logger.Error("Ошибка при выгрузке банков", ex);
        }
      }
      #endregion
      
      #region Персоны
      if (lenspec.Etalon.People.Is(ka) == true)
      {
        try
        {
          int action          = 2;
          var company = lenspec.Etalon.People.GetAll().Where(x => x.Id == ka.Id).FirstOrDefault();
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
        }
        catch(Exception ex)
        {
          Logger.Error("Ошибка при выгрузке персон", ex);
        }
      }
      #endregion
      
      if (!string.IsNullOrEmpty(errorMessage))
            Logger.Error($"Avis - UnloadingContractorslenspec. Выгрузка КА с ИД {args.counterPartiesid} в 1С. Ошбика выполнения в dll - {errorMessage}.");
      
      // Если нету ошибок при выгрузке, установка статуса экспорта в 1с "Выполнено" и дату выполнения.
      if (string.IsNullOrEmpty(errorMessage))
        lenspec.Etalon.PublicFunctions.Counterparty.UpdateCounterpartyExport1c(ka);
      
      Logger.Debug($"Avis - UnloadingContractorslenspec. Выгрузка КА с ИД {args.counterPartiesid} в 1С. Завершение.");
    }
  }
}