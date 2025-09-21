using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.AccreditationCommittee;

namespace lenspec.Tenders.Client
{
  partial class AccreditationCommitteeActions
  {
    public virtual void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = Functions.AccreditationCommittee.Remote.GetDuplicateAccreditationCommittees(_obj.Name, _obj.Index, _obj.Id);
      if (duplicates.Any())
        duplicates.Show();
      else
        Dialogs.NotifyMessage(lenspec.Tenders.AccreditationCommittees.Resources.DuplicateNotFound);
    }

    public virtual bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}