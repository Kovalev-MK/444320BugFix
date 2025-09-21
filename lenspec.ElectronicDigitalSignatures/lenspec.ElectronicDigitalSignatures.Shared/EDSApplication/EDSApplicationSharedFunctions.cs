using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.EDSApplication;

namespace lenspec.ElectronicDigitalSignatures.Shared
{
  partial class EDSApplicationFunctions
  {

    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var documentKind = _obj.DocumentKind;
      
      if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      
      if (documentKind == null || !documentKind.GenerateDocumentName.Value)
        return;
      
      var name = string.Empty;
      
      /* Имя в формате:
        <Вид документа> №<№ заявки> от <Дата> на сотрудника "<Сотрудник>"
       */
      using (TenantInfo.Culture.SwitchTo())
      {
        if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
        
        if (_obj.Created != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.Created.Value.ToString("d");
        
        if (_obj.PreparedBy != null)
        {
          name += lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.ForEmployeeFormat(_obj.PreparedBy.Name);
        }
      }
      
      if (string.IsNullOrWhiteSpace(name))
      {
        if (_obj.VerificationState == null)
          name = Sungero.Docflow.Resources.DocumentNameAutotext;
        else
          name = _obj.DocumentKind.ShortName;
      }
      else if (documentKind != null)
      {
        name = documentKind.ShortName + name;
      }
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      if (name.Length > _obj.Info.Properties.Name.Length)
        _obj.Name = name.Substring(0, _obj.Info.Properties.Name.Length);
      else
        _obj.Name = name;
    }
    
    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      _obj.State.Properties.Subject.IsRequired = false;
      _obj.State.Properties.Target.IsRequired = _obj.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Cancellation;
      _obj.State.Properties.Explanation.IsRequired = _obj.Target == lenspec.ElectronicDigitalSignatures.EDSApplication.Target.Other;
    }
    
    /// <summary>
    /// Изменить отображение панели регистрации.
    /// </summary>
    /// <param name="needShow">Признак отображения.</param>
    /// <param name="repeatRegister">Признак повторной регистрации\изменения реквизитов.</param>
    public override void ChangeRegistrationPaneVisibility(bool needShow, bool repeatRegister)
    {
      base.ChangeRegistrationPaneVisibility(needShow, repeatRegister);
      
      _obj.State.Properties.LifeCycleState.IsVisible = false;
      _obj.State.Properties.Created.IsVisible = needShow;
      _obj.State.Properties.Author.IsVisible = needShow;
      _obj.State.Properties.ApplicationState.IsVisible = needShow;
    }
    
    public override void RefreshDocumentForm()
    {
      base.RefreshDocumentForm();
      
      _obj.State.Properties.Target.IsVisible = _obj.ApplicationCategory == lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationCategory.Cancellation;
      _obj.State.Properties.Explanation.IsVisible = _obj.Target == lenspec.ElectronicDigitalSignatures.EDSApplication.Target.Other;
    }
    
    /// <summary>
    /// Обновить статус согласования документа.
    /// </summary>
    /// <param name="newState">Новый статус.</param>
    /// <param name="taskId">ИД задачи на согласование или null, если задача не указана.</param>
    [Public]
    public override void UpdateDocumentApprovalState(Enumeration? newState, long? taskId)
    {
      base.UpdateDocumentApprovalState(newState, taskId);
      
      if (newState == Sungero.Docflow.OfficialDocument.InternalApprovalState.Aborted)
        _obj.ApplicationState = lenspec.ElectronicDigitalSignatures.EDSApplication.ApplicationState.Cancelled;
    }

    /// <summary>
    /// Добавление сертификатов получателя УКЭП.
    /// </summary>
    /// <param name="group"></param>
    [Public]
    public void AddCertificatesToAttachmentGroup(Sungero.Workflow.Interfaces.IWorkflowEntityAttachmentGroup @group)
    {
      var certificates = Sungero.CoreEntities.Certificates.GetAll(x => x.Enabled == true && Equals(x.Owner, _obj.PreparedBy));
      foreach (var certificate in certificates)
      {
        if (!group.All.Contains(certificate))
          group.All.Add(certificate);
      }
    }
  }
}