using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.AccreditationCommittee;

namespace lenspec.Tenders
{
  partial class AccreditationCommitteeServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      _obj.Name = _obj.Name.Trim();
      _obj.Index = _obj.Index.Trim();
      
      var duplicates = Functions.AccreditationCommittee.GetDuplicateAccreditationCommittees(_obj.Name, _obj.Index, _obj.Id);
      if (duplicates.Any())
        e.AddError(lenspec.Tenders.AccreditationCommittees.Resources.DuplicatesDetected, _obj.Info.Actions.ShowDuplicates);
    }
  }

}