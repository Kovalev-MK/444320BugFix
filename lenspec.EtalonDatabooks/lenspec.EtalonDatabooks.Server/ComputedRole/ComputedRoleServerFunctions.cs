using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ComputedRole;

namespace lenspec.EtalonDatabooks.Server
{
  partial class ComputedRoleFunctions
  {
    //Добавлено Avis Expert
    
    #region Роли с одним участником
    
    public override Sungero.Company.IEmployee GetRolePerformer(Sungero.Docflow.IApprovalTask task)
    {
      // Руководитель из карточки задачи, Непосредственный руководитель инициатора.
      if (_obj.Type == EtalonDatabooks.ComputedRole.Type.ManagerTaskCard ||
          _obj.Type == EtalonDatabooks.ComputedRole.Type.InitManager)
      {
        return GetManagerTaskCardRolePerformer(task);
      }
      
      // Ответственный делопроизводитель.
      if (_obj.Type == EtalonDatabooks.ComputedRole.Type.ResponsibleClerk)
      {
        return GetResponsibleClerkPerformer(task);
      }
      
      // Отправитель рассылки.
      if (_obj.Type == EtalonDatabooks.ComputedRole.Type.MailingSender)
      {
        return GetMailingSenderPerformer(task);
      }
      
      // Ответственный отдела кадров.
      if (_obj.Type == EtalonDatabooks.ComputedRole.Type.ResponsibleHR)
      {
        return GetResponsibleHR(task);
      }
      
      // Председатель тендерного комитета.
      if (_obj.Type == EtalonDatabooks.ComputedRole.Type.TenderChairman)
      {
        return GetTenderChairman(task);
      }
      
      // Регистратор протокола тендерного комитета.
      if (_obj.Type == EtalonDatabooks.ComputedRole.Type.TenderRegistrar)
      {
        return GetTenderRegistrar(task);
      }
      
      // Ответственный делопроизводитель (доверенности)
      if (_obj.Type == EtalonDatabooks.ComputedRole.Type.RespClerkPOA)
      {
        return GetRespClerkPOA(task);
      }
      
      // Председатель комитета аккредитации.
      if (_obj.Type == EtalonDatabooks.ComputedRole.Type.AccreditationChairman)
      {
        return GetAccreditationChairman(task);
      }
      
      // Регистратор протокола комитета аккредитации.
      if (_obj.Type == EtalonDatabooks.ComputedRole.Type.AccreditationRegistrator)
      {
        return GetAccreditationRegistrator(task);
      }
      
      return base.GetRolePerformer(task);
    }
    
    #region Методы вычисления конкретных ролей согласования с одним участником
    
    /// <summary>
    /// Получить сотрудника-участника вычисляемой роли согласования.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Сотрудник.</returns>
    [Public]
    public static Sungero.Company.IEmployee GetAccreditationChairman(Sungero.Docflow.IApprovalTask task)
    {
      var performer = Sungero.Company.Employees.Null;
      
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        var protocol = Tenders.AccreditationCommitteeProtocols.As(document);
        if (protocol != null && protocol.AccreditationCommittee != null && protocol.AccreditationCommittee.Chairman != null)
          performer = protocol.AccreditationCommittee.Chairman;
      }
      return performer;
    }
    
    /// <summary>
    /// Получить сотрудника-участника вычисляемой роли согласования.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Сотрудник.</returns>
    [Public]
    public Sungero.Company.IEmployee GetRespClerkPOA(Sungero.Docflow.IApprovalTask task)
    {
      var performer = Sungero.Company.Employees.Null;
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      
      if (document != null && _obj.Type == EtalonDatabooks.ComputedRole.Type.RespClerkPOA)
      {
        var poa = lenspec.Etalon.PowerOfAttorneys.As(document);
        if (poa != null && poa.OurBusinessUavis.Any())
        {
          var businessUnit = lenspec.Etalon.BusinessUnits.As(poa.OurBusinessUavis.FirstOrDefault().Company);
          performer = GetApprovalRoleKindPerformerByBusinessUnit(businessUnit, task.ApprovalRule, lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk, document);
        }
      }
      return performer;
    }
    
