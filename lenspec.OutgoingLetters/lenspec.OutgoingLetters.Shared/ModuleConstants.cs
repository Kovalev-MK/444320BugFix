using System;
using Sungero.Core;

namespace lenspec.OutgoingLetters.Constants
{
  public static class Module
  {

    //Добавлено Avis Expert
    
    #region Типы документов
    
    // Строковый GUID типа документа "Заявка на рассылку массовых уведомлений".
    [Public]
    public const string MassMailingApplicationTypeGuid = "fa18be6a-226c-4868-b24a-1ae60e0c30a4";
    
    /// <summary>
    ///  GUID типа документа "Заявка на рассылку массовых уведомлений".
    /// </summary>
    [Public]
    public static readonly Guid MassMailingApplicationType = Guid.Parse("fa18be6a-226c-4868-b24a-1ae60e0c30a4");
    
    #endregion
    
    
    #region Виды документов
    
    /// <summary>
    /// Уникальный идентификатор для вида «Заявка на рассылку массовых уведомлений».
    /// </summary>
    public static readonly Guid MassMailingApplicationKind = Guid.Parse("B66A430A-3B84-4BC0-B7A2-10D01123A1D7");
    
    #endregion
    
    
    #region Роли
    
    /// <summary>
    /// GUID роли "Пользователи с правами на создание заявок на массовые рассылки".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid CreatingMassMailingApplicationRole = Guid.Parse("5E5ABB56-2001-4BE1-B039-74D9C3C8DB47");
    
    /// <summary>
    /// GUID роли "Права на отчет об исполнении поручений по входящим письмам".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid RightsToReportExecutionOrdersIncommingLetters = Guid.Parse("9ccc94d6-e68e-4b6c-bfe9-1cc095ac1c22");
    
    /// <summary>
    /// GUID роли "Права на вложение сканов исходящих писем".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid RightsToAttachScansOfOutgoingLetters = Guid.Parse("A6A9B25C-5CB8-4EE5-A712-D928A0555685");
    
    #endregion
    
    //конец Добавлено Avis Expert
  }
}