using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.ApprovingCounterpartyDEB.Server
{
  public class ModuleAsyncHandlers
  {
    /// <summary>
    /// Обновить записи справочника Реестр подрядчиков по согласованной организации.
    /// </summary>
    public virtual void UpdateContractorRegister(avis.ApprovingCounterpartyDEB.Server.AsyncHandlerInvokeArgs.UpdateContractorRegisterInvokeArgs args)
    {
      var contractorIdArray = args.ContractorRegisterIds.Split(',');
      var contractorRegistries = lenspec.Tenders.ContractorRegisters.GetAll(x => contractorIdArray.Contains(x.Id.ToString()));
      var company = lenspec.Etalon.Companies.Get(args.CompanyId);
      
      Logger.DebugFormat("Обновление записей справочника Реестр подрядчиков по контрагенту {0}. Старт обработчика.", company.Name);
      
      foreach (var entry in contractorRegistries)
      {
        try
        {
          if (Locks.TryLock(entry))
          {
            entry.ResultApprovalDEB = company.ResultApprovalDEBavis;
            entry.ResponsibleDEB = company.ResponsibleDEBavis;
            entry.InspectionDateDEB = company.InspectionDateDEBavis;
            entry.ApprovalDeadline = company.ApprovalPeriodavis;
            entry.Save();
          }
          else
          {
            Logger.DebugFormat("Обновление записей справочника Реестр подрядчиков по контрагенту {0}. Не удалось установить блокировку на запись с ИД {1}, произведется повторная попытка.", company.Name, entry.Id);
            args.Retry = true;
          }
        }
        catch (Exception ex)
        {
          var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
          Logger.ErrorFormat("Обновление записей справочника Реестр подрядчиков по контрагенту {0}. Ошибка обновления записи с ИД = {1}. {2}. {3}", company.Name, entry.Id, ex.Message, innerExceptionMessage);
          args.Retry = true;
        }
        finally
        {
          Locks.Unlock(entry);
        }
      }
      
      Logger.DebugFormat("Обновление записей справочника Реестр подрядчиков по контрагенту {0}. Завершение обработчика.", company.Name);
    }

    /// <summary>
    /// Обновить записи справочника Реестр подрядчиков по согласованной организации.
    /// </summary>
    public virtual void UpdateProviderRegister(avis.ApprovingCounterpartyDEB.Server.AsyncHandlerInvokeArgs.UpdateProviderRegisterInvokeArgs args)
    {
      var providerIdArray = args.ProviderRegisterIds.Split(',');
      var providerRegistries = lenspec.Tenders.ProviderRegisters.GetAll(x => providerIdArray.Contains(x.Id.ToString()));
      var company = lenspec.Etalon.Companies.Get(args.CompanyId);
      
      Logger.DebugFormat("Обновление записей справочника Реестр поставщиков по контрагенту {0}. Старт обработчика.", company.Name);
      
      foreach (var entry in providerRegistries)
      {
        try
        {
          if (Locks.TryLock(entry))
          {
            entry.ResultApprovalDEB = company.ResultApprovalDEBavis;
            entry.ResponsibleDEB = company.ResponsibleDEBavis;
            entry.InspectionDateDEB = company.InspectionDateDEBavis;
            entry.ApprovalDeadline = company.ApprovalPeriodavis;
            entry.Save();
          }
          else
          {
            Logger.DebugFormat("Обновление записей справочника Реестр поставщиков по контрагенту {0}. Не удалось установить блокировку на запись с ИД {1}, произведется повторная попытка.", company.Name, entry.Id);
            args.Retry = true;
          }
        }
        catch (Exception ex)
        {
          var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
          Logger.ErrorFormat("Обновление записей справочника Реестр поставщиков по контрагенту {0}. Ошибка обновления записи с ИД = {1}. {2}. {3}", company.Name, entry.Id, ex.Message, innerExceptionMessage);
          args.Retry = true;
        }
        finally
        {
          Locks.Unlock(entry);
        }
      }
      
      Logger.DebugFormat("Обновление записей справочника Реестр поставщиков по контрагенту {0}. Завершение обработчика.", company.Name);
    }

  }
}