using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Condition;

namespace lenspec.Etalon.Server
{
  partial class ConditionFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Получить текст условия.
    /// </summary>
    /// <returns>Текст условия.</returns>
    public override string GetConditionName()
    {
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.ConditionType == ConditionType.UploadBusinessU)
          return lenspec.Etalon.Conditions.Resources.ConditionNameUploadBU;
        
        if (_obj.ConditionType == ConditionType.RequiresApprov)
          return lenspec.Etalon.Conditions.Resources.ConditionNameRequiresApproval;
        
        if (_obj.ConditionType == ConditionType.RepresType)
          return lenspec.Etalon.Conditions.Resources.ConditionNameRepresentativeTypeFormat(lenspec.Etalon.Conditions.Info.Properties.RepresentativeTypeavis.GetLocalizedValue(_obj.RepresentativeTypeavis));
        
        if (_obj.ConditionType == ConditionType.BusinessUnitAddresseeavis)
          return lenspec.Etalon.Conditions.Resources.ConditionNameBusinessUnitAddresseeavis;
        
        if (_obj.ConditionType == ConditionType.DocumentIsSignedavis)
          return lenspec.Etalon.Conditions.Resources.ConditionNameDocumentIsSignedavis;
        
        if (_obj.ConditionType == ConditionType.FtsListState)
          return lenspec.Etalon.Conditions.Resources.ConditionNameFtsListStateFormat(lenspec.Etalon.Conditions.Info.Properties.FtsListStateavis.GetLocalizedValue(_obj.FtsListStateavis));
        
        if (_obj.ConditionType == ConditionType.IsUsedInRX)
          return lenspec.Etalon.Conditions.Resources.ConditionNameIsUsedInRX;
        
        if (_obj.ConditionType == ConditionType.EmployeeRegion)
          return lenspec.Etalon.Conditions.Resources.ConditionNameEmployeeRegionFormat(lenspec.Etalon.Conditions.Info.Properties.EmployeeRegionlenspec.GetLocalizedValue(_obj.EmployeeRegionlenspec));
        
        if (_obj.ConditionType == ConditionType.EDSAppCategory)
          return lenspec.Etalon.Conditions.Resources.ConditionNameEDSAppCategory;
      }
      
      return base.GetConditionName();
    }
    //конец Добавлено Avis Expert
  }
}