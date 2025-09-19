using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Tenders
{
  partial class SuppliersOfMaterialsAndServicesClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      var workGroupList = avis.EtalonParties.WorkGroups.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      var workKindList = avis.EtalonParties.WorkKinds.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      var materialGroupList = avis.EtalonParties.MaterialGroups.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      var materialList = avis.EtalonParties.Materials.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      var regionList = Sungero.Commons.Regions.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      var cityList = Sungero.Commons.Cities.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      
      var dialog = Dialogs.CreateInputDialog("Параметры отчета");
      var workGroupsSelected = dialog.AddSelectMany("Виды работ", false, avis.EtalonParties.WorkGroups.Null).From(workGroupList);
      var workKindsSelected = dialog.AddSelectMany("Детализация видов работ", false, avis.EtalonParties.WorkKinds.Null).From(workKindList);
      var materialGroupsSelected = dialog.AddSelectMany("Виды материалов", false, avis.EtalonParties.MaterialGroups.Null).From(materialGroupList);
      var materialsSelected = dialog.AddSelectMany("Детализация видов материалов", false, avis.EtalonParties.Materials.Null).From(materialList);
      var regionsSelected = dialog.AddSelectMany("Регионы присутствия", true, Sungero.Commons.Regions.Null).From(regionList);
      var citiesSelected = dialog.AddSelectMany("Населенные пункты", false, Sungero.Commons.Cities.Null).From(cityList);
      
      workGroupsSelected.SetOnValueChanged((x) =>
                                           {
                                             if (x.NewValue.Any())
                                               workKindsSelected.From(workKindList.Where(w => x.NewValue.Contains(w.Group)));
                                             else
                                               workKindsSelected.From(workKindList);
                                           });
      
      workKindsSelected.SetOnValueChanged((x) =>
                                          {
                                            if (!workGroupsSelected.Value.Any() && x.NewValue.Any())
                                              workGroupsSelected.Value = x.NewValue.Select(g => g.Group);
                                          });
      
      materialGroupsSelected.SetOnValueChanged((x) =>
                                               {
                                                 if (x.NewValue.Any())
                                                   materialsSelected.From(materialList.Where(m => x.NewValue.Contains(m.Group)));
                                                 else
                                                   materialsSelected.From(materialList);
                                               });
      
      materialsSelected.SetOnValueChanged((x) =>
                                          {
                                            if (!materialGroupsSelected.Value.Any() && x.NewValue.Any())
                                              materialGroupsSelected.Value = x.NewValue.Select(m => m.Group);
                                          });
      
      regionsSelected.SetOnValueChanged((x) =>
                                        {
                                          if (x.NewValue.Any())
                                            citiesSelected.From(cityList.Where(c => x.NewValue.Contains(c.Region)));
                                          else
                                            citiesSelected.From(cityList);
                                        });
      
      citiesSelected.SetOnValueChanged((x) =>
                                       {
                                         if (!regionsSelected.Value.Any() && x.NewValue.Any())
                                           regionsSelected.Value = x.NewValue.Select(c => c.Region);
                                       });
      
      dialog.SetOnRefresh((x) => { citiesSelected.IsRequired = !regionsSelected.Value.Any(); });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        SuppliersOfMaterialsAndServices.WorkKindGroups.AddRange(workGroupsSelected.Value);
        SuppliersOfMaterialsAndServices.WorkKinds.AddRange(workKindsSelected.Value);
        SuppliersOfMaterialsAndServices.MaterialGroups.AddRange(materialGroupsSelected.Value);
        SuppliersOfMaterialsAndServices.Materials.AddRange(materialsSelected.Value);
        SuppliersOfMaterialsAndServices.Regions.AddRange(regionsSelected.Value);
        SuppliersOfMaterialsAndServices.Cities.AddRange(citiesSelected.Value);
      }
    }

  }
}