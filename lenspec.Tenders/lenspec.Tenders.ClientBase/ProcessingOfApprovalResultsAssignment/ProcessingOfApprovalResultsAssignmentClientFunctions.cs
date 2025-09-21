using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ProcessingOfApprovalResultsAssignment;

namespace lenspec.Tenders.Client
{
  partial class ProcessingOfApprovalResultsAssignmentFunctions
  {
    
    /// <summary>
    /// Отобразить диалог подтверждения для неверно выбранного результата.
    /// </summary>
    /// <param name="isApprovedMajority">Признак согласования большинством.</param>
    /// <returns>true, если ответ – "Да"; Иначе – false.</returns>
    public bool ShowConfirmationDialogOnSolution(bool isApprovedMajority, int approvedResultCount, int rejectedResultCount)
    {
      // Для корректного результата выполнения диалог не отображаем.
      if (
        isApprovedMajority  && (approvedResultCount > rejectedResultCount) ||
        !isApprovedMajority && (approvedResultCount < rejectedResultCount)
       )
        return true;
      
      var description = lenspec.Tenders.ProcessingOfApprovalResultsAssignments.Resources.ResultMismatchWithMajorityContinueYN;
      var confirmDialog = Dialogs.CreateConfirmDialog(description, string.Empty, lenspec.Tenders.ProcessingOfApprovalResultsAssignments.Resources.Attention);
      
      return confirmDialog.Show();
    }
    
    /// <summary>
    /// Отобразить диалог подтверждения для предупреждений.
    /// </summary>
    /// <param name="warnings">Предупреждения.</param>
    /// <returns>true, если ответ – "Да"; Иначе – false.</returns>
    public static bool ShowConfirmationDialogOnWarnings(List<string> warnings)
    {
      // Фильтруем пустые сообщения.
      warnings = warnings.Where(x => !string.IsNullOrEmpty(x)).ToList();
      
      if (!warnings.Any())
        return true;
      
      // Обработка некритичных ошибок.
      var description = lenspec.Tenders.ProcessingOfApprovalResultsAssignments.Resources.WarningsOccurredDuringOperationContinueYNFormat(string.Join(Environment.NewLine, warnings));
      var confirmDialog = Dialogs.CreateConfirmDialog(description, string.Empty, lenspec.Tenders.ProcessingOfApprovalResultsAssignments.Resources.Attention);
      
      return confirmDialog.Show();
    }
    
