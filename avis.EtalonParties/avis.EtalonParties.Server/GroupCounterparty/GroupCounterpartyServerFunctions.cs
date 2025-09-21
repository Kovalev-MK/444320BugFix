using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.GroupCounterparty;

namespace avis.EtalonParties.Server
{
  partial class GroupCounterpartyFunctions
  {

    /// <summary>
    /// Получить группу контрагента по ИД Директм 5
    /// </summary>    
    [Public]
    public static avis.EtalonParties.IGroupCounterparty GetByIdDirectum5(int id)
    {
      return GroupCounterparties.GetAll(x => x.IdDirectum5 == id).SingleOrDefault();
    }

  }
}