using System;
using Sungero.Core;

namespace avis.PowerOfAttorneyModule.Constants
{
  public static class Module
  {

    #region Типы документов
    
    /// <summary>
    /// Гуид типа PowerOfAttorneyBase (коробка)
    /// </summary>
    [Public]
    public const string PowerOfAttorneyBaseTypeGuid = "a8d955b1-4b35-4013-b643-3ea9d80bd1ea";
    
    /// <summary>
    /// Гуид типа FormalizedPowerOfAttorney (коробка)
    /// </summary>
    [Public]
    public const string FormalizedPowerOfAttorneyTypeGuid = "104472db-b71b-42a8-bca5-581a08d1ca7b";
    
    /// <summary>
    /// Гуид типа PowerOfAttorney (Коробка)
    /// </summary>
    [Public]
    public const string PowerOfAttorneyTypeGuid = "be859f9b-7a04-4f07-82bc-441352bce627";
    
    #endregion

    #region Роли
    /// <summary>
    /// Sid роли Права на создание простых и нотариальных доверенностей без согласования
    /// </summary>
    [Public]
    public static readonly Guid RoleGuidRightCreatePowerOfAttorneys = Guid.Parse("009445D5-D56C-42F7-B47C-805C6AB1BD90");
    
    /// <summary>
    /// Sid роли Права на изменение доверенностей группы
    /// </summary>
    [Public]
    public static readonly Guid RoleGuidRightChangePowerOfAttorneysGroup = Guid.Parse("0FE77C5D-A8CF-4ADA-ADE5-8BC84B5EC380");
    
    /// <summary>
    /// Sid роли Ответственный за полномочия по доверенностям
    /// </summary>
    [Public]
    public static readonly Guid RoleGuidResponsibleForAuthorities = Guid.Parse("B047BFF9-378C-4686-9118-FC9BADADFD5F");

    /// <summary>
    /// Sid роли Ответственный за право подписи
    /// </summary>
    [Public]
    public static readonly Guid RoleGuidResponsibleRightSign = Guid.Parse("1487C981-5B84-4A24-A524-C401C44C0422");
    
    /// <summary>
    /// Sid роли Ответственный за отзыв доверенностей
    /// </summary>
    [Public]
    public static readonly Guid RoleGuidResponsibleRecallPowerOfAttorneys = Guid.Parse("8354CFB4-1EF8-4978-AF00-6269D58ABCAE");
    
    /// <summary>
    /// Наименование роли делопроизводители
    /// </summary>
    public const string RoleNameClerks = "Делопроизводители";
    
    /// <summary>
    /// Sid роли Права на все доверенности группы
    /// </summary>
    public static readonly Guid RoleGuidRightAllPowerOfAttorneysGroup = Guid.Parse("908C0F56-2554-450A-B5B4-E0C090E4904A");
    
    /// <summary>
    /// Гуид роли ответственный юрист для доверенностей
    /// </summary>
    [Public]
    public static readonly Guid RoleGuidResponsibleLawyer = Guid.Parse("C37E1CDA-BE51-45ED-9608-BB6248AAD0B1");
    
    /// <summary>
    /// Sid роли Права на просмотр всех доверенностей группы
    /// </summary>
    [Public]
    public static readonly Guid RoleGuidRightReadAllPowerOfAttorneysGroup = Guid.Parse("8BF119E9-51FB-4AB9-B286-E49F15341B44");
    
    /// <summary>
    /// Sid роли Права на вложение сканов бумажных доверенностей
    /// </summary>
    [Public]
    public static readonly Guid RoleGuidRightsToAttachScans = Guid.Parse("40B74B47-77FD-4322-8441-CE9EAB4C2425");
    
    #endregion
    
    #region Виды документов
    
    /// <summary>
    /// Sid вида Нотариальная доверенность
    /// </summary>
    [Public]
    public static readonly Guid DocumentNotarialKindGuid = Guid.Parse("03ED3F61-4EE5-40C2-B1D4-E4C20B7F7709");
    
    /// <summary>
    /// Sid вида 'Документы поверенных'
    /// </summary>
    [Public]
    public static readonly Guid DocumentAttorneyKindGuid = Guid.Parse("B2FEF71C-9C79-42D0-91B7-D86DF067DF22");
    
    /// <summary>
    /// Sid вида Заявление об отказе от полномочий по доверенности
    /// </summary>
    [Public]
    public static readonly Guid ApplicationRelinquishmentAuthorityKindGuid = Guid.Parse("8A377C85-A61A-4DE4-8F9D-E7ECAAE27E52");
    
    /// <summary>
    /// Sid вида Заявка на оформление доверенности
    /// </summary>
    [Public]
    public static readonly Guid RequestToCreatePowerOfAttorneyKindGuid = Guid.Parse("E5A9FF0F-4E7A-488A-BB7E-12C032C2DD4D");
    
    /// <summary>
    /// Sid вида Заявка на оформление нотариальной доверенности
    /// </summary>
    [Public]
    public static readonly Guid RequestToCreateNotarialPowerOfAttorneyKindGuid = Guid.Parse("3A9D57AC-D257-4964-BE3D-424F49EC24C5");
    
    #endregion
    
  }
}