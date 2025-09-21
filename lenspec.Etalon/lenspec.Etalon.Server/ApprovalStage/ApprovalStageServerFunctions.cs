using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalStage;

namespace lenspec.Etalon.Server
{
  partial class ApprovalStageFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Получить исполнителей этапа без раскрытия групп и ролей.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="additionalApprovers">Доп.согласующие.</param>
    /// <returns>Исполнители.</returns>
    [Remote(IsPure = true), Public]
    public override List<IRecipient> GetStageRecipients(Sungero.Docflow.IApprovalTask task, List<IRecipient> additionalApprovers)
    {
      var recipients = base.GetStageRecipients(task, additionalApprovers);
      
      // Обработать роль "Согласующие по виду роли" отдельно, так как она множественная.
      var role = _obj.ApprovalRoles
        .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ApprovRoleKind)
        .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole)).Where(x => x!= null)
        .SingleOrDefault();
      if (role!= null)
      {
        recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.Remote.GetApprovRoleKindPerformers(role, task, _obj));
      }
      
      //Обработать роль Члены коллегиального органа
      var roles = _obj.ApprovalRoles
        .Where(x => x.ApprovalRole.Type == ProtocolsCollegialBodies.CompRoleCollegialBody.Type.Members)
        .Select(x => ProtocolsCollegialBodies.CompRoleCollegialBodies.As(x.ApprovalRole)).Where(x => x!= null)
        .SingleOrDefault();
      if (roles!= null)
      {
        recipients.AddRange(ProtocolsCollegialBodies.PublicFunctions.CompRoleCollegialBody.Remote.GetProtocolsCollegialBodyMembers(roles, task));
      }
      
      //Обработать роль Члены тендерного комитета
      var tenderMembersRole = _obj.ApprovalRoles
        .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.TenderMembers)
        .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole)).Where(x => x!= null)
        .SingleOrDefault();
      if (tenderMembersRole != null)
      {
        recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.Remote.GetTenderMembersPerformers(tenderMembersRole, task));
      }
      
      //Обработать роль Члены комитета аккредитации
      var accreditationMembersRole = _obj.ApprovalRoles
        .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.AccreditationMembers)
        .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole)).Where(x => x!= null)
        .SingleOrDefault();
      if (accreditationMembersRole != null)
      {
        recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.Remote.GetAccreditationCommitteeMembersPerformers(accreditationMembersRole, task));
      }
      
      //Обработать роль Руководитель подразделения поверенного
      var managerAttorneyRole = _obj.ApprovalRoles
        .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ManagAttornDept)
        .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole)).Where(x => x != null)
        .SingleOrDefault();
      if (managerAttorneyRole != null)
      {
        recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.GetManagersAttorney(managerAttorneyRole, task));
      }
      
      //Обработать роль Получатель УКЭП
      var edsOwnerRole = _obj.ApprovalRoles
        .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.EDSOwner)
        .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole)).Where(x => x!= null)
        .SingleOrDefault();
      if (edsOwnerRole != null)
      {
        recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.Remote.GetEDSOwnerPerformers(edsOwnerRole, task));
      }
      
      //Обработать роль Поверенный
      var attorneyRole = _obj.ApprovalRoles
        .Where(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.Attorney)
        .Select(x => EtalonDatabooks.ComputedRoles.As(x.ApprovalRole)).Where(x => x != null)
        .SingleOrDefault();
      if (attorneyRole != null)
      {
        recipients.AddRange(EtalonDatabooks.PublicFunctions.ComputedRole.GetAttorneys(attorneyRole, task));
      }
      return recipients;
    }
    //конец Добавлено Avis Expert
  }
}