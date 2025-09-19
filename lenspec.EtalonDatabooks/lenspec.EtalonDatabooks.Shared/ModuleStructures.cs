using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.EtalonDatabooks.Structures.Module
{
  
  [Public(Isolated=true)]
  partial class XlsxFile
  {
    public Dictionary<string, lenspec.EtalonDatabooks.Structures.Module.IXlsxSheet> Sheets { get; set; }
  }
  
  [Public(Isolated=true)]
  partial class XlsxSheet
  {
    public lenspec.EtalonDatabooks.Structures.Module.IXlsxRow Headers { get; set; }
    
    public lenspec.EtalonDatabooks.Structures.Module.IXlsxTable Table { get; set; }
  }
  
  [Public(Isolated=true)]
  partial class XlsxTable
  {    
    public List<lenspec.EtalonDatabooks.Structures.Module.IXlsxRow> Rows { get; set; }
  }
  
  [Public(Isolated=true)]
  partial class XlsxRow
  {
    public string[] Data { get; set; }
  }
  
  /// <summary>
  /// Структура ответа API-сервиса.
  /// </summary>
  [Public]
  partial class APIResponse
  {
    /// <summary>
    /// Признак наличия ошибки в ответе.
    /// </summary>
    public bool Error { get; set; }
    
    /// <summary>
    /// Сообщение об ошибке, либо информационное.
    /// </summary>
    public string Message { get; set; }
  }

}