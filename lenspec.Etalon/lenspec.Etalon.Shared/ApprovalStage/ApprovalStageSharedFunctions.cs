using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalStage;

namespace lenspec.Etalon.Shared
{
  partial class ApprovalStageFunctions
  {

    //Добавлено Avis Expert
    public override List<Enumeration?> GetPossibleRoles()
    {
      var baseRoles = base.GetPossibleRoles();
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers)
      {
        // Роль согласования "Согласующий по виду роли".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ApprovRoleKind);
        // Роль согласования "Руководитель из карточки задачи".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ManagerTaskCard);
        // Роль согласования "Члены коллегиального органа".
        baseRoles.Add(ProtocolsCollegialBodies.CompRoleCollegialBody.Type.Members);
        // Роль согласования "Члены тендерного комитета".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.TenderMembers);
        // Роль согласования "Руководитель подразделения поверенного"
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ManagAttornDept);
        // Роль согласования "Члены комитета аккредитации".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.AccreditationMembers);
        // Роль согласования "Получатель УКЭП".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.EDSOwner);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Manager)
      {
        // Роль согласования "Руководитель из карточки задачи".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ManagerTaskCard);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Sign)
      {
        // Роль согласования "Председатель коллегиального органа".
        baseRoles.Add(ProtocolsCollegialBodies.CompRoleCollegialBody.Type.Chairman);
        // Роль согласования "Председатель тендерного комитета".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.TenderChairman);
        // Роль согласования "Ответственный делопроизводитель (доверенности)"
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.RespClerkPOA);
        // Роль согласования "Председатель комитета аккредитации".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.AccreditationChairman);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Register)
      {
        // Роль согласования "Ответственный делопроизводитель".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ResponsibleClerk);
        // Роль согласования "Регистратор протокола тендерного комитета".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.TenderRegistrar);
        // Роль согласования "Регистратор протокола комитета аккредитации".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.AccreditationRegistrator);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.CheckReturn)
      {
        // Роль согласования "Согласующий по виду роли".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ApprovRoleKind);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Notice)
      {
        // Роль согласования "Согласующий по виду роли".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ApprovRoleKind);
        // Роль согласования "Руководитель из карточки задачи".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ManagerTaskCard);
        // Роль согласования Поверенный
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.Attorney);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr)
      {
        // Роль согласования "Согласующий по виду роли".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ApprovRoleKind);
        // Роль согласования "Руководитель из карточки задачи".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ManagerTaskCard);
        // Роль согласования "Регистратор протокола тендерного комитета".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.TenderRegistrar);
        // Роль согласования "Ответственный делопроизводитель (доверенности)"
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.RespClerkPOA);
        // Роль согласования "Регистратор протокола комитета аккредитации".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.AccreditationRegistrator);
        // Роль согласования "Получатель УКЭП".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.EDSOwner);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Sending)
      {
        // Роль согласования "Ответственный делопроизводитель".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.ResponsibleClerk);
        // Роль согласования "Отправитель рассылки".
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.MailingSender);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Print)
      {
        // Роль согласования "Ответственный делопроизводитель (доверенности)"
        baseRoles.Add(EtalonDatabooks.ComputedRole.Type.RespClerkPOA);
      }

      return baseRoles;
    }
    //конец Добавлено Avis Expert
  }
}