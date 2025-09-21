using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalSigningAssignment;

namespace lenspec.Etalon.Server
{
  partial class ApprovalSigningAssignmentFunctions
  {

    /// <summary>
    /// Признак, определяющий нужно ли подписывать документы из группы Приложения.
    /// </summary>
    /// <returns>True, если нужно очистить список подписываемых приложений, иначе false.</returns>
    [Remote]
    public bool CheckTheTaskStage()
    {
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var currentStage = Functions.ApprovalTask.GetStage(task, Sungero.Docflow.ApprovalStage.StageType.Sign);
      if (currentStage != null)
      {
        var stage = Etalon.ApprovalStages.As(currentStage.Stage);
        if (stage != null)
        {
          return stage.DoNotSignApplicationslenspec.HasValue && stage.DoNotSignApplicationslenspec.Value == true;
        }
      }
      return false;
    }
  }
}