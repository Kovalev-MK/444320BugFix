using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.EtalonDatabooks.Server
{
  public class ModuleAsyncHandlers
  {
    
    /// <summary>
    /// Асинхронный обработчик для изменения полей Типовой и ИД шаблона в карточке договорного документа.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncChangeContractualDocumentIsStandardProperty(lenspec.EtalonDatabooks.Server.AsyncHandlerInvokeArgs.AsyncChangeContractualDocumentIsStandardPropertyInvokeArgs args)
    {
      var document = lenspec.Etalon.ContractualDocuments.GetAll(x => x.Id == args.DocumentId).FirstOrDefault();
      var logPrefix = lenspec.EtalonDatabooks.Resources.UpdateContractualDocumentIsStandardPropertyMessage;
      if (document == null)
      {
        var message = lenspec.EtalonDatabooks.Resources.CantFindContractualDocumentErrorMessageFormat(logPrefix, Etalon.ContractualDocuments.Info.LocalizedName, args.DocumentId);
        Logger.Error(message);
        return;
      }
      
      // Попытка установить блокировку сущности.
      var isForcedLocked = false;
      try
      {
        isForcedLocked = Locks.TryLock(document);
        if (isForcedLocked)
        {
          document.IsStandard = false;
          document.TemplateIDlenspec = null;
          document.Save();
        }
        else
        {
          var error = lenspec.EtalonDatabooks.Resources.FailLockErrorMessageFormat(logPrefix, args.DocumentId, Locks.GetLockInfo(document).LockedMessage);
          args.Retry = true;
          Logger.Error(error);
        }
      }
      catch (Exception ex)
      {
        Logger.Error(logPrefix, ex);
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(document);
      }
    }

    /// <summary>
    /// Асинхронный обработчик для обновления поля ИД шаблона в карточке договорного документа.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncChangeContractualDocumentTemplateIdProperty(lenspec.EtalonDatabooks.Server.AsyncHandlerInvokeArgs.AsyncChangeContractualDocumentTemplateIdPropertyInvokeArgs args)
    {
      var document = lenspec.Etalon.ContractualDocuments.GetAll(x => x.Id == args.documentId).FirstOrDefault();
      var logPrefix = lenspec.EtalonDatabooks.Resources.UpdateContractualDocumentMessage;
      if (document == null)
      {
        var message = lenspec.EtalonDatabooks.Resources.CantFindContractualDocumentErrorMessageFormat(logPrefix, Etalon.ContractualDocuments.Info.LocalizedName, args.documentId);
        Logger.Error(message);
        return;
      }
      
      // Попытка установить блокировку сущности.
      var isForcedLocked = false;
      try
      {
        isForcedLocked = Locks.TryLock(document);
        if (isForcedLocked)
        {
          document.TemplateIDlenspec = args.templateId;
          document.Save();
        }
        else
        {
          var error = lenspec.EtalonDatabooks.Resources.FailLockErrorMessageFormat(logPrefix, args.documentId, Locks.GetLockInfo(document).LockedMessage);
          args.Retry = true;
          Logger.Error(error);
        }
      }
      catch (Exception ex)
      {
        Logger.Error(logPrefix, ex);
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(document);
      }
    }

    /// <summary>
    /// Асинхронный обработчик для обновления карточки Настройки согласования.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncUpdateApprovalSetting(lenspec.EtalonDatabooks.Server.AsyncHandlerInvokeArgs.AsyncUpdateApprovalSettingInvokeArgs args)
    {
      var isForcedLocked = false;
      Logger.DebugFormat("Avis - AsyncUpdateApprovalSetting - обновление настройки согласования {0}", args.ApprovalSettingId);
      
      var setting = EtalonDatabooks.ApprovalSettings.GetAll(x => x.Id == args.ApprovalSettingId).SingleOrDefault();
      if (setting == null)
      {
        Logger.ErrorFormat("Avis - AsyncUpdateApprovalSetting - настройка согласования {0} не найдена.", args.ApprovalSettingId);
        args.Retry = false;
        return;
      }
      
      var approvalRule = Sungero.Docflow.ApprovalRuleBases.GetAll(x => x.Id == args.ApprovalRuleId).SingleOrDefault();
      if (approvalRule == null)
      {
        Logger.ErrorFormat("Avis - AsyncUpdateApprovalSetting - регламент {0} не найден.", args.ApprovalRuleId);
        args.Retry = false;
        return;
      }
      
      try
      {
        var lockinfo = Locks.GetLockInfo(setting);
        if (lockinfo != null && lockinfo.IsLocked)
        {
          args.Retry = true;
          Logger.ErrorFormat("Avis - AsyncUpdateApprovalSetting - настройка согласования {0} заблокирована пользователем {1}.", setting.Id, lockinfo.OwnerName);
          return;
        }
        else
          isForcedLocked = Locks.TryLock(setting);
        
        setting.ApprovalRule = approvalRule;
        setting.Save();
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - AsyncUpdateApprovalSetting - не удалось обновить настройку согласования {0} ", ex, setting.Id);
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(setting);
      }
    }

    /// <summary>
    /// Асинхронный обработчик для заполнения поля ИСП в карточке Помещения
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncObjectAnSaleOurCFFilling(lenspec.EtalonDatabooks.Server.AsyncHandlerInvokeArgs.AsyncObjectAnSaleOurCFFillingInvokeArgs args)
    {
      try
      {
        Logger.Debug("Avis - AsyncObjectAnSaleOurCFFilling started");
        var objectAnProject = EtalonDatabooks.ObjectAnProjects.GetAll(x => x.Id == args.ObjectAnProjectId).SingleOrDefault();
        if (objectAnProject == null)
        {
          Logger.ErrorFormat("Avis - AsyncObjectAnSaleAddressFilling - объект проекта {0} не найден.", args.ObjectAnProjectId);
          args.Retry = false;
          return;
        }
        var objectsAnSale = EtalonDatabooks.ObjectAnSales.GetAll().Where(x => objectAnProject.Equals(x.ObjectAnProject));
        foreach(var item in objectsAnSale)
        {
          var objectAnSale = EtalonDatabooks.ObjectAnSales.Get(item.Id);
          objectAnSale.OurCF = objectAnProject.OurCF;
          objectAnSale.Save();
        }
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - AsyncObjectAnSaleOurCFFilling - ", ex);
      }
    }

    /// <summary>
    /// Асинхронный обработчик для заполнения поля Адрес в карточке Помещения
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncObjectAnSaleAddressFilling(lenspec.EtalonDatabooks.Server.AsyncHandlerInvokeArgs.AsyncObjectAnSaleAddressFillingInvokeArgs args)
    {
      try
      {
        Logger.Debug("Avis - AsyncObjectAnSaleAddressFilling started");
        var objectAnProject = EtalonDatabooks.ObjectAnProjects.GetAll(x => x.Id == args.ObjectAnProjectId).SingleOrDefault();
        if (objectAnProject == null)
        {
          Logger.ErrorFormat("Avis - AsyncObjectAnSaleAddressFilling - объект проекта {0} не найден.", args.ObjectAnProjectId);
          args.Retry = false;
          return;
        }
        var objectsAnSale = EtalonDatabooks.ObjectAnSales.GetAll().Where(x => objectAnProject.Equals(x.ObjectAnProject));
        foreach(var item in objectsAnSale)
        {
          var objectAnSale = EtalonDatabooks.ObjectAnSales.Get(item.Id);
          objectAnSale.Address = string.Empty;
          if (!string.IsNullOrEmpty(objectAnProject.AddressMail))
          {
            objectAnSale.Address = objectAnProject.AddressMail + ", ";
          }
          if (objectAnSale.PurposeOfPremises != null && !string.IsNullOrEmpty(objectAnSale.PurposeOfPremises.ShortName))
          {
            objectAnSale.Address += objectAnSale.PurposeOfPremises.ShortName;
            if (!string.IsNullOrEmpty(objectAnSale.NumberRoomMail))
            {
              objectAnSale.Address += " ";
            }
          }
          if (!string.IsNullOrEmpty(objectAnSale.NumberRoomMail))
          {
            objectAnSale.Address += objectAnSale.NumberRoomMail;
          }
          objectAnSale.Save();
        }
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - AsyncObjectAnSaleAddressFilling - ", ex);
      }
    }

  }
}