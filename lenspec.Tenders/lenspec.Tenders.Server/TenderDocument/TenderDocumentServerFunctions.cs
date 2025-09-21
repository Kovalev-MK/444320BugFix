using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderDocument;

namespace lenspec.Tenders.Server
{
  partial class TenderDocumentFunctions
  {
    
    #region [Converters]
    
    /// <summary>
    /// Получить список наименований контрагентов.
    /// </summary>
    /// <param name="tenderDocument">Тендерный документ.</param>
    /// <returns>Наименования КА через запятую.</returns>
    [Converter("GetCounterpartyNames")]
    public static string GetCounterpartyNames(ITenderDocument tenderDocument)
    {
      var names = tenderDocument.Counterparties.Where(c => c.Counterparty != null).Select(c => c.Counterparty.Name);
      return string.Join(", ", names);
    }
    
    /// <summary>
    /// Получить список ИНН контрагентов.
    /// </summary>
    /// <param name="tenderDocument">Тендерный документ.</param>
    /// <returns>ИНН КА через запятую.</returns>
    [Converter("GetCounterpartyTINs")]
    public static string GetCounterpartyTINs(ITenderDocument tenderDocument)
    {
      var tinFiller = lenspec.Tenders.TenderDocuments.Resources.FillerFormat(Sungero.Parties.Counterparties.Info.Properties.TIN.LocalizedName);
      var tins = tenderDocument.Counterparties
        .Where(c => c.Counterparty != null)
        .Select(c => string.IsNullOrEmpty(c.Counterparty.TIN) ? tinFiller : c.Counterparty.TIN);
      
      return string.Join(", ", tins);
    }
    
    /// <summary>
    /// Получить список результатов согласования.
    /// </summary>
    /// <param name="tenderDocument">Тендерная доку.</param>
    /// <returns>Результаты согласования в формате "ФИО – результат".</returns>
    [Converter("GetApprovalResults")]
    public static string GetApprovalResults(ITenderDocument tenderDocument)
    {
      try
      {
        // Ищем задачу включения/исключения из реестра по дате принятия решения и вложенному документу.
        var task = Tenders.ApprovalCounterpartyRegisterTasks.GetAll(
          x =>
          Equals(x.QCDecisionDate, tenderDocument.QCDecisionDate) &&
          x.AttachmentDetails.Any(d => d.AttachmentId == tenderDocument.Id)
         ).FirstOrDefault();
        
        if (task != null)
          return Functions.ApprovalCounterpartyRegisterTask.GetApprovalResults(task);
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - GetApprovalResults error converter", ex);
      }
      return string.Empty;
    }
    
    #endregion [Converters]
    
    /// <summary>
    /// Сформировать решение комитета по квалификации о включении/исключении контрагента из реестра.
    /// </summary>
    /// <param name="task">Задача на согласование включения в реестр квалифицированных контрагентов/список дисквалифицированных контрагентов.</param>
    /// <returns>Решение комитета по квалификации о включении/исключении контрагента из реестра.</returns>
    [Public, Remote(IsPure = false)]
    public static ITenderDocument GetCounterpartyRegistryDecision(IApprovalCounterpartyRegisterTask task)
    {
      // Определение вида документа.
      var documentKind = Sungero.Docflow.DocumentKinds.Null;
      if (task.Procedure == Tenders.ApprovalCounterpartyRegisterTask.Procedure.Inclusion)
        documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DecisionOnInclusionOfCounterpartyKind);
      else if (task.Procedure == Tenders.ApprovalCounterpartyRegisterTask.Procedure.Exclusion)
        documentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.DecisionOnExclusionOfCounterpartyKind);
      else
        throw new ArgumentException(Tenders.Resources.UnacceptableProcedureValueError);
      
      // Подбор шаблона.
      var template = Sungero.Docflow.DocumentTemplates
        .GetAll(x => x.Status == Sungero.Docflow.DocumentTemplate.Status.Active && x.DocumentKinds.Any(k => Equals(k.DocumentKind, documentKind)))
        .FirstOrDefault();
      if (template == null)
        throw new Exception(lenspec.Tenders.TenderDocuments.Resources.MissingTemplateForDocumentKindFormat(documentKind.Name));
      
