using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonParties.CategoryCounterparty;

namespace avis.EtalonParties.Server
{
  partial class CategoryCounterpartyFunctions
  {

    /// <summary>
    /// Получить категорию контрагента по ИД директум 5
    /// </summary>
    [Public]
    public static EtalonParties.ICategoryCounterparty GetByIdDirectum5(int id)
    {
      return avis.EtalonParties.CategoryCounterparties.GetAll(x => x.IdDirectum5 == id).SingleOrDefault();
    }

  }
}