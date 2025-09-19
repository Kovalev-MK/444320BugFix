using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets
{
  partial class ReconciliationSettingsClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      var reportSessionId = System.Guid.NewGuid().ToString();
      var dialog = Dialogs.CreateInputDialog("Настройки согласования");
      var businessUnitsDefault = new List<lenspec.Etalon.IBusinessUnit>();
      var businessUnits = dialog.AddSelectMany("Наши организации", false, businessUnitsDefault.ToArray());
      businessUnits.IsEnabled = false;
      businessUnits.IsVisible = false;
      var selectedBussinesUnitText = dialog.AddMultilineString("Наши организации", false, string.Empty).WithRowsCount(3);
      selectedBussinesUnitText.IsEnabled = false;
      var addBussinesUnitLink = dialog.AddHyperlink("Добавить наши организации");
      var deleteBussinesUnitLink = dialog.AddHyperlink("Исключить наши организации");
      
      var approvalRulesDefault = new List<Sungero.Docflow.IApprovalRuleBase>();
      var approvalRules = dialog.AddSelectMany("Регламент", false, approvalRulesDefault.ToArray());
      approvalRules.IsEnabled = false;
      approvalRules.IsVisible = false;
      var selectedApprovalRulesText = dialog.AddMultilineString("Регламент", false, string.Empty).WithRowsCount(3);
      selectedApprovalRulesText.IsEnabled = false;
      var addApprovalRulesLink = dialog.AddHyperlink("Добавить регламент");
      var deleteApprovalRulesLink = dialog.AddHyperlink("Исключить регламент");
      
      var roleKindsDefault = new List<lenspec.EtalonDatabooks.IRoleKind>();
      var roleKinds = dialog.AddSelectMany("Роль", true, roleKindsDefault.ToArray());
      roleKinds.IsEnabled = false;
      roleKinds.IsVisible = false;
      var selectedRoleKindsText = dialog.AddMultilineString("Роль", false, string.Empty).WithRowsCount(3);
      selectedRoleKindsText.IsEnabled = false;
      var addRoleKindsLink = dialog.AddHyperlink("Добавить роль");
      var deleteRoleKindsLink = dialog.AddHyperlink("Исключить роль");
      
      #region Состояние
      
      var actionStatus = dialog.AddSelect("Состояние", false).From("Действующая", "Закрытая");

      #endregion

      #region Наши организации
      
      addBussinesUnitLink.SetOnExecute(
        () =>
        {
          var selectedBussinesUnits = lenspec.Etalon.BusinessUnits
            .GetAll(x => !businessUnits.Value.Contains(x))
            .ShowSelectMany()
            .AsEnumerable();
          if (selectedBussinesUnits != null && selectedBussinesUnits.Any())
          {
            var sourceBussinesUnits = businessUnits.Value.ToList();
            sourceBussinesUnits.AddRange(selectedBussinesUnits);
            businessUnits.Value = sourceBussinesUnits;
            
            selectedBussinesUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
          }
        });
      deleteBussinesUnitLink.SetOnExecute(
        () =>
        {
          var selectedBussinesUnits = businessUnits.Value.ShowSelectMany("Выберите наши организации для исключения");
          if (selectedBussinesUnits != null && selectedBussinesUnits.Any())
          {
            var currentBussinesUnits = businessUnits.Value.ToList();
            foreach (var bussinesUnit in selectedBussinesUnits)
              currentBussinesUnits.Remove(bussinesUnit);
            
            businessUnits.Value = currentBussinesUnits;
            selectedBussinesUnitText.Value = string.Join("; ", businessUnits.Value.Select(x => x.Name));
            
            var filteredApprovalRules = Sungero.Docflow.ApprovalRuleBases.GetAll();
            var approvalRulesToRemove = new List<Sungero.Docflow.IApprovalRuleBase>();
            foreach (var rules in filteredApprovalRules)
              if (rules.BusinessUnits.Any(x => selectedBussinesUnits.Contains(x.BusinessUnit)))
                approvalRulesToRemove.Add(rules);
            
            var currentApprovalRules = approvalRules.Value.ToList();
            foreach (var approvalRule in approvalRulesToRemove)
              currentApprovalRules.Remove(approvalRule);
            
            approvalRules.Value = currentApprovalRules;
            selectedApprovalRulesText.Value = string.Join("; ", approvalRules.Value.Select(x => x.Name).OrderBy(n => n));
            
            //Очистим роли по тем Регламентам, которые были удалены
            var filteredRoleKinds = lenspec.EtalonDatabooks.RoleKinds.GetAll();
            //Отфильтруем роли по Регламентам, если они указаны
            if (approvalRules.Value.Any())
            {
              var roleKindsToRemove  = new List<lenspec.EtalonDatabooks.IRoleKind>();
              //Вначале найдем все Настройки согласования, которые входят в выбранные регламенты
              var approvalSettings = lenspec.EtalonDatabooks.ApprovalSettings.GetAll();
              approvalSettings = approvalSettings.Where(x => approvalRules.Value.Contains(x.ApprovalRule));
              foreach (var roles in approvalSettings)
                foreach(var role in roles.RoleKindEmployee)
                  roleKindsToRemove.Add(role.RoleKind);
              
              var currentRoleKinds = roleKinds.Value.ToList();
              foreach (var rk in roleKindsToRemove)
                currentRoleKinds.Remove(rk);
              
              roleKinds.Value = currentRoleKinds;
              selectedRoleKindsText.Value = string.Join("; ", roleKinds.Value.Select(x => x.Name));
            }
          }
        });
      
      #endregion
      
      #region Регламенты
      
      addApprovalRulesLink.SetOnExecute(
        () =>
        {
          var filteredApprovalRules = Sungero.Docflow.ApprovalRuleBases.GetAll();
          
          // Если выбраны НОР, фильтруем по ним доступные регламенты. Регламенты с пустыми НОР оставляем.
          if (businessUnits.Value.Any())
          {
            var businessUnitsList = businessUnits.Value.ToList();
            
            filteredApprovalRules = filteredApprovalRules
              .Where(r =>
                     r.BusinessUnits.Any(record => businessUnitsList.Contains(record.BusinessUnit)) ||
                     !r.BusinessUnits.Any()
                    );
          }
          // Исключаем из списка ранее выбранные регламенты.
          filteredApprovalRules = filteredApprovalRules.Where(r => !approvalRules.Value.Contains(r));
          
          var selectedApprovalRules = filteredApprovalRules.ShowSelectMany().AsEnumerable();
          if (selectedApprovalRules != null && selectedApprovalRules.Any())
          {
            var sourceApprovalRules = approvalRules.Value.ToList();
            sourceApprovalRules.AddRange(selectedApprovalRules);
            approvalRules.Value = sourceApprovalRules.Distinct();
            selectedApprovalRulesText.Value = string.Join("; ", approvalRules.Value.Select(x => x.Name).OrderBy(n => n));
          }
        });
      deleteApprovalRulesLink.SetOnExecute(
        () =>
        {
          var selectedApprovalRules = approvalRules.Value.ShowSelectMany("Выберите регламенты для исключения");
          if (selectedApprovalRules != null && selectedApprovalRules.Any())
          {
            var currentApprovalRules = approvalRules.Value.ToList();
            foreach (var approvalRule in selectedApprovalRules)
              currentApprovalRules.Remove(approvalRule);
            
            approvalRules.Value = currentApprovalRules;
            selectedApprovalRulesText.Value = string.Join("; ", approvalRules.Value.Select(x => x.Name).OrderBy(n => n));
            
            // Очистим роли по тем Регламентам, которые были удалены.
            var filteredRoleKinds = lenspec.EtalonDatabooks.RoleKinds.GetAll();
            // Отфильтруем роли по Регламентам, если они указаны.
            if (approvalRules.Value.Any())
            {
              var roleKindsToRemove  = new List<lenspec.EtalonDatabooks.IRoleKind>();
              // Вначале найдем все Настройки согласования, которые входят в выбранные регламенты.
              var approvalSettings = lenspec.EtalonDatabooks.ApprovalSettings.GetAll();
              approvalSettings = approvalSettings.Where(x => selectedApprovalRules.Contains(x.ApprovalRule));
              foreach (var roles in approvalSettings)
                foreach(var role in roles.RoleKindEmployee)
                  roleKindsToRemove.Add(role.RoleKind);

              var currentRoleKinds = roleKinds.Value.ToList();
              foreach (var rk in roleKindsToRemove)
                currentRoleKinds.Remove(rk);
              
              roleKinds.Value = currentRoleKinds;
              selectedRoleKindsText.Value = string.Join("; ", roleKinds.Value.Select(x => x.Name));
            }
          }
        });
      
      #endregion
      
      #region Роли
      
      addRoleKindsLink.SetOnExecute(
        () =>
        {
          var filteredRoleKinds = lenspec.EtalonDatabooks.RoleKinds.GetAll(r => !roleKinds.Value.Contains(r));
          
          // Отфильтруем роли по Регламентам, если они указаны.
          if (approvalRules.Value.Any())
          {
            var rolesFiltered  = new List<lenspec.EtalonDatabooks.IRoleKind>();
            // Вначале найдем все Настройки согласования, которые входят в выбранные регламенты.
            var approvalSettings = lenspec.EtalonDatabooks.ApprovalSettings.GetAll();
            approvalSettings = approvalSettings.Where(x => approvalRules.Value.Contains(x.ApprovalRule));
            // Если список настроек не пустой, то отфильтруем по нему роли.
            if (approvalSettings.Any())
            {
              foreach (var roles in approvalSettings)
                foreach(var role in roles.RoleKindEmployee)
                  rolesFiltered.Add(role.RoleKind);
              
              if (rolesFiltered.Any())
                filteredRoleKinds = filteredRoleKinds.Where(x => rolesFiltered.Contains(x));
            }
          }
          
          var selectedRoleKinds = filteredRoleKinds.ShowSelectMany().AsEnumerable();
          if (selectedRoleKinds != null && selectedRoleKinds.Any())
          {
            var sourceRoleKinds = roleKinds.Value.ToList();
            sourceRoleKinds.AddRange(selectedRoleKinds);
            roleKinds.Value = sourceRoleKinds.Distinct();

            selectedRoleKindsText.Value = string.Join("; ", roleKinds.Value.Select(x => x.Name));
          }
        });
      deleteRoleKindsLink.SetOnExecute(
        () =>
        {
          var selectedRoleKinds = roleKinds.Value.ShowSelectMany("Выберите роли для исключения");
          if (selectedRoleKinds != null && selectedRoleKinds.Any())
          {
            var currentRoleKinds = roleKinds.Value.ToList();
            foreach (var roleKind in selectedRoleKinds)
            {
              currentRoleKinds.Remove(roleKind);
            }
            roleKinds.Value = currentRoleKinds;
            selectedRoleKindsText.Value = string.Join("; ", roleKinds.Value.Select(x => x.Name));
          }
        });
      
      #endregion
      
      #region Проверка состояния
      
      actionStatus.SetOnValueChanged(x =>
                                     {
                                       if (x.NewValue == x.OldValue)
                                         return;
                                       
                                       if (actionStatus.Value == null)
                                         return;
                                       
                                       // Фильтрация регламентов по наличию в правилах согласования с выбранным статусом.
                                       var approvalSettingsStatus = actionStatus.Value == "Действующая" ?
                                         lenspec.EtalonDatabooks.ApprovalSetting.Status.Active :
                                         lenspec.EtalonDatabooks.ApprovalSetting.Status.Closed;
                                       // Фильтрация настроек согласования по статусу.
                                       var approvalSettings = lenspec.EtalonDatabooks.ApprovalSettings.GetAll(s => Equals(s.Status, approvalSettingsStatus));
                                       // Оставляем те регламенты согласования, которые есть в настройках согласования с выбранным статусом.
                                       // Если нет правила согласования для регламента, он отфильтруется.
                                       approvalRules.Value = approvalRules.Value.Where(ar => approvalSettings.Any(s => Equals(s.ApprovalRule, ar)));
                                       selectedApprovalRulesText.Value = string.Join("; ", approvalRules.Value.Select(r => r.Name).OrderBy(n => n));
                                       
                                       // Фильтрация ролей.
                                       var filteredRoleKinds = lenspec.EtalonDatabooks.RoleKinds.GetAll();
                                       // Отфильтруем роли по Регламентам, если они указаны.
                                       if (!approvalRules.Value.Any())
                                         return;
                                       var availableRoleKinds = new HashSet<lenspec.EtalonDatabooks.IRoleKind>();
                                       // Вначале найдем все настройки согласования, в которые входят отфильтрованные регламенты.
                                       approvalSettings = lenspec.EtalonDatabooks.ApprovalSettings.GetAll(a => approvalRules.Value.Contains(a.ApprovalRule));
                                       foreach (var roles in approvalSettings)
                                         foreach(var role in roles.RoleKindEmployee)
                                           availableRoleKinds.Add(role.RoleKind);
                                       // Оставляем роли, доступные в правилах согласования для отфильтрованных регламентов.
                                       roleKinds.Value = roleKinds.Value.Where(rk => availableRoleKinds.Contains(rk));
                                       selectedRoleKindsText.Value = string.Join("; ", roleKinds.Value.Select(n => n.Name));
                                     });
      #endregion
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        ReconciliationSettings.businessUnits.AddRange(businessUnits.Value.ToList());
        ReconciliationSettings.approvalRules.AddRange(approvalRules.Value.ToList());
        ReconciliationSettings.roleKinds.AddRange(roleKinds.Value.ToList());
        ReconciliationSettings.status = actionStatus.Value;
        ReconciliationSettings.reportSessionId = reportSessionId;
      }
      else
      {
        e.Cancel = true;
      }
    }
  }
}