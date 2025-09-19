using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalRegistrationAssignment;

namespace lenspec.Etalon.Server
{
  partial class ApprovalRegistrationAssignmentFunctions
  {
    
    //Добавлено Avis Expert

    /// <summary>
    /// Проверить резервирование основного документа.
    /// </summary>
    /// <param name="document">Согласуемый документ.</param>
    /// <returns>Сообщение об ошибке, если документ не зарезервирован, иначе пустая строка.</returns>
    [Remote]
    public string CheckReservationDocument(Sungero.Docflow.IOfficialDocument document)
    {
      var result = string.Empty;
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var currentStage = Functions.ApprovalTask.GetStage(task, Sungero.Docflow.ApprovalStage.StageType.SimpleAgr);
      if (currentStage != null)
      {
        var stage = Etalon.ApprovalStages.As(currentStage.Stage);
        // Автоматическое обновление полей в теле документа.
        if (stage != null && stage.CheckReservationlenspec.HasValue && stage.CheckReservationlenspec.Value == true &&
            document.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Reserved &&
            document.RegistrationState != Sungero.Docflow.OfficialDocument.RegistrationState.Registered)
        {
          result = lenspec.Etalon.ApprovalSimpleAssignments.Resources.NeedMainDocumentReservation;
        }
      }
      return result;
    }
    
    /// <summary>
    /// Автоматически обновить поля в теле документа.
    /// </summary>
    /// <returns>Сообщение об ошибке, если не удалось обновить поля в теле документа, иначе пустая строка.</returns>
    [Remote]
    public string UpdateDocumentBody()
    {
      var result = string.Empty;
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var currentStage = Functions.ApprovalTask.GetStage(task, Sungero.Docflow.ApprovalStage.StageType.Register);
      if (currentStage != null)
      {
        var stage = Etalon.ApprovalStages.As(currentStage.Stage);
        if (stage != null)
        {
          var updateTemplateBeforeExecute = stage.UpdateTemplateBeforeExecuteavis.HasValue && stage.UpdateTemplateBeforeExecuteavis.Value == true;
          var updateTemplateNumberAndDateBeforeExecute = stage.UpdateTemplateNumberAndDateBeforeExecuteavis.HasValue && stage.UpdateTemplateNumberAndDateBeforeExecuteavis.Value == true;
          if (updateTemplateBeforeExecute || updateTemplateNumberAndDateBeforeExecute)
          {
            _obj.Save();
            result = Etalon.PublicFunctions.ApprovalTask.UpdateDocumentBody(task, updateTemplateBeforeExecute, updateTemplateNumberAndDateBeforeExecute);
          }
        }
      }
      return result;
    }
    //конец Добавлено Avis Expert
  }
}