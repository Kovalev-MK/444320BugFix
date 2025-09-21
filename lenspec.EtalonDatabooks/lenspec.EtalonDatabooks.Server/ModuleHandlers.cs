using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.EtalonDatabooks.Server
{
  partial class MassSendingOutgoingOtherDeliveryMethodsFolderHandlers
  {

    public virtual bool IsMassSendingOutgoingOtherDeliveryMethodsVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> MassSendingOutgoingOtherDeliveryMethodsDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.MassSendOutgOth);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      return result;
    }
  }

  partial class MassSendingOutgoingEmailsFolderHandlers
  {

    public virtual bool IsMassSendingOutgoingEmailsVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> MassSendingOutgoingEmailsDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.MassSendOutgEma);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      return result;
    }
  }

  partial class RegistrationIncomingLetterFolderHandlers
  {

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> RegistrationIncomingLetterDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.RegistrationIncomingLetter);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }

    public virtual bool IsRegistrationIncomingLetterVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
  }

  partial class RegistrationMassMailingApplicationFolderHandlers
  {

    public virtual bool IsRegistrationMassMailingApplicationVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> RegistrationMassMailingApplicationDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.RegistrationMassMailingApplication);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }

  partial class SendingOutgoingOtherDeliveryMethodsFolderHandlers
  {

    public virtual bool IsSendingOutgoingOtherDeliveryMethodsVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
    
    /// <summary>
    /// Получить данные для папки потока Отправка исх. писем (прочими способами).
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> SendingOutgoingOtherDeliveryMethodsDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.SendingOutgoingOther);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }

  partial class ApplicationCreatingChangingCounterpartiesFolderHandlers
  {

    public virtual bool IsApplicationCreatingChangingCounterpartiesVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
    
    /// <summary>
    /// Получить данные для папки потока Заявка на создание/изменение контрагентов и персон.
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> ApplicationCreatingChangingCounterpartiesDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.ApplicationCreatingChangingCounterparties);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }

  partial class RegistrationProtocolCollegialBodyFolderHandlers
  {

    public virtual bool IsRegistrationProtocolCollegialBodyVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
    
    /// <summary>
    /// Получить данные для папки потока Регистрация протоколов (ТК, КА, Коллег. органов).
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> RegistrationProtocolCollegialBodyDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.RegistrationProtocolCollegialBody);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }

  partial class VerificationProtocolCollegialBodyFolderHandlers
  {

    public virtual bool IsVerificationProtocolCollegialBodyVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
    
    /// <summary>
    /// Получить данные для папки потока Проверка протоколов (ТК, КА, Коллег. органов).
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> VerificationProtocolCollegialBodyDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.VerificationProtocolCollegialBody);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }

  partial class RegistrationLocalActFolderHandlers
  {

    public virtual bool IsRegistrationLocalActVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
    
    /// <summary>
    /// Получить данные для папки потока Регистрация локального акта.
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> RegistrationLocalActDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.RegistrationLocalAct);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }

  partial class CheckingLocalActFolderHandlers
  {

    public virtual bool IsCheckingLocalActVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
    
    /// <summary>
    /// Получить данные для папки потока Проверка локального акта.
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> CheckingLocalActDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.CheckingLocalAct);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }

  partial class ApprovalLocalActFolderHandlers
  {

    public virtual bool IsApprovalLocalActVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
    
    /// <summary>
    /// Получить данные для папки потока Согласование локального акта.
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> ApprovalLocalActDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.ApprovalLocalAct);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
      
      //return EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.ApprovalLocalAct);
    }
  }

  partial class SendingOutgoingEmailsFolderHandlers
  {

    public virtual bool IsSendingOutgoingEmailsVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
    
    /// <summary>
    /// Получить данные для папки потока Отправка исх. писем (эл. почтой).
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> SendingOutgoingEmailsDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.SendingOutgoingEmails);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }

  partial class RegistrationOutgoingEmailsFolderHandlers
  {

    public virtual bool IsRegistrationOutgoingEmailsVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
    
    /// <summary>
    /// Получить данные для папки потока Регистрация исходящих писем.
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> RegistrationOutgoingEmailsDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.RegistrationOutgoingEmails);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }

  partial class CheckingDesignOutgoingEmailsFolderHandlers
  {

    public virtual bool IsCheckingDesignOutgoingEmailsVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }
    
    /// <summary>
    /// Получить данные для папки потока Проверка оформления исходящих писем.
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> CheckingDesignOutgoingEmailsDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.CheckingDesignOutgoingEmails);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }

  partial class AppDispatchCourierMailFolderHandlers
  {

    public virtual bool IsAppDispatchCourierMailVisible()
    {
      var role = Roles.GetAll(r => r.Sid == Constants.Module.FoldersOffice).SingleOrDefault();
      return role != null && Users.Current.IncludedIn(role);
    }

    /// <summary>
    /// Получить данные для папки потока Заявки на отправление курьерской почтой.
    /// </summary>
    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> AppDispatchCourierMailDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.AppDispatchCourierMail);
      
      // Запрос непрочитанных без фильтра.
      if (_filter == null)
        return Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result);
      
      // Фильтры по статусу, замещению и периоду.
      result = Sungero.RecordManagement.PublicFunctions.Module.ApplyCommonSubfolderFilters(result, _filter.InProcess,
                                                                                           _filter.Last30Days, _filter.Last90Days, _filter.Last180Days, false);
      
      return result;
    }
  }




  partial class EtalonDatabooksHandlers
  {
  }
}