    /// <summary>
    /// Отобразить диалог "Виды работ или товарные группы".
    /// </summary>
    /// <returns>Признак нажатия кнопки "ОК".</returns>
    public bool ShowWorkKindOrMaterialGroupDialog(Enumeration? registerKind, Sungero.Parties.ICompany counterparty, Tenders.ITenderDocument decision)
    {
      var dialog = Dialogs.CreateInputDialog(lenspec.Tenders.ProcessingOfApprovalResultsAssignments.Resources.WorkKindOrMaterialGroup);
      // Вид реестра.
      var registerKindSelect = dialog.AddString(
        ApprovalCounterpartyRegisterTasks.Info.Properties.RegisterKind.LocalizedName,
        true,
        ApprovalCounterpartyRegisterTasks.Info.Properties.RegisterKind.GetLocalizedValue(registerKind)
       );
      // Контрагент.
      var counterpartySelect = dialog.AddSelect(ApprovalCounterpartyRegisterTasks.Info.Properties.Counterparty.LocalizedName, true, counterparty);
      registerKindSelect.IsEnabled = false;
      counterpartySelect.IsEnabled = false;
      
      var isContractor =  registerKind == ApprovalCounterpartyRegisterTask.RegisterKind.Contractor;
      var isProvider =    registerKind == ApprovalCounterpartyRegisterTask.RegisterKind.Provider;
      
      // Форма выбора подрядчиков.
      var contractorsDefault = new List<Tenders.IContractorRegister>();
      var contractors = dialog.AddSelectMany(ProcessingOfApprovalResultsAssignments.Resources.WorkGroupPlural, false, contractorsDefault.ToArray());
      contractors.IsVisible = false;
      // Видимое представление списка.
      var contractorsSelect = dialog.AddMultilineString(ProcessingOfApprovalResultsAssignments.Resources.WorkGroupPlural, isContractor);
      contractorsSelect.IsVisible = isContractor;
      contractorsSelect.IsEnabled = false;
      
      // Форма выбора поставщиков.
      var providersDefault = new List<Tenders.IProviderRegister>();
      var providers = dialog.AddSelectMany(ProcessingOfApprovalResultsAssignments.Resources.MaterialPlural, false, providersDefault.ToArray());
      providers.IsVisible = false;
      // Видимое представление списка.
      var providersSelect = dialog.AddMultilineString(ProcessingOfApprovalResultsAssignments.Resources.MaterialPlural, isProvider);
      providersSelect.IsVisible = isProvider;
      providersSelect.IsEnabled = false;
      
      var addRecords = dialog.AddHyperlink(lenspec.Tenders.ProcessingOfApprovalResultsAssignments.Resources.AddRecords);
      var removeRecords = dialog.AddHyperlink(lenspec.Tenders.ProcessingOfApprovalResultsAssignments.Resources.RemoveRecords);
      
      // Реестры поставщиков/подрядчиков, отфильтрованные по КА.
      var providersPrefiltered = Tenders.ProviderRegisters.GetAll(x => Equals(x.Counterparty, counterparty)).Where(x => x.Status == Tenders.ProviderRegister.Status.Active);
      var contractorsPrefiltered = Tenders.ContractorRegisters.GetAll(x => Equals(x.Counterparty, counterparty)).Where(x => x.Status == Tenders.ContractorRegister.Status.Active);
      
      #region [Добавить поставщиков/подрядчиков]
      
      addRecords.SetOnExecute(
        () =>
        {
          if (isContractor)
          {
            var title = ProcessingOfApprovalResultsAssignments.Resources.ChooseForInclusionFormat(ProcessingOfApprovalResultsAssignments.Resources.WorkGroupPlural.ToString().ToLower());
            var selected = contractorsPrefiltered
              .Where(x => !contractors.Value.Contains(x))
              .ShowSelectMany(title);
            
            if (selected == null || !selected.Any())
              return;
            
            var updated = contractors.Value.ToList();
            updated.AddRange(selected);
            contractors.Value = updated;
            contractorsSelect.Value = GetNamesListFormat(contractors.Value.Select(x => x.Name));
          }
          else if (isProvider)
          {
            var title = ProcessingOfApprovalResultsAssignments.Resources.ChooseForInclusionFormat(ProcessingOfApprovalResultsAssignments.Resources.MaterialPlural.ToString().ToLower());
            var selected = providersPrefiltered
              .Where(x => !providers.Value.Contains(x))
              .ShowSelectMany(title);
            
            if (selected == null || !selected.Any())
              return;
            
            var updated = providers.Value.ToList();
            updated.AddRange(selected);
            providers.Value = updated;
            providersSelect.Value = GetNamesListFormat(providers.Value.Select(x => x.Name));
          }
        });
      
      #endregion [Добавить поставщиков/подрядчиков]
      
      #region [Исключить поставщиков/подрядчиков]
      
      removeRecords.SetOnExecute(
        () =>
        {
          if (isContractor)
          {
            var title = ProcessingOfApprovalResultsAssignments.Resources.ChooseForExclusionFormat(ProcessingOfApprovalResultsAssignments.Resources.WorkGroupPlural.ToString().ToLower());
            var selected = contractors.Value.ShowSelectMany(title);
            
            if (selected == null || !selected.Any())
              return;

            contractors.Value = contractors.Value.Except(selected);
            contractorsSelect.Value = GetNamesListFormat(contractors.Value.Select(x => x.Name));
          }
          else if (isProvider)
          {
            var title = ProcessingOfApprovalResultsAssignments.Resources.ChooseForExclusionFormat(ProcessingOfApprovalResultsAssignments.Resources.MaterialPlural.ToString().ToLower());
            var selected = providers.Value.ShowSelectMany(title);
            
            if (selected == null || !selected.Any())
              return;

            providers.Value = providers.Value.Except(selected);
            providersSelect.Value = GetNamesListFormat(providers.Value.Select(x => x.Name));
          }
        });
      
      #endregion [Исключить поставщиков/подрядчиков]
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        if (isContractor)
          Functions.ProcessingOfApprovalResultsAssignment.Remote.CloseContractors(_obj, contractors.Value.ToList(), decision);
        else if (isProvider)
          Functions.ProcessingOfApprovalResultsAssignment.Remote.CloseProviders(_obj, providers.Value.ToList(), decision);
        
        return true;
      }
      return false;
    }

    /// <summary>
    /// Формирование списка наименований в заданном виде.
    /// </summary>
    /// <param name="names">Наименования.</param>
    /// <returns>Строка с форматированными наименованиями.</returns>
    private static string GetNamesListFormat(IEnumerable<string> names)
    {
      var separator = ";" + Environment.NewLine;
      return names.Any() ?
        string.Join(separator, names) + "." :
        string.Empty;
    }
    
  }
}