using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ExecutionLawyerAssignment;

namespace avis.PowerOfAttorneyModule.Shared
{
  partial class ExecutionLawyerAssignmentFunctions
  {

    /// <summary>
    /// Перезаполнить свойства в проекте доверенности
    /// </summary>       
    public void RefillPowerOffAttorney()
    {
      var document = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      document.DocKindsavis.Clear();
      document.ContractCategoryavis.Clear();
      document.Amountavis = _obj.Amount;
      foreach(var item in _obj.DocKindsavis)
      {
        var line = document.DocKindsavis.AddNew();
        line.Kind = item.Kind;
      }
      foreach(var item in _obj.ContractCategoryavis)
      {
        var line = document.ContractCategoryavis.AddNew();
        line.Category = item.Category;
      }
      document.UpdateTemplateParameters();
      document.Save();
    }

  }
}