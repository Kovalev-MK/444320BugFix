using System;
using Sungero.Core;

namespace lenspec.ElectronicDigitalSignatures.Constants
{
  public static class Module
  {

    #region Роли
    
    /// <summary>
    ///  GUID роли "Ответственный за выдачу, продление и аннулирование УКЭП по СПБ".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid QESResponsibleSpb = Guid.Parse("D1B94A69-EA1E-46D5-A2A0-9F9B0278C410");
    
    /// <summary>
    ///  GUID роли "Ответственный за выдачу, продление и аннулирование УКЭП по МСК и Регионам".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid QESResponsibleMsk = Guid.Parse("C2371874-3829-43EF-902F-B94276D57C43");
    
    /// <summary>
    ///  GUID роли "Права на запуск Заявок УКЭП на носителе".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid EDSApplicationsRights = Guid.Parse("C645711E-1FC8-4719-B1AE-433AF1F74D62");
    
    #endregion
    
    #region Виды документов
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Заявка УКЭП на носителе».
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid EDSApplicationKind = Guid.Parse("DEF3F0C1-08F1-48AD-A09F-E700E400CED5");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Паспорт РФ».
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid PassportKind = Guid.Parse("93120A1E-BAB5-4E8A-884B-97607BABBA9F");
    
    /// <summary>
    ///  Уникальный идентификатор для вида «Согласие на обработку персональных данных».
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ConsentToProcessingKind = Guid.Parse("708F529A-329A-4B5E-B013-07B94682718A");
    
    #endregion
    
    #region Типы документов
    
    /// <summary>
    ///  Идентификатор типа документа «Заявка УКЭП на носителе».
    /// </summary>
    [Sungero.Core.Public]
    public const string EDSApplicationTypeGuid = "f44d7c71-53f2-4697-b94d-d0fdef12505e";
    
    #endregion
  }
}