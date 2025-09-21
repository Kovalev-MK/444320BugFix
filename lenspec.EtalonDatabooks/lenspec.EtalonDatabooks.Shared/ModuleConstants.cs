using System;
using Sungero.Core;

namespace lenspec.EtalonDatabooks.Constants
{
  public static class Module
  {
    //Добавлено Avis Expert
    
    #region Строки подключения к API ФНС
    
    /// <summary>
    /// Строка подключения к API ФНС для отслеживания изменений в ЕГРЮЛ/ЕГРИП.
    /// </summary>
    [Public]
    public const string APIFnsCounterpartyChangesURL = "https://api-fns.ru/api/changes?key={0}&req={1}&dat={2}";
    
    /// <summary>
    /// Строка подключения к API ФНС для получения тела документа из ЕГРЮЛ.
    /// </summary>
    [Public]
    public const string APIFnsExcerptEGRULURL = "https://api-fns.ru/api/vyp?key={0}&req={1}";
    
    #endregion
    
    #region Параметры
    
    // Поле ИСП в карточке Объекта было изменено.
    [Sungero.Core.PublicAttribute]
    public const string ObjectAnProjectOurCFHasBeenChangedParam = "ObjectAnProjectOurCFHasBeenChanged";
    
    /// <summary>
    /// Ограничение на максимальный размер выборки клиентов и собственников для ИСП, Объектов проекта и Объектов продажи.
    /// </summary>
    [Public]
    public const int MaxClientsOwnersSelectionSize = 2000;
    
    #endregion
    
    #region Строки подключения к Контур.Фокус
    
    /// <summary>
    /// Строка подключения к Контур.Фокус для получения базовых реквизитов из ЕГРЮЛ.
    /// </summary>
    [Public]
    public const string KFBaseRequisitesConnectionString = "https://focus-api.kontur.ru/api3/req?inn={0}&key={1}";
    
    /// <summary>
    /// Строка подключения к Контур.Фокус для получения тела документа из ЕГРЮЛ.
    /// </summary>
    [Public]
    public const string KFExcerptEGRULConnectionString = "https://focus-api.kontur.ru/api3/excerpt?key={0}&inn={1}&pdf=False";
    
    #endregion
    
    #region Типы документов
    
    // GUID типа документа "Заявка на создание НОР".
    [Public]
    public const string ApplicationBUCreationTypeGuid = "c73edd42-4abe-4c8c-bb3a-740299a73bea";
    
    #endregion
    
    #region Виды документов
    
    // Уникальный идентификатор для вида «Заявка на создание Нашей организации».
    public static readonly Guid ApplicationBUCreationKind = Guid.Parse("f27b5dfa-2853-402d-8e20-d540fdb021a9");
    // Уникальный идентификатор для вида «Заявка на изменение Нашей организации».
    public static readonly Guid ApplicationBUEditingKind = Guid.Parse("0ba67a38-afde-4e77-875c-aebde65e9323");
    
    // Уникальный идентификатор для вида «Разрешение на ввод объекта».
    [Public]
    public static readonly Guid ObjectPermit = Guid.Parse("b84be5a9-5567-4d88-a3b2-227ac2f46be6");
    // Уникальный идентификатор для вида «Разрешение на строительство».
    [Public]
    public static readonly Guid BuildingPermit = Guid.Parse("7c66984c-4905-4b52-b670-d863e04d0251");
    
    #endregion
    
    #region Роли
    
    /// <summary>
    /// GUID роли "Администратор СЭД".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid AdministratorEDMS = Guid.Parse("4952D20F-6180-4E28-8CAE-364776E585D0");
    
    /// <summary>
    /// GUID роли "Полные права на справочник ИСП и документы РНС, РНВ".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid FullPermitISPAndRNSAndPNV = Guid.Parse("59ed33d7-7202-44ca-a8c8-cf1c0abe2b67");
    
    /// <summary>
    /// GUID роли "Канцелярия ГК".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid OfficeGK = Guid.Parse("59D87B25-0182-40B1-BDFD-5E2FDF27D0D8");
    
