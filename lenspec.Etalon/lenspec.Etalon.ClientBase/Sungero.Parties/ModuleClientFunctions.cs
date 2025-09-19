using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Parties.Client
{
  // Добавлено avis.
  
  partial class ModuleFunctions
  {

    /// <summary>
    /// Массовая выгрузка контрагентов
    /// </summary>
    [LocalizeFunction("UnloadingContractors_ResourceKey", "UnloadingContractors_DescriptionResourceKey")]
    public virtual void UnloadingContractors()
    {
      var counterPartiesIds  = string.Empty;
      var allCounterParties  = lenspec.Etalon.Counterparties.GetAll(x => x.Status == Sungero.Parties.Counterparty.Status.Active
                                                                    && (x.ResultApprovalDEBavis == Etalon.Counterparty.ResultApprovalDEBavis.CoopPossible   ||
                                                                        x.ResultApprovalDEBavis == Etalon.Counterparty.ResultApprovalDEBavis.CoopWithRisks  ||
                                                                        x.ResultApprovalDEBavis == Etalon.Counterparty.ResultApprovalDEBavis.DoesNotReqAppr));
      
      var user = Sungero.Company.Employees.Current;
      if (user == null)
        return;
      var dialog          = Dialogs.CreateInputDialog("Выгрузка контрагентов");
      //      var businesUnit     = dialog.AddSelect("Наша организация", true, Sungero.Company.BusinessUnits.Null);
      //      businesUnit.Value   = user.Department.BusinessUnit;
      var busineUnitCode = "000";
      var employee        = dialog.AddSelect("Инициатор", true, Sungero.Company.Employees.Null);
      employee.Value = user;
      var unloading       = dialog.AddSelect("Выгрузить", false).From("Все организации", "Все банки", "Все персоны", "Всех контрагентов");
      unloading.IsEnabled = true;
      
      var partiesDefault  = new List<lenspec.Etalon.ICounterparty>();
      var parties         = dialog.AddSelectMany("Контрагенты", true, partiesDefault.ToArray());
      parties.IsEnabled   = false;
      parties.IsVisible   = false;
      var selectedPartiesText = dialog.AddMultilineString("Контрагенты", false, string.Empty).RowsCount(3);
      selectedPartiesText.IsEnabled = false;
      var addPartiesLink    = dialog.AddHyperlink("Добавить контрагента");
      var deletePartiesLink = dialog.AddHyperlink("Исключить контрагента");
      
      var exceptUnloadingDefault = new List<lenspec.Etalon.ICounterparty>();
      var exceptUnloading = dialog.AddSelectMany("Кроме", false, exceptUnloadingDefault.ToArray());
      exceptUnloading.IsEnabled = false;
      exceptUnloading.IsVisible = false;
      var selectedExceptUnloadingText = dialog.AddMultilineString("Кроме", false, string.Empty).RowsCount(3);
      selectedExceptUnloadingText.IsEnabled = false;
      var addExceptUnloadingLink    = dialog.AddHyperlink("Добавить контрагента");
      var deleteExceptUnloadingLink = dialog.AddHyperlink("Исключить контрагента");
      addExceptUnloadingLink.IsEnabled = false;
      deleteExceptUnloadingLink.IsEnabled = false;
      
      #region Контрагенты
      addPartiesLink.SetOnExecute(
        () =>
        {
          var filteredParties = allCounterParties;
          var selectedParties = filteredParties.ShowSelectMany().AsEnumerable();
          if (selectedParties != null && selectedParties.Any())
          {
            var sourceParties = parties.Value.ToList();
            sourceParties.AddRange(selectedParties);
            parties.Value = sourceParties.Distinct();
            selectedPartiesText.Value = string.Join("; ", parties.Value.Select(x => x.Name));
          }
        });
      deletePartiesLink.SetOnExecute(
        () =>
        {
          var selectedParties = parties.Value.ShowSelectMany("Выберите Контрагента для исключения");
          if (selectedParties != null && selectedParties.Any())
          {
            var currentParties = parties.Value.ToList();
            foreach (var ka in selectedParties)
            {
              currentParties.Remove(ka);
            }
            parties.Value = currentParties;
            selectedPartiesText.Value = string.Join("; ", parties.Value.Select(x => x.Name));
          }
        });
      
      #endregion
      
      #region Кроме
      addExceptUnloadingLink.SetOnExecute(
        () =>
        {
          var filteredExceptUnloading = allCounterParties;
          if (unloading.Value == "Все банки")
          {
            filteredExceptUnloading = filteredExceptUnloading.Where(x => Sungero.Parties.Banks.Is(x));
          }
          if (unloading.Value == "Все персоны")
          {
            filteredExceptUnloading = filteredExceptUnloading.Where(x => Sungero.Parties.People.Is(x));
          }
          if (unloading.Value == "Все организации")
          {
            filteredExceptUnloading = filteredExceptUnloading.Where(x => Sungero.Parties.Companies.Is(x));
          }
          var selectedExceptUnloading = filteredExceptUnloading.ShowSelectMany().AsEnumerable();
          if (selectedExceptUnloading != null && selectedExceptUnloading.Any())
          {
            var sourceExceptUnloading = exceptUnloading.Value.ToList();
            sourceExceptUnloading.AddRange(selectedExceptUnloading);
            exceptUnloading.Value = sourceExceptUnloading.Distinct();
            selectedExceptUnloadingText.Value = string.Join("; ", exceptUnloading.Value.Select(x => x.Name));
          }
        });
      deleteExceptUnloadingLink.SetOnExecute(
        () =>
        {
          var selectedExceptUnloadings = exceptUnloading.Value.ShowSelectMany("Выберите Контрагента для исключения");
          if (selectedExceptUnloadings != null && selectedExceptUnloadings.Any())
          {
            var currentExceptUnloadings = exceptUnloading.Value.ToList();
            foreach (var eu in selectedExceptUnloadings)
            {
              currentExceptUnloadings.Remove(eu);
            }
            exceptUnloading.Value = currentExceptUnloadings;
            selectedExceptUnloadingText.Value = string.Join("; ", exceptUnloading.Value.Select(x => x.Name));
          }
        });
      
      #endregion
      
      #region SetOnRefresh
      dialog.SetOnRefresh(er =>
                          {
                            if (unloading.Value != null)
                            {
                              addExceptUnloadingLink.IsEnabled    = true;
                              deleteExceptUnloadingLink.IsEnabled = true;
                              addPartiesLink.IsEnabled            = false;
                              deletePartiesLink.IsEnabled         = false;
                            }
                            else
                            {
                              addExceptUnloadingLink.IsEnabled    = false;
                              deleteExceptUnloadingLink.IsEnabled = false;
                              addPartiesLink.IsEnabled            = true;
                              deletePartiesLink.IsEnabled         = true;
                              
                            };
                          });
      #endregion
      
      #region SetOnButtonClick
      dialog.SetOnButtonClick((args) =>
                              {
                                if (parties.Value.Count() < 1 && unloading.Value == null)
                                {
                                  args.AddError("Выберите контрагентов для выгрузки!");
                                  return;
                                }
                              });
      #endregion
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        var partiesToProcessingId  = new List<long>();
        if (unloading.Value != null)
        {
          var filteredExceptUnloadingForAsyn = allCounterParties;

          #region Все банки
          if (unloading.Value == "Все банки")
          {
            filteredExceptUnloadingForAsyn = filteredExceptUnloadingForAsyn.Where(x => Sungero.Parties.Banks.Is(x));
          }
          #endregion
          
          #region Все персоны
          if (unloading.Value == "Все персоны")
          {
            filteredExceptUnloadingForAsyn = filteredExceptUnloadingForAsyn.Where(x => Sungero.Parties.People.Is(x));
          }
          #endregion
          
          #region Все организации
          if (unloading.Value == "Все организации")
          {
            filteredExceptUnloadingForAsyn = filteredExceptUnloadingForAsyn.Where(x => Sungero.Parties.Companies.Is(x));
          }
          #endregion
          
          var exceptUnloadingId = exceptUnloading.Value.Select(x => x.Id).ToList();
          filteredExceptUnloadingForAsyn = filteredExceptUnloadingForAsyn.Where(x => !exceptUnloadingId.Contains(x.Id));
          partiesToProcessingId = filteredExceptUnloadingForAsyn.Select(x => x.Id).ToList();
        }
        else
        {
          var filteredExceptUnloadingForAsyn = parties.Value;
          partiesToProcessingId = filteredExceptUnloadingForAsyn.Select(x => x.Id).ToList();
        }
        if (!partiesToProcessingId.Any())
        {
          Dialogs.ShowMessage("Нет данных для выгрузки!", MessageType.Error);
          return;
        }
        Dialogs.NotifyMessage("Выгрузка в интеграционную базу началась");
//        var busineUnitCode = lenspec.Etalon.BusinessUnits.As(businesUnit.Value).ExternalCodeavis;
        lenspec.Etalon.Module.Parties.PublicFunctions.Module.UnloadingKA(partiesToProcessingId, busineUnitCode);
      }
    }
    
    #region Создание нового КА
    
    /// <summary>
    /// 
    /// </summary>
    [LocalizeFunction("FormCounterpartyApprovalReport_ResourceKey", "FormCounterpartyApprovalReport_DescriptionResourceKey")]
    public virtual void FormCounterpartyApprovalReport()
    {
      var role = Roles.GetAll(x => x.Sid == avis.ApprovingCounterpartyDEB.PublicConstants.Module.RightsReportOnApprovalCounterpartyRoleGuid).SingleOrDefault();
      if (!lenspec.Etalon.Employees.Current.IncludedIn(role) && !lenspec.Etalon.Employees.Current.IncludedIn(Roles.Administrators))
      {
        Dialogs.ShowMessage("Недостаточно прав доступа", MessageType.Error);
        return;
      }
      
      var approvalerList = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.GetAll(x => x.ApprovalerDEB != null)
        .Select(x => x.ApprovalerDEB)
        .ToList()
        .Distinct();
      var taskStateList = new string[] {"В работе", "Выполнено", "На приемке"};
      
      var dialog = Dialogs.CreateInputDialog("Параметры отчета 'Контрагенты на согласование'");
      var validFrom = dialog.AddDate("Дата старта задачи с", true, Calendar.Today.BeginningOfMonth());
      var validTill = dialog.AddDate("Дата старта задачи по", true, Calendar.Today);
      var approvaler = dialog.AddSelect("Проверяющий ДБ", false, lenspec.Etalon.Employees.Null).From(approvalerList);
      var taskState = dialog.AddSelect("Статус задачи", false, string.Empty).From(taskStateList);
      
      dialog.SetOnRefresh(er =>
                          {
                            if (validTill.Value != null && validFrom.Value != null && validTill.Value < validFrom.Value ||
                                validFrom.Value != null && validTill.Value != null && validFrom.Value > validTill.Value)
                              er.AddError("'Дата старта задачи по' должна быть больше 'Дата задачи с'");
                          });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        var report = avis.ApprovingCounterpartyDEB.Reports.GetCounterpartiesOnApproval();
        report.ValidFrom = validFrom.Value;
        report.ValidTill = validTill.Value.Value.AddDays(1);
        report.ApprovalerId = approvaler.Value != null ? approvaler.Value.Id : -1;
        if (taskState.Value != null)
        {
          if (taskState.Value.Equals(taskStateList[0]))
            report.Status = "InProcess";
          else if (taskState.Value.Equals(taskStateList[1]))
            report.Status = "Completed";
          else
            report.Status = "UnderReview";
        }
        else
          report.Status = string.Empty;
        report.Open();
      }
    }

    /// <summary>
    /// Показать задачи на создание, изменение контрагента/банка/персоны.
    /// </summary>
    [LocalizeFunction("ShowCreateCompanyTasks_ResourceKey", "ShowCreateCompanyTasks_DescriptionResourceKey")]
    public virtual void ShowCreateCompanyTasks()
    {
      var dialog = Dialogs.CreateInputDialog("Поиск заявок на создание и изменение контрагентов, банков и персон");
      
      var author = dialog.AddSelect("Инициатор", false, lenspec.Etalon.Employees.Current);
      
      var validFrom = dialog.AddDate("Дата старта задачи с", false, Calendar.Today.BeginningOfMonth());
      var validTill = dialog.AddDate("Дата старта задачи по", false, Calendar.Today);
      dialog.SetOnRefresh((x) =>
                          {
                            if (validFrom.Value != null && validTill.Value != null && validFrom.Value > validTill.Value)
                            {
                              x.AddError("'Дата старта задачи с' должна быть меньше 'Дата старта задачи по'");
                            }
                          });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        var tasks = avis.EtalonParties.CreateCompanyTasks.GetAll()
          .Where(x => (author.Value == null || x.Author == author.Value) &&
                 (validFrom.Value == null || x.Created >= validFrom.Value) &&
                 (validTill.Value == null || x.Created <= validTill.Value.Value.AddDays(1)));
        tasks.Show();
      }
    }

    /// <summary>
    /// Показать задачи на согласование контрагента/банка/персоны.
    /// </summary>
    [LocalizeFunction("ShowApprovalCounterpartyPersonTasks_ResourceKey", "ShowApprovalCounterpartyPersonTasks_DescriptionResourceKey")]
    public virtual void ShowApprovalCounterpartyPersonTasks() // TODO: Фильтрацию задач на сервер
    {
      var dialog = Dialogs.CreateInputDialog("Поиск заявок на согласование контрагентов, банков и персон");
      var counterpartyList = lenspec.Etalon.Counterparties.GetAll();
      
      var author = dialog.AddSelect("Инициатор", false, lenspec.Etalon.Employees.Current);
      
      var counterparty = dialog.AddString("Контрагент", false, string.Empty);
      counterparty.IsEnabled = false;
      var counterpartyAction = dialog.AddHyperlink("Выбрать контрагента");
      var selectedValues = new List<ICounterparty>();
      counterpartyAction.SetOnExecute(() =>
                                      {
                                        selectedValues = counterpartyList.ShowSelectMany().ToList();
                                        counterparty.Value = string.Empty;
                                        if (selectedValues != null && selectedValues.Any())
                                        {
                                          foreach (var item in selectedValues)
                                          {
                                            counterparty.Value += item == selectedValues.Last() ? item.Name : item.Name + ", ";
                                          }
                                        }
                                      });
      
      var validFrom = dialog.AddDate("Дата старта задачи с", false, Calendar.Today.BeginningOfMonth());
      var validTill = dialog.AddDate("Дата старта задачи по", false, Calendar.Today);
      dialog.SetOnRefresh((x) =>
                          {
                            if (validFrom.Value != null && validTill.Value != null && validFrom.Value > validTill.Value)
                            {
                              x.AddError("'Дата старта задачи с' должна быть меньше 'Дата старта задачи по'.");
                            }
                          });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        var tasks = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.GetAll()
          .Where(x => (author.Value == null || x.Author == author.Value) &&
                 (!selectedValues.Any() || selectedValues.Contains(x.Counterparty)) &&
                 (validFrom.Value == null || x.Created >= validFrom.Value) &&
                 (validTill.Value == null || x.Created <= validTill.Value.Value.AddDays(1)));
        tasks.Show();
      }
    }

    /// <summary>
    /// Новый документ согласование персоны.
    /// </summary>
    [LocalizeFunction("CreateApprovalPersonDEB_ResourceKey", "CreateApprovalPersonDEB_DescriptionResourceKey")]
    public virtual void CreateApprovalPersonDEB()
    {
      var document = avis.ApprovingCounterpartyDEB.ApprovalPersonDEBs.Create();
      document.Show();
    }

    /// <summary>
    /// Новый документ согласование контрагента/банка.
    /// </summary>
    [LocalizeFunction("CreateApprovalCounterpartyBankDEB_ResourceKey", "CreateApprovalCounterpartyBankDEB_DescriptionResourceKey")]
    public virtual void CreateApprovalCounterpartyBankDEB()
    {
      var document = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEBs.Create();
      document.Show();
    }
    
    /// <summary>
    /// Открыть окно для создания задачи для создания новой персоны.
    /// </summary>
    [LocalizeFunction("ShowCreatePersonTask_ResourceKey", "ShowCreatePersonTask_DescriptionResourceKey")]
    public virtual void ShowCreatePersonTask()
    {
      var task = avis.EtalonParties.CreateCompanyTasks.Create();
      task.TypeObject = avis.EtalonParties.CreateCompanyTask.TypeObject.Person;
      task.Show();
    }
    
    /// <summary>
    /// Создание новой организации.
    /// </summary>
    [LocalizeFunction("CreateNewCompany_ResourceKey", "CreateNewCompany_DescriptionResourceKey")]
    public virtual void CreateNewCompany()
    {
      // Полные права на создание всех видов контрагентов
      var createCounterpartyRole = Sungero.CoreEntities.Roles.GetAll(r => r.Sid == avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid).FirstOrDefault();
      
      if (Employees.Current.IncludedIn(createCounterpartyRole) == true || Employees.Current.IncludedIn(Roles.Administrators) == true)
      {
        Companies.Create().Show();
        return;
      }
      
      var dialog = Dialogs.CreateTaskDialog("Нет прав на создание организации", "У вас нет прав на создание организации.", MessageType.Error);
      dialog.Buttons.AddOk();
      dialog.Show();
    }
    
    /// <summary>
    /// Открыть окно для создания задачи для создания нового контрагента.
    /// </summary>
    [LocalizeFunction("ShowCreateCompanyTask_ResourceKey", "ShowCreateCompanyTask_DescriptionResourceKey")]
    public virtual void ShowCreateCompanyTask()
    {
      var task = avis.EtalonParties.CreateCompanyTasks.Create();
      task.TypeObject = avis.EtalonParties.CreateCompanyTask.TypeObject.Counterparty;
      task.Show();
    }
    
    #endregion
    
    #region Интеграция с Kontur Focus.
    
    /// <summary>
    /// Заполнить организацию из КонтурФокус
    /// </summary>
    /// <param name="company">Компания</param>
    [Public]
    public void FillCompanyFromKontur(lenspec.Etalon.ICompanyBase company)
    {
      // Если не заполнено КПП, компания = Юр лицо в РФ, то необходимо вывести диалог чтобы определить заполняется головная организация или филиал
      if (company.TIN.Trim().Count() == 10 && string.IsNullOrEmpty(company.TRRC) && !Equals(company.TIN.Substring(0, 4), "9909"))
        lenspec.Etalon.Module.Parties.Functions.Module.ShowDialogSelectCompanyKind(company);
      else
      {
        var errorMessage = lenspec.Etalon.Module.Parties.Functions.Module.Remote.FillCompanyFromKontur(company);
        if (!string.IsNullOrEmpty(errorMessage))
          Dialogs.ShowMessage(errorMessage, MessageType.Error);
      }
    }

    /// <summary>
    /// Показать диалог, выбора заполнения головной организацией или филиалом.
    /// </summary>
    /// <param name="company">Компания</param>
    public void ShowDialogSelectCompanyKind(lenspec.Etalon.ICompanyBase company)
    {
      var dialog = Dialogs.CreateInputDialog("Подтверждение", "Вы ищите филиал или головную организацию?");
      var btnBranch = dialog.Buttons.AddCustom("Филиал");
      var btnHead = dialog.Buttons.AddCustom("Головная организация");
      var btnSelected = dialog.Show();
      if (btnSelected == btnBranch)
      {
        var errorMessage = lenspec.Etalon.Module.Parties.Functions.Module.Remote.CheckOpportunitySelectBranch(company.TIN);
        if (!string.IsNullOrEmpty(errorMessage))
          Dialogs.ShowMessage(errorMessage, MessageType.Error);
        else
        {
          var branchNames = lenspec.Etalon.Module.Parties.Functions.Module.Remote.TryFormBranchNames(company.TIN);
          if (!branchNames.Any())
            Dialogs.ShowMessage(lenspec.Etalon.Module.Parties.Resources.ErrorMessageFailFillFromKontur, MessageType.Error);
          else
            ShowDialogSelectBranch(branchNames, company);
        }
      }
      else if (btnSelected == btnHead)
      {
        var errorMessage = lenspec.Etalon.Module.Parties.Functions.Module.Remote.TryFillLegalEntityLikeHeadCompanyFromDialog(company);
        if (!string.IsNullOrEmpty(errorMessage))
          Dialogs.ShowMessage(errorMessage, MessageType.Error);
      }
    }

    /// <summary>
    /// Показать диалог выбора филиала.
    /// </summary>
    /// <param name="companyDataModel">Модель данных компании из КонтурФокус</param>
    /// <param name="company">Компания</param>
    public void ShowDialogSelectBranch(List<string> branchNames, lenspec.Etalon.ICompanyBase company)
    {
      var dialog = Dialogs.CreateInputDialog("Выберите филиал организации.");
      var companySelected = dialog.AddSelect("Филиалы", true, string.Empty).From(branchNames.ToArray());
      dialog.Buttons.AddOkCancel();
      dialog.Width = 350;
      if (dialog.Show() == DialogButtons.Ok)
      {
        var errorMessage = lenspec.Etalon.Module.Parties.Functions.Module.Remote.TryFillLegalEntityLikeBranchFromDialog(company, companySelected.Value);
        if (!string.IsNullOrEmpty(errorMessage))
          Dialogs.ShowMessage(errorMessage, MessageType.Error);
      }
    }
    
    #endregion
  }
  
  // Конец добавлено avis.
}