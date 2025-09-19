using System;
using Sungero.Core;

namespace avis.EtalonContracts.Constants
{
  public static class Module
  {
    /// <summary>
    /// GUID роли "Ответственные за справочники договорных документов".
    /// </summary>
    [Public]
    public static readonly Guid ResponsibleContracDocuments = Guid.Parse("D9566917-F143-4192-A4CE-4A9A268C7930");
    
    /// <summary>
    /// GUID Тип документа "Приложение к договорному документу".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid AttachmentContractDocumentTypeGuid = Guid.Parse("77eb61a9-037f-4baa-ac26-b362338a9b44");
    
    /// <summary>
    /// GUID  Вид документа "Приложение к договорному документу".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid AttachmentDocumentKindGuid = Guid.Parse("1C9316A3-4199-44D3-A6FD-65DDFAD5CA93");
    
    /// <summary>
    /// GUID  Вид документа "Протокол разногласий".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ProtocolDisagreementKindGuid = Guid.Parse("3C7B78FC-2BB9-42B4-9AE8-01DB49511747");
    
    /// <summary>
    /// GUID роли "Права на выгрузку в 1С договорных документов".
    /// </summary>
    [Public]
    public static readonly Guid UploadingContractsInto1C = Guid.Parse("FFCDF62C-4FCF-4389-A85C-16B799689DCE");
    
    /// <summary>
    /// Код записи справочника настроек интеграции для KindsWorkTypes (Виды работ)
    /// </summary>
    [Public]
    public const string KindsWorkTypesCode = "KindsWorkTypes";
    
    /// <summary>
    /// Код записи справочника настроек интеграции для WorkTypes (Детализация видов работ)
    /// </summary>
    [Public]
    public const string WorkTypesCode = "WorkTypes";
    
    /// <summary>
    /// GUID роли "Права на проставление признака «Бюджет сформирован» в Подразделения".
    /// </summary>
    [Public]
    public static readonly Guid FillingBudgetFormed = Guid.Parse("456FD762-D747-4432-AA23-5F2F4E6BDF8B");
    
    /// <summary>
    /// GUID роли "Все сотрудники дирекции по правовым вопросам".
    /// </summary>
    [Public]
    public static readonly Guid AllEmployeesDirectionLegalIssues = Guid.Parse("9EB6EC89-0E3E-4812-88DF-001D72A0AC47");
    
    /// <summary>
    /// GUID роли "Полные права на договорные документы для редактирования карточек".
    /// </summary>
    [Public]
    public static readonly Guid FullAcessContractualDocuments = Guid.Parse("342E173E-C863-47B4-A017-382249643BCB");
    
    /// <summary>
    /// GUID роли "Права на отчет по использованию шаблонов ДД".
    /// </summary>
    [Public]
    public static readonly Guid RightsToReportForContractTemplates = Guid.Parse("0A81CA5B-4D79-48FD-BB97-B2089DC12908");
  }
}