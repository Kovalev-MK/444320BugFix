using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.ProtocolCollegialBody;

namespace lenspec.ProtocolsCollegialBodies
{

  partial class ProtocolCollegialBodySharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void DateMeetingChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      FillName();
    }
    //конец Добавлено Avis Expert

    public virtual void CollegialBodyChanged(lenspec.ProtocolsCollegialBodies.Shared.ProtocolCollegialBodyCollegialBodyChangedEventArgs e)
    {
      var CollegialBody = _obj.CollegialBody;
      if (CollegialBody != null)
      {
        _obj.IndexGK = CollegialBody.Index;
        _obj.Chairman = CollegialBody.Chairman;
        if (CollegialBody.CollectionProperty.Count > 0)
        {
          //Очистим членов, если были до этого заполнены
          _obj.Members.Clear();
          //Теперь заполним новыми
          foreach  (var employee in CollegialBody.CollectionProperty.ToList())
          {
            var newMember = _obj.Members.AddNew();
            newMember.Member = employee.Member;
          }
        }
      }
    }

    public override void SubjectChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.SubjectChanged(e);
      FillName();
    }

    public override void RegistrationDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.RegistrationDateChanged(e);
      FillName();
    }

    public override void RegistrationNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.RegistrationNumberChanged(e);
      FillName();
    }

  }
}