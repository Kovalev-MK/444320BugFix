using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sungero.Core;
using lenspec.EtalonDatabooks.Structures.Module;

namespace lenspec.EtalonDatabooks.Isolated.ExcelParseArea
{
  public class IsolatedFunctions
  {
    /// <summary>
    /// Получить значения в ячейках Excel-файла.
    /// </summary>
    /// <param name="stream">Excel-файл.</param>
    /// <returns>Данные из файла.</returns>
    [Public]
    public lenspec.EtalonDatabooks.Structures.Module.IXlsxFile GetCellsData(System.IO.Stream stream, bool hasHeader = true)
    {
      stream.Position = 0;
      
      var xlsxFile = new XlsxFile { Sheets = new Dictionary<string, IXlsxSheet>() };
      
      try
      {
        using (var workbook = new Aspose.Cells.Workbook(stream))
        {
          foreach (var worksheet in  workbook.Worksheets)
          {
            var cells = worksheet.Cells;
            
            var rowsCount = cells.MaxDataRow - cells.MinDataRow + 1;
            var colsCount = cells.MaxDataColumn - cells.MinDataColumn + 1;

            var xlsxSheet = new XlsxSheet { Table = new XlsxTable { Rows = new List<IXlsxRow>() } };
            var headers = new XlsxRow { Data = new string[colsCount] };
            var startIndex = 0;
            
            if (hasHeader && rowsCount > 0)
            {
              for (var columnIndex = 0; columnIndex < headers.Data.Length; columnIndex++)
                headers.Data[columnIndex] = cells[0, columnIndex].StringValue;

              startIndex++;
            }
            else
            {
              headers.Data = Enumerable.Range(1, colsCount)
                .Select(i => $"Column{i}")
                .ToArray();
            }
            
            xlsxSheet.Headers = headers;
            
            for (var rowIndex = startIndex; rowIndex < rowsCount; rowIndex++)
            {
              var row = new XlsxRow { Data = new string[colsCount] };

              for (var columnIndex = 0; columnIndex < row.Data.Length; columnIndex++)
                row.Data[columnIndex] = cells[rowIndex, columnIndex].StringValue;

              xlsxSheet.Table.Rows.Add(row);
            }
            
            xlsxFile.Sheets.Add(worksheet.Name, xlsxSheet);
          }
        }
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Не удалось получить данные из xlsx-файла: {0}", ex.Message), ex);
      }
      
      return xlsxFile;
    }

    [Public]
    public System.IO.Stream InsertDataToExcel(System.IO.Stream stream, lenspec.EtalonDatabooks.Structures.Module.IXlsxTable importResult, bool hasHeader = true, string sheetName = "")
    {
      var rowsResult = importResult.Rows;
      
      try
      {
        using (var workbook = new Aspose.Cells.Workbook(stream))
        {
          Aspose.Cells.Worksheet worksheet;
          
          if (string.IsNullOrEmpty(sheetName))
            worksheet = workbook.Worksheets[0];
          else
            worksheet = workbook.Worksheets.FirstOrDefault(x => x.Name == sheetName);
          
          if (worksheet == null)
            return null;
          
          var columnCount = worksheet.Cells.MaxDataColumn;
          var startIndex = worksheet.Cells.MinDataRow;
          var personResultColumnIndex = columnCount + 1;
          var objectResultColumnIndex = columnCount + 2;
          
          if (hasHeader)
          {
            var style = worksheet.Cells[startIndex, startIndex].GetStyle();
            worksheet.Cells[startIndex, personResultColumnIndex].PutValue("Результат импорта Собственника");
            worksheet.Cells[startIndex, personResultColumnIndex].SetStyle(style);
            worksheet.Cells[startIndex, objectResultColumnIndex].PutValue("Результат импорта Помещения");
            worksheet.Cells[startIndex, objectResultColumnIndex].SetStyle(style);
            startIndex++;
          }
          
          for (int i = 0, j = startIndex; i < rowsResult.Count(); i++, j++)
          {
            worksheet.Cells[j, columnCount + 1].PutValue(rowsResult[i].Data[0]);
            worksheet.Cells[j, columnCount + 2].PutValue(rowsResult[i].Data[1]);
          }
          
          stream.SetLength(0);
          workbook.Save(stream, Aspose.Cells.SaveFormat.Xlsx);
          stream.Seek(0, System.IO.SeekOrigin.Begin);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Avis - InsertDataToExcel - ", ex);
        throw new Exception(string.Format("Не удалось записать данные в xlsx-файл: {0}", ex.Message), ex);
      }
      
      return stream;
    }
  }
  
}