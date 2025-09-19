using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalManagerAssignment;

namespace lenspec.Etalon.Server
{
  partial class ApprovalManagerAssignmentFunctions
  {

    /// <summary>
    /// Установить разрешение на редактирование поля 'На подпись'
    /// </summary>
    [Remote(IsPure = true)]
    public bool CheckIsPossibilityChangingSigner()
    {
      var isAllowChangeSignatory = true;
      
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var stageLiteModel = Functions.ApprovalTask.GetStage(task, lenspec.Etalon.ApprovalStage.StageType.Manager);
      if (stageLiteModel != null)
      {
        var stage = lenspec.Etalon.ApprovalStages.As(stageLiteModel.Stage);
        if (stage != null)
          isAllowChangeSignatory = stage.IsProhibitChangeSigneravis != true;
      }
      return isAllowChangeSignatory;
    }
    
    /// <summary>
    /// Проверка возможности продления срока
    /// </summary>
    /// <returns>true - возможно продление</returns>
    [Remote(IsPure = true)]
    public bool CheckPossibilityExtending()
    {
      var task = Sungero.Docflow.ApprovalTasks.As(_obj.Task);
      var stageLiteModel = Functions.ApprovalTask.GetStage(task, lenspec.Etalon.ApprovalStage.StageType.Manager);
      var stage = lenspec.Etalon.ApprovalStages.As(stageLiteModel.Stage);
      return stage.IsProhibitExtensionOfTimeavis != true;
    }
  }
}