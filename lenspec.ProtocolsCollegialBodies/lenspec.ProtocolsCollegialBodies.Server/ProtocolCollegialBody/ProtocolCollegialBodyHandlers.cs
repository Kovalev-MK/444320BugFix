using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.ProtocolCollegialBody;


namespace lenspec.ProtocolsCollegialBodies
{
  partial class ProtocolCollegialBodyServerHandlers
  {
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.Secretary = Sungero.Company.Employees.As(Users.Current);
      _obj.CreatedGK = Calendar.UserToday;
    }
  }
}