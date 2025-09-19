using System;
using Sungero.Core;

namespace lenspec.ApplicationsForPayment.Constants
{
  public static class Module
  {
    public static class Params
    {
      /// Признак вхождения пользователя в роль "Полные права на корректировку и выгрузку заявок на оплату в 1С".
      public const string IsUploadApplicationRole = "IsUploadApplicationRole";
    }
    
    /// <summary>
    /// Guid роли Ответственный бухгалтер НДФЛ (ЗНО).
    /// </summary>
    [Public]
    public static readonly Guid PitResponsibleAccountantRoleGuid = Guid.Parse("B948DBA3-457D-45EF-866B-BCF0618870CC");
    
    /// <summary>
    /// Гуид роли Полные права на корректировку и выгрузку заявок на оплату в 1С.
    /// </summary>
    [Public]
    public static readonly Guid UploadApplicationForPaymentRoleGuid = Guid.Parse("B528298A-64DF-48DA-BF2F-621A944A2A6F");

    /// <summary>
    /// Гуид роли Полные права на справочник «Типы платежей».
    /// </summary>
    [Public]
    public static readonly Guid PaymentTypeAccessRoleGuid = Guid.Parse("91F7C77A-FB09-4B58-9C5E-90732D5E8C82");
    
    /// <summary>
    /// Гуид Вид документа "Заявка на оплату".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ApplicationForPaymentKind = Guid.Parse("9C18699B-6627-4DDA-BA36-FD95CAA023E7");
    
    /// <summary>
    /// Гуид типа документа "Заявка на оплату".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ApplicationForPaymentType = Guid.Parse("8ae29274-942b-4515-97df-0f8a71ab8e14");

  }
}