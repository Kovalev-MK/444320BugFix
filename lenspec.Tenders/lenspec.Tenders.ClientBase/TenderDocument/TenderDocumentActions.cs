using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderDocument;

namespace lenspec.Tenders.Client
{
  partial class TenderDocumentActions
  {

    public virtual bool CanCreatePDFWithStamp(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return false;
      
      // DIRRXMIGR-810: Использовалось для отладки действия, далее не нужно.
      // var isAvailable = false;
      // e.Params.TryGetValue(Constants.TenderDocument.Params.CanCreatePDFWithStamp, out isAvailable);
      //
      // return _obj.HasVersions && isAvailable;
    }

    public virtual void CreatePDFWithStamp(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Преобразовываем версию в ПДФ.
      var error = lenspec.Etalon.PublicFunctions.OfficialDocument.Remote.ConvertToPdfWithStamp(_obj, _obj.LastVersion.Id);
      
      if (!string.IsNullOrEmpty(error))
        Dialogs.CreateTaskDialog("Ошибка преобразования", error).Show();
    }
  }

}