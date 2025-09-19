using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PowerOfAttorneyModule.Server
{
  public class ModuleAsyncHandlers
  {

    public virtual void ChangingStatusExpiredPOA(avis.PowerOfAttorneyModule.Server.AsyncHandlerInvokeArgs.ChangingStatusExpiredPOAInvokeArgs args)
    {
      // Доверки и права подписи обрабатываются в одном АО, т.к. записей будет не много и рестарт АО не нагрузит сисетму
      Logger.Debug("Avis - ChangingStatusExpiredPOA. Старт смены статуса у просроченных доверенностей и связанных прав подписи.");
      
      // Доверенности
      IQueryable<Sungero.Docflow.IPowerOfAttorneyBase> powerOfAttorneys = null;
      if (!string.IsNullOrWhiteSpace(args.PowerOfAttorneyIds))
      {
        var powerOfAttorneyIds = args.PowerOfAttorneyIds.Split(',').Select(x => long.Parse(x));
        powerOfAttorneys = Sungero.Docflow.PowerOfAttorneyBases.GetAll(x => powerOfAttorneyIds.Contains(x.Id));
        foreach (var powerOfAttorney in powerOfAttorneys)
        {
          try
          {
            if (powerOfAttorney.LifeCycleState == Sungero.Docflow.PowerOfAttorneyBase.LifeCycleState.Obsolete)
              continue;
            powerOfAttorney.LifeCycleState = Sungero.Docflow.PowerOfAttorneyBase.LifeCycleState.Obsolete;
            powerOfAttorney.Save();
          }
          catch (Exception ex)
          {
            Logger.ErrorFormat("Avis - ChangingStatusExpiredPOA. Ошибка обработки доверенности с ИД = {0}: {1}", powerOfAttorney.Id, ex.Message);
            args.Retry = true;
          }
        }
      }

      // Права подписи
      if (!string.IsNullOrWhiteSpace(args.SignatureSettingIds))
      {
        var signatureSettingIds = args.SignatureSettingIds.Split(',').Select(x => long.Parse(x));
        foreach (var signatureSettingId in signatureSettingIds)
        {
          try
          {
            var signatureSetting = Sungero.Docflow.SignatureSettings.Get(signatureSettingId);
            if (signatureSetting.Status == Sungero.Docflow.SignatureSetting.Status.Closed)
              continue;
            signatureSetting.Status = Sungero.Docflow.SignatureSetting.Status.Closed;
            signatureSetting.Save();
          }
          catch (Exception ex)
          {
            Logger.ErrorFormat("Avis - ChangingStatusExpiredPOA. Ошибка обработки права подписи с ИД = {0}: {1}", signatureSettingId, ex.Message);
            args.Retry = true;
          }
        }
      }
      
      // Отправка уведомления Администратору СЭД
      if (!args.Retry)
      {
        var adminEDMsRole = Roles.GetAll(x => x.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).SingleOrDefault();
        var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices("Доверенности с истекшим сроком действия обработаны.", adminEDMsRole);
        if (args.RetryIteration < 10)
        {
          if (powerOfAttorneys != null && powerOfAttorneys.Any())
          {
            notice.ActiveText = "Смена состояния у просроченных доверенностей и связанных прав подписи завершена.";
            foreach (var powerOfAttorney in powerOfAttorneys)
            {
              notice.Attachments.Add(powerOfAttorney);
            }
          }
          else
            notice.ActiveText = "Просроченные доверенности не найдены в системе.";
        }
        else
          notice.ActiveText = "У некоторых просроченных доверенностей не удалось изменить состояние, проверьте лог.";
        
        notice.Start();
      }
      
      Logger.Debug("Avis - ChangingStatusExpiredPOA. Конец смены статуса у просроченных доверенностей и связанных прав подписи.");
    }
    
    

  }
}