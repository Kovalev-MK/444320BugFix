using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonParties.Server
{
  public class ModuleJobs
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Автоматическое заполнение поля Контрагент в Сведениях о контрагенте.
    /// </summary>
    public virtual void FillCounterpartyDocumentFields()
    {
      var activeCounterpartyDocuments = Sungero.Docflow.CounterpartyDocuments.GetAll(x => x.Counterparty == null &&
                                                                                     x.LifeCycleState == Sungero.Docflow.OfficialDocument.LifeCycleState.Active);
      var counterpartyDocumentTypeGuid = Guid.Parse(lenspec.Etalon.Module.Docflow.PublicConstants.Module.CounterpartyDocumentTypeGuid);
      var groupId = lenspec.Etalon.Module.Docflow.PublicConstants.Module.TaskGroups.ApprovalCounterpartyPersonDEBTaskCounterpartyInfoGroup;
      var approvalCounterpartyPersonDEBs = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.GetAll();
      foreach (var counterpartyDocument in activeCounterpartyDocuments)
      {
        var task = approvalCounterpartyPersonDEBs
          .Where(x => x.AttachmentDetails.Any(a => a.GroupId == groupId && a.EntityTypeGuid == counterpartyDocumentTypeGuid && a.AttachmentId == counterpartyDocument.Id))
          .OrderByDescending(x => x.Created)
          .FirstOrDefault();
        
        if (task != null)
        {
          var asyncHandler = avis.EtalonParties.AsyncHandlers.AsyncFillCounterpartyDocument.Create();
          asyncHandler.CounterpartyDocumentId = counterpartyDocument.Id;
          asyncHandler.CounterpartyId = task.Counterparty.Id;
          asyncHandler.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Закрытие всех записей в справочнике Банковские реквизиты
    /// </summary>
    public virtual void ClosingBankDetails()
    {
      Functions.Module.CloseBankDetailsAsync();
    }
    
    /// <summary>
    /// Форматирует строку для вставки в лог
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    [Public]
    public static string FormatLineForlog(string line)
    {
      return string.Format("{0}\t{1}\r\n", Calendar.Now.ToString("dd.MM.yyyy HH:mm:ss"), line);
    }
    
    /// <summary>
    /// Выгрузка банков из Dadata.ru.
    /// </summary>
    public virtual void ImportBanksFromDadata()
    {
      var setting = lenspec.Etalon.Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.DadataCode).FirstOrDefault();
      if (setting == null || string.IsNullOrEmpty(setting.ConnectionParams))
        throw AppliedCodeException.Create("Не найдены настройки подключения.");
      
      var settingsString = Encryption.Decrypt(setting.ConnectionParams);
      var connectionString = settingsString.Split(';');
      if (string.IsNullOrEmpty(connectionString[0]))
        throw AppliedCodeException.Create("Неверный токен.");
      
      string logtext = FormatLineForlog("Выгрузка банков из Dadata.ru");
      var success = true;
      var token = connectionString[0];
      DateTime startDate = Calendar.Now;
      try
      {
        var banks = lenspec.Etalon.Banks.GetAll(x => x.Status == lenspec.Etalon.Bank.Status.Active);
        foreach(var bank in banks)
        {
          if (string.IsNullOrEmpty(bank.BIC) && string.IsNullOrEmpty(bank.TIN) && string.IsNullOrEmpty(bank.TRRC))
          {
            logtext += FormatLineForlog(string.Format("Банк {0} - не заполнены БИК, ИНН, КПП.", bank.Name));
            continue;
          }
          Transactions.Execute(
            () => {
              try
              {
                var error = lenspec.Etalon.Module.Integration.PublicFunctions.Module.FillBankRequisitesFromDadata(bank, token);
                if (!string.IsNullOrEmpty(error))
                  logtext += FormatLineForlog(string.Format("ОШИБКА - Банк {0} - {1}", bank.Name, error));
                if (bank.State.IsChanged)
                  bank.Save();
              }
              catch(Exception ex)
              {
                Logger.Error("Avis - ImportBanksFromDadata - ", ex);
                success = false;
                logtext += FormatLineForlog(string.Format("ОШИБКА - Банк {0} - {1} {2}", bank.Name, ex.Message, ex.StackTrace));
              }
            });
        }
        logtext += FormatLineForlog("Выгрузка завершена");
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - ImportBanksFromDadata - ", ex);
        success = false;
        logtext += FormatLineForlog(string.Format("ОШИБКА - {0} {1}", ex.Message, ex.StackTrace));
      }
      finally
      {
        var asyncSaveResult = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncImportOrgStructureSaveResultlenspec.Create();
        asyncSaveResult.SettingId = setting.Id;
        asyncSaveResult.StartDate = startDate;
        asyncSaveResult.EndDate = Calendar.Now;
        asyncSaveResult.Success = success;
        asyncSaveResult.TextLog = logtext;
        asyncSaveResult.ExecuteAsync();
      }
    }

    /// <summary>
    /// Отслеживание окончания срока согласования контрагентов и персон.
    /// </summary>
    public virtual void TrackingTheApprovalOfCounterparties()
    {
      var expirationPeriod = Constants.Module.SendNotificationAboutExpirationCounterpatyAfterDays;
      var expirationDate = Calendar.Today.AddDays(expirationPeriod);
      var expirationCounterparties = lenspec.Etalon.Counterparties.GetAll(x => x.ApprovalPeriodavis.HasValue && x.ApprovalPeriodavis.Value == expirationDate);
      
      foreach (var counterparty in expirationCounterparties)
      {
        var performers = avis.EtalonParties.ResponsibleByCounterparties
          .GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && x.Counterparty == counterparty && x.Responsible.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
          .Select(x => x.Responsible)
          .Distinct()
          .ToList();
        if (!performers.Any())
          continue;
        
        foreach (var performer in performers)
        {
          try
          {
            var task = Sungero.Workflow.SimpleTasks.CreateWithNotices(avis.EtalonParties.Resources.TrackingTheApprovalOfCounterpartiesNoticeSubjectFormat(counterparty.Name), performer);
            task.ActiveText = avis.EtalonParties.Resources.ExpirationCounterpartyNotificationActiveText;
            task.Attachments.Add(counterparty);
            task.Start();
          }
          catch(Exception ex)
          {
            Logger.Error("Avis - TrackingTheApprovalOfCounterparties - failed to send notification to responsible by counterparty {0}, performer {1}", ex, counterparty.Id, performer.Id);
          }
        }
      }
    }
    //конец Добавлено Avis Expert

  }
}