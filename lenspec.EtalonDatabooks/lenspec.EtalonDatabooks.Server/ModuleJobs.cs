using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.EtalonDatabooks.Server
{
  public class ModuleJobs
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Актуализировать версии регламентов в справочнике Настройки согласования.
    /// </summary>
    public virtual void UpdateApprovalSettings()
    {
      var failedSettings = new List<lenspec.EtalonDatabooks.IApprovalSetting>();
      var approvalSettings = lenspec.EtalonDatabooks.ApprovalSettings.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
        .Where(x => x.ApprovalRule.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
        .AsEnumerable();
      foreach (var setting in approvalSettings)
      {
        var activeVersion = Sungero.Docflow.Server.ApprovalRuleBaseFunctions.GetAllRuleVersions(setting.ApprovalRule)
          .LastOrDefault(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
        if (activeVersion != null)
        {
          var asyncHandler = EtalonDatabooks.AsyncHandlers.AsyncUpdateApprovalSetting.Create();
          asyncHandler.ApprovalSettingId = setting.Id;
          asyncHandler.ApprovalRuleId = activeVersion.Id;
          asyncHandler.ExecuteAsync();
        }
        else
          failedSettings.Add(setting);
      }
      if (failedSettings.Any())
      {
        var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
        var task = Sungero.Workflow.SimpleTasks.Create(lenspec.EtalonDatabooks.Resources.ApprovalSettingsNotificationSubject, administratorEDMSRole);
        task.NeedsReview = false;
        task.ActiveText = lenspec.EtalonDatabooks.Resources.ApprovalSettingsNotificationActiveText;
        foreach (var setting in failedSettings)
        {
          task.Attachments.Add(setting);
        }
        task.Start();
      }
    }
    //конец Добавлено Avis Expert

  }
}