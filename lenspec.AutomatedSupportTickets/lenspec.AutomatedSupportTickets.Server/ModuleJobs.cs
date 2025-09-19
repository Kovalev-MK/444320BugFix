using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.AutomatedSupportTickets.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// Эталон. Компания. Автоматическое выполнение заданий членами КО и ТК.
    /// </summary>
    public virtual void AutoCompleteApprovalAssignments()
    {
    }

    //Добавлено Avis Expert
    /// <summary>
    /// Фоновый процесс для создания задач об окончании замещений.
    /// </summary>
    public virtual void SendSubstitutionExpirationTask()
    {
      var substitutions = Sungero.CoreEntities.Substitutions.GetAll().Where(x => x.IsSystem != true && x.StartDate != null && x.StartDate.Value < Calendar.Today &&
                                                                            x.EndDate != null && x.EndDate.Value.Equals(Calendar.Today.AddDays(3)) &&
                                                                            x.Comment != null && x.Comment != string.Empty &&
                                                                            x.Substitute.Status == Sungero.CoreEntities.DatabookEntry.Status.Active &&
                                                                            x.User.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      foreach(var substitution in substitutions)
      {
        try
        {
          var substitute = Sungero.Company.Employees.As(substitution.Substitute);
          var substitutedUser = Sungero.Company.Employees.As(substitution.User);
          var managerOfSubstitutedUser = substitute;
          
          var task = PublicFunctions.SubstitutionRequestTask.CreateSubstitutionRequestTask(substitute,
                                                                                           substitutedUser,
                                                                                           substitution.EndDate.Value.AddDays(1),
                                                                                           null,
                                                                                           managerOfSubstitutedUser,
                                                                                           substitution.Comment,
                                                                                           true);
          task.Start();
        }
        catch(Exception ex)
        {
          Logger.ErrorFormat("Avis - SendSubstitutionExpirationTask - не удалось создать задачу об окончании замещения {0}", ex, substitution.Id);
        }
      }
    }
    
    //конец Добавлено Avis Expert
  }
}