using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ExchangeDocument;

namespace lenspec.Etalon.Client
{
  partial class ExchangeDocumentFunctions
  {
    /// <summary>
    /// Получить список типов документов, доступных для смены типа.
    /// </summary>
    /// <returns>Список типов документов, доступных для смены типа.</returns>
    public override List<Sungero.Domain.Shared.IEntityInfo> GetTypesAvailableForChange()
    {
      var types = base.GetTypesAvailableForChange();
      types.Add(avis.EtalonContracts.AttachmentContractDocuments.Info);
      
      return types;
    }
  }
}