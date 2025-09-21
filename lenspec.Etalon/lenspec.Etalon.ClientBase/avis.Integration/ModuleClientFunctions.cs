using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Integration.Client
{
  partial class ModuleFunctions
  {

    /// <summary>
    /// Показать карточку настроек ConstractionSites.
    /// </summary>
    [LocalizeFunction("ShowAPIFnsIntegrationCard_ResourceKey", "ShowAPIFnsIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowAPIFnsIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.APIFnsCode).Show();
    }

    /// <summary>
    /// Показать карточку настроек ConstractionSites.
    /// </summary>
    [LocalizeFunction("ShowConstractionSitesIntegrationCard_ResourceKey", "ShowConstractionSitesIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowConstractionSitesIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.ConstractionSitesCode).Show();
    }

    /// <summary>
    /// Показать карточку настроек KBK
    /// </summary>
    [LocalizeFunction("ShowKBKIntegrationCard_ResourceKey", "ShowKBKIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowKBKIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.KBKCode).Show();
    }
    
    /// <summary>
    /// Показать карточку настроек Вида работ и Детализация видов работ
    /// </summary>
    [LocalizeFunction("ShowWorkTypeIntegrationCard_ResourceKey", "ShowWorkTypeIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowWorkTypeIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(avis.EtalonContracts.PublicConstants.Module.KindsWorkTypesCode).Show();
    }

    /// <summary>
    /// Показать карточку настроек PKP.
    /// </summary>
    [LocalizeFunction("ShowTemplateCustomerRequestCard_ResourceKey", "ShowTemplateCustomerRequestCard_DescriptionResourceKey")]
    public virtual void ShowTemplateCustomerRequestCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.TemplateCustomerRequest).Show();
    }
    
    /// <summary>
    /// Показать карточку настроек PKP.
    /// </summary>
    [LocalizeFunction("ShowPKPIntegrationCard_ResourceKey", "ShowPKPIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowPKPIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.PkpCode).Show();
    }

    /// <summary>
    /// Показать карточку настроек LK.
    /// </summary>
    [LocalizeFunction("ShowLKIntegrationCard_ResourceKey", "ShowLKIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowLKIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.LKCode).Show();
    }
    
    /// <summary>
    /// Показать карточку настроек CRM.
    /// </summary>
    [LocalizeFunction("ShowCRMIntegrationCard_ResourceKey", "ShowCRMIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowCRMIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.CRMCode).Show();
    }

    /// <summary>
    /// Показать карточку настроек NATS.
    /// </summary>
    [LocalizeFunction("ShowNatsIntegrationCard_ResourceKey", "ShowNatsIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowNatsIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.NatsCode).Show();
    }
    
    /// <summary>
    /// Показать карточку настроек GisClientInfo.
    /// </summary>
    [LocalizeFunction("ShowGisClientInfoIntegrationCard_ResourceKey", "ShowGisClientInfoIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowGisClientInfoIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.GisClientInfoCode).Show();
    }

    /// <summary>
    /// Показать карточку настроек Dadata.
    /// </summary>
    [LocalizeFunction("ShowDadataIntegrationCard_ResourceKey", "ShowDadataIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowDadataIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.DadataCode).Show();
    }
    
    /// <summary>
    /// Показать карточку настроек интеграции с API Контур.Фокус.
    /// </summary>
    [LocalizeFunction("ShowKonturFocusIntegrationCard_ResourceKey", "ShowKonturFocusIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowKonturFocusIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.KonturFocusEGRULRecordCode).Show();
    }
    
    /// <summary>
    /// Показать карточку настроек интеграции с ActiveDirectory Employee.
    /// </summary>
    [LocalizeFunction("ShowActiveDirectoryEmployeeIntegrationCard_ResourceKey", "ShowActiveDirectoryEmployeeIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowActiveDirectoryEmployeeIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.ActiveDirectoryEmployeeRecordCode).Show();
    }
    
    /// <summary>
    /// Показать карточку настроек интеграции с Invest.
    /// </summary>
    [LocalizeFunction("ShowInvestRabbitMqIntegrationCard_ResourceKey", "ShowInvestRabbitMqIntegrationCard_DescriptionResourceKey")]
    public virtual void ShowInvestRabbitMqIntegrationCard()
    {
      Functions.Module.Remote.GetIntegrationKFSettingsRec(Constants.Module.InvestConnectRabbitMQCode).Show();
    }
  }
}