using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB;

namespace avis.ApprovingCounterpartyDEB.Server
{
  partial class ApprovalCounterpartyPersonDEBRouteHandlers
  {

    #region Предпологаемая сумма больше 300000?
    
    public virtual bool Decision27Result()
    {
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.FirstOrDefault();
      var castedDocument = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEBs.As(document);
      return castedDocument != null && castedDocument.EstimatedAmountTransaction.HasValue && castedDocument.EstimatedAmountTransaction.Value >= Constants.ApprovalCounterpartyBankDEB.MinAmountVerification;
    }
    
    #endregion
    
    #region Уведомление старта согласования для роли 'Уведомляемые по согласование контрагентов, банков и персон с ДБ'
    
    public virtual void StartBlock25(Sungero.Workflow.Server.NoticeArguments e)
    {
      e.Block.Subject = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEBs.Resources.DBApprovalTaskOnStartNotificationFormat(_obj.Counterparty);
      var role = Roles.GetAll(x => x.Sid == Constants.Module.NotifiedMembersApprovalDEB).SingleOrDefault();
      e.Block.Performers.Add(role);
    }
    
    #endregion
    
    #region Заполнение полей вложений

    public virtual void Script22Execute()
    {
      var schemeVersion = _obj.GetStartedSchemeVersion();
      
      //cвойства вложений
      var mainDocument = _obj.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
      var databook = _obj.ApprovalDatabook.Counterparties.SingleOrDefault();
      
      //общие свойства вложений
      databook.LimitPerCounterpartyavis = _obj.CounterpartyLimit;
      databook.InspectionDateDEBavis = _obj.Date;
      databook.ResponsibleDEBavis = _obj.ApprovalerDEB;
      databook.ApprovalPeriodavis = _obj.CompleteDate;
      mainDocument.DateApprovalDEB = _obj.Date;
      mainDocument.ResponsibleDEB = _obj.ApprovalerDEB;
      mainDocument.PeriodApprovalDEB = _obj.CompleteDate;
      
      if (_obj.ResultApprovalDEB == ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.CoopPossible)
      {
        databook.ResultApprovalDEBavis = lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopPossible;
        mainDocument.ResultApprovalDEB = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBase.ResultApprovalDEB.CoopPossible;
      }
      if (_obj.ResultApprovalDEB == ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.CoopWithRisks)
      {
        databook.ResultApprovalDEBavis = lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopWithRisks;
        mainDocument.ResultApprovalDEB = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBase.ResultApprovalDEB.CoopWithRisks;
      }
      if (_obj.ResultApprovalDEB == ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.CoopNotRecomend)
      {
        databook.ResultApprovalDEBavis = lenspec.Etalon.Counterparty.ResultApprovalDEBavis.CoopNotRecomend;
        mainDocument.ResultApprovalDEB = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBase.ResultApprovalDEB.CoopNotRecomend;
      }
      if (_obj.ResultApprovalDEB == ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.DoesNotReqAppr)
      {
        databook.ResultApprovalDEBavis = lenspec.Etalon.Counterparty.ResultApprovalDEBavis.DoesNotReqAppr;
        mainDocument.ResultApprovalDEB = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBase.ResultApprovalDEB.DoesNotReqAppr;
      }
      
      databook.Save();
      
      //если контрагент организация или банк
      var approvalCounterpartyBank = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEBs.As(mainDocument);
      if (approvalCounterpartyBank != null)
      {
        approvalCounterpartyBank.CounterpartyLimitDEB = _obj.CounterpartyLimit;
        approvalCounterpartyBank.Save();
        
        if (schemeVersion == LayerSchemeVersions.V1 || schemeVersion == LayerSchemeVersions.V2 || schemeVersion == LayerSchemeVersions.V3)
        {
          //если контрагент организация
          var company = lenspec.Etalon.Companies.As(databook);
          if (company != null)
          {
            if (!company.Materialsavis.Any())
            {
              foreach (var line in approvalCounterpartyBank.Materials)
              {
                var newLine = company.Materialsavis.AddNew();
                newLine.Material = line.Material;
                newLine.MaterialGroup = line.MaterialGroup;
                newLine.Comment = line.Comment;
              }
            }
            if (!company.WorkKindsAvisavis.Any())
            {
              foreach (var line in approvalCounterpartyBank.WorkKinds)
              {
                var newLine = company.WorkKindsAvisavis.AddNew();
                newLine.WorkKind = line.WorkKind;
                newLine.WorkGroup = line.WorkGroup;
                newLine.Comment = line.Comment;
              }
            }
            company.Save();
          }
          
          var childCompanies = lenspec.Etalon.Companies.GetAll(x => x.HeadCompany != null && x.HeadCompany.Equals(company));
          foreach (var childCompany in childCompanies)
          {
            childCompany.ResultApprovalDEBavis = company.ResultApprovalDEBavis;
            childCompany.InspectionDateDEBavis = company.InspectionDateDEBavis;
            childCompany.LimitPerCounterpartyavis = company.LimitPerCounterpartyavis;
            childCompany.ResponsibleDEBavis = company.ResponsibleDEBavis;
            childCompany.ApprovalPeriodavis = company.ApprovalPeriodavis;
            childCompany.Save();
          }
        }
        //если контрагент банк
        var bank = lenspec.Etalon.Banks.As(databook);
        if (bank != null && approvalCounterpartyBank != null)
        {
          var childBanks = lenspec.Etalon.Banks.GetAll(x => x.HeadBankavis != null && x.HeadBankavis.Equals(bank));
          foreach (var childBank in childBanks)
          {
            childBank.ResultApprovalDEBavis = bank.ResultApprovalDEBavis;
            childBank.InspectionDateDEBavis = bank.InspectionDateDEBavis;
            childBank.LimitPerCounterpartyavis = bank.LimitPerCounterpartyavis;
            childBank.ResponsibleDEBavis = bank.ResponsibleDEBavis;
            childBank.ApprovalPeriodavis = bank.ApprovalPeriodavis;
            childBank.Save();
          }
        }
      }
    }
    
