using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.AccreditationCommitteeProtocol;

namespace lenspec.Tenders
{
  partial class AccreditationCommitteeProtocolMaterialKindsMaterialGroupPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MaterialKindsMaterialGroupFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(r => Equals(r.Status, avis.EtalonParties.MaterialGroup.Status.Active));
    }
  }

  partial class AccreditationCommitteeProtocolMaterialKindsMaterialPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MaterialKindsMaterialFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(r => Equals(r.Status, avis.EtalonParties.Material.Status.Active));
    }
  }

  partial class AccreditationCommitteeProtocolWorkKindsWorkGroupPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WorkKindsWorkGroupFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(r => Equals(r.Status, avis.EtalonParties.WorkGroup.Status.Active));
    }
  }

  partial class AccreditationCommitteeProtocolWorkKindsWorkKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WorkKindsWorkKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(r => Equals(r.Status, avis.EtalonParties.WorkKind.Status.Active));
    }
  }

  partial class AccreditationCommitteeProtocolPreparedByPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> PreparedByFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.PreparedByFiltering(query, e);
      
      var substituted = Sungero.CoreEntities.Substitutions.ActiveSubstitutedUsers.Select(x => x.Id).Append(Users.Current.Id);
      return query.Where(x => substituted.Contains(x.Id));
    }
  }

  partial class AccreditationCommitteeProtocolBusinessUnitPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> BusinessUnitFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.BusinessUnitFiltering(query, e);
      
      var documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.AccreditationCommitteeProtocolKind);
      var registrationSettingBusinessUnints = Sungero.Docflow.RegistrationSettings.GetAll(x => x.DocumentKinds.Any(d => d.DocumentKind == documentKind))
        .SelectMany(x => x.BusinessUnits.Select(b => b.BusinessUnit))
        .ToList()
        .Distinct();
      return query.Where(x => registrationSettingBusinessUnints.Contains(x));
    }
  }

  partial class AccreditationCommitteeProtocolAccreditationCommitteePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AccreditationCommitteeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(r => Equals(r.Status, AccreditationCommittee.Status.Active));
    }
  }


  partial class AccreditationCommitteeProtocolServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsProvider = false;
      _obj.IsContractor = false;
      _obj.IsFillingByDepartmentEmployees = false;
      _obj.BusinessUnit = null;
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.IsProvider != true && _obj.IsContractor != true)
        e.AddError(lenspec.Tenders.AccreditationCommitteeProtocols.Resources.NeedFillProviderOrContractor);
      
      if (_obj.IsContractor == true && !_obj.WorkKinds.Any())
        e.AddError(lenspec.Tenders.AccreditationCommitteeProtocols.Resources.NeedFillWorkKinds);
      
      if (_obj.IsProvider == true && !_obj.MaterialKinds.Any())
        e.AddError(lenspec.Tenders.AccreditationCommitteeProtocols.Resources.NeedFillMaterialKinds);
      
      if (_obj.Addressees == null || !_obj.Addressees.Any())
        e.AddError(lenspec.Tenders.AccreditationCommitteeProtocols.Resources.NeedFillAddressees);
      else
        Functions.AccreditationCommitteeProtocol.DeleteDublicatesAddressees(_obj);
      
      base.BeforeSave(e);
    }
  }

  partial class AccreditationCommitteeProtocolMaterialKindsCityPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MaterialKindsCityFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Region != null)
        query = query.Where(x => x.Region == _obj.Region);
      
      return query;
    }
  }

  partial class AccreditationCommitteeProtocolWorkKindsCityPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WorkKindsCityFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Region != null)
        query = query.Where(x => x.Region == _obj.Region);
      
      return query;
    }
  }

}