    /// <summary>
    /// GUID роли "Отображение папок потока заданий для канцелярии".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid FoldersOffice = Guid.Parse("188D4ED3-8A2F-490A-ADD2-46F6F8A21788");
    
    /// <summary>
    /// GUID роли "Права на поручения для канцелярий".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid OfficeAssignment = Guid.Parse("946CEDF1-7596-4B15-AA46-B14CD47C697E");
    
    /// <summary>
    /// GUID роли "Права на просмотр всех полей в Персонах (перс. данные)".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ViewPersonalData = Guid.Parse("099E38D3-E557-4B81-B842-BF4430FBA68D");
    
    /// <summary>
    /// GUID роли "Ответственный за Заполнение ИСП в Объектах проектов строительства из 1С".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ResponsibleFillingISP = Guid.Parse("7b26f17d-ca34-4f0f-a115-999e9f886e6e");
    
    /// <summary>
    /// GUID роли "Ответственные за внесение шаблонов договоров и доп.соглашений".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ResponsibleContractualTemplates = Guid.Parse("56F9DCEE-12BB-4887-A6C7-6276536560A3");
    
    /// <summary>
    /// GUID роли "Пользователи с правами на указание в документах сотрудников из любых НОР".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid RightsToSelectAnyEmployees = Guid.Parse("6426B093-3277-4CC7-A34D-F901634748F8");
    
    /// <summary>
    /// GUID роли "Права на сохранение Договорных документов с пустой таблицей Объекты строительства".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid RightsToSaveWithoutConstructionObjects = Guid.Parse("67A50E63-326F-451C-90E8-7CCABC85A49E");
    
    #endregion
    
    public static class FlowFoldersSetting
    {
      #region Типы задач
      
      /// <summary>
      /// GUID типа Задача на согласование по регламенту.
      /// </summary>
      [Public]
      public const string ApprovalTaskGuid = "100950d0-03d2-44f0-9e31-f9c8dfdf3829";
      
      /// <summary>
      /// GUID типа Контроль возврата документа.
      /// </summary>
      [Public]
      public const string CheckReturnTaskGuid = "af000bfc-7c6a-4308-883a-df6fe4ab7265";
      
      /// <summary>
      /// GUID типа Запрос на продление срока.
      /// </summary>
      [Public]
      public const string DocflowDeadlineExtensionTaskGuid = "ef92411f-9fd6-4009-8e8f-92c8a2419a0c";
      
      /// <summary>
      /// GUID типа Задача на свободное согласование.
      /// </summary>
      [Public]
      public const string FreeApprovalTaskGuid = "77f43035-9f23-4a19-9882-5a6a2cd5c9c7";
      
      /// <summary>
      /// GUID типа Задача на обработку входящих документов эл. обмена.
      /// </summary>
      [Public]
      public const string ExchangeDocumentProcessingTaskGuid = "ae03c598-ab50-4781-b1b2-968510b338b9";
      
      /// <summary>
      /// GUID типа Задача на отправку извещений о получении документов.
      /// </summary>
      [Public]
      public const string ReceiptNotificationSendingTaskGuid = "490e66a0-8618-4eb9-aab8-2dbc8a884d98";
      
      /// <summary>
      /// GUID типа Задача на обработку конфликтов синхронизации контрагентов.
      /// </summary>
      [Public]
      public const string CounterpartyConflictProcessingTaskGuid = "03a51b42-a322-4574-90bb-212ea03ed71e";
      
      /// <summary>
      /// GUID типа Задача на обработку приглашения к эл. обмену от контрагента.
      /// </summary>
      [Public]
      public const string IncomingInvitationTaskGuid = "1e5b11de-bd28-4dc2-a03c-74b8db9ac1c4";
      
      /// <summary>
      /// GUID типа Задача на ознакомление с документом.
      /// </summary>
      [Public]
      public const string AcquaintanceTaskGuid = "2d53959b-2cee-41f7-83c2-98ae1dbbd538";
      