    public static Sungero.Company.IEmployee GetAccreditationRegistrator(Sungero.Docflow.IApprovalTask task)
    {
      var performer = Sungero.Company.Employees.Null;
      
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        var protocol = Tenders.AccreditationCommitteeProtocols.As(document);
        if (protocol != null && protocol.AccreditationCommittee != null && protocol.AccreditationCommittee.Registrator != null)
          performer = protocol.AccreditationCommittee.Registrator;
      }
      return performer;
    }
    
    /// <summary>
    /// Получить сотрудника-участника вычисляемой роли согласования.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Сотрудник.</returns>
    [Public]
    public static Sungero.Company.IEmployee GetTenderChairman(Sungero.Docflow.IApprovalTask task)
    {
      var performer = Sungero.Company.Employees.Null;
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        var protocol = Tenders.TenderCommitteeProtocols.As(document);
        if (protocol != null && protocol.TenderCommittee != null && protocol.TenderCommittee.Chairman != null)
          performer = protocol.TenderCommittee.Chairman;
      }
      return performer;
    }
    
    /// <summary>
    /// Получить сотрудника-участника вычисляемой роли согласования.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Сотрудник.</returns>
    [Public]
    public static Sungero.Company.IEmployee GetTenderRegistrar(Sungero.Docflow.IApprovalTask task)
    {
      var performer = Sungero.Company.Employees.Null;
      var protocol = Tenders.TenderCommitteeProtocols.As(task.DocumentGroup.OfficialDocuments.FirstOrDefault());
      if (protocol != null && protocol.TenderCommittee != null && protocol.TenderCommittee.ProtocolRegistrar != null)
        performer = protocol.TenderCommittee.ProtocolRegistrar;
      return performer;
    }
    
    /// <summary>
    /// Получить сотрудника-участника вычисляемой роли согласования.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Сотрудник.</returns>
    [Public]
    public static Sungero.Company.IEmployee GetManagerTaskCardRolePerformer(Sungero.Docflow.IApprovalTask task)
    {
      var taskEtalon = Etalon.ApprovalTasks.As(task);
      return taskEtalon.HeadOfTheInitiatorlenspec;
    }
    
    /// <summary>
    /// Получить сотрудника-участника вычисляемой роли согласования.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="stage">Этап согласования.</param>
    /// <returns>Сотрудник.</returns>
    [Remote(IsPure = true), Public]
    public Sungero.Company.IEmployee GetResponsibleClerkPerformer(Sungero.Docflow.IApprovalTask task)
    {
      var performer = Sungero.Company.Employees.Null;
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && _obj.Type == EtalonDatabooks.ComputedRole.Type.ResponsibleClerk)
      {
        performer = GetApprovalRoleKindPerformer(task.ApprovalRule, document, lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk);
      }
      return performer;
    }
    
    /// <summary>
    /// Получить сотрудника-участника вычисляемой роли согласования.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="stage">Этап согласования.</param>
    /// <returns>Сотрудник.</returns>
    [Remote(IsPure = true), Public]
    public Sungero.Company.IEmployee GetResponsibleHR(Sungero.Docflow.IApprovalTask task)
    {
      var performer = Sungero.Company.Employees.Null;
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && _obj.Type == EtalonDatabooks.ComputedRole.Type.ResponsibleHR)
      {
        performer = GetApprovalRoleKindPerformer(task.ApprovalRule, document, lenspec.EtalonDatabooks.ComputedRoles.Resources.RoleDescriptionResponsibleHR);
      }
      return performer;
    }
    
    /// <summary>
    /// Получить сотрудника-участника вычисляемой роли согласования "Отправитель рассылки".
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="stage">Этап согласования.</param>
    /// <returns>Сотрудник.</returns>
    [Remote(IsPure = true), Public]
    public Sungero.Company.IEmployee GetMailingSenderPerformer(Sungero.Docflow.IApprovalTask task)
    {
      var performer = Sungero.Company.Employees.Null;
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && _obj.Type == EtalonDatabooks.ComputedRole.Type.MailingSender)
      {
        performer = GetApprovalRoleKindPerformer(task.ApprovalRule, document, lenspec.EtalonDatabooks.Resources.RoleDescriptionMailingSender);
      }
      return performer;
    }
    
    #endregion
    
    #region Глобальные вычиления исполнителя по виду роли
    
    /// <summary>
    /// Получить согласующего по Виду роли. (НОР вычисляется из документа)
    /// </summary>
    /// <param name="approvalRule">Правило согласования</param>
    /// <param name="document">Документ</param>
    /// <param name="roleKindName">Наименование вида роли</param>
    /// <returns>Сотрудник</returns>
    /// <remarks>Может использоваться в кастомных задачах (approvalRule передавать как null)</remarks>
    [Public]
    public static Sungero.Company.IEmployee GetApprovalRoleKindPerformer(Sungero.Docflow.IApprovalRuleBase approvalRule, Sungero.Docflow.IOfficialDocument document, string roleKindName)
    {
      var businessUnit = approvalRule == null ? document.OurSignatory.Department.BusinessUnit : document.BusinessUnit;
      return Functions.ComputedRole.GetApprovalRoleKindPerformerByBusinessUnit(lenspec.Etalon.BusinessUnits.As(businessUnit), approvalRule, roleKindName, document);
    }
    
    /// <summary>
    /// Получить согласующего по Виду роли. (Произвольная НОР)
    /// </summary>
    /// <param name="approvalRule">Правило согласования</param>
    /// <param name="roleKindName">Наименование вида роли</param>
    /// <param name="businessUnit">Наша организация</param>
    /// <returns>Сотрудник</returns>
    [Public]
    public static Sungero.Company.IEmployee GetApprovalRoleKindPerformerByBusinessUnit(lenspec.Etalon.IBusinessUnit businessUnit, Sungero.Docflow.IApprovalRuleBase approvalRule, string roleKindName, Sungero.Docflow.IOfficialDocument document)
    {
      return ComputePerformerByRoleKind(businessUnit, approvalRule, roleKindName, document);
    }
    
    /// <summary>
    /// Вычислить исполнителя по виду роли
    /// </summary>
    /// <param name="businessUnit">НОР</param>
    /// <param name="approvalRule">Правило согласования</param>
    /// <param name="roleKindName">Наименование вида роли</param>
    /// <param name="document">Документ</param>
    /// <returns>Исполнитель</returns>
    private static Sungero.Company.IEmployee ComputePerformerByRoleKind(lenspec.Etalon.IBusinessUnit businessUnit, Sungero.Docflow.IApprovalRuleBase approvalRule, string roleKindName, Sungero.Docflow.IOfficialDocument document)
    {
      var roleKind = lenspec.EtalonDatabooks.RoleKinds.GetAll(x => x.Name.Equals(roleKindName)).FirstOrDefault();
      // Если в виде роли чекбокс Вычислять из ИСП/Объекта = Да
      if (lenspec.Etalon.ContractualDocuments.Is(document) && roleKind != null && roleKind.IsComputeFromObject == true)
        return ComputePerformerByRoleKindFromOurCF(roleKind, document);

      //Остальные случаи
      var approvalSetting = GetApprovalSetting(approvalRule, document, businessUnit);
      return ComputePerformerByRoleKindStandard(approvalSetting, businessUnit, roleKind);
    }
    
    /// <summary>
    /// Вычислить исполнителя для договорного документа из ИСП
    /// </summary>
    /// <param name="roleKind">Вид роли</param>
    /// <param name="document">Документ</param>
    /// <returns>Исполнитель</returns>
    private static Sungero.Company.IEmployee ComputePerformerByRoleKindFromOurCF(lenspec.EtalonDatabooks.IRoleKind roleKind, Sungero.Docflow.IOfficialDocument document)
    {
      var performer = Sungero.Company.Employees.Null;
      var contractualDocument = lenspec.Etalon.ContractualDocuments.As(document);
      if (contractualDocument.OurCFavis != null)
      {
        if (contractualDocument.OurCFavis.IsComputeApprovalers == true)
        {
          var roleKindEmployee = contractualDocument.Objectlenspec.Approvalers.FirstOrDefault(x => x.RoleKind.Equals(roleKind));
          performer = roleKindEmployee?.Employee;
        }
        else
        {
          var roleKindEmployee = contractualDocument.OurCFavis.CollectionCoordinators.FirstOrDefault(x => x.Role.Equals(roleKind));
          performer = roleKindEmployee?.Employee;
        }
      }
      return performer;
    }
    
    /// <summary>
    /// Вычислить исполнителя по виду роли из Настройки согласования или из НОР
    /// </summary>
    /// <param name="approvalSetting">Настройка согласования</param>
    /// <param name="businessUnit">НОР</param>
    /// <param name="roleKind">Вид роли</param>
    /// <returns>Исполнитель</returns>
    private static Sungero.Company.IEmployee ComputePerformerByRoleKindStandard(EtalonDatabooks.IApprovalSetting approvalSetting, lenspec.Etalon.IBusinessUnit businessUnit, lenspec.EtalonDatabooks.IRoleKind roleKind)
    {
      var performer = Sungero.Company.Employees.Null;
      // Выбрать исполнителей по Видам роли в карточке Настройки согласования.
      if (approvalSetting != null && approvalSetting.RoleKindEmployee.Any(x => x.RoleKind.Equals(roleKind)))
      {
        var roleKindEmployee = approvalSetting.RoleKindEmployee.FirstOrDefault(x => x.RoleKind.Equals(roleKind));
        performer = roleKindEmployee?.Employee;
      }
      // Если исполнитель не вычислился через Настройки согласования, то выбрать по Видам ролей в карточке Нашей организации.
      else
      {
        if (businessUnit != null)
        {
          var roleKindEmployeeBU = Etalon.BusinessUnits.GetAll(x => x.Equals(businessUnit)).Single().RoleKindEmployeelenspec;
          if (roleKindEmployeeBU.Any())
          {
            var performerBU = roleKindEmployeeBU.Where(x => Equals(roleKind, x.RoleKind)).FirstOrDefault();
            if (performerBU != null)
            {
              performer = performerBU.Employee;
            }
          }
        }
      }
      return performer;
    }
    
    #endregion
    
    #endregion
    
    #region Роли с несколькими участниками
    
    #region Методы вычисления конкретных ролей согласования с несколькими участниками

    /// <summary>
    /// Получить исполнителей роли согласования с несколькими участниками.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="stage">Этап согласования.</param>
    /// <returns>Список исполнителей роли.</returns>
    [Remote(IsPure = true), Public]
    public List<Sungero.CoreEntities.IRecipient> GetTenderMembersPerformers(Sungero.Docflow.IApprovalTask task)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && _obj.Type == EtalonDatabooks.ComputedRole.Type.TenderMembers)
      {
        var protocol = lenspec.Tenders.TenderCommitteeProtocols.As(document);
        if (protocol != null && protocol.TenderCommittee != null)
        {
          result.AddRange(protocol.TenderCommittee.Members.Select(x => x.Member).ToList());
        }
      }
      return result;
    }
    
    /// <summary>
    /// Получить исполнителей роли согласования с несколькими участниками.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="stage">Этап согласования.</param>
    /// <returns>Список исполнителей роли.</returns>
    [Remote(IsPure = true), Public]
    public List<Sungero.CoreEntities.IRecipient> GetAccreditationCommitteeMembersPerformers(Sungero.Docflow.IApprovalTask task)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && _obj.Type == EtalonDatabooks.ComputedRole.Type.AccreditationMembers)
      {
        var protocol = lenspec.Tenders.AccreditationCommitteeProtocols.As(document);
        if (protocol != null && protocol.AccreditationCommittee != null)
        {
          result.AddRange(protocol.AccreditationCommittee.Members.Select(x => x.Member).ToList());
        }
      }
      
      return result;
    }
    
    /// <summary>
    /// Получить руководителей подразделений поверенных
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Сотрудник.</returns>
    [Public]
    public List<Sungero.Company.IEmployee> GetManagersAttorney(Sungero.Docflow.IApprovalTask task)
    {
      var result = new List<Sungero.Company.IEmployee>();
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && _obj.Type == EtalonDatabooks.ComputedRole.Type.ManagAttornDept)
      {
        var powerOfAttoneyBase = lenspec.Etalon.PowerOfAttorneyBases.As(document);
        if (powerOfAttoneyBase != null)
        {
          if (powerOfAttoneyBase.IsManyRepresentatives.HasValue && powerOfAttoneyBase.IsManyRepresentatives.Value && powerOfAttoneyBase.Representatives.Any())
          {
            var castedRepresentatives = powerOfAttoneyBase.Representatives as Sungero.Domain.Shared.IChildEntityCollection<lenspec.Etalon.IPowerOfAttorneyBaseRepresentatives>;
            var employees = castedRepresentatives.Where(x => x.AgentType == Sungero.Docflow.PowerOfAttorneyBaseRepresentatives.AgentType.Person && x.Employeelenspec != null).Select(x => x.Employeelenspec);
            foreach (var employee in employees)
            {
              var performer = avis.PowerOfAttorneyModule.PublicFunctions.Module.FoundManagerNotGD(employee, employee.Department);
              if (performer != null && !result.Contains(performer))
                result.Add(performer);
            }
          }
          else if (powerOfAttoneyBase.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.Employee && powerOfAttoneyBase.IssuedTo != null)
          {
            var performer = avis.PowerOfAttorneyModule.PublicFunctions.Module.FoundManagerNotGD(powerOfAttoneyBase.IssuedTo, powerOfAttoneyBase.IssuedTo.Department);
            result.Add(performer);
          }
        }
      }
      return result;
    }
    
    /// Получить исполнителей роли согласования с несколькими участниками.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="stage">Этап согласования.</param>
    /// <returns>Список исполнителей роли.</returns>
    [Remote(IsPure = true), Public]
    public List<Sungero.CoreEntities.IRecipient> GetEDSOwnerPerformers(Sungero.Docflow.IApprovalTask task)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && _obj.Type == EtalonDatabooks.ComputedRole.Type.EDSOwner)
      {
        var edsApplication = lenspec.ElectronicDigitalSignatures.EDSApplications.As(document);
        if (edsApplication != null && edsApplication.PreparedBy != null)
        {
          result.Add(edsApplication.PreparedBy);
        }
      }
      
      return result;
    }
    
    /// Получить исполнителей роли согласования (поверенных) с несколькими участниками.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="stage">Этап согласования.</param>
    /// <returns>Список исполнителей роли.</returns>
    [Public]
    public List<Sungero.Company.IEmployee> GetAttorneys(Sungero.Docflow.IApprovalTask task)
    {
      var result = new List<Sungero.Company.IEmployee>();
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && _obj.Type == EtalonDatabooks.ComputedRole.Type.Attorney)
      {
        var powerOfAttoneyBase = lenspec.Etalon.PowerOfAttorneyBases.As(document);
        if (powerOfAttoneyBase != null)
        {
          if (powerOfAttoneyBase.IsManyRepresentatives.HasValue && powerOfAttoneyBase.IsManyRepresentatives.Value && powerOfAttoneyBase.Representatives.Any())
          {
            var castedRepresentatives = powerOfAttoneyBase.Representatives as Sungero.Domain.Shared.IChildEntityCollection<lenspec.Etalon.IPowerOfAttorneyBaseRepresentatives>;
            var employees = castedRepresentatives.Where(x => x.AgentType == Sungero.Docflow.PowerOfAttorneyBaseRepresentatives.AgentType.Person && x.Employeelenspec != null).Select(x => x.Employeelenspec);
            result.AddRange(employees);
          }
          else if (powerOfAttoneyBase.AgentType == Sungero.Docflow.PowerOfAttorneyBase.AgentType.Employee && powerOfAttoneyBase.IssuedTo != null)
          {
            result.Add(powerOfAttoneyBase.IssuedTo);
          }
        }
      }
      return result;
    }
    
    #endregion
    
    #region Глобальные вычиления исполнителей по виду роли
    
    /// <summary>
    /// Получить исполнителей роли согласования с несколькими участниками.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="stage">Этап согласования.</param>
    /// <returns>Список исполнителей роли.</returns>
    [Remote(IsPure = true), Public]
    public List<Sungero.CoreEntities.IRecipient> GetApprovRoleKindPerformers(Sungero.Docflow.IApprovalTask task, Etalon.IApprovalStage stage)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document == null || _obj.Type != EtalonDatabooks.ComputedRole.Type.ApprovRoleKind)
        return result;
      
      // Не фильтруем по действующим?
      var roleKinds = stage.RoleKindslenspec.Select(x => x.RoleKind).ToList();
      result.AddRange(GetPerformersByRoleKinds(document, task.ApprovalRule, document.BusinessUnit, roleKinds));
      
      return result;
    }
    
    /// <summary>
    /// Получить исполнителей роли согласования с несколькими участниками.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="stage">Этап согласования.</param>
    /// <param name="stage">Имя вида роли.</param>
    /// <returns>Список исполнителей роли.</returns>
    [Public]
    public List<Sungero.CoreEntities.IRecipient> GetApprovRoleKindPerformersByName(Sungero.Docflow.IApprovalTask task, Sungero.Docflow.IApprovalStage stage, string roleKindName)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document == null)
        return result;
      
      var roleKinds = new List<lenspec.EtalonDatabooks.IRoleKind>();
      if (stage != null)
        roleKinds.AddRange(lenspec.Etalon.ApprovalStages.As(stage).RoleKindslenspec.Where(x => x.RoleKind.Name == roleKindName).Select(x => x.RoleKind).ToList());
      else
      {
        var roleKind = lenspec.EtalonDatabooks.RoleKinds.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && x.Name == roleKindName).FirstOrDefault();
        if (roleKind != null)
          roleKinds.Add(roleKind);
      }
      
      result.AddRange(GetPerformersByRoleKinds(document, task.ApprovalRule, document.BusinessUnit, roleKinds));
      
      return result;
    }
    
    /// <summary>
    /// Вычислить исполнителей по Видам ролей.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="roleKinds">Виды ролей из этапа.</param>
    /// <returns>Список исполнителей.</returns>
    private List<Sungero.CoreEntities.IRecipient> GetPerformersByRoleKinds(Sungero.Docflow.IOfficialDocument document, Sungero.Docflow.IApprovalRuleBase approvalRule,
                                                                           Sungero.Company.IBusinessUnit businessUnit, List<EtalonDatabooks.IRoleKind> roleKinds)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      
      if (Sungero.Contracts.ContractsApprovalRules.Is(approvalRule))
      {
        // Договорные документы.
        if (lenspec.Etalon.ContractualDocuments.Is(document))
        {
          result.AddRange(GetPerformersFromContractualDocument(document, roleKinds, approvalRule, businessUnit));
          if (result.Any())
            return result;
        }
        
        // Заявки на оплату.
        if (lenspec.ApplicationsForPayment.ApplicationForPayments.Is(document))
          result.AddRange(GetPerformersByApplicationForPayment(document, roleKinds));
      }
      
      // Общие вычисления по видам ролей
      // Для ЗНО исключить из общих вычислений Уполномоченный на утверждение ЗНО без договора до 300 тыс., Ответственный экономист, УИСП
      if (lenspec.ApplicationsForPayment.ApplicationForPayments.Is(document))
      {
        roleKinds = roleKinds.Where(x => x.Name != lenspec.EtalonDatabooks.ComputedRoles.Resources.AuthorizedToApproveWithoutContractRoleKindName &&
                                    x.Name != lenspec.EtalonDatabooks.ComputedRoles.Resources.ResponsibleEconomistRoleKindName &&
                                    x.Name != lenspec.EtalonDatabooks.ComputedRoles.Resources.UISPRoleKindName)
          .ToList();
      }
      
      var approvalSetting = GetApprovalSetting(approvalRule, document, businessUnit);
      if (approvalSetting != null)
        result.AddRange(GetPerformersFromApprovalSetting(businessUnit, approvalSetting, roleKinds));
      else
        result.AddRange(GetPerformersFromBusinessUnit(businessUnit, roleKinds));
      
      return result;
    }
    
    /// <summary>
    /// Вычислить исполнителей для Заявки на оплату.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="roleKinds">Виды ролей из этапа.</param>
    /// <returns>Список исполнителей.</returns>
    private List<Sungero.CoreEntities.IRecipient> GetPerformersByApplicationForPayment(Sungero.Docflow.IOfficialDocument document, List<EtalonDatabooks.IRoleKind> roleKinds)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      var applicationForPayment = lenspec.ApplicationsForPayment.ApplicationForPayments.As(document);
      if (applicationForPayment == null)
        return result;
      
      var roleKindsByName = new List<EtalonDatabooks.IRoleKind>();
      
      // Уполномоченный на утверждение ЗНО без договора до 300 тыс.
      var approveApplicationRoleKind = roleKinds.Where(x => x.Name == lenspec.EtalonDatabooks.ComputedRoles.Resources.AuthorizedToApproveWithoutContractRoleKindName).FirstOrDefault();
      if (approveApplicationRoleKind != null && applicationForPayment.BusinessUnit != null)
      {
        roleKindsByName.Clear();
        roleKindsByName.Add(approveApplicationRoleKind);
        
        var performersFromBusinessUnit = GetPerformersFromBusinessUnit(applicationForPayment.BusinessUnit, roleKindsByName);
        if (performersFromBusinessUnit.Any())
          result.AddRange(performersFromBusinessUnit);
        else if (applicationForPayment.BusinessUnit.CEO != null)
          result.Add(applicationForPayment.BusinessUnit.CEO);
      }
      
      // Ответственный экономист
      var responsibleEconomistRoleKind = roleKinds.Where(x => x.Name == lenspec.EtalonDatabooks.ComputedRoles.Resources.ResponsibleEconomistRoleKindName).FirstOrDefault();
      if (responsibleEconomistRoleKind != null)
      {
        roleKindsByName.Clear();
        roleKindsByName.Add(responsibleEconomistRoleKind);
        
        var performersFromObjectAnProjects = new List<Sungero.CoreEntities.IRecipient>();
        // Объекты проекта, у которых в табличной части Согласующие есть подходящий Вид роли.
        var objectAnProjects = applicationForPayment.ObjectAnProjects
          .Where(x => x.ObjectAnProject.Approvalers.Any(a => roleKindsByName.Contains(a.RoleKind)))
          .Select(x => x.ObjectAnProject);
        foreach (var objectAnProject in objectAnProjects)
        {
          performersFromObjectAnProjects.AddRange(GetPerformersFromObjectAnProject(objectAnProject, roleKindsByName));
        }
        if (performersFromObjectAnProjects.Any())
          result.AddRange(performersFromObjectAnProjects);
        else
        {
          // ИСП, у которых в табличной части Согласующие есть подходящий Вид роли.
          var ourCFs = applicationForPayment.ObjectAnProjects
            .Where(x => x.ObjectAnProject.OurCF != null && x.ObjectAnProject.OurCF.CollectionCoordinators.Any(a => roleKindsByName.Contains(a.Role)))
            .Select(x => x.ObjectAnProject.OurCF);
          foreach (var ourCF in ourCFs)
          {
            performersFromObjectAnProjects.AddRange(GetPerformersFromOurCF(ourCF, roleKindsByName));
          }
          if (performersFromObjectAnProjects.Any())
            result.AddRange(performersFromObjectAnProjects);
          else
          {
            result.AddRange(GetPerformersFromBusinessUnit(applicationForPayment.BusinessUnit, roleKindsByName));
          }
        }
      }
      
      // УИСП
      var uispRoleKind = roleKinds.Where(x => x.Name == lenspec.EtalonDatabooks.ComputedRoles.Resources.UISPRoleKindName).FirstOrDefault();
      if (uispRoleKind != null)
      {
        roleKindsByName.Clear();
        roleKindsByName.Add(uispRoleKind);
        
        var performersFromObjectAnProjects = new List<Sungero.CoreEntities.IRecipient>();
        // Объекты проекта, у которых в табличной части Согласующие есть подходящий Вид роли.
        var objectAnProjects = applicationForPayment.ObjectAnProjects
          .Where(x => x.ObjectAnProject.Approvalers.Any(a => roleKindsByName.Contains(a.RoleKind)))
          .Select(x => x.ObjectAnProject);
        foreach (var objectAnProject in objectAnProjects)
        {
          performersFromObjectAnProjects.AddRange(GetPerformersFromObjectAnProject(objectAnProject, roleKindsByName));
        }
        if (performersFromObjectAnProjects.Any())
          result.AddRange(performersFromObjectAnProjects);
        else
        {
          // ИСП, у которых в табличной части Согласующие есть подходящий Вид роли.
          var ourCFs = applicationForPayment.ObjectAnProjects
            .Where(x => x.ObjectAnProject.OurCF != null && x.ObjectAnProject.OurCF.CollectionCoordinators.Any(a => roleKindsByName.Contains(a.Role)))
            .Select(x => x.ObjectAnProject.OurCF);
          foreach (var ourCF in ourCFs)
          {
            performersFromObjectAnProjects.AddRange(GetPerformersFromOurCF(ourCF, roleKindsByName));
          }
          if (performersFromObjectAnProjects.Any())
            result.AddRange(performersFromObjectAnProjects);
          else
          {
            result.AddRange(GetPerformersFromBusinessUnit(applicationForPayment.BusinessUnit, roleKindsByName));
          }
        }
      }
      return result;
    }
    
    #endregion
    
    #endregion
    
    #region Общие методы
    
    /// <summary>
    /// Найти подходящую настройку согласования
    /// </summary>
    /// <param name="approvalRule">Правило согласования</param>
    /// <param name="document">Документ</param>
    /// <param name="businessUnit">НОР</param>
    /// <returns>Настройка согласования</returns>
    private static EtalonDatabooks.IApprovalSetting GetApprovalSetting(Sungero.Docflow.IApprovalRuleBase approvalRule, Sungero.Docflow.IOfficialDocument document, Sungero.Company.IBusinessUnit businessUnit)
    {
      var approvalSetting = EtalonDatabooks.ApprovalSettings.Null;
      if (approvalRule == null)
        return approvalSetting;
      
      var approvalRuleAllVersions = Etalon.PublicFunctions.ApprovalTask.Remote.GetAllRuleVersions(approvalRule);
      // Отобрать настройки согласования по регламенту в задаче.
      var settingByApprovalRule = EtalonDatabooks.ApprovalSettings.GetAll().Where(x => x.Status == EtalonDatabooks.ApprovalSetting.Status.Active
                                                                                  && approvalRuleAllVersions.Contains(x.ApprovalRule));
      
      // Своя фильтрация, если правило согласования для договорных документов
      if (Sungero.Contracts.ContractsApprovalRules.Is(approvalRule) && lenspec.Etalon.ContractualDocuments.Is(document))
      {
        var contractualDocument = lenspec.Etalon.ContractualDocuments.As(document);
        // Совпадение по всем признакам.
        approvalSetting = settingByApprovalRule.Where(x => Equals(x.BusinessUnit, contractualDocument.BusinessUnit)
                                                      && Equals(x.GroupKind, contractualDocument.GroupContractTypeavis)
                                                      && Equals(x.ContractKind, contractualDocument.ContractKindavis)
                                                      && x.ContractCategories.Any(c => Equals(c.ContractCategory, contractualDocument.ContractCategoryavis)))
          .FirstOrDefault();
        
        // "Наша организация" + "Группа категории" + "Категория договора" + "Вид договора = пустое значение"
        if (approvalSetting == null)
          approvalSetting = settingByApprovalRule.Where(x => Equals(x.BusinessUnit, contractualDocument.BusinessUnit)
                                                        && Equals(x.GroupKind, contractualDocument.GroupContractTypeavis)
                                                        && Equals(x.ContractKind, contractualDocument.ContractKindavis)
                                                        && !x.ContractCategories.Any())
            .FirstOrDefault();
        
        // "Наша организация" + "Группа категории" + "Категория договора  = пустое значение" + "Вид договора"
        if (approvalSetting == null)
          approvalSetting = settingByApprovalRule.Where(x => Equals(x.BusinessUnit, contractualDocument.BusinessUnit)
                                                        && Equals(x.GroupKind, contractualDocument.GroupContractTypeavis)
                                                        && x.ContractKind == null
                                                        && x.ContractCategories.Any(c => Equals(c.ContractCategory, contractualDocument.ContractCategoryavis)))
            .FirstOrDefault();
        
        // "Наша организация" + "Группа категории" + "Категория договора  = пустое значение" + "Вид договора  = пустое значение"
        if (approvalSetting == null)
          approvalSetting = settingByApprovalRule.Where(x => Equals(x.BusinessUnit, contractualDocument.BusinessUnit)
                                                        && Equals(x.GroupKind, contractualDocument.GroupContractTypeavis)
                                                        && x.ContractKind == null
                                                        && !x.ContractCategories.Any())
            .FirstOrDefault();
        
        if (approvalSetting != null)
          return approvalSetting;
        else
          settingByApprovalRule = settingByApprovalRule.Where(x => x.GroupKind == null && x.ContractKind == null && !x.ContractCategories.Any());
      }
      
      if (settingByApprovalRule.Any())
      {
        // Если есть хотя бы одна настройка согласования для Нашей организации, то выбрать первую,
        // иначе выбрать первую из настроек, где поле Наша организация пустое.
        approvalSetting = businessUnit != null && settingByApprovalRule.Any(x => x.BusinessUnit.Equals(businessUnit))
          ? settingByApprovalRule.Where(x => x.BusinessUnit.Equals(businessUnit)).FirstOrDefault()
          : settingByApprovalRule.Where(x => x.BusinessUnit == null).FirstOrDefault();
      }
      
      return approvalSetting;
    }
    
    /// <summary>
    /// Вычислить исполнителей для договорного документа из ролей.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="roleKinds">Виды ролей.</param>
    /// <returns>Список исполнителей.</returns>
    private List<Sungero.CoreEntities.IRecipient> GetPerformersFromContractualDocument(Sungero.Docflow.IOfficialDocument document, List<EtalonDatabooks.IRoleKind> roleKinds,
                                                                                       Sungero.Docflow.IApprovalRuleBase approvalRule, Sungero.Company.IBusinessUnit businessUnit)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      var contractualDocument = lenspec.Etalon.ContractualDocuments.As(document);
      var ourCF = contractualDocument.OurCFavis;
      
      if (ourCF == null)
        return result;
      
      var remainingRoleKinds = new List<EtalonDatabooks.IRoleKind>();
      foreach (var roleKind in roleKinds)
      {
        var performers = Enumerable.Empty<Sungero.Company.IEmployee>();
        
        if (ourCF.IsComputeApprovalers == true)
          performers = contractualDocument.Objectlenspec != null 
            ? contractualDocument.Objectlenspec.Approvalers.Where(x => Equals(roleKind, x.RoleKind)).Select(x => x.Employee) 
            : performers;
        else
          performers = ourCF.CollectionCoordinators.Where(x => Equals(roleKind, x.Role)).Select(x => x.Employee);
        
        if (performers.Any())
          result.AddRange(performers);
        else
          remainingRoleKinds.Add(roleKind);
      }
      
      if (remainingRoleKinds.Any())
      {
        var approvalSetting = GetApprovalSetting(approvalRule, document, businessUnit);
        if (approvalSetting != null)
          result.AddRange(GetPerformersFromApprovalSetting(businessUnit, approvalSetting, remainingRoleKinds));
        else
          result.AddRange(GetPerformersFromBusinessUnit(businessUnit, remainingRoleKinds));
      }
      
      return result;
    }
    
    /// <summary>
    /// Вычислить исполнителей по видам ролей из настройки согласования.
    /// </summary>
    /// <param name="businessUnit">НОР.</param>
    /// <param name="approvalSetting">Настройка согласования.</param>
    /// <param name="roleKinds">Виды ролей.</param>
    /// <returns>Список исполнителей.</returns>
    private List<Sungero.CoreEntities.IRecipient> GetPerformersFromApprovalSetting(Sungero.Company.IBusinessUnit businessUnit, lenspec.EtalonDatabooks.IApprovalSetting approvalSetting, List<EtalonDatabooks.IRoleKind> roleKinds)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      var remainingRoleKinds = new List<EtalonDatabooks.IRoleKind>();
      // По всем Видам ролей из текущего Этапа.
      foreach(var roleKind in roleKinds)
      {
        var employees = approvalSetting.RoleKindEmployee.Where(x => x.RoleKind.Equals(roleKind)).Select(x => x.Employee).ToList();
        if (employees.Any())
          result.AddRange(employees);
        else
          remainingRoleKinds.Add(roleKind);
      }
      // Выбрать оставшихся исполнителей по Видам ролей в карточке Нашей организации.
      var performersFromBusinessUnit = GetPerformersFromBusinessUnit(businessUnit, remainingRoleKinds);
      result.AddRange(performersFromBusinessUnit);
      return result;
    }
    
    /// <summary>
    /// Вычислить исполнителей по видам ролей из НОР.
    /// </summary>
    /// <param name="businessUnit">НОР.</param>
    /// <param name="roleKinds">Виды ролей.</param>
    /// <returns>Список исполнителей.</returns>
    private List<Sungero.CoreEntities.IRecipient> GetPerformersFromBusinessUnit(Sungero.Company.IBusinessUnit businessUnit, List<EtalonDatabooks.IRoleKind> roleKinds)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      var businessUnitEtalon = lenspec.Etalon.BusinessUnits.As(businessUnit);
      if (businessUnitEtalon == null)
        return result;
      
      foreach(var roleKind in roleKinds)
      {
        var employees = businessUnitEtalon.RoleKindEmployeelenspec.Where(x => x.RoleKind.Equals(roleKind)).Select(x => x.Employee).ToList();
        result.AddRange(employees);
      }
      return result;
    }
    
    /// <summary>
    /// Вычислить исполнителей по видам ролей из Объекта проекта, коллекция Согласующие.
    /// </summary>
    /// <param name="objectAnProject">Объект проекта.</param>
    /// <param name="roleKinds">Виды ролей.</param>
    /// <returns>Список исполнителей.</returns>
    private List<Sungero.CoreEntities.IRecipient> GetPerformersFromObjectAnProject(lenspec.EtalonDatabooks.IObjectAnProject objectAnProject, List<EtalonDatabooks.IRoleKind> roleKinds)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      if (objectAnProject == null)
        return result;
      
      foreach(var roleKind in roleKinds)
      {
        var employees = objectAnProject.Approvalers.Where(x => roleKind.Equals(x.RoleKind)).Select(x => x.Employee).ToList();
        result.AddRange(employees);
      }
      return result;
    }
    
    /// <summary>
    /// Вычислить исполнителей по видам ролей из ИСП, коллекция Согласующие.
    /// </summary>
    /// <param name="objectAnProject">ИСП.</param>
    /// <param name="roleKinds">Виды ролей.</param>
    /// <returns>Список исполнителей.</returns>
    private List<Sungero.CoreEntities.IRecipient> GetPerformersFromOurCF(lenspec.EtalonDatabooks.IOurCF ourCF, List<EtalonDatabooks.IRoleKind> roleKinds)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      if (ourCF == null)
        return result;
      
      foreach(var roleKind in roleKinds)
      {
        var employees = ourCF.CollectionCoordinators.Where(x => roleKind.Equals(x.Role)).Select(x => x.Employee).ToList();
        result.AddRange(employees);
      }
      return result;
    }
    
    #endregion

    //конец Добавлено Avis Expert
  }
}