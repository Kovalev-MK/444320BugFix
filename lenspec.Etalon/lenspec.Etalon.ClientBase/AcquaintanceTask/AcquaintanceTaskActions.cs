using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.AcquaintanceTask;

namespace lenspec.Etalon.Client
{
  partial class AcquaintanceTaskActions
  {
    
    //Добавлено Avis Expert
    public virtual void ShowSubtasksByPerformersPartsavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var subtasks = Etalon.AcquaintanceTasks.GetAll(x => _obj.Equals(x.TaskWithAllParticipantsavis));
      subtasks.Show();
    }

    public virtual bool CanShowSubtasksByPerformersPartsavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (_obj.State.IsInserted || _obj.Status == Sungero.RecordManagement.AcquaintanceTask.Status.Draft)
        return false;
      
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      return document != null && (Sungero.RecordManagement.OrderBases.Is(document) || Tenders.AccreditationCommitteeProtocols.Is(document));
    }
    //конец Добавлено Avis Expert

  }


}