using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using AvisIntegrationHelper;
using AvisIntegrationHelper.Repositories;
using AvisIntegrationHelper.Models;
using AvisIntegrationHelper.Helpers;

namespace lenspec.Etalon.Module.Integration.Server
{
  partial class ModuleJobs
  {

    /// <summary>
    /// Эталон. Миграция контрагентов из Директум 5.
    /// </summary>
    public virtual void CounterpartyD5Migrationlenspec()
    {
      Functions.Module.ExecuteCounterpartyMigration();
    }

    /// <summary>
    /// Импорт записей из таблицы ConstractionSites
    /// </summary>
    public virtual void ExportConstractionSiteslenspec()
    {
      //Ищем настройки подключения
      var settings = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.ConstractionSitesCode).FirstOrDefault();
      if (settings != null)
      {
        string logtext = FormatLineForlog("Старт импорта из таблицы dbo.ConstractionSites");
        logtext = logtext + FormatLineForlog("Инициализация подключения к интеграционной БД");
        bool withError = false;
        DateTime startDate = Calendar.Now;
        DateTime? lastStartDate = settings.SyncDateTime;
        //Создаем запись в истории загрузки
        var runEntry = settings.RunHistory.AddNew();
        // инициализируем helper для получения данных из интеграционной базы
        string connectionString = Encryption.Decrypt(settings.ConnectionParams);
        if (connectionString != string.Empty)
        {
          logtext += FormatLineForlog("Загрузка Объектов проектов и Объектов строительства, ИСП");
          List<AvisIntegrationHelper.ConstractionSites> constractionSitesList;
          constractionSitesList = AvisIntegrationHelper.DataBaseHelper.GetConstractionSites(connectionString, lastStartDate.Value);
          // Уведомление для Ответственный за Заполнение ИСП в Объектах проектов строительства из 1С
          var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.ResponsibleFillingISP).FirstOrDefault();
          // Создаём и стартуем задачу с уведомлением.
          var task = Sungero.Workflow.SimpleTasks.Create(lenspec.Etalon.Module.Integration.Resources.FillOurCFforObjectsAnProject, administratorEDMSRole);
          task.NeedsReview = false;
          var check = false;
          //Вначале прогоним только ИСП
          foreach (AvisIntegrationHelper.ConstractionSites integraBI in constractionSitesList)
          {
            var codeISP = integraBI.CodeISP;
            var nameISP = integraBI.NameISP;
            int? action = integraBI.Action;
            //Только если заполнен код ИСП и имя ИСП, то берем в работу
            if(!String.IsNullOrEmpty(codeISP) && !String.IsNullOrEmpty(nameISP))
            {
              var isp = lenspec.EtalonDatabooks.OurCFs.GetAll(x => Equals(x.Code1C, codeISP)).SingleOrDefault();
              if (isp == null)
              {
                logtext     += FormatLineForlog("ИСП не найден, создаем нового");
                isp         = lenspec.EtalonDatabooks.OurCFs.Create();
                isp.Status  = Sungero.CoreEntities.DatabookEntry.Status.Active;
                isp.CommercialName  = nameISP;
                isp.Code1C          = codeISP;
                isp.Save();
              }
            }
          }
          foreach (AvisIntegrationHelper.ConstractionSites integraBI in constractionSitesList)
          {
            var constractionSiteId  = integraBI.ConstractionSiteID;
            var codeISP = integraBI.CodeISP;
            var nameISP = integraBI.NameISP;
            var description = integraBI.Description;
            int? action = integraBI.Action;
            var cs = lenspec.EtalonDatabooks.ObjectAnProjects.GetAll(x => Equals(x.Code1c, constractionSiteId)).SingleOrDefault();
            if (cs == null)
            {
              logtext += FormatLineForlog("Объект не найден, создаем нового");
              cs = lenspec.EtalonDatabooks.ObjectAnProjects.Create();
              cs.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
              cs.IsLinkToInvest = false;
            }
            if (cs != null && action != 3)
            {
              logtext += FormatLineForlog(string.Format("Объект найден [{0}] ид [{1}], обновляем реквизиты", cs.Name, cs.Code1c));
              cs.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
              cs.IsLinkToInvest = false;
            }
            if (action == 3)
            {
              cs.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
              cs.IsLinkToInvest = false;
            }
            AvisIntegrationHelper.DataBaseHelper.UpdateConstractionSites(connectionString, "Done", constractionSiteId);
            cs.Name                               = description;
            cs.Code1c                             = constractionSiteId;
            cs.OurCF                              = lenspec.EtalonDatabooks.OurCFs.GetAll(x => Equals(x.Code1C, codeISP)).SingleOrDefault();
            cs.State.Properties.OurCF.IsRequired  = false;
            cs.Save();
            if (cs.OurCF == null && !task.Attachments.Contains(cs))
            {
              check = true;
              task.Attachments.Add(cs);
            }
          }
          if (check == true)
          {
            task.ActiveText = lenspec.Etalon.Module.Integration.Resources.LinkObjectsAnProjectToOurCF;
            task.Start();
          }
          logtext += FormatLineForlog("Конец импорта Объектов");
          try
          {
            runEntry.StartDate = startDate;
            runEntry.EndDate = Calendar.Now;
            runEntry.Success = !withError;
            runEntry.TextLog = logtext;
            settings.SyncDateTime = startDate;
            settings.Save();
          }
          catch(Exception ex)
          {
            Logger.ErrorFormat("Integration - ImportContractors - не удалось сохранить изменения в настройке интеграции {0} - ", ex, settings.Id);
          }
        }
      }
      ClearLog(settings);
    }

    /// <summary>
    /// Импорт записей КБК(С датой)
    /// </summary>
    public virtual void ExportKBKlenspec()
    {
      //Ищем настройки подключения
      var settings = Integrationses.GetAll(s=>s.Code== lenspec.Etalon.Module.Integration.PublicConstants.Module.KBKCode).FirstOrDefault();
      if (settings != null)
      {
        string logtext = FormatLineForlog("Старт импорта Групп статей бюджета");
        logtext = logtext + FormatLineForlog("Инициализация подключения к интеграционной БД");
        bool withError = false;
        DateTime startDate = Calendar.Now;
        DateTime? lastStartDate = settings.SyncDateTime;
        //Создаем запись в истории загрузки
        var runEntry = settings.RunHistory.AddNew();
        // инициализируем helper для получения данных из интеграционной базы
        string connectionString = Encryption.Decrypt(settings.ConnectionParams);
        if (connectionString != string.Empty)
        {
          logtext += FormatLineForlog("Загрузка Групп статей бюджета");
          List<AvisIntegrationHelper.GroupsBudgetItems> groupsbudjetItemsList;
          string condition = null;
          groupsbudjetItemsList = AvisIntegrationHelper.DataBaseHelper.GetGroupsBudgetItems(connectionString, condition, lastStartDate.Value);
          foreach (AvisIntegrationHelper.GroupsBudgetItems integraBI in groupsbudjetItemsList)
          {
            var status = integraBI.Status;
            var name = integraBI.Name;
            var mainCode = integraBI.MainCode1C;
            var code1C = integraBI.Code1C;
            int? action = integraBI.Action;
            var kbk = lenspec.ContractualWork.GroupsBudgetItemses.GetAll(x => Equals(x.Code1C, code1C)).SingleOrDefault();
            if (kbk == null && action != 3)
            {
              logtext += FormatLineForlog("Группа не найдена, создаем нового");
              kbk = lenspec.ContractualWork.GroupsBudgetItemses.Create();
              if (mainCode != null)
              {
                AvisIntegrationHelper.DataBaseHelper.UpdateGroupsBudgetItems(connectionString, "Update", code1C);
              }
              else
              {
                AvisIntegrationHelper.DataBaseHelper.UpdateGroupsBudgetItems(connectionString, null, code1C);
              }
              kbk.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
              kbk.Code1C = code1C;
              kbk.Name = name;
            }
            if (kbk != null && action != 3)
            {
              logtext += FormatLineForlog(string.Format("Группа найдена [{0}] ид [{1}], обновляем реквизиты", kbk.Name, kbk.Id));
              kbk.Code1C = code1C;
              kbk.Name = name;
              if (mainCode != null)
              {
                AvisIntegrationHelper.DataBaseHelper.UpdateGroupsBudgetItems(connectionString, "Update", code1C);
              }
              else
              {
                AvisIntegrationHelper.DataBaseHelper.UpdateGroupsBudgetItems(connectionString, null, code1C);
              }
            }
            else if (kbk != null && action == 3)
            {
              kbk.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
              AvisIntegrationHelper.DataBaseHelper.UpdateGroupsBudgetItems(connectionString, null, code1C);
            }
            kbk.Save();
          }
          condition = "Update";
          groupsbudjetItemsList = AvisIntegrationHelper.DataBaseHelper.GetGroupsBudgetItems(connectionString, condition, lastStartDate.Value);
          foreach (AvisIntegrationHelper.GroupsBudgetItems integraBI in groupsbudjetItemsList)
          {
            var mainCode = integraBI.MainCode1C;
            var code1C = integraBI.Code1C;
            var name = integraBI.Name;
            var kbk = lenspec.ContractualWork.GroupsBudgetItemses.GetAll(x => Equals(x.Code1C, code1C)).SingleOrDefault();
            var leadingGroup = lenspec.ContractualWork.GroupsBudgetItemses.GetAll(x => Equals(x.Code1C, mainCode)).SingleOrDefault();
            if (leadingGroup != null)
            {
              kbk.LeadingGroup = leadingGroup;
              AvisIntegrationHelper.DataBaseHelper.UpdateGroupsBudgetItems(connectionString, null, code1C);
              kbk.Save();
            }
            else
            {
              AvisIntegrationHelper.DataBaseHelper.UpdateGroupsBudgetItems(connectionString, "Error", code1C);
              // Уведомление для Администратора СЭД.
              var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
              // Создаём и стартуем задачу с уведомлением.
              var task = Sungero.Workflow.SimpleTasks.Create($"Не удалось заполнить Ведущую группу для Группы статей бюджета {name}", administratorEDMSRole);
              task.NeedsReview = false;
              task.ActiveText = $"Не удалось заполнить Ведущую группу для Группы статей бюджета {name}";
              task.Start();
            }
          }
          logtext += FormatLineForlog("Конец импорта Групп статей бюджета");
          
          logtext += FormatLineForlog("Загрузка статей бюджета");
          List<AvisIntegrationHelper.BudjetItems> budjetItemsList;
          budjetItemsList = AvisIntegrationHelper.DataBaseHelper.GetBudjetItems(connectionString, lastStartDate.Value);
          foreach (AvisIntegrationHelper.BudjetItems integraBI in budjetItemsList)
          {
            var name = integraBI.Name;
            var codeKBK = integraBI.CodeKBK;
            var code1C = integraBI.Code1C;
            var groupCode1C = integraBI.GroupCode1C;
            int? action = integraBI.Action;
            var kbk = lenspec.ContractualWork.BudgetItemses.GetAll(x => Equals(x.Code1C, code1C)).SingleOrDefault();
            if (kbk == null )
            {
              logtext += FormatLineForlog("Кбк не найден, создаем нового");
              kbk = lenspec.ContractualWork.BudgetItemses.Create();
              kbk.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
            }
            if (kbk != null && action != 3)
            {
              logtext += FormatLineForlog(string.Format("Кбк найден [{0}] ид [{1}], обновляем реквизиты", kbk.Name, kbk.Id));
              kbk.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
            }
            else if (kbk != null && action == 3)
            {
              kbk.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
            }
            AvisIntegrationHelper.DataBaseHelper.UpdateBudjetItems(connectionString, null, code1C);
            var groupKBK = lenspec.ContractualWork.GroupsBudgetItemses.GetAll(x => Equals(x.Code1C, groupCode1C)).SingleOrDefault();
            if (groupKBK != null)
            {
              kbk.Group = groupKBK;
            }
            else if (action != 3)
            {
              AvisIntegrationHelper.DataBaseHelper.UpdateBudjetItems(connectionString, "Error", code1C);
              // Уведомление для Администратора СЭД.
              var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
              // Создаём и стартуем задачу с уведомлением.
              var task = Sungero.Workflow.SimpleTasks.Create($"Не удалось заполнить Ведущую группу для cтатьи бюджета {name}", administratorEDMSRole);
              task.NeedsReview = false;
              task.ActiveText = $"Не удалось заполнить Ведущую группу для cтатьи бюджет {name}";
              task.Start();
            }
            kbk.Codifier = codeKBK;
            kbk.FullName = name;
            kbk.Code1C = code1C;
            kbk.Save();
          }
          logtext += FormatLineForlog("Конец импорта КБК");
          
          logtext += FormatLineForlog("Загрузка Расшифровки статей бюджета");
          List<AvisIntegrationHelper.DecodingBudgetItems> decodingbudjetItemsList;
          condition = null;
          decodingbudjetItemsList = AvisIntegrationHelper.DataBaseHelper.GetDecodingBudgetItems(connectionString, condition, lastStartDate.Value);
          foreach (AvisIntegrationHelper.DecodingBudgetItems integraBI in decodingbudjetItemsList)
          {
            var name = integraBI.Name;
            var code1C = integraBI.Code;
            var headCode = integraBI.HeadCode;
            int? action = integraBI.State;
            var codeOracle = integraBI.CodeOracle;
            var status = integraBI.Status;
            var kbk = lenspec.ContractualWork.DecodingBudgetItemses.GetAll(x => Equals(x.Code1C, code1C)).SingleOrDefault();
            if (kbk == null && action == 0)
            {
              logtext += FormatLineForlog("РСТ не найдена, создаем нового");
              kbk = lenspec.ContractualWork.DecodingBudgetItemses.Create();
              if (headCode != null)
              {
                AvisIntegrationHelper.DataBaseHelper.UpdateDecodingBudgetItems(connectionString, "Update", code1C);
              }
              else AvisIntegrationHelper.DataBaseHelper.UpdateDecodingBudgetItems(connectionString, null, code1C);
              kbk.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
              kbk.Code1C = code1C;
              kbk.Name = name;
            }
            if (kbk != null)
            {
              logtext += FormatLineForlog(string.Format("Группа найдена [{0}] ид [{1}], обновляем реквизиты", kbk.Name, kbk.Id));
              kbk.Code1C = code1C;
              kbk.Name = name;
              if (headCode != null)
              {
                AvisIntegrationHelper.DataBaseHelper.UpdateDecodingBudgetItems(connectionString, "Update", code1C);
              }
              else AvisIntegrationHelper.DataBaseHelper.UpdateDecodingBudgetItems(connectionString, null, code1C);
            }
            else if (kbk != null && action == 1)
            {
              kbk.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
              AvisIntegrationHelper.DataBaseHelper.UpdateDecodingBudgetItems(connectionString, null, code1C);
            }
            kbk.CodeOracle = codeOracle;
            kbk.Save();
          }
          condition = "Update";
          decodingbudjetItemsList = AvisIntegrationHelper.DataBaseHelper.GetDecodingBudgetItems(connectionString, condition, lastStartDate.Value);
          foreach (AvisIntegrationHelper.DecodingBudgetItems integraBI in decodingbudjetItemsList)
          {
            var headCode = integraBI.HeadCode;
            var code1C = integraBI.Code;
            var name = integraBI.Name;
            var kbk = lenspec.ContractualWork.DecodingBudgetItemses.GetAll(x => Equals(x.Code1C, code1C)).SingleOrDefault();
            var leadingArticle = lenspec.ContractualWork.DecodingBudgetItemses.GetAll(x => Equals(x.Code1C, headCode)).SingleOrDefault();
            if (leadingArticle != null)
            {
              kbk.LeadingArticle = leadingArticle;
              AvisIntegrationHelper.DataBaseHelper.UpdateDecodingBudgetItems(connectionString, null, code1C);
              kbk.Save();
            }
            else
            {
              AvisIntegrationHelper.DataBaseHelper.UpdateDecodingBudgetItems(connectionString, "Error", code1C);
              // Уведомление для Администратора СЭД.
              var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
              // Создаём и стартуем задачу с уведомлением.
              var task = Sungero.Workflow.SimpleTasks.Create($"Не удалось заполнить Ведущую статью для Расшифровки статей бюджета {name}", administratorEDMSRole);
              task.NeedsReview = false;
              task.ActiveText = $"Не удалось заполнить Ведущую статью для Расшифровки статей бюджета {name}";
              task.Start();
            }
          }
          logtext += FormatLineForlog("Конец импорта Расшисрофвки статей бюджета");
          try
          {
            runEntry.StartDate = startDate;
            runEntry.EndDate = Calendar.Now;
            runEntry.Success = !withError;
            runEntry.TextLog = logtext;
            settings.SyncDateTime = startDate;
            settings.Save();
          }
          catch(Exception ex)
          {
            Logger.ErrorFormat("Integration - ImportContractors - не удалось сохранить изменения в настройке интеграции {0} - ", ex, settings.Id);
          }
        }
      }
      ClearLog(settings);
    }


    /// <summary>
    /// Создание записи в ИСП.
    /// </summary>
    private void CreateOurCF(string name, string code1C)
    {
      var ourCF = lenspec.EtalonDatabooks.OurCFs.Create();
      ourCF.Code1C = code1C;
      ourCF.Name = name;
      ourCF.Save();
    }
    
    /// <summary>
    /// Обновить запись ИСП.
    /// </summary>
    /// <param name="ourCF"></param>
    /// <param name="name"></param>
    private void UpdateOurCF(lenspec.EtalonDatabooks.IOurCF ourCF, string name)
    {
      ourCF.Name = name;
      ourCF.Save();
    }
    
    /// <summary>
    /// Закрыть запись ИСП.
    /// </summary>
    /// <param name="ourCF"></param>
    private void CloseOurCF(lenspec.EtalonDatabooks.IOurCF ourCF)
    {
      ourCF.Status = lenspec.EtalonDatabooks.OurCF.Status.Closed;
      ourCF.Save();
    }
    
    /// <summary>
    /// Интеграция 1c ИСП и группы объектов строительства.
    /// </summary>
    public virtual void Import1cOurCFavis()
    {
      //OrgStruct1C
      // Получение строки подключения.
      var setting = lenspec.Etalon.Module.Integration.Server.ModuleFunctions.GetIntegrationKFSettingsRec("OrgStruct1C");
      var settingString = Encryption.Decrypt(setting.ConnectionParams);
      var repository = new AvisIntegrationHelper.Repositories.GroupConstrSiteRepository(settingString);
      
      // Получить все записи из интеграционной таблицы, со стаусом New (пустое).
      var newGroupConstrSites = repository.GetListByStatusIsNullOrEmpty();
      
      foreach(var gorupConstrSite in newGroupConstrSites)
      {
        var ourCF = lenspec.EtalonDatabooks.OurCFs.GetAll(o => o.Code1C == gorupConstrSite.Code1C).FirstOrDefault();
        
        // Не нашли ИСП, статус 1.
        if (ourCF == null && (gorupConstrSite.Action == 1 || gorupConstrSite.Action == 2))
          CreateOurCF(gorupConstrSite.Name, gorupConstrSite.Code1C);
        
        // Нашли ИСП, статус 1.
        if (ourCF != null && (gorupConstrSite.Action == 1 || gorupConstrSite.Action == 2))
          UpdateOurCF(ourCF, gorupConstrSite.Name);
        
        // Нашли ИСП, статус 3.
        if (ourCF != null && gorupConstrSite.Action == 3)
          CloseOurCF(ourCF);
        
        
        // Устанавливаем стутс.
        if ((gorupConstrSite.Action == 1 || gorupConstrSite.Action == 2) && string.IsNullOrEmpty(gorupConstrSite.MainCode1C))
          repository.EditStatus(gorupConstrSite.Id, "Done");
        else if (gorupConstrSite.Action == 3 && ourCF == null)
          repository.EditStatus(gorupConstrSite.Id, "Done");
        else
          repository.EditStatus(gorupConstrSite.Id, "Update");
      }
      
      // Получить все записи из интеграционной таблицы, со стаусом Update.
      var updateGroupConstrSites = repository.GetList("Update");
      
      foreach(var gorupConstrSite in updateGroupConstrSites)
      {
        var ourCF = lenspec.EtalonDatabooks.OurCFs.GetAll(o =>  o.Code1C == gorupConstrSite.Code1C.ToString()).FirstOrDefault();
        
        // Если не нашли, то записываем как ошибку и отправляем уведомление.
        if (ourCF == null)
        {
          // Устанавливаем статус.
          repository.EditStatus(gorupConstrSite.Id, "Error");
          
          // Формируем уведомление Администратору сэд.
          var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
          var task = Sungero.Workflow.SimpleTasks.CreateWithNotices($"Не удалось заполнить Ведущую группу для ИСП {gorupConstrSite.Code1C}.", administratorEDMSRole);
          task.ActiveText = $"Текст задачи: не удалось заполнить Ведущую группу для ИСП:\nКод 1С - {gorupConstrSite.Code1C}\nИмя - {gorupConstrSite.Name}";
          task.Start();
          
          continue;
        }
        var mainOurCF = lenspec.EtalonDatabooks.OurCFs.GetAll(o => o.Code1C == gorupConstrSite.MainCode1C.ToString()).FirstOrDefault();
        
        ourCF.MainGroupOurCF = mainOurCF;
        ourCF.Save();
        
        repository.EditStatus(gorupConstrSite.Id, "Done");
      }
      
      // Получаем все записи Дон, и вырежим их.
      var doneGroupConstrSites = repository.GetList("Done");
      foreach (var groupConstrSite in doneGroupConstrSites)
      {
        repository.DeleteById(groupConstrSite.Id);
      }
    }

    /// <summary>
    /// Экпорт ГИС ЖКХ в IntegraDB.
    /// </summary>
    public virtual void ExportGisJKHInfoavis()
    {
      string logtext = FormatLineForlog("Старт экспорта гис жкх.");
      bool withError = false;
      DateTime startDate = Calendar.Now;
      var setting = lenspec.Etalon.Module.Integration.Server.ModuleFunctions.GetIntegrationKFSettingsRec(Constants.Module.GisClientInfoCode);
      var runEntry = setting.RunHistory.AddNew();
      try
      {
        if (setting == null)
          throw AppliedCodeException.Create("Не найдены настройки подключения к интеграционной БД");
        
        // Получаем параметры подключения к ад и БД
        if (string.IsNullOrEmpty(setting.ConnectionParams))
          throw AppliedCodeException.Create("Строка подключения к интеграционной БД пуста.");
        
        var settingString = Encryption.Decrypt(setting.ConnectionParams);
        var gisClientInfoRepository = new GISClientInfoRepository(settingString);
        
        Logger.Debug("Integration - ExportGisJKHInfoavis - старт экспорта");
        logtext = logtext + FormatLineForlog("Старт экспорта");
        // Получаем все клиентские договоры с заполненной "Дата передачи по АПП".
        var clientContrcts = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(c => c.TransferOfOwnershipDate.HasValue == true);
        logtext = logtext + FormatLineForlog($"Получили список клиентских договор с даной АПП : {clientContrcts.Count()} шт.");

        // Проверяем все клиентские договоры.
        foreach (var clientContrct in clientContrcts)
        {
          // Получаем дату последнего изменения в истории, если изменения нету, берем дату создания.
          var updateHistory = clientContrct.History.GetAll().Where(c => c.Action == Sungero.CoreEntities.History.Action.Update).OrderByDescending(c => c.HistoryDate).FirstOrDefault();
          if (updateHistory == null)
            updateHistory = clientContrct.History.GetAll().Where(c => c.Action == Sungero.CoreEntities.History.Action.Create).OrderByDescending(c => c.HistoryDate).FirstOrDefault();
          
          if (setting.SyncDateTime.HasValue == true && updateHistory.HistoryDate < setting.SyncDateTime)
            continue;
          
          // Пробегаем по всем клиентам.
          foreach (var client in clientContrct.CounterpartyClient)
          {
            // Получаем персону, если не перона то не передаём данные.
            var person = lenspec.Etalon.People.As(client.ClientItem);
            if (person == null)
              continue;
            
            // Заполняем данные по клиенту.
            logtext = logtext + FormatLineForlog($"Заполняем данные по клиенту.");
            var gisClientInfo = new GISClientInfoModel();
            try
            {
              gisClientInfo.LastName = person.LastName;
              gisClientInfo.FirstName = person.FirstName;
              gisClientInfo.MiddleName = person.MiddleName;
              if (person.DateOfBirth.HasValue)
                gisClientInfo.BirthDay = person.DateOfBirth.Value;
              gisClientInfo.InvestCode = person.CodeInvestavis;
              gisClientInfo.Phone = person.Phones;
              gisClientInfo.Email = person.Email;
              
              // Заполняем данные по клиентскому договору.
              if (clientContrct.ObjectAnProject == null)
                continue;
              gisClientInfo.ContractObjectId = clientContrct.ObjectAnProject.Id;
              if (clientContrct.Premises == null)
                continue;
              gisClientInfo.ContractRoomId = clientContrct.Premises.Id;
              
              // Получаем все МКД документы.
              var mkds = avis.ManagementCompanyJKHArhive.ManagementContractMKDs.GetAll(m => m.Client.Equals(person) && m.ObjectAnSale.Equals(clientContrct.Premises));
              logtext = logtext + FormatLineForlog($"Нашли документов мкд {mkds.Count()} шт.");
              // Проходим по всем МКД и отправляем в БД.
              foreach (var mkd in mkds)
              {
                gisClientInfo.ContractNumber = mkd.Number;
                if (mkd.DateDocument.HasValue)
                  gisClientInfo.ContractDate = mkd.DateDocument.Value;
                
                gisClientInfo.ContractDateTransferred = clientContrct.TransferOfOwnershipDate;
                
                // Создаём новую запись в интеграционной таблице.
                gisClientInfoRepository.Add(gisClientInfo);
                logtext = logtext + FormatLineForlog($"Записали в БД клиентский договор : mkd.Id = {mkd.Id} / gisClientInfo.Id = {gisClientInfo.Id}");
                
                // Убераем признак у персоны "Покупатель дольщик", если не нашли не одного договора без даты для этой персоны.
                if (lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(c => c.TransferOfOwnershipDate.HasValue == false && c.CounterpartyClient.FirstOrDefault(cc => cc.ClientItem == person) != null).FirstOrDefault() == null)
                  person.IsClientBuyersavis = false;
                
                // Устанавливаем персоне "Клиент собственник".
                person.IsClientOwnersavis = true;
                try
                {
                  person.Save();
                }
                catch(Exception ex)
                {
                  Logger.ErrorFormat("Integration - ExportGisJKHInfoavis - не удалось сохранить персону {0} - ", ex, person.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.ErrorFormat("Integration - ExportGisJKHInfoavis - не удалось заполнить поля для gisClientInfo, КД {0}, клиент {1} - ", ex, clientContrct.Id, client.Id);
            }
          }
        }
        // Сохраняем дату последней синхронизации.1
        setting.SyncDateTime = Calendar.Now;
        setting.Save();
        Logger.Debug("Integration - ExportGisJKHInfoavis - экспорт завершен");
      }
      catch (Exception ex)
      {
        withError = true;
        logtext += FormatLineForlog(string.Format("Integration - ExportGisJKHInfoavis: {0} {1}", ex.Message, ex.StackTrace));
        Logger.Error("Integration - ExportGisJKHInfoavis - ", ex);
      }
      finally
      {
        // Сохраняем результат загрузки в справочник
        logtext += FormatLineForlog("Конец экспорта гис жкх.");
        try
        {
          runEntry.StartDate = startDate;
          runEntry.EndDate = Calendar.Now;
          runEntry.Success = !withError;
          runEntry.TextLog = logtext;
          setting.SyncDateTime = startDate;
          setting.Save();
        }
        catch(Exception ex)
        {
          Logger.ErrorFormat("Integration - ExportGisJKHInfoavis - не удалось сохранить изменения в настройке интеграции {0} - ", ex, setting.Id);
        }

      }
    }

    /// <summary>
    /// Импорт контрагентов из интеграционной таблицы
    /// </summary>
    public override void ImportContractors()
    {
      //Ищем настройки подключения
      var settings = Integrationses.GetAll(s => s.Code == avis.Integration.Constants.Module.ContractorsImportRecordCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
      {
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      }
      
      string logtext = FormatLineForlog("Старт импорта контрагентов");
      logtext = logtext + FormatLineForlog("Инициализация подключения к интеграционной БД");
      bool withError = false;
      DateTime startDate = Calendar.Now;
      //Создаем запись в истории загрузки
      var runEntry = settings.RunHistory.AddNew();
      try
      {
        // инициализируем helper для получения данных из интеграционной базы
        string connectionString = Encryption.Decrypt(settings.ConnectionParams);
        DateTime? lastStartDate = settings.SyncDateTime;
        if (!string.IsNullOrEmpty(connectionString))
        {
          Logger.Debug("Integration - ImportContractors - старт импорта контрагентов");
          
          #region Организации
          
          // Создаем словарь, в котором будем хранить контрагентов, по которым не удалось сразу обновить головное
          // после загрузки всех контрагентов заполним по ним головные
          // key - ид контрагента в RX, value - данные из интеграционной таблицы по этому контрагенту
          Dictionary<long, AvisIntegrationHelper.Contractor> contrsToUpdateLead = new Dictionary<long, AvisIntegrationHelper.Contractor>();
          
          // Шаг 1. Загрузка контрагентов
          logtext += FormatLineForlog("Загрузка контрагентов");
          List<Contractor> contractorsList;
          if (lastStartDate.HasValue)
            contractorsList = AvisIntegrationHelper.DataBaseHelper.GetContractors(connectionString, lastStartDate.Value);
          else
            contractorsList = AvisIntegrationHelper.DataBaseHelper.GetContractors(connectionString);
          
          Logger.Debug("Integration - ImportContractors - загрузка организаций");
          foreach (Contractor integraContr in contractorsList)
          {
            string directumID = integraContr.DirectumID.ToString();
            // Ищем контрагента с кодом Директум
            logtext += FormatLineForlog(string.Format("Загрузка контрагента [{0}] ИД Directum [{1}] ", integraContr.Name, directumID));
            try
            {
              var company = Etalon.Companies.GetAll(d => Equals(d.ExternalCodeavis, directumID)).SingleOrDefault();
              if (company == null)
              {
                logtext += FormatLineForlog("Контрагент не найден, создаем нового");
                // Создаем нового контрагента
                company = Etalon.Companies.Create();
                company.ExternalCodeavis = directumID;
              }
              logtext += FormatLineForlog(string.Format("Контрагент найден [{0}] ид [{1}], обновляем реквизиты", company.Name, company.Id));
              // Обновляем данные в записи контрагента
              if (integraContr.State)
                company.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
              else
                company.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
              
              company.Name = integraContr.Name;
              company.LegalName = integraContr.LegalName;
              
              //Ищем головного контрагента
              if (!string.IsNullOrEmpty(integraContr.HeadOrg))
              {
                logtext += FormatLineForlog(string.Format("Поиск головного контрагента по коду [{0}]", integraContr.HeadOrg));
                var leadOrg = Etalon.Companies.GetAll(d => Equals(d.ExternalCodeavis, integraContr.HeadOrg)).SingleOrDefault();
                if (leadOrg != null && !Equals(company, leadOrg))
                {
                  logtext += FormatLineForlog(string.Format("Головной контрагент найден: [{0}] ид [{1}]", leadOrg.Name, leadOrg.Id));
                  company.HeadCompany = leadOrg;
                }
                else
                {
                  logtext = logtext + FormatLineForlog("Головной контрагент не найден, заполним после импорта всех контрагентов");
                  contrsToUpdateLead.Add(company.Id, integraContr);
                  // Помечаем к обработке позже
                }
              }
              else
                company.HeadCompany = null;
              
              company.Nonresident = integraContr.Foreign;
              company.TIN = integraContr.INN;
              company.TRRC = integraContr.KPP;
              company.PSRN = integraContr.OGRN;
              company.NCEO = integraContr.OKPO;
              company.LegalAddress = integraContr.LegalAddress;
              company.PostalAddress = integraContr.PostalAddress;
              company.Phones = integraContr.Telephone;
              company.Email = integraContr.Email;
              company.Homepage = integraContr.Site;
              company.Note = integraContr.Note;
              if (integraContr.Group != null)
              {
                var groupCounterparties = avis.EtalonParties.GroupCounterparties.GetAll(x => x.IdDirectum5 != null && x.IdDirectum5 == integraContr.Group).FirstOrDefault();
                company.GroupCounterpartyavis = groupCounterparties;
              }
              else
              {
                company.GroupCounterpartyavis = null;
              }
              if (integraContr.Category != null)
              {
                var categoryCounterparties = avis.EtalonParties.CategoryCounterparties.GetAll(x => x.IdDirectum5 != null && x.IdDirectum5 == integraContr.Category).FirstOrDefault();
                company.CategoryCounterpartyavis = categoryCounterparties;
              }
              else
              {
                company.CategoryCounterpartyavis = null;
              }
              
              var cityName = integraContr.City;
              var regionName = integraContr.Region;
              //ищем Регион
              if (!string.IsNullOrEmpty(regionName))
              {
                var region = GetOrCreateRegion(regionName);
                company.Region = region;
                //ищем нас. пункт
                if (!string.IsNullOrEmpty(cityName))
                {
                  var city = GetOrCreateCity(cityName, region);
                  company.City = city;
                }
              }
              
              try
              {
                if (company != null)
                  company.Save();
              }
              catch (Exception ex)
              {
                logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось сохранить контрагента: {0} {1}", ex.Message, ex.StackTrace));
                // TODO: Исправить, из-за этих ошибок не сохраняет дату последней синхронизации, в следвии чего постоянно синхронизирует все организации.
                //withError = true;
              }
            }
            catch (InvalidOperationException)
            {
              // Найдено несколько контрагентов
              logtext += FormatLineForlog("ОШИБКА Найдено несколько контрагентов, пропускаем контрагента");
              //withError = true;
            }
          }
          
          #endregion
          
          # region Заполнение головных организаций
          
          // Заполняем головную огранизацию, там где не смогли заполнить сразу
          logtext += FormatLineForlog(string.Format("Обновление контрагентов по которым не заполнено головное. Количество контрагентов:{0}", contrsToUpdateLead.Count));
          Logger.Debug("Integration - ImportContractors - заполнение головных организаций");
          foreach (var contrToUpdate in contrsToUpdateLead)
          {
            var company = Etalon.Companies.Get(contrToUpdate.Key);
            //ищем головную организацию
            logtext += FormatLineForlog(string.Format("Обновление головной организации у [{0}] ид [{1}])", company.Name, company.Id));
            try
            {
              var leadOrg = Etalon.Companies.GetAll(d => Equals(d.ExternalCodeavis, contrToUpdate.Value.HeadOrg)).SingleOrDefault();
              if (leadOrg != null && !Equals(company, leadOrg))
              {
                logtext += FormatLineForlog(string.Format("Головная организация найдена: [{0}] ид [{1}]", leadOrg.Name, leadOrg.Id));
                company.HeadCompany = leadOrg;
                company.Save();
              }
              else
              {
                logtext += FormatLineForlog("ОШИБКА Головная организация не найдена");
                //withError = true;
              }
            }
            catch (InvalidOperationException)
            {
              // Найдено несколько организаций
              logtext += FormatLineForlog("ОШИБКА Найдено несколько головных организаций");
              //withError = true;
            }
            catch (Exception ex)
            {
              logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось обновить головную организацию: {0} {1}", ex.Message, ex.StackTrace));
              //withError = true;
            }
          }
          
          #endregion
          
          #region Контактные лица
          
          List<AvisIntegrationHelper.Contact> contacts;
          if (lastStartDate.HasValue)
            contacts = AvisIntegrationHelper.DataBaseHelper.GetContacts(connectionString, lastStartDate.Value);
          else
            contacts = AvisIntegrationHelper.DataBaseHelper.GetContacts(connectionString);
          
          Logger.Debug("Integration - ImportContractors - загрузка контактных лиц");
          foreach(var integraContact in contacts)
          {
            try
            {
              var fio = string.IsNullOrEmpty(integraContact.FIOPerson) ? integraContact.FIO : integraContact.FIOPerson;
              logtext += FormatLineForlog(string.Format("Загрузка контакта [{0}]", fio));
              
              var contactCompany = Etalon.Companies.GetAll().Where(x => x.ExternalCodeavis.Equals(integraContact.Org)).SingleOrDefault();
              if (contactCompany != null)
              {
                var contact = GetOrCreateContact(fio, Sungero.Parties.CompanyBases.As(contactCompany));
                if (contact != null)
                {
                  //При импорте Контактного лица для новых записей поле Персона останется пустым, для существующих редактироваться не будет.
                  //contact.Person = GetPerson(integraContact.FIOPerson, integraContact.DateOfBirthPerson);
                  // ФИО задать после указания Персоны, т.к. ФИО обновляется из нее. Если Персона не найдена, обязательное поле ФИО будет пустым.
                  //if (string.IsNullOrEmpty(contact.Name))
                  //{
                  //contact.Name = integraContact.FIOPerson;
                  //}
                  contact.Name = fio;
                  logtext += FormatLineForlog(string.Format("Контакт найден или создан [{0}] ид [{1}]", contact.Name, contact.Id));
                  contact.JobTitle = integraContact.Title;
                  contact.Department = integraContact.Podr;
                  contact.Phone = integraContact.Telephone;
                  contact.Fax = integraContact.Fax;
                  contact.Email = integraContact.Email;
                  contact.Note = integraContact.Prim;
                  if (integraContact.State)
                  {
                    contact.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
                  }
                  else
                  {
                    contact.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
                  }
                  
                  try
                  {
                    contact.Save();
                  }
                  catch (Exception ex)
                  {
                    logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось сохранить контакт: {0} {1}", ex.Message, ex.StackTrace));
                    //withError = true;
                  }
                }
                else
                {
                  // Не удалось корректно найти контакт
                  logtext += FormatLineForlog(string.Format("ОШИБКА Найдено больше одного контакта с ФИО {0} для компании с кодом {1}", integraContact.FIO, contactCompany.ExternalCodeavis));
                  //withError = true;
                }
              }
              else
              {
                // Не удалось найти Организацию
                logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось найти Организацию с кодом {0} для контакта ФИО {1}", contactCompany.ExternalCodeavis, integraContact.FIO));
                //withError = true;
              }
            }
            catch (InvalidOperationException)
            {
              // Найдено несколько Организаций контакта
              logtext += FormatLineForlog(string.Format("ОШИБКА Найдено несколько Организаций с кодом {0}, пропускаем контакт", integraContact.Org));
              //withError = true;
            }
          }
          #endregion
          
          Logger.Debug("Integration - ImportContractors - старт импорта контрагентов");
        }
      }
      catch (Exception ex)
      {
        withError = true;
        logtext += FormatLineForlog(string.Format("ОШИБКА при загрузке, загрузка прекращена: {0} {1}", ex.Message, ex.StackTrace));
        Logger.Error("Integration - ImportContractors - ", ex);
      }
      finally
      {
        // Сохраняем результат загрузки в справочник
        logtext += FormatLineForlog("Конец импорта контрагентов");
        try
        {
          runEntry.StartDate = startDate;
          runEntry.EndDate = Calendar.Now;
          runEntry.Success = !withError;
          runEntry.TextLog = logtext;
          settings.SyncDateTime = startDate;
          settings.Save();
        }
        catch(Exception ex)
        {
          Logger.ErrorFormat("Integration - ImportContractors - не удалось сохранить изменения в настройке интеграции {0} - ", ex, settings.Id);
        }
        // Очистим историю запусков
        ClearLog(settings);
      }
    }
    
    /// <summary>
    /// Получить существующий контакт по коду или создать новый, если еще нет.
    /// </summary>
    /// <param name="fio">ФИО контакта.</param>
    /// <param name="company">Организация контакта.</param>
    /// <returns>Контакт.</returns>
    private Etalon.IContact GetOrCreateContact(string fio, Sungero.Parties.ICompanyBase company)
    {
      // Ищем или создаем контакт.
      Etalon.IContact contact;
      try
      {
        contact = Etalon.Contacts.GetAll(x => x.Name.Trim().Equals(fio.Trim()) && Etalon.Companies.As(x.Company).Equals(company)).SingleOrDefault();
      }
      catch (InvalidOperationException)
      {
        return null;
      }
      if (contact == null)
      {
        contact = Etalon.Contacts.Create();
        contact.Company = company;
      }
      return contact;
    }
    
    /// <summary>
    /// Получить существующую персону по ФИО и дате рождения.
    /// </summary>
    /// <param name="fio">ФИО персоны</param>
    /// <param name="birthdate">Дата рождения персоны</param>
    /// <returns>Персона</returns>
    private Sungero.Parties.IPerson GetPerson(string fio, Nullable<DateTime> birthdate)
    {
      try
      {
        var person = Sungero.Parties.People.GetAll(p => Equals(p.Name, fio) && Equals(p.DateOfBirth, birthdate)).SingleOrDefault();
        return person;
      }
      catch(Exception)
      {
        return null;
      }
    }
    
    /// <summary>
    /// Получить регион по названию или создать новый, если с таким наименованием еще нет
    /// </summary>
    /// <param name="name">Название региона</param>
    /// <returns>Регион</returns>
    public override Sungero.Commons.IRegion GetOrCreateRegion(string name)
    {
      var region = Sungero.Commons.Regions.GetAll(j => Equals(j.Name, name)).FirstOrDefault();
      // Создаем, если не нашли
      if (region == null)
      {
        region = Sungero.Commons.Regions.Create();
        region.Name = name;
        region.Country = Sungero.Commons.Countries.GetAll(x => x.Code.Equals("643")).FirstOrDefault(); //TODO: отредактировать получение страны
        region.Save();
      }
      return region;
    }
    
    /// <summary>
    /// Получить нас. пункт по названию или создать новый, если с таким наименованием еще нет
    /// </summary>
    /// <param name="name">Название нас. пункта</param>
    /// <param name="region">Регион</param>
    /// <returns>Нас. пункт</returns>
    public override Sungero.Commons.ICity GetOrCreateCity(string name, Sungero.Commons.IRegion region)
    {
      var city = Sungero.Commons.Cities.GetAll(j => Equals(j.Name, name) && Equals(j.Region, region)).FirstOrDefault();
      // Создаем, если не нашли
      if (city == null)
      {
        city = Sungero.Commons.Cities.Create();
        city.Name = name;
        city.Region = region;
        city.Country = region.Country;
        city.Save();
      }
      return city;
    }

    /// <summary>
    /// Импорт оргструктуры из интеграционной таблицы
    /// </summary>
    public override void ImportOrgstructure()
    {
      var settings = Integrationses.GetAll(s => s.Code == avis.Integration.Constants.Module.OrgSructureImportRecordCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения");
      
      string logtext = FormatLineForlog("Старт импорта оргструктуры");
      logtext += FormatLineForlog("Инициализация подключения к интеграционной БД");
      string connectionString = Encryption.Decrypt(settings.ConnectionParams);
      if (string.IsNullOrEmpty(connectionString))
        throw AppliedCodeException.Create("Не удалось получить строку подключения");
      
      var settingId = settings.Id;
      var withError = false;
      DateTime startDate = Calendar.Now;
      DateTime? lastStartDate = settings.SyncDateTime;
      try
      {
        if (lastStartDate.HasValue)
        {
          logtext = logtext + FormatLineForlog(string.Format("Из интеграционной таблицы будут взяты данные от {0}", lastStartDate.Value.ToString()));
          Logger.DebugFormat("Integration - ImportOrgstructure - старт импорта оргструктуры от {0}", lastStartDate.Value.ToString());
        }
        else
        {
          Logger.Debug("Integration - ImportOrgstructure - старт импорта оргструктуры");
        }
        
        #region Данные для загрузки
        
        logtext = logtext + FormatLineForlog("Получаем список НОР, по которым будем загружать данные");
        var ourOrgs = BusinessUnits.GetAll().Where(u => u.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && u.TIN != null);
        logtext += FormatLineForlog(string.Format("Найдено НОР, по которым будем загружать данные: {0}", ourOrgs.Count()));
        
        List<AvisIntegrationHelper.Department> departmentsFull;
        if (lastStartDate.HasValue)
          departmentsFull = AvisIntegrationHelper.DataBaseHelper.GetDepartments(connectionString, lastStartDate.Value);
        else
          departmentsFull = AvisIntegrationHelper.DataBaseHelper.GetDepartments(connectionString);
        logtext += FormatLineForlog(string.Format("Найдено подразделений: {0}", departmentsFull.Count()));
        
        List<Worker> workersFull;
        if (lastStartDate.HasValue)
          workersFull = AvisIntegrationHelper.DataBaseHelper.GetWorkers(connectionString, lastStartDate.Value);
        else
          workersFull = AvisIntegrationHelper.DataBaseHelper.GetWorkers(connectionString);
        logtext += FormatLineForlog(string.Format("Найдено сотрудников: {0}", workersFull.Count()));
        
        #endregion
        
        #region Справочная информация для уведомления о закрытых сотрудниках
        
        // ИД групп и ролей, которые участвуют в действующих этапах согласования.
        var approvalStageGroupIds = new List<long>();
        var approvalStages = Sungero.Docflow.ApprovalStages.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
        approvalStageGroupIds.AddRange(approvalStages.Where(x => x.Assignee != null && Sungero.CoreEntities.Groups.Is(x.Assignee))
                                       .Select(x => x.Assignee.Id)
                                       .ToList());
        approvalStageGroupIds.AddRange(approvalStages.SelectMany(x => x.Recipients.Select(r => Sungero.CoreEntities.Groups.As(r.Recipient)))
                                       .Where(x => x != null)
                                       .Select(x => x.Id)
                                       .ToList());
        if (approvalStageGroupIds.Any())
        {
          approvalStageGroupIds = approvalStageGroupIds.Distinct().ToList();
        }
        var roleByJobTitleKindIds = EtalonDatabooks.JobTitleKinds.GetAll(x => x.Role != null)
          .Select(x => x.Role.Id)
          .ToList()
          .Distinct();
        // Группы и роли, которые участвуют в этапах согласования, не являются ролями по Виду должности, НОР или Подразделением.
        var groups = Sungero.CoreEntities.Groups.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
          .Where(x => (!Sungero.CoreEntities.Roles.Is(x) || Sungero.CoreEntities.Roles.Is(x) && Sungero.CoreEntities.Roles.As(x).IsSingleUser != true) &&
                 !Sungero.Company.BusinessUnits.Is(x) && !Sungero.Company.Departments.Is(x) &&
                 !roleByJobTitleKindIds.Contains(x.Id) && approvalStageGroupIds.Contains(x.Id));
        
        #endregion
        
        var countries = Sungero.Commons.Countries.GetAll();
        var identityDocumentKind = Sungero.Parties.IdentityDocumentKinds.GetAll(x => x.Status == Sungero.Parties.IdentityDocumentKind.Status.Active && x.Code == "21").FirstOrDefault();
        
        foreach (var businessUnit in ourOrgs)
        {
          Logger.DebugFormat("Integration - ImportOrgstructure - НОР [{0}] ИНН [{1}]", businessUnit.Name, businessUnit.TIN);
          try
          {
            Dictionary<long, AvisIntegrationHelper.Department> depsToUpdateLead = new Dictionary<long, AvisIntegrationHelper.Department>();
            Dictionary<long, AvisIntegrationHelper.Department> depsToUpdateManager= new Dictionary<long, AvisIntegrationHelper.Department>();
            List<Sungero.Company.IEmployee> closedEmployees = new List<Sungero.Company.IEmployee>();
            
            logtext += FormatLineForlog(string.Format("Загрузка для НОР [{0}] ИНН [{1}]", businessUnit.Name, businessUnit.TIN));
            
            #region Подразделения
            
            logtext += FormatLineForlog("Загрузка подразделений. Начало фильтирации подразделений");
            var departmentsList = departmentsFull.Where(d=>Equals(d.OrgCode,businessUnit.TIN));
            Logger.DebugFormat("Integration - ImportOrgstructure - всего подразделений для обработки {0}", departmentsList.Count());
            logtext += FormatLineForlog("Окончание фильтирации подразделений");
            
            Logger.Debug("Integration - ImportOrgstructure - загрузка подразделений");
            foreach (AvisIntegrationHelper.Department integraDep in departmentsList)
            {
              Transactions.Execute(
                () =>
                {
                  Logger.DebugFormat("Integration - ImportOrgstructure - подразделение [{0}] код [{1}]", integraDep.Name, integraDep.ExternalCode);
                  try
                  {
                    string externalCode = integraDep.ExternalCode.Trim();
                    logtext += FormatLineForlog(string.Format("Загрузка подразделения [{0}] код [{1}] ", integraDep.Name, externalCode));
                    var department = avis.EtalonIntergation.Departments.GetAll(d => Equals(d.BusinessUnit, businessUnit) && Equals(d.ExternalCodeavis, externalCode)).SingleOrDefault();
                    if (!integraDep.IsRemoved)
                    {
                      #region Подразделение действующее
                      
                      if (department == null)
                      {
                        logtext += FormatLineForlog("Подразделение не найдено, создаем новое");
                        department = avis.EtalonIntergation.Departments.Create();
                        department.ExternalCodeavis = externalCode;
                        department.BusinessUnit = businessUnit;
                      }
                      logtext += FormatLineForlog(string.Format("Подразделение найдено [{0}] ид [{1}], обновляем реквизиты", department.Name, department.Id));
                      department.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
                      department.ShortName = integraDep.Name;
                      department.Name = integraDep.Name;
                      
                      if (!string.IsNullOrEmpty(integraDep.LeadDepId))
                      {
                        logtext += FormatLineForlog(string.Format("Поиск головного подразделения по коду [{0}]", integraDep.LeadDepId));
                        var leadDep = avis.EtalonIntergation.Departments.GetAll(d => Equals(d.BusinessUnit, businessUnit) && Equals(d.ExternalCodeavis, integraDep.LeadDepId)).SingleOrDefault();
                        if (leadDep!=null)
                        {
                          logtext += FormatLineForlog(string.Format("Головное подразделение найдено: [{0}] ид [{1}]", leadDep.Name, leadDep.Id));
                          department.HeadOffice = leadDep;
                        }
                        else
                        {
                          logtext += FormatLineForlog("Подразделение не найдено, заполним после импорта всех подразделений");
                          depsToUpdateLead.Add(department.Id, integraDep);
                        }
                      }
                      else
                      {
                        department.HeadOffice = null;
                        logtext += FormatLineForlog("Головное подразделение очищено");
                      }
                      
                      if (!string.IsNullOrEmpty(integraDep.ManagerId))
                      {
                        var manager = GetExistedEmployee(integraDep.ManagerId, department.BusinessUnit);
                        if (manager != null)
                        {
                          logtext += FormatLineForlog(string.Format("Руководитель найден: [{0}], ид [{1}]", manager.Name, manager.Id));
                          department.Manager = manager;
                        }
                        else
                        {
                          logtext += FormatLineForlog("Руководитель не найден, заполним после импорта сотрудников");
                          depsToUpdateManager.Add(department.Id, integraDep);
                        }
                      }
                      else
                      {
                        department.Manager = null;
                        logtext += FormatLineForlog("Руководитель очищен");
                      }
                      
                      #endregion
                    }
                    else
                    {
                      #region Если помечен к удалению
                      
                      if (department!=null)
                      {
                        logtext += FormatLineForlog(string.Format("Подразделение найдено [{0}] ид [{1}], статус IsRemoved, закрываем", department.Name, department.Id));
                        department.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
                      }
                      
                      #endregion
                    }
                    if (department != null)
                    {
                      department.Save();
                    }
                  }
                  catch (Exception ex)
                  {
                    withError = true;
                    logtext += FormatLineForlog(string.Format("ОШИБКА при загрузке подразделения {0}: {1} {2}", integraDep.ExternalCode, ex.Message, ex.StackTrace));
                    Logger.Error("Integration - ImportOrgstructure - ", ex);
                  }
                });
            }
            
            #endregion
            
            # region Заполнение головных подразделений
            Logger.Debug("Integration - ImportOrgstructure - заполнение головных подразделений");
            
            logtext += FormatLineForlog(string.Format("Обновление подразделений, по которым не заполнено головное. Количество подразделений: {0}",depsToUpdateLead.Count));
            foreach (var depToUpdate in depsToUpdateLead)
            {
              Transactions.Execute(
                () =>
                {
                  var department =  avis.EtalonIntergation.Departments.Get(depToUpdate.Key);
                  Logger.DebugFormat("Integration - ImportOrgstructure - подразделение [{0}] ИД [{1}]", department.Name, department.Id);
                  try
                  {
                    logtext += FormatLineForlog(string.Format("Обновление головного подразделения у [{0}] ид [{1}])",department.Name,department.Id));
                    var leadDep =  avis.EtalonIntergation.Departments.GetAll(d=>Equals(d.BusinessUnit,businessUnit) && d.ExternalCodeavis==depToUpdate.Value.LeadDepId).SingleOrDefault();
                    if (leadDep!=null)
                    {
                      logtext += FormatLineForlog(string.Format("Головное подразделение найдено: [{0}] ид [{1}]", leadDep.Name, leadDep.Id));
                      department.HeadOffice = leadDep;
                      department.Save();
                    }
                    else
                      logtext += FormatLineForlog("Головное подразделение не найдено");
                  }
                  catch (Exception ex)
                  {
                    withError = true;
                    logtext += FormatLineForlog(string.Format("ОШИБКА при обновлении подразделения {0} {1}: {2} {3}", department.Id, department.Name, ex.Message, ex.StackTrace));
                    Logger.Error("Integration - ImportOrgstructure - ", ex);
                  }
                });
            }
            #endregion

            #region Сотрудники
            Logger.Debug("Integration - ImportOrgstructure - загрузка сотрудников");
            
            logtext += FormatLineForlog("Загрузка сотрудников. Начало фильтирации сотрудников");
            var workersList = workersFull.Where(d => Equals(d.OrgCode, businessUnit.TIN));
            logtext += FormatLineForlog("Окончание фильтирации сотрудников");
            
            foreach (Worker integraWorker in workersList)
            {
              Transactions.Execute(
                () =>
                {
                  Logger.DebugFormat("Integration - ImportOrgstructure - сотрудник [{0}]", integraWorker.ExternalCode);
                  try
                  {
                    string externalCode = integraWorker.ExternalCode;
                    logtext += FormatLineForlog(string.Format("Загрузка сотрудника с кодом [{0}]",externalCode ));
                    var worker = Etalon.Employees.GetAll(d => Equals(d.Department.BusinessUnit, businessUnit) && Equals(d.ExternalCodeavis,externalCode)).SingleOrDefault();
                    
                    // Если сотрудник не найден в НОР, то искать дополнительно в подразделении для закрытых сотрудников.
                    if (worker == null)
                    {
                      var idDepartmentForClosedEmployees = Convert.ToInt32(lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.DepartmentForClosedEmployees).FirstOrDefault().Value);
                      worker = Etalon.Employees.GetAll(d => d.ExternalCodeavis == externalCode && d.Department.Id == idDepartmentForClosedEmployees).SingleOrDefault();
                    }
                    
                    if (!integraWorker.IsRemoved)
                    {
                      #region Запись сотрудника помечена как действующая
                      try
                      {
                        var existedDepartment = avis.EtalonIntergation.Departments.GetAll(d=>Equals(d.BusinessUnit,businessUnit) && Equals(d.ExternalCodeavis,integraWorker.DepartmentCode)).SingleOrDefault();
                        if (existedDepartment == null)
                        {
                          logtext += FormatLineForlog("ОШИБКА Не найдено подразделение сотрудника");
                          withError = true;
                        }
                        else
                        {
                          logtext += FormatLineForlog(string.Format("Найдено подразделение сотрудника [{0}] ид [{1}]",existedDepartment.Name, existedDepartment.Id));
                          var integraPerson = integraWorker.Person;
                          
                          if (worker != null)
                          {
                            #region Редактирование сотрудника
                            
                            #region Персона пришла из Интегры
                            if (integraPerson != null)
                            {
                              Etalon.IPerson person = null;
                              var errorMessage = string.Empty;
                              var equalsPersons = Etalon.People.GetAll(p => Equals(p.FirstName, integraPerson.Fistname)
                                                                       && ((integraPerson.Secondname != null && integraPerson.Secondname != string.Empty) && Equals(p.MiddleName, integraPerson.Secondname)
                                                                           || (integraPerson.Secondname == null || integraPerson.Secondname == string.Empty) && (Equals(p.MiddleName, null) || Equals(p.MiddleName, string.Empty)))
                                                                       && Equals(p.LastName, integraPerson.Surname)
                                                                       && Equals(p.DateOfBirth, integraPerson.Birthdate));
                              // Если не найдена персона по ФИО + ДР, искать по ФИО + ДР 01.01.1900
                              if (!equalsPersons.Any())
                              {
                                var defaultDateOfBirth = Calendar.GetDate(1900, 1, 1);
                                equalsPersons = Etalon.People.GetAll(p => Equals(p.FirstName, integraPerson.Fistname)
                                                                     && ((integraPerson.Secondname != null && integraPerson.Secondname != string.Empty) && Equals(p.MiddleName, integraPerson.Secondname)
                                                                         || (integraPerson.Secondname == null || integraPerson.Secondname == string.Empty) && (Equals(p.MiddleName, null) || Equals(p.MiddleName, string.Empty)))
                                                                     && Equals(p.LastName, integraPerson.Surname)
                                                                     && Equals(p.DateOfBirth, defaultDateOfBirth));
                              }
                              if (equalsPersons.Count() == 1)
                              {
                                person = equalsPersons.FirstOrDefault();
                                errorMessage = string.Empty;
                              }
                              if (equalsPersons.Count() == 0)
                              {
                                errorMessage = "Не удалось найти Персону с указанными данными.";
                              }
                              if (equalsPersons.Count() > 1)
                              {
                                errorMessage = "Найдено больше 1 персоны.";
                              }
                              if (!string.IsNullOrEmpty(errorMessage) && worker.Person != null && !Equals(worker.Person, person))
                              {
                                var asyncNotification = AsyncHandlers.AsyncImportOrgStructureNotificationavis.Create();
                                asyncNotification.Subject = $"Проверить, почему не совпадают данные у персоны {worker.Person.ShortName} и переданной из 1С ЗУП.";
                                asyncNotification.ActiveText = $"Данные переданной персоны - {integraPerson.Surname} {integraPerson.Fistname} {integraPerson.Secondname}, {integraPerson.Birthdate.Value.ToString("d")}.";
                                if (!string.IsNullOrEmpty(errorMessage))
                                {
                                  asyncNotification.ActiveText += " " + errorMessage;
                                }
                                asyncNotification.PersonId = worker.Person.Id;
                                asyncNotification.ExecuteAsync();
                              }
                              else if (person != null)
                              {
                                logtext += FormatLineForlog(string.Format("Обновляем реквизиты персоны [{0}] ид [{1}]", person.Name, person.Id));
                                // 0 - Женский, 1 - Мужской
                                person.Sex = (integraPerson.Sex=="0"? Sungero.Parties.Person.Sex.Female : Sungero.Parties.Person.Sex.Male);
                                person.TIN = integraPerson.INN;
                                person.LegalAddress = integraPerson.Regadress;
                                person.PostalAddress = integraPerson.Factadress;
                                person.IsEmployeeGKavis = true;
                                person.INILA = integraPerson.SNILS;
                                if (!string.IsNullOrEmpty(integraPerson.Citizenship))
                                  person.Citizenship = countries.FirstOrDefault(x => x.Code == integraPerson.Citizenship.Trim());
                                else
                                  person.Citizenship = null;
                                
                                // Паспортные данные.
                                if (!string.IsNullOrEmpty(integraPerson.IdentitySeries) && !string.IsNullOrEmpty(integraPerson.IdentityNumber))
                                  person.IdentityKind = identityDocumentKind;
                                else
                                  person.IdentityKind = null;
                                
                                person.IdentitySeries = integraPerson.IdentitySeries;
                                person.IdentityNumber = integraPerson.IdentityNumber;
                                person.IdentityDateOfIssue = integraPerson.IdentityDateOfIssue;
                                person.IdentityAuthority = integraPerson.IdentityAuthority;
                                person.IdentityAuthorityCode = integraPerson.IdentityAuthorityCode;
                                person.BirthPlace = integraPerson.BirthPlace;
                                person.LegalAddress = integraPerson.RegistrationAddress;
                                
                                // Если документ, удостоверяющий личность не соответствует формату, то отправить уведомление (после сохранения) и очистить вкладку.
                                var identityDocumentErrors = lenspec.Etalon.PublicFunctions.Person.CheckIdentityProperties(person);
                                if (identityDocumentErrors.Any())
                                {
                                  person.IdentityKind = null;
                                  person.IdentitySeries = null;
                                  person.IdentityNumber = null;
                                  person.IdentityDateOfIssue = null;
                                  person.IdentityAuthority = null;
                                  person.IdentityAuthorityCode = null;
                                  person.BirthPlace = null;
                                }
                                try
                                {
                                  person.Save();
                                  if (identityDocumentErrors.Any())
                                  {
                                    var asyncNotification = AsyncHandlers.AsyncImportOrgStructureNotificationavis.Create();
                                    asyncNotification.Subject = $"У созданной персоны {person.Name} не заполнились паспортные данные";
                                    asyncNotification.ActiveText = string.Join(Environment.NewLine, identityDocumentErrors.Select(x => x.Value).ToList());
                                    asyncNotification.PersonId = person.Id;
                                    asyncNotification.ExecuteAsync();
                                  }
                                }
                                catch (Exception ex)
                                {
                                  logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось сохранить персону: {0} {1}", ex.Message,ex.StackTrace));
                                  withError = true;
                                }
                              }
                            }
                            #endregion
                            
                            logtext += FormatLineForlog(string.Format("Обновляем реквизиты сотрудника [{0}] ид [{1}]", worker.Person !=null ? worker.Person.Name : string.Empty, worker.Id));
                            worker.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
                            worker.Department = existedDepartment;
                            worker.Phone = integraWorker.Phone;
                            worker.Email = integraWorker.Email;
                            worker.PersonnelNumber = integraWorker.Tabnum;
                            worker.CivilLawContractlenspec = integraWorker.GPH;
                            worker.ContractValidFromlenspec = integraWorker.GPHDateBegin;
                            worker.ContractValidTilllenspec = integraWorker.GPHDateEnd;
                            if (!string.IsNullOrEmpty(integraWorker.Position))
                            {
                              var jobTitle = GetOrCreateJobTitle(integraWorker.Position.Trim());
                              worker.JobTitle = jobTitle;
                            }
                            else
                              worker.JobTitle = null;
                            
                            if (integraWorker.PosType != null)
                              worker.JobTitleKindlenspec = EtalonDatabooks.JobTitleKinds.GetAll(x => x.ExternalCode != null && x.ExternalCode == integraWorker.PosType).SingleOrDefault();
                            else
                              worker.JobTitleKindlenspec = null;
                            
                            if (integraWorker.MainPlaceOfWork != null)
                              worker.PlaceOfWorkavis = EtalonDatabooks.PlaceOfWorks.GetAll(x => x.Code1C != null && x.Code1C == integraWorker.MainPlaceOfWork).SingleOrDefault();
                            else
                              worker.PlaceOfWorkavis = null;
                            
                            if (string.IsNullOrEmpty(worker.Email))
                            {
                              worker.NeedNotifyAssignmentsSummary = false;
                              worker.NeedNotifyExpiredAssignments = false;
                              worker.NeedNotifyNewAssignments = false;
                            }
                            
                            if (integraWorker.IsParentalLeave)
                            {
                              worker.IsMaternityLeavelenspec = Employee.IsMaternityLeavelenspec.Yes;
                              worker.Status = Employee.Status.Closed;
                            }
                            else
                              worker.IsMaternityLeavelenspec = Employee.IsMaternityLeavelenspec.No;
                            
                            try
                            {
                              worker.Save();
                            }
                            catch (Exception ex)
                            {
                              logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось сохранить сотрудника: {0} {1}", ex.Message,ex.StackTrace));
                              withError = true;
                            }
                            #endregion
                          }
                          
                          else
                          {
                            #region Создание нового сотрудника
                            if (integraPerson == null)
                            {
                              logtext += FormatLineForlog("ОШИБКА для нового сотрудника отсутствует Персона");
                            }
                            else
                            {
                              string personCode = integraPerson.ExternalCode.Trim();
                              // Ищем или создаем персону
                              var person = GetOrCreatePerson(integraPerson.Fistname, integraPerson.Secondname, integraPerson.Surname, integraPerson.Birthdate);
                              if (person != null)
                              {
                                logtext += FormatLineForlog(string.Format("Персона найдена или создана [{0}] ид [{1}]", person.Name, person.Id));
                                //Признак, что персона отработана корректно
                                bool personProcessed = true;
                                // 0 - Женский, 1 - Мужской
                                // На всякий случай, чтобы не было ошибки, другие варианты тоже заполняем значением по умолчанию
                                person.Sex = (integraPerson.Sex=="0"? Sungero.Parties.Person.Sex.Female : Sungero.Parties.Person.Sex.Male);
                                person.TIN = integraPerson.INN;
                                person.LegalAddress = integraPerson.Regadress;
                                person.PostalAddress = integraPerson.Factadress;
                                person.IsEmployeeGKavis = true;
                                person.INILA = integraPerson.SNILS;
                                if (!string.IsNullOrEmpty(integraPerson.Citizenship))
                                  person.Citizenship = countries.FirstOrDefault(x => x.Code == integraPerson.Citizenship.Trim());
                                else
                                  person.Citizenship = null;
                                
                                // Паспортные данные.
                                if (!string.IsNullOrEmpty(integraPerson.IdentitySeries) && !string.IsNullOrEmpty(integraPerson.IdentityNumber))
                                  person.IdentityKind = identityDocumentKind;
                                else
                                  person.IdentityKind = null;
                                
                                person.IdentitySeries = integraPerson.IdentitySeries;
                                person.IdentityNumber = integraPerson.IdentityNumber;
                                person.IdentityDateOfIssue = integraPerson.IdentityDateOfIssue;
                                person.IdentityAuthority = integraPerson.IdentityAuthority;
                                person.IdentityAuthorityCode = integraPerson.IdentityAuthorityCode;
                                person.BirthPlace = integraPerson.BirthPlace;
                                person.LegalAddress = integraPerson.RegistrationAddress;
                                
                                // Если документ, удостоверяющий личность не соответствует формату, то отправить уведомление (после сохранения) и очистить вкладку.
                                var identityDocumentErrors = lenspec.Etalon.PublicFunctions.Person.CheckIdentityProperties(person);
                                if (identityDocumentErrors.Any())
                                {
                                  person.IdentityKind = null;
                                  person.IdentitySeries = null;
                                  person.IdentityNumber = null;
                                  person.IdentityDateOfIssue = null;
                                  person.IdentityAuthority = null;
                                  person.IdentityAuthorityCode = null;
                                  person.BirthPlace = null;
                                }
                                
                                try
                                {
                                  person.Save();
                                  if (identityDocumentErrors.Any())
                                  {
                                    var asyncNotification = AsyncHandlers.AsyncImportOrgStructureNotificationavis.Create();
                                    asyncNotification.Subject = $"У созданной персоны {person.Name} не заполнились паспортные данные";
                                    asyncNotification.ActiveText = string.Join(Environment.NewLine, identityDocumentErrors.Select(x => x.Value).ToList());
                                    asyncNotification.PersonId = person.Id;
                                    asyncNotification.ExecuteAsync();
                                  }
                                }
                                catch (Exception ex)
                                {
                                  logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось сохранить персону: {0} {1}", ex.Message,ex.StackTrace));
                                  personProcessed = false;
                                  withError = true;
                                }
                                
                                if (personProcessed)
                                {
                                  logtext += FormatLineForlog("Сотрудник не найден, создаем нового");
                                  // Создаем нового сотрудника
                                  worker = Etalon.Employees.Create();
                                  worker.Person = person;
                                  // Заполняем данные
                                  logtext += FormatLineForlog(string.Format("Заполняем реквизиты сотрудника [{0}] ид [{1}]", worker.Person != null ? worker.Person.Name : string.Empty, worker.Id));
                                  worker.ExternalCodeavis = externalCode;
                                  worker.NeedNotifyExpiredAssignments = false;
                                  worker.NeedNotifyNewAssignments = false;
                                  worker.NeedNotifyAssignmentsSummary = false;
                                  worker.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
                                  worker.Department = existedDepartment;
                                  worker.Phone = integraWorker.Phone;
                                  worker.Email = integraWorker.Email;
                                  worker.PersonnelNumber = integraWorker.Tabnum;
                                  worker.CivilLawContractlenspec = integraWorker.GPH;
                                  worker.ContractValidFromlenspec = integraWorker.GPHDateBegin;
                                  worker.ContractValidTilllenspec = integraWorker.GPHDateEnd;
                                  
                                  if (!string.IsNullOrEmpty(integraWorker.Position))
                                  {
                                    var jobTitle = GetOrCreateJobTitle(integraWorker.Position.Trim());
                                    worker.JobTitle = jobTitle;
                                  }
                                  else
                                    worker.JobTitle = null;
                                  
                                  if (integraWorker.PosType != null)
                                    worker.JobTitleKindlenspec = EtalonDatabooks.JobTitleKinds.GetAll(x => x.ExternalCode != null && x.ExternalCode == integraWorker.PosType).SingleOrDefault();
                                  else
                                    worker.JobTitleKindlenspec = null;
                                  
                                  if (integraWorker.MainPlaceOfWork != null)
                                    worker.PlaceOfWorkavis = EtalonDatabooks.PlaceOfWorks.GetAll(x => x.Code1C != null && x.Code1C == integraWorker.MainPlaceOfWork).SingleOrDefault();
                                  else
                                    worker.PlaceOfWorkavis = null;
                                  
                                  if (integraWorker.IsParentalLeave)
                                  {
                                    worker.IsMaternityLeavelenspec = Employee.IsMaternityLeavelenspec.Yes;
                                    worker.Status = Employee.Status.Closed;
                                  }
                                  else
                                    worker.IsMaternityLeavelenspec = Employee.IsMaternityLeavelenspec.No;
                                  
                                  try
                                  {
                                    worker.Save();
                                  }
                                  catch (Exception ex)
                                  {
                                    logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось сохранить сотрудника: {0} {1}", ex.Message, ex.StackTrace));
                                    withError = true;
                                  }
                                }
                              }
                              else
                              {
                                // Не удалось корректно найти персону
                                logtext += FormatLineForlog(string.Format("ОШИБКА Найдено больше одной персоны с кодом {0}", personCode));
                                withError = true;
                              }
                            }
                            #endregion
                          }
                        }
                      }
                      catch (InvalidOperationException)
                      {
                        // Найдено несколько подразделений
                        logtext += FormatLineForlog("ОШИБКА Найдено несколько подразделений, пропускаем сотрудника");
                        withError = true;
                      }
                      #endregion
                    }
                    else
                    {
                      #region Запись сотрудника помечена на удаление
                      if (worker != null)
                      {
                        logtext += FormatLineForlog(string.Format("Сотрудник найден [{0}] ид [{1}], статус IsRemoved, закрываем", worker.Person != null ? worker.Person.Name : string.Empty, worker.Id));
                        try
                        {
                          #region Уведомление о группах, в которых участвовал закрытый сотрудник (найти вхождения в группы до сохранения статуса Закрытой записи)
                          var employeeGroups = string.Empty;
                          var groupsByMember = groups.Where(x => x.RecipientLinks.Any(r => r.Member.Equals(worker)));
                          foreach(var group in groupsByMember)
                          {
                            employeeGroups += $"\r\n{Hyperlinks.Get(group)}";
                          }
                          if (!string.IsNullOrEmpty(employeeGroups))
                          {
                            employeeGroups = " Группы и роли, в которых участвовал сотрудник:" + employeeGroups;
                          }
                          
                          var asyncNotification = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncClosedEmployeeNotificationavis.Create();
                          asyncNotification.EmployeeId = worker.Id;
                          asyncNotification.EmployeeGroups = employeeGroups;
                          asyncNotification.ExecuteAsync();
                          #endregion
                          
                          if (integraWorker.IsParentalLeave)
                            worker.IsMaternityLeavelenspec = Employee.IsMaternityLeavelenspec.Yes;
                          worker.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
                          worker.Save();
                          //Добавляем в список уволенных
                          closedEmployees.Add(worker);
                          
                          var activeEmployees = Sungero.Company.Employees.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && worker.Person.Id == x.Person.Id);
                          if (!activeEmployees.Any())
                          {
                            var asyncHandler = lenspec.Etalon.Module.Company.AsyncHandlers.AsyncUpdatingPersonOfFiredEmployeelenspec.Create();
                            asyncHandler.PersonId = worker.Person.Id;
                            asyncHandler.IsFillNote = true;
                            asyncHandler.ExecuteAsync();
                          }
                        }
                        catch (Exception ex)
                        {
                          logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось сохранить сотрудника: {0} {1}", ex.Message,ex.StackTrace));
                          withError = true;
                        }
                      }
                      #endregion
                    }
                  }
                  catch (Exception ex)
                  {
                    withError = true;
                    logtext += FormatLineForlog(string.Format("ОШИБКА при загрузке сотрудника {0}: {1} {2}", integraWorker.ExternalCode, ex.Message, ex.StackTrace));
                    Logger.Error("Integration - ImportOrgstructure - ", ex);
                  }
                });
            }
            
            #endregion
            
            #region Заполнение руководителей по подразделениям
            Logger.Debug("Integration - ImportOrgstructure - заполнение руководителей по подразделениям");
            
            logtext += FormatLineForlog(string.Format("Обновление подразделений, по которым не заполнено головное. Количество подразделений: {0}",depsToUpdateManager.Count));
            foreach (var depToUpdate in depsToUpdateManager)
            {
              Transactions.Execute(
                () =>
                {
                  var department =  avis.EtalonIntergation.Departments.Get(depToUpdate.Key);
                  Logger.DebugFormat("Integration - ImportOrgstructure - подразделение [{0}] ИД [{1}]", department.Name, department.Id);
                  try
                  {
                    logtext += FormatLineForlog(string.Format("Обновление подразделения [{0}] Код руководителя: [{1}]",department.Name,depToUpdate.Value.ManagerId));
                    var manager = Etalon.Employees.GetAll(d => Equals(d.Department.BusinessUnit, businessUnit) && Equals(d.ExternalCodeavis, depToUpdate.Value.ManagerId)).SingleOrDefault();
                    if (manager!=null)
                    {
                      logtext += FormatLineForlog(string.Format("Руководитель найден: [{0}] ид [{1}]",manager.Name, manager.Id));
                      department.Manager = manager;
                      department.Save();
                    }
                    else
                      logtext += FormatLineForlog("Не удалось определить руководителя");
                  }
                  catch(Exception ex)
                  {
                    withError = true;
                    logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось обновить подразделение {0} {1}: {2} {3}", department.Id, department.Name, ex.Message, ex.StackTrace));
                    Logger.Error("Integration - ImportOrgstructure - ", ex);
                  }
                });
            }
            # endregion
            
            #region Очистка уволенных руководителей из карточки подразделения и отправка уведомлений по ним
            Logger.Debug("Integration - ImportOrgstructure - очистка уволенных руководителей из карточки подразделения и отправка уведомлений по ним");
            
            var depsToClearManager = avis.EtalonIntergation.Departments.GetAll(d=>Equals(d.BusinessUnit,businessUnit) && closedEmployees.Contains(d.Manager)).ToList();
            logtext += FormatLineForlog(string.Format("Обновление подразделений с уволенными руководителями. Найдено подразделений: {0}",depsToClearManager.Count()));
            foreach (var depToUpdate in depsToClearManager)
            {
              Transactions.Execute(
                () =>
                {
                  var department =  avis.EtalonIntergation.Departments.Get(depToUpdate.Id);
                  Logger.DebugFormat("Integration - ImportOrgstructure - подразделение [{0}] ИД [{1}]", department.Name, department.Id);
                  try
                  {
                    logtext += FormatLineForlog(string.Format("Очистка руководителя у подразделения [{0}] ид [{1}]", department.Name, department.Id));
                    department.Manager = null;
                    department.Save();
                  }
                  catch (Exception ex)
                  {
                    withError = true;
                    logtext += FormatLineForlog(string.Format("ОШИБКА Не удалось обновить подразделение {0} {1}: {2} {3}", department.Id, department.Name, ex.Message, ex.StackTrace));
                    Logger.ErrorFormat("Integration - ImportOrgstructure - ", ex);
                  }
                });
            }
            
            bool isCEOClosed = closedEmployees.Exists(e=>Equals(e,businessUnit.CEO));
            var role = Roles.GetAll(r => r.Sid == avis.Integration.Constants.Module.Integration1CResponsibleID).FirstOrDefault();
            if ((isCEOClosed || depsToClearManager.Count()>0) && role!=null && Roles.GetAllUsersInGroup(role).Count()>0)
            {
              var task = Sungero.Workflow.SimpleTasks.Create();
              task.Subject = string.Format("Обработка ошибок импорта данных из 1С по {0}", businessUnit.Name);
              var taskStep = task.RouteSteps.AddNew();
              taskStep.AssignmentType = Sungero.Workflow.SimpleTask.AssignmentType.Notice;
              taskStep.Performer = role;
              task.ActiveText = "По вложенным записям был уволен руководитель, необходимо скорректировать руководителя в соответствующих карточках";
              foreach (var dep in depsToClearManager)
              {
                task.Attachments.Add(dep);
              }
              
              if (isCEOClosed)
                task.Attachments.Add(businessUnit);
              task.Start();
            }
            #endregion
          }
          catch(Exception ex)
          {
            withError = true;
            logtext += FormatLineForlog(string.Format("ОШИБКА при загрузке НОР {0} {1}: {2} {3}", businessUnit.TIN, businessUnit.Name, ex.Message, ex.StackTrace));
            Logger.Error("Integration - ImportOrgstructure - ", ex);
          }
        }
        Logger.Debug("Integration - ImportOrgstructure - конец импорта оргструктуры");
      }
      catch (Exception ex)
      {
        withError = true;
        logtext += FormatLineForlog(string.Format("ОШИБКА при загрузке, загрузка прекращена: {0} {1}", ex.Message, ex.StackTrace));
        Logger.Error("Integration - ImportOrgstructure - ", ex);
      }
      finally
      {
        logtext += FormatLineForlog("Конец импорта оргструктуры");
        
        #region Сохраняем результат загрузки в справочник
        
        Logger.DebugFormat("Integration - ImportOrgstructure - сохранение информации о последнем запуске, длина лога {0}, ИД настройки {1}", logtext.Length, settingId);
        var asyncSaveResult = AsyncHandlers.AsyncImportOrgStructureSaveResultlenspec.Create();
        asyncSaveResult.SettingId = settingId;
        asyncSaveResult.StartDate = startDate;
        asyncSaveResult.EndDate = Calendar.Now;
        asyncSaveResult.Success = !withError;
        asyncSaveResult.TextLog = logtext;
        asyncSaveResult.ExecuteAsync();
        
        #endregion
      }
    }
    
    /// <summary>
    /// Получить существующую персону по ФИО и дате рождения.
    /// </summary>
    /// <param name="firstName">Имя персоны</param>
    /// <param name="secondName">Отчество персоны</param>
    /// <param name="surname">Фамилия персоны</param>
    /// <param name="birthdate">Дата рождения персоны</param>
    /// <returns>Персона, если удалось найти запись с указанными параметрами, иначе null.</returns>
    private Etalon.IPerson GetPerson(string firstName, string secondName, string surname, Nullable<DateTime> birthdate)
    {
      try
      {
        var person = Etalon.People.GetAll(p => Equals(p.FirstName, firstName)
                                          && ((secondName != null && secondName != string.Empty) && Equals(p.MiddleName, secondName)
                                              || (secondName == null || secondName == string.Empty) && (Equals(p.MiddleName, null) || Equals(p.MiddleName, string.Empty)))
                                          && Equals(p.LastName, surname)
                                          && Equals(p.DateOfBirth, birthdate)).SingleOrDefault();
        return person;
      }
      catch (InvalidOperationException)
      {
        return null;
      }
    }
    
    /// <summary>
    /// Получить существующую персону по ФИО и дате рождения.
    /// </summary>
    /// <param name="firstName">Имя персоны</param>
    /// <param name="secondName">Отчество персоны</param>
    /// <param name="surname">Фамилия персоны</param>
    /// <param name="birthdate">Дата рождения персоны</param>
    /// <param name="person">Персона.</param>
    /// <returns>Текст ошибки, либо null.</returns>
    private string GetPersonErrorMessage(string firstName, string secondName, string surname, Nullable<DateTime> birthdate, Etalon.IPerson person)
    {
      try
      {
        var persons = Etalon.People.GetAll(p => Equals(p.FirstName, firstName)
                                           && ((secondName != null && secondName != string.Empty) && Equals(p.MiddleName, secondName)
                                               || (secondName == null || secondName == string.Empty) && (Equals(p.MiddleName, null) || Equals(p.MiddleName, string.Empty)))
                                           && Equals(p.LastName, surname)
                                           && Equals(p.DateOfBirth, birthdate));
        if (persons.Count() == 0)
          return "Не удалось найти Персону с указанными данными.";
        if (persons.Count() > 1)
          return "Найдено больше 1 персоны.";
        
        person = persons.FirstOrDefault();
        return string.Empty;
      }
      catch (Exception ex)
      {
        return string.Format("Ошибка при поиске Персоны: {0}", ex.Message);
      }
    }
    
    /// <summary>
    /// Получить существующую персону по ФИО и дате рождения или создать новую, если еще нет
    /// </summary>
    /// <param name="firstName">Имя персоны</param>
    /// <param name="secondName">Отчество персоны</param>
    /// <param name="surname">Фамилия персоны</param>
    /// <param name="birthdate">Дата рождения персоны</param>
    /// <returns>Персона</returns>
    private Etalon.IPerson GetOrCreatePerson(string firstName, string secondName, string surname, Nullable<DateTime> birthdate)
    {
      // Искать по переданным ФИО и ДР.
      var defaultDateOfBirth = Calendar.GetDate(1900, 1, 1);
      var person = Etalon.People.GetAll(p => Equals(p.FirstName, firstName)
                                        && ((secondName != null && secondName != string.Empty) && Equals(p.MiddleName, secondName)
                                            || (secondName == null || secondName == string.Empty) && (Equals(p.MiddleName, null) || Equals(p.MiddleName, string.Empty)))
                                        && Equals(p.LastName, surname)
                                        && Equals(p.DateOfBirth, birthdate)).SingleOrDefault();
      // Если персона не найдена, то искать по переданным ФИО и ДР = 01.01.1900.
      if (person == null)
      {
        person = Etalon.People.GetAll(p => Equals(p.FirstName, firstName)
                                      && ((secondName != null && secondName != string.Empty) && Equals(p.MiddleName, secondName)
                                          || (secondName == null || secondName == string.Empty) && (Equals(p.MiddleName, null) || Equals(p.MiddleName, string.Empty)))
                                      && Equals(p.LastName, surname)
                                      && Equals(p.DateOfBirth, defaultDateOfBirth)).SingleOrDefault();
      }
      // Если персона не найдена, то создать новую.
      if (person == null)
      {
        person = Etalon.People.Create();
        person.FirstName = firstName;
        person.MiddleName = secondName;
        person.LastName = surname;
        person.DateOfBirth = birthdate;
        person.IsEmployeeGKavis = true;
      }
      return person;
    }
    
    /// <summary>
    /// Очистить историю старых запусков (оставить последние 10)
    /// </summary>
    public virtual void ClearLog(Etalon.IIntegrations setting)
    {
      int maxEntities = 10;
      var runHistory = setting.RunHistory;
      int index = 0;
      foreach (var hist in runHistory.OrderByDescending(d=>d.StartDate))
      {
        index++;
        if (index>maxEntities)
          setting.RunHistory.Remove(hist);
      }
      
      setting.Save();
    }
    
    /// <summary>
    /// Импорт пользователей из Active Directory в IntegraDB.
    /// </summary>
    public virtual void ImportEmployeeActiveDirectoryavis()
    {
      //Ищем настройки подключения
      var setting = lenspec.Etalon.Module.Integration.Server.ModuleFunctions.GetIntegrationKFSettingsRec(Constants.Module.ActiveDirectoryEmployeeRecordCode);
      
      if (setting == null)
        throw AppliedCodeException.Create("Не найдены настройки подключения к интеграционной БД");
      if (string.IsNullOrEmpty(setting.ConnectionParams))
        throw AppliedCodeException.Create("Строка подключения к интеграционной БД пуста.");
      
      
      // Получаем параметры подключения к ад и БД
      var settingString = Encryption.Decrypt(setting.ConnectionParams);
      var settings = settingString.Split('|');
      if (settings.Length != 4)
      {
        throw AppliedCodeException.Create($"Не все настройки указаны в строке подключения.");
      }
      
      // Создаем запись в истории загрузки
      var runEntry = setting.RunHistory.AddNew();
      runEntry.StartDate = Calendar.Now;
      
      var adHelper = new ADHelper();
      // Получаем список пользователей.
      //List<AvisExpert.EtalonIntegraionHelper.Models.EmployeeADModel> adUsers;
      List<AvisIntegrationHelper.Models.EmployeeADModel> adUsers;
      try
      {
        adUsers = adHelper.GetUsersAD(settings[1],settings[2],settings[3]);
      }
      catch
      {
        throw AppliedCodeException.Create("Проблема при получении пользователей из AD.");
      }

      // Заполняем пользователей в БД
      try
      {
        var employeeADRepository = new EmployeeADRepository(settings[0]);

        foreach(var user in adUsers)
        {
          // Проверяем что у пользователя не нулевой логин. У некоторых найденных учетоков в АД вместо логина пустота.
          if (string.IsNullOrEmpty(user.Login))
          {
            continue;
          }

          var employer = employeeADRepository.GetByGuid(user.Sid);
          
          // Если пользователя не существует в таблице.
          if (employer == null)
          {
            employeeADRepository.Add(user);
            continue;
          }

          user.Id = employer.Id;

          if (employer != user)
          {
            employeeADRepository.Edit(user);
          }
        }
      }
      catch
      {
        throw AppliedCodeException.Create("Проблема при записи в БД.");
      }
      
      //throw AppliedCodeException.Create($"Выполнено (Количество найденных учеток: {adUsers.Count})(Интераций: {count})");
    }
    
    //конец Добавлено Avis Expert
  }
}