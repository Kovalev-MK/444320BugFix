using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ProtocolsCollegialBodies.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Найти прокололы коллегиальных органов, подходящие по критериям поиска.
    /// </summary>
    /// <returns>Список прокололов коллегиальных органов.</returns>
    [Remote(IsPure = true)]
    public static IQueryable<IProtocolCollegialBody> SearchProtocol(DateTime? createdFrom, DateTime? createdTo, Sungero.Company.IEmployee author)
    {
      var protocols = ProtocolsCollegialBodies.ProtocolCollegialBodies.GetAll();
      
      if (createdFrom != null)
        protocols = protocols.Where(x => x.Created >= createdFrom);
      
      if (createdTo != null)
      {
        createdTo = createdTo.Value.Date.EndOfDay();
        protocols = protocols.Where(x => x.Created <= createdTo);
      }
      
      if (author != null)
        protocols = protocols.Where(x => x.Author.Equals(author));
      
      return protocols;
    }
    
    /// <summary>
    /// Создать клиентский договор.
    /// </summary>
    /// <returns>Клиентский договор.</returns>
    [Remote, Public]
    public static ProtocolsCollegialBodies.IProtocolCollegialBody CreateProtocol()
    {
      return ProtocolsCollegialBodies.ProtocolCollegialBodies.Create();
    }

  }
}