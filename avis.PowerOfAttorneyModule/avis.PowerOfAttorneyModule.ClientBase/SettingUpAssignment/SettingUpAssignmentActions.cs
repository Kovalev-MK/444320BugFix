using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.SettingUpAssignment;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class SettingUpAssignmentActions
  {
    public virtual void CreateSigningRight(Sungero.Domain.Client.ExecuteActionArgs e)
    {
//      var dialog = Dialogs.CreateInputDialog(avis.PowerOfAttorneyModule.SettingUpAssignments.Resources.CreateSigningRightActionDialogTitle);
//      var task = PowerOfAttorneyModule.ExecutionPowerOfAttorneys.As(_obj.Task);
//      var poaProject = task.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
//      var poas = task.POAs.PowerOfAttorneys.AsEnumerable();
//      
//      var kinds = poaProject.DocKindsavis.Select(i => i.Kind).ToArray();
//      var categories = poaProject.ContractCategoryavis.Select(i => i.Category).ToArray();
//      var businessUnits = poaProject.OurBusinessUavis.Select(i => i.Company).ToArray();
//      var attorneys = Functions.ExecutionPowerOfAttorney.Remote.GetAttorneyPerformers(task).Where(x => x.Login != null && x.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).ToArray();
//      
//      var attorneySelected = dialog.AddSelectMany("Поверенные", true, attorneys).From(attorneys);
//      var kindSelected = dialog.AddSelectMany("Виды документов", false, kinds).From(kinds);
//      var amountSelected = dialog.AddDouble("Лимит суммы", false, poaProject.Amountavis);
//      var categorySelected = dialog.AddSelectMany("Категории договоров", false, categories).From(categories);
//      var companySelected = dialog.AddSelectMany("Наши организации", false, businessUnits).From(businessUnits);
//      
//      var noLimitEnumerationValue = Sungero.Docflow.SignatureSettings.Info.Properties.Limit.GetLocalizedValue(Sungero.Docflow.SignatureSetting.Limit.NoLimit);
//      var amountEnumerationValue = Sungero.Docflow.SignatureSettings.Info.Properties.Limit.GetLocalizedValue(Sungero.Docflow.SignatureSetting.Limit.Amount);
//      var limitSelected = dialog.AddSelect("Лимит", true, amountEnumerationValue).From(noLimitEnumerationValue, amountEnumerationValue);
//      limitSelected.IsEnabled = false;
//      amountSelected.SetOnValueChanged((x) =>
//                                       {
//                                         if(x.NewValue != null)
//                                         {
//                                           limitSelected.Value = amountEnumerationValue;
//                                           limitSelected.IsEnabled = false;
//                                         }
//                                         else
//                                         {
//                                           limitSelected.Value = noLimitEnumerationValue;
//                                           limitSelected.IsEnabled = true;
//                                         }
//                                       });
//      limitSelected.SetOnValueChanged((x) =>
//                                      {
//                                        if(x.NewValue.Equals(amountEnumerationValue))
//                                          amountSelected.IsRequired = true;
//                                        else
//                                          amountSelected.IsRequired = false;
//                                      });
//      var createButton = dialog.Buttons.AddCustom("Создать");
//      dialog.Buttons.AddCancel();
//      if(dialog.Show() == createButton)
//      {
//        foreach (var poa in poas)
//        {
//          foreach(var attorney in attorneySelected.Value)
//          {
//            var signatureSetting = Sungero.Docflow.SignatureSettings.Create();
//            signatureSetting.Reason = Sungero.Docflow.SignatureSetting.Reason.PowerOfAttorney;
//            signatureSetting.SigningReason = $"Доверенность № {poa.RegistrationNumber}";
//            
//            signatureSetting.Document = Sungero.Docflow.OfficialDocuments.As(poa);
//            signatureSetting.Recipient = Sungero.CoreEntities.Recipients.As(attorney);
//            if(kindSelected != null && kindSelected.Value.Any())
//            {
//              foreach(var kind in kindSelected.Value)
//              {
//                var lineKind = signatureSetting.DocumentKinds.AddNew();
//                lineKind.DocumentKind = kind;
//              }
//            }
//            if(categorySelected != null && categorySelected.Value.Any())
//            {
//              foreach(var category in categorySelected.Value)
//              {
//                var lineCategory = signatureSetting.Categories.AddNew();
//                lineCategory.Category = category;
//              }
//            }
//            if(!signatureSetting.BusinessUnits.Any(x => poa.BusinessUnit.Equals(x.BusinessUnit)) && companySelected != null && companySelected.Value.Any(x => x.Equals(poa.BusinessUnit)))
//            {
//              var bu = signatureSetting.BusinessUnits.AddNew();
//              bu.BusinessUnit = poa.BusinessUnit;
//            }
//            var limit = limitSelected.Value.Equals(noLimitEnumerationValue) ? Sungero.Docflow.SignatureSetting.Limit.NoLimit : Sungero.Docflow.SignatureSetting.Limit.Amount;
//            signatureSetting.Limit = limit;
//            signatureSetting.Amount = amountSelected.Value ?? 0;
//            signatureSetting.Currency = Sungero.Commons.Currencies.GetAll(x => x.AlphaCode.Equals(Sungero.Docflow.Resources.CurrencyAlphaCodeRUB)).FirstOrDefault();
//            signatureSetting.ValidFrom = poa.ValidFrom;
//            signatureSetting.ValidTill = poa.ValidTill;
//            signatureSetting.Save();
//          }
//        }
//        e.AddInformation(avis.PowerOfAttorneyModule.SettingUpAssignments.Resources.InformationMessageSettingUpAction);
//      }
    }

    public virtual bool CanCreateSigningRight(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void Complete(Sungero.Workflow.Client.ExecuteResultActionArgs e)
    {
      
    }

    public virtual bool CanComplete(Sungero.Workflow.Client.CanExecuteResultActionArgs e)
    {
      return true;
    }

  }

}