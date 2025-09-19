using System;
using Sungero.Core;

namespace lenspec.Etalon.Module.Docflow.Constants
{
  public static class Module
  {
    
    /// <summary>
    /// GUID типов документов.
    /// </summary>
    public static class DocumentTypeGuids
    {
      /// <summary>
      ///  Уникальный идентификатор для типа "Официальный документ".
      /// </summary>
      [Sungero.Core.Public]
      public static readonly Guid OfficialDocumentType = Guid.Parse("58cca102-1e97-4f07-b6ac-fd866a8b7cb1");
    }

    /// <summary>
    /// GUID видов документов.
    /// </summary>
    public static class DocumentKindGuids
    {
      /// <summary>
      ///  Уникальный идентификатор для вида "Акт о приёмке выполненных работ (КС-2)".
      /// </summary>
      [Sungero.Core.Public]
      public static readonly Guid ContractStatementC2Kind = Guid.Parse("EA1D8FB3-F08B-4D10-9404-D28215729888");

      /// <summary>
      ///  Уникальный идентификатор для вида "Справка о приёмке выполненных работ (КС-3)".
      /// </summary>
      [Sungero.Core.Public]
      public static readonly Guid ContractStatementC3Kind = Guid.Parse("26F3713E-3C69-4803-A9AF-8948025A127C");
    }
    
    /// <summary>
    /// Фоновые процессы.
    /// </summary>
    public static class Jobs
    {
      /// <summary>
      /// Количество документов для выдачи прав на связанные задачи в рамках одного АО.
      /// </summary>
      public const int DocumentsGrantingBulkSize = 100;
    }

    public static class TaskGroups
    {
      [Sungero.Core.Public]
      public static readonly Guid ApprovalCounterpartyPersonDEBTaskCounterpartyInfoGroup = Guid.Parse("acf65403-5eec-476e-8db9-d6fdcc3775ee");
    }
    
    [Sungero.Core.Public]
    public const string CounterpartyDocumentTypeGuid = "49d0c5e7-7069-44d2-8eb6-6e3098fc8b10";
    
    #region Ставки НДС
    
    /// <summary>
    /// Значение по умолчанию для ставки "5%".
    /// </summary>
    public const double DefaultVatRateFivePercent = 5;
    
    /// <summary>
    /// Sid ставки НДС "10%".
    /// </summary>
    [Sungero.Core.Public]
    public const string VatRateFivePercentSid = "90E8BF18-B034-4C9F-A14C-01779AADCE28";
    
    /// <summary>
    /// Значение по умолчанию для ставки "7%".
    /// </summary>
    public const double DefaultVatRateSevenPercent = 7;
    
    /// <summary>
    /// Sid ставки НДС "20%".
    /// </summary>
    [Sungero.Core.Public]
    public const string VatRateSevenPercentSid = "1852B9F1-532A-4A77-A176-3A85C3FE2F9B";
    
    /// <summary>
    /// Значение по умолчанию для ставки "18%".
    /// </summary>
    public const double DefaultVatRateEighteenPercent = 18;
    
    /// <summary>
    /// Sid ставки НДС "20%".
    /// </summary>
    [Sungero.Core.Public]
    public const string VatRateEighteenPercentSid = "C92E4A73-233B-48B7-923F-32F91227BC6E";
    
    /// <summary>
    /// Значение по умолчанию для ставки "0%".
    /// </summary>
    public const double DefaultVatRateMixedPercent = 0;
    
    /// <summary>
    /// Sid ставки НДС "Смешанная".
    /// </summary>
    [Sungero.Core.Public]
    public const string VatRateMixedPercentSid = "F395A32D-A818-41CB-AFBF-4C042050069B";
    
    /// <summary>
    /// Sid ставки НДС "10%" (скопировано из базового решения из-за уровня доступа).
    /// </summary>
    [Sungero.Core.Public]
    public const string VatRateTenPercentSid = "C35498D8-511F-4218-8137-39EC11A1596B";
    
    /// <summary>
    /// Sid ставки НДС "20%" (скопировано из базового решения из-за уровня доступа).
    /// </summary>
    [Sungero.Core.Public]
    public const string VatRateTwentyPercentSid = "D99F2F70-1069-4190-95EE-948F49C065C5";
    
    /// <summary>
    /// Sid ставки НДС "9.09%" (скопировано из базового решения из-за уровня доступа).
    /// </summary>
    [Sungero.Core.Public]
    public const string VatRateNinePointZeroNinePercentSid = "8EF79E56-D917-4EDA-81B0-172738D0989C";
    
    /// <summary>
    /// Sid ставки НДС "16.67%" (скопировано из базового решения из-за уровня доступа).
    /// </summary>
    [Sungero.Core.Public]
    public const string VatRateSixteenPointSixSevenPercentSid = "900CEA8D-F52E-44B2-B14E-24F22C7C07AA";
    
    /// <summary>
    /// Sid ставки НДС "0%".
    /// </summary>
    [Sungero.Core.Public]
    public const string VatRateZeroPercentSid = "D54D8812-2BBF-4BBA-8CA5-BECB3FBD6DDB";
    
    /// <summary>
    /// Sid ставки НДС "Без НДС".
    /// </summary>
    [Sungero.Core.Public]
    public const string VatRateWithoutVatSid = "930EC682-0CA7-4B9E-9F0F-2F5CE8B6A90B";
    
    #endregion
  }
}