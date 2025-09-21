using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using lenspec.Etalon.ApprovalTask;

namespace lenspec.Etalon.Server
{
  partial class ApprovalTaskRouteHandlers
  {

    //Добавлено Avis Expert
    public override void CompleteAssignment5(Sungero.Docflow.IApprovalReworkAssignment assignment, Sungero.Docflow.Server.ApprovalReworkAssignmentArguments e)
    {
      base.CompleteAssignment5(assignment, e);
      
      var document  = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        _obj.DocumentIsSignedlenspec = document.InternalApprovalState == Sungero.Docflow.OfficialDocument.InternalApprovalState.Signed;
        if (Sungero.Docflow.FormalizedPowerOfAttorneys.Is(document))
        {
          var fpoa = Sungero.Docflow.FormalizedPowerOfAttorneys.As(document);
          if (fpoa.FtsListState == Sungero.Docflow.FormalizedPowerOfAttorney.FtsListState.Registered)
            _obj.DocumentFtsListStatelenspec = Etalon.ApprovalTask.DocumentFtsListStatelenspec.Registered;
          
          if (fpoa.FtsListState == Sungero.Docflow.FormalizedPowerOfAttorney.FtsListState.Rejected)
            _obj.DocumentFtsListStatelenspec = Etalon.ApprovalTask.DocumentFtsListStatelenspec.Rejected;
        }
        _obj.Save();
      }
    }

    public override void EndBlock33(Sungero.Docflow.Server.ApprovalSimpleNotificationEndBlockEventArguments e)
    {
      base.EndBlock33(e);
    }

    // Задание.
    public override void CompleteAssignment30(Sungero.Docflow.IApprovalSimpleAssignment assignment, Sungero.Docflow.Server.ApprovalSimpleAssignmentArguments e)
    {
      base.CompleteAssignment30(assignment, e);
      
      // Завершить, если в этапе согласования поле Старт не равно "Одновременно" и не установлен признак "После первого выполнения с прерыванием".
      var ruleStage = Functions.ApprovalTask.GetStage(_obj, Sungero.Docflow.ApprovalStage.StageType.SimpleAgr);
      if (ruleStage == null || ruleStage.Stage.Sequence != Sungero.Docflow.ApprovalStage.Sequence.Parallel
          || Etalon.ApprovalStages.As(ruleStage.Stage).WithInterruptionlenspec != true)
        return;
      
      var parallelAssignments = Functions.ApprovalTask.GetParallelAssignments(_obj, assignment);
      var activeParallelAssignments = parallelAssignments
        .Where(a => a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
        .Where(a => !Equals(a, assignment))
        .Where(a => Sungero.Docflow.ApprovalSimpleAssignments.Is(a));
      
      foreach (var parallelAssignment in activeParallelAssignments)
      {
        if (!string.IsNullOrEmpty(parallelAssignment.ActiveText))
          parallelAssignment.ActiveText += Environment.NewLine;
        
        if (assignment.CompletedBy != null && !assignment.CompletedBy.Equals(assignment.Performer))
          parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.DoneAnotherUserWithOnBehalfOfFormat(assignment.CompletedBy.Name,
                                                                                                                      assignment.Performer.Name);
        else
          parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.DoneAnotherUserFormat(assignment.Performer.Name);

        parallelAssignment.Abort();
      }
    }

    // Согласование.
    public override void CompleteAssignment6(Sungero.Docflow.IApprovalAssignment assignment, Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      base.CompleteAssignment6(assignment, e);
      // Завершить, если в этапе согласования поле Старт не равно "Одновременно" и не установлен признак "После первого выполнения с прерыванием".
      var stage = Etalon.ApprovalStages.As(assignment.Stage);
      if (stage.Sequence != Sungero.Docflow.ApprovalStage.Sequence.Parallel || stage.WithInterruptionlenspec != true)
        return;
      
      var parallelAssignments = Functions.ApprovalTask.GetParallelAssignments(_obj, assignment);
      var activeParallelAssignments = parallelAssignments
        .Where(a => a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess)
        .Where(a => !Equals(a, assignment))
        .Where(a => Sungero.Docflow.ApprovalAssignments.Is(a));
      
      foreach (var parallelAssignment in activeParallelAssignments)
      {
        if (!string.IsNullOrEmpty(parallelAssignment.ActiveText))
          parallelAssignment.ActiveText += Environment.NewLine;
        
        if (assignment.CompletedBy != null && !assignment.CompletedBy.Equals(assignment.Performer))
          parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserWithOnBehalfOfFormat(assignment.CompletedBy.Name,
                                                                                                                          assignment.Performer.Name);
        else
          parallelAssignment.ActiveText += lenspec.Etalon.ApprovalTasks.Resources.ApprovedAnotherUserFormat(assignment.Performer.Name);

        parallelAssignment.Abort();
      }
    }
    //конец Добавлено Avis Expert

  }
}