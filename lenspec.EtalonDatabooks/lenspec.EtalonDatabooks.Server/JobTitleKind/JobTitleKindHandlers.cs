using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.JobTitleKind;

namespace lenspec.EtalonDatabooks
{
  partial class JobTitleKindServerHandlers
  {

    //Добавлено Avis Expert
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if(_obj.Role == null)
      {
        var role = Sungero.CoreEntities.Roles.GetAll().Where(r => r.Name.Equals(_obj.Name)).FirstOrDefault();
        if (role == null)
        {
          role = Sungero.CoreEntities.Roles.Create();
          role.Name = _obj.Name;
          role.IsSystem = true;
          role.Save();
        }
        _obj.Role = role;
      }
      else
      {
        if(!_obj.Name.Equals(_obj.Role.Name))
        {
          //TODO: изменение в роли не сохраняются
          _obj.Role.Name = _obj.Name;
        }
      }
    }
    //конец Добавлено Avis Expert
    
  }

}