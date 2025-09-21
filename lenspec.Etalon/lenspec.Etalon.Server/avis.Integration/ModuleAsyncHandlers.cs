using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Newtonsoft.Json;
using NATS.Client;
using Minio;
using Minio.Exceptions;

namespace lenspec.Etalon.Module.Integration.Server
{
  // Добавлено avis.
  partial class ModuleAsyncHandlers
  {

    public virtual void AsyncExportCounterpartieslenspec(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncExportCounterpartieslenspecInvokeArgs args)
    {
    }

    public virtual void AsyncUpdateClientContractlenspec(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncUpdateClientContractlenspecInvokeArgs args)
    {
      var isForcedLocked = false;
      Logger.DebugFormat("AsyncUpdateClientContractlenspec - обновление КД ИД = {0}.", args.ClientContractId);
      var contract = SalesDepartmentArchive.SDAClientContracts.Get(args.ClientContractId);
      try
      {
        var lockinfo = Locks.GetLockInfo(contract);
        if (lockinfo != null && lockinfo.IsLocked)
        {
          args.Retry = true;
          Logger.ErrorFormat("AsyncUpdateClientContractlenspec - КД ИД = {0} - заблокирован пользователем {1}.", contract.Id, lockinfo.OwnerName);
          return;
        }
        else
        {
          isForcedLocked = Locks.TryLock(contract);
        }
        
        if (!isForcedLocked)
        {
          throw new Exception($"не удалось заблокировать карточку документа в Directum RX.");
        }
        
        contract.InvestContractCode = args.DogId;
        contract.ClientDocumentNumber = args.DogNum;
        contract.ClientDocumentDate = DateTime.Parse(args.DogDT);

        // Ищем объект проекта, если нашли добавляем в карточку КД.
        var objectAnProject =  lenspec.EtalonDatabooks.ObjectAnProjects.GetAll(x => x.Id.ToString() == args.DrxobjDrxid).FirstOrDefault();
        if (objectAnProject != null)
          contract.ObjectAnProject = objectAnProject;
        
        // Если не нашли объект, то проверяем наличие инн и ищем нашу организацию по нему.
        var businessUnit = BusinessUnits.GetAll(b => b.TIN == args.DeveloperInn).FirstOrDefault();
        if (businessUnit == null && objectAnProject == null)
        {
          args.Retry = false;
          throw new Exception("не найден объект, а так же по инн не найдена наша организация.");
        }
        
        // Ищем помещение, если не нашли и есть объект, то создаём новое помещение.
        var objectAnSale = lenspec.EtalonDatabooks.ObjectAnSales.GetAll(x => x.IdInvest == args.KvId).FirstOrDefault();
        if (objectAnSale == null && objectAnProject != null)
        {
          objectAnSale = lenspec.EtalonDatabooks.ObjectAnSales.Create();  //  Заполнить по умолчанию
          objectAnSale.IdInvest = args.KvId;

          var purposeOfPremises = lenspec.EtalonDatabooks.PurposeOfPremiseses.GetAll().FirstOrDefault();
          if (purposeOfPremises != null)
            objectAnSale.PurposeOfPremises = purposeOfPremises;
          
          objectAnSale.ObjectAnProject =  contract.ObjectAnProject;
          objectAnSale.OurCF =  contract.ObjectAnProject.OurCF;
          if (string.IsNullOrWhiteSpace(objectAnSale.Address))
            objectAnSale.Address = "Адрес";
          
          objectAnSale.NumberRoomMail = "Номер";
          objectAnSale.Settlement = lenspec.EtalonDatabooks.ObjectAnSale.Settlement.NotRequired;
          objectAnSale.Name = "Новое помещение из пакета интеграции контрактов SDA";
          objectAnSale.Save();
        }
        
        contract.Premises = objectAnSale;
        
        // Уведомление для Администратора СЭД.
        var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
        if (objectAnSale == null && objectAnProject == null)
        {
          // Создаём и стартуем задачу с уведомлением.
          var asyncNotification = AsyncHandlers.AsyncAdminEDMSNotificationavis.Create();
          asyncNotification.Subject = $"Ошибка синхронизации с Инвестом: в Клиентском договоре не найдены Объект проекта и Помещение объекта.";
          asyncNotification.ActiveText = $"В Клиентском договоре {contract.Name}, объект проекта и помещение объекта не найдены.";
          asyncNotification.SDAClientContractId = contract.Id;
          asyncNotification.ExecuteAsync();
        }
        else if (objectAnSale == null)
        {
          // Создаём и стартуем задачу с уведомлением.
          var asyncNotification = AsyncHandlers.AsyncAdminEDMSNotificationavis.Create();
          asyncNotification.Subject = $"Ошибка синхронизации с Инвестом: в Клиентском договоре не найдено Помещение объекта.";
          asyncNotification.ActiveText = $"В Клиентском договоре {contract.Name}, помещение объекта не найдено. Объект - {objectAnProject.Name}";
          asyncNotification.SDAClientContractId = contract.Id;
          asyncNotification.ExecuteAsync();
        }
        else if (objectAnProject == null)
        {
          // Создаём и стартуем задачу с уведомлением.
          var asyncNotification = AsyncHandlers.AsyncAdminEDMSNotificationavis.Create();
          asyncNotification.Subject = $"Ошибка синхронизации с Инвестом: в Клиентском договоре не найден Объект.";
          asyncNotification.ActiveText = $"В Клиентском договоре {contract.Name}, объект не найден. Помещение - {objectAnSale.Name}";
          asyncNotification.SDAClientContractId = contract.Id;
          asyncNotification.ExecuteAsync();
        }
        else if (objectAnSale.ObjectAnProject != objectAnProject)
        {
          // Создаём и стартуем задачу с уведомлением.
          var asyncNotification = AsyncHandlers.AsyncAdminEDMSNotificationavis.Create();
          asyncNotification.Subject = $"Ошибка синхронизации с Инвестом: в Клиентском договоре Помещение не относится к Объекту.";
          asyncNotification.ActiveText = $"В Клиентском договоре {contract.Name}, помещение {objectAnSale.Name} не относится к объекту {objectAnProject.Name}";
          asyncNotification.SDAClientContractId = contract.Id;
          asyncNotification.ExecuteAsync();
        }
        
        var oldPersList = contract.CounterpartyClient.Select(x => x.ClientItem).ToList();  // Будем пытаться закрывать кого не передали в это раз
        contract.CounterpartyClient.Clear();  // Если каждый раз полный комплект то очистить
        
        var dp_ids = args.DpId.Split(',');
        foreach (var pers in dp_ids)
        {
          var persona = lenspec.Etalon.People.GetAll().Where(x => x.CodeInvestavis == pers).FirstOrDefault();
          if (persona != null)
          {
            var p = contract.CounterpartyClient.AddNew();
            p.ClientItem = Sungero.Parties.Counterparties.As(persona);
          }
        }
        
        if (args.EcpFl == 1)
          contract.SignedWithAnElectronicSignature = SalesDepartmentArchive.SDAClientContract.SignedWithAnElectronicSignature.Yes;
        else
          contract.SignedWithAnElectronicSignature = SalesDepartmentArchive.SDAClientContract.SignedWithAnElectronicSignature.No;
        
        if (args.DogStatus == 1 || args.DogStatus == 2)
          contract.LifeCycleState =  SalesDepartmentArchive.SDAClientContract.LifeCycleState.Active;
        else
          contract.LifeCycleState =  SalesDepartmentArchive.SDAClientContract.LifeCycleState.Obsolete;
        
        bool wasEmptyOwnDate = false;
        if (contract.TransferOfOwnershipDate == null)
          wasEmptyOwnDate = true;
        
        if (!string.IsNullOrWhiteSpace(args.AppDt))
          contract.TransferOfOwnershipDate = DateTime.Parse(args.AppDt);
        
        // Если не нашли объект проекта, то берем нашу организацию из инн.
        if (contract.ObjectAnProject == null)
          contract.BusinessUnit = businessUnit;
        else
          contract.BusinessUnit = contract.ObjectAnProject.SpecDeveloper;  // НОР из объекта
        contract.Save();
        
        //-------------      1 условие ---------------------
        
        oldPersList = oldPersList.Where(x => !contract.CounterpartyClient.Select(y => y.ClientItem).Contains(x)).ToList();  // остались которые были и не стали
        
        foreach  (var clientToDel in oldPersList)
        {
          var client = lenspec.Etalon.People.As(clientToDel);
          
          if (client.Status == lenspec.Etalon.Person.Status.Closed)
            continue;
          
          var hasContract = false;
          var contractsForPers = SalesDepartmentArchive.SDAClientContracts.GetAll().Where(x => x.CounterpartyClient.Any(y => Equals(y.ClientItem.Id,client.Id))
                                                                                          && x.LifeCycleState ==  SalesDepartmentArchive.SDAClientContract.LifeCycleState.Active).Count();
          if (contractsForPers > 0)
            hasContract = true;
          
          if (!hasContract)
          {
            client.IsClientBuyersavis = false;
            
            if (  client.IsLawyeravis == false &&
                client.IsClientOwnersavis == false &&
                client.IsEmployeeGKavis == false &&
                client.IsOtheravis == false
               )
            {
              client.Status =  lenspec.Etalon.Person.Status.Closed;
            }
            
            client.Save();
          }
        }
        //-------------    2 условие
        
        if (wasEmptyOwnDate == true &&  contract.TransferOfOwnershipDate != null)  //надо проверять ..
        {
          foreach (var clnt in contract.CounterpartyClient)
          {
            var c = lenspec.Etalon.People.As(clnt.ClientItem);
            c.IsClientOwnersavis = true;
            
            // Есть ли КД для данной персоны с пустой датой передачи в собственность
            var contractsAll = SalesDepartmentArchive.SDAClientContracts.GetAll().Where(x => x.LifeCycleState == SalesDepartmentArchive.SDAClientContract.LifeCycleState.Active &&
                                                                                        x.CounterpartyClient.Any(y => Equals(y.ClientItem.Id,c.Id )) &&
                                                                                        x.TransferOfOwnershipDate == null).Count();
            if (contractsAll == 0)
              c.IsClientBuyersavis = false;
            
            c.Save();
          }
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("AsyncUpdateClientContractlenspec - не удалось сохранить изменения по КД ИД = {0} - {1}.", contract.Id, ex.Message);
        args.Retry = false;
        
        var asyncNotification = AsyncHandlers.AsyncAdminEDMSNotificationavis.Create();
        asyncNotification.Subject = $"Ошибка синхронизации с Инвестом: не удалось сохранить изменения по КД.";
        asyncNotification.ActiveText = $"Не удалось сохранить изменения по клиентскому договору (ИД {contract.Id}): {ex.Message}\r\nКод договора в Инвест: {args.DogId}";
        asyncNotification.SDAClientContractId = contract.Id;
        asyncNotification.ExecuteAsync();
      }
      finally
      {
        if (isForcedLocked)
          Locks.Unlock(contract);
      }
    }
    
    /// <summary>
    /// Сохранить информацию о результате последнего запуска ФП импорта орг.структуры.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncClosedEmployeeNotificationavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncClosedEmployeeNotificationavisInvokeArgs args)
    {
      try
      {
        Logger.DebugFormat("Avis - AsyncClosedEmployeeNotificationavis started for employee {0}", args.EmployeeId);
        
        var employee = Sungero.Company.Employees.Get(args.EmployeeId);
        if (employee == null)
        {
          Logger.ErrorFormat("Avis - AsyncClosedEmployeeNotificationavis - сотрудник с ИД {0} не найден.", args.EmployeeId);
          args.Retry = false;
          return;
        }
        
        var attachments = new List<Sungero.CoreEntities.IDatabookEntry>();
        var recipient = Sungero.CoreEntities.Recipients.As(employee);
        //var groups = Sungero.CoreEntities.Groups.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
        var approvalStages = Sungero.Docflow.ApprovalStages.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
        
        #region НОР
        attachments.AddRange(Etalon.BusinessUnits.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
                             .Where(x => x.CEO.Equals(employee) || x.RoleKindEmployeelenspec.Any(r => r.Employee.Equals(employee)))
                             .ToList());
        #endregion
        
        #region Роли с одним участником (нельзя удалить единственного)
        var roleByJobTitleKindIds = EtalonDatabooks.JobTitleKinds.GetAll(x => x.Role != null).Select(x => x.Role.Id).ToList().Distinct();
        attachments.AddRange(approvalStages.Select(x => Sungero.CoreEntities.Roles.As(x.Assignee))
                             .Where(x => x != null && x.IsSingleUser == true && !roleByJobTitleKindIds.Contains(x.Id) && x.RecipientLinks.Any(r => r.Member.Equals(recipient)))
                             .Distinct()
                             .ToList());
        #endregion
        
        /*
        #region Группы (из многих групп и ролей закрытый сотрудник исключается автоматически в событии После сохранения сотрудника)
        attachments.AddRange(groups.Where(x => !Etalon.BusinessUnits.Is(x) && !Sungero.Company.Departments.Is(x) &&
                                          !Sungero.CoreEntities.Roles.Is(x) && x.RecipientLinks.Any(r => r.Member.Equals(recipient))).ToList());
        #endregion
         */
        
        #region Этапы согласования
        attachments.AddRange(approvalStages.Where(x => x.Assignee != null && x.Assignee.Equals(recipient) ||
                                                  x.Recipients.Any(r => r.Recipient.Equals(recipient))).ToList());
        #endregion
        
        #region Настройки согласования
        attachments.AddRange(EtalonDatabooks.ApprovalSettings.GetAll(x => x.Status == EtalonDatabooks.ApprovalSetting.Status.Active)
                             .Where(x => x.RoleKindEmployee.Any(r => r.Employee.Equals(employee)))
                             .ToList());
        #endregion
        
        #region Настройки исполнения по обращениям клиентов
        attachments.AddRange(avis.CustomerRequests.CustReqSetups.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
                             .Where(x => employee.Equals(x.Controller) || employee.Equals(x.Executor))
                             .ToList());
        #endregion
        
        #region Коллегиальные органы
        attachments.AddRange(lenspec.ProtocolsCollegialBodies.CollegialBodies.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
                             .Where(x => employee.Equals(x.Chairman) || x.CollectionProperty.Any(c => employee.Equals(c.Member)))
                             .ToList());
        #endregion
        
        #region Права подписи
        attachments.AddRange(Sungero.Docflow.SignatureSettings.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
                             .Where(x => employee.Equals(x.Recipient))
                             .ToList());
        #endregion
        
        if (!attachments.Any())
        {
          Logger.Debug("Avis - AsyncClosedEmployeeNotificationavis - вложения не найдены");
          args.Retry = false;
          return;
        }
        
        var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
        var task = Sungero.Workflow.SimpleTasks.Create($"Закрылась запись сотрудника {employee.Name}, являющегося исполнителем по регламенту", administratorEDMSRole);
        task.NeedsReview = false;
        task.ActiveText = "Проверьте настройки и замените сотрудника.";
        if (!string.IsNullOrEmpty(args.EmployeeGroups))
        {
          task.ActiveText += args.EmployeeGroups;
        }
        task.Attachments.Add(employee);
        foreach (var attachment in attachments)
        {
          task.Attachments.Add(attachment);
        }
        task.Start();
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - AsyncClosedEmployeeNotificationavis ", ex);
      }
    }

    /// <summary>
    /// Сохранить информацию о результате последнего запуска ФП импорта орг.структуры.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncImportOrgStructureSaveResultlenspec(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncImportOrgStructureSaveResultlenspecInvokeArgs args)
    {
      Logger.Debug("Avis - AsyncImportOrgStructureSaveResultlenspec started");
      var setting = Integrationses.Get(args.SettingId);
      try
      {
        var runEntry = setting.RunHistory.AddNew();
        runEntry.StartDate = args.StartDate;
        runEntry.EndDate = args.EndDate;
        runEntry.Success = args.Success;
        runEntry.TextLog = args.TextLog;
        setting.SyncDateTime = args.StartDate;
        // Очистить историю запусков - оставить последние 10 записей.
        int maxEntities = 10;
        var runHistory = setting.RunHistory;
        int index = 0;
        foreach (var hist in runHistory.OrderByDescending(d => d.StartDate))
        {
          index++;
          if (index > maxEntities)
            setting.RunHistory.Remove(hist);
        }
        setting.Save();
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - AsyncImportOrgStructureSaveResultlenspec ", ex);
        args.Retry = true;
      }
      
      if (args.Retry == false && !args.Success && setting.Code == avis.Integration.Constants.Module.OrgSructureImportRecordCode)
      {
        Logger.Debug("Avis - AsyncImportOrgStructureSaveResultlenspec отправка уведомления об ошибках импорта оргрструктуры");
        
        var asyncNotification = AsyncHandlers.AsyncImportOrgStructureNotificationavis.Create();
        asyncNotification.Subject = "Ошибки при выполнении ФП Эталон. Интеграции с внешними системами. Импорт оргструктуры";
        asyncNotification.ActiveText = $"Проверьте лог-файл от {args.StartDate.ToString("d")}. Модуль Интеграции с внешними системами -> Импорт оргструктуры из 1С -> Загрузить лог.";
        asyncNotification.PersonId = 0;
        asyncNotification.ExecuteAsync();
      }
      
      Logger.Debug("Avis - AsyncImportOrgStructureSaveResultlenspec ended");
    }

    /// <summary>
    /// Создать уведомление для Администратора СЭД о работе ФП импорта орг.структуры.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncImportOrgStructureNotificationavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncImportOrgStructureNotificationavisInvokeArgs args)
    {
      try
      {
        Logger.Debug("Avis - AsyncImportOrgStructureNotificationavis started");
        var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
        var task = Sungero.Workflow.SimpleTasks.Create(args.Subject, administratorEDMSRole);
        task.NeedsReview = false;
        task.ActiveText = args.ActiveText;
        if (args.PersonId != 0)
        {
          var person = Sungero.Parties.People.GetAll(x => x.Id == args.PersonId).SingleOrDefault();
          if (person != null)
          {
            task.Attachments.Add(person);
          }
        }
        task.Start();
      }
      catch(Exception ex)
      {
        Logger.Error("Avis - AsyncImportOrgStructureNotificationavis ", ex);
      }
    }
    
    #region Интеграция с ЛК.
    /// <summary>
    /// Получить версию документа.
    /// </summary>
    /// <param name="docId">ИД документа.</param>
    /// <param name="verNumber">Версия документа.</param>
    /// <returns></returns>
    private Sungero.Content.IElectronicDocumentVersions GetVersionCustomer(int docId, int verNumber)
    {
      // Ищем документ с таким ИД.
      var doc = avis.CustomerRequests.CustomerRequests.GetAll(c => c.Id == docId).FirstOrDefault();
      if (doc == null)
        throw new Exception("Не найдено обращение с таким docId.");
      
      // Ищем версию с таким номером.
      var version = doc.Versions.Where(d => d.Number == verNumber).FirstOrDefault();
      if (version == null)
        throw new Exception("Не найдена версия документа.");
      
      return version;
    }

    /// <summary>
    /// Отправка тела обращения в промежуточное хранилище МИНИО.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncGetCustomerBodyavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncGetCustomerBodyavisInvokeArgs args)
    {
      var result = lenspec.Etalon.Module.Integration.Structures.Module.LkGetBodyCustomerResponseModel.Create();
      result.Header = lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse.Create();
      result.Body = lenspec.Etalon.Module.Integration.Structures.Module.LkGetBodyCustomerResponseBody.Create();
      result.Header.Status = "OK";
      try
      {
        // Заполняем ИД сообщения в ответ.
        result.Header.MessageId = args.MessageID;
        
        // Получаем тело обращения.
        var version = GetVersionCustomer(args.DocID, args.VerNumber);
        
        // Проверки на заполненность настроек для минио.
        var settings = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.NatsCode).FirstOrDefault();
        if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
          throw new Exception("Не найдены настройки подключения к minio.");
        
        // Расшифровываем, и адрес и путь к ключу.
        var settingsString = Encryption.Decrypt(settings.ConnectionParams);
        var connectionString = settingsString.Split(';');
        if (string.IsNullOrEmpty(connectionString[0]))
          throw new Exception("Отсутствует строка endpoint.");
        if (string.IsNullOrEmpty(connectionString[1]))
          throw new Exception("Отсутствует строка accessKey.");
        if (string.IsNullOrEmpty(connectionString[2]))
          throw new Exception("Отсутствует строка secretKey.");
        if (string.IsNullOrEmpty(connectionString[3]))
          throw new Exception("Отсутствует строка bucketNameGet.");
        if (string.IsNullOrEmpty(connectionString[4]))
          throw new Exception("Отсутствует строка bucketNamePut.");
        
        // Заполняем имя файла в промежуточном хранилище.
        var objectName = $"{args.DocID}_{args.VerNumber}.{version.AssociatedApplication.Extension}";
        
        var minio = new MinioClient()
          .WithEndpoint(connectionString[0])
          .WithCredentials(connectionString[1], connectionString[2])
          .WithSSL(false)
          .Build();
        
        // Upload a file to bucket.
        var putObjectArgs = new PutObjectArgs()
          .WithBucket(connectionString[4]) // название бакета.
          .WithObject(objectName) // название файла с типом файла name.txt.
          .WithStreamData(version.Body.Read()) // Поток с телом.
          .WithContentType(version.AssociatedApplication.Extension); // Поидеи контент тип надо в формате "text/*" но для теста попробуем так.
        minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
        
        // Get url.
        PresignedGetObjectArgs argsMinio = new PresignedGetObjectArgs()
          .WithBucket(connectionString[4]) // название бакета.
          .WithObject(objectName) // название файла с типом файла name.txt.
          .WithExpiry(60 * 60 * 24);
        // Заполняем url для ответа.
        result.Body.DocLink = minio.PresignedGetObjectAsync(argsMinio).ToString();
        //var url = minio.PresignedGetObjectAsync(argsMinio).ToString();
      }
      catch(Exception ex)
      {
        // Заполняем как ошибку.
        result.Header.Status = "Error";
        result.Header.StatusMessage = ex.Message;
      }
      
      // Отправляем в NATS.
      var jsonResult = JsonConvert.SerializeObject(result);
      var async = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncNatsRequestavis.Create();
      async.Subject = "lk";
      async.Data = JsonConvert.SerializeObject(result);
      async.ExecuteAsync();
    }
    
