using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Tenders
{
  partial class SuppliersOfMaterialsAndServicesServerHandlers
  {

    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Tenders.Constants.SuppliersOfMaterialsAndServices.MaterialsAndWorkKindsTableName, SuppliersOfMaterialsAndServices.ReportSessionId);
    }

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      var companiesWorkKindsMatch = GetMatchingCompaniesByWorkKinds();
      var companiesMaterialsMatch = GetMatchingCompaniesByMaterials();
      var modelList = FillModelList(companiesWorkKindsMatch, companiesMaterialsMatch);
      Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Tenders.Constants.SuppliersOfMaterialsAndServices.MaterialsAndWorkKindsTableName, modelList);
      
      var workKindGroupsInputs = SuppliersOfMaterialsAndServices.WorkKindGroups.ToList();
      var workKindsInputs = SuppliersOfMaterialsAndServices.WorkKinds.ToList();
      var materialGroupsInputs = SuppliersOfMaterialsAndServices.MaterialGroups.ToList();
      var materialsInputs = SuppliersOfMaterialsAndServices.Materials.ToList();
      var regionsInputs = SuppliersOfMaterialsAndServices.Regions.ToList();
      var citiesInputs = SuppliersOfMaterialsAndServices.Cities.ToList();
      var inputParams = lenspec.Tenders.PublicFunctions.Module.ConvertCollectionParams(workKindGroupsInputs, workKindsInputs, materialGroupsInputs, materialsInputs, regionsInputs, citiesInputs);
      SuppliersOfMaterialsAndServices.InputParams = inputParams;
    }
    
    #region Служебные методы
    
    /// <summary>
    /// Выборка Организаций с подходящими материалами
    /// </summary>
    /// <returns>Организации</returns>
    private IQueryable<lenspec.Etalon.ICompany> GetMatchingCompaniesByMaterials()
    {
      var companiesMaterialsMatch = lenspec.Etalon.Companies.GetAll();
      if (SuppliersOfMaterialsAndServices.MaterialGroups.Any() || SuppliersOfMaterialsAndServices.Materials.Any())
      {
        
        var materialGroups = avis.EtalonParties.MaterialGroups.GetAll().Where(x => SuppliersOfMaterialsAndServices.MaterialGroups.Contains(x));
        var materials = avis.EtalonParties.Materials.GetAll().Where(x => SuppliersOfMaterialsAndServices.Materials.Contains(x));
        if (materialGroups.Any())
          companiesMaterialsMatch = companiesMaterialsMatch.Where(x => x.Materialsavis.Any(mt => materialGroups.Any(mg => Equals(mg, mt.MaterialGroup))));
        if (materials.Any())
          companiesMaterialsMatch = companiesMaterialsMatch.Where(x => x.Materialsavis.Any(mt => materials.Any(m => Equals(m, mt.Material))));
      }

      if (SuppliersOfMaterialsAndServices.Regions.Any())
      {
        var regions = Sungero.Commons.Regions.GetAll().Where(x => SuppliersOfMaterialsAndServices.Regions.Contains(x));
        companiesMaterialsMatch = companiesMaterialsMatch.Where(x => x.Materialsavis.Any(mt => regions.Any(r => Equals(r, mt.Region))));
      }
      if (SuppliersOfMaterialsAndServices.Cities.Any())
      {
        var cities = Sungero.Commons.Cities.GetAll().Where(x => SuppliersOfMaterialsAndServices.Cities.Contains(x));
        companiesMaterialsMatch = companiesMaterialsMatch.Where(x => x.Materialsavis.Any(mt => cities.Any(c => Equals(c, mt.City))));
      }
      return companiesMaterialsMatch;
    }
    
    /// <summary>
    /// Выборка Организаций с подходящими видами работ
    /// </summary>
    /// <returns>Организации</returns>
    private IQueryable<lenspec.Etalon.ICompany> GetMatchingCompaniesByWorkKinds()
    {
      var companiesWorkKindsMatch = lenspec.Etalon.Companies.GetAll();
      if (SuppliersOfMaterialsAndServices.WorkKindGroups.Any() || SuppliersOfMaterialsAndServices.WorkKinds.Any())
      {
        var workKindGroups = avis.EtalonParties.WorkGroups.GetAll().Where(x => SuppliersOfMaterialsAndServices.WorkKindGroups.Contains(x));
        var workKinds = avis.EtalonParties.WorkKinds.GetAll().Where(x => SuppliersOfMaterialsAndServices.WorkKinds.Contains(x));
        if (workKindGroups.Any())
          companiesWorkKindsMatch = companiesWorkKindsMatch.Where(x => x.WorkKindsAvisavis.Any(wkt => workKindGroups.Any(wkg => Equals(wkg, wkt.WorkGroup))));
        if (workKinds.Any())
          companiesWorkKindsMatch = companiesWorkKindsMatch.Where(x => x.WorkKindsAvisavis.Any(wkt => workKinds.Any(wk => Equals(wk, wkt.WorkKind))));
      }
      
      if (SuppliersOfMaterialsAndServices.Regions.Any())
      {
        var regions = Sungero.Commons.Regions.GetAll().Where(x => SuppliersOfMaterialsAndServices.Regions.Contains(x));
        companiesWorkKindsMatch = companiesWorkKindsMatch.Where(x => x.WorkKindsAvisavis.Any(wkt => regions.Any(r => Equals(r, wkt.Region))));
      }
      if (SuppliersOfMaterialsAndServices.Cities.Any())
      {
        var cities = Sungero.Commons.Cities.GetAll().Where(x => SuppliersOfMaterialsAndServices.Cities.Contains(x));
        companiesWorkKindsMatch = companiesWorkKindsMatch.Where(x => x.WorkKindsAvisavis.Any(wkt => cities.Any(c => Equals(c, wkt.City))));
      }
      return companiesWorkKindsMatch;
    }
    
    private List<Tenders.Structures.SuppliersOfMaterialsAndServices.WorkKindsAndMaterials> FillModelList(IQueryable<lenspec.Etalon.ICompany> companiesWorkKindsMatch, IQueryable<lenspec.Etalon.ICompany> companiesMaterialsMatch)
    {
      SuppliersOfMaterialsAndServices.ReportSessionId = Guid.NewGuid().ToString();
      
      var modelList = new List<Tenders.Structures.SuppliersOfMaterialsAndServices.WorkKindsAndMaterials>();
      // Виды работ
      if (companiesWorkKindsMatch != null)
      {
        foreach (var company in companiesWorkKindsMatch)
        {
          foreach (var workKindLine in company.WorkKindsAvisavis)
          {
            var model = Tenders.Structures.SuppliersOfMaterialsAndServices.WorkKindsAndMaterials.Create();
            model.ReportSessionId = SuppliersOfMaterialsAndServices.ReportSessionId;
            model.Name = company.Name;
            model.TIN = company.TIN;
            model.TRRC = company.TRRC;
            model.PSRN = company.PSRN;
            model.Accreditation = lenspec.Etalon.Companies.Info.Properties.RegistryStatusavis.GetLocalizedValue(company.RegistryStatusavis);
            model.TargetEntity = "Работы";
            model.EntityGroup = workKindLine.WorkGroup?.Name;
            model.EntityDetailing = workKindLine.WorkKind?.Name;
            model.Region = workKindLine.Region?.Name;
            model.City = workKindLine.City?.Name;
            modelList.Add(model);
          }
        }
      }
      // Материалы
      if (companiesMaterialsMatch != null)
      {
        foreach (var company in companiesMaterialsMatch)
        {
          foreach (var materialLine in company.Materialsavis)
          {
            var model = Tenders.Structures.SuppliersOfMaterialsAndServices.WorkKindsAndMaterials.Create();
            model.ReportSessionId = SuppliersOfMaterialsAndServices.ReportSessionId;
            model.Name = company.Name;
            model.TIN = company.TIN;
            model.TRRC = company.TRRC;
            model.PSRN = company.PSRN;
            model.Accreditation = lenspec.Etalon.Companies.Info.Properties.RegistryStatusavis.GetLocalizedValue(company.RegistryStatusavis);
            model.TargetEntity = "Материалы";
            model.EntityGroup = materialLine.MaterialGroup?.Name;
            model.EntityDetailing = materialLine.Material?.Name;
            model.Region = materialLine.Region?.Name;
            model.City = materialLine.City?.Name;
            modelList.Add(model);
          }
        }
      }
      
      return modelList;
    }
    
    #endregion
    
  }
}