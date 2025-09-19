using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Tenders.Client
{
  public class ModuleFunctions
  {
    
    //Добавлено Avis Expert

    /// <summary>
    /// Создать Протокол комитета аккредитации.
    /// </summary>
    [LocalizeFunction("CreateAccreditationCommitteeProtocol_ResourceKey", "CreateAccreditationCommitteeProtocol_DescriptionResourceKey")]
    public virtual void CreateAccreditationCommitteeProtocol()
    {
      Tenders.Functions.Module.Remote.CreateAccreditationCommitteeProtocol().Show();
    }

    /// <summary>
    /// Создать Анкету аккредитации.
    /// </summary>
    [LocalizeFunction("CreateTenderAccreditationForm_ResourceKey", "CreateTenderAccreditationForm_DescriptionResourceKey")]
    public virtual void CreateTenderAccreditationForm()
    {
      Tenders.Functions.Module.Remote.CreateTenderAccreditationForm().Show();
    }
    
    /// <summary>
    /// Поиск задач на согласование Протоколов тендерного комитета.
    /// </summary>
    [LocalizeFunction("SearchTenderCommitteeProtocolTasks_ResourceKey", "SearchTenderCommitteeProtocolTasks_DescriptionResourceKey")]
    public virtual void SearchTenderCommitteeProtocolTasks()
    {
      var searchDialog = Dialogs.CreateSearchDialog<Sungero.Docflow.IApprovalTask>();

      if (searchDialog.Show())
      {
        var query = searchDialog.GetQuery();
        query.Show();
      }
    }

    /// <summary>
    /// Поиск Протоколов тендерного комитета.
    /// </summary>
    [LocalizeFunction("SearchTenderCommitteeProtocol_ResourceKey", "SearchTenderCommitteeProtocol_DescriptionResourceKey")]
    public virtual void SearchTenderCommitteeProtocol()
    {
      var documentTypeDialog = Dialogs.CreateInputDialog("Выбор типа документа");
      var documentType = documentTypeDialog.AddSelect("Тип документа", true)
        .From(lenspec.Tenders.Resources.TenderCommitteeProtocol, lenspec.Tenders.Resources.AccreditationCommitteeProtocol);
      documentTypeDialog.Buttons.AddOkCancel();
      
      if (documentTypeDialog.Show() == DialogButtons.Ok)
      {
        if (documentType.Value == lenspec.Tenders.Resources.TenderCommitteeProtocol)
        {
          var searchDialog = Dialogs.CreateSearchDialog<Tenders.ITenderCommitteeProtocol>();
          if (searchDialog.Show())
          {
            var query = searchDialog.GetQuery();
            query.Show();
          }
        }
        if (documentType.Value == lenspec.Tenders.Resources.AccreditationCommitteeProtocol)
        {
          var searchDialog = Dialogs.CreateSearchDialog<Tenders.IAccreditationCommitteeProtocol>();
          if (searchDialog.Show())
          {
            var query = searchDialog.GetQuery();
            query.Show();
          }
        }
      }
    }
    
    /// <summary>
    /// Список организаций-подрядчиков.
    /// </summary>
    [LocalizeFunction("ShowContractorList_ResourceKey", "ShowContractorList_DescriptionResourceKey")]
    public virtual void ShowContractorList()
    {
      lenspec.Etalon.Companies.GetAll(x => x.IsContractoravis == true).Show();
    }

    /// <summary>
    /// Список организаций-поставщиков.
    /// </summary>
    [LocalizeFunction("ShowProvidersList_ResourceKey", "ShowProvidersList_DescriptionResourceKey")]
    public virtual void ShowProvidersList()
    {
      lenspec.Etalon.Companies.GetAll(x => x.IsProvideravis == true).Show();
    }

    /// <summary>
    /// Создать Протокол тендерного комитета.
    /// </summary>
    [LocalizeFunction("CreateTenderCommitteeProtocol_ResourceKey", "CreateTenderCommitteeProtocol_DescriptionResourceKey")]
    public virtual void CreateTenderCommitteeProtocol()
    {
      Tenders.Functions.Module.Remote.CreateTenderCommitteeProtocol().Show();
    }
    //конец Добавлено Avis Expert
    
    /// <summary>
    /// Создать документ Тендерная документация.
    /// </summary>
    [LocalizeFunction("CreateTenderDocument_ResourceKey", "CreateTenderDocument_DescriptionResourceKey")]
    public virtual void CreateTenderDocument()
    {
      Tenders.Functions.Module.Remote.CreateTenderDocument().Show();
    }

  }
}