      var tenderDocument = TenderDocuments.CreateFrom(template);
      
      // Заполнение полей карточки.
      tenderDocument.DocumentKind = documentKind;
      SetBusinessUnit(tenderDocument);
      SetProcedure(tenderDocument, task);
      SetQCDecision(tenderDocument, task);
      tenderDocument.QCDecisionDate = task.QCDecisionDate;
      tenderDocument.ApprovalResult = task.ApprovalResult;
      
      var line = tenderDocument.Counterparties.AddNew();
      line.Counterparty = task.Counterparty;
      
      
      tenderDocument.Save();
      return tenderDocument;
    }
    
    /// <summary>
    /// Установить значение НОР из карточки текущего сотрудника.
    /// </summary>
    /// <param name="tenderDocument">Тендерная документация.</param>
    private static void SetBusinessUnit(ITenderDocument tenderDocument)
    {
      var employee = Sungero.Company.Employees.Current;
      if (employee == null)
        throw new Exception(lenspec.Tenders.TenderDocuments.Resources.CompleteActionAsEmployee);
      var businessUnit = employee.Department.BusinessUnit;
      if (businessUnit == null)
        throw new Exception(lenspec.Tenders.TenderDocuments.Resources.FillBusinessUnitForEmployeeFormat(employee.Name));
      
      tenderDocument.BusinessUnit = businessUnit;
    }
    
    /// <summary>
    /// Установить значение перечисления "Процедура" из задачи.
    /// </summary>
    /// <param name="tenderDocument">Тендерная документация.</param>
    /// <param name="task">Задача на согласование КК включения/исключения КА.</param>
    private static void SetProcedure(ITenderDocument tenderDocument, IApprovalCounterpartyRegisterTask task)
    {
      if (task.Procedure == Tenders.ApprovalCounterpartyRegisterTask.Procedure.Inclusion)
        tenderDocument.Procedure = Tenders.TenderDocument.Procedure.Inclusion;
      else if (task.Procedure == Tenders.ApprovalCounterpartyRegisterTask.Procedure.Exclusion)
        tenderDocument.Procedure = Tenders.TenderDocument.Procedure.Exclusion;
      else
        throw new ArgumentException(Tenders.Resources.UnacceptableProcedureValueError);
    }
    
    /// <summary>
    /// Установить значение перечисления "Решение КК" из задачи.
    /// </summary>
    /// <param name="tenderDocument">Тендерная документация.</param>
    /// <param name="task">Задача на согласование КК включения/исключения КА.</param>
    private static void SetQCDecision(ITenderDocument tenderDocument, IApprovalCounterpartyRegisterTask task)
    {
      if (task.QCDecision == Tenders.ApprovalCounterpartyRegisterTask.QCDecision.Positive)
        tenderDocument.QCDecision = Tenders.TenderDocument.QCDecision.Positive;
      else if (task.QCDecision == Tenders.ApprovalCounterpartyRegisterTask.QCDecision.Negative)
        tenderDocument.QCDecision = Tenders.TenderDocument.QCDecision.Negative;
      else
        throw new ArgumentException(Tenders.Resources.UnacceptableQCDecisionValueError);
    }
    
    /// <summary>
    /// Установить штамп на документ.
    /// </summary>
    /// <returns>Пустая строка, если выполнено без ошибок; Иначе – текст ошибки.</returns>
    [Public, Remote(IsPure = false)]
    public string AddStamp()
    {
      if (_obj == null)
        return lenspec.Tenders.TenderDocuments.Resources.DocumentNotFoundForStamp;
      
      if (!_obj.HasVersions)
        return Sungero.Docflow.OfficialDocuments.Resources.NoVersionError;
      
      return lenspec.Etalon.PublicFunctions.OfficialDocument.Remote.ConvertToPdfWithStamp(_obj, _obj.LastVersion.Id);
    }
  }
}