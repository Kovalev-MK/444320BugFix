using System;
using Sungero.Core;

namespace avis.ApprovingCounterpartyDEB.Constants
{
  public static class ApprovalCounterpartyBankDEB
  {

    /// <summary>
    /// Гуид типа Согласование контрагента/банка ДБ
    /// </summary>
    public static readonly Guid ApprovalCounterpartyTypeGuid = Guid.Parse("3fafeece-8bc5-42ad-b1b8-f5ec928d7190");
    
    /// <summary>
    /// Гуид вида Согласовнаие контрагента/банка ДБ
    /// </summary>
    public static readonly Guid ApprovalCounterpartyKindGuid = Guid.Parse("F83B8147-4579-4BCF-B115-EC46E27885B2");
    
    /// <summary>
    /// Минмальная сумма с которой проверка лимита по КА обязательна
    /// </summary>
    public const double MinAmountVerification = 300000;

  }
}