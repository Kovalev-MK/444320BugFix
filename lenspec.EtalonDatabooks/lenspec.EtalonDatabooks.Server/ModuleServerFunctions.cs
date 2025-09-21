using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Packaging;
using CommonLibrary;
using Sungero.Domain.Shared;
using Sungero.Metadata;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace lenspec.EtalonDatabooks.Server
{
  public class ModuleFunctions
  {

    //Добавлено Avis Expert
    
    /// <summary>
    /// Получить массив ИД НОР из константы "ИД НОР с фильтрацией по бюджету".
    /// </summary>
    /// <returns>Массив ИД НОР.</returns>
    [Public]
    public string[] GetBusinessUnitIdsFromConstant()
    {
      var businessUnitIdsString = EtalonDatabooks.PublicFunctions.Module.GetConstantValueByCode(EtalonDatabooks.PublicConstants.ConstantDatabook.BussinessUnitsForFilteringByBudget);
      return businessUnitIdsString.Split(';');
    }
    
    /// <summary>
    /// Получить значение константы по её коду.
    /// </summary>
    /// <param name="code">Код константы.</param>
    /// <returns>Значение константы.</returns>
    [Public]
    public string GetConstantValueByCode(string code)
    {
      return EtalonDatabooks.ConstantDatabooks.GetAll(x => x.Code == code).FirstOrDefault()?.Value;
    }
    
    #region Работа с Excel
    
    [Public]
    public lenspec.EtalonDatabooks.Structures.Module.IXlsxSheet GetExcelSheet(System.IO.Stream body, bool hasHeader = true)
    {
      return GetExcelSheet(body, hasHeader, string.Empty);
    }
    
    [Public]
    public lenspec.EtalonDatabooks.Structures.Module.IXlsxSheet GetExcelSheet(System.IO.Stream body)
    {
      return GetExcelSheet(body, true, string.Empty);
    }
    
    [Public]
    public lenspec.EtalonDatabooks.Structures.Module.IXlsxSheet GetExcelSheet(System.IO.Stream body, bool hasHeader = true, string sheetName = "")
    {
      var xlsxFile = lenspec.EtalonDatabooks.IsolatedFunctions.ExcelParseArea.GetCellsData(body, hasHeader);
      
      if (!xlsxFile.Sheets.Any())
        return null;
      
      if (string.IsNullOrEmpty(sheetName) || xlsxFile.Sheets.Count() == 1)
        return xlsxFile.Sheets.FirstOrDefault().Value;
      
      return xlsxFile.Sheets.FirstOrDefault(x => x.Key == sheetName).Value;
    }
    
    [Public]
    public string InsertDataToExcel(System.IO.Stream body, List<string[]> importResult)
    {
      return InsertDataToExcel(body, importResult, true, string.Empty);
    }
    
    [Public]
    public string InsertDataToExcel(System.IO.Stream body, List<string[]> importResult, bool hasHeader = true)
    {
      return InsertDataToExcel(body, importResult, hasHeader, string.Empty);
    }
    
    [Public]
    public string InsertDataToExcel(System.IO.Stream body, List<string[]> importResult, bool hasHeader = true, string sheetName = "")
    {
      var importResultRow = lenspec.EtalonDatabooks.Structures.Module.XlsxTable.Create();
      importResultRow.Rows = importResult.Select(x => (lenspec.EtalonDatabooks.Structures.Module.IXlsxRow) new lenspec.EtalonDatabooks.Structures.Module.XlsxRow { Data = x } ).ToList();
      
      try
      {
        var stream = lenspec.EtalonDatabooks.IsolatedFunctions.ExcelParseArea.InsertDataToExcel(body, importResultRow, hasHeader, sheetName);
        body.SetLength(0);
        stream.CopyTo(body);
        body.Seek(0, System.IO.SeekOrigin.Begin);
      }
      catch (Exception ex)
      {
        return ex.Message;
      }

      return string.Empty;
    }
    
    /// <summary>
    /// Экспортировать excel-файл архивом.
    /// </summary>
    /// <param name="body">Бинарное тело документа.</param>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Архив.</returns>
    [Public]
    public Sungero.Core.IZip ExportBinaryDataAsZip(byte[] body, string fileName)
    {
      try
      {
        fileName = "Результат загрузки - " + fileName;
        var zip = Sungero.Core.Zip.Create();
        zip.Add(body, fileName, "xlsx");
        string zipName = $"Результат загрузки {Calendar.UserToday.ToShortDateString()}.zip";
        string tempFolderName = CommonLibrary.FileUtils.NormalizeFileName(zipName);
        zip.Save(tempFolderName);
        return zip;
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - ExportBinaryDataAsZip - ", ex);
        return null;
      }
    }
    
    /// <summary>
    /// Получить структуры данных из файла Excel.
    /// </summary>
    /// <returns>Матрица полей.</returns>
    [Public]
    public List<string[]> GetDataFromExcel(System.IO.Stream body)
    {
      var result = new List<string[]>();
      try
      {
        using (var document = SpreadsheetDocument.Open(body, false))
        {
          var workbookPart = document.WorkbookPart;
          var stringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
          var stringTable = stringTablePart.SharedStringTable;
          var worksheetPart = workbookPart.WorksheetParts.First();
          var sheet = worksheetPart.Worksheet;
          var cells = sheet.Descendants<Cell>();
          var rows = sheet.Descendants<Row>();
          var maxElements = rows.First().Elements<Cell>().Count();

          foreach (var row in rows)
          {
            var rowData = new string[maxElements];
            for (var i = 0; i < maxElements; i++)
              rowData[i] = string.Empty;

            foreach (var cell in row.Elements<Cell>())
            {
              if (cell.CellValue != null)
              {
                // Дата.
                if (cell.DataType != null && cell.DataType == CellValues.Date)
                {
                  DateTime dateOfBirth;
                  if (Calendar.TryParseDate(stringTable.ChildElements[int.Parse(cell.CellValue.Text)].InnerText, out dateOfBirth))
                  {
                    rowData[CellReferenceToIndex(cell)] = dateOfBirth.ToShortDateString();
                  }
                  continue;
                }
                // Общий.
                var innerText = cell.DataType != null && cell.DataType == CellValues.SharedString ?
                  stringTable.ChildElements[int.Parse(cell.CellValue.Text)].InnerText :
                  cell.CellValue.InnerText;
                rowData[CellReferenceToIndex(cell)] = innerText;
              }
            }

            // В обработку попадают строки, над которыми проводились какие-либо действия в редакторе,
            // включая пустые. Если в строке все ячейки пустые, то завершаем обработку.
            if (!string.IsNullOrWhiteSpace(rowData.Max()))
              result.Add(rowData);
            else
              break;
          }
          result.RemoveAt(0);
        }
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Не удалось получить данные из xlsx-файла: {0}", ex.Message), ex);
      }
      return result;
    }
    
    /// <summary>
    /// Поиск правильного индекса ячейки.
    /// </summary>
    /// <param name="cell">Ячейка.</param>
    /// <returns>Индекс.</returns>
    private static int CellReferenceToIndex(Cell cell)
    {
      int index = 0;
      int countChar = 0;
      string reference = cell.CellReference.ToString().ToUpper();
      foreach (char ch in reference)
      {
        countChar++;
        if (Char.IsLetter(ch))
        {
          int value = (int)ch - (int)'A';
          //index = (index == 0) ? value : ((index + 1) * 26) + value;
          if (index == 0)
            index = value;
          if (countChar > 1)
            index = 26 + index;
        }
        else
          return index;
      }
      return index;
    }
    
    /// <summary>
    /// Получить структуры данных из файла Excel.
    /// </summary>
    /// <returns>Матрица полей.</returns>
    [Public]
    public string InsertDataFromExcel(System.IO.Stream body, List<string[]> rowsResult)
    {
      var result = string.Empty;
      try
      {
        using (var document = SpreadsheetDocument.Open(body, true))
        {
          var workbookPart = document.WorkbookPart;
          var stringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
          var stringTable = stringTablePart.SharedStringTable;
          var worksheetPart = workbookPart.WorksheetParts.First();
          
          for (int i = 0; i < rowsResult.Count; i++)
          {
            Cell cell = InsertCellInWorksheet("M", (uint)(i + 2), worksheetPart);
            cell.CellValue = new CellValue(rowsResult[i][0]);
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
            
            cell = InsertCellInWorksheet("N", (uint)(i + 2), worksheetPart);
            cell.CellValue = new CellValue(rowsResult[i][1]);
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
          }
          worksheetPart.Worksheet.Save();
          Logger.Debug("Avis - InsertDataFromExcel - запись в файл");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Avis - InsertDataFromExcel - ", ex);
        result = string.Format("Не удалось записать данные в xlsx-файл: {0}", ex.Message);
      }
      return result;
    }
    
    private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
    {
      Worksheet worksheet = worksheetPart.Worksheet;
      SheetData sheetData = worksheet.GetFirstChild<SheetData>();
      string cellReference = columnName + rowIndex;
      Row row;
      if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
      {
        row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
      }
      else
      {
        row = new Row() { RowIndex = rowIndex };
        sheetData.Append(row);
      }

      if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
      {
        return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
      }
      else
      {
        Cell refCell = null;
        foreach (Cell cell in row.Elements<Cell>())
        {
          if (cell.CellReference.Value.Length == cellReference.Length)
          {
            if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
            {
              refCell = cell;
              break;
            }
          }
        }

        Cell newCell = new Cell() { CellReference = cellReference };
        row.InsertBefore(newCell, refCell);

        worksheet.Save();
        return newCell;
      }
    }
    
    #endregion
    
    #region Автоматизированные сотрудники
    
    /// <summary>
    /// Проверить, является ли сотрудник автоматизированным.
    /// </summary>
    /// <returns>True, если струдник автоматизирован, false, если не автоматизирован.</returns>
    [Public, Remote(IsPure = true)]
    public bool IsAutomatedEmployee(Sungero.Company.IEmployee employee)
    {
      var notAutomatedEmployeesIds = EtalonDatabooks.PublicFunctions.Module.GetNotAutomatedEmployeesAvis(new List<Sungero.Company.IEmployee>() { employee }).Select(x => x.Id); //Sungero.Company.PublicFunctions.Module.Remote.GetNotAutomatedEmployees(new List<Sungero.Company.IEmployee>() { employee }).Select(x => x.Id);
      return !notAutomatedEmployeesIds.Contains(employee.Id);
    }
    
    /// <summary>
    /// Получить неавтоматизированных сотрудников без замещения.
    /// </summary>
    /// <param name="employees"> Список сотрудников для обработки.</param>
    /// <returns> Список неавтоматизированных сотрудников без замещения.</returns>
    [Public]
    public IQueryable<Sungero.Company.IEmployee> GetNotAutomatedEmployeesAvis(List<Sungero.Company.IEmployee> employees)
    {
      var notAutomatedEmployeesWoSubstitution = new List<Sungero.Company.IEmployee>();
      try
      {
        var signOutEmployees = employees.Where(r => r.Login == null || r.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed).ToList();
        
        var substitutionsSource = Sungero.CoreEntities.Substitutions.GetAll().Where(x => x.IsSystem != true &&
                                                                                    x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active &&
                                                                                    (!x.StartDate.HasValue || Calendar.Today >= x.StartDate) &&
                                                                                    (!x.EndDate.HasValue || Calendar.Today <= x.EndDate));
        foreach (var employee in signOutEmployees)
        {
          var substitutions = substitutionsSource.Where(x => Equals(x.User, employee)).ToList();
          
          if (!substitutions.Any())
          {
            notAutomatedEmployeesWoSubstitution.Add(employee);
            continue;
          }
          
          var substitutors = substitutions.Select(x => x.Substitute).ToList();
          if (substitutors.Any(x => x.Login != null && x.Login.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed))
          {
            continue;
          }
          
          var substitutorsCount = substitutors.Count;
          while (substitutorsCount > 0)
          {
            substitutions = new List<Sungero.CoreEntities.ISubstitution>();
            
            foreach (var substitutor in substitutors)
            {
              substitutions.AddRange(substitutionsSource.Where(x => Equals(x.User, substitutor) && !Equals(x.Substitute, employee)));
            }
            if (!substitutions.Any())
            {
              notAutomatedEmployeesWoSubstitution.Add(employee);
              break;
            }
            
            substitutors = substitutions.Select(x => x.Substitute).ToList();
            substitutorsCount = substitutors.Count;
            if (substitutors.Any(x => x.Login != null && x.Login.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed))
            {
              notAutomatedEmployeesWoSubstitution.Add(employee);
              break;
            }
          }
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - EtalonDatabooks GetNotAutomatedEmployeesAvis - {0} {1} {2}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty, ex.StackTrace);
      }
      return notAutomatedEmployeesWoSubstitution.AsQueryable();
    }
    
    #endregion

    /// <summary>
    /// Получить для суммы значение прописью с указанной валютой.
    /// </summary>
    /// <param name="amount">Сумма.</param>
    /// <param name="currency">Валюта.</param>
    /// <returns>Сумма прописью с валютой.</returns>
    [Public]
    public static string AmountInCurrencyToWords(Nullable<double> amount, Sungero.Commons.ICurrency currency)
    {
      if (amount == null || currency == null)
        return null;
      
      var integerPart = (long)Math.Truncate(amount.Value);
      var fractionalPart = (int)Math.Round((amount.Value - integerPart) * 100);
      
      var currencyShortName = currency.ShortName.Trim();
      var currencyName = currencyShortName.EndsWith(".") ?
        currencyShortName :
        StringUtils.NumberDeclension(integerPart,
                                     currencyShortName,
                                     Sungero.Core.CaseConverter.ConvertCurrencyNameToTargetDeclension(currencyShortName, Sungero.Core.DeclensionCase.Genitive),
                                     Sungero.Core.CaseConverter.ConvertCurrencyNameToTargetDeclension(currencyShortName.Pluralize(), Sungero.Core.DeclensionCase.Genitive));
      
      var currencyFractionName = currency.FractionName.Trim();
      var currencyFractionalName = currencyFractionName.EndsWith(".") ?
        currencyFractionName :
        StringUtils.NumberDeclension(fractionalPart,
                                     currencyFractionName,
                                     Sungero.Core.CaseConverter.ConvertCurrencyNameToTargetDeclension(currencyFractionName, Sungero.Core.DeclensionCase.Genitive),
                                     Sungero.Core.CaseConverter.ConvertCurrencyNameToTargetDeclension(currencyFractionName.Pluralize(), Sungero.Core.DeclensionCase.Genitive));
      
      return string.Format("({0}) {1}, {2} {3}", StringUtils.NumberToWords(integerPart).Capitalize(), currencyName, StringUtils.NumberToWords(fractionalPart), currencyFractionalName);
    }
    
    /// <summary>
    /// Перевести дату в строковое представление
    /// </summary>
    /// <param name="date">Дата</param>
    /// <returns>Дата прописью</returns>
    [Public]
    public static string DateToWords(DateTime date)
    {
      if (date == null)
        return null;
      
      var days = new Dictionary<int, string>()
      {
        {1, "первое"},
        {2, "второе"},
        {3, "третье"},
        {4, "четвертое"},
        {5, "пятое"},
        {6, "шестое"},
        {7, "седьмое"},
        {8, "восьмое"},
        {9, "девятое"},
        {10, "десятое"},
        {11, "одиннадцатое"},
        {12, "двенадцатое"},
        {13, "тринадцатое"},
        {14, "четырнадцатое"},
        {15, "пятнадцатое"},
        {16, "шестнадцатое"},
        {17, "семнадцатое"},
        {18, "восемнадцатое"},
        {19, "девятнадцатое"},
        {20, "двадцатое"},
        {21, "двадцать первое"},
        {22, "двадцать второе"},
        {23, "двадцать третье"},
        {24, "двадцать четвертое"},
        {25, "двадцать пятое"},
        {26, "двадцать шестое"},
        {27, "двадцать седьмое"},
        {28, "двадцать восьмое"},
        {29, "двадцать девятое"},
        {30, "тридцатое"},
        {31, "тридцать первое"}
      };
      
      var months = new Dictionary<int, string>()
      {
        {1, "января"},
        {2, "февраля"},
        {3, "марта"},
        {4, "апреля"},
        {5, "мая"},
        {6, "июня"},
        {7, "июля"},
        {8, "августа"},
        {9, "сентября"},
        {10, "октября"},
        {11, "ноября"},
        {12, "декабря"}
      };
      
      var hundredYear = new Dictionary<int, string>()
      {
        {100, "сотого"},
        {200, "двухсотого"},
        {1, "сто"},
        {2, "двести"}
      };
      
      var tensYear = new Dictionary<int, string>()
      {
        {10, "десятого"},
        {11, "одиннадцатого"},
        {12, "двенадцатого"},
        {13, "тринадцатого"},
        {14, "четырнадцатого"},
        {15, "пятнадцатого"},
        {16, "шестнадцатого"},
        {17, "семнадцатого"},
        {18, "восемнадцатого"},
        {19, "девятнадцатого"},
        {20, "двадцатого"},
        {30, "тридцатого"},
        {40, "сорокового"},
        {50, "пятидесятого"},
        {60, "шестидесятого"},
        {70, "семидесятого"},
        {80, "восьмидесятого"},
        {90, "девяностого"},
        {2, "двадцать"},
        {3, "тридцать"},
        {4, "сорок"},
        {5, "пятьдесят"},
        {6, "шестьдесят"},
        {7, "семьдесят"},
        {8, "восемьдесят"},
        {9, "девяносто"}
      };
      
      var unitsYear = new Dictionary<int, string>()
      {
        {1, "первого"},
        {2, "второго"},
        {3, "третьего"},
        {4, "четвертого"},
        {5, "пятого"},
        {6, "шестого"},
        {7, "седьмого"},
        {8, "восьмого"},
        {9, "девятого"}
      };
      
      var stringYear = "две тысячи ";
      var year = date.Year - 2000;
      if (year >= 100)
      {
        if (year % 100 == 0)
        {
          var hundred = hundredYear[year];
          stringYear += hundred;
        }
        else
        {
          int firstNumber = year / 100;
          var hundred = hundredYear[firstNumber];
          stringYear += string.Concat(hundred, " ");
        }
      }
      year = year - ((int)year / 100 * 100);
      if (year >= 10)
      {
        if (year % 10 == 0 || year <= 19)
        {
          var ten = tensYear[year];
          stringYear += ten;
        }
        else
        {
          int firstNumber = year / 10;
          var ten = tensYear[firstNumber];
          stringYear += string.Concat(ten, " ");
        }
      }
      year = year - ((int)year / 10 * 10);
      if (year > 0)
      {
        var unit = unitsYear[year];
        stringYear += unit;
      }
      var stringDate = string.Format("{0} {1} {2} года", days[date.Day], months[date.Month], stringYear);
      return stringDate;
    }
    
    #region Настройки папок потока
    
    [Public]
    public IQueryable<Sungero.Workflow.IAssignmentBase> GetAssignmentsByFolderSettings(IQueryable<Sungero.Workflow.IAssignmentBase> query, Enumeration folderName)
    {
      var folderSettings = EtalonDatabooks.FlowFolderSettings.GetAll()
        .Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && x.FolderName == folderName).AsEnumerable();
      var assignmentBySettingIds = new List<long>();
      foreach(var setting in folderSettings)
      {
        var queryPart = query;
        // Фильтр по статусу задачи.
        if (setting.TaskStatus != null)
        {
          queryPart = queryPart.Where(a => a.Task.Status == setting.TaskStatus);
        }
        // Фильтр по теме задачи.
        if (setting.TaskSubject != null && setting.TaskSubject.Any())
        {
          var assignmentByTaskSubject = new List<long>();
          foreach(var taskSubject in setting.TaskSubject)
          {
            var assignmentsPart = queryPart;
            assignmentsPart = assignmentsPart.Where(a => a.Task.Subject.Contains(taskSubject.Subject));
            assignmentByTaskSubject.AddRange(assignmentsPart.Select(a => a.Id).ToList());
          }
          queryPart = queryPart.Where(a => assignmentByTaskSubject.Contains(a.Id));
        }
        
        var settingTaskGuid = setting.TaskType.TaskTypeGuid;
        
        #region Фильтр по типу задачи
        switch(settingTaskGuid)
        {
          case Constants.Module.FlowFoldersSetting.ApprovalTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.Docflow.ApprovalTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.CheckReturnTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.Docflow.CheckReturnTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.DocflowDeadlineExtensionTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.Docflow.DeadlineExtensionTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.FreeApprovalTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.Docflow.FreeApprovalTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.ExchangeDocumentProcessingTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.Exchange.ExchangeDocumentProcessingTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.ReceiptNotificationSendingTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.Exchange.ReceiptNotificationSendingTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.CounterpartyConflictProcessingTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.ExchangeCore.CounterpartyConflictProcessingTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.IncomingInvitationTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.ExchangeCore.IncomingInvitationTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.AcquaintanceTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.RecordManagement.AcquaintanceTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.ActionItemExecutionTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.RecordManagement.ActionItemExecutionTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.RecordManagmentDeadlineExtensionTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.RecordManagement.DeadlineExtensionTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.DocumentReviewTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.RecordManagement.DocumentReviewTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.StatusReportRequestTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.RecordManagement.StatusReportRequestTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.VerificationTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.SmartProcessing.VerificationTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.EditComponentRXRequestTaskGuid:
            {
              queryPart = queryPart.Where(a => lenspec.AutomatedSupportTickets.EditComponentRXRequestTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.SubstitutionRequestTaskGuid:
            {
              queryPart = queryPart.Where(a => lenspec.AutomatedSupportTickets.SubstitutionRequestTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.CreateCompanyTaskGuid:
            {
              queryPart = queryPart.Where(a => avis.EtalonParties.CreateCompanyTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.SimpleTaskGuid:
            {
              queryPart = queryPart.Where(a => Sungero.Workflow.SimpleTasks.Is(a.Task));
              break;
            }
          case Constants.Module.FlowFoldersSetting.ApprovalCounterpartyRegisterTaskGuid:
            {
              queryPart = queryPart.Where(a => lenspec.Tenders.ApprovalCounterpartyRegisterTasks.Is(a.Task));
              break;
            }
        }
        #endregion
        
        if (setting.AssignmentTypeByTask.Any())
        {
          #region Фильтр по типам, статусам и темам заданий
          var assignmentByTaskIds = new List<long>();
          foreach(var assignmentType in setting.AssignmentTypeByTask)
          {
            var assignmentsPart = queryPart;
            if (assignmentType.AssignmentType != null)
            {
              var settingAssignmentGuid = assignmentType.AssignmentType.AssignmentTypeGuid;
              switch(settingAssignmentGuid)
              {
                  #region Задача на согласование по регламенту
                case Constants.Module.FlowFoldersSetting.ApprovalAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalCheckingAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalCheckingAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalCheckReturnAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalCheckingAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalExecutionAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalExecutionAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalManagerAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalManagerAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalPrintingAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalPrintingAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalRegistrationAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalRegistrationAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalReviewAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalReviewAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalReworkAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalReworkAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalSendingAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalSendingAssignments.Is(a));
                    if (assignmentType.AssignmentDeliveryMethod != null)
                    {
                      assignmentsPart = assignmentsPart.Where(a => assignmentType.AssignmentDeliveryMethod.Equals(Sungero.Docflow.ApprovalSendingAssignments.As(a).DeliveryMethod));
                    }
                    else
                    {
                      assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalSendingAssignments.As(a).DeliveryMethod == null ||
                                                              !Sungero.Docflow.ApprovalSendingAssignments.As(a).DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) &&
                                                              !Sungero.Docflow.ApprovalSendingAssignments.As(a).DeliveryMethod.Name.Trim().Equals("Электронная почта"));
                    }
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalSigningAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalSigningAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalSimpleAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalSimpleAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalNotifications.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalSimpleNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.ApprovalSimpleNotifications.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Контроль возврата документа
                case Constants.Module.FlowFoldersSetting.CheckReturnAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.CheckReturnAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.CheckReturnCheckAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.CheckReturnCheckAssignments.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Запрос на продление срока
                case Constants.Module.FlowFoldersSetting.DeadlineExtensionAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.DeadlineExtensionAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.DeadlineRejectionAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.DeadlineRejectionAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.DeadlineExtensionNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.DeadlineExtensionNotifications.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Задача на свободное согласование
                case Constants.Module.FlowFoldersSetting.FreeApprovalAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.FreeApprovalAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.FreeApprovalFinishAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.FreeApprovalFinishAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.FreeApprovalReworkAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.FreeApprovalReworkAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.FreeApprovalNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Docflow.FreeApprovalNotifications.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Задача на обработку входящих документов эл. обмена
                case Constants.Module.FlowFoldersSetting.ExchangeDocumentProcessingAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Exchange.ExchangeDocumentProcessingAssignments.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Задача на отправку извещений о получении документов
                case Constants.Module.FlowFoldersSetting.ReceiptNotificationSendingAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Exchange.ReceiptNotificationSendingAssignments.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Задача на обработку конфликтов синхронизации контрагентов
                case Constants.Module.FlowFoldersSetting.CounterpartyConflictProcessingAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.ExchangeCore.CounterpartyConflictProcessingAssignments.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Задача на обработку приглашения к эл. обмену от контрагента
                case Constants.Module.FlowFoldersSetting.IncomingInvitationAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.ExchangeCore.IncomingInvitationAssignments.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Задача на ознакомление с документом
                case Constants.Module.FlowFoldersSetting.AcquaintanceAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.AcquaintanceAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.AcquaintanceFinishAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.AcquaintanceFinishAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.AcquaintanceCompleteNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.AcquaintanceCompleteNotifications.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Задача на исполнение поручения
                case Constants.Module.FlowFoldersSetting.ActionItemExecutionAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ActionItemExecutionAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ActionItemSupervisorAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ActionItemSupervisorAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ActionItemExecutionNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ActionItemExecutionNotifications.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ActionItemObserversNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ActionItemObserversNotifications.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ActionItemSupervisorNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ActionItemSupervisorNotifications.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Продление срока
                case Constants.Module.FlowFoldersSetting.RMDeadlineExtensionAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.DeadlineExtensionAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.RMDeadlineRejectionAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.DeadlineRejectionAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.RMDeadlineExtensionNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.DeadlineExtensionNotifications.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Задача на рассмотрение документа
                case Constants.Module.FlowFoldersSetting.PreparingDraftResolutionAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.PreparingDraftResolutionAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ReviewDraftResolutionAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ReviewDraftResolutionAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ReviewManagerAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ReviewManagerAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ReviewResolutionAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ReviewResolutionAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ReviewReworkAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ReviewReworkAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ReviewClerkNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ReviewClerkNotifications.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ReviewObserverNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ReviewObserverNotifications.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ReviewObserversNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ReviewObserversNotifications.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Запрос отчета по поручению
                case Constants.Module.FlowFoldersSetting.ReportRequestAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ReportRequestAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ReportRequestCheckAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.RecordManagement.ReportRequestCheckAssignments.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Задача на верификацию комплекта документов
                case Constants.Module.FlowFoldersSetting.VerificationAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.SmartProcessing.VerificationAssignments.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Заявка на изменение компонентов Directum RX
                case Constants.Module.FlowFoldersSetting.ApprovalAdministratorGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => lenspec.AutomatedSupportTickets.ApprovalAdministrators.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApprovalManagerGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => lenspec.AutomatedSupportTickets.ApprovalManagers.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Заявка на формирование замещения
                case Constants.Module.FlowFoldersSetting.ApprovalSubstitutionAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => lenspec.AutomatedSupportTickets.ApprovalSubstitutionAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.SubstitutionRequestNotificationGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => lenspec.AutomatedSupportTickets.SubstitutionRequestNotifications.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Заявка на редактирование справочника Контрагенты
                case Constants.Module.FlowFoldersSetting.ApproveCounterpartyAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => avis.EtalonParties.ApproveCounterpartyAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ApproveRevisionGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => avis.EtalonParties.ApproveRevisions.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Простая задача
                case Constants.Module.FlowFoldersSetting.SimpleAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => Sungero.Workflow.SimpleAssignments.Is(a));
                    break;
                  }
                  #endregion
                  
                  #region Задача на согласование включения в реестр квалифицированных контрагентов/список дисквалифицированных контрагентов
                case Constants.Module.FlowFoldersSetting.CommitteeApprovalAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => lenspec.Tenders.CommitteeApprovalAssignments.Is(a));
                    break;
                  }
                case Constants.Module.FlowFoldersSetting.ProcessingOfApprovalResultsAssignmentGuid:
                  {
                    assignmentsPart = assignmentsPart.Where(a => lenspec.Tenders.ProcessingOfApprovalResultsAssignments.Is(a));
                    break;
                  }
                  #endregion
              }
            }
            
            if (assignmentType.AssignmentStatus != null)
            {
              assignmentsPart = assignmentsPart.Where(a => a.Status == assignmentType.AssignmentStatus);
            }
            
            if (!string.IsNullOrEmpty(assignmentType.AssignmentSubject))
            {
              assignmentsPart = assignmentsPart.Where(a => a.Subject.Contains(assignmentType.AssignmentSubject));
            }
            
            assignmentByTaskIds.AddRange(assignmentsPart.Select(a => a.Id).ToList());
          }
          #endregion
          
          // Если был фильтр по заданиям, то оставить конечный набор, иначе отфильтрованный только по задаче.
          queryPart = queryPart.Where(a => assignmentByTaskIds.Contains(a.Id));
        }

        assignmentBySettingIds.AddRange(queryPart.Select(a => a.Id).ToList());
      }
      
      return query.Where(a => assignmentBySettingIds.Contains(a.Id));
    }
    
    #endregion
    
    /// <summary>
    /// Получить следующий номер, для нумерации в константе.
    /// </summary>
    /// <returns>Следующий номер, если не для нумерации или не найдето, то -1</returns>
    [Public]
    public int GetNextValueConstant(string code)
    {
      var constant = ConstantDatabooks.GetAll(c => c.Code == code).FirstOrDefault();
      if (constant == null || constant.IsNumeration == false)
        return -1;
      
      // Проверяем что значение число.
      int newValue;
      var success = int.TryParse(constant.Value, out newValue);
      if (success == false)
        return -1;
      
      // Изменяем значение на следующий номер и возвращаем его.
      newValue++;
      constant.Value = $"{newValue}";
      constant.Save();
      
      return newValue;
    }

    //TODO: закомментирована неиспользуемая функция
    /// <summary>
    /// Проверка вхождения текущего пользователя в произвольную роль.
    /// </summary>
    /// <param name="roleSid">Sid роли.</param>
    /// <returns> Функция возвращает значение True,
    /// если пользователь входит в роль, иначе возвращает False.</returns>
    //    public static bool IncludedInRole(Guid roleSid)
    //    {
    //      var role = Roles.GetAll(r => r.Sid == roleSid).SingleOrDefault();
    //      return role != null && Sungero.CoreEntities.Users.Current.IncludedIn(role);
    //    }

    /// <summary>
    /// Передать ресурс с инструкцией этапа.
    /// </summary>
    /// <param name="stage">Этап с инструкцией.</param>
    /// <returns>Ресурс.</returns>
    [Public]
    public string GetAssignmentInstruction(Sungero.Docflow.IApprovalStage stage)
    {
      var instruction = string.Empty;
      
      var etalonStage = Etalon.ApprovalStages.As(stage);
      if (etalonStage != null && etalonStage.Instructionlenspec != null)
      {
        instruction = etalonStage.Instructionlenspec.InstructionAvis ?? string.Empty;
      }
      return instruction;
    }
    
    /// <summary>
    /// Передать ресурс с инструкцией этапа.
    /// </summary>
    /// <param name="stage">Этап с инструкцией.</param>
    /// <returns>Ресурс.</returns>
    [Public]
    public StateView GetInstruction(Etalon.IApprovalStage stage)
    {
      // Текст инструкции длиной более 6 строк будет сворачиваться,
      // остальные строки доступны по кнопке "Показать еще".
      var instruction = StateView.Create();
      
      if (stage != null && stage.Instructionlenspec != null)
      {
        var block = instruction.AddBlock();
        var instructionText = stage.Instructionlenspec.InstructionAvis ?? string.Empty;
        block.AddLabel(instructionText, false);
      }
      
      return instruction;
    }
    
    /// <summary>
    /// Обращение к API сервиса Контур.Фокус для получения реквизитов ЕГРЮЛ по организации.
    /// </summary>
    /// <param name="tin">ИНН организации.</param>
    /// <returns>Наименование организации.</returns>
    [Remote]
    public string GetKonturFocusBaseRequisites(string key, string tin)
    {
      var connectionString = EtalonDatabooks.Constants.Module.KFBaseRequisitesConnectionString;
      var uri = string.Format(connectionString, tin, key);
      using (var client = new WebClient())
      {
        client.Headers.Add("Content-Type", "application/json");
        try
        {
          var responseString = client.DownloadString(uri);
          var token = Newtonsoft.Json.Linq.JToken.Parse(responseString);
          
          var tokenArray = token != null && token.Type == Newtonsoft.Json.Linq.JTokenType.Array
            ? token.ToObject<Newtonsoft.Json.Linq.JArray>()
            : null;
          if(tokenArray == null || tokenArray.Count == 0)
            return null;
          
          var jobject = tokenArray[0];
          var legalName = jobject != null
            ? jobject["UL"]?["legalName"]
            : null;
          if(legalName == null)
            return null;
          
          if(legalName["short"] != null)
            return legalName["short"].ToString();
          
          if(legalName["full"] != null)
            return legalName["full"].ToString();
          
          return null;
        }
        catch(WebException wex)
        {
          LoggerResponseStatusCode(wex);
          return null;
        }
      }
    }
    
    /// <summary>
    /// Обращение к API сервиса ФНС для отслеживания изменений в ЕГРЮЛ/ЕГРИП.
    /// </summary>
    /// <param name="tin">ИНН контрагента.</param>
    /// <param name="documentDate">Дата документа.</param>
    /// <returns>True, если выписка актуальна, иначе false.</returns>
    [Public, Remote]
    public bool CheckForCounterpartyChangesEGRUL(string tin, DateTime? documentDate)
    {
      var key = GetConnectionParams(Etalon.Module.Integration.PublicConstants.Module.APIFnsCode);
      if (string.IsNullOrEmpty(key))
        throw new ApplicationException("Не удалось выполнить запрос: укажите значение API-ключа для использования сервиса API ФНС.");
      
      var uri = string.Format(Constants.Module.APIFnsCounterpartyChangesURL, key, tin, documentDate.Value.ToString("yyyy-MM-dd"));
      using (var client = new WebClient())
      {
        client.Headers.Add("Content-Type", "application/json");
        try
        {
          var responseString = client.DownloadString(uri);
          var token = Newtonsoft.Json.Linq.JToken.Parse(responseString);
          var items = token != null ? token["items"] : null;
          
          var itemsArray = items != null && items.Type == Newtonsoft.Json.Linq.JTokenType.Array
            ? items.ToObject<Newtonsoft.Json.Linq.JArray>()
            : null;
          if (itemsArray == null || itemsArray.Count == 0)
            throw new ApplicationException("Организация/Банк с таким ИНН не найдена.");
          
          var jobject = itemsArray[0];
          var changes = jobject != null ? jobject["ЮЛ"]?["Изменения"] : null;
          if (changes != null && changes.Type == Newtonsoft.Json.Linq.JTokenType.Object)
            return false;
          
          var changesArray = changes != null && changes.Type == Newtonsoft.Json.Linq.JTokenType.Array
            ? changes.ToObject<Newtonsoft.Json.Linq.JArray>()
            : null;
          if(changesArray == null || changesArray.Count == 0)
            return true;
        }
        catch(WebException wex)
        {
          throw new ApplicationException(LoggerResponseStatusCode(wex));
        }
      }
      return false;
    }
    
    /// <summary>
    /// Обращение к API сервиса ФНС для получения выписки ЕГРЮЛ по организации.
    /// </summary>
    /// <param name="tin">ИНН контрагента.</param>
    /// <param name="document">Документ для создания версии.</param>
    /// <returns>Сообщение об ошибке при наличии.</returns>
    /// <remarks>Выписка записывается в новую версию документа</remarks>
    [Public, Remote]
    public string GetExcerptEGRUL(string tin, Sungero.Docflow.IOfficialDocument document)
    {
      var key = GetConnectionParams(Etalon.Module.Integration.PublicConstants.Module.APIFnsCode);
      if (string.IsNullOrEmpty(key))
      {
        return "Не удалось выполнить запрос: укажите значение API-ключа для использования сервиса API ФНС.";
      }
      
      var uri = string.Format(Constants.Module.APIFnsExcerptEGRULURL, key, tin);
      using (var client = new WebClient())
      {
        client.Headers.Add("Content-Type", "application/json");
        try
        {
          var responseData = client.DownloadData(uri);
          if (responseData != null)
          {
            using(var stream = new System.IO.MemoryStream(responseData))
            {
              document.CreateVersionFrom(stream, "pdf");
            }
          }
        }
        catch(WebException wex)
        {
          return LoggerResponseStatusCode(wex);
        }
      }
      return null;
    }
    
    private string LoggerResponseStatusCode(WebException wex)
    {
      var errorMessage = string.Empty;
      using (var response = wex.Response as HttpWebResponse)
      {
        if(response == null)
        {
          errorMessage = wex.Message;
        }
        else
        {
          switch(response.StatusCode)
          {
            case HttpStatusCode.BadRequest:
              errorMessage = "ИНН, ИННФЛ или ОГРН не указаны или указаны не по формату.";
              break;
            case HttpStatusCode.Forbidden:
              errorMessage = "доступ запрещен. Возможные причины: в запросе указан неверно ключ или нет доступа к используемому методу; закончился лимит запросов по используемому методу; истек срок действия ключа; достигнут дневной лимит запросов.";
              break;
            case HttpStatusCode.NotFound:
              errorMessage = "выписка не найдена по указанной организации.";
              break;
          }
        }
      }
      Logger.ErrorFormat("LoggerResponseStatusCode - API request failed: {0}", wex.Message);
      return string.Format("Не удалось выполнить запрос: {0}", errorMessage);
    }
    
    /// <summary>
    ///  Получить параметры подключения к API.
    /// </summary>
    /// <returns>Строка подключения.</returns>
    [Public, Remote]
    public string GetConnectionParams(string settingCode)
    {
      var integrationsRecord = avis.Integration.Integrationses
        .GetAll(r => r.Code.Equals(settingCode))
        .Single();
      if (!string.IsNullOrEmpty(integrationsRecord.ConnectionParams))
        return Encryption.Decrypt(integrationsRecord.ConnectionParams);
      else
        return string.Empty;
    }
    //конец Добавлено Avis Expert
  }
}