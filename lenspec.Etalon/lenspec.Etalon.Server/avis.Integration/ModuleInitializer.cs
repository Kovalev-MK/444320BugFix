using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.Etalon.Module.Integration.Server
{
  public partial class ModuleInitializer
  {
    public override bool IsModuleVisible()
    {
      return Users.Current.IncludedIn(Roles.Administrators);
    }

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      try
      {
        base.Initializing(e);
        InitializationLogger.Debug("Init: Create Kontur.Focus integration settings");
        CreateKonturFocusEGRULIntegrationSettings();
        CreateActiveDirectoryEmployeeIntegrationSettings();
        CreateInvestRabbitMqIntegrationSettings();
        CreateDadataIntegrationSettings();
        CreateGisClientInfoIntegrationSettings();
        CreateNatsIntegrationSettings();
        CreateCRMIntegrationSettings();
        CreateKBKIntegrationSettings();
        CreateConstractionSitesIntegrationSettings();
        CreateLKIntegrationSettings();
        CreatePKPIntegrationSettings();
        CreateTemplateCustomerRequestSettings();
        CreateWorkTypesIntegrationSettings();
        CreateAPIFnsSettings();

        // Справочники.
        GrantRightsOnDatabooks();
      }
      catch(Exception)
      {
        
      }
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateKonturFocusEGRULIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for Kontur.Focus integration");
      //Проверяем есть ли настройка для интеграции с API Контур.Фокус
      string settingsCode = Constants.Module.KonturFocusEGRULRecordCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Информация из ЕРГЮЛ (Контур.Фокус)";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: https://focus-api.kontur.ru/api3/{0}key=ключ_доступа_к_API{1}\r\nДля корректного подключения к сервису Контур.Фокус рекомендуется изменять только значение ключ_доступа_к_API.";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateActiveDirectoryEmployeeIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for ActiveDirectory Employee integration");
      //Проверяем есть ли настройка для интеграции с API Контур.Фокус
      string settingsCode = Constants.Module.ActiveDirectoryEmployeeRecordCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Информация из ad, со списком пользователей.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: Строка подключения к БД|LDAP://OU=Users,DC=avis,DC=local|LoginAD|PasswordAD\r\n" +
          "Пример: Initial Catalog=IntegraDB;server=localhost;User ID=directumRX;Password=123|LDAP://OU=Users,DC=domain,DC=ru|LoginAD|PasswordAD";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateInvestRabbitMqIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for Invest rabbit mq integration");
      //Проверяем есть ли настройка для интеграции с инвест.
      var settingsCode = Constants.Module.InvestConnectRabbitMQCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к rabbit mq Invest.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: Строка подключения к rabbit Login|Password|VirtualHost|IPAddress\r\n" +
          "Пример: Login|Password|TestHost|127.0.0.1";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateDadataIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for dadata integration");
      // Проверяем есть ли настройка для интеграции с dadata.
      var settingsCode = Constants.Module.DadataCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к Dadata.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: token;secretToken";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateGisClientInfoIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for GisClientInfo integration");
      // Проверяем есть ли настройка для интеграции с GisClientInfo.
      var settingsCode = Constants.Module.GisClientInfoCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к GisClientInfo.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: строка подключения к БД.";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateNatsIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for Nats integration");
      // Проверяем есть ли настройка для интеграции с Nats.
      var settingsCode = Constants.Module.NatsCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к Nats.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: connectionString;PathNats";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateCRMIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for CRM integration");
      // Проверяем есть ли настройка для интеграции с Nats.
      var settingsCode = Constants.Module.CRMCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к CRM.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: url";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateLKIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for LK integration");
      // Проверяем есть ли настройка для интеграции с Nats.
      var settingsCode = Constants.Module.LKCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к промежуточному хранилищу LK Minio.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: endpoint;accessKey;secretKey;bucketNameGet;bucketNamePut";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreatePKPIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for PKP integration");
      // Проверяем есть ли настройка для интеграции с Nats.
      var settingsCode = Constants.Module.PkpCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к промежуточному хранилищу PKP Minio.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: endpoint;accessKey;secretKey;bucketNameGet;bucketNamePut";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек шаблона для обращения клиента.
    /// </summary>
    public void CreateTemplateCustomerRequestSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for TemplateCustomerRequest.");
      // Проверяем есть ли настройка для интеграции с TemplateCustomerRequest.
      var settingsCode = Constants.Module.TemplateCustomerRequest;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для шаблона обращений клиента.";
        settingsRec.Notelenspec = "Строка с названием шаблона обращения клиента: Название шаблона";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Выдать права на справочники.
    /// </summary>
    public static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Grant rights on databooks to all users.");
      
      avis.Integration.Integrationses.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      avis.Integration.Integrationses.AccessRights.Save();
    }

    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateKBKIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for CRM integration");
      // Проверяем есть ли настройка для интеграции с КБК.
      var settingsCode = Constants.Module.KBKCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к KBK.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: url";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateWorkTypesIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for CRM integration");
      // Проверяем есть ли настройка для интеграции с Видами работ.
      var settingsCode = avis.EtalonContracts.PublicConstants.Module.KindsWorkTypesCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к WorkTypes.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: url";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateConstractionSitesIntegrationSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for CRM integration");
      // Проверяем есть ли настройка для интеграции с КБК.
      var settingsCode = Constants.Module.ConstractionSitesCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к ConstractionSites.";
        settingsRec.Notelenspec = "Строка подключения формируется в следующем виде: url";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
    
    /// <summary>
    /// Создать запись в справочнике настроек интеграции.
    /// </summary>
    public void CreateAPIFnsSettings()
    {
      InitializationLogger.Debug("Checking if exists settings for API Fns");
      // Проверяем есть ли настройка для API ФНС.
      var settingsCode = Constants.Module.APIFnsCode;
      var settingsRec = Etalon.Integrationses.GetAll().FirstOrDefault(i => i.Code == settingsCode);
      if (settingsRec == null)
      {
        InitializationLogger.Debug(@"Settings doesn't exists, creating new");
        settingsRec = Etalon.Integrationses.Create();
        settingsRec.Code = settingsCode;
        settingsRec.Name = "Настройки для подключения к API ФНС";
        settingsRec.Notelenspec = "В параметрах подключения укажите значение API-ключа для использования сервиса API ФНС.";
        settingsRec.Save();
        InitializationLogger.Debug(@"Settings created");
      }
      else
        InitializationLogger.Debug("Settings already exist");
    }
  }
}