      /// <summary>
      /// GUID типа Задача на исполнение поручения.
      /// </summary>
      [Public]
      public const string ActionItemExecutionTaskGuid = "c290b098-12c7-487d-bb38-73e2c98f9789";
      
      /// <summary>
      /// GUID типа Продление срока.
      /// </summary>
      [Public]
      public const string RecordManagmentDeadlineExtensionTaskGuid = "67b46acc-07a9-43ed-86dc-8f9dc3ccf12f";
      
      /// <summary>
      /// GUID типа Задача на рассмотрение документа.
      /// </summary>
      [Public]
      public const string DocumentReviewTaskGuid = "4ef03457-8b42-4239-a3c5-d4d05e61f0b6";
      
      /// <summary>
      /// GUID типа Запрос отчета по поручению.
      /// </summary>
      [Public]
      public const string StatusReportRequestTaskGuid = "c8aed854-ad26-4ee3-88a3-080bc98c12e1";
      
      /// <summary>
      /// GUID типа Задача на верификацию комплекта документов.
      /// </summary>
      [Public]
      public const string VerificationTaskGuid = "999a5ae0-17ec-4735-bc90-d85c7fe08dd3";
      
      /// <summary>
      /// GUID типа Заявка на изменение компонентов Directum RX.
      /// </summary>
      [Public]
      public const string EditComponentRXRequestTaskGuid = "156cd6e7-ebca-41f1-aafa-dc18f595de9a";
      
      /// <summary>
      /// GUID типа Заявка на формирование замещения.
      /// </summary>
      [Public]
      public const string SubstitutionRequestTaskGuid = "b5d3d91e-1cb1-4c86-8a82-ee997a8b781b";
      
      /// <summary>
      /// GUID типа Заявка на редактирование справочника Контрагенты.
      /// </summary>
      [Public]
      public const string CreateCompanyTaskGuid = "dc31c215-c0f1-48e6-89ce-38fdf24f9c63";
      
      /// <summary>
      /// GUID типа Задача на оформление доверенности.
      /// </summary>
      [Public]
      public const string ExecutionPowerOfAttorneyTaskGuid = "1487899c-8974-4d83-bcdf-210aedbb2f1a";
      
      /// <summary>
      /// GUID типа Простая задача.
      /// </summary>
      [Public]
      public const string SimpleTaskGuid = "83F2A537-0CF0-4429-AE76-E9A386CA53AA";
      
      /// <summary>
      /// GUID типа Задача на согласование включения в реестр квалифицированных контрагентов/список дисквалифицированных контрагентов.
      /// </summary>
      [Public]
      public const string ApprovalCounterpartyRegisterTaskGuid = "2ec894f8-d7da-46be-a14f-36d51722b0b5";
      
      #endregion
      
      #region Типы заданий
      
      #region Согласование по регламенту
      /// <summary>
      /// GUID типа задания Согласование.
      /// </summary>
      [Public]
      public const string ApprovalAssignmentGuid = "daf1900f-e66b-4368-b724-a073266145d7";
      
      /// <summary>
      /// GUID типа задания Задание (с возможностью доработки).
      /// </summary>
      [Public]
      public const string ApprovalCheckingAssignmentGuid = "c09f0ae4-c959-4a57-9895-ae9aaf1f1855";
      
      /// <summary>
      /// GUID типа задания Контроль возврата.
      /// </summary>
      [Public]
      public const string ApprovalCheckReturnAssignmentGuid = "f40f1e48-b0f7-4c53-9658-ebefa07fe094";
      
      /// <summary>
      /// GUID типа задания Создание поручений.
      /// </summary>
      [Public]
      public const string ApprovalExecutionAssignmentGuid = "495600a5-5f7a-49aa-ac49-9351c9af1109";
      
      /// <summary>
      /// GUID типа задания Согласование руководителем.
      /// </summary>
      [Public]
      public const string ApprovalManagerAssignmentGuid = "bbb08f45-60c1-4496-9ff6-b32caed44215";
      
