using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalStage;

namespace lenspec.Etalon
{
  partial class ApprovalStageClientHandlers
  {

    //Добавлено Avis Expert
    public override void ReworkPerformerValueInput(Sungero.Docflow.Client.ApprovalStageReworkPerformerValueInputEventArgs e)
    {
      if (e.NewValue != null && Sungero.Company.Employees.Is(e.NewValue) && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(Sungero.Company.Employees.As(e.NewValue)))
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);

      base.ReworkPerformerValueInput(e);
    }

    public override void AssigneeValueInput(Sungero.Docflow.Client.ApprovalStageAssigneeValueInputEventArgs e)
    {
      if (e.NewValue != null && Sungero.Company.Employees.Is(e.NewValue) && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(Sungero.Company.Employees.As(e.NewValue)))
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      
      base.AssigneeValueInput(e);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      // Инструкции этапов согласования.
      _obj.State.Properties.Instructionlenspec.IsVisible = _obj.State.Properties.Instructionlenspec.IsEnabled =
        _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers ||
        _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Execution ||
        _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Manager ||
        _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Review ||
        _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Sending ||
        _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Sign;
      
      // Вид роли для одного исполнителя.
      var containsApprovRoleKind = _obj.ApprovalRole != null && _obj.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ApprovRoleKind;
      _obj.State.Properties.RoleKindlenspec.IsVisible = _obj.State.Properties.RoleKindlenspec.IsRequired = containsApprovRoleKind;
      
      // Вид роли для нескольких исполнителей.
      containsApprovRoleKind = _obj.ApprovalRoles.Any(x => x.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ApprovRoleKind);
      _obj.State.Properties.RoleKindslenspec.IsVisible = _obj.State.Properties.RoleKindslenspec.IsRequired = containsApprovRoleKind;
      if (!containsApprovRoleKind)
        _obj.RoleKindslenspec.Clear();
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Sign)
      {
        _obj.State.Properties.AssigneeType.IsEnabled = _obj.State.Properties.AssigneeType.IsVisible;
        _obj.State.Properties.Assignee.IsEnabled = _obj.State.Properties.Assignee.IsVisible;
        _obj.State.Properties.ApprovalRole.IsEnabled = _obj.State.Properties.ApprovalRole.IsVisible;
      }
      
      // Чекбокс После первого выполнения с прерыванием.
      _obj.State.Properties.WithInterruptionlenspec.IsVisible =
        (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers || _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr)
        && _obj.Sequence == Sungero.Docflow.ApprovalStage.Sequence.Parallel;
      
      if (_obj.State.Properties.WithInterruptionlenspec.IsVisible == true && _obj.WithInterruptionlenspec == null)
        _obj.WithInterruptionlenspec = false;
      
      // Чекбокс Проверка резервирования основного документа перед выполнением.
      _obj.State.Properties.CheckReservationlenspec.IsVisible = _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      
      if (_obj.State.Properties.CheckReservationlenspec.IsVisible == true && _obj.CheckReservationlenspec == null)
        _obj.CheckReservationlenspec = false;

      // Чекбокс Автоматически обновлять поля в теле документа при выполнении.
      _obj.State.Properties.UpdateTemplateBeforeExecuteavis.IsVisible = _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers ||
        _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Register || _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      
      if (_obj.State.Properties.UpdateTemplateBeforeExecuteavis.IsVisible == true && _obj.UpdateTemplateBeforeExecuteavis == null)
        _obj.UpdateTemplateBeforeExecuteavis = false;
      
      // Чекбокс Автоматически обновлять поля Номер и Дата в теле документа при выполнении.
      _obj.State.Properties.UpdateTemplateNumberAndDateBeforeExecuteavis.IsVisible = _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers ||
        _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Register || _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      
      if (_obj.State.Properties.UpdateTemplateNumberAndDateBeforeExecuteavis.IsVisible == true && _obj.UpdateTemplateNumberAndDateBeforeExecuteavis == null)
        _obj.UpdateTemplateNumberAndDateBeforeExecuteavis = false;
      
      // Чекбокс Массовая отправка по электронной почте.
      _obj.State.Properties.BulkEmaillenspec.IsVisible = _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Sending;
      
      if (_obj.State.Properties.BulkEmaillenspec.IsVisible == true && _obj.BulkEmaillenspec == null)
        _obj.BulkEmaillenspec = false;

      // Чекбокс «Способ доставки» доступен для редактирования.
      _obj.State.Properties.DeliveryMethodIsEditableavis.IsVisible = _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      
      if (_obj.State.Properties.DeliveryMethodIsEditableavis.IsVisible == true && _obj.DeliveryMethodIsEditableavis == null)
        _obj.DeliveryMethodIsEditableavis = false;
      
      // Чекбокс Не подписывать документы из группы «Приложения»..
      _obj.State.Properties.DoNotSignApplicationslenspec.IsVisible = _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Sign;
      
      if (_obj.State.Properties.DoNotSignApplicationslenspec.IsVisible == true && _obj.DoNotSignApplicationslenspec == null)
        _obj.DoNotSignApplicationslenspec = false;
      
      // Чекбокс Запретить запрос на продление срока
      _obj.State.Properties.IsProhibitExtensionOfTimeavis.IsVisible = _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers || 
        _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Manager || 
        _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      if (_obj.State.Properties.IsProhibitExtensionOfTimeavis.IsVisible == true && _obj.IsProhibitExtensionOfTimeavis == null)
        _obj.IsProhibitExtensionOfTimeavis = false;
      
      // Чекбокс Запретить смену подписанта
      _obj.State.Properties.IsProhibitChangeSigneravis.IsVisible = _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Manager;
      if (_obj.State.Properties.IsProhibitChangeSigneravis.IsVisible == true && _obj.IsProhibitChangeSigneravis == null)
        _obj.IsProhibitChangeSigneravis = false;
      
      // Чекбокс Проверять наличие версии документа
      _obj.State.Properties.IsCheckVersionlenspec.IsVisible = _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      if (_obj.State.Properties.IsCheckVersionlenspec.IsVisible == true && _obj.IsCheckVersionlenspec == null)
        _obj.IsCheckVersionlenspec = false;
      
      // Чекбокс Проверять экспорт в 1С ЗНО
      _obj.State.Properties.IsCheckExport1CApplicationForPaymentlenspec.IsVisible = _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr;
      if (_obj.State.Properties.IsCheckExport1CApplicationForPaymentlenspec.IsVisible == true && _obj.IsCheckExport1CApplicationForPaymentlenspec == null)
        _obj.IsCheckExport1CApplicationForPaymentlenspec = false;
    }
    //конец Добавлено Avis Expert

  }
}