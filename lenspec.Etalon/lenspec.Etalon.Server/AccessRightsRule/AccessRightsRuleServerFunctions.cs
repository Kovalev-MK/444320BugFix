using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.AccessRightsRule;

namespace lenspec.Etalon.Server
{
  partial class AccessRightsRuleFunctions
  {
    /// <summary>
    /// Получить документы по правилу.
    /// </summary>
    /// <returns>Документы по правилу.</returns>
    public override List<long> GetDocumentsByRule()
    {
      var documentIds = base.GetDocumentsByRule();
      
      if (_obj.IsGrantOnOurCompanyDocumentslenspec != true)
        return documentIds;
      
      var companies = _obj.BusinessUnits.Select(x => x.BusinessUnit.Company.Id).ToList();
      var contractualDocuments = Etalon.ContractualDocuments
        .GetAll(x => x.Archiveavis == true)
        .Where(x => companies.Contains(x.Counterparty.Id))
        .Select(x => x.Id);
      documentIds.AddRange(contractualDocuments);
      
      return documentIds;
    }
    
    /// <summary>
    /// Получить действующие записи справочника с признаком назначения прав на связанные задачи.
    /// </summary>
    /// <returns>Правила назначения прав.</returns>
    [Public]
    public static IQueryable<IAccessRightsRule> GetGrantingRightsOnTasks()
    {
      return AccessRightsRules.GetAll(x => x.IsGrantOnTasksavis == true && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
    }
    
    /// <summary>
    /// Получить документы по правилу назначения прав.
    /// </summary>
    /// <returns>Документы, подходящие под критерии правила назначения прав</returns>
    [Public]
    public IQueryable <Sungero.Docflow.IOfficialDocument> GetMatchingDocuments()
    {
      if (_obj == null)
        throw new ArgumentNullException(AccessRightsRules.Info.LocalizedName);
      
      var documentKinds = _obj.DocumentKinds.Select(t => t.DocumentKind);
      var businessUnits = _obj.BusinessUnits.Select(t => t.BusinessUnit);
      var departments =   _obj.Departments.Select(t => t.Department);
      
      var documents = Sungero.Docflow.OfficialDocuments
        .GetAll(d => !documentKinds.Any() || documentKinds.Contains(d.DocumentKind))
        .Where(d => !businessUnits.Any()  || businessUnits.Contains(d.BusinessUnit))
        .Where(d => !departments.Any()    || departments.Contains(d.Department));
      
      if (_obj.DocumentGroups.Any())
        documents = documents.Where(x => _obj.DocumentGroups.Any(groupRecord => Equals(x.DocumentGroup, groupRecord.DocumentGroup)));
      
      return documents;
    }
  }
}