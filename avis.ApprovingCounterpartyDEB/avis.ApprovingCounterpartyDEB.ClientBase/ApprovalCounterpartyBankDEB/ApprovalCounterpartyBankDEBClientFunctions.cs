using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEB;

namespace avis.ApprovingCounterpartyDEB.Client
{
  partial class ApprovalCounterpartyBankDEBFunctions
  {

    /// <summary>
    /// Видимость и доступность полей в зависимости от типа контрагента
    /// </summary>
    public void SetControlPropery()
    {
      var company = lenspec.Etalon.Companies.As(_obj.Counterparty);
      var isCompany = company != null;
      
      _obj.State.Properties.IsProvider.IsVisible = isCompany;
      _obj.State.Properties.IsContractor.IsVisible = isCompany;
      _obj.State.Properties.WorkKinds.IsVisible = isCompany;
      _obj.State.Properties.Materials.IsVisible = isCompany;
      _obj.State.Properties.CounterpartyType.IsVisible = isCompany;
      
      var isNotIncluded = isCompany && company.RegistryStatusavis != lenspec.Etalon.Company.RegistryStatusavis.Included;
      
      _obj.State.Properties.IsProvider.IsEnabled = isNotIncluded;
      _obj.State.Properties.IsContractor.IsEnabled = isNotIncluded;
      _obj.State.Properties.WorkKinds.IsEnabled = isNotIncluded;
      _obj.State.Properties.Materials.IsEnabled = isNotIncluded;
      
      _obj.State.Properties.PresenceRegion.IsEnabled = !isCompany || isNotIncluded;
      _obj.State.Properties.Cities.IsEnabled = _obj.PresenceRegion.Any() && (!isCompany || isNotIncluded);
    }
  }
}