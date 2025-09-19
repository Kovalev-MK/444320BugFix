using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.CustomerRequests.Server
{
  public class ModuleAsyncHandlers
  {
    /// <summary>
    /// Отправляем обращение клиента в СРМ.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncSendMessageCRM(avis.CustomerRequests.Server.AsyncHandlerInvokeArgs.AsyncSendMessageCRMInvokeArgs args)
    {
      // Получаем обращение клиента по ид.
      var customerRequest = avis.CustomerRequests.CustomerRequests.GetAll(c => c.Id == args.CustomerRequestId).FirstOrDefault();
      // Отправляем информацию в СРМ.
      lenspec.Etalon.Module.Integration.PublicFunctions.Module.SendCrmPutClaimRequest(customerRequest);
    }

    public virtual void ChangeRelations(avis.CustomerRequests.Server.AsyncHandlerInvokeArgs.ChangeRelationsInvokeArgs args)
    {
      var req =  avis.CustomerRequests.CustomerRequests.GetAll().Where(x => x.Id == args.DocId).FirstOrDefault();
      var docs = req.Relations.GetRelatedFromDocuments(Sungero.Docflow.PublicConstants.Module.SimpleRelationName);
      
      foreach (var doc in docs)
      {
        req.Relations.RemoveFrom(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, doc);
      }
      foreach (var contract in req.SDAContracts)
      {
        req.Relations.AddFrom(Sungero.Docflow.PublicConstants.Module.SimpleRelationName, contract.Contract);
      }
      try
      {
        req.Save();
      }
      catch
      {
        args.Retry = true;
      }
    }
  }
}