using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentFlowTask;

namespace lenspec.Etalon
{
  partial class DocumentFlowTaskSharedHandlers
  {

    public override void AddendaGroupPopulating(Sungero.Workflow.Interfaces.AttachmentGroupPopulatingEventArgs e)
    {
      var contractStatement = Etalon.ContractStatements.As(_obj.DocumentGroup.ElectronicDocuments.FirstOrDefault());
      if (contractStatement == null)
      {
        base.AddendaGroupPopulating(e);
        return;
      }
      
      var contractStatementC2Kind = Etalon.PublicFunctions.DocumentKind.Remote.GetDocumentKindByConstantCode(EtalonDatabooks.PublicConstants.ConstantDatabook.ContractStatementC2DocumentKind);
      var contractStatementC3Kind = Etalon.PublicFunctions.DocumentKind.Remote.GetDocumentKindByConstantCode(EtalonDatabooks.PublicConstants.ConstantDatabook.ContractStatementC3DocumentKind);

      if (!Equals(contractStatement.DocumentKind, contractStatementC2Kind) && !Equals(contractStatement.DocumentKind, contractStatementC3Kind))
        base.AddendaGroupPopulating(e);
      else
        e.PopulateFrom(_obj.DocumentGroup, documentGroup =>
                       {
                         // Документы, связанные связью "Приложение" с основным документом.
                         var allAddendas = Sungero.Docflow.PublicFunctions.Module.GetAllAddenda(contractStatement);
                         
                         var relatedContractStatements = PublicFunctions.ContractStatement.Remote.GetAllRelatedDocuments(contractStatement, Sungero.Docflow.PublicConstants.Module.SimpleRelationName,
                                                                                                                         new List<Sungero.Docflow.IDocumentKind> {contractStatementC2Kind, contractStatementC3Kind});
                         // Документы, связанные связью "Прочие" с основным документом.
                         allAddendas = allAddendas.Concat(relatedContractStatements).ToList();
                         
                         return allAddendas;
                       });
    }

    public override void DocumentGroupAdded(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      base.DocumentGroupAdded(e);

      var contractStatement = Etalon.ContractStatements.As(_obj.DocumentGroup.ElectronicDocuments.FirstOrDefault());
      if (contractStatement == null)
        return;
      
      var contractStatementC2Kind = Etalon.PublicFunctions.DocumentKind.Remote.GetDocumentKindByConstantCode(EtalonDatabooks.PublicConstants.ConstantDatabook.ContractStatementC2DocumentKind);
      var contractStatementC3Kind = Etalon.PublicFunctions.DocumentKind.Remote.GetDocumentKindByConstantCode(EtalonDatabooks.PublicConstants.ConstantDatabook.ContractStatementC3DocumentKind);
      
      if (!Equals(contractStatement.DocumentKind, contractStatementC2Kind) && !Equals(contractStatement.DocumentKind, contractStatementC3Kind))
        return;
      
      // Получить документы со связью "Прочие".
      var relatedContractStatements = PublicFunctions.ContractStatement.Remote.GetAllRelatedDocuments(contractStatement, Sungero.Docflow.PublicConstants.Module.SimpleRelationName,
                                                                                                      new List<Sungero.Docflow.IDocumentKind> {contractStatementC2Kind, contractStatementC3Kind});
      
      var otherGroupContractStatements = relatedContractStatements.Where(x => _obj.OtherGroup.All.Contains(x));
      
      foreach (var otherGroupContractStatement in otherGroupContractStatements)
        _obj.OtherGroup.All.Remove(otherGroupContractStatement);
    }

  }
}