      /// <summary>
      /// GUID типа задания Печать документа.
      /// </summary>
      [Public]
      public const string ApprovalPrintingAssignmentGuid = "8cd7f587-a910-4e2f-ac4f-afcc15fc3e2f";
      
      /// <summary>
      /// GUID типа задания Регистрация.
      /// </summary>
      [Public]
      public const string ApprovalRegistrationAssignmentGuid = "a3b19bde-a0a5-4c7b-9ad4-5a7e800156a9";
      
      /// <summary>
      /// GUID типа задания Рассмотрение.
      /// </summary>
      [Public]
      public const string ApprovalReviewAssignmentGuid = "079b6ce1-8a62-41a6-aa89-0de5e5266253";
      
      /// <summary>
      /// GUID типа задания Доработка.
      /// </summary>
      [Public]
      public const string ApprovalReworkAssignmentGuid = "040862cd-a46f-4366-b068-e659c7acaea6";
      
      /// <summary>
      /// GUID типа задания Отправка контрагенту.
      /// </summary>
      [Public]
      public const string ApprovalSendingAssignmentGuid = "5d86a6e4-ae51-497a-9122-8a812eba0fc7";
      
      /// <summary>
      /// GUID типа задания Подписание.
      /// </summary>
      [Public]
      public const string ApprovalSigningAssignmentGuid = "db516acc-0f02-4ea7-960a-08f3f734db4f";
      
      /// <summary>
      /// GUID типа задания Задание (без возможности доработки).
      /// </summary>
      [Public]
      public const string ApprovalSimpleAssignmentGuid = "b0931934-7981-4139-a398-f0e39abbb981";
      
      /// <summary>
      /// GUID типа задания Уведомление согласующим о том, что документ был повторно отправлен на согласование.
      /// </summary>
      [Public]
      public const string ApprovalNotificationGuid = "be83f002-c2c0-49da-aff5-b7f3bddaabf7";
      
      /// <summary>
      /// GUID типа задания Уведомление по документу с предопределенной темой.
      /// </summary>
      [Public]
      public const string ApprovalSimpleNotificationGuid = "32ce5b61-1be2-4d61-b98a-37b99aff3560";
      #endregion
      
      #region Контроль возврата документа
      /// <summary>
      /// GUID типа задания Возврат документа.
      /// </summary>
      [Public]
      public const string CheckReturnAssignmentGuid = "c9cff422-0936-4dd4-906d-a2fd487e5c2f";
      
      /// <summary>
      /// GUID типа задания Контроль возврата документа.
      /// </summary>
      [Public]
      public const string CheckReturnCheckAssignmentGuid = "0d7f53bd-74bd-42d5-93aa-188ac51e5852";
      #endregion
      
      #region Запрос на продление срока
      /// <summary>
      /// GUID типа задания Продление срока.
      /// </summary>
      [Public]
      public const string DeadlineExtensionAssignmentGuid = "47f07044-beda-4949-b348-d2afa52ab4ba";
      
      /// <summary>
      /// GUID типа задания Отказ в продлении срока.
      /// </summary>
      [Public]
      public const string DeadlineRejectionAssignmentGuid = "6d840402-fdbb-4e90-8f2b-1c7950ccccdc";
      
      /// <summary>
      /// GUID типа задания Уведомление о продлении срока.
      /// </summary>
      [Public]
      public const string DeadlineExtensionNotificationGuid = "3dad0441-cd89-4928-b6ff-9b7dd7fc20cf";
      #endregion
      
      #region Свободное согласование
      /// <summary>
      /// GUID типа задания Согласование.
      /// </summary>
      [Public]
      public const string FreeApprovalAssignmentGuid = "01be6c28-8785-4f74-9877-e3270704452e";
      
      /// <summary>
      /// GUID типа задания Завершение согласования.
      /// </summary>
      [Public]
      public const string FreeApprovalFinishAssignmentGuid = "d3277007-c49f-4aaa-a0f7-e397fa4fb9fc";
      
      /// <summary>
      /// GUID типа задания Доработка.
      /// </summary>
      [Public]
      public const string FreeApprovalReworkAssignmentGuid = "593ff79e-38b4-4903-b20e-9e08cfea6307";
      
