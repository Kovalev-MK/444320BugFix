using System;
using Sungero.Core;

namespace lenspec.SalesDepartmentArchive.Constants
{
  public static class Module
  {

    //Добавлено Avis Expert
    
    #region Роли
    
    /// <summary>
    ///  GUID роли "Полные права на документы УК".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid SalesDepartmentArchiveResponsible = Guid.Parse("5AD48902-F20E-4C5A-97E5-665B99998258");
    
    /// <summary>
    ///  GUID роли "Полные права на акты передачи квартир покупателям".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid FullRightsActsApartmentsBuyers = Guid.Parse("057648DE-2258-4535-A27F-42C4011C8B73");
    
     /// <summary>
    ///  GUID роли "Полные права на акты управляющих компаний".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid FullRightsActsOfManagementCompany = Guid.Parse("30066C80-4294-411E-B45E-3A459C3E5317");
    
     /// <summary>
    ///  GUID роли "Права на создание клиентских договоров".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ClientContractsCreatingRole = Guid.Parse("73508345-A996-4F7B-88B7-84527081BACE");
    
    #endregion
    
    
    #region Типы документов
    
    ///<summary>
    ///  GUID типа документа "Клиентский договор".
    /// </summary>
    [Public]
    public const string SDAClientContractTypeGuid = "c4f798a1-be37-4701-8036-f81e45f44e5d";
    
    /// <summary>
    ///  GUID типа документа "Клиентский документ".
    /// </summary>
    [Public]
    public const string SDAClientDocumentTypeGuid = "e97c6527-f4b1-4d4c-bb65-e1c64a0cede8";
    
    /// <summary>
    ///  Строковый GUID типа документа "Заявка на сдачу в архив".
    /// </summary>
    [Public]
    public const string SDARequestSubmissionToArchiveTypeGuid = "00c772b5-208f-4f79-8a9d-030f11fff827";
    
    /// <summary>
    ///  GUID типа документа "Заявка на сдачу в архив".
    /// </summary>
    [Public]
    public static readonly Guid SDARequestSubmissionToArchiveType = Guid.Parse("00c772b5-208f-4f79-8a9d-030f11fff827");
    
    /// <summary>
    ///  GUID типа документа "Заявка на выдачу из архив".
    /// </summary>
    [Public]
    public const string SDARequestIssuanceFromArchiveTypeGuid = "232d7d27-6d31-4c15-9f71-9625f7d26235";
    
    /// <summary>
    ///  GUID типа документа "Акты при приемке объектов долевого строительства".
    /// </summary>
    [Public] 
    public static readonly Guid ActAcceptanceOfApartmentTypeGuid = Guid.Parse("bfe4d2e9-04f4-45f5-b3a9-5cd2ceeb4634");
    
    /// <summary>
    ///  GUID типа документа "Акты в гарантийный период".
    /// </summary>
    [Public] 
    public static readonly Guid ActWarrantyPeriodTypeGuid = Guid.Parse("4df68695-0dbb-448d-818f-5413f90fa822");
    
    /// <summary>
    /// GUID типа документа "Акты управляющих компаний".
    /// </summary>
    [Public]
    public const string ActOfManagementCompanyGuid = "6cf094b7-0ac8-4c9e-ba93-7705406b3083";
    
    #endregion    
    
    
    #region Виды документов
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Клиентский договор».
    /// </summary>
    [Public]
    public static readonly Guid SDAClientContractKind = Guid.Parse("D6F034DD-E335-4DFA-8D2E-2DDE1776E0E8");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Доп. соглашение к клиентскому договору».
    /// </summary>
    public static readonly Guid SDAClientDocumentAgreementKind = Guid.Parse("ED132C2F-5488-4EE3-8AD4-154F0F84A4CE");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Приложение к клиентскому договору».
    /// </summary>
    public static readonly Guid SDAClientDocumentAddendumKind = Guid.Parse("1757B21D-F8A8-4FE6-AD8B-DC559BED0953");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Прочий документ к клиентскому договору».
    /// </summary>
    public static readonly Guid SDAClientDocumentOtherKind = Guid.Parse("F6A6C67C-EE70-46A1-9C71-AF504621077F");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Заявка на сдачу в архив».
    /// </summary>
    public static readonly Guid SDARequestSubmissionToArchiveKind = Guid.Parse("F815C9BA-9E57-4701-8369-24AA98937FEF");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Заявка на выдачу из архив».
    /// </summary>
    public static readonly Guid SDARequestIssuanceFromArchiveKind = Guid.Parse("42D5603A-7C30-421F-8DB4-C21473043201");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Акт осмотра».
    /// </summary>
    [Public]
    public static readonly Guid InspectionAct = Guid.Parse("884F99CB-C520-4974-900C-E9788EC3D4A6");
    
    /// <summary>
    /// Уникальный идентификатор для вида "Акт выполненных работ (приемка)"  // Старое «Акт выполненных работ».
    /// </summary>
    [Public]
    public static readonly Guid CertificateOfCompletion = Guid.Parse("907C7E2A-CA09-463A-A296-79A053BAA9C5");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Акт строительной готовности».
    /// </summary>
    [Public]
    public static readonly Guid ConstructionReadinessAct = Guid.Parse("32607ACB-FF0D-4572-A867-63F4F0AF08A0");
   
    /// <summary>
    /// Уникальный идентификатор для вида «Акт приема-передачи помещения».
    /// </summary>
    [Public]
    public static readonly Guid AcceptanceTransferApartmentKind = Guid.Parse("B03C772E-469E-49A3-B16D-977E1F223BB1");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Акт комиссионного обследования».
    /// </summary>
    public static readonly Guid CommissionInspectionActKind = Guid.Parse("48659D5D-04BA-45EA-A056-FAFFC2EBAC30");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Акт об устранении дефектов».
    /// </summary>
    public static readonly Guid DefectRemedyActKind = Guid.Parse("DCF5CBB4-7E41-4F15-A07A-8B3B46724E9C");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Акт выполненных работ (гарантия)».
    /// </summary>
    public static readonly Guid CertificateOfCompletionActKind = Guid.Parse("0758922C-5CB8-4884-81DF-F630DC9ADC3C");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Акт комиссионного осмотра мест общего пользования ».
    /// </summary>
    public static readonly Guid CommissionInspectionReportPublicKind = Guid.Parse("FC3D3C20-51DC-47D9-998B-FCA0648203F4");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Акт комиссионного обследования (УК)».
    /// </summary>
    public static readonly Guid CommissionInspectionActUkKind = Guid.Parse("19DDEA07-52FF-479C-8D08-3614898B4925");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Акт об устранении дефектов (УК)».
    /// </summary>
    public static readonly Guid DefectRemedyActUkKind = Guid.Parse("8A6C8006-5259-4C38-BBE5-078AC3B746A6");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Акт выполненных работ (УК)».
    /// </summary>
    public static readonly Guid CertificateOfCompletionActUkKind = Guid.Parse("E4382AA3-D369-4124-93E0-CFA4E2585C2B");
    
    #endregion
    
    /// <summary>
    /// Количество календарных дней после завершения задачи, в которую вложена Заявка на сдачу в архив. Используется в ФП DeleteCompletedRequestsForSubmission.
    /// </summary>
    public const int DeleteCompletedRequestsForSubmissionAfterDays = 30;
    
    //конец Добавлено Avis Expert
  }
}