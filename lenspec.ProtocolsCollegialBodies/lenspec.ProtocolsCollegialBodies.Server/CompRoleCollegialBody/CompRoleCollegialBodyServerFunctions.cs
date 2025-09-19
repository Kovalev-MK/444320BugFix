using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ProtocolsCollegialBodies.CompRoleCollegialBody;

namespace lenspec.ProtocolsCollegialBodies.Server
{
  partial class CompRoleCollegialBodyFunctions
  {

    /// Получить список исполнителей из роли "Члены коллегиального органа".
    /// <param name="task">Задача.</param>
    /// <returns>Список исполнителей.</returns>
    [Remote(IsPure = true), Public]
    public List<Sungero.CoreEntities.IRecipient> GetProtocolsCollegialBodyMembers(Sungero.Docflow.IApprovalTask task)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var protocol = ProtocolsCollegialBodies.ProtocolCollegialBodies.As(document);
      if (protocol!= null && _obj.Type == ProtocolsCollegialBodies.CompRoleCollegialBody.Type.Members)
      {
        var members = protocol.Members.Where(x => x.Member != null);
        foreach (var item in members)
          result.Add(item.Member);
      }
      
      return result;
    }

    /// <summary>
    /// Вычисление роли Председатель коллегиального органа.
    /// </summary>
    public override Sungero.Company.IEmployee GetRolePerformer(Sungero.Docflow.IApprovalTask task)
    {
      if (_obj.Type == ProtocolsCollegialBodies.CompRoleCollegialBody.Type.Chairman)
      {
        var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
        var protocol = ProtocolsCollegialBodies.ProtocolCollegialBodies.As(document);
        if (protocol != null)
          return protocol.CollegialBody.Chairman;
        
        return null;
      }
      
      return base.GetRolePerformer(task);
    }
  }
}