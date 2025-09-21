using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using avis.CustomerRequests.CustomerRequestTask;

namespace avis.CustomerRequests.Server
{
  partial class CustomerRequestTaskRouteHandlers
  {
    /// <summary>
    /// Старт задания "Регистрация обращения клиента".
    /// </summary>
    /// <param name="e"></param>
    public virtual void StartBlock5(avis.CustomerRequests.Server.CustomerRequestRegistrationAssignmentArguments e)
    {
      e.Block.Subject = avis.CustomerRequests.CustomerRequestTasks.Resources.CustomerRequestRegistrationBlockSubject;

      // Указываем иполнителем ответственного за ОЛК.
      var perfomer = avis.CustomerRequests.PublicFunctions.CustomerRequestTask.GetResponsibleOLK(_obj);
      if (perfomer != null)
      {
        e.Block.Performers.Add(perfomer);
        return;
      }
      
      // Если нету ОЛК то, Отправляем администратору сэд.
      var role = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
      e.Block.Performers.Add(role);
    }

    /// <summary>
    /// Преобразование в ПДФ.
    /// </summary>
    public virtual void Script12Execute()
    {
      var document = lenspec.Etalon.OfficialDocuments.As(_obj.AllAttachments.FirstOrDefault());
     
      lenspec.Etalon.Module.Docflow.PublicFunctions.Module.ConvertToPdfAvis(document, false);
    }
    
    /// <summary>
    /// Очищение полей карточки Обращение клиента.
    /// </summary>
    public virtual void Script10Execute()
    {
      var document = _obj.AllAttachments.FirstOrDefault();
      var customerRequest = avis.CustomerRequests.CustomerRequests.As(document);
      
      customerRequest.HFAddress = null;
      customerRequest.HFBankAccount = null;
      customerRequest.HFBankBIK = null;
      customerRequest.HFBankCorrAcc = null;
      customerRequest.HFBankInfo= null;
      customerRequest.HFBankINN = null;
      customerRequest.HFBankKPP = null;
      customerRequest.HFRecipient = null;
      customerRequest.HFSumReturn = null;
      customerRequest.Save();
    }
    
    /// <summary>
    /// Интеграция обращений с CRM.
    /// </summary>
    public virtual void Script9Execute()
    {
      var document = _obj.AllAttachments.FirstOrDefault();
      var customerRequest = avis.CustomerRequests.CustomerRequests.As(document);
      
      // Отправляем в интеграцию с CRM.
      var async = avis.CustomerRequests.AsyncHandlers.AsyncSendMessageCRM.Create();
      async.CustomerRequestId = customerRequest.Id;
      async.ExecuteAsync();
    }
  }
}