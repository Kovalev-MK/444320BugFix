using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace lenspec.Etalon.Module.Docflow.Server
{
  public partial class ModuleInitializer
  {

    //Добавлено Avis Expert.
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      base.Initializing(e);
      
      EditRightToRegistrationManagersRole();
      
      UpdateDefaultResponseRelationType();
      
      CreateDefaultVatRatesAvis();
    }
    
    #region Виды документов
    
    /// <summary>
    /// Создание видов документов.
    /// </summary>
    public static void CreateDocumentKindsAvis()
    {
      // Создание вида документа «Акт о приёмке выполненных работ (КС-2)».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Etalon.Module.Docflow.Resources.ContractStatementC2KindName,
                                                                              lenspec.Etalon.Module.Docflow.Resources.ContractStatementC2KindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true, false,
                                                                              Sungero.FinancialArchive.Constants.Module.Initialize.ContractStatementKind,
                                                                              new Sungero.Domain.Shared.IActionInfo[]
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval,
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForDocumentFlow
                                                                              },
                                                                              Constants.Module.DocumentKindGuids.ContractStatementC2Kind);
      // Создание вида документа «Справка о приёмке выполненных работ (КС-3)».
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(lenspec.Etalon.Module.Docflow.Resources.ContractStatementC3KindName,
                                                                              lenspec.Etalon.Module.Docflow.Resources.ContractStatementC3KindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.Numerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Contracts, true, false,
                                                                              Sungero.FinancialArchive.Constants.Module.Initialize.ContractStatementKind,
                                                                              new Sungero.Domain.Shared.IActionInfo[]
                                                                              {
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval,
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForDocumentFlow
                                                                              },
                                                                              Constants.Module.DocumentKindGuids.ContractStatementC3Kind);
    }
    
    #endregion
    
    /// <summary>
    /// Скорректировать права роли "Ответственные за настройку регистрации", выданные в базовом решении.
    /// </summary>
    public static void EditRightToRegistrationManagersRole()
    {
      InitializationLogger.Debug("Init: Grant rights on logs and registration settings to registration managers.");
      
      var registrationManagers = Roles.GetAll().SingleOrDefault(n => n.Sid == Sungero.Docflow.PublicConstants.Module.RoleGuid.RegistrationManagersRole);
      if (registrationManagers == null)
        return;
      
      // Модуль "Документооборот".
      Sungero.Docflow.DocumentKinds.AccessRights.RevokeAll(registrationManagers);
      Sungero.Docflow.DocumentKinds.AccessRights.Grant(registrationManagers, DefaultAccessRightsTypes.Read);
      Sungero.Docflow.DocumentKinds.AccessRights.Save();
      
      Sungero.Docflow.CaseFiles.AccessRights.RevokeAll(registrationManagers);
      Sungero.Docflow.CaseFiles.AccessRights.Grant(registrationManagers, DefaultAccessRightsTypes.Read);
      Sungero.Docflow.CaseFiles.AccessRights.Save();
      
      Sungero.Docflow.DocumentRegisters.AccessRights.RevokeAll(registrationManagers);
      Sungero.Docflow.DocumentRegisters.AccessRights.Grant(registrationManagers, DefaultAccessRightsTypes.Read);
      Sungero.Docflow.DocumentRegisters.AccessRights.Save();
      
      Sungero.Docflow.RegistrationSettings.AccessRights.RevokeAll(registrationManagers);
      Sungero.Docflow.RegistrationSettings.AccessRights.Grant(registrationManagers, DefaultAccessRightsTypes.Read);
      Sungero.Docflow.RegistrationSettings.AccessRights.Save();
      
      Sungero.Docflow.MailDeliveryMethods.AccessRights.RevokeAll(registrationManagers);
      Sungero.Docflow.MailDeliveryMethods.AccessRights.Grant(registrationManagers, DefaultAccessRightsTypes.Read);
      Sungero.Docflow.MailDeliveryMethods.AccessRights.Save();
      
      Sungero.Docflow.FileRetentionPeriods.AccessRights.RevokeAll(registrationManagers);
      Sungero.Docflow.FileRetentionPeriods.AccessRights.Grant(registrationManagers, DefaultAccessRightsTypes.Read);
      Sungero.Docflow.FileRetentionPeriods.AccessRights.Save();
    }
    
    /// <summary>
    /// Добавить в базовый тип связи "В ответ на" тип Обращение клиента.
    /// </summary>
    public static void UpdateDefaultResponseRelationType()
    {
      // Ответное письмо.
      var response = RelationTypes.GetAll(r => r.Name == Sungero.Docflow.PublicConstants.Module.ResponseRelationName).FirstOrDefault();
      if (response != null)
      {
        var responseRow = response.Mapping.AddNew();
        responseRow.Source = avis.CustomerRequests.CustomerRequests.Info;
        responseRow.Target = Sungero.RecordManagement.OutgoingLetters.Info;
        responseRow.RelatedProperty = lenspec.Etalon.OutgoingLetters.Info.Properties.InResponseToCustRequestavis;
        response.Save();
      }
    }
    
    #region Создание ставок НДС
    
    /// <summary>
    /// Создать базовые ставки НДС.
    /// </summary>
    public static void CreateDefaultVatRatesAvis()
    {
      InitializationLogger.Debug("Init: Create default VAT Rates.");
      
      CreateVatRateAvis(Constants.Module.VatRateFivePercentSid,
                        lenspec.Etalon.Module.Docflow.Resources.DefaultVatRateFivePercentName,
                        Constants.Module.DefaultVatRateFivePercent);
      CreateVatRateAvis(Constants.Module.VatRateSevenPercentSid,
                        lenspec.Etalon.Module.Docflow.Resources.DefaultVatRateSevenPercentName,
                        Constants.Module.DefaultVatRateSevenPercent);
      CreateVatRateAvis(Sungero.Docflow.Constants.Module.VatRateZeroPercentSid,
                        lenspec.Etalon.Module.Docflow.Resources.DefaultVatRateZeroPercentName,
                        Sungero.Docflow.Constants.Module.DefaultVatRateZeroPercent);
      CreateVatRateAvis(Constants.Module.VatRateEighteenPercentSid,
                        lenspec.Etalon.Module.Docflow.Resources.DefaultVatRateEighteenPercentName,
                        Constants.Module.DefaultVatRateEighteenPercent);
      CreateVatRateAvis(Constants.Module.VatRateMixedPercentSid,
                        lenspec.Etalon.Module.Docflow.Resources.DefaultVatRateMixedPercentName,
                        Constants.Module.DefaultVatRateMixedPercent);
    }
    
    /// <summary>
    /// Создать ставку НДС.
    /// </summary>
    /// <param name="sid">Sid.</param>
    /// <param name="name">Наименование ставки НДС.</param>
    /// <param name="rate">Ставка НДС в %.</param>
    public static void CreateVatRateAvis(string sid, string name, double rate)
    {
      InitializationLogger.DebugFormat("Init: Create Vat Rate {0}", name);
      
      var vatRate = Sungero.Commons.VatRates.GetAll(x => x.Sid == sid).FirstOrDefault();
      if (vatRate == null)
      {
        vatRate = Sungero.Commons.VatRates.Create();
        vatRate.Sid = sid;
        vatRate.Name = name;
        vatRate.Rate = rate;
        vatRate.Save();
      }
      else
      {
        if (vatRate.Name != name)
          vatRate.Name = name;
        
        if (vatRate.Rate != rate)
          vatRate.Rate = rate;
        
        if (vatRate.State.IsChanged)
          vatRate.Save();
      }
    }
    
    #endregion
    //конец Добавлено Avis Expert
  }
}