    #endregion

    #region Контрагент компания или банк?
    
    public virtual bool Decision21Result()
    {
      var schemeVersion = _obj.GetStartedSchemeVersion();
      var databook = _obj.ApprovalDatabook.Counterparties.SingleOrDefault();
      return (schemeVersion == LayerSchemeVersions.V1 || schemeVersion == LayerSchemeVersions.V2 || schemeVersion == LayerSchemeVersions.V3) && lenspec.Etalon.CompanyBases.Is(databook);
    }
    
    #endregion

    #region Уведомление о установке лимита
    
    public virtual void StartBlock20(Sungero.Workflow.Server.NoticeArguments e)
    {
      e.Block.Subject = string.Format("Выполнено, для контрагента {0} установлен лимит", _obj.Counterparty?.Name);
      e.Block.Performers.Add(_obj.ApprovalerDEB);
    }
    
    #endregion
    
    #region Установка лимита по КА

    public virtual void EndBlock19(avis.ApprovingCounterpartyDEB.Server.SettingLimitAssignmentEndBlockEventArguments e)
    {
      var performerRole = Roles.GetAll(x => x.Sid == Constants.Module.ResponsibleLimitEconomist).SingleOrDefault();
      foreach (var atherDoc in _obj.AtherDocuments.OfficialDocuments)
      {
        if (Sungero.Docflow.SimpleDocuments.Is(atherDoc))
        {
          atherDoc.AccessRights.Revoke(performerRole, DefaultAccessRightsTypes.FullAccess);
          atherDoc.AccessRights.Save();
        }
      }
    }

