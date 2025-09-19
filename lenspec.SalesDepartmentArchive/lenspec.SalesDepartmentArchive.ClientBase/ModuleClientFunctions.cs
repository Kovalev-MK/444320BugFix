using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.SalesDepartmentArchive.Client
{
  public class ModuleFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Вызвать диалог поиска.
    /// </summary>
    [LocalizeFunction("SearchActs_ResourceKey", "SearchActs_DescriptionResourceKey")]
    public virtual void SearchActs()
    {
      var searchDialog = Dialogs.CreateSearchDialog<lenspec.SalesDepartmentArchive.ISDAActBase>();

      if (searchDialog.Show())
      {
        var query = searchDialog.GetQuery();
        query.Show();
      }
    }

    /// <summary>
    /// Создать заявку на сдачу в архив.
    /// </summary>
    [LocalizeFunction("CreateRequestSubmissionToArchive_ResourceKey", "CreateRequestSubmissionToArchive_DescriptionResourceKey")]
    public virtual void CreateRequestSubmissionToArchive()
    {
      SalesDepartmentArchive.PublicFunctions.Module.Remote.CreateRequestSubmissionToArchive().Show();
    }
    
    /// <summary>
    /// Создать заявку на выдачу из архива.
    /// </summary>
    [LocalizeFunction("CreateRequestIssuanceFromArchive_ResourceKey", "CreateRequestIssuanceFromArchive_DescriptionResourceKey")]
    public virtual void CreateRequestIssuanceFromArchive()
    {
      SalesDepartmentArchive.PublicFunctions.Module.Remote.CreateRequestIssuanceFromArchive().Show();
    }

    /// <summary>
    /// Поиск завок в архив.
    /// </summary>
    /// <returns>Заявки в архив, удовлетворяющие условиям.</returns>
    [LocalizeFunction("SearchRequestToArchive_ResourceKey", "SearchRequestToArchive_DescriptionResourceKey")]
    public virtual IQueryable<ISDARequestSubmissionToArchive> SearchRequestToArchive()
    {
      // Создать диалог данных заявки в архив.
      var dialog = Dialogs.CreateInputDialog(lenspec.SalesDepartmentArchive.Resources.SearchRequestToArchive);
      
      var clientContractNumbber = dialog.AddString(lenspec.SalesDepartmentArchive.Resources.ClientContractNumber, false);
      var client = dialog.AddSelect(lenspec.SalesDepartmentArchive.Resources.Client, false, Sungero.Parties.Counterparties.Null);
      
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok; 
      
      dialog.SetOnButtonClick((args) =>
                              {
                                if (string.IsNullOrEmpty(clientContractNumbber.Value) && client.Value == null)
                                {
                                  args.AddError(lenspec.SalesDepartmentArchive.Resources.FillInAtLeastOneField);
                                }
                              });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        return Functions.Module.Remote.GetFilteredRequestToArchive(clientContractNumbber.Value, client.Value);
      }
      return null;
    }

    /// <summary>
    /// Поиск клиентских договоров.
    /// </summary>
    /// <returns>Клиентские договоры, удовлетворяющие условиям.</returns>
    [LocalizeFunction("SearchClientContracts_ResourceKey", "SearchClientContracts_DescriptionResourceKey")]
    public virtual IQueryable<ISDAClientContract> SearchClientContracts()
    {
      // Создать диалог данных клиентского договора.
      var dialog = Dialogs.CreateInputDialog(lenspec.SalesDepartmentArchive.Resources.SearchClientContracts);
      
      var contractNumber = dialog.AddString(lenspec.SalesDepartmentArchive.Resources.ClientContractNumber, false);
      var contractDate = dialog.AddDate(lenspec.SalesDepartmentArchive.Resources.ClientContractDate, false);
      var businessUnit = dialog.AddSelect(lenspec.SalesDepartmentArchive.Resources.BusinessUnit, false, Sungero.Company.BusinessUnits.Null);
      var client = dialog.AddSelect(lenspec.SalesDepartmentArchive.Resources.Client, false, Sungero.Parties.Counterparties.Null);
      
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok;      
      
      dialog.SetOnButtonClick((args) =>
                              {
                                if (string.IsNullOrEmpty(contractNumber.Value) && contractDate.Value == null && 
                                    businessUnit.Value == null && client.Value == null)
                                {
                                  args.AddError(lenspec.SalesDepartmentArchive.Resources.FillInAtLeastOneField);
                                }
                              });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        return Functions.Module.Remote.GetFilteredClientContracts(contractNumber.Value, contractDate.Value, businessUnit.Value, client.Value);
      }
      return null;
    }

    /// <summary>
    /// Создать доп. соглашение к клиентскому договору.
    /// </summary>
    [LocalizeFunction("CreateAgreementToClientContract_ResourceKey", "CreateAgreementToClientContract_DescriptionResourceKey")]
    public virtual void CreateAgreementToClientContract()
    {
      SalesDepartmentArchive.PublicFunctions.Module.Remote.CreateAgreementToClientContract().Show();
    }
    
    /// <summary>
    /// Создать приложение к клиентскому договору.
    /// </summary>
    [LocalizeFunction("CreateAddendumToClientContract_ResourceKey", "CreateAddendumToClientContract_DescriptionResourceKey")]
    public virtual void CreateAddendumToClientContract()
    {
      SalesDepartmentArchive.PublicFunctions.Module.Remote.CreateAddendumToClientContract().Show();
    }
    
    /// <summary>
    /// Создать прочий документ к клиентскому договору.
    /// </summary>
    [LocalizeFunction("CreateOtherDocumentToClientContract_ResourceKey", "CreateOtherDocumentToClientContract_DescriptionResourceKey")]
    public virtual void CreateOtherDocumentToClientContract()
    {
      SalesDepartmentArchive.PublicFunctions.Module.Remote.CreateOtherDocumentToClientContract().Show();
    }

    /// <summary>
    /// Создать клиентский договор.
    /// </summary>
    [LocalizeFunction("CreateClientContract_ResourceKey", "CreateClientContract_DescriptionResourceKey")]
    public virtual void CreateClientContract()
    {
      SalesDepartmentArchive.PublicFunctions.Module.Remote.CreateClientContract().Show();
    }
    //конец Добавлено Avis Expert

  }
}