using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.EditComponentRXRequestTask;

namespace lenspec.AutomatedSupportTickets.Server
{
  partial class EditComponentRXRequestTaskFunctions
  {
    /// <summary>
    /// Формируем описание для заявки с типом Изменение документа.
    /// </summary>
    /// <returns>Описание для задачи.</returns>
    [Public]
    public void GetEditDocumentDiscription(Sungero.Core.StateBlock block)
    {
      // Формируем список сотрудников через запятую.
      var employeesNames = GetEmployeesNames();
      // Формируем список наших организаций через запятую.
      var businessUnitsName = GetBusinessUnitsNames();
      
      // Формируем список документов через запятую.
      var documentTypesNames = string.Empty;
      foreach (var documentType in _obj.CollectionDocumentTypes)
      {
        if (string.IsNullOrEmpty(documentTypesNames))
        {
          documentTypesNames = documentType.DocumentType.Name;
          continue;
        }
        
        documentTypesNames += $", {documentType.DocumentType.Name}";
      }
      
      block.AddLabel($"№ заявки - {_obj.Number}");
      block.AddLineBreak();
      block.AddLabel($"Тип заявки - {_obj.Info.Properties.TypeRequest.GetLocalizedValue(_obj.TypeRequest)}");
      block.AddLineBreak();
      block.AddLabel($"ПРАВА ДОСТУПА");
      block.AddLineBreak();
      block.AddLabel($"Сотрудник для которого запрашиваются права: {employeesNames}");
      block.AddLineBreak();
      block.AddLabel($"Виды документов на которые запрашиваются права: {documentTypesNames}");
      block.AddLineBreak();     
      block.AddLabel($"Для организации: {businessUnitsName}");
      block.AddLineBreak();
      block.AddLineBreak();
      block.AddLabel($"Описание прав и обоснование получения доступа:");
      block.AddLineBreak();
      block.AddLabel($"{_obj.DescriptionPermit}");
    }
    
    /// <summary>
    /// Формируем описание для заявки с типом Изменение процессов.
    /// </summary>
    /// <returns>Описание для задачи.</returns>
    [Public]
    public void GetEditProcessDiscription(Sungero.Core.StateBlock block)
    {
      // Формируем список наших организаций через запятую.
      var businessUnitsName = GetBusinessUnitsNames();
      
      block.AddLabel($"№ заявки - {_obj.Number}");
      block.AddLineBreak();
      block.AddLabel($"Тип заявки - { _obj.Info.Properties.TypeRequest.GetLocalizedValue(_obj.TypeRequest)}");
      block.AddLineBreak();
      block.AddLabel($"ОБЪЕКТЫ СИСТЕМЫ ДЛЯ ИЗМЕНЕНИЯ");
      block.AddLineBreak();
      block.AddLabel($"Объект для изменения - {_obj.Info.Properties.ObjectEdit.GetLocalizedValue(_obj.ObjectEdit)}");
      block.AddLineBreak();
      block.AddLabel($"Процесс для доработки - {_obj.ProcessEdit}");
      block.AddLineBreak();
      block.AddLabel($"Наша организация - {businessUnitsName}");
      block.AddLineBreak();
      block.AddLabel($"Пояснение инициатора:");
      block.AddLineBreak();
      block.AddLabel($"{_obj.Comment}");
    }
    
    /// <summary>
    /// Формируем описание для заявки с типом Изменение процессов.
    /// </summary>
    /// <returns>Описание для задачи.</returns>
    [Public]
    public void GetOtherDiscription(Sungero.Core.StateBlock block)
    { 
      // Формируем список наших организаций через запятую.
      var businessUnitsName = GetBusinessUnitsNames();
      
      block.AddLabel($"№ заявки - {_obj.Number}");
      block.AddLineBreak();
      block.AddLabel($"Тип заявки - {_obj.Info.Properties.TypeRequest.GetLocalizedValue(_obj.TypeRequest)}");
      block.AddLineBreak();
      block.AddLabel($"Наша организация - {businessUnitsName}");
      block.AddLineBreak();
      block.AddLabel($"Описание заявки:");
      block.AddLineBreak();
      block.AddLabel($"{_obj.Description}");
    }
    
    /// <summary>
    /// Формируем список наших организаций через запятую.
    /// </summary>
    /// <returns></returns>
    private string GetBusinessUnitsNames()
    {
      var result = new StringBuilder();
      
      foreach (var businessUnit in _obj.CollectionBussinessUnits)
      {
        if (result.Length == 0)
          result.Append(businessUnit.BusinessUnit.Name);
        else
          result.Append($", {businessUnit.BusinessUnit.Name}");
      }
      
      return result.ToString();
    }
    
    /// <summary>
    /// Формируем список сотрудников через запятую.
    /// </summary>
    /// <returns></returns>
    private string GetEmployeesNames()
    {
      var result = new StringBuilder();
      
      foreach (var employee in _obj.CollectionEmployees)
      {
        if (result.Length == 0)
          result.Append(employee.Employee.Name);
        else
          result.Append($", {employee.Employee.Name}");
      }
      
      return result.ToString();
    }
  }
}