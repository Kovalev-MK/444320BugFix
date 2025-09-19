using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.CounterpartyDocument;

namespace lenspec.Etalon.Server
{
  partial class CounterpartyDocumentFunctions
  {

    /// <summary>
    /// Получить инструкцию к документу.
    /// </summary>
    [Remote(IsPure = true)]
    public static string GetInstruction(Sungero.Docflow.IDocumentKind documentKind)
    {
      // Учредительный документ.
      if (IsDocumentKind(documentKind, Module.Parties.Constants.Module.ConstituentDocumentKind))
        return CounterpartyDocuments.Resources.ConstituentDocumentKindInstruction;
      
      // Устав и изменения.
      if (IsDocumentKind(documentKind, Module.Parties.Constants.Module.CharterAndChangesKind))
        return null;
      
      // Выписка из ЕГРЮЛ/ЕГРИП.
      if (IsDocumentKind(documentKind, Module.Parties.Constants.Module.ExtractFromEGRULKind))
        return null;
      
      // Прочие сведения о контрагенте.
      return CounterpartyDocuments.Resources.AdditionalInfoDocumentKindInstruction;
    }
    
    /// <summary>
    /// Проверка соответствия вида документа.
    /// </summary>
    /// <param name="counterpartyDocument">Сведения о контрагенте.</param>
    /// <param name="documentKindGuid">GUID вида документа для сравнения.</param>
    /// <returns>true, если вид документа совпадает. Иначе - false.</returns>
    [Public, Remote(IsPure = true)]
    public static bool IsDocumentKind(ICounterpartyDocument counterpartyDocument, Guid documentKindGuid)
    {
      if (counterpartyDocument == null)
        return false;
      
      return IsDocumentKind(counterpartyDocument.DocumentKind, documentKindGuid);
    }
    
    /// <summary>
    /// Проверка соответствия вида документа.
    /// </summary>
    /// <param name="documentKind">Вид сведений о контрагенте.</param>
    /// <param name="documentKindGuid">GUID вида документа для сравнения.</param>
    /// <returns>true, если вид документа совпадает. Иначе - false.</returns>
    [Public, Remote(IsPure = true)]
    public static bool IsDocumentKind(Sungero.Docflow.IDocumentKind documentKind, Guid documentKindGuid)
    {
      if (documentKind == null)
        return false;
      
      var documentKindFromGuid = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(documentKindGuid);
      if (documentKindFromGuid == null)
        return false;
      
      return Equals(documentKind, documentKindFromGuid);
    }
    
    /// <summary>
    /// Получение дублей сведений о контрагенте.
    /// </summary>
    /// <returns>Сведения, дублирующие текущие.</returns>
    [Remote(IsPure = true)]
    public IQueryable<ICounterpartyDocument> GetDuplicates()
    {
      return CounterpartyDocuments.GetAll(d =>
                                          !Equals(d, _obj) &&
                                          Equals(d.DocumentKind, _obj.DocumentKind) &&
                                          Equals(d.Counterparty, _obj.Counterparty));
    }
    
    /// <summary>
    /// Получить сведения о контрагенте.
    /// </summary>
    /// <param name="companyBase">Контрагент.</param>
    /// <returns>Все сведения, связанные с заданным контрагентом.</returns>
    [Public, Remote(IsPure = true)]
    public static List<ICounterpartyDocument> GetCounterpartyDocuments(Sungero.Parties.ICounterparty counterparty, List<System.Guid> documentKindGuids)
    {
      var result = new List<ICounterpartyDocument>();
      
      if (counterparty == null)
        return result;
      
      foreach (var counterpartyDocument in CounterpartyDocuments.GetAll(d => Equals(d.Counterparty, counterparty)))
        foreach (var documentKindGuid in documentKindGuids)
          if (IsDocumentKind(counterpartyDocument, documentKindGuid))
            result.Add(counterpartyDocument);
      
      return result;
    }
    
    /// <summary>
    /// Создать выписку из ЕГРЮЛ/ЕРГИП для контрагента.
    /// </summary>
    /// <param name="counterparty"></param>
    /// <returns></returns>
    [Public, Remote(IsPure = false)]
    public static ICounterpartyDocument CreateExtractFromEGRUL(Sungero.Parties.ICounterparty counterparty)
    {
      var counterpartyDocument = lenspec.Etalon.CounterpartyDocuments.Create();
      counterpartyDocument.DocumentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Module.Parties.Constants.Module.ExtractFromEGRULKind);
      counterpartyDocument.Counterparty = counterparty;
      var error = lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.GetExcerptEGRUL(counterpartyDocument.Counterparty.TIN, counterpartyDocument);
      if (!string.IsNullOrEmpty(error))
      {
        Logger.ErrorFormat("Avis - CounterpartyDocument - CreateExtractFromEGRUL - Не удалось получить Выписку из ЕГРЮЛ: {0}", error);
        return null;
      }

      counterpartyDocument.CustomDocumentDatelenspec = Calendar.Today;
      counterpartyDocument.Save();
      return counterpartyDocument;
    }
  }
}