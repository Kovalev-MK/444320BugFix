using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalSendingAssignment;

namespace lenspec.Etalon.Server
{
  partial class ApprovalSendingAssignmentFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Вернуть признак необходимости отображать кнопку действия Массовой отправки исх. писем
    /// </summary>
    /// <returns>True, если нужно отобразить кнопку, false - иначе.</returns>
    [Remote(IsPure = true)]
    public bool NeedShowMassSendingOutgoingLetters()
    {
      var currentStage = Functions.ApprovalTask.GetStage(Sungero.Docflow.ApprovalTasks.As(_obj.Task), Sungero.Docflow.ApprovalStage.StageType.Sending);
      if (currentStage != null)
      {
        var stage = Etalon.ApprovalStages.As(currentStage.Stage);
        return stage != null && stage.BulkEmaillenspec.HasValue && stage.BulkEmaillenspec.Value == true;
      }
      else
      {
        return false;
      }
    }
    //конец Добавлено Avis Expert
  }
}