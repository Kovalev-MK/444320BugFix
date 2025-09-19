using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalSimpleAssignment;

namespace lenspec.Etalon.Client
{
  partial class ApprovalSimpleAssignmentActions
  {
    public virtual void Export1CApplicationForPaymentlenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var applicationForPayment = ApplicationsForPayment.ApplicationForPayments.As(document);
      
      if (!Locks.TryLock(applicationForPayment))
      {
        ApplicationsForPayment.PublicFunctions.ApplicationForPayment.GetLockErrorMessage(applicationForPayment);
        return;
      }
      
      lenspec.ApplicationsForPayment.PublicFunctions.ApplicationForPayment.Export1C(applicationForPayment);
    }

    public virtual bool CanExport1CApplicationForPaymentlenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (!lenspec.ApplicationsForPayment.ApplicationForPayments.Is(document))
        return false;
      
      var applicationForPayment = lenspec.ApplicationsForPayment.ApplicationForPayments.As(document);
      return applicationForPayment.Export1CState != ApplicationsForPayment.ApplicationForPayment.Export1CState.Yes && !applicationForPayment.Export1CDate.HasValue;
    }

    public virtual void GetBarcodeslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var documents = _obj.AddendaGroup.OfficialDocuments.ToList();
      lenspec.Etalon.Functions.ApprovalTask.SaveBarcodesToFolderForPrining(documents);
    }

    public virtual bool CanGetBarcodeslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var document = _obj.DocumentGroup.OfficialDocuments.SingleOrDefault();
      var requestCreatePOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
      var requestCreateNPOAKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
      var poa = lenspec.Etalon.PowerOfAttorneys.As(document);
      var businessUnit = lenspec.Etalon.BusinessUnits.As(poa.OurBusinessUavis.FirstOrDefault().Company);
      var responsibleClerkPoa = lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovalRoleKindPerformerByBusinessUnit(businessUnit, task.ApprovalRule, lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk, document);
      
      return (responsibleClerkPoa != null && (Equals(Sungero.Company.Employees.Current, responsibleClerkPoa) || Substitutions.ActiveUsersWhoSubstitute(responsibleClerkPoa).Contains(Sungero.Company.Employees.Current))) &&
        (Equals(document.DocumentKind, requestCreatePOAKind) || Equals(document.DocumentKind, requestCreateNPOAKind)) && _obj.Status == Sungero.Workflow.Assignment.Status.InProcess;
    }

    public override void ExtendDeadline(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ExtendDeadline(e);
    }

    public override bool CanExtendDeadline(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      //TODO:
      return base.CanExtendDeadline(e) && Functions.ApprovalSimpleAssignment.Remote.CheckPossibilityExtending(_obj);
    }

    
    //Добавлено Avis Expert
    public virtual bool CanAssignDocumentNumberavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      return document.RegistrationState == null || document.RegistrationState != null && document.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.NotRegistered;
    }

    public virtual void AssignDocumentNumberavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        if (document.RegistrationState != null && document.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Reserved)
        {
          e.AddError(lenspec.Etalon.ApprovalSimpleAssignments.Resources.DocumentIsReserved);
        }
        
        var isNumerable = document.DocumentKind.NumberingType == Sungero.Docflow.DocumentKind.NumberingType.Numerable;
        if (isNumerable)
        {
          var errorAssingNumber = Etalon.Client.OfficialDocumentFunctions.AssignNumberAvis(document);
          if (!string.IsNullOrEmpty(errorAssingNumber))
          {
            e.AddError(errorAssingNumber);
          }
        }
        else
        {
          var errorReserveNumber = Etalon.Client.OfficialDocumentFunctions.ReserveNumberAvis(document);
          if (!string.IsNullOrEmpty(errorReserveNumber))
          {
            e.AddError(errorReserveNumber);
          }
        }
      }
    }

    public override void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (FormalizedPowerOfAttorneys.Is(document))
      {
        if (!document.HasVersions)
        {
          e.AddError(lenspec.Etalon.ApprovalSimpleAssignments.Resources.NeedCreateFPOAVersion);
          return;
        }
      }
      if (document != null)
      {
        #region Проверка на заполненность полей для Курьерских отправлений
        if (CourierShipments.CourierShipmentsApplications.Is(document))
        {
          var courierShipmentsApplication = CourierShipments.CourierShipmentsApplications.As(document);
          if (string.IsNullOrEmpty(courierShipmentsApplication.WaybillNumber) || courierShipmentsApplication.CourierService == null)
          {
            e.AddError(lenspec.Etalon.ApprovalSimpleAssignments.Resources.FillTheFieldsBeforeComplete);
            return;
          }
        }
        #endregion
        
        #region Резервирование документа
        
        var isReserved = Functions.ApprovalSimpleAssignment.Remote.CheckReservationDocument(_obj, document);
        if (!string.IsNullOrEmpty(isReserved))
        {
          e.AddError(isReserved, _obj.Info.Actions.AssignDocumentNumberavis);
          //e.ClearMessageAfterAction = true;
          return;
        }
        
        #endregion
        
        #region Обновление полей в теле документа
        
        var isBodyUpdated = Functions.ApprovalSimpleAssignment.Remote.UpdateDocumentBody(_obj);
        if (!string.IsNullOrEmpty(isBodyUpdated) && !isBodyUpdated.Equals(lenspec.Etalon.ApprovalTasks.Resources.FailedToUpdateFieldsInDocumentsPrefix))
        {
          e.AddError(isBodyUpdated);
          return;
        }
        
        #endregion
        
        #region Проверка статуса регистрации всех приложений при согласовании заявки на создание доверенности
        
        var addendums = _obj.AddendaGroup.OfficialDocuments;
        if (Sungero.Docflow.PowerOfAttorneys.Is(document))
        {
          var poa = lenspec.Etalon.PowerOfAttorneys.As(document);
          if (poa.OurBusinessUavis.Any())
          {
            var businessUnit = lenspec.Etalon.BusinessUnits.As(poa.OurBusinessUavis.FirstOrDefault().Company);
            var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
            var performer = lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetApprovalRoleKindPerformerByBusinessUnit(businessUnit, task.ApprovalRule, lenspec.EtalonDatabooks.Resources.RoleDescriptionResponsibleClerk, document);
            if (_obj.Performer.Equals(performer))
            {
              var requestPoaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
              var requestNpoaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
              if (requestPoaKind.Equals(document.DocumentKind))
              {
                if (addendums.Any(x => x.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered))
                {
                  e.AddError("Зарегистрируйте доверенности в карточках документов.");
                  return;
                }
              }
              if (requestNpoaKind.Equals(document.DocumentKind))
              {
                if (addendums.Any(x => lenspec.Etalon.PowerOfAttorneys.Is(x) && string.IsNullOrEmpty(lenspec.Etalon.PowerOfAttorneys.As(x).RegistryNumavis)))
                {
                  e.AddError("Заполните реестровый номер в карточках документов.");
                }
              }
            }
          }
        }
        
        #endregion
        
        #region Проверка на наличие версии документа
        
        var checkVersionsErrorMessage = Functions.ApprovalSimpleAssignment.Remote.CheckDocumentVersions(_obj, document);
        if (!string.IsNullOrEmpty(checkVersionsErrorMessage))
        {
          e.AddError(checkVersionsErrorMessage);
          return;
        }
        
        #endregion
        
        #region Проверка на заполненность полей Экспорт 1С и Дата экспорта в 1С
        
        var check1CFieldsErrorMessage = Functions.ApprovalSimpleAssignment.Remote.Check1CFields(_obj, document);
        if (!string.IsNullOrEmpty(check1CFieldsErrorMessage))
        {
          e.AddError(check1CFieldsErrorMessage, _obj.Info.Actions.Export1CApplicationForPaymentlenspec);
          return;
        }
        
        #endregion
      }
      
      base.Complete(e);
      
      if (_obj.DeliveryMethodavis != null)
      {
        Functions.ApprovalSimpleAssignment.Remote.RefreshDocumentDeliveryMethod(_obj, _obj.DeliveryMethodavis);
      }
    }

    public override bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return base.CanComplete(e);
    }
    //конец Добавлено Avis Expert

  }



}