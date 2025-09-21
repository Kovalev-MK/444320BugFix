using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace avis.EtalonParties.Server
{
  public partial class ModuleInitializer
  {
    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      try
      {
        InitializationLogger.DebugFormat("Инициализация модуля EtalonParties");
        // Создание таблиц для данных отчёта.
        CreateDBTable();
        // Создание ролей.
        CreateRoles();
        // Заполянем справочник групп контрагентов.
        CreateGroupCounterpartyFromDirectum5();
        // Заполняем справочник категорий контрагентов.
        CreateCategoryCounterpartyFromDirectum5();
        // Установить категорию контрагентам, компаниям у которых оно не указано.
        SetCategoryCounterpartyFromCompanies();
        // Назначить права на папки потока.
        GrantRightsOnFlowFolders();
        // Права на задачи
        GrantRightsOnTasks();
      }
      catch(Exception ex)
      {
        InitializationLogger.DebugFormat(ex.Message);
      }
    }
    
    /// <summary>
    /// Создание таблиц для данных отчёта.
    /// </summary>
    private void CreateDBTable()
    {
      InitializationLogger.DebugFormat("Create DB Activities");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebActivities });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebActivities, new[] { Constants.DebReport.TableNameDebActivities });
      
      InitializationLogger.DebugFormat("Create DB Founders");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebFounders });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebFounders, new[] { Constants.DebReport.TableNameDebFounders });
      
      InitializationLogger.DebugFormat("Create DB History Founders");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebHistoryFounders });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebHistoryFounders, new[] { Constants.DebReport.TableNameDebHistoryFounders });
      
      InitializationLogger.DebugFormat("Create DB Filials");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebFilials });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebFilials, new[] { Constants.DebReport.TableNameDebFilials });
      
      InitializationLogger.DebugFormat("Create DB Licenses");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebLicenses });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebLicenses, new[] { Constants.DebReport.TableNameDebLicenses });
      
      InitializationLogger.DebugFormat("Create DB Finances");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebFinances });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebFinances, new[] { Constants.DebReport.TableNameDebFinances });
      
      InitializationLogger.DebugFormat("Create DB Taxes");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebTaxes });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebTaxes, new[] { Constants.DebReport.TableNameDebTaxes });
      
      InitializationLogger.DebugFormat("Create DB Fns");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebFns });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebFns, new[] { Constants.DebReport.TableNameDebFns });
      
      InitializationLogger.DebugFormat("Create DB HistoryUL");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebHistoryUL });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebHistoryUL, new[] { Constants.DebReport.TableNameDebHistoryUL });

      InitializationLogger.DebugFormat("Create DB Banks");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebBanks });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebBanks, new[] { Constants.DebReport.TableNameDebBanks });
      
      InitializationLogger.DebugFormat("Create DB Directors");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebDirectors });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebDirectors, new[] { Constants.DebReport.TableNameDebDirectors });
      
      InitializationLogger.DebugFormat("Create DB Arbitrations");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebArbitrations });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebArbitrations, new[] { Constants.DebReport.TableNameDebArbitrations });
      
      InitializationLogger.DebugFormat("Create DB Contracts");
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { Constants.DebReport.TableNameDebContracts });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.Module.CreateTableDebContracts, new[] { Constants.DebReport.TableNameDebContracts });
    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    private static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(avis.EtalonParties.Resources.RoleNameResponsibleForMonopolists,
                                                                      avis.EtalonParties.Resources.RoleDescriptionResponsibleForMonopolists,
                                                                      PublicConstants.Module.RoleResponsibleForMonopolistsGuid);
    }
    
    /// <summary>
    /// Создаёт предустановленные категории контрагентов в справочник.
    /// </summary>
    private void CreateCategoryCounterpartyFromDirectum5()
    {
      InitializationLogger.DebugFormat("Init: Create category counterparty for Etalon Parties.");
      // Создаём список предопределенных категорий.
      var categories = new List<avis.EtalonParties.Structures.Module.CategoryCounterpartyModel>
      { // CategoryCounterparty
        new avis.EtalonParties.Structures.Module.CategoryCounterpartyModel{DirectumId = 18868803, Name = avis.EtalonParties.Resources.LegislativeCategoryCounterparty, GroupCounterpartyId = new List<int>{17896396, 17896402}},
        new avis.EtalonParties.Structures.Module.CategoryCounterpartyModel{DirectumId = 18868806, Name = avis.EtalonParties.Resources.ExecutiveCategoryCounterparty, GroupCounterpartyId = new List<int>{17896396, 17896402}},
        new avis.EtalonParties.Structures.Module.CategoryCounterpartyModel{DirectumId = 18868807, Name = avis.EtalonParties.Resources.JudicialCategoryCounterparty, GroupCounterpartyId = new List<int>{17896396, 17896402}},
        new avis.EtalonParties.Structures.Module.CategoryCounterpartyModel{DirectumId = 17896511, Name = avis.EtalonParties.Resources.CommercialCategoryCounterparty, GroupCounterpartyId = new List<int>{17896403}},
        new avis.EtalonParties.Structures.Module.CategoryCounterpartyModel{DirectumId = 17896520, Name = avis.EtalonParties.Resources.NoCommercialCategoryCounterparty, GroupCounterpartyId = new List<int>{17896403}},
        new avis.EtalonParties.Structures.Module.CategoryCounterpartyModel{DirectumId = 17896527, Name = avis.EtalonParties.Resources.IPCategoryCounterparty, GroupCounterpartyId = new List<int>{17896408}},
        new avis.EtalonParties.Structures.Module.CategoryCounterpartyModel{DirectumId = 17896528, Name = avis.EtalonParties.Resources.OtherCategoryCounterparty, GroupCounterpartyId = new List<int>{17896409}},
        new avis.EtalonParties.Structures.Module.CategoryCounterpartyModel{DirectumId = 7, Name = avis.EtalonParties.Resources.EtalonCompanyCategoryCounterparty, GroupCounterpartyId = new List<int>{6}}
      };
      
      // Проверяем записи на наличие в справочнике; Если нет – создаём.
      foreach (var category in categories)
      {
        var categoryCounterparty = CategoryCounterparties.GetAll(c => Equals(c.IdDirectum5, category.DirectumId)).FirstOrDefault();
        
        if (categoryCounterparty == null)
        {
          categoryCounterparty = CategoryCounterparties.Create();
          categoryCounterparty.IdDirectum5 = category.DirectumId;
          categoryCounterparty.Name = category.Name;
          categoryCounterparty.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
          
          // Список групп, в которые входит данная категория.
          var groups = new List<avis.EtalonParties.IGroupCounterparty>();
          
          // Получаем группы из справочника по DirectumId5.
          foreach (var groupId in category.GroupCounterpartyId)
            groups.Add(GroupCounterparties.GetAll(g => g.IdDirectum5 == groupId).FirstOrDefault());
          
          // Заполняем найденные группы в категории.
          foreach (var group in groups)
            categoryCounterparty.GroupCounterparties.AddNew().GroupCounterparty = group;
          
          categoryCounterparty.Save();
          InitializationLogger.DebugFormat($"Создана категория контрагентов {category.DirectumId} {category.Name}.");
        }
      }
    }
    
    /// <summary>
    /// Создаёт предустановленные группы контрагентов в справочник.
    /// </summary>
    private void CreateGroupCounterpartyFromDirectum5()
    {
      InitializationLogger.DebugFormat("Init: Create group counterparty for Etalon Parties.");
      
      // Создаём список предопределенных групп.
      var groups = new List<avis.EtalonParties.Structures.Module.GroupCounterpartyModel>
      {
        new avis.EtalonParties.Structures.Module.GroupCounterpartyModel{DirectumId = 17896396, Name = avis.EtalonParties.Resources.FederalGroupCounterparty},
        new avis.EtalonParties.Structures.Module.GroupCounterpartyModel{DirectumId = 17896402, Name = avis.EtalonParties.Resources.GosGroupCounterparty},
        new avis.EtalonParties.Structures.Module.GroupCounterpartyModel{DirectumId = 17896403, Name = avis.EtalonParties.Resources.LegalGroupCounterparty},
        new avis.EtalonParties.Structures.Module.GroupCounterpartyModel{DirectumId = 17896408, Name = avis.EtalonParties.Resources.IPGroupCounterparty},
        new avis.EtalonParties.Structures.Module.GroupCounterpartyModel{DirectumId = 17896409, Name = avis.EtalonParties.Resources.OtherGroupCounterparty},
        new avis.EtalonParties.Structures.Module.GroupCounterpartyModel{DirectumId = 6, Name = avis.EtalonParties.Resources.EtalonCompanyGroupCounterparty},
      };
      
      // Проверяем записи на наличие в справочните, если нету создаём.
      foreach (var group in groups)
      {
        var groupCounterparty = GroupCounterparties.GetAll(g => Equals(g.IdDirectum5, group.DirectumId)).FirstOrDefault();
        
        if (groupCounterparty == null)
        {
          groupCounterparty = GroupCounterparties.Create();
          groupCounterparty.IdDirectum5 = group.DirectumId;
          groupCounterparty.Name = group.Name;
          groupCounterparty.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
          groupCounterparty.Save();
          
          InitializationLogger.DebugFormat($"Создана группа контрагентов {group.DirectumId} {group.Name}.");
        }
      }
    }
    
    /// <summary>
    /// Установить категорию контрагентам, компаниям у которых оно не указано.
    /// </summary>
    private void SetCategoryCounterpartyFromCompanies()
    {
      InitializationLogger.Debug("Init: Set Category Counterparty From Companies for Etalon Parties.");
      
      // Получаем категорию "Компании группы эталон".
      var groupCounterparty = avis.EtalonParties.GroupCounterparties.GetAll(g => g.IdDirectum5 == 6).FirstOrDefault();
      var categoryCounterparty = avis.EtalonParties.CategoryCounterparties.GetAll(c => c.IdDirectum5 == 7).FirstOrDefault();
      
      // Находим все компании у кого не заполненна категория контрагента.
      var companies = lenspec.Etalon.Companies.GetAll(c => c.GroupCounterpartyavis == groupCounterparty && c.CategoryCounterpartyavis == null);
      
      // Заполняем всем компаниям группу компаний эталон.
      foreach (var company in companies)
      {
        company.CategoryCounterpartyavis = categoryCounterparty;
        company.Save();
      }
    }

    /// <summary>
    /// Назначить права на папки потока.
    /// </summary>
    private static void GrantRightsOnFlowFolders()
    {
      InitializationLogger.Debug("Init: Grant rights on flow folders to responsible for tenders role.");
      
      var tendersResponsibleRole = Roles.GetAll().SingleOrDefault(r => r.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.TenderResponsibleGuid);
      if (tendersResponsibleRole != null)
      {
        EtalonParties.SpecialFolders.ContractorsRegister.AccessRights.Grant(tendersResponsibleRole, DefaultAccessRightsTypes.Read);
        EtalonParties.SpecialFolders.ProviderRegister.AccessRights.Grant(tendersResponsibleRole, DefaultAccessRightsTypes.Read);
        
        EtalonParties.SpecialFolders.ContractorsRegister.AccessRights.Save();
        EtalonParties.SpecialFolders.ProviderRegister.AccessRights.Save();
      }
    }
    
    /// <summary>
    /// Выдача прав на задачи
    /// </summary>
    public static void GrantRightsOnTasks()
    {
      InitializationLogger.Debug("Init: Grant rights on counterparties to responsible role for Etalon Parties.");
      
      var counterpartiesResponsible = Roles.GetAll().SingleOrDefault(n => n.Sid == Sungero.Docflow.PublicConstants.Module.RoleGuid.CounterpartiesResponsibleRole);
      
      CreateCompanyTasks.AccessRights.Grant(counterpartiesResponsible, DefaultAccessRightsTypes.Create);
      CreateCompanyTasks.AccessRights.Save();
    }
  }
}