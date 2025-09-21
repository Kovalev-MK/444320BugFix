using System;
using Sungero.Core;

namespace lenspec.Tenders.Constants
{
  public static class Module
  {

    //Добавлено Avis Expert
    
    #region Роли
    
    /// <summary>
    ///  GUID роли "Права на просмотр всех протоколов тендерного комитета".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid TenderCommitteeProtocolReadingRole = Guid.Parse("1DE424F6-5B32-4657-A05F-834A223B0164");
    
    /// <summary>
    ///  GUID роли "Права на создание протоколов тендерного комитета".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid TenderCommitteeProtocolCreatingRole = Guid.Parse("DEACCEEA-5ABC-40D7-A908-6C570130317C");
    
    /// <summary>
    ///  GUID роли "Полные права на справочник Тендерные комитеты".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid TenderCommitteeAccessRole = Guid.Parse("56DB624F-DE73-4044-878E-62F8246D127D");
    
    /// <summary>
    ///  GUID роли "Права на вложение сканов протоколов тендерного комитета".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ScanOfTenderCommitteeProtocolCreatingRole = Guid.Parse("7FBA10AF-BC8D-4FDB-9018-A95982516A15");
    
    /// <summary>
    ///  GUID роли "Права на просмотр тендерной документации".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid TenderDocumentsReadingRoleRole = Guid.Parse("5420FDC0-8CDF-4C56-B5B8-3B88C9EE7149");
    
    /// <summary>
    ///  GUID роли "Права на просмотр всех протоколов комитета аккредитации".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid AccreditationCommitteeProtocolReadingRole = Guid.Parse("DDD3FACF-51F8-4193-A2DE-51B693635972");
    
    /// <summary>
    ///  GUID роли "Ответственные за квалификацию и дисквалификацию контрагентов".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ResponsibleForCounterpartyQualificationRole = Guid.Parse("8304042D-67BD-4A9F-AC65-DF1A9506B938");
    
    #endregion
    
    #region Типы документов
    
    /// <summary>
    /// GUID типа документа "Тендерная документация".
    /// </summary>
    [Public]
    public const string TenderDocumentTypeGuid = "0ef286b4-b2c4-4b7e-a676-e309a0a8fb00";
    
    /// <summary>
    /// GUID типа документа "Протокол тендерного комитета".
    /// </summary>
    [Public]
    public const string TenderCommitteeProtocolTypeGuid = "c7c493ec-aae8-467f-b1c9-1148cb6bbee9";
    
    /// <summary>
    /// GUID типа документа "Анкета квалификации".
    /// </summary>
    [Public]
    public const string TenderQualificationFormTypeGuid = "46300a05-fbe4-4493-b2d6-9fd4170b984c";
    
    #endregion Типы документов
    
    #region Виды документов
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Протокол тендерного комитета».
    /// </summary>
    public static readonly Guid TenderCommitteeProtocolKind = Guid.Parse("E460FE59-58E0-47D0-9E46-0FFB6A37A9B3");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Сводная таблица коммерческих предложений».
    /// </summary>
    public static readonly Guid CommercialOffersSummaryKind = Guid.Parse("2FD25D4E-F16C-4A81-BF5E-4B54F1C8E883");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Протокол квалификационного отбора».
    /// </summary>
    public static readonly Guid QualificationSelectionProtocolKind = Guid.Parse("95B48BBA-1A82-44ED-9EFA-4E9ADE861EFB");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Анкета квалификации».
    /// </summary>
    public static readonly Guid TenderAccreditationFormKind = Guid.Parse("E384E1A5-0EF8-44A0-9424-EEB9B9D6EFC7");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Протокол комитета аккредитации».
    /// </summary>
    public static readonly Guid AccreditationCommitteeProtocolKind = Guid.Parse("834FA648-06CA-446F-8F58-DD767DB726F5");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Заявка на закупку».
    /// </summary>
    public static readonly Guid PurchaseRequisitionKind = Guid.Parse("5B65A77F-9FC0-458A-A92A-F4C8F6B7499D");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Решение комитета по квалификации включения контрагента в реестр».
    /// </summary>
    [Public]
    public static readonly Guid DecisionOnInclusionOfCounterpartyKind = Guid.Parse("AC746804-DE98-4EAE-896A-91F06483C568");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Решение комитета по квалификации исключения контрагента из реестра».
    /// </summary>
    [Public]
    public static readonly Guid DecisionOnExclusionOfCounterpartyKind = Guid.Parse("099B00DC-2D0A-4828-A295-6CD3CCB8C4B9");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Служебная записка на включение в реестр».
    /// </summary>
    public static readonly Guid MemoOnInclusionOfCounterpartyKind = Guid.Parse("9B55D915-CD9F-4166-9250-411668DBDBFC");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Служебная записка на исключение из реестра».
    /// </summary>
    public static readonly Guid MemoOnExclusionOfCounterpartyKind = Guid.Parse("A307DCCB-40D9-4176-9627-87789A18E985");
    
    #endregion
    
    public static class Params
    {
      /// Сообщение из задания на обработку результатов согласования на карточку реестра КА.
      public const string MessageToCPRegisterFromProcessingAssignment = "MessageToCPRegisterFromProcessingAssignment";
    }
    
    //конец Добавлено Avis Expert
  }
}