    /// <summary>
    /// Отправляем обращение клиента в промежуточное хранилище (MINIO).
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncCreateCustomerRequestavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncCreateCustomerRequestavisInvokeArgs args)
    {
      // Отправляем в NATS.
      var async = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncNatsRequestavis.Create();
      async.Subject = "lk";
      async.Data = args.JsonResult;
      async.ExecuteAsync();
    }
    
    /// <summary>
    /// Отправить запрос в очередь NATS.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncNatsRequestavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncNatsRequestavisInvokeArgs args)
    {
      // Проверяем заполненность днных.
      if (string.IsNullOrEmpty(args.Subject))
        throw new Exception("Имя очереди не может быть пустой.");
      
      if (string.IsNullOrEmpty(args.Data))
        throw new Exception("data не может быть пустой.");
      
      // Получаем токены.
      var settings = Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.NatsCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw new Exception("Не найдены настройки подключения");
      
      // Расшифровываем, и адрес и путь к ключу.
      var settingsString = Encryption.Decrypt(settings.ConnectionParams);
      var connectionString = settingsString.Split(';');
      if (string.IsNullOrEmpty(connectionString[0]))
        throw new Exception("Отсутствует строка подключения.");
      
      if (string.IsNullOrEmpty(connectionString[1]))
        throw new Exception("Отутствует путь к ключу.");
      
      // Подключаемся и отправляем запрос в NATS.
      var connectionFactory = new ConnectionFactory();
      var connection = connectionFactory.CreateConnection(connectionString[0], connectionString[1]);
      connection.Publish(args.Subject, System.Text.Encoding.UTF8.GetBytes(args.Data));
      // Закрываем соединение
      connection.Close();
    }
    #endregion
    
