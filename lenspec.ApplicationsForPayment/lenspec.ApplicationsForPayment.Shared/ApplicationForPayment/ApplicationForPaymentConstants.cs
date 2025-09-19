using System;
using Sungero.Core;

namespace lenspec.ApplicationsForPayment.Constants
{
  public static class ApplicationForPayment
  {

    
    #region Параметры
    
    [Sungero.Core.PublicAttribute]
    public const string IsBusinessUnitMainParam = "isBusinessUnitMain";
    
    [Sungero.Core.PublicAttribute]
    public const string IsSafeCounterpartyParam = "isSafeCounterparty";
    
    [Sungero.Core.PublicAttribute]
    public const string IsSignedContractParam = "isSignedContract";
    
    [Sungero.Core.PublicAttribute]
    public const string IsSafeThirdSideParam = "isSafeThirdSide";
    
    [Sungero.Core.PublicAttribute]
    public const string IsNeedUpdateIncomingInvoice = "isNeedUpdateIncomingInvoice";
    
    [Sungero.Core.PublicAttribute]
    public const string IsCounterpartyWithEmptyFields = "isCounterpartyWithEmptyFields";
    
    [Sungero.Core.PublicAttribute]
    public const string IsThirdSideWithEmptyFields = "isThirdSideWithEmptyFields";
    
    #endregion
    
    /// <summary>
    /// Максимальная общая сумма платежа Заявки на оплату без договора в рублях
    /// </summary>
    [Public]
    public const long LimitOfTotalAmountWithoutContract = 300000;
    
    /// <summary>
    /// Максимальная общая сумма платежа Заявки на оплату без договора в USD/EUR
    /// </summary>
    [Public]
    public const long LimitOfTotalAmountWithoutContractUSDEUR = 3000;
  }
}