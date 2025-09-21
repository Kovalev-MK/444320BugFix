using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonParties
{
  partial class DebReportServerHandlers
  {
    /// <summary>
    /// После сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      // Удаление данных из временной таблицы, после обработки отчёта.
      // При поиске ошибки с заполнение табличных частей, заккомунтируйте и смотрите в sql manager.
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebActivities, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebFounders, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebHistoryFounders, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebFilials, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebLicenses, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebFinances, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebTaxes, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebFns, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebHistoryUL, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebBanks, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebDirectors, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebArbitrations, DebReport.SessionId);
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DebReport.TableNameDebContracts, DebReport.SessionId);
    }
    
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      DebReport.SessionId = Guid.NewGuid().ToString();
      
      var konturSetting = lenspec.Etalon.Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.KonturFocusEGRULRecordCode).FirstOrDefault();
      if(konturSetting == null)
        throw AppliedCodeException.Create("Не найдены настройки подключения.");

      var konturApiKey = Encryption.Decrypt(konturSetting.ConnectionParams);
      
      var kontur = new KonturFocusIntegration.Rest(konturApiKey);
      var konturCompany = kontur.GetCompanyInfo(DebReport.Inn);
      if (konturCompany == null)
        return;
      
      DebReport.Date = Calendar.Today.ToString("d");
      DebReport.Ogrn = konturCompany?.Ogrn;
      if (konturCompany?.UL != null)
      {
        DebReport.Legalname = konturCompany?.UL?.LegalName?.Full;
        DebReport.Kpp = konturCompany?.UL?.Kpp;
        DebReport.ShortName = konturCompany?.UL?.LegalName?.Short;
        DebReport.Status = konturCompany?.UL?.Status?.StatusString;
        DebReport.DateFormation = konturCompany?.UL?.RegistrationDate;
      }
      if (konturCompany?.IP != null)
      {
        DebReport.Legalname = konturCompany?.IP?.Fio;
        DebReport.ShortName = konturCompany?.IP?.Fio;
        DebReport.Status = konturCompany?.IP?.Status?.StatusString;
        DebReport.DateFormation = konturCompany?.IP?.RegistrationDate;
        DebReport.Director = $"{konturCompany?.IP?.Fio}, ИНН {DebReport.Inn}";
        DebReport.GDName = $"{konturCompany?.IP?.Fio}, ИНН {DebReport.Inn}";
      }
      
      var egrDetails = kontur.GetCompanyEgrDetail(DebReport.Inn);
      if (egrDetails?.UL?.StatedCapital != null)
        DebReport.AuthorizedCapital = $"{egrDetails?.UL?.StatedCapital?.Sum.ToString()} руб.";
      DebReport.AuthorizedCapitalDate = egrDetails?.UL?.StatedCapital?.Date;
      
      // Заполняем данные о руководителе.
      if (konturCompany?.UL?.Heads != null && konturCompany?.UL?.Heads.Count > 0)
      {
        var head = konturCompany?.UL?.Heads.FirstOrDefault();
        DebReport.GDJobTitle = head?.Position;
        DebReport.GDDate = head?.Date;
        DebReport.GDName = $"{head?.Fio}, ИНН {head?.Innfl}";
        DebReport.Director = $"{head?.Fio}, {head?.Position}, ИНН {head?.Innfl}";
        DebReport.DirectorDate = $"{head?.Date}";
        if (head?.IsInaccuracy == true)
          DebReport.IsInaccuracyManager = "Сведения в ЕГРЮЛ о руководителе признаны ФНС недостоверными.";
      }

      var siteDetail = kontur.GetCompanySite(DebReport.Inn);
      DebReport.Sites = GetSites(siteDetail?.Sites);
      var phoneDetail = kontur.GetCompanyPhone(DebReport.Inn);
      DebReport.Phones = GetPhones(phoneDetail?.ContactPhones?.Phones);
      
      var licensesDetail = kontur.GetCompanyLicenses(DebReport.Inn);
      var accountReport = kontur.GetCompanyAccountingReport(DebReport.Inn);
      var taxes = kontur.GetCompanyTaxe(DebReport.Inn);
      var analytic = kontur.GetAnalytic(DebReport.Inn);
      var fnsInfo = kontur.GetFnsBlockedBankAccount(DebReport.Inn);
      var bankAccounts = kontur.GetBankAccounts(DebReport.Inn);
      var petitionersOfArbitration = kontur.GetPetitionersOfArbitration(DebReport.Inn);
      var purchasesOfParticipant = kontur.GetPurchasesOfParticipant(DebReport.Inn);
      var bankruptcy = kontur.GetCompanyBankruptcy(DebReport.Inn);
      if (!string.IsNullOrEmpty(bankruptcy.CaseNumber))
        DebReport.CaseNumber = $"Номер арбитражного дела о банкротстве: {bankruptcy.CaseNumber}";
      
      var pledger = kontur.GetPledger(DebReport.Inn);
      if (pledger.Pledges != null && pledger.Pledges.Count() > 0)
        DebReport.Pledges = "По данным Контур.Фокус в собственности контрагента имеется движимое имуществао, переданное в залог. Информация о заложенном имуществе представлена в приложении _.";

      // Заполняем адрес.
      if (konturCompany?.UL?.LegalAddress?.ParsedAddressRF != null)
        DebReport.LegalAddress = FillAddressRF(konturCompany?.UL?.LegalAddress?.ParsedAddressRF);
      else if (konturCompany?.UL?.LegalAddress?.ParsedAddressRFFias != null)
        DebReport.LegalAddress = FillAddressRFias(konturCompany?.UL?.LegalAddress?.ParsedAddressRFFias);
      
      if (konturCompany?.UL?.LegalAddress?.IsInaccuracy == true)
        DebReport.IsInaccuracy = "Сведения в ЕГРЮЛ об адресе местонахождения признаны ФНС недостоверными.";
      
      if (!string.IsNullOrEmpty(konturCompany?.UL?.LegalAddress?.InaccuracyDate))
        DebReport.InaccuracyDate = $"{konturCompany?.UL?.LegalAddress?.InaccuracyDate}";

      GetPetitionersOfArbitrations(petitionersOfArbitration);
      GetDirectors(konturCompany);
      GetActivities(egrDetails);
      GetFounders(egrDetails);
      GetHisotoryFounders(egrDetails);
      GetFilials(konturCompany);
      GetLicenses(licensesDetail);
      GetFinances(accountReport);
      GetTaxes(taxes);
      GetAnalutics(analytic);
      GetFns(analytic, fnsInfo);
      GetHistoryUL(konturCompany);
      GetBankAccounts(bankAccounts);
      GetContracts(purchasesOfParticipant);
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "Статистика о контрактах".
    /// </summary>
    /// <param name="konturCompany"></param>
    private void GetContracts(KonturFocusIntegration.Models.PurchasesOfParticipant.PurchasesOfParticipantModel purchasesOfParticipant)
    {
      // Получаем контрактов.
      try
      {
        var contracts = new List<Structures.DebReport.DebContract>();
        
        if (purchasesOfParticipant?.PurchasesOfParticipant != null)
        {
          var number = 1;
          
          foreach (var rawContract in purchasesOfParticipant.PurchasesOfParticipant)
          {
            var contract = Structures.DebReport.DebContract.Create();
            contract.SessionId = DebReport.SessionId;
            contract.Number = $"{number}";
            contract.Type = $"{rawContract?.Type}";
            contract.Customer = $"{rawContract?.Customers?.FirstOrDefault()?.Name}";
            contract.Inn = $"{rawContract?.Customers?.FirstOrDefault()?.Inn}";
            contract.Description = $"{rawContract?.TopicDescription}";
            contract.ContractNumber = $"{rawContract?.ContractInfo?.Number}";
            contract.ContractDate = $"{rawContract?.ContractInfo?.Date}";
            contract.Price = $"{rawContract?.ContractInfo?.Price?.Sum / 1000}";
            contract.Status = $"{rawContract?.ContractInfo?.Stage}";
            contract.Reason = $"Нет данных";
            
            number++;
            contracts.Add(contract);
          }
        }
        
        if (contracts.Count <= 0)
        {
          var contract = Structures.DebReport.DebContract.Create();
          contract.SessionId = DebReport.SessionId;
          contract.Number = "-";
          contract.Type = "-";
          contract.Customer = "-";
          contract.Inn = "-";
          contract.Description = "-";
          contract.ContractNumber = "-";
          contract.ContractDate = "-";
          contract.Price = "-";
          contract.Status = "-";
          contract.Reason = "-";
          
          contracts.Add(contract);
        }
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebContracts, contracts);
      }
      catch (Exception ex)
      {
        Logger.Error($"Contracts: {ex}");
      }
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "Статистика по истцам в арбитраже".
    /// </summary>
    /// <param name="konturCompany"></param>
    private void GetPetitionersOfArbitrations(KonturFocusIntegration.Models.PetitionersOfArbitration.PetitionersOfArbitrationModel petitionersOfArbitration)
    {
      // Получаем истцов.
      try
      {
        var arbitrations = new List<Structures.DebReport.DebArbitration>();
        
        if (petitionersOfArbitration?.Petitioners != null)
        {
          var number = 1;
          
          foreach (var rawArbitration in petitionersOfArbitration.Petitioners)
          {
            var arbitration = Structures.DebReport.DebArbitration.Create();
            arbitration.SessionId = DebReport.SessionId;
            arbitration.Number = $"{number}";
            arbitration.Name = $"{rawArbitration.Name}";
            arbitration.Inn = $"{rawArbitration.Inn}";
            arbitration.MainActivity = $"{rawArbitration?.PrincipalActivity?.Text}";
            arbitration.Count = $"{rawArbitration.Y3Count}";
            arbitration.Summ = $"{rawArbitration.Y3Sum}";
            arbitration.LastDate = $"{rawArbitration.LastCaseDate}";
            
            number++;
            arbitrations.Add(arbitration);
          }
        }
        
        if (arbitrations.Count <= 0)
        {
          var arbitration = Structures.DebReport.DebArbitration.Create();
          arbitration.SessionId = DebReport.SessionId;
          arbitration.Number = "-";
          arbitration.Name = "-";
          arbitration.Inn = "-";
          arbitration.MainActivity = "-";
          arbitration.Count = "-";
          arbitration.Summ = "-";
          arbitration.LastDate = "-";

          arbitrations.Add(arbitration);
        }
        
        if (arbitrations.Count >= 50)
          DebReport.ArbitrationCount50 = "В таблице представлены 50 истцов с наибольшими суммами исковых требований за последние 3 года.";
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebArbitrations, arbitrations);
      }
      catch (Exception ex)
      {
        Logger.Error($"Arbitrations: {ex}");
      }
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "История руководителей".
    /// </summary>
    /// <param name="konturCompany"></param>
    private void GetDirectors(KonturFocusIntegration.Models.CompanyInfo.CompanyInfoModel konturCompany)
    {
      // Получаем историю руководителей.
      try
      {
        var directors = new List<Structures.DebReport.DebDirector>();
        
        if (konturCompany?.UL?.History?.Heads != null)
        {
          foreach (var rawDirector in konturCompany.UL.History.Heads)
          {
            var director = Structures.DebReport.DebDirector.Create();
            director.SessionId = DebReport.SessionId;
            director.Name = rawDirector.Fio;
            director.Date = rawDirector.Date;
            
            directors.Add(director);
          }
        }
        
        if (directors.Count <= 0)
        {
          var director = Structures.DebReport.DebDirector.Create();
          director.SessionId = DebReport.SessionId;
          director.Name = "-";
          director.Date = "-";

          directors.Add(director);
        }
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebDirectors, directors);
      }
      catch (Exception ex)
      {
        Logger.Error($"GetDirectors: {ex}");
      }
    }
    
    private void GetBankAccounts(KonturFocusIntegration.Models.BankAccounts.BankAccountsModel bankAccounts)
    {
      // Получаем историю.
      try
      {
        var banks = new List<Structures.DebReport.DebBanks>();
        
        if (bankAccounts?.Banks != null)
        {
          var bankList = bankAccounts.Banks.Take(50);
          var number = 1;
          
          foreach (var rawBank in bankList)
          {
            var bank = Structures.DebReport.DebBanks.Create();
            bank.SessionId = DebReport.SessionId;
            
            bank.Number = $"{number}";
            bank.Bik = $"{rawBank.Bik}";
            bank.Name = $"{rawBank.Name}";
            
            var info = rawBank.Info.FirstOrDefault();
            
            if (info != null)
            {
              bank.Source = $"{info.Source}";
              bank.Date = $"{info.UpdateDate}";
            }
            
            number++;
            banks.Add(bank);
          }
        }
        
        if (banks.Count <= 0)
        {
          var bank = Structures.DebReport.DebBanks.Create();
          bank.SessionId = DebReport.SessionId;
          bank.Number = "-";
          bank.Bik = "-";
          bank.Name = "-";
          bank.Source = "-";
          bank.Date = "-";

          banks.Add(bank);
        }
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebBanks, banks);
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetBanks: {ex}");
      }
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "История ЮР Адресов".
    /// </summary>
    /// <param name="konturCompany"></param>
    private void GetHistoryUL(KonturFocusIntegration.Models.CompanyInfo.CompanyInfoModel konturCompany)
    {
      // Получаем историю
      try
      {
        var historyUL = new List<Structures.DebReport.DebHistoryUL>();
        
        if (konturCompany?.UL?.History?.LegalAddresses != null)
        {
          foreach (var rawAddress in konturCompany.UL.History.LegalAddresses)
          {
            var address = Structures.DebReport.DebHistoryUL.Create();
            address.SessionId = DebReport.SessionId;
            address.Address = FillAddressRF(rawAddress.ParsedAddressRF);
            address.Date = rawAddress.Date;
            
            historyUL.Add(address);
          }
        }
        
        if (historyUL.Count <= 0)
        {
          var address = Structures.DebReport.DebHistoryUL.Create();
          address.SessionId = DebReport.SessionId;
          address.Address = "-";
          address.Date = "-";

          historyUL.Add(address);
        }
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebHistoryUL, historyUL);
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetHistoryUL: {ex}");
      }
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "Сведения из ФНС об отсутствии задолженности".
    /// </summary>
    /// <param name="egrDetails"></param>
    private void GetFns(KonturFocusIntegration.Models.Analytic.CompanyAnalyticModel analytic, KonturFocusIntegration.Models.FnsBlockedBankAccount.FnsBlockedBankAccountModel fnsInfo)
    {
      try
      {
        
        DebReport.DateUpdate = $"Последняя проверка наличия ограничений (по данным Контур.Фокус): {fnsInfo?.BlockedAccountsInfo?.UpdateDate}";
        if (analytic?.Analytics?.M7010 != null && analytic.Analytics.M7010 == true)
          DebReport.m7010 = "Имеется информация о наличии ограничений на операции по банковским счетам контрагента, установленных ФНС. Необходима проверка актуальности данных.";
        
        if (analytic?.Analytics?.M5004 != null && analytic.Analytics.M5004 == true)
          DebReport.m5004 = "Организация найдена в списке юридических лиц, имеющих задолженность по уплате налогов более 1000 руб., которая направлялась на взыскание судебному приставу-исполнителю (ФНС).";
        else
          DebReport.m5004 = "Организация не найдена в списке юридических лиц, имеющих задолженность по уплате налогов более 1000 руб., которая направлялась на взыскание судебному приставу-исполнителю (ФНС).";
        
        if (analytic?.Analytics?.M5005 != null && analytic.Analytics.M5005 == true)
          DebReport.m5005 = "Организация найдена в списке юридических лиц, не представляющих налоговую отчетность более года (ФНС). ";
        else
          DebReport.m5005 = "Организация не найдена в списке юридических лиц, не представляющих налоговую отчетность более года (ФНС).";
        
        // Обнуляем.
        DebReport.q2002 = $"{analytic?.Analytics?.Q2002}";
        DebReport.q2011 = $"{analytic?.Analytics?.Q2011}";
        DebReport.q2012 = $"{analytic?.Analytics?.Q2012}";
        DebReport.q2013 = $"{analytic?.Analytics?.Q2013}";
        DebReport.q2014 = $"{analytic?.Analytics?.Q2014}";
        DebReport.q2015 = $"{analytic?.Analytics?.Q2015}";
        DebReport.q2029 = $"{analytic?.Analytics?.Q2029}";
        DebReport.s2002 = $"{analytic?.Analytics?.S2002}";
        DebReport.s2011 = $"{analytic?.Analytics?.S2011}";
        DebReport.s2012 = $"{analytic?.Analytics?.S2012}";
        DebReport.s2013 = $"{analytic?.Analytics?.S2013}";
        DebReport.s2014 = $"{analytic?.Analytics?.S2014}";
        DebReport.s2015 = $"{analytic?.Analytics?.S2015}";
        DebReport.s2029 = $"{analytic?.Analytics?.S2029}";
        DebReport.q2001 = $"{analytic?.Analytics?.Q2001}";
        DebReport.q2016 = $"{analytic?.Analytics?.Q2016}";
        DebReport.q2017 = $"{analytic?.Analytics?.Q2017}";
        DebReport.q2018 = $"{analytic?.Analytics?.Q2018}";
        DebReport.q2019 = $"{analytic?.Analytics?.Q2019}";
        DebReport.q2027 = $"{analytic?.Analytics?.Q2027}";
        DebReport.q2028 = $"{analytic?.Analytics?.Q2028}";
        // Сумма исков.
        DebReport.s2001 = $"{analytic?.Analytics?.S2001 / 1000}";
        DebReport.s2016 = $"{analytic?.Analytics?.S2016 / 1000}";
        DebReport.s2017 = $"{analytic?.Analytics?.S2017 / 1000}";
        DebReport.s2018 = $"{analytic?.Analytics?.S2018 / 1000}";
        DebReport.s2019 = $"{analytic?.Analytics?.S2019 / 1000}";
        DebReport.s2027 = $"{analytic?.Analytics?.S2027 / 1000}";
        DebReport.s2028 = $"{analytic?.Analytics?.S2028 / 1000}";
        
        if (analytic?.Analytics?.Q7026 == null || analytic?.Analytics?.Q7026 == 0)
          DebReport.q7026 = "Арбитражные дела о банкротстве контрагента не обнаружены.";
        else
        {
          DebReport.q7026 = $"Обнаружены {analytic?.Analytics?.Q7026} арбитражных дел о банкротстве контрагента.";
          if (analytic?.Analytics?.Q2041 != null)
            DebReport.q7026 += $" Из них в рассмотрении на момент проверки: {analytic?.Analytics?.Q2041}.";
        }
        
        if (!string.IsNullOrEmpty(analytic?.Analytics?.E7014))
          DebReport.e7014 = $"Последняя введенная процедура банкротства: {analytic?.Analytics?.E7014}";
        else
          DebReport.e7014 = "Информация о введенных процедурах банкротства не установлена.";
        
        if (!string.IsNullOrEmpty(analytic?.Analytics?.D7014))
          DebReport.d7014 = $"Дата решения суда о введении процедуры банкротства: {analytic?.Analytics?.D7014}";
        
        if (analytic?.Analytics?.M7015 != true)
          DebReport.m7015 = "Сообщения кредиторов о намерении обратиться в суд с заявлением о банкротстве контрагента за последний месяц не установлены.";
        else
          DebReport.m7015 = "Обнаружены сообщения кредиторов о намерении обратиться в суд с заявлением о банкротстве контрагента за последний месяц.";
        
        DebReport.s1001 = $"{analytic?.Analytics?.S1001}";
        DebReport.s1002 = $"{analytic?.Analytics?.S1002}";
        DebReport.q1001 = $"{analytic?.Analytics?.Q1001}";
        DebReport.q1002 = $"{analytic?.Analytics?.Q1002}";
        
        // m7035 вывод текста в зависимости от условий.
        if (analytic?.Analytics?.M7035 == true)
          DebReport.m7035 = "Контрагент включен в единый реестр членов СРО Ассоциации «Национальное объединение строителей» (НОСТРОЙ).";
        else
          DebReport.m7035 = "Информация о включении контрагента в единый реестр членов СРО Ассоциации «Национальное объединение строителей» (НОСТРОЙ) не установлена.";
        
        if (analytic?.Analytics?.M7036 == true)
          DebReport.m7036 = "Контрагент включен в единый реестр членов СРО Национального объединения изыскателей и проектировщиков (НОПРИЗ).";
        else
          DebReport.m7036 = "Информация о включении контрагента в единый реестр членов СРО Национального объединения изыскателей и проектировщиков (НОПРИЗ) не установлена.";
        
        // Табличную часть.
        var fnsList = new List<Structures.DebReport.DebFns>();
        
        if (fnsInfo?.BlockedAccountsInfo?.Suspensions == null)
        {
          var fns = Structures.DebReport.DebFns.Create();
          fns.SessionId = DebReport.SessionId;
          fns.Bik = "-";
          fns.Name = "-";
          fns.Date = "-";
          fns.Number = "-";
          
          fnsList.Add(fns);
        }
        else
        {
          foreach (var suspension in fnsInfo.BlockedAccountsInfo.Suspensions)
          {
            var fns = Structures.DebReport.DebFns.Create();
            fns.SessionId = DebReport.SessionId;
            fns.Bik = suspension.Bik;
            fns.Name = suspension.Name;
            fns.Date = suspension.Date;
            fns.Number = suspension.Number;
            
            fnsList.Add(fns);
          }
        }

        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebFns, fnsList);
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetFns: {ex}");
      }
    }
    
    /// <summary>
    /// Заполняем адрес компании.
    /// </summary>
    /// <param name="parseAddressRF"></param>
    private string FillAddressRF(KonturFocusIntegration.Models.CompanyInfo.ParsedAddressRFModel parseAddressRF)
    {
      var result = "";
      
      if (parseAddressRF == null)
        return result;
      
      // Формируем Юридический адресс.
      if (parseAddressRF?.ZipCode != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRF?.ZipCode}", ",");
      
      if (parseAddressRF?.RegionName != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRF?.RegionName?.TopoShortName}. {parseAddressRF?.RegionName?.TopoValue}", ",");
      
      if (parseAddressRF?.City != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRF?.City?.TopoShortName}. {parseAddressRF?.City?.TopoValue}", ",");
      
      if (parseAddressRF?.Settlement != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRF?.Settlement?.TopoShortName}. {parseAddressRF?.Settlement?.TopoValue}", ",");
      
      if (parseAddressRF?.District != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRF?.District?.TopoShortName}. {parseAddressRF?.District?.TopoValue}", ",");

      if (parseAddressRF?.Street != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRF?.Street?.TopoShortName}. {parseAddressRF?.Street?.TopoValue}", ",");
      
      if (parseAddressRF?.House != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRF?.House?.TopoShortName}. {parseAddressRF?.House?.TopoValue}", ",");
      
      if (parseAddressRF?.BulkRaw != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRF?.BulkRaw}", ",");
      
      if (parseAddressRF?.Flat != null)
      {
        if (string.IsNullOrEmpty(parseAddressRF?.Flat?.TopoShortName))
          result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRF?.Flat?.TopoValue}", ",");
        else
          result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRF?.Flat?.TopoShortName}. {parseAddressRF?.Flat?.TopoValue}", ",");
      }
      
      return result;
    }
    
    /// <summary>
    /// Заполняем адрес компании если это новые территории.
    /// </summary>
    /// <param name="parseAddressRFFias"></param>
    private string FillAddressRFias(KonturFocusIntegration.Models.CompanyInfo.ParsedAddressRFFiasModel parseAddressRFFias)
    {
      // Заполняем название региона.
      var result = "";
      
      if (parseAddressRFFias == null)
        return result;
      
      // Добавляем в адрес индекс.
      if (parseAddressRFFias?.ZipCode != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRFFias?.ZipCode}", ",");
      
      // Добавляем в адрес название региона.
      if (parseAddressRFFias?.Region != null)
      {
        parseAddressRFFias.Region.TopoShortName = parseAddressRFFias.Region.TopoShortName.Trim('.');
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRFFias?.Region?.TopoShortName}. {parseAddressRFFias?.Region?.TopoValue}", ",");
      }
      // Добавляем в адрес город.
      if (parseAddressRFFias?.City != null)
      {
        parseAddressRFFias.City.TopoShortName = parseAddressRFFias.City.TopoShortName.Trim('.');
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRFFias?.City?.TopoShortName}. {parseAddressRFFias?.City?.TopoValue}", ",");
      }
      // Добавляем в адрес поселок\деревни.
      if (parseAddressRFFias?.Settlement != null)
      {
        parseAddressRFFias.Settlement.TopoShortName = parseAddressRFFias.Settlement.TopoShortName.Trim('.');
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRFFias?.Settlement?.TopoShortName}. {parseAddressRFFias?.Settlement?.TopoValue}", ",");
      }
      // Добавляем в адрес улицу.
      if (parseAddressRFFias?.Street != null)
      {
        parseAddressRFFias.Street.TopoShortName = parseAddressRFFias.Street.TopoShortName.Trim('.');
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRFFias?.Street?.TopoShortName}. {parseAddressRFFias?.Street?.TopoValue}", ",");
      }
      // Добавляем в адрес номер дома.
      if (parseAddressRFFias?.House != null)
      {
        parseAddressRFFias.House.TopoShortName = parseAddressRFFias.House.TopoShortName.Trim('.');
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRFFias?.House?.TopoShortName}. {parseAddressRFFias?.House?.TopoValue}", ",");
      }
      
      // Добавляем в адрес номер корпуса строения\дома.
      if (parseAddressRFFias?.BulkRaw != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRFFias?.BulkRaw}", ",");
      else if(parseAddressRFFias?.Buildings[0] != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRFFias.Buildings[0].TopoFullName} {parseAddressRFFias.Buildings[0].TopoValue}", ",");
      // Добавляем в адрес номер квартиры.
      if (parseAddressRFFias?.Flat != null)
      {
        parseAddressRFFias.Flat.TopoShortName = parseAddressRFFias.Flat.TopoShortName.Trim('.');
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{parseAddressRFFias?.Flat?.TopoShortName}. {parseAddressRFFias?.Flat?.TopoValue}", ",");
      }
      
      return result;
    }

    /// <summary>
    /// Заполнение таблицы с данными "Уплаченные налоги и сборы".
    /// </summary>
    /// <param name="egrDetails"></param>
    private void GetAnalutics(KonturFocusIntegration.Models.Analytic.CompanyAnalyticModel analytic)
    {
      try
      {
        if (analytic?.Analytics?.M4001 != null && analytic.Analytics.M4001 == 1)
          DebReport.m4001 = "Контрагент включен в Реестр недобросовестных поставщиков по данным ФАС, Федерального казначейства. Рекомендовано проведение дополнительной проверки актуальности информации.";
        
        if (analytic?.Analytics?.Q4003 == null && analytic?.Analytics?.S4003 == null)
          DebReport.NoContract = "Информация о госконтрактах, заключенных контрагентом в рамках 44-ФЗ, 94-ФЗ, 223-ФЗ не установлена.";
        
        DebReport.s4002 = analytic?.Analytics?.S4002 != null ? $"{analytic?.Analytics?.S4002 / 1000}" : "-";
        DebReport.s4003 = analytic?.Analytics?.S4003 != null ? $"{analytic?.Analytics?.S4003 / 1000}" : "-";
        DebReport.q4002 = analytic?.Analytics?.Q4002 != null ? $"{analytic?.Analytics?.Q4002 / 1000}" : "-";
        DebReport.q4003 = analytic?.Analytics?.Q4003 != null ? $"{analytic?.Analytics?.Q4003 / 1000}" : "-";
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetAnalutics: {ex}");
      }
    }
    
    /// <summary>
    /// Преобразование данных по налогам из контура в отчётный вид.
    /// </summary>
    /// <param name="rawTaxe">Не обработанный список уплаченныз налогов.</param>
    /// <param name="taxes">Список уплаченных налогов.</param>
    /// <param name="yearNumber">Столбец в табличной части отчёта, куда поместятся значения.</param>
    private void CheckTaxe(KonturFocusIntegration.Models.CompanyTaxe.TaxeModel rawTaxe, List<Structures.DebReport.DebTaxe> taxes, int yearNumber)
    {
      foreach (var data in rawTaxe.Data)
      {
        // Проверяем есть ли такой налог уже в списке.
        var taxe = taxes.FirstOrDefault(t => t.Name == data.Name);
        
        // Если не нашли налога по названию, вносим его в отчёт.
        if (taxe == null)
        {
          taxe = Structures.DebReport.DebTaxe.Create();
          taxe.SessionId = DebReport.SessionId;
          taxe.Name = data.Name;
          taxe.Year1 = "-";
          taxe.Year2 = "-";
          taxe.Year3 = "-";
          
          taxes.Add(taxe);
        }
        
        // Заполняем данные о налоги в зависимости за какой год они пришли.
        if (yearNumber == 1)
          taxe.Year1 = $"{data.Sum / 1000}";
        
        if (yearNumber == 2)
          taxe.Year2 = $"{data.Sum / 1000}";
        
        if (yearNumber == 3)
          taxe.Year3 = $"{data.Sum / 1000}";
      }
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "Уплаченные налоги и сборы".
    /// </summary>
    /// <param name="egrDetails"></param>
    private void GetTaxes(KonturFocusIntegration.Models.CompanyTaxe.CompanyTaxeModel taxeInfo)
    {
      try
      {
        var taxes = new List<Structures.DebReport.DebTaxe>();
        var year = Calendar.Now.Year;
        
        if (taxeInfo?.Taxes != null)
        {
          foreach (var rawTaxe in taxeInfo.Taxes)
          {
            if (year-3 == rawTaxe.Year)
              CheckTaxe(rawTaxe, taxes, 1);
            
            if (year-2 == rawTaxe.Year)
              CheckTaxe(rawTaxe, taxes, 2);
            
            if (year-1 == rawTaxe.Year)
              CheckTaxe(rawTaxe, taxes, 3);
          }
          Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebTaxes, taxes);
        }
        
        if (taxes.Count == 0)
          DebReport.NoTaxes = $"Информация о сумме уплаченных налогов (сборов) за {year-3} – {year-1} гг. не установлена.";
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetTaxes: {ex}");
      }
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "Финансовое состояние".
    /// </summary>
    /// <param name="licensesDetail"></param>
    private void GetFinances(KonturFocusIntegration.Models.CompanyAccountingReport.CompanyAccountingReportModel accountingReport)
    {
      try
      {
        var finances = new List<Structures.DebReport.DebFinanc>();
        var financ = Structures.DebReport.DebFinanc.Create();
        financ.SessionId = DebReport.SessionId;
        
        // Получаем 3 последних года, без учета нынешнего.
        var year = Calendar.Now.Year;
        DebReport.FinanceYear1 = $"{year-3}";
        DebReport.FinanceYear2 = $"{year-2}";
        DebReport.FinanceYear3 = $"{year-1}";
        
        // Заполняем пустыми данными.
        financ.Code11501 = "Нет данных";
        financ.Code11502 = "Нет данных";
        financ.Code11503 = "Нет данных";
        financ.Code12101 = "Нет данных";
        financ.Code12102 = "Нет данных";
        financ.Code12103 = "Нет данных";
        financ.Code12301 = "Нет данных";
        financ.Code12302 = "Нет данных";
        financ.Code12303 = "Нет данных";
        financ.Code12501 = "Нет данных";
        financ.Code12502 = "Нет данных";
        financ.Code12503 = "Нет данных";
        financ.Code13001 = "Нет данных";
        financ.Code13002 = "Нет данных";
        financ.Code13003 = "Нет данных";
        financ.Code14101 = "Нет данных";
        financ.Code14102 = "Нет данных";
        financ.Code14103 = "Нет данных";
        financ.Code15101 = "Нет данных";
        financ.Code15102 = "Нет данных";
        financ.Code15103 = "Нет данных";
        financ.Code15201 = "Нет данных";
        financ.Code15202 = "Нет данных";
        financ.Code15203 = "Нет данных";
        financ.Code16001 = "Нет данных";
        financ.Code16002 = "Нет данных";
        financ.Code16003 = "Нет данных";
        financ.Code17001 = "Нет данных";
        financ.Code17002 = "Нет данных";
        financ.Code17003 = "Нет данных";
        financ.Code21101 = "Нет данных";
        financ.Code21102 = "Нет данных";
        financ.Code21103 = "Нет данных";
        financ.Code21201 = "Нет данных";
        financ.Code21202 = "Нет данных";
        financ.Code21203 = "Нет данных";
        financ.Code22101 = "Нет данных";
        financ.Code22102 = "Нет данных";
        financ.Code22103 = "Нет данных";
        financ.Code23401 = "Нет данных";
        financ.Code23402 = "Нет данных";
        financ.Code23403 = "Нет данных";
        financ.Code23501 = "Нет данных";
        financ.Code23502 = "Нет данных";
        financ.Code23503 = "Нет данных";
        financ.Code24001 = "Нет данных";
        financ.Code24002 = "Нет данных";
        financ.Code24003 = "Нет данных";
        
        if (accountingReport.BuhForms == null)
        {
          // Заполняем таблицу
          finances.Add(financ);
          Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebFinances, finances);
          return;
        }
        
        foreach (var rawFinance in accountingReport.BuhForms)
        {
          if (year-3 == rawFinance.Year)
          {
            financ.Code11501 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1150)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1150)?.EndValue / 1000}": "Нет данных";
            financ.Code12101 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1210)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1210)?.EndValue / 1000}": "Нет данных";
            financ.Code12301 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1230)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1230)?.EndValue / 1000}": "Нет данных";
            financ.Code12501 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1250)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1250)?.EndValue / 1000}": "Нет данных";
            financ.Code16001 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1600)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1600)?.EndValue / 1000}": "Нет данных";
            
            financ.Code13001 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1300)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1300)?.EndValue / 1000}": "Нет данных";
            financ.Code14101 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1410)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1410)?.EndValue / 1000}": "Нет данных";
            financ.Code15101 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1510)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1510)?.EndValue / 1000}": "Нет данных";
            financ.Code15201 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1520)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1520)?.EndValue / 1000}": "Нет данных";
            financ.Code17001 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1700)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1700)?.EndValue / 1000}": "Нет данных";
            
            financ.Code21101 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2110)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2110)?.EndValue / 1000}": "Нет данных";
            financ.Code21201 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2120)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2120)?.EndValue / 1000}": "Нет данных";
            financ.Code23401 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2340)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2340)?.EndValue / 1000}": "Нет данных";
            financ.Code23501 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2350)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2350)?.EndValue / 1000}": "Нет данных";
            financ.Code24001 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2400)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2400)?.EndValue / 1000}": "Нет данных";
            
            var codeText2210 = "Нет данных";
            var code2210 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2210)?.EndValue;
            var code2220 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2220)?.EndValue;
            if (code2210 != null && code2220 != null)
              codeText2210 = $"{(code2210 + code2220) / 1000}";
            else if (code2210 != null)
              codeText2210 = $"{code2210 / 1000}";
            else if (code2220 != null)
              codeText2210 = $"{code2220 / 1000}";
            
            financ.Code15101 = codeText2210;
          }
          
          if (year-2 == rawFinance.Year)
          {
            financ.Code11502 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1150)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1150)?.EndValue / 1000}": "Нет данных";
            financ.Code12102 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1210)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1210)?.EndValue / 1000}": "Нет данных";
            financ.Code12302 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1230)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1230)?.EndValue / 1000}": "Нет данных";
            financ.Code12502 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1250)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1250)?.EndValue / 1000}": "Нет данных";
            financ.Code16002 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1600)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1600)?.EndValue / 1000}": "Нет данных";
            
            financ.Code13002 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1300)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1300)?.EndValue / 1000}": "Нет данных";
            financ.Code14102 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1410)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1410)?.EndValue / 1000}": "Нет данных";
            financ.Code15102 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1510)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1510)?.EndValue / 1000}": "Нет данных";
            financ.Code15202 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1520)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1520)?.EndValue / 1000}": "Нет данных";
            financ.Code17002 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1700)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1700)?.EndValue / 1000}": "Нет данных";
            
            financ.Code21102 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2110)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2110)?.EndValue / 1000}": "Нет данных";
            financ.Code21202 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2120)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2120)?.EndValue / 1000}": "Нет данных";
            financ.Code23402 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2340)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2340)?.EndValue / 1000}": "Нет данных";
            financ.Code23502 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2350)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2350)?.EndValue / 1000}": "Нет данных";
            financ.Code24002 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2400)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2400)?.EndValue / 1000}": "Нет данных";
            
            var codeText2210 = "Нет данных";
            var code2210 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2210)?.EndValue;
            var code2220 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2220)?.EndValue;
            if (code2210 != null && code2220 != null)
              codeText2210 = $"{(code2210 + code2220) / 1000}";
            else if (code2210 != null)
              codeText2210 = $"{code2210 / 1000}";
            else if (code2220 != null)
              codeText2210 = $"{code2220 / 1000}";
            
            financ.Code15102 = codeText2210;
          }
          
          if (year-1 == rawFinance.Year)
          {
            financ.Code11503 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1150)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1150)?.EndValue / 1000}": "Нет данных";
            financ.Code12103 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1210)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1210)?.EndValue / 1000}": "Нет данных";
            financ.Code12303 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1230)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1230)?.EndValue / 1000}": "Нет данных";
            financ.Code12503 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1250)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1250)?.EndValue / 1000}": "Нет данных";
            financ.Code16003 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1600)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1600)?.EndValue / 1000}": "Нет данных";
            
            financ.Code13003 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1300)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1300)?.EndValue / 1000}": "Нет данных";
            financ.Code14103 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1410)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1410)?.EndValue / 1000}": "Нет данных";
            financ.Code15103 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1510)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1510)?.EndValue / 1000}": "Нет данных";
            financ.Code15203 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1520)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1520)?.EndValue / 1000}": "Нет данных";
            financ.Code17003 = rawFinance.Form1.FirstOrDefault(f => f.Code == 1700)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 1700)?.EndValue / 1000}": "Нет данных";
            
            financ.Code21103 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2110)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2110)?.EndValue / 1000}": "Нет данных";
            financ.Code21203 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2120)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2120)?.EndValue / 1000}": "Нет данных";
            financ.Code23403 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2340)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2340)?.EndValue / 1000}": "Нет данных";
            financ.Code23503 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2350)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2350)?.EndValue / 1000}": "Нет данных";
            financ.Code24003 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2400)?.EndValue != null ? $"{rawFinance.Form1.FirstOrDefault(f => f.Code == 2400)?.EndValue / 1000}": "Нет данных";
            
            var codeText2210 = "Нет данных";
            var code2210 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2210)?.EndValue;
            var code2220 = rawFinance.Form1.FirstOrDefault(f => f.Code == 2220)?.EndValue;
            if (code2210 != null && code2220 != null)
              codeText2210 = $"{(code2210 + code2220) / 1000}";
            else if (code2210 != null)
              codeText2210 = $"{code2210 / 1000}";
            else if (code2220 != null)
              codeText2210 = $"{code2220 / 1000}";
            
            financ.Code15103 = codeText2210;
          }
        }
        
        // Заполняем таблицу
        finances.Add(financ);
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebFinances, finances);
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetFinances: {ex}");
      }
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "Лицензий".
    /// </summary>
    /// <param name="licensesDetail"></param>
    private void GetLicenses(KonturFocusIntegration.Models.CompanyLicense.CompanyLicenseModel licensesDetail)
    {
      try
      {
        if (licensesDetail.Licenses == null || !licensesDetail.Licenses.Any())
          DebReport.NoLicense = "Информация о действующих либо прекративших действие лицензиях не установлена.";
        
        var licenses = new List<Structures.DebReport.DebLicense>();
        
        var noActiveLicense = 0;
        var noInstallLicense = 0;
        
        if (licensesDetail?.Licenses != null)
        {
          foreach (var rawLicense in licensesDetail.Licenses)
          {
            if (string.IsNullOrEmpty(rawLicense.StatusDescription))
              noInstallLicense++;
            
            // В табличную часть только действующие записи.
            if (rawLicense.StatusDescription != avis.EtalonParties.Reports.Resources.DebReport.StatusCurrent || rawLicense?.DateEnd == null)
            {
              noActiveLicense++;
              continue;
            }
            
            // Проверяем что лицензия активна, в противном случае добавляем не активную лицензию.
            var dateEnd = DateTime.Parse(rawLicense?.DateEnd);
            if (dateEnd < Calendar.Now)
            {
              noActiveLicense++;
              continue;
            }
            
            var license = Structures.DebReport.DebLicense.Create();
            license.SessionId = DebReport.SessionId;
            license.Source = $"{rawLicense?.Source}";
            if (!string.IsNullOrEmpty(rawLicense?.IssuerName))
              license.Source += $" / {rawLicense?.IssuerName}";
            
            license.Number = rawLicense?.OfficialNum;
            license.Date = $"{rawLicense?.DateStart}";
            if (!string.IsNullOrEmpty(rawLicense?.DateEnd))
              license.Date += $"-{rawLicense?.DateEnd}";
            
            // services коллекция.
            var services = ListToString(rawLicense?.Services);
            license.Name = $"{rawLicense?.Activity}";
            if (!string.IsNullOrEmpty(services))
              license.Name += $" / {services}";
            
            licenses.Add(license);
          }
        }
        
        if (licenses.Count <= 0)
        {
          var license = Structures.DebReport.DebLicense.Create();
          license.SessionId = DebReport.SessionId;
          license.Source = "-";
          license.Number = "-";
          license.Date = "-";
          license.Name = "-";
          
          DebReport.NoLicense = "Информация о действующих лицензиях не установлена.";
          DebReport.NonoLinecses = "Информация о прекративших действие лицензиях не установлена.";
          
          licenses.Add(license);
        }
        
        if (noActiveLicense > 0)
          DebReport.NoActiveLicense = $"Установлено наличие {noActiveLicense} недействующих лицензий.";
        
        if (noInstallLicense > 0)
          DebReport.NoInstalledLicense = $"Установлено наличие {noInstallLicense} лицензий, для которых статус действия не установлен.";
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebLicenses, licenses);
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetLicenses: {ex}");
      }
    }
    
    /// <summary>
    /// Заполняем адрес компании если это филиал.
    /// </summary>
    /// <param name="companyInfo"></param>
    private string GetAddressBranch(KonturFocusIntegration.Models.CompanyInfo.BranchModel konturBranch)
    {
      var result = "";
      
      // Формируем Юридический адресс.
      if (konturBranch?.ParsedAddressRF?.ZipCode != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{konturBranch?.ParsedAddressRF?.ZipCode}", ",");
      if (konturBranch?.ParsedAddressRF?.RegionName != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{konturBranch?.ParsedAddressRF?.RegionName?.TopoShortName}. {konturBranch?.ParsedAddressRF?.RegionName?.TopoValue}", ",");
      if (konturBranch?.ParsedAddressRF?.City != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{konturBranch?.ParsedAddressRF?.City?.TopoShortName}. {konturBranch?.ParsedAddressRF?.City?.TopoValue}", ",");
      if (konturBranch?.ParsedAddressRF?.Settlement != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{konturBranch?.ParsedAddressRF?.Settlement?.TopoShortName}. {konturBranch?.ParsedAddressRF?.Settlement?.TopoValue}", ",");
      if (konturBranch?.ParsedAddressRF?.Street != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{konturBranch?.ParsedAddressRF?.Street?.TopoShortName}. {konturBranch?.ParsedAddressRF?.Street?.TopoValue}", ",");
      if (konturBranch?.ParsedAddressRF?.House != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{konturBranch?.ParsedAddressRF?.House?.TopoShortName}. {konturBranch?.ParsedAddressRF?.House?.TopoValue}", ",");
      if (konturBranch?.ParsedAddressRF?.BulkRaw != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{konturBranch?.ParsedAddressRF?.BulkRaw}", ",");
      if (konturBranch?.ParsedAddressRF?.Flat != null)
        result = lenspec.Etalon.Module.Parties.PublicFunctions.Module.FormConcatination(result, $"{konturBranch?.ParsedAddressRF?.Flat?.TopoShortName}. {konturBranch?.ParsedAddressRF?.Flat?.TopoValue}", ",");
      
      return result;
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "Филиалы".
    /// </summary>
    /// <param name="egrDetails"></param>
    private void GetFilials(KonturFocusIntegration.Models.CompanyInfo.CompanyInfoModel req)
    {
      try
      {
        var filials = new List<Structures.DebReport.DebFilial>();
        
        if (req.UL.Branches != null)
        {
          foreach (var rawFilial in req.UL.Branches)
          {
            var filial = Structures.DebReport.DebFilial.Create();
            filial.SessionId = DebReport.SessionId;
            filial.Date = rawFilial.Date;
            filial.Address = GetAddressBranch(rawFilial);
            
            filials.Add(filial);
          }
        }
        
        if (filials.Count <= 0)
        {
          var filial = Structures.DebReport.DebFilial.Create();
          filial.SessionId = DebReport.SessionId;
          filial.Address = "-";
          filial.Date = "-";
          
          filials.Add(filial);
        }
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebFilials, filials);
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetFilials: {ex}");
      }
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "Учредители История".
    /// </summary>
    /// <param name="egrDetails"></param>
    private void GetHisotoryFounders(KonturFocusIntegration.Models.CompanyEgrDetail.CompanyEgrDetailModel egrDetails)
    {
      try
      {
        var historyFounders = new List<Structures.DebReport.DebHistoryFounder>();
        
        // Учеридители ФЛ.
        if (egrDetails.UL?.History?.FoundersFL != null)
        {
          foreach (var rawHistoryFounder in egrDetails.UL?.History?.FoundersFL)
          {
            var historyFounder = Structures.DebReport.DebHistoryFounder.Create();
            historyFounder.SessionId = DebReport.SessionId;
            historyFounder.ShareRub = rawHistoryFounder.Share.Sum.ToString();
            historyFounder.Name = $"{rawHistoryFounder.Fio}, {rawHistoryFounder.Innfl}";
            historyFounder.Date = rawHistoryFounder.Date;
            
            historyFounders.Add(historyFounder);
          }
        }
        
        // Учеридители ЮЛ.
        if (egrDetails.UL?.History?.FoundersUL != null)
        {
          foreach (var rawHistoryFounder in egrDetails.UL?.History?.FoundersUL)
          {
            var historyFounder = Structures.DebReport.DebHistoryFounder.Create();
            historyFounder.SessionId = DebReport.SessionId;
            historyFounder.ShareRub = rawHistoryFounder.Share.Sum.ToString();
            historyFounder.Name = $"{rawHistoryFounder.FullName}, {rawHistoryFounder.Inn}";
            historyFounder.Date = rawHistoryFounder.Date;
            
            historyFounders.Add(historyFounder);
          }
        }
        
        // Учеридители Иностранный ЮЛ.
        if (egrDetails.UL?.History?.FoundersForeign != null)
        {
          foreach (var rawHistoryFounder in egrDetails.UL?.History?.FoundersForeign)
          {
            var historyFounder = Structures.DebReport.DebHistoryFounder.Create();
            historyFounder.SessionId = DebReport.SessionId;
            historyFounder.ShareRub = rawHistoryFounder.Share.Sum.ToString();
            historyFounder.Name = $"{rawHistoryFounder.FullName}, {rawHistoryFounder.Country}";
            historyFounder.Date = rawHistoryFounder.Date;
            
            historyFounders.Add(historyFounder);
          }
        }
        
        if (historyFounders.Count <= 0)
        {
          var historyFounder = Structures.DebReport.DebHistoryFounder.Create();
          historyFounder.SessionId = DebReport.SessionId;
          historyFounder.ShareRub = "-";
          historyFounder.Name = "-";
          historyFounder.Date = "-";
          
          historyFounders.Add(historyFounder);
        }
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebHistoryFounders, historyFounders);
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetHistoryFounders: {ex}");
      }
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "Учредители".
    /// </summary>
    /// <param name="egrDetails"></param>
    private void GetFounders(KonturFocusIntegration.Models.CompanyEgrDetail.CompanyEgrDetailModel egrDetails)
    {
      try
      {
        var founders = new List<Structures.DebReport.DebFounder>();
        
        if (egrDetails.UL?.FoundersFL != null)
        {
          foreach (var rawFounder in egrDetails.UL.FoundersFL)
          {
            var founder = Structures.DebReport.DebFounder.Create();
            founder.SessionId = DebReport.SessionId;
            founder.Share = rawFounder.Share.PercentagePlain.ToString();
            founder.ShareRub = rawFounder.Share.Sum.ToString();
            founder.Name = $"{rawFounder.Fio}, {rawFounder.Innfl}";
            founder.Date = rawFounder.Date;
            founder.Comment = "";
            
            founders.Add(founder);
          }
        }
        
        if (founders.Count <= 0)
        {
          var founder = Structures.DebReport.DebFounder.Create();
          founder.SessionId = DebReport.SessionId;
          founder.Share = "-";
          founder.ShareRub = "-";
          founder.Name = "-";
          founder.Date = "-";
          founder.Comment = "-";
          
          founders.Add(founder);
        }
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebFounders, founders);
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetFounders: {ex}");
      }
    }
    
    /// <summary>
    /// Заполнение таблицы с данными "Виды деятельности".
    /// </summary>
    /// <param name="egrDetails"></param>
    private void GetActivities(KonturFocusIntegration.Models.CompanyEgrDetail.CompanyEgrDetailModel egrDetails)
    {
      try
      {
        // Заполняем основной вид деятельности.
        if (egrDetails.UL != null)
          DebReport.MainViewActivities = egrDetails.UL.Activities.PrincipalActivity.Text;
        
        if (egrDetails.IP != null)
          DebReport.MainViewActivities = egrDetails.IP.Activities.PrincipalActivity.Text;
        
        // Заполняем виды деятельности.
        var avtivities = new List<Structures.DebReport.DebActivity>();
        
        // Добавляем основной вид деятельности.
        if (egrDetails?.UL?.Activities?.PrincipalActivity != null)
        {
          var activity = Structures.DebReport.DebActivity.Create();
          activity.SessionId = DebReport.SessionId;
          activity.Name = egrDetails.UL.Activities.PrincipalActivity.Text;
          activity.Code = egrDetails.UL.Activities.PrincipalActivity.Code;
          
          avtivities.Add(activity);
        }
        if (egrDetails?.IP?.Activities?.PrincipalActivity != null)
        {
          var activity = Structures.DebReport.DebActivity.Create();
          activity.SessionId = DebReport.SessionId;
          activity.Name = egrDetails.IP.Activities.PrincipalActivity.Text;
          activity.Code = egrDetails.IP.Activities.PrincipalActivity.Code;
          
          avtivities.Add(activity);
        }
        
        // ComplementaryActivities
        if (egrDetails?.IP?.Activities?.ComplementaryActivities != null)
        {
          foreach (var rawActivity in egrDetails.IP.Activities.ComplementaryActivities)
          {
            var activity = Structures.DebReport.DebActivity.Create();
            activity.SessionId = DebReport.SessionId;
            activity.Name = rawActivity.Text;
            activity.Code = rawActivity.Code;
            
            avtivities.Add(activity);
          }
        }
        
        if (avtivities.Count <= 0)
        {
          var activity = Structures.DebReport.DebActivity.Create();
          activity.SessionId = DebReport.SessionId;
          activity.Name = "";
          activity.Code = "";
          
          avtivities.Add(activity);
        }
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DebReport.TableNameDebActivities, avtivities);
      }
      catch (Exception ex)
      {
        Logger.Error($"DebGetActivities: {ex}");
      }
    }

    /// <summary>
    /// Первые 10 сайтов из Контур.Фокус.
    /// </summary>
    /// <param name="sites"></param>
    /// <returns></returns>
    private string GetSites(List<string> sites)
    {
      if (sites == null || sites.Count <= 0)
        return "";
      
      var list = sites.Take(10);
      var result = "";
      
      
      foreach (var site in list)
      {
        if (string.IsNullOrEmpty(result))
          result += site;
        else
          result += $", {site}";
      }
      
      return result;
    }

    /// <summary>
    /// Первые 10 телефонов из Контур.Фокус.
    /// </summary>
    /// <param name="sites"></param>
    /// <returns></returns>
    private string GetPhones(List<string> phones)
    {
      if (phones == null || phones.Count <= 0)
        return "";
      
      var list = phones.Take(10);
      var result = "";
      
      
      foreach (var phone in list)
      {
        if (string.IsNullOrEmpty(result))
          result += phone;
        else
          result += $", {phone}";
      }
      
      return result;
    }
    
    private string ListToString(List<string> list)
    {
      var result = "";
      
      if (list == null)
        return result;
      
      foreach (var item in list)
      {
        if (string.IsNullOrEmpty(result))
          result += item;
        else
          result += $", {item}";
      }
      
      return result;
    }
  }
}