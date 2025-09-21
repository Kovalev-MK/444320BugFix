using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets.Server
{
  public class ModuleAsyncHandlers
  {

    /// <summary>
    /// Автоматическое выполнение заданий членами КО.
    /// </summary>
    /// <param name="args"></param>
    public virtual void CompleteCollegialBodiesMembersTasks(lenspec.AutomatedSupportTickets.Server.AsyncHandlerInvokeArgs.CompleteCollegialBodiesMembersTasksInvokeArgs args)
    {
      //Подготовим переменные
      var employeeId                  = args.EmployeeId;
      var date                        = args.ClosingDate;
      var employee                    = Sungero.Company.Employees.GetAll(x => x.Id == employeeId).FirstOrDefault();
      var groupId                     = Sungero.Docflow.PublicConstants.Module.TaskMainGroup.ApprovalTask;
      var protocolCollegialBodyTypes  = lenspec.ProtocolsCollegialBodies.PublicConstants.Module.ProtocolCollegialBodyTypes;
      
      //Найдем задачи которые подходят
      var assignment          = Sungero.Docflow.ApprovalAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee) && x.Deadline < date);
      assignment              = assignment.Where(x => x.Task.AttachmentDetails.Any(a => a.GroupId == groupId));
      assignment              = assignment.Where(x => x.Task.AttachmentDetails.Any(a => a.AttachmentTypeGuid == protocolCollegialBodyTypes));
      assignment              = assignment.Where(x => x.Stage.ApprovalRoles.Any(y => y.ApprovalRole.Type == lenspec.ProtocolsCollegialBodies.CompRoleCollegialBody.Type.Members));
      
      //Перебираем задачи и закрываем
      foreach(var assign in assignment)
      {
        var doc = ProtocolsCollegialBodies.ProtocolCollegialBodies.As(assign.DocumentGroup.OfficialDocuments.FirstOrDefault());
        //В карточке протокола проверяет, что у записи из поля «Коллегиальный орган» чекбокс «Автоматическое согласование по истечении срока» = «Да»
        if(doc.CollegialBody.IsAutomaticApproval == true)
        {
          try
          {
            assign.ActiveText = lenspec.AutomatedSupportTickets.Resources.TextAssignment;
            assign.Complete(Sungero.Docflow.ApprovalAssignment.Result.Approved);
          }
          catch(Exception ex)
          {
            Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания на согласование с ИД {0}", ex, assign.Id);
          }
        }
      }
    }

    /// <summary>
    /// Отключение учетных записей.
    /// </summary>
    /// <param name="args"></param>
    public virtual void DisablingAccounts(lenspec.AutomatedSupportTickets.Server.AsyncHandlerInvokeArgs.DisablingAccountsInvokeArgs args)
    {
      try
      {
        Logger.DebugFormat("TryDisablingAccounts: отключение УЗ сотрудника {0}", args.employeeId);
        var userId = args.userId;
        var employeeId = args.employeeId;
        var textTasks = args.textTasks;
        var taskAbort = args.taskAbort;
        var textAssignment = args.textAssignment;
        var assignmentAbort = args.assignmentAbort;
        var substitutionsAbort = args.substitutionsAbort;
        var employee = Sungero.Company.Employees.GetAll(x => x.Id == employeeId).FirstOrDefault();
        var performer = Users.GetAll(x => x.Id == userId).SingleOrDefault();
        //Создадим список заданий, которые не завершились
        var result = new List<Sungero.Workflow.IAssignmentBase>();
        
        #region Прекращение задач
        try
        {
          if (taskAbort == "Да")
          {
            
            #region Прекращение задач по регламенту
            try
            {
              var approvalTasks = Sungero.Docflow.ApprovalTasks.GetAll(x => x.Status == Sungero.Workflow.Task.Status.InProcess && x.Author.Equals(employee));
              if (approvalTasks != null)
              {
                foreach (var task in approvalTasks)
                {
                  try
                  {
                    task.ActiveText = textTasks;
                    task.Abort();
                  }
                  catch(Exception ex)
                  {
                    Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении Задачи на согласование по регламенту с ИД {0}", ex, task.Id);
                  }
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении Задач на согласование по регламенту.", ex);
            }
            #endregion
            
            #region Прекращение Задач на исполнение поручения
            try
            {
              var actionItemExecutionTask = Sungero.RecordManagement.ActionItemExecutionTasks.GetAll(x => x.Status == Sungero.Workflow.Task.Status.InProcess && x.Author.Equals(employee));
              if (actionItemExecutionTask != null)
              {
                foreach (var task in actionItemExecutionTask)
                {
                  try
                  {
                    //Прекращение всех подчиненных поручений.
                    var mainTasks = Sungero.RecordManagement.ActionItemExecutionTasks.GetAll(x => x.ParentTask != null && x.ParentTask.Id == task.Id);
                    foreach (var subtask in mainTasks)
                    {
                      try
                      {
                        subtask.Abort();
                      }
                      catch(Exception ex)
                      {
                        Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении подчиненного поручения с ИД {0}", ex, subtask.Id);
                      }
                    }
                    task.ActiveText = textTasks;
                    task.Abort();
                  }
                  catch(Exception ex)
                  {
                    Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задачи на исполнение поручения с ИД {0}", ex, task.Id);
                  }
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении Задач на исполнение поручения.", ex);
            }
            #endregion
            
            #region Прекращение Задач на ознакомление с документом
            try
            {
              var acquaintanceTask = Sungero.RecordManagement.AcquaintanceTasks.GetAll(x => x.Status == Sungero.Workflow.Task.Status.InProcess && x.Author.Equals(employee));
              if (acquaintanceTask != null)
              {
                foreach (var task in acquaintanceTask)
                {
                  try
                  {
                    task.ActiveText = textTasks;
                    task.Abort();
                  }
                  catch(Exception ex)
                  {
                    Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задачи на ознакомление с документом с ИД {0}", ex, task.Id);
                  }
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении Задач на ознакомление с документом.", ex);
            }
            #endregion
            
            #region Прекращение всех оставшихся задач
            try
            {
              var allTasks = Sungero.Workflow.Tasks.GetAll(x => x.Status == Sungero.Workflow.Task.Status.InProcess && x.Author.Equals(employee));
              if (allTasks != null)
              {
                foreach (var task in allTasks)
                {
                  try
                  {
                    task.ActiveText = textTasks;
                    task.Abort();
                  }
                  catch(Exception ex)
                  {
                    Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении оставшейся задачи с ИД {0}", ex, task.Id);
                  }
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении всех оставшихся задач.", ex);
            }
            #endregion
          }
        }
        catch(Exception ex)
        {
          Logger.Error("TryDisablingAccounts: Ошибка при прекращении задач.", ex);
        }
        #endregion
        
        #region Прекращение заданий
        try
        {
          if (assignmentAbort == "Да")
          {
            #region Прекращение заданий на Подписание
            try
            {
              var approvalSigningAssignments = Sungero.Docflow.ApprovalSigningAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalSigningAssignments)
              {
                try
                {
                  assignment.ActiveText = textAssignment;
                  assignment.Complete(Sungero.Docflow.ApprovalSigningAssignment.Result.Abort);
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания на подписание с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий на подписание.", ex);
            }
            #endregion
            
            #region Прекращение заданий на Согласование
            try
            {
              var approvalAssignments = Sungero.Docflow.ApprovalAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalAssignments)
              {
                try
                {
                  var rule = Sungero.Docflow.ApprovalTasks.As(assignment.Task).ApprovalRule;
                  var responsibleType = rule.ReworkPerformerType;
                  var responsible = Sungero.CoreEntities.Recipients.Null;
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.ApprovalRole)
                  {
                    var approvalRole = rule.ReworkApprovalRole;
                    responsible = lenspec.Etalon.PublicFunctions.ApprovalRole.GetRolePerformer(lenspec.Etalon.ApprovalRoles.As(approvalRole), Sungero.Docflow.ApprovalTasks.As(assignment.Task));
                  }
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.Author)
                  {
                    responsible = assignment.Task.Author;
                  }
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.EmployeeRole)
                  {
                    responsible = rule.ReworkPerformer;
                  }
                  if (responsible != null)
                  {
                    assignment.Complete(Sungero.Docflow.ApprovalAssignment.Result.ForRevision);
                  }
                  else
                  {
                    assignment.ActiveText = textAssignment;
                    assignment.Complete(Sungero.Docflow.ApprovalAssignment.Result.Approved);
                  }
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания на согласование с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий на согласование.", ex);
            }
            #endregion
            
            #region Прекращение заданий на Печать
            try
            {
              var approvalPrintingAssignments = Sungero.Docflow.ApprovalPrintingAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalPrintingAssignments)
              {
                try
                {
                  var rule = Sungero.Docflow.ApprovalTasks.As(assignment.Task).ApprovalRule;
                  var responsibleType = rule.ReworkPerformerType;
                  var responsible = Sungero.CoreEntities.Recipients.Null;
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.ApprovalRole)
                  {
                    var approvalRole = rule.ReworkApprovalRole;
                    responsible = lenspec.Etalon.PublicFunctions.ApprovalRole.GetRolePerformer(lenspec.Etalon.ApprovalRoles.As(approvalRole), Sungero.Docflow.ApprovalTasks.As(assignment.Task));
                  }
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.Author)
                  {
                    responsible = assignment.Task.Author;
                  }
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.EmployeeRole)
                  {
                    responsible = rule.ReworkPerformer;
                  }
                  if (responsible != null)
                  {
                    assignment.Complete(Sungero.Docflow.ApprovalPrintingAssignment.Result.ForRevision);
                  }
                  else
                  {
                    assignment.ActiveText = textAssignment;
                    assignment.Complete(Sungero.Docflow.ApprovalPrintingAssignment.Result.Execute);
                  }
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания на печать с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий на печать.", ex);
            }
            #endregion
            
            #region Прекращение заданий на Согласование с руководителем
            try
            {
              var approvalManagerAssignments = Sungero.Docflow.ApprovalManagerAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalManagerAssignments)
              {
                try
                {
                  assignment.ActiveText = textAssignment;
                  assignment.Complete(Sungero.Docflow.ApprovalManagerAssignment.Result.ForRevision);
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания на согласование с руководителем с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий на согласование с руководителем.", ex);
            }
            #endregion
            
            #region Прекращение заданий на Рассмотрение с адресатом
            try
            {
              var approvalReviewAssignments = Sungero.Docflow.ApprovalReviewAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalReviewAssignments)
              {
                try
                {
                  assignment.ActiveText = textAssignment;
                  assignment.Complete(Sungero.Docflow.ApprovalReviewAssignment.Result.ForRework);
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания на рассмотрение с адресатом с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий на рассмотрение с адресатом.", ex);
            }
            #endregion
            
            #region Прекращение заданий на Регистрацию
            try
            {
              var approvalRegistrationAssignments = Sungero.Docflow.ApprovalRegistrationAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalRegistrationAssignments)
              {
                try
                {
                  var rule = Sungero.Docflow.ApprovalTasks.As(assignment.Task).ApprovalRule;
                  var responsibleType = rule.ReworkPerformerType;
                  var responsible = Sungero.CoreEntities.Recipients.Null;
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.ApprovalRole)
                  {
                    var approvalRole = rule.ReworkApprovalRole;
                    responsible = lenspec.Etalon.PublicFunctions.ApprovalRoleBase.GetRolePerformer(lenspec.Etalon.ApprovalRoleBases.As(approvalRole), Sungero.Docflow.ApprovalTasks.As(assignment.Task));
                    
                    //                    if (lenspec.Etalon.ApprovalRoles.Is(approvalRole))
                    //                      responsible = lenspec.Etalon.PublicFunctions.ApprovalRole.GetRolePerformer(lenspec.Etalon.ApprovalRoles.As(approvalRole), Sungero.Docflow.ApprovalTasks.As(assignment.Task));
                    //                    if (lenspec.EtalonDatabooks.ComputedRoles.Is(approvalRole))
                    //                      responsible = lenspec.EtalonDatabooks.PublicFunctions.ComputedRole.GetRolePerformer(lenspec.EtalonDatabooks.ComputedRoles.As(approvalRole), Sungero.Docflow.ApprovalTasks.As(assignment.Task));
                  }
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.Author)
                  {
                    responsible = assignment.Task.Author;
                  }
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.EmployeeRole)
                  {
                    responsible = rule.ReworkPerformer;
                  }
                  if (responsible != null)
                  {
                    assignment.Complete(Sungero.Docflow.ApprovalRegistrationAssignment.Result.ForRevision);
                  }
                  else
                  {
                    assignment.ActiveText = textAssignment;
                    assignment.Complete(Sungero.Docflow.ApprovalRegistrationAssignment.Result.Execute);
                  }
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания на регистрацию с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий на регистрацию.", ex);
            }
            #endregion
            
            #region Прекращение заданий на Создание поручений
            try
            {
              var approvalExecutionAssignments = Sungero.Docflow.ApprovalExecutionAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalExecutionAssignments)
              {
                try
                {
                  var rule = Sungero.Docflow.ApprovalTasks.As(assignment.Task).ApprovalRule;
                  var responsibleType = rule.ReworkPerformerType;
                  var responsible = Sungero.CoreEntities.Recipients.Null;
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.ApprovalRole)
                  {
                    var approvalRole = rule.ReworkApprovalRole;
                    responsible = lenspec.Etalon.PublicFunctions.ApprovalRole.GetRolePerformer(lenspec.Etalon.ApprovalRoles.As(approvalRole), Sungero.Docflow.ApprovalTasks.As(assignment.Task));
                  }
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.Author)
                  {
                    responsible = assignment.Task.Author;
                  }
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.EmployeeRole)
                  {
                    responsible = rule.ReworkPerformer;
                  }
                  if (responsible != null)
                  {
                    assignment.Complete(Sungero.Docflow.ApprovalExecutionAssignment.Result.ForRevision);
                  }
                  else
                  {
                    assignment.ActiveText = textAssignment;
                    assignment.Complete(Sungero.Docflow.ApprovalExecutionAssignment.Result.Complete);
                  }
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания на создание поручений с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий на создание поручений.", ex);
            }
            #endregion
            
            #region Прекращение заданий
            try
            {
              var approvalCheckingAssignments = Sungero.Docflow.ApprovalCheckingAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalCheckingAssignments)
              {
                try
                {
                  var rule = Sungero.Docflow.ApprovalTasks.As(assignment.Task).ApprovalRule;
                  var responsibleType = rule.ReworkPerformerType;
                  var responsible = Sungero.CoreEntities.Recipients.Null;
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.ApprovalRole)
                  {
                    var approvalRole = rule.ReworkApprovalRole;
                    responsible = lenspec.Etalon.PublicFunctions.ApprovalRole.GetRolePerformer(lenspec.Etalon.ApprovalRoles.As(approvalRole), Sungero.Docflow.ApprovalTasks.As(assignment.Task));
                  }
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.Author)
                  {
                    responsible = assignment.Task.Author;
                  }
                  if (responsibleType == Sungero.Docflow.ApprovalRule.ReworkPerformerType.EmployeeRole)
                  {
                    responsible = rule.ReworkPerformer;
                  }
                  if (responsible != null)
                  {
                    assignment.Complete(Sungero.Docflow.ApprovalCheckingAssignment.Result.ForRework);
                  }
                  else
                  {
                    assignment.ActiveText = textAssignment;
                    assignment.Complete(Sungero.Docflow.ApprovalCheckingAssignment.Result.Accept);
                  }
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий.", ex);
            }
            #endregion
            
            #region Прекращение заданий на Отправку контрагенту
            try
            {
              var approvalSendingAssignments = Sungero.Docflow.ApprovalSendingAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalSendingAssignments)
              {
                try
                {
                  assignment.ActiveText = textAssignment;
                  assignment.Complete(Sungero.Docflow.ApprovalSendingAssignment.Result.Complete);
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания на отправку контрагенту с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий на отправку контрагенту.", ex);
            }
            #endregion
            
            #region Прекращение заданий Заявка на изменение компонентов Directum RX
            try
            {
              var editComponentRXRequestAssignments = lenspec.AutomatedSupportTickets.ApprovalManagers.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in editComponentRXRequestAssignments)
              {
                try
                {
                  assignment.ActiveText = textAssignment;
                  assignment.Complete(lenspec.AutomatedSupportTickets.ApprovalManager.Result.Reject);
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания Заявка на изменение компонентов Directum RX с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий Заявка на изменение компонентов Directum RX.", ex);
            }
            #endregion
            
            #region Прекращение заданий Заявка на формирование замещения
            try
            {
              var approvalSubstitutionAssignment = lenspec.AutomatedSupportTickets.ApprovalSubstitutionAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalSubstitutionAssignment)
              {
                try
                {
                  assignment.ActiveText = textAssignment;
                  assignment.Complete(lenspec.AutomatedSupportTickets.ApprovalSubstitutionAssignment.Result.Reject);
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания Заявка на формирование замещения с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий Заявка на формирование замещения.", ex);
            }
            #endregion
            
            #region Прекращение заданий на Контроль возврата
            try
            {
              var approvalCheckReturnAssignments = Sungero.Docflow.ApprovalCheckReturnAssignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in approvalCheckReturnAssignments)
              {
                try
                {
                  assignment.ActiveText = textAssignment;
                  assignment.Complete(Sungero.Docflow.ApprovalCheckReturnAssignment.Result.NotSigned);
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания на контроль возврата с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий на контроль возврата.", ex);
            }
            #endregion
            
            #region Прекращение всех оставшихся заданий
            try
            {
              var allAssignments = Sungero.Workflow.Assignments.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
              foreach (var assignment in allAssignments)
              {
                try
                {
                  assignment.ActiveText = textAssignment;
                  assignment.Complete(null);
                }
                catch(Exception ex)
                {
                  result.Add(assignment);
                  Logger.ErrorFormat("TryDisablingAccounts: Ошибка при прекращении задания с ИД {0}", ex, assignment.Id);
                }
              }
            }
            catch(Exception ex)
            {
              Logger.Error("TryDisablingAccounts: Ошибка при прекращении всех заданий.", ex);
            }
            #endregion
          }
        }
        catch(Exception ex)
        {
          Logger.Error("TryDisablingAccounts: Ошибка при прекращении заданий", ex);
        }
        #endregion
        
        //Если скопились задания с ошибкой, то отправим уведомление
        if (result.Count() > 0)
        {
          if (performer != null)
          {
            var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices(lenspec.AutomatedSupportTickets.Resources.AssignmentsInProgressByEmployeeFormat(employee.Name), performer);
            notice.ActiveText = lenspec.AutomatedSupportTickets.Resources.CheckAssignmentsInProgress;
            foreach (var res in result)
            {
              if (!notice.Attachments.Contains(res))
                notice.Attachments.Add(res);
            }
            notice.Start();
          }
        }
        
        #region Прекращение уведомлений
        try
        {
          var notices = Sungero.Workflow.Notices.GetAll(x => x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess && x.Performer.Equals(employee));
          foreach (var notice in notices)
          {
            notice.IsRead = true;
          }
        }
        catch(Exception ex)
        {
          Logger.Error("TryDisablingAccounts: Ошибка при прочтении уведомлений.", ex);
        }
        #endregion
        
        #region Прекращение замещений
        try
        {
          if (substitutionsAbort == "Да")
          {
            //Закрытие замещений пользователей(Замещающий)
            var substitutions = Sungero.CoreEntities.Substitutions.GetAll().Where(s => s.IsSystem != true
                                                                                  && (Equals(s.Substitute, employee) || Equals(s.User, employee))
                                                                                  && (!s.EndDate.HasValue || s.EndDate >= Calendar.UserToday));
            if (substitutions != null)
            {
              foreach (var substitution in substitutions)
              {
                substitution.EndDate = Calendar.UserToday;
                substitution.Save();
              }
            }
          }
        }
        catch(Exception ex)
        {
          Logger.Error("TryDisablingAccounts: Ошибка при прекращении замещений.", ex);
        }
        
        #endregion
        
        #region Закрытие УЗ
        try
        {
          var login = employee.Login;
          if (login != null && login.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
          {
            var loginToClose = Sungero.CoreEntities.Logins.Get(login.Id);
            loginToClose.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
            loginToClose.Save();
          }
        }
        catch(Exception ex)
        {
          Logger.Error("TryDisablingAccounts: Ошибка при закрытии УЗ", ex);
        }
        #endregion
        
        if (performer != null)
        {
          var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices(lenspec.AutomatedSupportTickets.Resources.DisablingAccountsCompleteNoticeSubject, performer);
          notice.Start();
        }
      }
      catch(Exception ex)
      {
        Logger.Error("TryDisablingAccounts: ", ex);
        var performer = Users.GetAll(x => x.Id == args.userId).SingleOrDefault();
        if (performer != null)
        {
          var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices(lenspec.AutomatedSupportTickets.Resources.DisablingAccountsErrorNoticeSubject, performer);
          notice.ActiveText = ex.Message;
          notice.Start();
        }
      }
    }
  }
}