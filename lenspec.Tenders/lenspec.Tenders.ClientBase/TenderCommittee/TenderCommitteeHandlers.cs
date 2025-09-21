using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderCommittee;

namespace lenspec.Tenders
{
  partial class TenderCommitteeMembersClientHandlers
  {

    //Добавлено Avis Expert
    public virtual void MembersMemberValueInput(lenspec.Tenders.Client.TenderCommitteeMembersMemberValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.Tenders.TenderCommittees.Resources.NeedSpecifyAutomatedEmployee);
      }
    }
    //конец Добавлено Avis Expert
  }

  partial class TenderCommitteeClientHandlers
  {

    //Добавлено Avis Expert
    public virtual void ProtocolRegistrarValueInput(lenspec.Tenders.Client.TenderCommitteeProtocolRegistrarValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.Tenders.TenderCommittees.Resources.NeedSpecifyAutomatedEmployee);
      }
    }

    public virtual void ChairmanValueInput(lenspec.Tenders.Client.TenderCommitteeChairmanValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.Tenders.TenderCommittees.Resources.NeedSpecifyAutomatedEmployee);
      }
    }
    //конец Добавлено Avis Expert

  }
}