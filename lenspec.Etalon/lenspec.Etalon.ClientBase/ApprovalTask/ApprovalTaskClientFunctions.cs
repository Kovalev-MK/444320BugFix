using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalTask;

namespace lenspec.Etalon.Client
{
  partial class ApprovalTaskFunctions
  {

    /// <summary>
    /// Проверить тип согласуемого документ
    /// </summary>
    ///<returns>True - если является чем-то из перечисленного, иначе False</returns>
    public bool CheckDocumentType()
    {
      var document = _obj.DocumentGroup.OfficialDocuments.SingleOrDefault();
      return Sungero.RecordManagement.OutgoingLetters.Is(document) || Sungero.RecordManagement.Orders.Is(document) || Sungero.RecordManagement.CompanyDirectives.Is(document) ||
        lenspec.ProtocolsCollegialBodies.ProtocolCollegialBodies.Is(document) || Sungero.Docflow.Memos.Is(document) || Sungero.Docflow.PowerOfAttorneyRevocations.Is(document) ||
        Sungero.Docflow.FormalizedPowerOfAttorneys.Is(document) || Sungero.Docflow.PowerOfAttorneys.Is(document) || Sungero.Contracts.SupAgreements.Is(document) || Sungero.Contracts.Contracts.Is(document) ||
        lenspec.Tenders.AccreditationCommitteeProtocols.Is(document) || lenspec.Tenders.TenderCommitteeProtocols.Is(document) || lenspec.ElectronicDigitalSignatures.EDSApplications.Is(document);
    }
    
    /// <summary>
    /// Показать диалог с задачами-дубликатами
    /// </summary>
    /// <param name="duplicates">Список задач</param>
    public static void GetDuplicateTasksDialog(List<Sungero.Docflow.IApprovalTask> duplicates)
    {
      var dialog = Dialogs.CreateTaskDialog("Документ уже направлен на согласование.");
      var showTasksButton = dialog.Buttons.AddCustom("Показать задачи");
      dialog.Buttons.AddCancel();
      if (dialog.Show() == showTasksButton)
        duplicates.Show("Дублирующие задачи");
    }

    /// <summary>
    /// Проверить наличие задач-дублкатов и вывести соответствующий диалог (вызывается из кода документа)
    /// </summary>
    /// <param name="document">Документ</param>
    /// <param name="isCheckAddendums">Проверять приложения?</param>
    /// <returns>True - есть дубликаты</returns>
    [Public]
    public static bool CheckDuplicates(Sungero.Docflow.IOfficialDocument document, bool isCheckAddendums)
    {
      var result = false;
      var duplicates = Functions.ApprovalTask.Remote.GetTaskDuplicates(document, isCheckAddendums);
      if (duplicates.Any())
      {
        result = true;
        GetDuplicateTasksDialog(duplicates);
      }
      return result;
    }
    
    /// <summary>
    /// Проверить наличие задач-дублкатов и вывести соответствующий диалог (вызывается из кода задачи)
    /// </summary>
    /// <param name="task">Задача</param>
    /// <param name="isCheckAddendums">Проверять приложения?</param>
    /// <returns>True - есть дубликаты</returns>
    [Public]
    public static bool CheckDuplicates(Sungero.Docflow.IApprovalTask task, bool isCheckAddendums)
    {
      var result = false;
      var document = task.DocumentGroup.OfficialDocuments.SingleOrDefault();
      var duplicates = Functions.ApprovalTask.Remote.GetTaskDuplicates(document, isCheckAddendums).Where(x => x.Id != task.Id).ToList();
      if (duplicates.Any())
      {
        result = true;
        GetDuplicateTasksDialog(duplicates);
      }
      return result;
    }
    
    /// <summary>
    /// Проверить наличие задач-дублкатов и вывести соответствующий диалог (вызывается из кода задачи)
    /// </summary>
    /// <param name="task">Задача</param>
    /// <param name="document">Документ</param>
    /// <param name="isCheckAddendums">Проверять приложения?</param>
    /// <returns>True - есть дубликаты</returns>
    [Public]
    public static bool CheckDuplicates(Sungero.Docflow.IApprovalTask task, Sungero.Docflow.IOfficialDocument document, bool isCheckAddendums)
    {
      var result = false;
      var duplicates = Functions.ApprovalTask.Remote.GetTaskDuplicates(document, isCheckAddendums).Where(x => x.Id != task.Id).ToList();
      if (duplicates.Any())
      {
        result = true;
        GetDuplicateTasksDialog(duplicates);
      }
      return result;
    }
    
    /// Отправить на печать штрихкоды документов.
    /// <param name="documents">Список документов.</param>
    /// </summary>
    public static void SaveBarcodesToFolderForPrining(List<Sungero.Docflow.IOfficialDocument> documents)
    {
      // Получаем список принтеров у данного пользователя.
      var printers = avis.PrinterSettings.PublicFunctions.Module.Remote.GetActivePrinters();
      if (printers.Count == 0)
      {
        Dialogs.ShowMessage(lenspec.SalesDepartmentArchive.SDARequestSubmissionToArchives.Resources.PrinterSettingsNotFound, MessageType.Warning);
        return;
      }
      var printerName = string.Empty;
      if (printers.Count == 1)
        printerName = printers.FirstOrDefault().Printer;
      else
      {
        var dialog = Dialogs.CreateInputDialog("Выберите принтер");
        var printer = dialog.AddSelect("Принтер", true).From(printers.Select(p => p.Printer).ToArray());
        if (dialog.Show() == DialogButtons.Ok)
          printerName = printer.Value;
        else
          return;
      }
      foreach (var addenda in documents)
        lenspec.Etalon.PublicFunctions.OfficialDocument.SaveBarcode(addenda, printerName);
    }
    
    /// <summary>
    /// Вывести диалоговое окно для выбора документа-обоснования.
    /// </summary>
    /// <param name="documents">Доступные документы-обоснования.</param>
    /// <returns>Выбранный документ-обоснование.</returns>
    public static Sungero.Docflow.IOfficialDocument ShowJustificationDocumentSelectDialog(IQueryable<Sungero.Docflow.IOfficialDocument> documents)
    {
      if (documents == null)
        return null;
      
      var dialog = Dialogs.CreateInputDialog(lenspec.Etalon.ApprovalTasks.Resources.ChooseAddendaDocument);
      
      // Форма выбора документа обоснования.
      var document = dialog.AddSelect(string.Empty, false, Sungero.Docflow.OfficialDocuments.Null);
      document.IsVisible = false;
      // Видимое представление контрола.
      var documentSelect = dialog.AddString(lenspec.Etalon.ApprovalTasks.Resources.JustificationDocument, true);
      documentSelect.IsEnabled = false;
      
      var chooseDocument = dialog.AddHyperlink(lenspec.Etalon.ApprovalTasks.Resources.ChooseDocument);
      chooseDocument.SetOnExecute(
        () =>
        {
          var selected = documents.ShowSelect(lenspec.Etalon.ApprovalTasks.Resources.JustificationDocumentPlural);
          if (selected == null)
            return;
          
          document.Value = selected;
          documentSelect.Value = selected.Name;
        });
      
      if (dialog.Show() == DialogButtons.Ok)
        return document.Value;
      
      return null;
    }
    
  }
}