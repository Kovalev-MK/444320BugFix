using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.RecallPowerOfAttorney;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class RecallPowerOfAttorneyFunctions
  {
    
    public void GetDuplicateTasksDialog(List<PowerOfAttorneyModule.IRecallPowerOfAttorney> duplicates)
    {
      var dialog = Dialogs.CreateTaskDialog("Документ уже направлен на согласование.");
      var showTasksButton = dialog.Buttons.AddCustom("Показать задачи");
      dialog.Buttons.AddCancel();
      if (dialog.Show() == showTasksButton)
        duplicates.Show("Дублирующие задачи");
    }

    /// <summary>
    /// Проверить есть ли дубликаты задач
    /// </summary>
    /// <returns>True - есть, иначе - false</returns>
    public bool CheckDuplicates()
    {
      var duplicates = Functions.RecallPowerOfAttorney.Remote.GetTaskDuplicates(_obj);
      var isHereDuplicates = duplicates.Any();
      if (isHereDuplicates)
        GetDuplicateTasksDialog(duplicates);
      return isHereDuplicates;
    }

  }
}