      /// <summary>
      /// GUID типа задания Уведомление.
      /// </summary>
      [Public]
      public const string FreeApprovalNotificationGuid = "7d9816f6-1dea-4718-84a5-1a413df81145";
      #endregion
      
      #region Задача на обработку входящих документов эл. обмена
      /// <summary>
      /// GUID типа задания Обработка документов.
      /// </summary>
      [Public]
      public const string ExchangeDocumentProcessingAssignmentGuid = "0917ca0b-898f-429d-be37-c12da50c5a95";
      #endregion
      
      #region Задача на отправку извещений о получении документов
      /// <summary>
      /// GUID типа задания Отправка извещений.
      /// </summary>
      [Public]
      public const string ReceiptNotificationSendingAssignmentGuid = "ad88c31d-5ae1-4821-9615-7d2fa575726b";
      #endregion
      
      #region Задача на обработку конфликтов синхронизации контрагентов
      /// <summary>
      /// GUID типа задания Обработка конфликтов синхронизации.
      /// </summary>
      [Public]
      public const string CounterpartyConflictProcessingAssignmentGuid = "d4870243-705a-483a-bcc2-89e53b80073f";
      #endregion
      
      #region Задача на обработку приглашения к эл. обмену от контрагента
      /// <summary>
      /// GUID типа задания Обработка приглашения к эл. обмену.
      /// </summary>
      [Public]
      public const string IncomingInvitationAssignmentGuid = "cf5edde0-03df-4db3-9e1e-15b22cf3198c";
      #endregion
      
      #region Задача на ознакомление с документом
      /// <summary>
      /// GUID типа задания Ознакомление.
      /// </summary>
      [Public]
      public const string AcquaintanceAssignmentGuid = "8fee99ee-b3fd-49dd-9b48-e51b83597227";
      
      /// <summary>
      /// GUID типа задания Завершение работ по ознакомлению.
      /// </summary>
      [Public]
      public const string AcquaintanceFinishAssignmentGuid = "e04a433b-5b48-40c2-993a-41370b9ebb8a";
      
      /// <summary>
      /// GUID типа задания Уведомление о завершении ознакомления.
      /// </summary>
      [Public]
      public const string AcquaintanceCompleteNotificationGuid = "8484c5ba-5646-4327-9158-cd9fe0eb082b";
      #endregion
      
      #region Задача на исполнение поручения
      /// <summary>
      /// GUID типа задания Исполнение поручения.
      /// </summary>
      [Public]
      public const string ActionItemExecutionAssignmentGuid = "d238ef51-607e-46a5-b86a-ede4482f7f19";
      
      /// <summary>
      /// GUID типа задания Приемка работ контролером.
      /// </summary>
      [Public]
      public const string ActionItemSupervisorAssignmentGuid = "f44faafc-cd55-4c5b-b16d-93b6fc966ffb";
      
      /// <summary>
      /// GUID типа задания Уведомление о приемке работ.
      /// </summary>
      [Public]
      public const string ActionItemExecutionNotificationGuid = "dfe429f9-8bbe-4920-b911-fe278031315a";
      
      /// <summary>
      /// GUID типа задания Уведомление о создании поручения.
      /// </summary>
      [Public]
      public const string ActionItemObserversNotificationGuid = "ab194340-550c-41c7-badd-adc2ee208741";
      
      /// <summary>
      /// GUID типа задания Уведомление контролеру.
      /// </summary>
      [Public]
      public const string ActionItemSupervisorNotificationGuid = "da4946ab-6876-4bae-b35b-fc63e76974fd";
      #endregion
      
      #region Продление срока (RM)
      /// <summary>
      /// GUID типа задания Продление срока.
      /// </summary>
      [Public]
      public const string RMDeadlineExtensionAssignmentGuid = "8a5b9f7b-0c69-455f-95b2-8b525b4e7bb1";
      
