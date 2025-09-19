using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PowerOfAttorneyModule.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Найти электронные доверенности по критериям из диалога.
    /// </summary>
    [LocalizeFunction("SearchFormalizedPowerOfAttorney_ResourceKey", "SearchFormalizedPowerOfAttorney_DescriptionResourceKey")]
    public virtual void SearchFormalizedPowerOfAttorney()
    {
      var dialog = Dialogs.CreateInputDialog("Поиск электронных доверенностей");
      var company = dialog.AddSelect("Наша организация", true, Sungero.Company.BusinessUnits.Null);
      var validFrom = dialog.AddDate("Действует с", true);
      var responsible = dialog.AddSelect("Ответственный инициатор", false, Sungero.Company.Employees.Null);
      var attorneyFilteredList = Sungero.Parties.Counterparties.GetAll(i => Sungero.Parties.People.Is(i) || Sungero.Parties.Companies.Is(i));
      var attorney = dialog.AddSelect("Поверенный", false, Sungero.Parties.Counterparties.Null).From(attorneyFilteredList);
      
      if(dialog.Show() == DialogButtons.Ok)
      {
        var result = lenspec.Etalon.FormalizedPowerOfAttorneys.GetAll(x => x.BusinessUnit == company.Value && x.ValidFrom >= validFrom.Value);
        if(responsible.Value != null)
        {
          result = result.Where(x => x.PreparedBy == responsible.Value);
        }
        if(attorney.Value != null)
        {
          result = result.Where(x => x.IssuedTo.Person == attorney.Value || x.IssuedToParty == attorney.Value);
        }
        result.Show();
      }
    }

    /// <summary>
    /// Создать электронную доверенность.
    /// </summary>
    [LocalizeFunction("CreateElectronicPowerOfAttorney_ResourceKey", "CreateElectronicPowerOfAttorney_DescriptionResourceKey")]
    public virtual void CreateElectronicPowerOfAttorney()
    {
      var document = lenspec.Etalon.FormalizedPowerOfAttorneys.Create();
      document.Show();
    }

    /// <summary>
    /// Найти доверенности по критериям из диалога.
    /// </summary>
    [LocalizeFunction("SearchPowerOfAttorney_ResourceKey", "SearchPowerOfAttorney_DescriptionResourceKey")]
    public virtual void SearchPowerOfAttorney()
    {
      var documentKindList = Sungero.Docflow.DocumentKinds.GetAll(x => x.DocumentType.DocumentTypeGuid == avis.PowerOfAttorneyModule.PublicConstants.Module.PowerOfAttorneyTypeGuid || 
                                                                 x.DocumentType.DocumentTypeGuid == avis.PowerOfAttorneyModule.PublicConstants.Module.FormalizedPowerOfAttorneyTypeGuid).ToList();
      
      var dialog = Dialogs.CreateInputDialog("Поиск доверенностей");
      var documentKind = dialog.AddSelect("Вид документа", false, Sungero.Docflow.DocumentKinds.Null).From(documentKindList);
      var company = dialog.AddSelect("Наша организация", true, Sungero.Company.BusinessUnits.Null);
      var validFrom = dialog.AddDate("Действует с", true);
      var responsible = dialog.AddSelect("Ответственный инициатор", false, Sungero.Company.Employees.Null);
      var attorneyFilteredList = Sungero.Parties.Counterparties.GetAll(i => Sungero.Parties.People.Is(i) || Sungero.Parties.Companies.Is(i));
      var attorney = dialog.AddSelect("Поверенный", false, Sungero.Parties.Counterparties.Null).From(attorneyFilteredList);
      
      if(dialog.Show() == DialogButtons.Ok)
      {
        var result = Functions.Module.Remote.SearchPoawerOfAttorneyFromDialog(documentKind.Value, company.Value, validFrom.Value.Value, responsible.Value, attorney.Value);
        result.Show("Найденные доверенности");
      }
    } 

    /// <summary>
    /// Создать задачу на отзыв доверенности.
    /// </summary>
    [LocalizeFunction("CreateRecallPowerOfAttorneyTask_ResourceKey", "CreateRecallPowerOfAttorneyTask_DescriptionResourceKey")]
    public virtual void CreateRecallPowerOfAttorneyTask()
    {
      var task = RecallPowerOfAttorneys.Create();
      task.Show();
    }

    /// <summary>
    /// Создать доверенность и показать.
    /// </summary>
    [LocalizeFunction("CreatePowerOfAttorney_ResourceKey", "CreatePowerOfAttorney_DescriptionResourceKey")]
    public virtual void CreatePowerOfAttorney()
    {
      var projectPoa = lenspec.Etalon.PowerOfAttorneys.Create();
      projectPoa.Show();
    }



  }
}