using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.SubstitutionRequestTask;

namespace lenspec.AutomatedSupportTickets.Client
{
  partial class SubstitutionRequestTaskFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Проверить возможность отклонения задания.
    /// </summary>
    /// <param name="assignment">Задание.</param>
    /// <param name="errorMessage">Сообщение об ошибке.</param>
    /// <param name="eventArgs">Аргумент обработчика вызова.</param>
    /// <returns>True - разрешить отклонение, иначе false.</returns>
    public static bool ValidateBeforeReject(Sungero.Workflow.IAssignment assignment, string errorMessage, Sungero.Domain.Client.ExecuteActionArgs eventArgs)
    {
      if (string.IsNullOrWhiteSpace(assignment.ActiveText))
      {
        eventArgs.AddError(errorMessage);
        return false;
      }
      
      if (!eventArgs.Validate())
        return false;
      
      return true;
    }
    //конец Добавлено Avis Expert
  }
}