      /// <summary>
      /// GUID типа задания Отказ в продлении срока.
      /// </summary>
      [Public]
      public const string RMDeadlineRejectionAssignmentGuid = "dea721dd-12f9-498a-93e9-451a1fbfad67";
      
      /// <summary>
      /// GUID типа задания Уведомление о продлении срока.
      /// </summary>
      [Public]
      public const string RMDeadlineExtensionNotificationGuid = "db855988-d42a-4cc5-a610-afff7ffb9560";
      #endregion
      
      #region Задача на рассмотрение документа
      /// <summary>
      /// GUID типа задания Подготовка проекта резолюции.
      /// </summary>
      [Public]
      public const string PreparingDraftResolutionAssignmentGuid = "7cca016a-80f0-4562-9042-57bb748d5b30";
      
      /// <summary>
      /// GUID типа задания Рассмотрение руководителем (с подготовленным проектом резолюции).
      /// </summary>
      [Public]
      public const string ReviewDraftResolutionAssignmentGuid = "e2dd5bf3-54c8-4846-b158-9c42d09fbc33";
      
      /// <summary>
      /// GUID типа задания Рассмотрение руководителем (без проекта резолюции).
      /// </summary>
      [Public]
      public const string ReviewManagerAssignmentGuid = "69ac657a-0e74-46be-acba-f6bbbbd2bc73";
      
      /// <summary>
      /// GUID типа задания Обработка резолюции.
      /// </summary>
      [Public]
      public const string ReviewResolutionAssignmentGuid = "018e582e-5b0e-4e4f-af57-be1e0a468efa";
      
      /// <summary>
      /// GUID типа задания Доработка инициатором.
      /// </summary>
      [Public]
      public const string ReviewReworkAssignmentGuid = "1d5433e5-b285-4310-9a63-fc4e76f0a9b7";
      
      /// <summary>
      /// GUID типа задания Уведомление делопроизводителю.
      /// </summary>
      [Public]
      public const string ReviewClerkNotificationGuid = "4ca82b63-1b51-4aef-a42b-57b7c97dab64";
      
      /// <summary>
      /// GUID типа задания Уведомление наблюдателю.
      /// </summary>
      [Public]
      public const string ReviewObserverNotificationGuid = "75d6c458-7725-4133-8ff4-848e16ec5bd3";
      
      /// <summary>
      /// GUID типа задания Уведомление о рассмотрении.
      /// </summary>
      [Public]
      public const string ReviewObserversNotificationGuid = "8724ada0-fa56-481d-b3d0-ae3e46ff3e31";
      #endregion
      
      #region Запрос отчета по поручению
      /// <summary>
      /// GUID типа задания Подготовка отчета по поручению.
      /// </summary>
      [Public]
      public const string ReportRequestAssignmentGuid = "3c8d9436-3368-4dfc-9154-a125b95c600e";
      
      /// <summary>
      /// GUID типа задания Проверка отчета по поручению.
      /// </summary>
      [Public]
      public const string ReportRequestCheckAssignmentGuid = "5f794b11-e700-4fba-9021-39006a567729";
      #endregion
      
      #region Задача на верификацию комплекта документов
      /// <summary>
      /// GUID типа задания Верификация комплекта.
      /// </summary>
      [Public]
      public const string VerificationAssignmentGuid = "e825fc6a-c82b-4b89-a9fc-33fb181cb161";
      #endregion
      
      #region Заявка на изменение компонентов Directum RX
      /// <summary>
      /// GUID типа задания Задание.
      /// </summary>
      [Public]
      public const string ApprovalAdministratorGuid = "8e236014-f003-45ad-a813-48aa766cae48";
      
      /// <summary>
      /// GUID типа задания Задание.
      /// </summary>
      [Public]
      public const string ApprovalManagerGuid = "a23d92ea-681a-45b1-af37-c68e1d9130ea";
      #endregion
      
      #region Заявка на формирование замещения
      /// <summary>
      /// GUID типа задания Согласование.
      /// </summary>
      [Public]
      public const string ApprovalSubstitutionAssignmentGuid = "ec7fe4b9-1bfa-4f77-ad22-b9a3d5661038";
      
