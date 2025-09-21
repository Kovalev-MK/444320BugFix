using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractualDocument;

namespace lenspec.Etalon.Client
{
  partial class ContractualDocumentFunctions
  {

    /// <summary>
    /// Изменить значения полей "Типовой" и "ИД шаблона".
    /// </summary>
    public void ChangeIsStandardProperty()
    {
      if (_obj.InternalApprovalState == InternalApprovalState.Signed)
        return;
      
      try
      {
        _obj.IsStandard = false;
        _obj.TemplateIDlenspec = null;
        _obj.Save();
      }
      catch(Exception ex)
      {
        Logger.Error(ex);
        var asyncHandler = EtalonDatabooks.AsyncHandlers.AsyncChangeContractualDocumentIsStandardProperty.Create();
        asyncHandler.DocumentId = _obj.Id;
        asyncHandler.ExecuteAsync();
      }
    }

  }
}