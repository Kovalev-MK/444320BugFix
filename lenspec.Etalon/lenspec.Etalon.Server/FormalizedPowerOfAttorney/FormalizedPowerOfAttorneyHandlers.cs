using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.FormalizedPowerOfAttorney;

namespace lenspec.Etalon
{

  partial class FormalizedPowerOfAttorneyCreatingFromServerHandler
  {

    public override void CreatingFrom(Sungero.Domain.CreatingFromEventArgs e)
    {
      base.CreatingFrom(e);
    }
    
    public override void CopySigningGroup(Sungero.Domain.CreatingFromEventArgs e)
    {
      if (_source.PrincipalRepresentativeTypelenspec != lenspec.Etalon.FormalizedPowerOfAttorney.PrincipalRepresentativeTypelenspec.LegalEntity)
        base.CopySigningGroup(e);
    }
  }


  partial class FormalizedPowerOfAttorneyServerHandlers
  {
    
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // В коробке очищаются полномочия в зависимости от типа полномочий, нам необходимо сохранять не зависимо от типа, поэтому перезаполняем
      var generalPowers = _obj.Powers;
      var structuredPowerIds = new List<Sungero.PowerOfAttorneyCore.IPowerOfAttorneyClassifier>();
      if (_obj.StructuredPowers.Any(x => x.Power != null))
        structuredPowerIds.AddRange(_obj.StructuredPowers.Where(x => x.Power != null).Select(x => x.Power));
      
      base.BeforeSave(e);
      
      if (_obj.PowersType == Sungero.Docflow.FormalizedPowerOfAttorney.PowersType.Classifier)
        _obj.Powers = generalPowers;
      else
      {
        foreach (var item in structuredPowerIds)
        {
          var line = _obj.StructuredPowers.AddNew();
          line.Power = item;
        }
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.FlagOurSignatoryavis = lenspec.Etalon.Constants.Docflow.FormalizedPowerOfAttorney.FlagOurSignatoryValue;
      _obj.PrincipalRepresentativeTypelenspec = null;
      base.Created(e);
      _obj.BusinessUnit = Sungero.Docflow.PublicFunctions.Module.GetDefaultBusinessUnit(Sungero.Company.Employees.Current);
    }
    
  }

  partial class FormalizedPowerOfAttorneyIssuedToPartyPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> IssuedToPartyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.AgentType == FormalizedPowerOfAttorney.AgentType.LegalEntity)
        query = query.Where(x => lenspec.Etalon.Companies.Is(x) && lenspec.Etalon.Companies.As(x).GroupCounterpartyavis.IdDirectum5 != 17896408);
      else
        query = base.IssuedToPartyFiltering(query, e);
      return query;
    }
  }

  partial class FormalizedPowerOfAttorneyAuthoritiesavisAuthorityPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> AuthoritiesavisAuthorityFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.IsFormalizedPowerOfAttorneys == true);
      return query;
    }
  }
}