      /// <summary>
      /// GUID типа задания Уведомление.
      /// </summary>
      [Public]
      public const string SubstitutionRequestNotificationGuid = "4492e85e-3f66-4460-aeeb-5ca47e33b3ee";
      #endregion
      
      #region Заявка на редактирование справочника Контрагенты
      /// <summary>
      /// GUID типа задания Согласуйте.
      /// </summary>
      [Public]
      public const string ApproveCounterpartyAssignmentGuid = "9257ab7d-f433-41a0-bfbd-38463133f172";
      
      /// <summary>
      /// GUID типа задания Задание на доработку.
      /// </summary>
      [Public]
      public const string ApproveRevisionGuid = "2819c96b-cbf7-430c-9ccc-a65de2758317";
      #endregion
      
      #region Задача на оформление доверенности
      
      /// <summary>
      /// GUID типа задания Задание на согласование с руководителем (доверки)
      /// </summary>
      [Public]
      public const string ApprovalManagerInitiatorAssignmentGuid = "1a98eaed-22a1-4a59-9a04-c4ed5964c6a4";
      
      /// <summary>
      /// GUID типа задания Задание на оформление доверенности
      /// </summary>
      [Public]
      public const string ExecutionLawyerAssignmentGuid = "035f5310-908d-4c13-9c09-fc4cb3cc36bb";
      
      /// <summary>
      /// GUID типа задания Задание на выдачу оригинала
      /// </summary>
      [Public]
      public const string IssuanceOriginalAssignmentGuid = "f2e75f84-5044-4f11-8712-252ec1ccb66d";
      
      /// <summary>
      /// GUID типа задания Задание на печать и подписание
      /// </summary>
      [Public]
      public const string PrintAndSigningAssignmentGuid = "f73a2758-3c78-4b49-8a5f-e5ea5a514913";
      
      /// <summary>
      /// GUID типа задания Задание на регистрацию
      /// </summary>
      [Public]
      public const string RegistrationPowerOfAttorneyAssignmentGuid = "b443d004-3ce9-409d-a110-5bb956394e52";
      
      /// <summary>
      /// GUID типа задания Задание на доработку.
      /// </summary>
      [Public]
      public const string ReworkAssignmentGuid = "4c38f2e6-59db-4608-b4e2-ec1e8815946e";
      
      /// <summary>
      /// GUID типа задания Задание на сканирование
      /// </summary>
      [Public]
      public const string ScanningAssignmentGuid = "1b803104-02ae-4e52-916d-8afaaa24a57f";
      
      /// <summary>
      /// GUID типа задания Задание на настройку правил подписания
      /// </summary>
      [Public]
      public const string SettingUpAssignmentGuid = "f1d6f88c-050e-469e-904f-80dd2b3b2feb";
      
      /// <summary>
      /// GUID типа задания Уведомление о выдачи доверенности
      /// </summary>
      [Public]
      public const string NoticeIssuanceOriginalGuid = "958f94d3-1dcb-4d09-a0ca-260d184f7c92";
      
      #endregion
      
      #region Простая задача
      
      /// <summary>
      /// GUID типа задания Простое задание
      /// </summary>
      [Public]
      public const string SimpleAssignmentGuid = "90DAECB7-5D5D-465E-95A4-3235B8C01D5B";
      
      #endregion
      
      #region Задача на согласование включения в реестр квалифицированных контрагентов/список дисквалифицированных контрагентов
      
      /// <summary>
      /// GUID типа задания Задание на согласование с комитетом
      /// </summary>
      [Public]
      public const string CommitteeApprovalAssignmentGuid = "02c231f9-6297-4476-8d52-f213a907d215";
      
      /// <summary>
      /// GUID типа задания Задание на обработку результатов рассмотрения включения в реестр
      /// </summary>
      [Public]
      public const string ProcessingOfApprovalResultsAssignmentGuid = "2cf20e9c-c400-4b9e-9b1d-ac10d86fce37";
      
      #endregion
      
      #endregion
    }
    //конец Добавлено Avis Expert
  }
}