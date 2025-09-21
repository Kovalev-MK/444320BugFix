using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Order;

namespace lenspec.Etalon.Client
{
  partial class OrderActions
  {
    
    //Добавлено Avis Expert
    public virtual void CreateDocumentApprovedByOrderavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var documentApprovedByOrder = Functions.Order.Remote.CreateDocumentApprovedByOrder(_obj);
      documentApprovedByOrder.Show();
    }

    public virtual bool CanCreateDocumentApprovedByOrderavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && !_obj.State.IsChanged;
    }
    //конец Добавлено Avis Expert

  }


}