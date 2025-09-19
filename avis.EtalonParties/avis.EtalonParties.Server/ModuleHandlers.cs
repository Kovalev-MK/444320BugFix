using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonParties.Server
{
  partial class ContractorsRegisterFolderHandlers
  {

    public virtual IQueryable<lenspec.Etalon.ICompany> ContractorsRegisterDataQuery(IQueryable<lenspec.Etalon.ICompany> query)
    {
      query = query.Where(c => c.WorkKindsAvisavis.Where(k => k.WorkKind != null).Any());
      
      if (_filter == null)
        return query;
      
      // Группа Основное.
      if (_filter.Region != null)
        query = query.Where(c => c.RegionOfPresencesavis.Any(r => _filter.Region == r.Region));
      
      // Набор флажков Статус контрагента.
      if (_filter.Strategic || _filter.NonStrategic)
        query = query.Where(c => _filter.Strategic && c.CounterpartyStatusavis == lenspec.Etalon.Company.CounterpartyStatusavis.Strategic ||
                            _filter.NonStrategic && c.CounterpartyStatusavis == lenspec.Etalon.Company.CounterpartyStatusavis.NonStrategic);
      
      // Набор флажков Статус нахождения в реестре.
      if (_filter.Included || _filter.Excluded)
        query = query.Where(c => _filter.Included && c.RegistryStatusavis == lenspec.Etalon.Company.RegistryStatusavis.Included ||
                            _filter.Excluded && c.RegistryStatusavis == lenspec.Etalon.Company.RegistryStatusavis.Deleted);
      
      // Группа Виды работ и материалы.
      if (_filter.WorkKind != null)
        query = query.Where(c => c.WorkKindsAvisavis.Any(r => _filter.WorkKind == r.WorkKind));
      if (_filter.Material != null)
        query = query.Where(c => c.Materialsavis.Any(r => _filter.Material == r.Material));
      
      return query;
    }

    public virtual IQueryable<lenspec.Etalon.ICompany> ContractorsRegisterPreFiltering(IQueryable<lenspec.Etalon.ICompany> query)
    {
      return query;
    }
  }

  partial class ProviderRegisterFolderHandlers
  {

    public virtual IQueryable<lenspec.Etalon.ICompany> ProviderRegisterDataQuery(IQueryable<lenspec.Etalon.ICompany> query)
    {
      query = query.Where(c =>c.Materialsavis.Where(m => m.Material != null).Any());
      
      if (_filter == null)
        return query;
      
            // Группа Основное.
      if (_filter.Region != null)
        query = query.Where(c => c.RegionOfPresencesavis.Any(r => _filter.Region == r.Region));
      
      // Набор флажков Статус контрагента.
      if (_filter.Strategic || _filter.NonStrategic)
        query = query.Where(c => _filter.Strategic && c.CounterpartyStatusavis == lenspec.Etalon.Company.CounterpartyStatusavis.Strategic ||
                            _filter.NonStrategic && c.CounterpartyStatusavis == lenspec.Etalon.Company.CounterpartyStatusavis.NonStrategic);
      
      // Набор флажков Статус нахождения в реестре.
      if (_filter.Included || _filter.Excluded)
        query = query.Where(c => _filter.Included && c.RegistryStatusavis == lenspec.Etalon.Company.RegistryStatusavis.Included ||
                            _filter.Excluded && c.RegistryStatusavis == lenspec.Etalon.Company.RegistryStatusavis.Deleted);
      
      // Группа Виды работ и материалы.
      if (_filter.WorkKind != null)
        query = query.Where(c => c.WorkKindsAvisavis.Any(r => _filter.WorkKind == r.WorkKind));
      if (_filter.Material != null)
        query = query.Where(c => c.Materialsavis.Any(r => _filter.Material == r.Material));
      
      return query;
    }

    public virtual IQueryable<lenspec.Etalon.ICompany> ProviderRegisterPreFiltering(IQueryable<lenspec.Etalon.ICompany> query)
    {
      return query;
    }
  }

  partial class EtalonPartiesHandlers
  {
  }
}