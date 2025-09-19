using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBankDEB;

namespace avis.ApprovingCounterpartyDEB.Shared
{
  partial class ApprovalCounterpartyBankDEBFunctions
  {
    /// <summary>
    /// Заполнить имя.
    /// </summary>
    public override void FillName()
    {
      var date = _obj.Created.Value.ToShortDateString();
      _obj.Name = $"Согласование от {date} контрагента/банка с ДБ.";
      if (_obj.Counterparty != null)
      _obj.Name += $" Название {_obj.Counterparty.Name}";
    }
    
    /// <summary>
    /// Проверка типа контрагента
    /// </summary>
    public void CheckCounterpartyType()
    {
      var company = lenspec.Etalon.Companies.As(_obj.Counterparty);
      if (company != null && company.RegistryStatusavis == lenspec.Etalon.Company.RegistryStatusavis.Included)
      {
        _obj.IsProvider = company.IsProvideravis;
        _obj.IsContractor = company.IsContractoravis;
        foreach (var region in company.RegionOfPresencesavis)
        {
          var line = _obj.PresenceRegion.AddNew();
          line.Region = region.Region;
        }
        foreach (var city in company.Citieslenspec)
        {
          var line = _obj.Cities.AddNew();
          line.City = city.City;
        }
        foreach (var kind in company.WorkKindsAvisavis)
        {
          var line = _obj.WorkKinds.AddNew();
          line.WorkKind = kind.WorkKind;
          line.WorkGroup = kind.WorkGroup;
          line.Comment = kind.Comment;
        }
        foreach (var material in company.Materialsavis)
        {
          var line = _obj.Materials.AddNew();
          line.Material = material.Material;
          line.MaterialGroup = material.MaterialGroup;
          line.Comment = line.Comment;
        }
      }
      else
      {
        _obj.IsProvider = false;
        _obj.IsContractor = false;
        _obj.PresenceRegion.Clear();
        _obj.WorkKinds.Clear();
        _obj.Materials.Clear();
      }
    }
  }
}