    public virtual void CompleteAssignment19(avis.ApprovingCounterpartyDEB.ISettingLimitAssignment assignment, avis.ApprovingCounterpartyDEB.Server.SettingLimitAssignmentArguments e)
    {
      if (assignment.Result == avis.ApprovingCounterpartyDEB.SettingLimitAssignment.Result.Forward)
        assignment.Forward(assignment.Forward);
      
      if (assignment.Result == avis.ApprovingCounterpartyDEB.SettingLimitAssignment.Result.Set)
      {
        //Прекратить парарллельные задания
        var parallelAssignments = ApprovingCounterpartyDEB.PublicFunctions.Module.GetParallelAssignments(assignment);
        var activeParallelAssignments = parallelAssignments
          .Where(a => a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
          .Where(a => !Equals(a, assignment))
          .Where(a => SettingLimitAssignments.Is(a));
        foreach (var parallelAssignment in activeParallelAssignments)
        {
          if (!string.IsNullOrEmpty(parallelAssignment.ActiveText))
            parallelAssignment.ActiveText += Environment.NewLine;
          
          if (assignment.CompletedBy != null && !assignment.CompletedBy.Equals(assignment.Performer))
            parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserWithOnBehalfOfFormat(assignment.CompletedBy.Name,
                                                                                                                            assignment.Performer.Name);
          else
            parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserFormat(assignment.Performer.Name);

          parallelAssignment.Abort();
        }
        
        //Актуализация свойств
        _obj.CounterpartyLimit = assignment.Limit;
        _obj.Save();
      }
    }

    public virtual void StartAssignment19(avis.ApprovingCounterpartyDEB.ISettingLimitAssignment assignment, avis.ApprovingCounterpartyDEB.Server.SettingLimitAssignmentArguments e)
    {
      
    }

    public virtual void StartBlock19(avis.ApprovingCounterpartyDEB.Server.SettingLimitAssignmentArguments e)
    {
      e.Block.Subject = string.Format("Установление лимита для поставщиков и подрядчиков контрагента {0}", _obj.Counterparty?.Name);
      var performerRole = Roles.GetAll(x => x.Sid == Constants.Module.ResponsibleLimitEconomist).SingleOrDefault();
      e.Block.Performers.Add(performerRole);
      
      foreach (var atherDoc in _obj.AtherDocuments.OfficialDocuments)
      {
        if (Sungero.Docflow.SimpleDocuments.Is(atherDoc))
        {
          atherDoc.AccessRights.Grant(performerRole, DefaultAccessRightsTypes.FullAccess);
          atherDoc.AccessRights.Save();
        }
      }
    }
    
    #endregion

    #region Уведомления наблюдателям
    
    public virtual void StartBlock18(Sungero.Workflow.Server.NoticeArguments e)
    {
      if (_obj.Observers.Any())
      {
        var resultApproval = ApprovalCounterpartyPersonDEBs.Info.Properties.ResultApprovalDEB.GetLocalizedValue(_obj.ResultApprovalDEB);
        e.Block.Subject = string.Format("Согласование {0} завершено с результатом {1}.", _obj.Counterparty, resultApproval);
        foreach (var recipient in _obj.Observers)
          e.Block.Performers.Add(recipient.Observer);
      }
    }
    
    #endregion

    #region Уведомление об отклонении (служба безопасности)
    
    public virtual void StartBlock15(Sungero.Workflow.Server.NoticeArguments e)
    {
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
      document.ApprovalStateNew = ApprovingCounterpartyDEB.ApprovalCounterpartyBase.ApprovalStateNew.NotApproved;
      document.Save();
      
      e.Block.Subject = $"Контрагент {_obj.Counterparty} не согласован службой безопасности";
      e.Block.Performers.Add(_obj.Author);
    }
    
    #endregion

    #region Уведомление об исполнении (служба безопасности)
    
    public virtual void StartBlock14(Sungero.Workflow.Server.NoticeArguments e)
    {
      e.Block.Subject = $"Контрагент {_obj.Counterparty} согласован службой безопасности";
      e.Block.Performers.Add(_obj.Author);
    }
    
    #endregion

    #region Доработка согласующим ДБ
    
    public virtual void StartBlock13(avis.ApprovingCounterpartyDEB.Server.ReworkApprovalerAssignmentArguments e)
    {
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
      document.ApprovalStateNew = ApprovingCounterpartyDEB.ApprovalCounterpartyBase.ApprovalStateNew.OnRework;
      document.Save();
      
      e.Block.Subject = $"Доработка заявки на одобрение внешнего контрагента {_obj.Counterparty}";
      e.Block.Performers.Add(_obj.ApprovalerDEB);
    }
    
    #endregion

    #region Доработка инициатором
    
    public virtual void EndBlock12(avis.ApprovingCounterpartyDEB.Server.ReworkInitiatorAssignmentEndBlockEventArguments e)
    {
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
      document.AccessRights.Revoke(_obj.Author, DefaultAccessRightsTypes.Change);
      document.AccessRights.Save();
    }

    public virtual void StartBlock12(avis.ApprovingCounterpartyDEB.Server.ReworkInitiatorAssignmentArguments e)
    {
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
      document.ApprovalStateNew = ApprovingCounterpartyDEB.ApprovalCounterpartyBase.ApprovalStateNew.OnRework;
      document.Save();
      
      e.Block.RelativeDeadlineHours = 3;
      e.Block.Subject = $"Доработка заявки на одобрение внешнего контрагента {_obj.Counterparty}";
      document.AccessRights.Grant(_obj.Author, DefaultAccessRightsTypes.Change);
      document.AccessRights.Save();
      e.Block.Performers.Add(_obj.Author);
    }
    
    #endregion

    #region Уведомление об исполнении (включение в реестр)
    
    public virtual void StartBlock11(Sungero.Workflow.Server.NoticeArguments e)
    {
      e.Block.Subject = $"Выполнено, контрагент {_obj.Counterparty} добавлен в реестр поставщиков и подрядчиков.";
      var performer = _obj.ApprovalerDEB;
      e.Block.Performers.Add(performer);
    }
    
    #endregion

    #region Включение контрагента в реестр
    
    public virtual void EndBlock9(avis.ApprovingCounterpartyDEB.Server.IncludeCounterpartyRegistryEndBlockEventArguments e)
    {
      var performRole = Roles.GetAll(x => x.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.TenderCoordinatorGuid).SingleOrDefault();
      foreach (var atherDoc in _obj.AtherDocuments.OfficialDocuments)
      {
        if (Sungero.Docflow.SimpleDocuments.Is(atherDoc))
        {
          atherDoc.AccessRights.Revoke(performRole, DefaultAccessRightsTypes.FullAccess);
          atherDoc.AccessRights.Save();
        }
      }
    }

    public virtual void CompleteAssignment9(avis.ApprovingCounterpartyDEB.IIncludeCounterpartyRegistry assignment, avis.ApprovingCounterpartyDEB.Server.IncludeCounterpartyRegistryArguments e)
    {
      //Переадресовать задание
      if (assignment.Result == ApprovingCounterpartyDEB.IncludeCounterpartyRegistry.Result.Forward)
        assignment.Forward(assignment.Forward);
      
      if (assignment.Result == ApprovingCounterpartyDEB.IncludeCounterpartyRegistry.Result.Include)
      {
        //Прекратить парарллельные задания
        var parallelAssignments = ApprovingCounterpartyDEB.PublicFunctions.Module.GetParallelAssignments(assignment);
        var activeParallelAssignments = parallelAssignments
          .Where(a => a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
          .Where(a => !Equals(a, assignment))
          .Where(a => IncludeCounterpartyRegistries.Is(a));
        foreach (var parallelAssignment in activeParallelAssignments)
        {
          if (!string.IsNullOrEmpty(parallelAssignment.ActiveText))
            parallelAssignment.ActiveText += Environment.NewLine;
          
          if (assignment.CompletedBy != null && !assignment.CompletedBy.Equals(assignment.Performer))
            parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserWithOnBehalfOfFormat(assignment.CompletedBy.Name,
                                                                                                                            assignment.Performer.Name);
          else
            parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserFormat(assignment.Performer.Name);

          parallelAssignment.Abort();
        }
      }
    }

    public virtual void StartBlock9(avis.ApprovingCounterpartyDEB.Server.IncludeCounterpartyRegistryArguments e)
    {
      e.Block.Subject = $"Добавление в реестр поставщиков и подрядчиков контрагента {_obj.Counterparty}";
      var performRole = Roles.GetAll(x => x.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.TenderCoordinatorGuid).SingleOrDefault();
      e.Block.Performers.Add(performRole);
      foreach (var atherDoc in _obj.AtherDocuments.OfficialDocuments)
      {
        if (Sungero.Docflow.SimpleDocuments.Is(atherDoc))
        {
          atherDoc.AccessRights.Grant(performRole, DefaultAccessRightsTypes.FullAccess);
          atherDoc.AccessRights.Save();
        }
      }
    }

    #endregion
    
    #region Сумма разовой сделки в год более тендерной суммы?
    
    public virtual bool Decision8Result()
    {
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
      var approvalCounterpartyBankDEB = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEBs.As(document);
      return approvalCounterpartyBankDEB != null && approvalCounterpartyBankDEB.IsAmountBigestYearAmount == avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEB.IsAmountBigestYearAmount.Yes;
    }
    
    #endregion
    
    #region Тип контрагента заполнен?

    public virtual bool Decision7Result()
    {
      var databook = _obj.ApprovalDatabook.Counterparties.SingleOrDefault();
      var company = lenspec.Etalon.Companies.As(databook);
      return company != null && (company.IsProvideravis == true || company.IsContractoravis == true);
    }
    
    #endregion

    #region Проверка Персоны или Банка?
    
    public virtual bool Decision6Result()
    {
      var schemeVersion = _obj.GetStartedSchemeVersion();
      var databook = _obj.ApprovalDatabook.Counterparties.SingleOrDefault();
      return schemeVersion == LayerSchemeVersions.V4 || (lenspec.Etalon.Banks.Is(databook) || lenspec.Etalon.People.Is(databook));
    }
    
    #endregion

    #region Согласование руководством ДЭБ
    
    public virtual void CompleteAssignment5(avis.ApprovingCounterpartyDEB.IApprovalManagerDEBAssignment assignment, avis.ApprovingCounterpartyDEB.Server.ApprovalManagerDEBAssignmentArguments e)
    {
      //Переадресовать задание
      if (assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.Forward)
        assignment.Forward(assignment.Forward);
      
      //Прекратить парарллельные задания
      if (assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopPossible ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopWithRisks ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopNotRecomend ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.DoesNotReqAppr ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.Rework)
      {
        ApprovingCounterpartyDEB.PublicFunctions.ApprovalCounterpartyPersonDEB.AbortParallelAssignments(assignment);
      }
      
      //Актуализация свойств задачи
      if (assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopPossible ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopWithRisks ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.DoesNotReqAppr ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopNotRecomend)
      {
        if (assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopPossible ||
            assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopWithRisks)
        {
          _obj.CompleteDate = assignment.CompletionDate;
          if (assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopPossible)
            _obj.ResultApprovalDEB = ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.CoopPossible;
          if (assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopWithRisks)
            _obj.ResultApprovalDEB = ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.CoopWithRisks;
        }
        if (assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopNotRecomend)
        {
          _obj.ResultApprovalDEB = ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.CoopNotRecomend;
          _obj.CounterpartyLimit = null;
          _obj.CompleteDate = null;
        }
        if (assignment.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.DoesNotReqAppr)
        {
          _obj.ResultApprovalDEB = ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.DoesNotReqAppr;
          _obj.CounterpartyLimit = null;
          _obj.CompleteDate = null;
        }
        
        _obj.ManagerDEB = lenspec.Etalon.Employees.As(assignment.CompletedBy);
        _obj.Save();
        
        //Результат выполнения + комментарий в текст переписки
        var result = ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignments.Info.Properties.Result.GetLocalizedValue(assignment.Result);
        var comment = result.Equals(assignment.ActiveText.Trim('.')) ? result : string.Format("{0}. {1}", result, assignment.ActiveText);
        assignment.ActiveText = comment;
      }
    }
    
    public virtual void StartAssignment5(avis.ApprovingCounterpartyDEB.IApprovalManagerDEBAssignment assignment, avis.ApprovingCounterpartyDEB.Server.ApprovalManagerDEBAssignmentArguments e)
    {
      var schemeVersion = _obj.GetStartedSchemeVersion();
      if (schemeVersion == LayerSchemeVersions.V1 || schemeVersion == LayerSchemeVersions.V2 || schemeVersion == LayerSchemeVersions.V3)
      {
        // Решили что Руководство ДЭБ не будет согласовывать, задание выполняется программно, чтобы отработала связанная логика
        var previousAssignment = ApprovalerDEBApprovalAssignments.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId &&
                                                                         x.Status == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Status.Completed &&
                                                                         x.Result != avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.Forward &&
                                                                         x.Result != avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.Rework).SingleOrDefault();
        if (previousAssignment != null)
        {
          var result = avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopPossible;
          if (previousAssignment.Result == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopWithRisks)
            result = avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopWithRisks;
          if (previousAssignment.Result == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopNotRecomend)
            result = avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.CoopNotRecomend;
          if (previousAssignment.Result == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.DoesNotReqAppr)
            result = avis.ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.DoesNotReqAppr;
          
          assignment.Complete(result);
        }
      }
    }

    public virtual void StartBlock5(avis.ApprovingCounterpartyDEB.Server.ApprovalManagerDEBAssignmentArguments e)
    {
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
      document.ApprovalStateNew = ApprovingCounterpartyDEB.ApprovalCounterpartyBase.ApprovalStateNew.OnApproval;
      document.Save();
      
      e.Block.Subject = $"Необходимо проверить контрагента {_obj.Counterparty}.";
      e.Block.CompletionDate = _obj.CompleteDate;
      e.Block.CounterpartyLimit = _obj.CounterpartyLimit;
      
      var schemeVersion = _obj.GetStartedSchemeVersion();
      if (schemeVersion == LayerSchemeVersions.V1 || schemeVersion == LayerSchemeVersions.V2 || schemeVersion == LayerSchemeVersions.V3)
      {
        // Решили что Руководство ДЭБ не будет согласовывать, задание выполняется программно, чтобы отработала связанная логика
        var previousAssignment = ApprovalerDEBApprovalAssignments.GetAll(x => _obj.Equals(x.Task) && x.TaskStartId == _obj.StartId &&
                                                                         x.Status == avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Status.Completed &&
                                                                         x.Result != avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.Forward &&
                                                                         x.Result != avis.ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.Rework).SingleOrDefault();
        e.Block.Performers.Add(previousAssignment.CompletedBy);
      }
      
      if (schemeVersion == LayerSchemeVersions.V4)
      {
        //После доработки исполнителем будет тот кто отправил на доработку, если доработки не было, то исполнитель вычисляется из роли
        var performer = Sungero.CoreEntities.Recipients.Null;
        var assignment = ApprovalManagerDEBAssignments.GetAll(x => Equals(x.Task, _obj) &&
                                                              Equals(x.BlockUid, e.Block.Id) &&
                                                              x.TaskStartId == _obj.StartId &&
                                                              x.Status == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Status.Completed &&
                                                              x.Result == ApprovingCounterpartyDEB.ApprovalManagerDEBAssignment.Result.Rework).OrderBy(x => x.Created).FirstOrDefault();

        if (assignment != null)
          performer = assignment.Performer;
        else
          performer = Roles.GetAll(x => x.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.DEBManagementGuid).SingleOrDefault();
        e.Block.Performers.Add(performer);
        
        //Выдать полные права на простые документы во вложениях Других документов
        foreach (var atherDoc in _obj.AtherDocuments.OfficialDocuments)
        {
          if (Sungero.Docflow.SimpleDocuments.Is(atherDoc))
          {
            atherDoc.AccessRights.Grant(performer, DefaultAccessRightsTypes.FullAccess);
            atherDoc.AccessRights.Save();
          }
        }
      }
    }
    
    #endregion

    #region Проверка контрагента согласующим ДБ
    
    public virtual void CompleteAssignment4(avis.ApprovingCounterpartyDEB.IApprovalerDEBApprovalAssignment assignment, avis.ApprovingCounterpartyDEB.Server.ApprovalerDEBApprovalAssignmentArguments e)
    {
      //Прекратить парарллельные задания
      if (assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopPossible ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopWithRisks ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopNotRecomend ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.DoesNotReqAppr ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.Rework ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.Forward)
      {
        ApprovingCounterpartyDEB.PublicFunctions.ApprovalCounterpartyPersonDEB.AbortParallelAssignments(assignment);
      }
      
      if (assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.Forward)
      {
        assignment.Forward(assignment.Forward);
        return;
      }
      
      //Актуализация свойств задачи
      if (assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopPossible ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopWithRisks ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopNotRecomend ||
          assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.DoesNotReqAppr)
      {
        _obj.ApprovalerDEB = lenspec.Etalon.Employees.As(assignment.CompletedBy);
        _obj.Date = Calendar.Now;
        _obj.Note = assignment.ActiveText;
        if (assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopPossible ||
            assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopWithRisks)
        {
          _obj.CompleteDate = assignment.CompletionDate;
          if (assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopPossible)
            _obj.ResultApprovalDEB = ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.CoopPossible;
          else
            _obj.ResultApprovalDEB = ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.CoopWithRisks;
        }
        
        if (assignment.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.CoopNotRecomend)
        {
          _obj.ResultApprovalDEB = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.CoopNotRecomend;
          _obj.CounterpartyLimit = null;
          _obj.CompleteDate = null;
          _obj.ManagerDEB = null;
        }
        
        _obj.Save();
        
        //Результат выполнения + коммент в области переписки.
        var result = ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignments.Info.Properties.Result.GetLocalizedValue(assignment.Result);
        var comment = result.Equals(assignment.ActiveText.Trim('.')) ? result : string.Format("{0}. {1}", result, assignment.ActiveText);
        assignment.ActiveText = comment;
      }
    }

    public virtual void EndBlock4(avis.ApprovingCounterpartyDEB.Server.ApprovalerDEBApprovalAssignmentEndBlockEventArguments e)
    {
      //Забрать полный доступ на простые документы во вложениях
      foreach (var atherDoc in _obj.AtherDocuments.OfficialDocuments)
      {
        if (Sungero.Docflow.SimpleDocuments.Is(atherDoc))
        {
          atherDoc.AccessRights.Revoke(Roles.AllUsers, DefaultAccessRightsTypes.FullAccess);
          atherDoc.AccessRights.Save();
        }
      }
    }

    public virtual void StartBlock4(avis.ApprovingCounterpartyDEB.Server.ApprovalerDEBApprovalAssignmentArguments e)
    {
      e.Block.Subject = $"Необходимо проверить контрагента {_obj.Counterparty}.";
      
      //После доработки исполнителем будет тот кто отправил на доработку, если доработки не было, то исполнитель вычисляется из роли
      var performer = Sungero.CoreEntities.Recipients.Null;
      var assignment = ApprovalerDEBApprovalAssignments.GetAll(x => Equals(x.Task, _obj) &&
                                                               Equals(x.BlockUid, e.Block.Id) &&
                                                               x.TaskStartId == _obj.StartId &&
                                                               x.Status == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Status.Completed &&
                                                               x.Result == ApprovingCounterpartyDEB.ApprovalerDEBApprovalAssignment.Result.Rework).OrderBy(x => x.Created).FirstOrDefault();
      
      if (assignment != null)
        performer = assignment.Performer;
      else
        performer = Roles.GetAll(x => x.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.DEBCoordinationgGuid).SingleOrDefault();
      e.Block.Performers.Add(performer);
      
      //выдача полного доступа на простые документы
      foreach (var atherDoc in _obj.AtherDocuments.OfficialDocuments)
      {
        if (Sungero.Docflow.SimpleDocuments.Is(atherDoc))
        {
          atherDoc.AccessRights.Grant(performer, DefaultAccessRightsTypes.FullAccess);
          atherDoc.AccessRights.Save();
        }
      }
    }

    #endregion
    
    #region Согласование с руководителем инициатора
    
    public virtual void CompleteAssignment3(avis.ApprovingCounterpartyDEB.IManagerApprovalAssignment assignment, avis.ApprovingCounterpartyDEB.Server.ManagerApprovalAssignmentArguments e)
    {
      var schemeVersion = _obj.GetStartedSchemeVersion();
      if (schemeVersion == LayerSchemeVersions.V4)
      {
        if (assignment.Result == avis.ApprovingCounterpartyDEB.ManagerApprovalAssignment.Result.Complete)
        {
          _obj.ResultApprovalDEB = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB.ResultApprovalDEB.DoesNotReqAppr;
          _obj.ManagerDEB = lenspec.Etalon.Employees.As(assignment.CompletedBy);
        }
      }
    }
    
    public virtual void StartBlock3(avis.ApprovingCounterpartyDEB.Server.ManagerApprovalAssignmentArguments e)
    {
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
      document.ApprovalStateNew = ApprovingCounterpartyDEB.ApprovalCounterpartyBase.ApprovalStateNew.OnApproval;
      document.Save();
      
      var schemeVersion = _obj.GetStartedSchemeVersion();
      // Пропуск этапа после доработки инициатором в версиях схемы ниже 4
      if (schemeVersion == LayerSchemeVersions.V1 || schemeVersion == LayerSchemeVersions.V2 || schemeVersion == LayerSchemeVersions.V3)
      {
        var assignments = ManagerApprovalAssignments.GetAll().Where(x => Equals(x.Task, _obj) &&
                                                                    Equals(x.BlockUid, e.Block.Id) &&
                                                                    x.TaskStartId == _obj.StartId &&
                                                                    x.Status == ApprovingCounterpartyDEB.ManagerApprovalAssignment.Status.Completed &&
                                                                    x.Result == ApprovingCounterpartyDEB.ManagerApprovalAssignment.Result.Complete);
        if (assignments.Any())
          return;
      }
      
      e.Block.Subject = $"Контрагент: {_obj.Counterparty}. Согласование с ДБ.";
      e.Block.Performers.Add(_obj.ManagerInitiator);
    }

    #endregion
    
  }
}