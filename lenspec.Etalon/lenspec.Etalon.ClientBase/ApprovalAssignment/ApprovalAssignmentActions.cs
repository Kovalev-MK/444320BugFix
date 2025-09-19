using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalAssignment;

namespace lenspec.Etalon.Client
{
  partial class ApprovalAssignmentActions
  {
    public virtual void SearchPowerOfAttorneyslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var powerOfAttorney = lenspec.Etalon.PowerOfAttorneys.As(document);
      var businessUnits = powerOfAttorney.OurBusinessUavis.Select(x => x.Company);
      var powerOfAttorneys = Sungero.Docflow.PowerOfAttorneys.GetAll(x => businessUnits.Contains(x.BusinessUnit));
      if (!powerOfAttorneys.Any())
      {
        e.AddInformation(lenspec.Etalon.ApprovalAssignments.Resources.PoANotFound);
        return;
      }
      powerOfAttorneys.ShowModal();
    }

    public virtual bool CanSearchPowerOfAttorneyslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var requestPoa = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
      var requestNpoa = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var isInProcess = _obj.Status == Sungero.Workflow.AssignmentBase.Status.InProcess;
      var roleLawyer = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidResponsibleLawyer).SingleOrDefault();
      return isInProcess && _obj.Performer.IncludedIn(roleLawyer) && document != null && (requestPoa.Equals(document.DocumentKind) || requestNpoa.Equals(document.DocumentKind));
    }

    public virtual void CreatePowerOfAttorneylenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var powerOfAttorney = lenspec.Etalon.PowerOfAttorneys.As(document);
      
      var employeeTypeValue = PowerOfAttorneys.Info.Properties.AgentType.GetLocalizedValue(PowerOfAttorney.AgentType.Employee);
      var agentTypeValues = new string[] {
        employeeTypeValue,
        PowerOfAttorneys.Info.Properties.AgentType.GetLocalizedValue(PowerOfAttorney.AgentType.Entrepreneur),
        PowerOfAttorneys.Info.Properties.AgentType.GetLocalizedValue(PowerOfAttorney.AgentType.LegalEntity),
        PowerOfAttorneys.Info.Properties.AgentType.GetLocalizedValue(PowerOfAttorney.AgentType.Person)
      };
      
      // Список НОР, по которым доверенности уже сформированы.
      var businessUnitsWithPoA = _obj.AddendaGroup.OfficialDocuments
        .Where(d => Sungero.Docflow.PowerOfAttorneys.Is(d))
        .Select(d => d.BusinessUnit)
        .ToList();
      // Список НОР, доступных для формирования доверенностей.
      var businessUnits = Functions.ApprovalAssignment.Remote.GetBusinessUnitsForDialog().ToList().Except(businessUnitsWithPoA);
      var employees = Functions.ApprovalAssignment.Remote.GetEmployeesForDialog();
      var counterparties = Functions.ApprovalAssignment.Remote.GetCounterpartiesForDialog();
      var documentKinds = Functions.ApprovalAssignment.Remote.GetDocumentKindsForDialog();
      var contractCategories = Functions.ApprovalAssignment.Remote.GetContractCategoriesForDialog();
      
      var dialog = Dialogs.CreateInputDialog(lenspec.Etalon.ApprovalAssignments.Resources.CreatePoA);
      
      var businessUnitsSelected = dialog.AddSelectMany(PowerOfAttorneys.Info.Properties.OurBusinessUavis.LocalizedName, true, powerOfAttorney.OurBusinessUavis.Select(x => x.Company).ToArray()).From(businessUnits);
      businessUnitsSelected.IsEnabled = false;
      businessUnitsSelected.IsVisible = false;
      var businessUnitsText = dialog
        .AddMultilineString(PowerOfAttorneys.Info.Properties.OurBusinessUavis.LocalizedName, true, EtalonDatabooks.PublicFunctions.Module.GetBusinessUnitsNames(businessUnitsSelected.Value.ToList()))
        .WithRowsCount(PublicConstants.Docflow.ApprovalAssignment.Params.BusinessUnitsTextRowsCount);
      businessUnitsText.IsEnabled = false;
      var addBusinessUnits = dialog.AddHyperlink(lenspec.Etalon.ApprovalAssignments.Resources.AddBusinessUnits);
      var deleteBusinessUnits = dialog.AddHyperlink(lenspec.Etalon.ApprovalAssignments.Resources.RemoveBusinessUnits);
      
      var agentTypeSelected = dialog.AddSelect(PowerOfAttorneys.Info.Properties.AgentType.LocalizedName, true, PowerOfAttorneys.Info.Properties.AgentType.GetLocalizedValue(powerOfAttorney.AgentType)).From(agentTypeValues);
      var issuedToSelected = dialog.AddSelect(PowerOfAttorneys.Info.Properties.IssuedTo.LocalizedName, false, powerOfAttorney.IssuedTo).From(employees);
      var issuedToPartySelected = dialog.AddSelect(PowerOfAttorneys.Info.Properties.IssuedTo.LocalizedName, false, powerOfAttorney.IssuedToParty).From(counterparties);
      var documentKindsSelected = dialog.AddSelectMany(PowerOfAttorneys.Info.Properties.DocKindsavis.LocalizedName, false, powerOfAttorney.DocKindsavis.Select(x => x.Kind).ToArray()).From(documentKinds);
      var contractCategorySelected = dialog.AddSelectMany(lenspec.Etalon.ApprovalAssignments.Resources.ContractCategories, false, powerOfAttorney.ContractCategoryavis.Select(x => x.Category).ToArray()).From(contractCategories);
      var amountSelected = dialog.AddDouble(PowerOfAttorneys.Info.Properties.Amountavis.LocalizedName, false, powerOfAttorney.Amountavis);
      var validFromSelected = dialog.AddDate(PowerOfAttorneys.Info.Properties.ValidFrom.LocalizedName, true, powerOfAttorney.ValidFrom);
      var validTillSelected = dialog.AddDate(PowerOfAttorneys.Info.Properties.ValidTill.LocalizedName, true, powerOfAttorney.ValidTill);
      
      var isIssuedToEmployee = agentTypeSelected.Value == employeeTypeValue;
      var isManyRepresentatives = powerOfAttorney.IsManyRepresentatives == true;
      
      issuedToPartySelected.IsVisible =     !isIssuedToEmployee && !isManyRepresentatives;
      issuedToPartySelected.IsRequired =    !isIssuedToEmployee && !isManyRepresentatives;
      issuedToSelected.IsVisible =          isIssuedToEmployee  && !isManyRepresentatives;
      issuedToSelected.IsRequired =         isIssuedToEmployee  && !isManyRepresentatives;
      
      agentTypeSelected.IsVisible = !isManyRepresentatives;
      contractCategorySelected.IsEnabled =
        documentKindsSelected.Value != null &&
        documentKindsSelected.Value.Any(i => Sungero.Docflow.DocumentKinds.As(i).DocumentType.DocumentTypeGuid.Equals(Sungero.Contracts.PublicConstants.Module.ContractGuid));
      
      agentTypeSelected.SetOnValueChanged((x) =>
                                          {
                                            isIssuedToEmployee = x.NewValue == employeeTypeValue;

                                            issuedToPartySelected.IsVisible = !isIssuedToEmployee;
                                            issuedToPartySelected.IsRequired = !isIssuedToEmployee;
                                            issuedToSelected.IsVisible = isIssuedToEmployee;
                                            issuedToSelected.IsRequired = isIssuedToEmployee;
                                          });
      
      documentKindsSelected.SetOnValueChanged((x) =>
                                              {
                                                contractCategorySelected.IsEnabled =
                                                  x.NewValue != null &&
                                                  x.NewValue.Any(i => Sungero.Docflow.DocumentKinds.As(i).DocumentType.DocumentTypeGuid.Equals(Sungero.Contracts.PublicConstants.Module.ContractGuid));
                                              });
      
      // Обновление поля с наименованиеями НОР при изменении поля с НОР.
      businessUnitsSelected.SetOnValueChanged(
        (args) =>
        {
          businessUnitsText.Value = EtalonDatabooks.PublicFunctions.Module.GetBusinessUnitsNames(businessUnitsSelected.Value.ToList());
        });
      
      #region Гиперссылки на добавление и удаление НОР
      
      addBusinessUnits.SetOnExecute(
        () =>
        {
          var selectedBusinessUnits = businessUnits
            .Except(businessUnitsSelected.Value)
            .ShowSelectMany(lenspec.Etalon.ApprovalAssignments.Resources.ChooseBusinessUnitsForAdd);
          if (selectedBusinessUnits != null && selectedBusinessUnits.Any())
          {
            businessUnitsSelected.Value = businessUnitsSelected.Value.Union(selectedBusinessUnits);
          }
        });
      
      deleteBusinessUnits.SetOnExecute(
        () =>
        {
          var selectedBusinessUnits = businessUnitsSelected.Value
            .ShowSelectMany(lenspec.Etalon.ApprovalAssignments.Resources.ChooseBusinessUnitsForDelete);
          if (selectedBusinessUnits != null && selectedBusinessUnits.Any())
          {
            businessUnitsSelected.Value = businessUnitsSelected.Value.Except(selectedBusinessUnits);
          }
        });
      
      #endregion
      
      dialog.Buttons.AddOkCancel();
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        if (!Functions.ApprovalAssignment.Remote.CheckResponsibleClerkOnDialog(businessUnitsSelected.Value.ToList()))
        {
          e.AddError(lenspec.Etalon.PowerOfAttorneys.Resources.ErrorMessageDifferentResponsible);
          return;
        }
        
        // Заполнение полей для обновления карточки доверенности.
        var fields = lenspec.Etalon.Structures.Docflow.PowerOfAttorney.UpdatableFields.Create();
        fields.AgentType = lenspec.Etalon.PublicFunctions.PowerOfAttorney.Remote.GetAgentType(agentTypeSelected.Value);
        fields.Amount = amountSelected.Value;
        fields.BusinessUnits = businessUnitsSelected.Value.ToList<Sungero.Company.IBusinessUnit>();
        fields.ContractCategories = contractCategorySelected.Value.ToList<Sungero.Contracts.IContractCategory>();
        fields.DocumentKinds = documentKindsSelected.Value.ToList<Sungero.Docflow.IDocumentKind>();
        // Кому выдана.
        if (isIssuedToEmployee)
          fields.IssuedTo = issuedToSelected.Value;
        else
          fields.IssuedToParty = issuedToPartySelected.Value;
        fields.ValidFrom = validFromSelected.Value.Value;
        fields.ValidTill = validTillSelected.Value.Value;
        
        var updatingResult = Functions.ApprovalAssignment.Remote.UpdatePowerOfAttorney(_obj, powerOfAttorney, fields);
        if (updatingResult)
        {
          var message = Functions.ApprovalAssignment.Remote.CreatePowerOfAttorneysFromDialog(_obj, powerOfAttorney);
          if (!string.IsNullOrEmpty(message))
          {
            e.AddError(message);
            return;
          }
        }
        else
        {
          e.AddError(lenspec.Etalon.ApprovalAssignments.Resources.ErrorUpdatingRequestForCreatingPoA);
          return;
        }
      }
    }

    public virtual bool CanCreatePowerOfAttorneylenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var isInRoleLawyer = false;
      var isRequestToCreatePowerOfAttorneyKind = false;
      var isRequestToCreateNotarialPowerOfAttorneyKind = false;
      
      e.Params.TryGetValue(Constants.Docflow.ApprovalAssignment.Params.IsInRoleLawyer, out isInRoleLawyer);
      e.Params.TryGetValue(Constants.Docflow.ApprovalAssignment.Params.IsRequestToCreatePowerOfAttorneyKind, out isRequestToCreatePowerOfAttorneyKind);
      e.Params.TryGetValue(Constants.Docflow.ApprovalAssignment.Params.IsRequestToCreateNotarialPowerOfAttorneyKind, out isRequestToCreateNotarialPowerOfAttorneyKind);

      var isInProcess = _obj.Status == Sungero.Workflow.AssignmentBase.Status.InProcess;
      return
        isInProcess &&
        isInRoleLawyer &&
        (isRequestToCreatePowerOfAttorneyKind || isRequestToCreateNotarialPowerOfAttorneyKind);
    }

    public override void ExtendDeadline(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ExtendDeadline(e);
    }

    public override bool CanExtendDeadline(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      //TODO: заменить вызов remote-функции
      return base.CanExtendDeadline(e) && Functions.ApprovalAssignment.Remote.CheckPossibilityExtending(_obj);
    }

    //Добавлено Avis Expert
    public override void Forward(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      base.Forward(e);
    }

    public override bool CanForward(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      // Для Служебной записки скрыть результат выполнения Переадресовать.
      return base.CanForward(e) && !(document != null && Sungero.Docflow.Memos.Is(document));
    }

    public override void Approved(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      var isBodyUpdated = Functions.ApprovalAssignment.Remote.UpdateDocumentBody(_obj);
      if (!string.IsNullOrEmpty(isBodyUpdated) && !isBodyUpdated.Equals(lenspec.Etalon.ApprovalTasks.Resources.FailedToUpdateFieldsInDocumentsPrefix))
      {
        e.AddError(isBodyUpdated);
        return;
      }
      
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      // Проверка вложенности доверенности при согласовании заявки на создание доверенности
      if (Sungero.Docflow.PowerOfAttorneys.Is(document))
      {
        var lawyerRole = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidResponsibleLawyer).SingleOrDefault();
        if (_obj.Performer.IncludedIn(lawyerRole))
        {
          var requestPoaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
          var requestNpoaKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
          if (requestPoaKind.Equals(document.DocumentKind) || requestNpoaKind.Equals(document.DocumentKind))
          {
            if (!_obj.AddendaGroup.OfficialDocuments.Any(x => Sungero.Docflow.PowerOfAttorneys.Is(x)))
            {
              e.AddError("Сформируйте доверенности по действию на ленте.");
              return;
            }
          }
        }
      }
      
      if (lenspec.ApplicationsForPayment.ApplicationForPayments.Is(document))
      {
        if (!string.IsNullOrEmpty(_obj.ActiveText))
        {
          e.AddError(lenspec.Etalon.ApprovalAssignments.Resources.CommentMustBeEmpty);
          return;
        }
        if (lenspec.Etalon.ApprovalStages.As(_obj.Stage).RoleKindslenspec.Any(x => x.RoleKind?.Name == lenspec.Etalon.ApprovalAssignments.Resources.BudgetControllerRoleKindName ||
                                                                              x.RoleKind?.Name == lenspec.Etalon.ApprovalAssignments.Resources.ResponsibleForEconomicsRoleKindName ||
                                                                             x.RoleKind?.Name == lenspec.Etalon.ApprovalAssignments.Resources.BudgetController300 ||
                                                                             x.RoleKind?.Name == lenspec.Etalon.ApprovalAssignments.Resources.BudgetControllerRegl ||
                                                                             x.RoleKind?.Name == lenspec.Etalon.ApprovalAssignments.Resources.BudgetControllerFine ||
                                                                             x.RoleKind?.Name == lenspec.Etalon.ApprovalAssignments.Resources.BudgetControllerRefund))
          Functions.ApprovalAssignment.Remote.CheckUpdateApplicationForPayment(_obj, document);
        
      }
      
      base.Approved(e);
    }

    public override bool CanApproved(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return base.CanApproved(e);
    }
    //конец Добавлено Avis Expert

  }

}