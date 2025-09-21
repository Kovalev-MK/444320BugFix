using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;
using Dadata;
using Dadata.Model;
using Minio;
using Minio.Exceptions;
using AvisIntegrationHelper;

namespace lenspec.Etalon.Module.Integration.Server
{
  partial class ModuleFunctions
  {

    #region Загрузка Контрагентов и Д5 (Таблицы Интегры)
    
    /// <summary>
    /// Выполнить миграцию КА из Директум 5
    /// </summary>
    public static void ExecuteCounterpartyMigration()
    {
      Logger.Debug("Загрузка контрагентов из Д5. Старт.");
      try
      {
        var connectionString = GetConnectionStringForCounterpartyMigration();
        var models = AvisIntegrationHelper.DataBaseHelper.GetCounterpartyMigrationModels(connectionString);
        var results = ProcessingCounterpartyModels(models);
        AvisIntegrationHelper.DataBaseHelper.FillCounterpartyMigrationResult(connectionString, results);
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Загрузка контрагентов из Д5. Ошибка - {0}", ex.Message, innerMessage);
      }
      Logger.Debug("Загрузка контрагентов из Д5. Завершение.");
    }

    /// <summary>
    /// Получить строку подключения к БД, в которой находится таблица TenderDocOrganizationsToRX
    /// </summary>
    /// <returns></returns>
    private static string GetConnectionStringForCounterpartyMigration()
    {
      var connectionSettings = Integrationses.GetAll(s => s.Code == avis.Integration.Constants.Module.ContractorsImportRecordCode).FirstOrDefault();
      if (connectionSettings == null)
        throw AppliedCodeException.Create("Не найдена настройка интеграции для загрузки КА из Д5.");
      
      var connectionString = Encryption.Decrypt(connectionSettings.ConnectionParams);
      if (string.IsNullOrEmpty(connectionString))
        throw AppliedCodeException.Create("В настройке интеграции для выгрузки КА не указана строка подключения к БД.");
      
      return connectionString;
    }
    
    /// <summary>
    /// Обработать модели КА из БД
    /// </summary>
    /// <param name="models">Список моделей</param>
    /// <returns>Список результатов обработки по каждой модели</returns>
    private static List<AvisIntegrationHelper.MigrationResult> ProcessingCounterpartyModels(List<AvisIntegrationHelper.Counterparty> models)
    {
      if (models == null || !models.Any())
        throw AppliedCodeException.Create("Список моделей КА из Д5 пуст.");
      
      var results = new List<AvisIntegrationHelper.MigrationResult>();
      foreach (var model in models)
      {
        var result = TryProcessingCounterpartyModel(model);
        results.Add(result);
      }
      
      return results;
    }
    
    /// <summary>
    /// Обработать модель КА из БД
    /// </summary>
    /// <param name="model">Модель</param>
    /// <returns>Результат обрбаотки</returns>
    private static AvisIntegrationHelper.MigrationResult TryProcessingCounterpartyModel(AvisIntegrationHelper.Counterparty model)
    {
      var started = Sungero.Core.Calendar.Now;
      
      try
      {
        var company = GetProcessedCounterparty(model);
        FillCounterpartyData(model, company);
        
        var message = CheckEmptyFields(model);
        return string.IsNullOrEmpty(message) ?
          AvisIntegrationHelper.MigrationResult.GetSuccessResult(int.Parse(model.ID), started) :
          AvisIntegrationHelper.MigrationResult.GetDefectedResult(int.Parse(model.ID), started, message);
      }
      catch (AppliedCodeException ex)
      {
        return AvisIntegrationHelper.MigrationResult.GetErrorResult(int.Parse(model.ID), started, ex.Message);
      }
      catch (Exception ex)
      {
        var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
        Logger.ErrorFormat("Загрузка контрагентов из Д5. Ошибка обработки записи с KodD5 = {0}. {1}. {2}", model.KodD5, ex.Message, innerMessage);
        return AvisIntegrationHelper.MigrationResult.GetErrorResult(int.Parse(model.ID), started, "Внутренняя ошибка сервера при обработке записи.");
      }
    }
    
    /// <summary>
    /// Получить обрабатываемого КА из RX соответствующего модели
    /// </summary>
    /// <param name="model">Модель</param>
    /// <returns>Организация</returns>
    /// <remarks>Создается новая организация, если КА не найден в RX</remarks>
    private static lenspec.Etalon.ICompany GetProcessedCounterparty(AvisIntegrationHelper.Counterparty model)
    {
      var company = GetCounterpartyByExternalCode(model);
      if (company == null)
        company = GetCounterpartyByTinTrrc(model);
      
      return company != null ? company : lenspec.Etalon.Companies.Create();
    }
    
    /// <summary>
    /// Найти КА по Коду Директум 5
    /// </summary>
    /// <param name="model">Модель</param>
    /// <returns>Организация</returns>
    private static lenspec.Etalon.ICompany GetCounterpartyByExternalCode(AvisIntegrationHelper.Counterparty model)
    {
      var company = lenspec.Etalon.Companies.Null;
      
      var matchCompanies = lenspec.Etalon.Companies.GetAll(x => x.ExternalCodeavis == model.KodD5).ToList();
      if (matchCompanies.Count > 1)
      {
        var matchByTinTrrc = matchCompanies.Where(x => model.INN == x.TIN && model.KPP == x.TRRC).ToList();
        if (!matchByTinTrrc.Any())
           throw AppliedCodeException.Create("В RX найдено несколько организаций с таким KodD5, но ни одна не совпадает по ИНН/КПП");
        else if (matchByTinTrrc.Count > 1)
        {
          matchByTinTrrc = matchByTinTrrc.Where(x => x.Status == lenspec.Etalon.Company.Status.Active).ToList();
          if (matchByTinTrrc.Count > 1)
            throw AppliedCodeException.Create("В RX найдено несколько действующих организаций с таким KodD5 и ИНН/КПП");
          else if (!matchByTinTrrc.Any())
            throw AppliedCodeException.Create("В RX найдено несколько организаций с таким KodD5 и ИНН/КПП и все закрытые");
          else
            company = matchByTinTrrc.Single();
        }
        else
          company = matchByTinTrrc.Single();
      }
      else if (matchCompanies.Count == 1)
      {
        company = matchCompanies.Single();
        var isTrrcEmpty = string.IsNullOrEmpty(company.TRRC) && string.IsNullOrEmpty(model.KPP);
        if (company.TIN != model.INN || (!isTrrcEmpty && company.TRRC != model.KPP))
          throw AppliedCodeException.Create("ИНН / КПП в RX не совпадают с ИНН / КПП в Directum 5.");
      }
      
      return company;
    }
    
    /// <summary>
    /// Найти КА по ИНН / КПП
    /// </summary>
    /// <param name="model">Модель</param>
    /// <returns>Организация</returns>
    private static lenspec.Etalon.ICompany GetCounterpartyByTinTrrc(AvisIntegrationHelper.Counterparty model)
    {
      if (string.IsNullOrEmpty(model.INN))
        throw AppliedCodeException.Create("В таблице не заполнено поле INN");
      
      var isLegalEntity = model.INN.Length == 10;
      if (isLegalEntity && string.IsNullOrEmpty(model.KPP))
        throw AppliedCodeException.Create("INN = 10 символов, но KPP в таблице не заполнено.");
      
      var matchCompanies = isLegalEntity ? lenspec.Etalon.Companies.GetAll(x => model.INN == x.TIN && model.KPP == x.TRRC).ToList() : lenspec.Etalon.Companies.GetAll(x => model.INN == x.TIN).ToList();
      if (matchCompanies.Count == 1)
        return matchCompanies.Single();
      if (matchCompanies.Count == 0)
        return null;
      
      matchCompanies = matchCompanies.Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToList();
      if (matchCompanies.Count != 1)
        throw AppliedCodeException.Create("Найдено несколько записей с одинаковыми ИНН, КПП.");
      else
        return matchCompanies.Single();
    }
    
    /// <summary>
    /// Заполнить данные найденном или созданном КА
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">КА</param>
    private static void FillCounterpartyData(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      if (company.State.IsInserted)
        FillCounterpartyAllData(model, company);
      else
        FillCounterpartyApprovalData(model, company);
      
      company.Save();
    }
    
    /// <summary>
    /// Заполнить группу полей Согласование ДБ
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">КА</param>
    private static void FillCounterpartyApprovalData(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      if ((company.ApprovalPeriodavis == null || !company.ApprovalPeriodavis.HasValue) && 
          (company.ResultApprovalDEBavis != lenspec.Etalon.Company.ResultApprovalDEBavis.DoesNotReqAppr && company.ResultApprovalDEBavis != lenspec.Etalon.Company.ResultApprovalDEBavis.CoopNotRecomend))
      {
        FillCounterpartyApprovalResult(model, company);
        FillCounterpartyResponsibleApprover(model, company);
        var inspectionDate = new DateTime();
        var approvalDeadline = new DateTime();
        if (Sungero.Core.Calendar.TryParseDate(model.DateOfApproval, out inspectionDate))
          company.InspectionDateDEBavis = inspectionDate;
        if (Sungero.Core.Calendar.TryParseDate(model.ClosingDateForApproval, out approvalDeadline))
          company.ApprovalPeriodavis = approvalDeadline;
      }
    }
    
    /// <summary>
    /// Заполнить все поля в соответствии с моделью
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">Организация</param>
    private static void FillCounterpartyAllData(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      company.ExternalCodeavis = model.KodD5;
      company.TIN = model.INN;
      company.TRRC = model.KPP;
      company.PSRN = model.OGRN;
      company.NCEO = model.OKPO;
      company.LegalAddress = model.LegalAddress;
      company.PostalAddress = model.PostalAddress;
      company.Phones = model.Phone;
      company.Email = model.Email;
      company.Homepage = model.Website;
      company.Note = model.Note;
      FillCounterpartyName(model, company);
      FillCounterpartyStatus(model, company);
      FillCounterpartyGroup(model, company);
      FillCounterpartyRegion(model, company);
      FillHeadCompanyCounterparty(model, company);
      FillCounterpartyApprovalData(model, company);
    }
    
    /// <summary>
    /// Заполнить наименование КА
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">КА</param>
    private static void FillCounterpartyName(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      if (string.IsNullOrEmpty(model.Name) && string.IsNullOrEmpty(model.LegalName))
        throw AppliedCodeException.Create("Не заполнены поля и Name и LegalName");
      
      if (string.IsNullOrEmpty(model.Name))
      {
        company.Name = model.LegalName.Length > company.Info.Properties.Name.Length ? model.LegalName.Substring(0, company.Info.Properties.Name.Length - 2) : model.LegalName;
        company.LegalName = company.Name;
      }
      else if (string.IsNullOrEmpty(model.LegalName))
      {
        company.Name = model.Name.Length > company.Info.Properties.Name.Length ? model.Name.Substring(0, company.Info.Properties.Name.Length - 2) : model.Name;
        company.LegalName = company.Name;
      }
      else
      {
        company.Name = model.Name.Length > company.Info.Properties.Name.Length ? model.Name.Substring(0, company.Info.Properties.Name.Length - 2) : model.Name;
        company.LegalName = model.LegalName.Length > company.Info.Properties.LegalName.Length ? model.LegalName.Substring(0, company.Info.Properties.Name.Length - 2) : model.LegalName;
      }
    }
    
    /// <summary>
    /// Заполнить состояние КА
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">КА</param>
    private static void FillCounterpartyStatus(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      switch (model.State.Trim())
      {
        case "Д":
          company.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
          break;
        default:
          company.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
          break;
      }
    }
    
    /// <summary>
    /// Заполнить группу и связанную категорию КА
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">КА</param>
    private static void FillCounterpartyGroup(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      if (string.IsNullOrEmpty(model.CounterpartyGroup))
        return;
      
      switch (model.CounterpartyGroup.Trim())
      {
        case "Физические лица/Индивидуальные предприниматели":
          company.GroupCounterpartyavis = avis.EtalonParties.PublicFunctions.GroupCounterparty.GetByIdDirectum5(17896408); // ИП. Нет константы (сверять с инициализацией EtalonParties)
          company.CategoryCounterpartyavis = avis.EtalonParties.PublicFunctions.CategoryCounterparty.GetByIdDirectum5(17896527); // ИП. Нет константы (сверять с инициализацией EtalonParties)
          break;
        default:
          company.GroupCounterpartyavis = avis.EtalonParties.PublicFunctions.GroupCounterparty.GetByIdDirectum5(17896403); // ЮЛ. Нет константы (сверять с инициализацией EtalonParties)
          company.CategoryCounterpartyavis = avis.EtalonParties.PublicFunctions.CategoryCounterparty.GetByIdDirectum5(17896511); // Ком орг. Нет константы (сверять с инициализацией EtalonParties)
          break;
      }
    }
    
