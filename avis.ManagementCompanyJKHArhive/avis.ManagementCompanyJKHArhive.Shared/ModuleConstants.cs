using System;
using Sungero.Core;

namespace avis.ManagementCompanyJKHArhive.Constants
{
  public static class Module
  {
    #region Виды документов
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Договор управления МКД».
    /// </summary>
    public static readonly Guid MKDManagementAgreementKind = Guid.Parse("56CD703C-EA22-4D80-810F-B77BE9772865");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Направление от застройщика (при заселении в дом)».
    /// </summary>
    [Obsolete("Используется одноименный вид документа для другого типа")]
    public static readonly Guid DirectionFromDeveloperKind = Guid.Parse("48A6B332-9A3E-4C97-BFCB-056C06E673A5");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Правоустанавливающий документ (выписка из ЕГРН)».
    /// </summary>
    [Obsolete("Используется одноименный вид документа для другого типа")]
    public static readonly Guid TitleDocumentKind = Guid.Parse("D3DBEBCE-641F-4F14-AE37-B661C75FFFBD");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Прочие документы ЖКХ».
    /// </summary>
    public static readonly Guid OtherDocJKHKind = Guid.Parse("2D2F43ED-B9B1-413F-AEF7-A488BD8028EA");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Направление от застройщика (при заселении в дом)».
    /// </summary>
    public static readonly Guid DirectionFromDeveloperBaseForManagementAgreementKind = Guid.Parse("C6D8A91E-831A-4A10-864F-64B2DA30274C");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Правоустанавливающий документ (выписка из ЕГРН)».
    /// </summary>
    public static readonly Guid TitleDocumentBaseForManagementAgreementKind = Guid.Parse("B4DEC546-094B-4F03-A1C9-72901BBB5618");
    
    #endregion
    
    
    #region Связи
    
    // Имя типа связи "Документ-основание для создания договора управления МКД - Договор управления МКД".
    [Sungero.Core.PublicAttribute]
    public const string DocumentBasisToManagementAgreementMKDRelationName = "BasisMKD";
    
    #endregion    
  }
}