    #region Остальное
    /// <summary>
    /// Отправка уведомления администраторам СЭД.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncAdminEDMSNotificationavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncAdminEDMSNotificationavisInvokeArgs args)
    {
      try
      {
        var administratorEDMSRole = Roles.GetAll(r => r.Sid == lenspec.EtalonDatabooks.PublicConstants.Module.AdministratorEDMS).FirstOrDefault();
        
        // Создаём и стартуем задачу с уведомлением.
        var task= Sungero.Workflow.SimpleTasks.CreateWithNotices(args.Subject, administratorEDMSRole);
        task.ActiveText = args.ActiveText;
        
        // Получаем клиентский договор.
        var clientContract = SalesDepartmentArchive.SDAClientContracts.GetAll(c => c.Id == args.SDAClientContractId).FirstOrDefault();
        task.Attachments.Add(clientContract);
        
        task.Start();
      }
      catch(Exception ex)
      {
        Logger.Error("Avis AsyncAdminEDMSNotificationavis - ", ex);
      }
    }
    
    public virtual void AsyncUpdateObjectAnProjectavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncUpdateObjectAnProjectavisInvokeArgs args)
    {
      // Ищем настройки подключения.
      var setting = lenspec.Etalon.Module.Integration.Server.ModuleFunctions.GetIntegrationKFSettingsRec(Constants.Module.InvestConnectRabbitMQCode);
      
      if (setting == null || string.IsNullOrEmpty(setting.ConnectionParams))
      {
        Logger.DebugFormat("AsyncEditOurCFavis: Не найдены настройки подключения к инвест.");
        return;
      }
      
      // Получаем параметры подключения к инвест.
      var settingString = Encryption.Decrypt(setting.ConnectionParams);
      var settings = settingString.Split('|');
      if (settings.Length != 4)
      {
        Logger.DebugFormat("AsyncEditOurCFavis: Не все настройки указаны в строке подключения.");
        return;
      }
      
      // Создаём сервис.
      var rabbitMq = new RabbitIntegrationHelper.RabbitMqService(settings[0], settings[1], settings[2], settings[3]);
      // Создаём объект с информацией об изменениях.
      var body = new RabbitIntegrationHelper.Models.DrxPibstatusRqstModel();
      body.drxobj_id = args.IdInvest;
      body.message_id = args.EmployeeId;
      
      // Создаём базовый объект.
      var request = new RabbitIntegrationHelper.Models.RequestModel("drx_pibstatus_rqst", body);
      
      // Отправляем пакет.
      rabbitMq.SendMessage(request);
    }

    public virtual void AsyncUpdateObjavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncUpdateObjavisInvokeArgs args)
    {
      var objId = args.ObjId;
      var avisCode = args.AvisCode;
      
      var obct =  lenspec.EtalonDatabooks.ObjectAnProjects.GetAll(x => x.Id == objId).FirstOrDefault();
      if (obct != null)
      {
        try
        {
          obct.IdInvest = avisCode;
          obct.Save();
        }
        catch
        {
          args.Retry = true;
        }
      }
    }

    public virtual void AsyncUpdatePrjavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncUpdatePrjavisInvokeArgs args)
    {
      var prjId = args.PrjId;
      var avisCode = args.AvisCode;
      
      var prj = lenspec.EtalonDatabooks.OurCFs.GetAll(x => x.Id == prjId).FirstOrDefault();
      if (prj != null)
      {
        try
        {
          prj.IdInvest = avisCode;
          prj.Save();
        }
        catch
        {
          args.Retry = true;
        }
      }
    }
    
    /// <summary>
    /// Отправить запрос на обновление данных клиента инвест.
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncUpdateClientavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncUpdateClientavisInvokeArgs args)
    {
      // Ищем настройки подключения.
      var setting = lenspec.Etalon.Module.Integration.Server.ModuleFunctions.GetIntegrationKFSettingsRec(Constants.Module.InvestConnectRabbitMQCode);
      
      if (setting == null || string.IsNullOrEmpty(setting.ConnectionParams))
      {
        Logger.DebugFormat("AsyncEditOurCFavis: Не найдены настройки подключения к инвест.");
        return;
      }
      
      // Получаем параметры подключения к инвест.
      var settingString = Encryption.Decrypt(setting.ConnectionParams);
      var settings = settingString.Split('|');
      if (settings.Length != 4)
      {
        Logger.DebugFormat("AsyncEditOurCFavis: Не все настройки указаны в строке подключения.");
        return;
      }
      
      // Создаём сервис.
      var rabbitMq = new RabbitIntegrationHelper.RabbitMqService(settings[0], settings[1], settings[2], settings[3]);
      // Создаём объект с информацией об изменениях.
      var body = new RabbitIntegrationHelper.Models.DrxClientRqstModel();
      body.dp_id = args.IdInvest;
      
      // Создаём базовый объект.
      var request = new RabbitIntegrationHelper.Models.RequestModel("drx_client_rqst", body);
      
      // Отправляем пакет.
      rabbitMq.SendMessage(request);
    }
    
    /// <summary>
    /// Отправка изменений в карточке "Объект проекта".
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncEditObjectAnProjectvisavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncEditObjectAnProjectvisavisInvokeArgs args)
    {
      // Ищем настройки подключения.
      var setting = lenspec.Etalon.Module.Integration.Server.ModuleFunctions.GetIntegrationKFSettingsRec(Constants.Module.InvestConnectRabbitMQCode);
      
      if (setting == null || string.IsNullOrEmpty(setting.ConnectionParams))
      {
        Logger.DebugFormat("AsyncEditObjectAnProjectvisavis: Не найдены настройки подключения к инвест.");
        return;
      }
      
      // Получаем параметры подключения к инвест.
      var settingString = Encryption.Decrypt(setting.ConnectionParams);
      var settings = settingString.Split('|');
      if (settings.Length != 4)
      {
        Logger.DebugFormat("AsyncEditObjectAnProjectvisavis: Не все настройки указаны в строке подключения.");
        return;
      }
      
      // Создаём сервис.
      var rabbitMq = new RabbitIntegrationHelper.RabbitMqService(settings[0], settings[1], settings[2], settings[3]);
      // Создаём объект с информацией об изменениях.
      var body = new RabbitIntegrationHelper.Models.DrxObjectUpdtModel();
      body.object_id = args.Id;
      body.project_id = args.IdOurCF;
      body.developer_title = args.SpecDeveloperName;
      body.developer_inn = args.SpecDeveloperInn;
      body.drxobj_id = args.IdInvest;
      body.object_title = args.Name;
      body.object_saddress = args.AddressBuild;
      body.rns_id = args.IdBuildingPermit;
      body.rns_number = args.NumberRNS;
      //body.rns_dt = args.DateRNS;
      body.rnv_id = args.IdEnterAnObjectPermit;
      body.rnv_number = args.NumberRNV;
      //body.rnv_dt = args.DateRNV;
      body.rnv_title = args.NameRNV;
      body.rnv_address = args.AddressRNV;
      
      // Проверка дат.
      //Дата для проверки, что значение не 01 01 0001
      var dateCheck = new DateTime(1900, 1, 1);
      
      if (args.DateRNS > dateCheck)
        body.rns_dt = args.DateRNS.ToString("yyyy-MM-dd");
      
      if (args.DateRNV > dateCheck)
        body.rnv_dt = args.DateRNV.ToString("yyyy-MM-dd");
      
      // Создаём базовый объект.
      var request = new RabbitIntegrationHelper.Models.RequestModel("drx_object_updt", body);
      
      // Отправляем пакет.
      rabbitMq.SendMessage(request);
    }
    
    /// <summary>
    /// Отправка изменений в карточке "Инвестиционно-строительный проект".
    /// </summary>
    /// <param name="args"></param>
    public virtual void AsyncEditOurCFavis(lenspec.Etalon.Module.Integration.Server.AsyncHandlerInvokeArgs.AsyncEditOurCFavisInvokeArgs args)
    {
      // Ищем настройки подключения.
      var setting = lenspec.Etalon.Module.Integration.Server.ModuleFunctions.GetIntegrationKFSettingsRec(Constants.Module.InvestConnectRabbitMQCode);
      
      if (setting == null || string.IsNullOrEmpty(setting.ConnectionParams))
      {
        Logger.DebugFormat("AsyncEditOurCFavis: Не найдены настройки подключения к инвест.");
        return;
      }
      
      // Получаем параметры подключения к инвест.
      var settingString = Encryption.Decrypt(setting.ConnectionParams);
      var settings = settingString.Split('|');
      if (settings.Length != 4)
      {
        Logger.DebugFormat("AsyncEditOurCFavis: Не все настройки указаны в строке подключения.");
        return;
      }
      
      // Создаём сервис.
      var rabbitMq = new RabbitIntegrationHelper.RabbitMqService(settings[0], settings[1], settings[2], settings[3]);
      // Создаём объект с информацией об изменениях.
      var body = new RabbitIntegrationHelper.Models.DrxProjectUpdtModel();
      body.project_id = args.Id;
      body.dpxprj_id = args.IdInvest;
      body.project_title = args.CommercialName;
      
      // Создаём базовый объект.
      var request = new RabbitIntegrationHelper.Models.RequestModel("drx_project_updt", body);
      
      // Отправляем пакет.
      rabbitMq.SendMessage(request);
    }
    #endregion
  }
  // Конец добавлено avis.
}