    /// <summary>
    /// Заполнить регион КА
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">КА</param>
    private static void FillCounterpartyRegion(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      var regionId = default(long);
      if (long.TryParse(model.Region, out regionId))
        company.Region = Sungero.Commons.Regions.GetAll(x => x.Id == regionId).SingleOrDefault(); // Если использовать Get(id), исключение прервет текущую и запустит следующую итерацию обработки моделей
    }
    
    /// <summary>
    /// Заполнить головную организацию
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">КА</param>
    private static void FillHeadCompanyCounterparty(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      if (!string.IsNullOrEmpty(model.ParentOrg_KodD5))
      {
        var headCompanyId = default(long);
        if (long.TryParse(model.ParentOrg_KodD5, out headCompanyId))
          company.HeadCompany = Sungero.Parties.Companies.GetAll(x => x.Id == headCompanyId).SingleOrDefault(); // Если использовать Get(id), исключение прервет текущую и запустит следующую итерацию обработки моделей
      }
      else
      {
        var headOrgNoteRow = string.Format("Головная организация - ИНН:{0} КПП:{1}.",
                                           string.IsNullOrEmpty(model.ParentOrg_INN) ? "Не указано" : model.ParentOrg_INN,
                                           string.IsNullOrEmpty(model.ParentOrg_KPP) ? "Не указано" : model.ParentOrg_KPP);
        company.Note += string.IsNullOrWhiteSpace(company.Note) ? headOrgNoteRow : System.Environment.NewLine + headOrgNoteRow;
      }
    }
    
    /// <summary>
    /// Заполнить чекбокс Агент продаж в КА
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">Организация</param>
    private static void FillSalesAgentCounterparty(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      if (string.IsNullOrEmpty(model.SalesAgent))
        return;
      company.SalesAgentlenspec = model.SalesAgent == bool.TrueString;
    }

    /// <summary>
    /// Заполнить результат согласования ДБ в КА
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">КА</param>
    private static void FillCounterpartyApprovalResult(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      switch (model.SS_Status.Trim())
      {
        case "Согласован СБ":
          company.ResultApprovalDEBavis = lenspec.Etalon.Company.ResultApprovalDEBavis.CoopPossible;
          break;
        case "Не согласован СБ":
          company.ResultApprovalDEBavis = lenspec.Etalon.Company.ResultApprovalDEBavis.CoopNotRecomend;
          break;
        case "Не проходил согласование в СБ":
          company.ResultApprovalDEBavis = lenspec.Etalon.Company.ResultApprovalDEBavis.NotAssessed;
          break;
      }
    }
    
    /// <summary>
    /// Заполнить ответственного от ДБ в КА
    /// </summary>
    /// <param name="model">Модель</param>
    /// <param name="company">КА</param>
    private static void FillCounterpartyResponsibleApprover(AvisIntegrationHelper.Counterparty model, lenspec.Etalon.ICompany company)
    {
      if (!string.IsNullOrEmpty(model.Kod1C))
      {
        var matchEmployees = lenspec.Etalon.Employees.GetAll(x => x.ExternalCodeavis == model.Kod1C).ToList();
        if (matchEmployees.Count > 1)
          throw AppliedCodeException.Create($"В RX найдено более 1 сотрудника с Кодом 1С = {model.Kod1C}");
        
        if (matchEmployees.Count == 1)
        {
          company.ResponsibleDEBavis = matchEmployees.Single();
          return;
        }
      }
      
      var serviceUserIdConst = EtalonDatabooks.ConstantDatabooks.GetAll(x => x.Code == EtalonDatabooks.PublicConstants.ConstantDatabook.TenderDocsMigrationUserId).SingleOrDefault();
      var serviceUserId = default(long);
      if (!long.TryParse(serviceUserIdConst.Value, out serviceUserId))
        throw AppliedCodeException.Create("Не найдено значение константы 'ИД сотрудника Миграция тендерных документов'");
      
      var authorNoteRow = string.Format("Ответственный от ДБ - {0}", model.SS_User ?? "Не указан");
      company.Note += string.IsNullOrWhiteSpace(company.Note) ? authorNoteRow : System.Environment.NewLine + authorNoteRow;
      company.ResponsibleDEBavis = lenspec.Etalon.Employees.Get(serviceUserId);
      
    }
    
    /// <summary>
    /// Проверить на пустоту ключевые поля модели, которые не вызывают исключения при обработке
    /// </summary>
    /// <param name="model">Модель</param>
    /// <returns>Строка с ошибкой</returns>
    private static string CheckEmptyFields(AvisIntegrationHelper.Counterparty model)
    {
      var fieldsToCheck = new System.Collections.Generic.Dictionary<string, bool>
      {
        { "SS_Status", string.IsNullOrEmpty(model.SS_Status) },
        { "SS_User", string.IsNullOrEmpty(model.SS_User) },
        { "DateOfApproval", string.IsNullOrEmpty(model.DateOfApproval) },
        { "ClosingDateForApproval", string.IsNullOrEmpty(model.ClosingDateForApproval) },
        { "Region", string.IsNullOrEmpty(model.Region) },
        { "Phone", string.IsNullOrEmpty(model.Phone) },
        { "Email", string.IsNullOrEmpty(model.Email) },
        { "Website", string.IsNullOrEmpty(model.Website) }
      };
      
      var emptyFields = fieldsToCheck.Where(f => f.Value).Select(f => f.Key).ToList();

      if (!emptyFields.Any())
        return string.Empty;

      return GenerateEmptyFieldsErrorMessage(emptyFields);
    }
    
    private static string GenerateEmptyFieldsErrorMessage(List<string> emptyFields)
    {
      for (var i = 0; i < emptyFields.Count; i++)
      {
        switch (emptyFields[i])
        {
          case "SS_Status":
            emptyFields[i] = "Статус согласования службой безопасности";
            break;
          case "SS_User":
            emptyFields[i] = "Сотрудник СБ, согласовавший контрагента";
            break;
          case "DateOfApproval":
            emptyFields[i] = "Дата и время согласования";
            break;
          case "ClosingDateForApproval":
            emptyFields[i] = "Срок окончания согласования";
            break;
          case "Region":
            emptyFields[i] = "Регион";
            break;
          case "Phone":
            emptyFields[i] = "Телефоны";
            break;
          case "Website":
            emptyFields[i] = "Сайт";
            break;
        }
      }
      return string.Format("Нет информации в Директум 5 по {0}", string.Join(", ", emptyFields));
    }
    
    #endregion
    
