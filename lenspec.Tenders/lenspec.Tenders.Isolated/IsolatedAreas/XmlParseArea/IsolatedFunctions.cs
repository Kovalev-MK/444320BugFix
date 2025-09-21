using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sungero.Core;
using lenspec.Tenders.Structures.Module;

namespace lenspec.Tenders.Isolated.XmlParseArea
{
  public class IsolatedFunctions
  {

    /// <summary>
    /// Получить значения в ячейках XML-файла.
    /// </summary>
    /// <param name="stream">XML.</param>
    /// <param name="cells">Список ячеек.</param>
    /// <returns>Словарь "Ячейка-Значение".</returns>
    [Public]
    public Dictionary<string, string> GetCellsData(System.IO.Stream stream, List<string> cells)
    {
      stream.Position = 0;
      // Загрузить файл Excel.
      var workbook = new Aspose.Cells.Workbook(stream);
      // Получить все рабочие листы.
      var worksheetCollection = workbook.Worksheets;
      // Получить первый рабочий лист по индексу.
      var worksheet = worksheetCollection[0];
      
      var result = new Dictionary<string, string>();
      // Записать значения из ячеек в словарь.
      foreach (var cellName in cells)
      {
        var cellValue = worksheet.Cells[cellName].Value;
        var cellValueString = cellValue == null ?
          string.Empty :
          cellValue.ToString();
        result.Add(cellName, cellValueString);
      }
      
      return result;
    }
    
  }
}