using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.ManagementCompanyJKHArhive.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Создание новой карточки "Акты управляющих компаний".
    /// </summary>
    [LocalizeFunction("CreateActsOfManagementCompany_ResourceKey", "CreateActsOfManagementCompany_DescriptionResourceKey")]
    public virtual void CreateActsOfManagementCompany()
    {
      lenspec.SalesDepartmentArchive.ActsOfManagementCompanies.Create().Show();
    }

    /// <summary>
    /// Поиск актов управляющих компаний.
    /// </summary>
    [LocalizeFunction("FindActsOfManagementCompanies_ResourceKey", "FindActsOfManagementCompanies_DescriptionResourceKey")]
    public virtual void FindActsOfManagementCompanies()
    {
      var searchDialog = Dialogs.CreateSearchDialog<lenspec.SalesDepartmentArchive.IActsOfManagementCompany>();

      if (searchDialog.Show())
      {
        var query = searchDialog.GetQuery();
        query.Show();
      }
    }
    
    /// <summary>
    /// Открыть список "Акты управляющих компаний".
    /// </summary>
    [LocalizeFunction("ShowActsOfManagementCompanies_ResourceKey", "ShowActsOfManagementCompanies_DescriptionResourceKey")]
    public virtual void ShowActsOfManagementCompanies()
    {
      lenspec.SalesDepartmentArchive.ActsOfManagementCompanies.GetAll().Show();
    }

    /// <summary>
    /// Создание новой карточки Документ-основание для создания договора управления МКД.
    /// </summary>
    [LocalizeFunction("CreateBasisForManagementContractMKD_ResourceKey", "CreateBasisForManagementContractMKD_DescriptionResourceKey")]
    public virtual void CreateBasisForManagementContractMKD()
    {
      BasisForManagementContractMKDs.Create().Show();
    }
    
    /// <summary>
    /// Создание новой карточки документа УК.
    /// </summary>
    [LocalizeFunction("CreateDocMC_ResourceKey", "CreateDocMC_DescriptionResourceKey")]
    public virtual void CreateDocMC()
    {
      DocumentForManagementCompanies.Create().Show();
    }

    /// <summary>
    /// Создание новой карточки документа МКД.
    /// </summary>
    [LocalizeFunction("CreateMKD_ResourceKey", "CreateMKD_DescriptionResourceKey")]
    public virtual void CreateMKD()
    {
      ManagementContractMKDs.Create().Show();
    }

    /// <summary>
    /// Поиск документов для УК.
    /// </summary>
    [LocalizeFunction("FindDucumentForMC_ResourceKey", "FindDucumentForMC_DescriptionResourceKey")]
    public virtual void FindDucumentForMC()
    {
      var dialog = Dialogs.CreateInputDialog("Поиск документов для УК");
      var number = dialog.AddString("№ договора", false);
      var date = dialog.AddDate("Дата договора управления МКД", false);
      var businessUnit = dialog.AddSelect("Наша орг.", false, Sungero.Company.BusinessUnits.Null);
      var client = dialog.AddSelect("Собственник", false, Sungero.Parties.Counterparties.Null);
      
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok; 
      
      dialog.SetOnButtonClick((args) =>
                              {
                                if (client.Value == null && businessUnit.Value == null && date.Value == null && string.IsNullOrEmpty(number.Value) == true)
                                {
                                  args.AddError("Должно быть заполнено хотя бы одно поле.");
                                }
                              });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        // Получаем список документов МКД.
        var mkds = ManagementContractMKDs.GetAll();
        
        if(!string.IsNullOrEmpty(number.Value))
          mkds = mkds.Where(m => m.Number == number.Value);
        
        if (date.Value != null)
          mkds = mkds.Where(m => m.RegistrationDate == date.Value);
        
        if (businessUnit.Value != null)
          mkds = mkds.Where(m => m.BusinessUnit == businessUnit.Value);
        
        if (client.Value != null)
          mkds = mkds.Where(m => m.Client == client.Value);
        
        // Получаем ид всех документов МКД.
        var mkdIds = mkds.Select(s => s.Id);
        // Получаем список Прочие документы для договорного архива управления МКД .
        var documentMCs = DocumentForManagementCompanies.GetAll(d => mkdIds.Contains(d.MKD.Id)).Select(s => s.Id);
        // Получаем список Документ-основание для создания договора управления МКД.
        var basisForContractMKDs = BasisForManagementContractMKDs.GetAll(d => mkdIds.Contains(d.MKD.Id)).Select(s => s.Id);
        
        // Соединяем список документов МКД и УК.
        var documents = avis.ManagementCompanyJKHArhive.ArhiveJKHDocumentBases.GetAll(d => mkdIds.Contains(d.Id) || documentMCs.Contains(d.Id) || basisForContractMKDs.Contains(d.Id));
        documents.Show("Документы для УК");
      }
    }
  }
}