    #region Заявки на оплату
    #region Выгрузка Заявок на оплату
    /// <summary>
    /// Выгрузка Заявок на оплату в Integra
    /// </summary>
    [Public]
    public void UnloadingPaymentRequest(int action, string stOrgCode, string contractorID, string requestNo, DateTime? date, DateTime modTime,
                                        long? iDMainContract, decimal sum, decimal? taxGroup, string currency, decimal taxSum, long payReqID, string author, string recipient,
                                        int prePay, string reqName, string subject, DateTime? planPayDate, int targetSpending, string exportActionTitle, int exportTaskID,
                                        string region, string directorateDivision, string departmentDivision, int requestType, DateTime? dateActualPayment, string paymentStatus,
                                        decimal actualPaymentAmount, string decodingCashItems, string caseNumber, string basisPayment, string customerAgreementNumber,
                                        string resolutionNumber, string win, string contractorINN, string contractorKPP, string recipientINN, string recipientKPP, string rx,
                                        string note_Text, DateTime? earlyPayment_Date, DateTime? contractPayment_Date, decimal totalAmount_WithDiscount, long contractorRXID,
                                        string recipientID, long recipientRXID, string contractorRS, string contractorBIK, string contractorKS, string contractorBank,
                                        string recipientRS, string recipientBIK, string recipientKS, string recipientBank, long iDMainContractRX, string departmentRegion,
                                        string fam, string name, string otch, string clientINN, string famTP, string nameTP, string otchTP, string clientINNTP)
    {
      var settings = Integrationses.GetAll(s => s.Code== lenspec.Etalon.Module.Integration.PublicConstants.Module.KBKCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      // инициализируем helper для получения данных из интеграционной базы
      string connectionString = Encryption.Decrypt(settings.ConnectionParams);
      
      AvisIntegrationHelper.DataBaseHelper.UnloadingPaymentRequest(connectionString, action, stOrgCode, contractorID, requestNo, date, modTime,
                                                                   iDMainContract, sum, taxGroup, currency, taxSum, payReqID, author, recipient,
                                                                   prePay, reqName, subject, planPayDate, targetSpending, exportActionTitle, exportTaskID,
                                                                   region, directorateDivision, departmentDivision, requestType, dateActualPayment, paymentStatus,
                                                                   actualPaymentAmount, decodingCashItems, caseNumber, basisPayment, customerAgreementNumber,
                                                                   resolutionNumber, win, contractorINN, contractorKPP, recipientINN, recipientKPP, rx,
                                                                   note_Text, earlyPayment_Date, contractPayment_Date, totalAmount_WithDiscount, contractorRXID,
                                                                   recipientID, recipientRXID, contractorRS, contractorBIK, contractorKS, contractorBank,
                                                                   recipientRS, recipientBIK, recipientKS, recipientBank, iDMainContractRX, departmentRegion,
                                                                   fam, name, otch, clientINN, famTP, nameTP, otchTP, clientINNTP);
    }
    #endregion
    
    #region Выгрузка Заявок на оплату(Детали)
    /// <summary>
    /// Выгрузка Заявок на оплату(Детали) в Integra
    /// </summary>
    [Public]
    public void UnloadingPaymentRequestDetail(string contrSites, string workType, long payReqID, string stOrgCode, decimal sum,DateTime? modTime,
                                              decimal taxGroupOS, decimal taxSumOS, int headerID, string kbk, long numStr, string rx)
    {
      var settings = Integrationses.GetAll(s => s.Code== lenspec.Etalon.Module.Integration.PublicConstants.Module.KBKCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      // инициализируем helper для получения данных из интеграционной базы
      string connectionString = Encryption.Decrypt(settings.ConnectionParams);
      
      AvisIntegrationHelper.DataBaseHelper.UnloadingPaymentRequestDetail(connectionString, contrSites, workType, payReqID, stOrgCode,
                                                                         sum, modTime, taxGroupOS, taxSumOS, headerID, kbk, numStr, rx);
    }
    #endregion
    
    #region Выгрузка Заявок на оплату(Детали получение значения полу HeaderID)
    /// <summary>
    /// Выгрузка Заявок на оплату(Детали получение значения полу HeaderID) в Integra
    /// </summary>
    [Public]
    public static int UnloadingPaymentRequestDetailHeaderID(long payReqID)
    {
      var settings = Integrationses.GetAll(s => s.Code== lenspec.Etalon.Module.Integration.PublicConstants.Module.KBKCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      // инициализируем helper для получения данных из интеграционной базы
      string connectionString = Encryption.Decrypt(settings.ConnectionParams);
      int headerID = AvisIntegrationHelper.DataBaseHelper.UnloadingPaymentRequestDetailHeaderID(connectionString, payReqID);
      return headerID;
    }
    #endregion
    
    #region Выгрузка Заявок на оплату(Договор)
    /// <summary>
    /// Выгрузка Заявок на оплату(Договор) в Integra
    /// </summary>
    [Public]
    public void UnloadingPaymentRequestContract(long contractRXID, long? contractID, string stOrgCode, string contractorID, string contractNo,
                                                DateTime? date, DateTime modTime, string subjContract, long idMainContract, DateTime? startDate,
                                                DateTime? expDate, int? contrType, decimal sum, decimal taxGroupC, string currency, decimal taxSum, int isp, int smr,
                                                string author, string region, string fiasDistrictCode, string nameDocumentTypeGroup, string nameDocumentType,
                                                string category, int action, int phys, string rx, long contractorRXID)
    {
      var settings = Integrationses.GetAll(s => s.Code== lenspec.Etalon.Module.Integration.PublicConstants.Module.KBKCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      // инициализируем helper для получения данных из интеграционной базы
      string connectionString = Encryption.Decrypt(settings.ConnectionParams);
      long idMainContractRX = 0;
      
      AvisIntegrationHelper.DataBaseHelper.UnloadingContracts(connectionString, contractRXID, contractID, stOrgCode, contractorID,
                                                              contractNo, date, modTime, subjContract, idMainContract, startDate,
                                                              expDate, contrType, sum, taxGroupC, currency, taxSum, isp, smr,
                                                              author, region, fiasDistrictCode, nameDocumentTypeGroup, nameDocumentType,
                                                              category, action, phys, contractorRXID, idMainContractRX, rx);
    }
    #endregion
    #endregion

    #region Unloading Contracts
    /// <summary>
    /// Выгрузка договорного документа в Integra
    /// </summary>
    [Public]
    public void UnloadingContractsIntegrationHelper(long contractRXID, long? contractID, string stOrgCode, string contractorID,
                                                    string contractNo, DateTime? date, DateTime modTime, string subjContract, long idMainContract, DateTime? startDate,
                                                    DateTime? expDate, int? contrType, decimal sum, decimal taxGroup, string currency, decimal taxSum, int isp, int smr,
                                                    string author, string region, string fiasDistrictCode, string nameDocumentTypeGroup, string nameDocumentType,
                                                    string category, int action, int phys, long contractorRXID, long idMainContractRX, string rx)
    {
      var settings = Integrationses.GetAll(s => s.Code== lenspec.Etalon.Module.Integration.PublicConstants.Module.KBKCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      // инициализируем helper для получения данных из интеграционной базы
      string connectionString = Encryption.Decrypt(settings.ConnectionParams);
      
      var errorMessage = AvisIntegrationHelper.DataBaseHelper.UnloadingContracts(connectionString, contractRXID, contractID, stOrgCode, contractorID, contractNo, date, modTime, subjContract, idMainContract, startDate,
                                                              expDate, contrType, sum, taxGroup, currency, taxSum, isp, smr, author, region, fiasDistrictCode, nameDocumentTypeGroup,
                                                              nameDocumentType, category, action, phys, contractorRXID, idMainContractRX, rx);
      if (!string.IsNullOrEmpty(errorMessage))
        Logger.Error($"Avis - UnloadingContracts. Выгрузка ДД с ИД {contractRXID}. Ошибка выполнения в dll - {errorMessage}.");
    }
    #endregion
    
    #region Unloading Contracts OS
    /// <summary>
    /// Выгрузка договорного документа в Integra
    /// </summary>
    [Public]
    public bool UnloadingContractsOSIntegrationHelper(long contractRXID, long? contractID, string stOrgCode, string contrSites, string workType,
                                                      float sum, int? xrecID, int? xrecIdFrom1C, DateTime modTime, string rx)
    {
      var settings = Integrationses.GetAll(s => s.Code== lenspec.Etalon.Module.Integration.PublicConstants.Module.KBKCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      // инициализируем helper для получения данных из интеграционной базы
      string connectionString = Encryption.Decrypt(settings.ConnectionParams);
      
      bool check = AvisIntegrationHelper.DataBaseHelper.UnloadingContractsDetails(connectionString, contractRXID, contractID, stOrgCode,
                                                                                  contrSites, workType, sum,
                                                                                  xrecID, xrecIdFrom1C, modTime, rx);
      return check;
    }
    #endregion
    
    #region Dadata.
    /// <summary>
    /// Заполнить реквизиты банка из DaData.
    /// </summary>
    /// <param name="bank">Банк.</param>
    /// <param name="token">Токен.</param>
    /// <returns>Текст ошибки или null.</returns>
    [Public]
    public string FillBankRequisitesFromDadata(lenspec.Etalon.IBank bank, string token)
    {
      if (string.IsNullOrEmpty(bank.BIC) && string.IsNullOrEmpty(bank.TIN) && string.IsNullOrEmpty(bank.TRRC))
        return "Заполните информацию об ИНН и КПП.";
      
      var api = new SuggestClientAsync(token);
      var response = !string.IsNullOrEmpty(bank.BIC)
        ? api.FindBank(bank.BIC)
        : api.FindBank(new FindBankRequest(query: bank.TIN, kpp: bank.TRRC));
      
      // Обработка ошибок 401, 403.
      if (response.Exception != null && !string.IsNullOrEmpty(response.Exception.Message))
      {
        if (response.Exception.Message.Contains("401"))
          return "Запрос не выполнен. Обратитесь к Администратору системы. В запросе отсутствует API-ключ.";
        
        if (response.Exception.Message.Contains("403"))
          return "Запрос не выполнен. Обратитесь к Администратору системы. В запросе указан неправильный API-ключ или исчерпан лимит запросов на день.";
        
        return response.Exception.Message;
      }
      
      // БИК, ИНН, КПП в базе Dadata не найден.
      if (response.Result.suggestions.Count() == 0)
        return "По заданным БИК, ИНН, КПП банк в базе Dadata не найден.";
      
      var data = response.Result.suggestions[0].data;
      if (data != null)
      {
        if (response.Result.suggestions[0].data.state.status == PartyStatus.LIQUIDATED)
          return "Банк ликвидирован.";
        
        var emptyFields = new List<string>();
        
        if (!string.IsNullOrEmpty(data.inn))
          bank.TIN = data.inn;
        else
          emptyFields.Add("ИНН");
        
        if (!string.IsNullOrEmpty(data.kpp))
          bank.TRRC = data.kpp;
        else
          emptyFields.Add("КПП");
        
        if (!string.IsNullOrEmpty(data.bic))
          bank.BIC = data.bic;
        else
          emptyFields.Add("БИК");
        
        if (!string.IsNullOrEmpty(data.swift))
          bank.SWIFT = data.swift;
        else
          emptyFields.Add("SWIFT");
        
        if (!string.IsNullOrEmpty(data.correspondent_account))
          bank.CorrespondentAccount = data.correspondent_account;
        else
          emptyFields.Add("Корр. счет");
        
        if (emptyFields.Any())
        {
          return string.Format("Не заполнены реквизиты: {0} в базе Dadata не найдены.", string.Join(", ", emptyFields));
        }
      }
      return null;
    }
    
    /// <summary>
    /// Заполнить реквизиты банка из DaData.
    /// </summary>
    /// <param name="bank">Банк.</param>
    /// <returns>Текст ошибки или null.</returns>
    [Public, Remote]
    public string FillBankRequisitesFromDadata(lenspec.Etalon.IBank bank)
    {
      // Получаем токены.
      var settings = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.DadataCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        return "Не найдены настройки подключения.";
      
      // Расшифровываем, и разделяем на токен и секретный ключ.
      var settingsString = Encryption.Decrypt(settings.ConnectionParams);
      var connectionString = settingsString.Split(';');
      if (string.IsNullOrEmpty(connectionString[0]))
        return "Неверный токен.";
      
      return FillBankRequisitesFromDadata(bank, connectionString[0]);
    }
    
    /// <summary>
    /// Генерирует правильное название региона как в директуме.
    /// </summary>
    /// <param name="typeRegion">Тип региона (область/республика и тд).</param>
    /// <param name="topoValue">Название региона.</param>
    /// <returns></returns>
    private string GetRegionName(string typeRegion, string topoValue)
    {
      // Список исключений.
      string[] exceptionNames = {
        "Москва",
        "Санкт-Петербург",
        "Севастополь",
        "Ханты-Мансийский автономный округ - Югра",
        "Кемеровская область - Кузбасс"};
      
      // Проверка входит ли регион в список исключения.
      if (exceptionNames.Where(en => en == topoValue).FirstOrDefault() != null)
      {
        return topoValue;
      }
      
      // Проверяем республики.
      if (typeRegion == "республика")
      {
        typeRegion.Replace('р', 'Р');
        return $"{typeRegion} {topoValue}";
      }
      
      // В любом другом случае добавляем тип региона в конец.
      return $"{topoValue} {typeRegion}";
    }
    
    /// <summary>
    /// Сформировать улицу адрес компании.
    /// </summary>
    /// <param name="dadataAddress">Адресс полученный из дадата.</param>
    /// <returns>Улица компании.</returns>
    private string GetStreet(Dadata.Model.Address dadataAddress)
    {
      var address = "";
      
      // Заполняем улицу.
      if (dadataAddress.street != null)
        address += $"{dadataAddress.street_type}. {dadataAddress.street}";
      
      if (string.IsNullOrEmpty(address) && dadataAddress.settlement != null)
        address += $"{dadataAddress.settlement_type}. {dadataAddress.settlement}";
      
      // Заполняем номер дома.
      if (dadataAddress.house != null)
        address += $", {dadataAddress.house_type}. {dadataAddress.house}";
      
      // Заполняем строение.
      if (dadataAddress.block != null)
        address += $", {dadataAddress.block_type}. {dadataAddress.block}";
      
      // Заполняем номер помещения.
      if (dadataAddress.flat != null)
        address += $", {dadataAddress.flat_type}. {dadataAddress.flat}";
      
      return address;
    }
    
    /// <summary>
    /// Сформировать название города.
    /// </summary>
    /// <param name="dadataAddress"></param>
    /// <returns></returns>
    private string GetCityName(Dadata.Model.Address dadataAddress)
    {
      // Формируем имя населенного пункта, в зависимости от присланных значений. Если не город не поселок не пришли, значит город регионального значения указываем в имя города регион.
      var cityName = "";
      
      if (!string.IsNullOrEmpty(dadataAddress.settlement))
        cityName = $"{dadataAddress.settlement_type}. {dadataAddress.settlement}";
      
      if (!string.IsNullOrEmpty(dadataAddress.city))
        cityName = $"{dadataAddress.city_type}. {dadataAddress.city}";
      
      if (!string.IsNullOrEmpty(dadataAddress.area))
        cityName =$"{dadataAddress.area_type}. {dadataAddress.area}";
      
      if (string.IsNullOrEmpty(cityName))
        cityName = $"{dadataAddress.region_type}. {dadataAddress.region}";
      
      return cityName;
    }
    
    /// <summary>
    /// Сформировать почтовый адрес компании.
    /// </summary>
    /// <param name="dadataAddress">Адресс полученный из дадата.</param>
    /// <returns>Почтовый адресс компании.</returns>
    private string GetPostalAddress(Dadata.Model.Address dadataAddress)
    {
      var postalAddress = "";
      
      if (dadataAddress.postal_code != null)
        postalAddress = $"{dadataAddress.postal_code}";
      
      var region = "";
      if (dadataAddress.region != null)
      {
        region = $"{dadataAddress.region_type}. {dadataAddress.region}";
        postalAddress += $", {region}";
      }
      
      var cityName = GetCityName(dadataAddress);
      if (!string.IsNullOrEmpty(cityName) && cityName != region)
        postalAddress += $", {cityName}";
      
      var address = GetStreet(dadataAddress);
      if (!string.IsNullOrEmpty(address))
        postalAddress += $", {address}";
      
      if (!string.IsNullOrEmpty(dadataAddress.unparsed_parts))
        postalAddress += $" {dadataAddress.unparsed_parts}";
      
      return postalAddress;
    }
    
    // Заполняем компанию из дадаты.
    /// <summary>
    /// Заполняем компанию\банк из дадаты.
    /// </summary>
    /// <param name="companyBase">Компания или банк</param>
    /// <returns>Текст ошибки или null.</returns>
    [Public, Remote]
    public string FillCompanyFromDadata(lenspec.Etalon.ICompanyBase companyBase)
    {
      // Получаем токены.
      var settings = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.DadataCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        return "Не найдены настройки подключения";
      
      // Расшифровываем, и разделяем на токен и секретный ключ.
      var settingsString = Encryption.Decrypt(settings.ConnectionParams);
      var connectionString = settingsString.Split(';');
      if (string.IsNullOrEmpty(connectionString[0]))
        return "Не правильная строка токен.";
      
      if (string.IsNullOrEmpty(companyBase.PostalAddress))
        return "Заполните поле 'Почтовый адрес'";
      
      var token = connectionString[0];
      var secret = connectionString[1];
      var api = new CleanClientAsync(token, secret);
      var result = api.Clean<Address>(companyBase.PostalAddress);
      
      // Формируем имя региона.
      var regionName = GetRegionName(result.Result.region_type_full, result.Result.region);
      
      // Получить название города.
      var cityName = GetCityName(result.Result);

      // Заполняем данные
      companyBase.Indexavis = result.Result.postal_code;
      // Заполняем улицу.
      companyBase.Streetavis = GetStreet(result.Result);
      // Заполняем почтовый адрес.
      companyBase.PostalAddress = GetPostalAddress(result.Result);
      
      return null;
    }
    
    #endregion
    
    #region Инвест.
    
    /// <summary>
    /// Интеграция Отправка сообщения
    /// </summary>
    public string SendMessageData(lenspec.Etalon.Module.Integration.Structures.Module.PackageMsg pkg)
    {
      var persona = Sungero.Company.Employees.GetAll(x => pkg.Body.message_id == x.Id.ToString() && x.Status == Sungero.Company.Employee.Status.Active).FirstOrDefault();
      if (persona == null)
        return "Нет сотрудника с ИД=" + pkg.Body.message_id;
      
      var obct =  lenspec.EtalonDatabooks.ObjectAnProjects.GetAll(x => x.IdInvest == pkg.Body.drxobj_id).FirstOrDefault();
      if (obct == null)
        return "Нет объекта с ИД=" + pkg.Body.drxobj_id;
      
      var noticeList = new List<IRecipient>();
      noticeList.Add(Sungero.CoreEntities.Recipients.As(persona));
      
      var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices($"Оповещение об обновлении данных объекта {obct.Name}" , noticeList.ToArray());  //  Subject only 255
      notice.ActiveText =  "Данные по помещениям объекта " + obct.Name  + " из Инвест обновлены. Количество обновленных помещений = " + pkg.Body.kv_amount.ToString();
      
      try
      {
        notice.Start();
        return pkg.Body.message_id;
      }
      catch (Exception ex)
      {
        return "Не удалось отправить сообщение " + ex.Message;
      }
    }
    
    /// <summary>
    /// Интеграция ДОГОВОРА SDA
    /// </summary>
    public string UpdateContractData(lenspec.Etalon.Module.Integration.Structures.Module.PackageCtr pkg)
    {
      var isForcedLocked = false;
      var contract = SalesDepartmentArchive.SDAClientContracts.GetAll(x => x.InvestContractCode == pkg.Body.dog_id).FirstOrDefault();
      try
      {
        #region Редактирование существующего договора.
        if (contract != null)
        {
          var lockinfo = Locks.GetLockInfo(contract);
          // Если КД заблокирован другим пользователем, то отправить сохранение в АО.
          if (lockinfo != null && lockinfo.IsLocked)
          {
            Logger.DebugFormat("UpdateContractData - КД ИД = {0} заблокирован пользователем {1}. Карточка документа будет сохранена в АО.", contract.Id, lockinfo.OwnerName);
            var asyncUpdateClientContract = AsyncHandlers.AsyncUpdateClientContractlenspec.Create();
            asyncUpdateClientContract.ClientContractId = contract.Id;
            asyncUpdateClientContract.DogId = pkg.Body.dog_id;
            asyncUpdateClientContract.DogNum = pkg.Body.dog_num;
            asyncUpdateClientContract.DogDT = pkg.Body.dog_dt;
            asyncUpdateClientContract.DrxobjDrxid = pkg.Body.drxobj_drxid;
            asyncUpdateClientContract.DeveloperInn = pkg.Body.developer_inn;
            asyncUpdateClientContract.KvId = pkg.Body.kv_id;
            asyncUpdateClientContract.DpId = string.Join(",", pkg.Body.dp_id);
            asyncUpdateClientContract.EcpFl = pkg.Body.ecp_fl;
            asyncUpdateClientContract.DogStatus = pkg.Body.dog_status;
            asyncUpdateClientContract.AppDt = pkg.Body.app_dt;
            asyncUpdateClientContract.ExecuteAsync();
            return $"Клиентский договор c ИД = {contract.Id} заблокирован другим пользователем. Он будет изменен, когда освободится карточка документа.";
          }
          else
          {
            isForcedLocked = Locks.TryLock(contract);
          }
          if (!isForcedLocked)
          {
            Logger.ErrorFormat("UpdateContractData - не удалось заблокировать КД ИД = {0}.", contract.Id);
            
            var asyncNotification = AsyncHandlers.AsyncAdminEDMSNotificationavis.Create();
            asyncNotification.Subject = $"Ошибка синхронизации с Инвестом: не удалось заблокировать карточку документа.";
            asyncNotification.ActiveText = $"Не удалось заблокировать карточку клиентского договора {contract.Name} (ИД {contract.Id}).";
            asyncNotification.SDAClientContractId = contract.Id;
            asyncNotification.ExecuteAsync();
            
            return "Не удалось сохранить контракт c ID=" + pkg.Body.dog_id + " - не удалось заблокировать карточку документа в Directum RX.";
          }
        }
        #endregion
        
        if (contract == null)
        {
          contract = SalesDepartmentArchive.SDAClientContracts.Create();
          contract.Name = "Новый контракт по договору "  + pkg.Body.dog_num;
        }
        
        contract.InvestContractCode = pkg.Body.dog_id;
        contract.ClientDocumentNumber = pkg.Body.dog_num;
        contract.ClientDocumentDate = DateTime.Parse(pkg.Body.dog_dt);

        // Ищем объект проекта, если нашли добавляем в карточку КД.
        var objectAnProject =  lenspec.EtalonDatabooks.ObjectAnProjects.GetAll(x => x.Id.ToString() == pkg.Body.drxobj_drxid).FirstOrDefault();
        if (objectAnProject != null)
          contract.ObjectAnProject = objectAnProject;
        
        // Если не нашли объект, то проверяем наличие инн и ищем нашу организацию по нему.
        var businessUnit = BusinessUnits.GetAll(b => b.TIN == pkg.Body.developer_inn).FirstOrDefault();
        if (businessUnit == null && objectAnProject == null)
          throw new Exception("Не найден объект, а так же по инн не найдена наша организация.");
        
        // Ищем помещение, если не нашли и есть объект, то создаём новое помещение.
        var objectAnSale = lenspec.EtalonDatabooks.ObjectAnSales.GetAll(x => x.IdInvest == pkg.Body.kv_id).FirstOrDefault();
        if (objectAnSale == null && objectAnProject != null)
        {
          objectAnSale = lenspec.EtalonDatabooks.ObjectAnSales.Create();  //  Заполнить по умолчанию
          objectAnSale.IdInvest = pkg.Body.kv_id;

          var purposeOfPremises = lenspec.EtalonDatabooks.PurposeOfPremiseses.GetAll().FirstOrDefault();
          if (purposeOfPremises != null)
            objectAnSale.PurposeOfPremises = purposeOfPremises;
          
          objectAnSale.ObjectAnProject =  contract.ObjectAnProject;
          objectAnSale.OurCF =  contract.ObjectAnProject.OurCF;
          if (string.IsNullOrWhiteSpace(objectAnSale.Address))
            objectAnSale.Address = "Адрес";
          
          objectAnSale.NumberRoomMail = "Номер";
          objectAnSale.Settlement = lenspec.EtalonDatabooks.ObjectAnSale.Settlement.NotRequired;
          objectAnSale.Name = "Новое помещение из пакета интеграции контрактов SDA";
          objectAnSale.Save();
        }
        
        contract.Premises = objectAnSale;
        
        // Уведомление для Администратора СЭД.
        var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
        if (objectAnSale == null && objectAnProject == null)
        {
          // Создаём и стартуем задачу с уведомлением.
          var asyncNotification = AsyncHandlers.AsyncAdminEDMSNotificationavis.Create();
          asyncNotification.Subject = $"Ошибка синхронизации с Инвестом: в Клиентском договоре не найдены Объект проекта и Помещение объекта.";
          asyncNotification.ActiveText = $"В Клиентском договоре {contract.Name}, объект проекта и помещение объекта не найдены.";
          asyncNotification.SDAClientContractId = contract.Id;
          asyncNotification.ExecuteAsync();
        }
        else if (objectAnSale == null)
        {
          // Создаём и стартуем задачу с уведомлением.
          var asyncNotification = AsyncHandlers.AsyncAdminEDMSNotificationavis.Create();
          asyncNotification.Subject = $"Ошибка синхронизации с Инвестом: в Клиентском договоре не найдено Помещение объекта.";
          asyncNotification.ActiveText = $"В Клиентском договоре {contract.Name}, помещение объекта не найдено. Объект - {objectAnProject.Name}";
          asyncNotification.SDAClientContractId = contract.Id;
          asyncNotification.ExecuteAsync();
        }
        else if (objectAnProject == null)
        {
          // Создаём и стартуем задачу с уведомлением.
          var asyncNotification = AsyncHandlers.AsyncAdminEDMSNotificationavis.Create();
          asyncNotification.Subject = $"Ошибка синхронизации с Инвестом: в Клиентском договоре не найден Объект.";
          asyncNotification.ActiveText = $"В Клиентском договоре {contract.Name}, объект не найден. Помещение - {objectAnSale.Name}";
          asyncNotification.SDAClientContractId = contract.Id;
          asyncNotification.ExecuteAsync();
        }
        else if (objectAnSale.ObjectAnProject != objectAnProject)
        {
          // Создаём и стартуем задачу с уведомлением.
          var asyncNotification = AsyncHandlers.AsyncAdminEDMSNotificationavis.Create();
          asyncNotification.Subject = $"Ошибка синхронизации с Инвестом: в Клиентском договоре Помещение не относится к Объекту.";
          asyncNotification.ActiveText = $"В Клиентском договоре {contract.Name}, помещение {objectAnSale.Name} не относится к объекту {objectAnProject.Name}";
          asyncNotification.SDAClientContractId = contract.Id;
          asyncNotification.ExecuteAsync();
        }
        
        var oldPersList = contract.CounterpartyClient.Select(x => x.ClientItem).ToList();  // Будем пытаться закрывать кого не передали в это раз
        contract.CounterpartyClient.Clear();  // Если каждый раз полный комплект то очистить
        
        foreach (var pers in pkg.Body.dp_id)
        {
          var persona = lenspec.Etalon.People.GetAll().Where(x => x.CodeInvestavis == pers).FirstOrDefault();
          if (persona != null)
          {
            var p = contract.CounterpartyClient.AddNew();
            p.ClientItem = Sungero.Parties.Counterparties.As(persona);
          }
        }
        
        if (pkg.Body.ecp_fl == 1)
          contract.SignedWithAnElectronicSignature = SalesDepartmentArchive.SDAClientContract.SignedWithAnElectronicSignature.Yes;
        else
          contract.SignedWithAnElectronicSignature = SalesDepartmentArchive.SDAClientContract.SignedWithAnElectronicSignature.No;
        
        if (pkg.Body.dog_status == 1 || pkg.Body.dog_status == 2)
          contract.LifeCycleState =  SalesDepartmentArchive.SDAClientContract.LifeCycleState.Active;
        else
          contract.LifeCycleState =  SalesDepartmentArchive.SDAClientContract.LifeCycleState.Obsolete;
        
        bool wasEmptyOwnDate = false;
        if (contract.TransferOfOwnershipDate == null)
          wasEmptyOwnDate = true;
        
        if (!string.IsNullOrWhiteSpace(pkg.Body.app_dt))
          contract.TransferOfOwnershipDate = DateTime.Parse(pkg.Body.app_dt);
        
        // Если не нашли объект проекта, то берем нашу организацию из инн.
        if (contract.ObjectAnProject == null)
          contract.BusinessUnit = businessUnit;
        else
          contract.BusinessUnit = contract.ObjectAnProject.SpecDeveloper;  // НОР из объекта
        contract.Save();
        
        //-------------      1 условие ---------------------
        
        oldPersList = oldPersList.Where(x => !contract.CounterpartyClient.Select(y => y.ClientItem).Contains(x)).ToList();  // остались которые были и не стали
        
        foreach  (var clientToDel in oldPersList)
        {
          var client = lenspec.Etalon.People.As(clientToDel);
          
          if (client.Status == lenspec.Etalon.Person.Status.Closed)
            continue;
          
          var hasContract = false;
          var contractsForPers = SalesDepartmentArchive.SDAClientContracts.GetAll().Where(x => x.CounterpartyClient.Any(y => Equals(y.ClientItem.Id,client.Id))
                                                                                          && x.LifeCycleState ==  SalesDepartmentArchive.SDAClientContract.LifeCycleState.Active).Count();
          if (contractsForPers > 0)
            hasContract = true;
          
          if (!hasContract)
          {
            client.IsClientBuyersavis = false;
            
            if (  client.IsLawyeravis == false &&
                client.IsClientOwnersavis == false &&
                client.IsEmployeeGKavis == false &&
                client.IsOtheravis == false
               )
            {
              client.Status =  lenspec.Etalon.Person.Status.Closed;
            }
            
            client.Save();
          }
        }
        //-------------    2 условие
        
        if (wasEmptyOwnDate == true &&  contract.TransferOfOwnershipDate != null)  //надо проверять ..
        {
          foreach (var clnt in contract.CounterpartyClient)
          {
            var c = lenspec.Etalon.People.As(clnt.ClientItem);
            c.IsClientOwnersavis = true;
            
            // Есть ли КД для данной персоны с пустой датой передачи в собственность
            var contractsAll = SalesDepartmentArchive.SDAClientContracts.GetAll().Where(x => x.LifeCycleState == SalesDepartmentArchive.SDAClientContract.LifeCycleState.Active &&
                                                                                        x.CounterpartyClient.Any(y => Equals(y.ClientItem.Id,c.Id )) &&
                                                                                        x.TransferOfOwnershipDate == null).Count();
            if (contractsAll == 0)
              c.IsClientBuyersavis = false;
            
            c.Save();
          }
        }

        return contract.Id.ToString();
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Avis - UpdateContractData - не удалось внести изменения в КД (dog_id = {0}) ", ex, pkg.Body.dog_id);
        return "Не удалось сохранить контракт c ID=" + pkg.Body.dog_id + " " + ex.Message;
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(contract);
      }
    }
    
    /// <summary>
    /// Интеграция ДАННЫХ О ПОМЕЩЕНИЯХ
    /// </summary>
    public string UpdateFlatData(lenspec.Etalon.Module.Integration.Structures.Module.PackageFlat pkg)
    {
      var flat = lenspec.EtalonDatabooks.ObjectAnSales.GetAll(x => x.IdInvest == pkg.Body.kv_id).FirstOrDefault();
      
      if (flat == null)
        flat = lenspec.EtalonDatabooks.ObjectAnSales.Create();
      
      flat.IdInvest = pkg.Body.kv_id;
      
      var sort = lenspec.EtalonDatabooks.PurposeOfPremiseses.GetAll().Where(x => x.IdInvest == pkg.Body.kvsort_id).FirstOrDefault();
      if (sort != null)
        flat.PurposeOfPremises = sort;
      
      flat.NumberRoomMail = pkg.Body.kv_num;
      
      var obct = lenspec.EtalonDatabooks.ObjectAnProjects.GetAll().Where(x => x.Id.ToString() == pkg.Body.object_id).FirstOrDefault();
      if (obct != null)
      {
        flat.ObjectAnProject = obct;
        flat.OurCF = obct.OurCF;
      }
      else
      {
        return "Объект " + pkg.Body.object_id + " не найден";
      }
      
      flat.Settlement =  lenspec.EtalonDatabooks.ObjectAnSale.Settlement.NotRequired;
      
      /*
      if (pkg.Body.settlement =="0")
      {
        flat.Settlement =  lenspec.EtalonDatabooks.ObjectAnSale.Settlement.NotRequired;
      }
      else if (pkg.Body.settlement =="1")
      {
        flat.Settlement =  lenspec.EtalonDatabooks.ObjectAnSale.Settlement.Required;
      }
      else
      {
        flat.Settlement =  lenspec.EtalonDatabooks.ObjectAnSale.Settlement.Performed;
      }
       */
      
      var style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign ;
      var culture = CultureInfo.CreateSpecificCulture("en-US");
      
      double d = 0.0;
      
      if (Double.TryParse(pkg.Body.kv_pibsqdif,style, culture, out d))
        flat.EditSquere = d;
      
      d = 0.0;
      if (Double.TryParse(pkg.Body.kv_pibcostdif,style, culture, out d))
        flat.EditPrice =  d;
      
      try
      {
        
        flat.Name = "Помещение №" + flat.NumberRoomMail + ", назначение  " + flat.PurposeOfPremises.Name + ", объект " + flat.ObjectAnProject.Name;
        flat.Save();
        return flat.Id.ToString();
      }
      catch (Exception ex)
      {
        return "Не удалось создать/обновить помещение " + pkg.Body.kv_id + " " + ex.Message;
      }
    }
    
    /// <summary>
    /// Интеграция Подтверждение синхронизации объектов проектов
    /// </summary>
    public string UpdateObjData(lenspec.Etalon.Module.Integration.Structures.Module.PackageObj pkg)
    {
      var obct = lenspec.EtalonDatabooks.ObjectAnProjects.GetAll(x => x.Id.ToString() == pkg.Body.object_id).FirstOrDefault();
      
      if (obct == null)
        return "Нет объекта с ИД = " + pkg.Body.object_id;
      
      obct.IdInvest = pkg.Body.drxobj_id;
      
      try
      {
        var lockInfo = Locks.GetLockInfo(obct);
        if (lockInfo.IsLocked || lockInfo.IsLockedByOther)
        {
          // Старт асинхронного процесса пока не разблокируется, хотя можно было бы и просто разблокировать, но не будем - жестоко по отношению к пользователю.
          var asyncRightsHandler = AsyncHandlers.AsyncUpdateObjavis.Create();
          asyncRightsHandler.ObjId = obct.Id;
          asyncRightsHandler.AvisCode = obct.IdInvest;
          asyncRightsHandler.ExecuteAsync();
          return obct.Id.ToString();
        }
        else
        {
          obct.Save();
          return obct.Id.ToString();
        }
      }
      catch (Exception ex)
      {
        return "Не удалось сохранить объект c ID=" + pkg.Body.object_id + " " + ex.Message;
      }
    }
    
    /// <summary>
    /// Интеграция Подтверждение синхронизации проектов
    /// </summary>
    public string UpdateCFData(lenspec.Etalon.Module.Integration.Structures.Module.PackageCF pkg)
    {
      var cf = lenspec.EtalonDatabooks.OurCFs.GetAll(x => x.Id.ToString() == pkg.Body.project_id).FirstOrDefault();
      
      if (cf == null)
        return "Нет проекта с ИД = " + pkg.Body.project_id;
      
      cf.IdInvest = pkg.Body.drxprj_id;
      
      try
      {
        var lockInfo = Locks.GetLockInfo(cf);
        if (lockInfo.IsLocked || lockInfo.IsLockedByOther)
        {
          // Старт асинхронного процесса пока не разблокируется, хотя можно было бы и просто разблокировать, но не будем - жестоко по отношению к пользователю.
          var asyncRightsHandler = AsyncHandlers.AsyncUpdatePrjavis.Create();
          asyncRightsHandler.PrjId = cf.Id;
          asyncRightsHandler.AvisCode = pkg.Body.drxprj_id ;
          asyncRightsHandler.ExecuteAsync();
          return cf.Id.ToString();  // Поди когда нибудь обновиться ..
        }
        else
        {
          cf.Save();
          return cf.Id.ToString();
        }
      }
      catch (Exception ex)
      {
        return "Не удалось сохранить OurCF c ID=" + pkg.Body.project_id + " " + ex.Message;
      }
    }
    
    /// <summary>
    /// Интеграция ТИПОВ ПОМЕЩЕНИЙ
    /// </summary>
    public string UpdateSortData(lenspec.Etalon.Module.Integration.Structures.Module.PackageSort pkg)
    {
      var sort = lenspec.EtalonDatabooks.PurposeOfPremiseses.GetAll(x => x.IdInvest == pkg.Body.kvsort_id).FirstOrDefault();
      
      if (sort == null)
        sort = lenspec.EtalonDatabooks.PurposeOfPremiseses.Create();
      
      sort.Name = pkg.Body.kvsort_title;
      sort.IdInvest = pkg.Body.kvsort_id;
      
      if (pkg.Body.kvsort_actf == 1)
        sort.Status = lenspec.EtalonDatabooks.PurposeOfPremises.Status.Active;
      else
        sort.Status = lenspec.EtalonDatabooks.PurposeOfPremises.Status.Closed;
      
      try
      {
        sort.Save();
        return sort.Id.ToString();
      }
      catch
      {
        return "Не удалось создать тип помещения " + pkg.Body.kvsort_title;
      }
    }
    
    /// <summary>
    /// Интеграция ПЕРСОН
    /// </summary>
    public string UpdatePersonData(lenspec.Etalon.Module.Integration.Structures.Module.Package pkg)
    {
      DateTime db = DateTime.Parse(pkg.Body.birth_dt);
      //Проверим только действующие записи
      var persona = lenspec.Etalon.People.GetAll().Where(x => Equals(pkg.Body.lname,x.LastName) &&
                                                         Equals(pkg.Body.fname,x.FirstName) &&
                                                         Equals(pkg.Body.mname,x.MiddleName) &&
                                                         x.DateOfBirth == db &&
                                                         x.Status == Person.Status.Active).FirstOrDefault();
      if (persona == null)
      {
        // Если персона не найдена, то искать по переданным ФИО и ДР = 01.01.1900. и действующим записям.
        var defaultDateOfBirth = Sungero.Core.Calendar.GetDate(1900, 1, 1);
        persona = lenspec.Etalon.People.GetAll()
          .Where(x => Equals(pkg.Body.lname,x.LastName) && Equals(pkg.Body.fname,x.FirstName) &&
                 Equals(pkg.Body.mname,x.MiddleName) && Equals(x.DateOfBirth, defaultDateOfBirth) &&
                 x.Status == Person.Status.Active).FirstOrDefault();
      }
      if (persona == null)
      {
        // Если персона не найдена, то искать по коду Инвест и действующим записям.
        persona = lenspec.Etalon.People.GetAll().Where(x => x.CodeInvestavis == pkg.Body.dp_id &&
                                                       x.Status == Person.Status.Active).FirstOrDefault();
        if (persona == null)
          // Если персона не найдена, то ищем по всем статусам
          persona = lenspec.Etalon.People.GetAll().Where(x => Equals(pkg.Body.lname,x.LastName) &&
                                                         Equals(pkg.Body.fname,x.FirstName) &&
                                                         Equals(pkg.Body.mname,x.MiddleName) &&
                                                         x.DateOfBirth == db ).FirstOrDefault();
        if (persona == null)
        {
          // Если персона не найдена, то искать по переданным ФИО и ДР = 01.01.1900.
          var defaultDateOfBirth = Sungero.Core.Calendar.GetDate(1900, 1, 1);
          persona = lenspec.Etalon.People.GetAll()
            .Where(x => Equals(pkg.Body.lname,x.LastName) && Equals(pkg.Body.fname,x.FirstName) &&
                   Equals(pkg.Body.mname,x.MiddleName) && Equals(x.DateOfBirth, defaultDateOfBirth)).FirstOrDefault();
        }
        
        if (persona == null)
          // Если персона не найдена, то искать по коду Инвест
          persona = lenspec.Etalon.People.GetAll().Where(x => x.CodeInvestavis == pkg.Body.dp_id).FirstOrDefault();
        if (persona == null)
          persona = lenspec.Etalon.People.Create();
        else
        {
          if ((persona.IsLawyeravis.HasValue && persona.IsLawyeravis == true)  ||             //если есть другие категории - не обновлять и отправить оповещение
              (persona.IsEmployeeGKavis.HasValue && persona.IsEmployeeGKavis == true)  ||
              (persona.IsOtheravis.HasValue && persona.IsOtheravis == true)
             )
          {
            // <Имя модуля или решения>.PublicConstants.Module.<Имя константы>

            var role = Sungero.CoreEntities.Roles.GetAll().Where(x => x.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
            if (role != null)
            {
              var noticeList =new List<IRecipient>();
              
              foreach (var r in role.RecipientLinks)
              {
                noticeList.Add(r.Member);
              }
              
              // Вычисление, что не совпадает, для сообщения
              var diff = "";
              if (persona.LastName != pkg.Body.lname)
                diff = "Фамилия";
              
              if (persona.FirstName != pkg.Body.fname)
                diff = diff == "" ? "Имя": diff + ", Имя";
              
              if (persona.MiddleName != pkg.Body.mname)
                diff = diff == "" ? "Отчество" : diff + ", Отчество";
              
              if (persona.DateOfBirth != db)
                diff = diff == "" ? "Дата рождения" : diff + ", Дата рождения";
              
              if (noticeList.Any())
              {
                var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices("У " + persona.Name + " с кодом Инвест = " + persona.CodeInvestavis + " не совпадают ключевые данные: " + diff , noticeList.ToArray());
                notice.Attachments.Add(persona);
                notice.Start();
              }
            }
            
            return "Не совпадают данные у персоны с ID = " + persona.Id.ToString();
          }
        }
      }
      
      persona.Name = pkg.Body.lname + " " + pkg.Body.fname + " " + pkg.Body.mname;
      persona.FirstName = pkg.Body.fname;
      persona.LastName = pkg.Body.lname;
      persona.MiddleName = pkg.Body.mname;
      persona.DateOfBirth = DateTime.Parse(pkg.Body.birth_dt);
      persona.Status = Person.Status.Active;
      persona.Email = pkg.Body.email;
      persona.CodeLKavis = pkg.Body.lkuser_id;
      
      if (pkg.Body.dp_inn != "-")
        persona.TIN = pkg.Body.dp_inn;
      
      persona.Phones = pkg.Body.phone;
      
      persona.CodeInvestavis = pkg.Body.dp_id;
      
      if (!string.IsNullOrWhiteSpace(pkg.Body.person_sex))
      {
        if (pkg.Body.person_sex == "1")
          persona.Sex = lenspec.Etalon.Person.Sex.Male;
        
        if (pkg.Body.person_sex == "2")
          persona.Sex = lenspec.Etalon.Person.Sex.Female;
      }

      persona.IsClientBuyersavis = true;
      persona.Countryavis = pkg.Body.adress_country;
      persona.Regionavis  = pkg.Body.address_region;
      persona.Indexavis = pkg.Body.address_zip;
      
      //Поля «Город» и «Район» и "Населенный пункт" объединяются в одно поле через разделитель «, »
      persona.Cityavis =  pkg.Body.address_city;
      if (!string.IsNullOrWhiteSpace(persona.Cityavis) && !string.IsNullOrWhiteSpace(pkg.Body.address_district))
        persona.Cityavis += ", " + pkg.Body.address_district;
      else
      {
        if (!string.IsNullOrWhiteSpace(pkg.Body.address_district))
          persona.Cityavis = pkg.Body.address_district;
      }
      if (!string.IsNullOrWhiteSpace(persona.Cityavis) && !string.IsNullOrWhiteSpace(pkg.Body.address_locality))
        persona.Cityavis += ", " + pkg.Body.address_locality;
      else
      {
        if (!string.IsNullOrWhiteSpace(pkg.Body.address_locality))
          persona.Cityavis = pkg.Body.address_locality;
      }
      
      persona.PostalAddress = pkg.Body.mail_address;
      persona.LegalAddress = pkg.Body.address_reg;
      persona.HomeAddressavis = pkg.Body.address_street + ", " +  pkg.Body.address_house;
      
      if (!string.IsNullOrWhiteSpace(pkg.Body.address_bldg))
        persona.HomeAddressavis  += ", " + pkg.Body.address_bldg;
      
      if (!string.IsNullOrWhiteSpace(pkg.Body.address_flat))
        persona.HomeAddressavis  += ", " + pkg.Body.address_flat;
      
      if ( pkg.Body.rev_pd == 1)  // Согласен
        persona.IsPersonalDataavis = true;
      
      if ( pkg.Body.rev_pd == 0)  // NOT Согласен ELSE NO DATA
        persona.IsPersonalDataavis = false;
      
      try
      {
        persona.Save();
        return persona.Id.ToString();
      }
      catch (Exception ex)
      {
        return "Не удалось создать/обновить персону " + persona.CodeInvestavis + " - " + ex.Message;
      }
    }
    
    #endregion
    
    #region Интеграция с ЛК.
    /// <summary>
    /// Создание обращения из ЛК.
    /// </summary>
    /// <param name="content">Строка в формате Base64.</param>
    /// <returns>Результат запроса.</returns>
    [Public(WebApiRequestType = RequestType.Post)]
    public string CreateCustomerRequest(Structures.Module.ILkHeaderRequest header, Structures.Module.ILkCreateCustomerRequestBody body)
    {
      Logger.Debug("CreateCustomerRequest function started.");
      
      var result = lenspec.Etalon.Module.Integration.Structures.Module.LkCreateCustomerResponseModel.Create();
      result.Header = lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse.Create();
      result.Header.Status = "OK";
      try
      {
        // Заполняем ИД сообщения в ответ.
        result.Header.MessageId = header.MessageId;
        // Создаём обращение и в ответ записываем ид созданного обращения.
        result.Body = CreateCustomer(header, body).ToString();
      }
      catch(Exception ex)
      {
        // Заполняем как ошибку.
        result.Header.Status = "Error";
        result.Header.StatusMessage = ex.Message;
      }
      
      var jsonResult = JsonConvert.SerializeObject(result);
      // Отправляем ответ в nats.
      var async = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncNatsRequestavis.Create();
      async.Subject = "lk";
      async.Data = jsonResult;
      async.ExecuteAsync();
      
      // Отправляем пустой ответ, его всеравно не кто не будет слушать.
      return jsonResult;
    }
    
    [Public(WebApiRequestType = RequestType.Post)]
    public string GetStatusCustomerRequests(Structures.Module.ILkHeaderRequest header, Structures.Module.ILkGetStatusCustomerRequestBody body)
    {
      Logger.Debug("GetStatusCustomerRequests function started.");
      
      var result = lenspec.Etalon.Module.Integration.Structures.Module.LkGetStatusCustomerResponseModel.Create();
      result.Header = lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse.Create();
      result.Header.Status = "OK";
      try
      {
        // Заполняем ИД сообщения в ответ.
        result.Header.MessageId = header.MessageId;
        
        // Получаем задачи с обращениями.
        result.Body = GetStatusCustomer(header, body);
      }
      catch(Exception ex)
      {
        // Заполняем как ошибку.
        result.Header.Status = "Error";
        result.Header.StatusMessage = ex.Message;
      }

      // Отправляем в NATS.
      var jsonResult = JsonConvert.SerializeObject(result);
      var async = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncNatsRequestavis.Create();
      async.Subject = "lk";
      async.Data = jsonResult;
      async.ExecuteAsync();
      
      // Отправляем ответ.
      return jsonResult;
    }
    
    [Public(WebApiRequestType = RequestType.Post)]
    public string GetBodyCustomerRequest(Structures.Module.ILkHeaderRequest header, Structures.Module.IGetBodyCustomerRequestBody body)
    {
      Logger.Debug("GetBodyCustomerRequest function started.");
      
      var result = lenspec.Etalon.Module.Integration.Structures.Module.LkGetBodyCustomerResponseModel.Create();
      result.Header = lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse.Create();
      result.Body = lenspec.Etalon.Module.Integration.Structures.Module.LkGetBodyCustomerResponseBody.Create();
      result.Header.Status = "OK";
      try
      {
        // Заполняем ИД сообщения в ответ.
        result.Header.MessageId = header.MessageId;
        
        // Получаем тело обращения.
        var version = GetVersionCustomer(body.DocID, body.VerNumber);
        
        // Проверки на заполненность настроек для минио.
        var settings = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.LKCode).FirstOrDefault();
        if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
          throw new Exception("Не найдены настройки подключения к minio.");
        
        // Расшифровываем, и адрес и путь к ключу.
        var settingsString = Encryption.Decrypt(settings.ConnectionParams);
        var connectionString = settingsString.Split(';');
        if (string.IsNullOrEmpty(connectionString[0]))
          throw new Exception("Отсутствует строка endpoint.");
        if (string.IsNullOrEmpty(connectionString[1]))
          throw new Exception("Отсутствует строка accessKey.");
        if (string.IsNullOrEmpty(connectionString[2]))
          throw new Exception("Отсутствует строка secretKey.");
        if (string.IsNullOrEmpty(connectionString[3]))
          throw new Exception("Отсутствует строка bucketNameGet.");
        if (string.IsNullOrEmpty(connectionString[4]))
          throw new Exception("Отсутствует строка bucketNamePut.");
        
        // Заполняем имя файла в промежуточном хранилище.
        var objectName = $"{body.DocID}_{body.VerNumber}.{version.AssociatedApplication.Extension}";
        
        var minio = new MinioClient()
          .WithEndpoint(connectionString[0])
          .WithCredentials(connectionString[1], connectionString[2])
          .WithSSL(false)
          .Build();
        
        // Upload a file to bucket.
        var putObjectArgs = new PutObjectArgs()
          .WithBucket(connectionString[4]) // название бакета.
          .WithObject(objectName) // название файла с типом файла name.txt.
          .WithStreamData(version.Body.Read()) // Поток с телом.
          .WithContentType(version.AssociatedApplication.Extension); // Поидеи контент тип надо в формате "text/*" но для теста попробуем так.
        minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
        
        // Get url.
        PresignedGetObjectArgs argsMinio = new PresignedGetObjectArgs()
          .WithBucket(connectionString[4]) // название бакета.
          .WithObject(objectName) // название файла с типом файла name.txt.
          .WithExpiry(60 * 60 * 24);
        // Заполняем url для ответа.
        result.Body.DocLink = minio.PresignedGetObjectAsync(argsMinio).Result;
      }
      catch(Exception ex)
      {
        // Заполняем как ошибку.
        result.Header.Status = "Error";
        result.Header.StatusMessage = ex.Message;
      }

      var resultJson = JsonConvert.SerializeObject(result);

      // Отправляем в NATS.
      var jsonResult = JsonConvert.SerializeObject(result);
      var async = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncNatsRequestavis.Create();
      async.Subject = "lk";
      async.Data = resultJson;
      async.ExecuteAsync();
      // Отправляем ответ пустой так как лк его всеравно не ждет.
      return resultJson;
    }
    
    [Public(WebApiRequestType = RequestType.Post)]
    public string GetStatusDocRequestsByContract(Structures.Module.ILkHeaderRequest header, Structures.Module.ILkGetStatusDocRequestsByContractRequestBody body)
    {
      var result = lenspec.Etalon.Module.Integration.Structures.Module.LkGetStatusDocRequestsByContractResponseModel.Create();
      result.Header = lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse.Create();
      result.Body = new List<lenspec.Etalon.Module.Integration.Structures.Module.LkGetStatusDocRequestsByContractResponseBody>();
      result.Header.Status = "OK";
      try
      {
        // Заполняем ИД сообщения в ответ.
        result.Header.MessageId = header.MessageId;
        
        // Ищем и проверяем персону.
        var person = lenspec.Etalon.People.GetAll(p => p.CodeLKavis == body.LkUserId).FirstOrDefault();
        if (person == null)
          throw new Exception("Не найдена персона с таким ИД ЛК.");
        
        // Ищем и проверяем договор.
        var clientContract = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(c => c.ClientDocumentNumber == body.ContractNumber && c.CounterpartyClient.Where(cc => cc.ClientItem == person).FirstOrDefault() != null).FirstOrDefault();
        if (clientContract == null)
          throw new Exception("Не найден клиентский договор с таким № и клиентом.");
        
        // Ищем категорию обрашения по коду.
        var categoryRequest = avis.CustomerRequests.CategoryRequests.GetAll(c => c.CategoryCode == body.Category).FirstOrDefault();
        if (categoryRequest == null)
          throw new Exception("Не найдена категория обращения по коду.");
        
        // Ищем обращение клиента.
        var customers = avis.CustomerRequests.CustomerRequests.GetAll(c =>
                                                                      c.Client == person &&
                                                                      c.SDAContracts.Where(s => s.Contract == clientContract) != null &&
                                                                      c.ReqCategory == categoryRequest);
        
        // Подготавливаем ответ с обращениями.
        foreach (var customer in customers)
        {
          var customerResponse = lenspec.Etalon.Module.Integration.Structures.Module.LkGetStatusDocRequestsByContractResponseBody.Create();
          customerResponse.DocId = customer.Id;
          customerResponse.DocName = customer.Name;
          customerResponse.VerNumber = customer.LastVersion.Id;
          
          result.Body.Add(customerResponse);
        }
      }
      catch(Exception ex)
      {
        // Заполняем как ошибку.
        result.Header.Status = "Error";
        result.Header.StatusMessage = ex.Message;
      }
      
      var jsonResult = JsonConvert.SerializeObject(result);
      
      /// Отправляем ответ в nats
      var async = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncNatsRequestavis.Create();
      async.Subject = "lk";
      async.Data = jsonResult;
      async.ExecuteAsync();
      
      // Отправляем пустой ответ, его всеравно не кто не будет слушать.
      return jsonResult;
    }
    
    /// <summary>
    /// Получить версию документа.
    /// </summary>
    /// <param name="docId">ИД документа.</param>
    /// <param name="verNumber">Версия документа.</param>
    /// <returns></returns>
    private Sungero.Content.IElectronicDocumentVersions GetVersionCustomer(long docId, long verNumber)
    {
      // Ищем документ с таким ИД.
      var doc = avis.CustomerRequests.CustomerRequests.GetAll(c => c.Id == docId).FirstOrDefault();
      if (doc == null)
        throw new Exception("Не найдено обращение с таким docId.");
      
      // Ищем версию с таким номером.
      var version = doc.Versions.Where(d => d.Number == verNumber).FirstOrDefault();
      if (version == null)
        throw new Exception("Не найдена версия документа.");
      
      return version;
    }
    
    /// <summary>
    /// Создание обращения в RX.
    /// </summary>
    /// <param name="createCustomer"></param>
    /// <returns>ИД созданного обращения.</returns>
    private long CreateCustomer(Structures.Module.ILkHeaderRequest header, Structures.Module.ILkCreateCustomerRequestBody body)
    {
      // Проверяем поля на заполненность.
      if (string.IsNullOrEmpty(header.MessageId))
        throw new Exception("Не заполнено обязательное поле 'Уникальный GUID запроса'.");
      
      if (string.IsNullOrEmpty(body.LkUserId))
        throw new Exception("Не заполнено обязательное поле 'ИД клиента'.");
      
      if (string.IsNullOrEmpty(body.ContractNumber))
        throw new Exception("Не заполнено обязательное поле '№ договора'.");
      
      if (string.IsNullOrEmpty(body.ContractDate))
        throw new Exception("Не заполнено обязательное поле 'Дата договора'.");
      
      if (string.IsNullOrEmpty(body.ClientPhoneNumber))
        throw new Exception("Не заполнено обязательное поле 'Телефон клиента'");
      
      if (string.IsNullOrEmpty(body.RequestDate))
        throw new Exception("Не заполнено обязательное поле 'Дата обращения'.");
      
      if (string.IsNullOrEmpty(body.Category))
        throw new Exception("Не заполнено обязательное поле 'Номер обращения/маршрута'.");
      
      if (string.IsNullOrEmpty(body.CompanyINN))
        throw new Exception("Не заполнено обязательное поле 'Наша организация (передается ИНН)'.");
      
      if (string.IsNullOrEmpty(body.Address))
        throw new Exception("Не заполнено обязательное поле 'Адрес проживания клиента'.");
      
      // Вычисляем персону клиента.
      var person = lenspec.Etalon.People.GetAll(p => p.CodeLKavis == body.LkUserId).FirstOrDefault();
      if (person == null)
        throw new Exception("Не найден клиент с таким 'ИД клиента'.");
      
      // Дата договора в DateTime формате.
      var contractDate = DateTime.Parse(body.ContractDate);
      // Вычисляем клиентский договор.
      var clientContract = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(c =>
                                                                                    c.CounterpartyClient.Where(cc => cc.ClientItem == person).FirstOrDefault() != null &&
                                                                                    c.ClientDocumentNumber == body.ContractNumber &&
                                                                                    c.ClientDocumentDate.Value == contractDate).FirstOrDefault();
      
      if (clientContract == null)
        throw new Exception("Не найден клиентский договор.");
      
      // Вычисляем категорию обращения.
      var categoryRequest = avis.CustomerRequests.CategoryRequests.GetAll(c => c.CategoryCode == body.Category).FirstOrDefault();
      if (categoryRequest == null)
        throw new Exception("Не найдена категория обращения по 'Код категории'.");
      
      // Если объект обязательно, но его нету в клиентском договоре, то отправляем ошибку.
      if (categoryRequest.IsObject == true && clientContract.ObjectAnProject == null)
        throw new Exception("В клиентском договоре отсутствует Объект, который для данной категории обязателен.");
      
      // Если тип заявки материальная, то проверяем что 'сумму возврата' прислали.
      if (categoryRequest.ClaimType == avis.CustomerRequests.CategoryRequest.ClaimType.YesMat && string.IsNullOrEmpty(body.Sum))
        throw new Exception("Для данного типа заявки, должна быть указана 'Сумма возврата'.");
      
      // Если банк обязательно, то проверяем что все поля банка заполненны.
      if (categoryRequest.IsBankDetail == true)
      {
        if (string.IsNullOrEmpty(body.PaymentAccount))
          throw new Exception("Поле 'Расчётный счёт' обязательно для данной категории.");
        if (string.IsNullOrEmpty(body.Bank))
          throw new Exception("Поле 'Банк' обязательно для данной категории.");
        if (string.IsNullOrEmpty(body.BicBank))
          throw new Exception("Поле 'БИК банка' обязательно для данной категории.");
        if (string.IsNullOrEmpty(body.InnBank))
          throw new Exception("Поле 'ИНН банка' обязательно для данной категории.");
        if (string.IsNullOrEmpty(body.KppBank))
          throw new Exception("Поле 'КПП банка' обязательно для данной категории.");
        if (string.IsNullOrEmpty(body.CorrespondentAccount))
          throw new Exception("Поле 'Корр. счёт' обязательно для данной категории.");
        if (string.IsNullOrEmpty(body.Recipient))
          throw new Exception("Поле 'Получатель' обязательно для данной категории.");
      }
      
      // Вычисляем нашу организацию по инн.
      var businessUnit = BusinessUnits.GetAll(b => b.TIN == body.CompanyINN).FirstOrDefault();
      if (businessUnit == null)
        throw new Exception("Не найдена наша организация по 'инн'.");
      
      // Способ доставки.
      var deliveryMethod = lenspec.Etalon.MailDeliveryMethods.GetAll(d => d.Name == "Личный кабинет").FirstOrDefault();
      if (deliveryMethod == null)
        throw new Exception("Отсутствует способ доставки 'Личный кабинет'.");
      
      // Если ссылка на документ есть, то...
      foreach (var url in body.Urls)
      {
        CreateOrUpdateClientDocument(url.Url, url.Extencion, clientContract, person, categoryRequest);
      }
      
      // Получаем имя шаблона из которого собздавать новую заявку.
      var templateNameRaw = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.TemplateCustomerRequest).FirstOrDefault();
      if (templateNameRaw == null || string.IsNullOrEmpty(templateNameRaw.ConnectionParams))
        throw new Exception("Не найдены настройка с именем шаблона.");
      
      // Расшифровываем имя шаблона.
      var templateName = Encryption.Decrypt(templateNameRaw.ConnectionParams);
      
      // Создание новой заявки.
      var template = Sungero.Content.ElectronicDocumentTemplates.GetAll(t => t.Name.Contains(templateName)).FirstOrDefault();
      var newCustomerRequest = avis.CustomerRequests.CustomerRequests.CreateFrom(template);
      newCustomerRequest.Client = person;
      newCustomerRequest.SDAContracts.AddNew().Contract = clientContract;
      newCustomerRequest.Telephon = body.ClientPhoneNumber;
      newCustomerRequest.Created = contractDate;
      newCustomerRequest.ReqCategory = categoryRequest;
      newCustomerRequest.BusinessUnit = businessUnit;
      newCustomerRequest.Subject = body.TerritoryAddress;
      newCustomerRequest.DeliveryMethod = deliveryMethod;
      
      newCustomerRequest.HFAddress = body.Address;
      if (!string.IsNullOrEmpty(body.Sum))
        newCustomerRequest.ClaimAmt = double.Parse(body.Sum);
      
      // Если банк обязательно, то заполняем.
      if (categoryRequest.IsBankDetail == true)
      {
        newCustomerRequest.HFBankAccount = body.PaymentAccount;
        newCustomerRequest.BankName = body.Bank;
        newCustomerRequest.HFBankBIK = body.BicBank;
        newCustomerRequest.HFBankINN = body.InnBank;
        newCustomerRequest.HFBankKPP = body.KppBank;
        newCustomerRequest.HFBankCorrAcc = body.CorrespondentAccount;
        newCustomerRequest.HFRecipient = body.Recipient;
      }
      
      newCustomerRequest.Subject = "\nОбращение клиента создано из Личного Кабинета";
      // Обновляем поля в шаблоне.
      newCustomerRequest.UpdateTemplateParameters();
      newCustomerRequest.Save();
      
      // Стартуем задачу.
      var task = avis.CustomerRequests.CustomerRequestTasks.Create();
      task.Subject = "Регистрация обращения клиента";
      task.Attachments.Add(newCustomerRequest);
      task.Start();
      
      return newCustomerRequest.Id;
    }
    
    /// <summary>
    /// Создаём новый клиентский документ или обновляем версию каждого найденного документа.
    /// </summary>
    /// <param name="url">Ссылка на документв промежуточном хранилище.</param>
    /// <param name="clientContract">Клиентский договор.</param>
    /// <param name="people">Клиент.</param>
    /// <param name="categoryRequest">Категория обращения клиента.</param>
    private void CreateOrUpdateClientDocument(string url,
                                              string extension,
                                              lenspec.SalesDepartmentArchive.ISDAClientContract clientContract,
                                              lenspec.Etalon.IPerson people,
                                              avis.CustomerRequests.ICategoryRequest categoryRequest)
    {
      // Ищем клиентские договоры подходящие под параметры.
      var clientDocuments = lenspec.SalesDepartmentArchive.SDAClientDocuments.GetAll(c =>
                                                                                     c.ClientContract == clientContract &&
                                                                                     c.CounterpartyClient.Where(cc => cc.ClientItem == people).FirstOrDefault() != null &&
                                                                                     c.RealEstateDocumentKind == categoryRequest.DocKindRealEstate);
      
      // Получить документ из промежуточного хранилища в поток.
      var webClient = new WebClient();
      var stream = webClient.OpenRead(url);
      
      // Если найдены клиентские договоры, то во всех обновляем версию документа.
      if (clientDocuments.Count() > 0)
      {
        foreach (var clientDocument in clientDocuments)
        {
          clientDocument.CreateVersionFrom(stream, extension);
        }
        
        return;
      }
      
      // Если не нашли клиентских документов. То создаём новый.
      var newClientDocument = lenspec.SalesDepartmentArchive.SDAClientDocuments.Create();
      newClientDocument.CreateVersionFrom(stream, extension);
      newClientDocument.ClientContract = clientContract;
      newClientDocument.BusinessUnit = clientContract.BusinessUnit;
      newClientDocument.RealEstateDocumentKind = categoryRequest.DocKindRealEstate;
      newClientDocument.Save();
    }
    
    /// <summary>
    /// Получить список задач с вложенными обращниями.
    /// </summary>
    /// <param name="getStatusCustomer"></param>
    /// <returns></returns>
    private List<Structures.Module.LkGetStatusCustomerResponseBody> GetStatusCustomer(Structures.Module.ILkHeaderRequest header, Structures.Module.ILkGetStatusCustomerRequestBody body)
    {
      // Получаем персону по ИД ЛК.
      var person = lenspec.Etalon.People.GetAll(p => p.CodeLKavis == body.LkUserId).FirstOrDefault();
      if (person == null)
        throw new Exception("Не найдено персоны с таким ид лк.");
      
      // Получаем все обращения персоны.
      var customerRequests = avis.CustomerRequests.CustomerRequests.GetAll(c => c.Client == person);
      
      if (customerRequests == null)
        throw new Exception("Не найдено не одного обращения у данной персоны.");
      
      // Находим все задачи с Исполнение поручения, и проверяем что во вложении есть какое либо из обращений
      var tasks = lenspec.Etalon.ActionItemExecutionTasks.GetAll();

      var result = new List<lenspec.Etalon.Module.Integration.Structures.Module.LkGetStatusCustomerResponseBody>();
      
      foreach (var task in tasks)
      {
        var customerRequest = avis.CustomerRequests.CustomerRequests.As(task.AllAttachments.Where(i => avis.CustomerRequests.CustomerRequests.Is(i)).FirstOrDefault());
        // Если не вложено обращение клиента, то пропускаем.
        if (customerRequest == null)
          continue;
        
        // Если обращение клиента не на данную персону, пропускаем.
        if (customerRequest.Client != person)
          continue;

        var bodyResponse = lenspec.Etalon.Module.Integration.Structures.Module.LkGetStatusCustomerResponseBody.Create();
        bodyResponse.DocID = customerRequest.Id;
        bodyResponse.DocName = customerRequest.Name;
        bodyResponse.VerNumber = customerRequest.LastVersion.Number.Value;
        bodyResponse.State = task.Status.Value.ToString();
        
        result.Add(bodyResponse);
      }
      
      return result;
    }
    #endregion
    
    #region Интеграция с ПКП.
    
    /// <summary>
    /// Создаём или обновляем версию акта.
    /// </summary>
    /// <param name="url">Ссылка на документ в минио.</param>
    /// <param name="extension">Расширение документа.</param>
    /// <param name="act">Акт.</param>
    private void CreateOrUpdateAct(string url,
                                   string extension,
                                   lenspec.SalesDepartmentArchive.ISDAActAcceptanceOfApartment act)
    {
      // Получить документ из промежуточного хранилища в поток.
      var webClient = new WebClient();
      var stream = webClient.OpenRead(url);
      
      // Запимываем в версию документа.
      act.CreateVersionFrom(stream, extension);
    }
    
    /// <summary>
    /// Запрос на создание акта.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    [Public(WebApiRequestType = RequestType.Post)]
    public string CreateConstructionObjectAct(Structures.Module.ILkHeaderRequest header, Structures.Module.IPkpCreateConstructionObjectActRequestBody body)
    {
      Logger.Debug("CreateConstructionObjectAct function started.");
      
      // Обьявляем ответ.
      var result = lenspec.Etalon.Module.Integration.Structures.Module.PkpCreateConstructionObjectActResponseModel.Create();
      result.Header = lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse.Create();
      result.Body = lenspec.Etalon.Module.Integration.Structures.Module.PkpCreateConstructionObjectActResponseBody.Create();
      result.Header.Status = "OK";
      
      try
      {
        // Заполняем ИД сообщения в ответ.
        result.Header.MessageId = header.MessageId;
        
        // Проверяем поля на пустоту.
        if (string.IsNullOrEmpty(header.MessageId))
          throw new Exception("В запросе отсутствует обязательное поле MessageId.");
        if (string.IsNullOrEmpty(body.InvestContractId))
          throw new Exception("В запросе отсутствует обязательное поле InvestContractId.");
        if (string.IsNullOrEmpty(body.DocumentKind))
          throw new Exception("В запросе отсутствует обязательное поле DocumentKind.");
        if (string.IsNullOrEmpty(body.DateAct))
          throw new Exception("В запросе отсутствует обязательное поле DateAct.");
        
        // Ищем клиентский договор.
        var clientContract = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(c => c.InvestContractCode == body.InvestContractId).FirstOrDefault();
        if (clientContract == null)
          throw new Exception("Клиентский договор с таким Кодом договора не найден.");
        
        // Получаем DocumentKind
        long kindId;
        switch (body.DocumentKind)
        {
          case "1":
            kindId = Sungero.Docflow.PublicFunctions.Module.GetDocumentKindIdByGuid(lenspec.SalesDepartmentArchive.PublicConstants.Module.ActAcceptanceOfApartmentTypeGuid,
                                                                                    lenspec.SalesDepartmentArchive.PublicConstants.Module.InspectionAct);
            break;
          case "2":
            kindId = Sungero.Docflow.PublicFunctions.Module.GetDocumentKindIdByGuid(lenspec.SalesDepartmentArchive.PublicConstants.Module.ActAcceptanceOfApartmentTypeGuid,
                                                                                    lenspec.SalesDepartmentArchive.PublicConstants.Module.CertificateOfCompletion);
            break;
          case "3":
            kindId = Sungero.Docflow.PublicFunctions.Module.GetDocumentKindIdByGuid(lenspec.SalesDepartmentArchive.PublicConstants.Module.ActAcceptanceOfApartmentTypeGuid,
                                                                                    lenspec.SalesDepartmentArchive.PublicConstants.Module.AcceptanceTransferApartmentKind);
            break;
          case "4":
            kindId = Sungero.Docflow.PublicFunctions.Module.GetDocumentKindIdByGuid(lenspec.SalesDepartmentArchive.PublicConstants.Module.ActAcceptanceOfApartmentTypeGuid,
                                                                                    lenspec.SalesDepartmentArchive.PublicConstants.Module.ConstructionReadinessAct);
            break;
          default:
            throw new Exception("Не найден такой вид документа.");
        }
        var docKind = Sungero.Docflow.DocumentKinds.GetAll(d => d.Id == kindId).FirstOrDefault();
        if (docKind == null)
          throw new Exception("DocumentKind не найден.");
        
        // Создаём акт.
        var act = lenspec.SalesDepartmentArchive.SDAActAcceptanceOfApartments.Create();
        act.DocumentKind = docKind;
        act.ClientContract = clientContract;
        act.ActDate = DateTime.Parse(body.DateAct);
        
        // Заполняем список клиентов.
        foreach (var client in clientContract.CounterpartyClient)
        {
          var newClient = act.ClientNew.AddNew();
          newClient.Client = client.ClientItem;
        }
        
        // Надо что бы в запросе присылали extension.
        if (!string.IsNullOrEmpty(body.Link) && !string.IsNullOrEmpty(body.Extension))
          CreateOrUpdateAct(body.Link, body.Extension, act);
        
        // Сохраняем акт.
        act.Save();
        
        result.Body.DirectumId = act.Id.ToString();
      }
      catch(Exception ex)
      {
        // Заполняем как ошибку.
        result.Header.Status = "Error";
        result.Header.StatusMessage = ex.Message;
      }
      
      // Отправляем ответ в Nats.
      var async = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncNatsRequestavis.Create();
      async.Subject = "pkp";
      async.Data = JsonConvert.SerializeObject(result);
      async.ExecuteAsync();
      
      // Отправляем пустой ответ, его всеравно не кто не будет слушать.
      return JsonConvert.SerializeObject(result);
    }
    #endregion
    
    #region Интеграция с CRM.
    [Public]
    public void SendCrmPutClaimRequest(avis.CustomerRequests.ICustomerRequest customerRequest)
    {
      // Получаем токены.
      var settings = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.CRMCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        return; //throw new Exception("Не найдены настройки подключения");
      
      // Расшифровываем, и разделяем на токен и секретный ключ.
      var settingsString = Encryption.Decrypt(settings.ConnectionParams);
      
      // Адрес CRM.
      var url = settingsString;

      // Подготавляем данные для отправки.
      var crmRequest = lenspec.Etalon.Module.Integration.Structures.Module.CrmPutClaimRequest.Create();
      var json = JsonConvert.SerializeObject(crmRequest);
      
      var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
      httpWebRequest.ContentType = "application/json";
      httpWebRequest.Method = "POST";

      using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
        streamWriter.Write(json);
      }

      var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
      using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
      {
        var result = streamReader.ReadToEnd();
      }
    }
    #endregion
    
    #region Другое.
    
    /// <summary>
    /// Сохранить json запроса в лог файл.
    /// </summary>
    /// <param name="json">json строка.</param>
    private void SavePackageToFile(string json)
    {
      // Получаем путь к папке для логов.
      var pathConstant = lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.InvestInegrationLogCode).FirstOrDefault();
      
      // Если константы нету, или она пуста, то не сохраняем.
      if (pathConstant == null || string.IsNullOrEmpty(pathConstant.Value))
        return;
      
      // Проверяем существование директории.
      if (Directory.Exists(pathConstant.Value) == false)
        return;
      
      // Проверяем существование файла логов.
      var calendarNow = Sungero.Core.Calendar.Now.ToString("dd-MM-yyyy");
      var pathFile = $"{pathConstant.Value}\\InvestInegrationLog-{calendarNow}.txt";
      
      if (File.Exists(pathFile))
      {
        // Создаём.
        using (StreamWriter writer = new StreamWriter(pathFile, false))
        {
          writer.WriteLine(json);
          Logger.Debug("InvestInegrationLog create file.");
        }
      }
      else
      {
        // Изменяем.
        using (StreamWriter writer = new StreamWriter(pathFile, true))
        {
          writer.WriteLine(json);
          Logger.Debug("InvestInegrationLog edit file.");
        }
      }
    }
    
    /// <summary>
    /// Записать информацию из POST-запроса в текстовый файл.
    /// </summary>
    /// <param name="content">Строка в формате Base64.</param>
    /// <returns>Результат запроса.</returns>
    [Public(WebApiRequestType = RequestType.Post)]
    public string ImportPackageFromInvest(string content)
    {
      Logger.Debug("ImportPackageFromInvest function started.");
      
      var result = "Запрос не выполнен. ";
      try
      {
        var body = Convert.FromBase64String(content);
        string jsonText = System.Text.Encoding.Default.GetString(body);
        
        // Сохраняем запрос в лог.
        SavePackageToFile(jsonText);
        // Один раз дернем, чтобы определить, что нам пришло, хотя можно и INDEXOF + SUBSTRING из JSON.
        
        var defineCode = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageSort>(jsonText);
        var code = defineCode.Header.Code;
        
        if (code == "drx_flatsort_updt")
        {
          var pkgSort = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageSort>(jsonText);
          if (pkgSort != null)
          {
            var pkgData = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageSort>(jsonText);
            result = Functions.Module.UpdateSortData(pkgData);
          }
        }
        else if (code ==  "drx_client_updt")
        {
          var pkgPers = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.Package>(jsonText);
          if (pkgPers != null)
          {
            var pkgData = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.Package>(jsonText);
            result = Functions.Module.UpdatePersonData(pkgData);
          }
        }
        else if (code == "drx_project_rspns" )
        {
          
          var pkgCF = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageCF>(jsonText);
          if (pkgCF != null)
          {
            var pkgData = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageCF>(jsonText);
            result = Functions.Module.UpdateCFData(pkgData);
          }
        }
        else if (code == "drx_object_rspns" )
        {
          var pkgObj = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageObj>(jsonText);
          if (pkgObj != null)
          {
            var pkgData = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageObj>(jsonText);
            result = Functions.Module.UpdateObjData(pkgData);
          }
        }
        else if (code == "drx_flatsale_updt" )
        {
          var pkgFlat = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageFlat>(jsonText);
          if (pkgFlat != null)
          {
            var pkgData = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageFlat>(jsonText);
            result = Functions.Module.UpdateFlatData(pkgData);
          }
        }
        else if (code == "drx_contract_updt")
        {
          var pkgCtr = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageCtr>(jsonText);
          if (pkgCtr != null)
          {
            var pkgData = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageCtr>(jsonText);
            result = Functions.Module.UpdateContractData(pkgData);
          }
        }
        else if (code == "drx_pibstatus_rspns")
        {
          var pkg = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageMsg>(jsonText);
          if (pkg != null)
          {
            var pkgData = JsonConvert.DeserializeObject<lenspec.Etalon.Module.Integration.Structures.Module.PackageMsg>(jsonText);
            result = Functions.Module.SendMessageData(pkgData);
          }
        }
        else
        {
          // Иначе - файл.
          var calendarNow = Sungero.Core.Calendar.Now.ToString().Replace(':', '-');
          var fileGuid = System.Guid.NewGuid().ToString();
          var path = $@"C:\Invest integration\Invest {calendarNow} {fileGuid}.txt";  //$@"\\SPBV-TSTDIRRX\Invest integration\Invest {calendarNow} {fileGuid}.txt";
          using (var ms = new MemoryStream(body))
          {
            using (var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            {
              ms.WriteTo(file);
              Logger.Debug("ImportPackageFromInvest function completed.");
              result = "*Информация записана в файл.";
            }
          }
        }
      }
      catch(Exception ex)
      {
        result += $"test {ex.Message}";
        if (ex.InnerException != null)
          result += " " + ex.InnerException.Message;
      }
      
      return result;
    }
    
    #region Обложка
    
    /// <summary>
    /// Получить запись настроек интеграции по коду.
    /// </summary>
    [Remote]
    public static avis.Integration.IIntegrations GetIntegrationKFSettingsRec(string code)
    {
      try
      {
        var record = avis.Integration.Integrationses.GetAll(r=>r.Code.Equals(code)).Single();
        return record;
      }
      catch (InvalidOperationException)
      {
        throw AppliedCodeException.Create("Не удалось найти запись справочника с кодом " + code);
      }
    }
    
    #endregion
    
    [Remote]
    public string GetDBConnectionString()
    {
      var setting = lenspec.Etalon.Module.Integration.Server.ModuleFunctions.GetIntegrationKFSettingsRec(Constants.Module.ActiveDirectoryEmployeeRecordCode);
      if (setting == null)
      {
        throw AppliedCodeException.Create("Не найдены настройки подключения к интеграционной БД");
      }
      
      // Получаем параметры подключения к ад и БД.
      var settingString = Encryption.Decrypt(setting.ConnectionParams);
      var settings = settingString.Split('|');
      
      return settings[0];
    }

    #endregion
  }
}