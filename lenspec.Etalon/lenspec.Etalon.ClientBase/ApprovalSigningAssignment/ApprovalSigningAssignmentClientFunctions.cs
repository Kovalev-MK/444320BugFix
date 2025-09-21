using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalSigningAssignment;
using Sungero.Docflow.ApprovalSigningAssignment;

namespace lenspec.Etalon.Client
{
  partial class ApprovalSigningAssignmentFunctions
  {
    /// <summary>
    /// Подписать документ.
    /// </summary>
    /// <param name="needStrongSign">Требуется квалифицированная электронная подпись.</param>
    /// <param name="eventArgs">Аргумент обработчика вызова.</param>
    public override void ApproveDocument(bool needStrongSign, Sungero.Domain.Client.ExecuteActionArgs eventArgs)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.Single();
      var addenda = _obj.AddendaGroup.OfficialDocuments.ToList();
      //добавить условие на чекбокс
      var checkStage = Functions.ApprovalSigningAssignment.Remote.CheckTheTaskStage(_obj);
      if (checkStage == true)
      {
        addenda.Clear();
      }
      var performer = Sungero.Company.Employees.As(_obj.Performer);
      var comment = string.IsNullOrWhiteSpace(_obj.ActiveText) ? string.Empty : _obj.ActiveText;
      
      lenspec.Etalon.Module.Docflow.Functions.Module.ApproveDocument(document, addenda, performer, needStrongSign, comment, eventArgs);
    }
  }
}