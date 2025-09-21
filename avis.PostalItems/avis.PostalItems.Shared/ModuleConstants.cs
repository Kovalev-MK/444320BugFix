using System;
using Sungero.Core;

namespace avis.PostalItems.Constants
{
  public static class Module
  {
    #region Тип документа
    public static readonly Guid LetterComponentDocumentGuid = Guid.Parse("98f851d6-9bb2-4c2b-b795-ef0a80335d9a");
    #endregion
    
    #region Виды документов
    
    /// <summary>
    /// Уникальный идентификатор для вида «Конверт».
    /// </summary>
    public static readonly Guid PostalItemEnvelope = Guid.Parse("1EB4A0F8-1BEA-49FB-9CE6-82EF67ECEE87");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Чек».
    /// </summary>
    public static readonly Guid PostalItemCheck = Guid.Parse("DA456FB5-A8CC-47EE-8698-7B5B30C3D0D1");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Уведомление».
    /// </summary>
    public static readonly Guid PostalItemNotification = Guid.Parse("59596FB4-5C77-428F-B1FC-C65B9930C1A2");
    
    #endregion
    
    #region РПО.
    /// <summary>
    /// GUID Категории РПО "Заказные письма".
    /// </summary>
    [Public]
    public static readonly Guid RPOCustomGuid = Guid.Parse("5E1FE82E-DD8F-4A7D-AD50-15EE47DF9FE5");
    /// <summary>
    /// GUID Категории РПО "Письма заграницу".
    /// </summary>
    [Public]
    public static readonly Guid RPOAbroadGuid = Guid.Parse("1FE85289-EC19-46E5-9B64-A0C56EACB76E");
    /// <summary>
    /// GUID Категории РПО "Простые письма".
    /// </summary>
    [Public]
    public static readonly Guid RPOSimpleGuid = Guid.Parse("4A0F3983-8A55-49BC-9A36-9D24C1464F98");
    /// <summary>
    /// GUID Категории РПО "Ценные письма".
    /// НАПОМИНАЛКА: Если меняешь/удаляешь тут гуид, исправь конверты!!! ТАМ ИСПОЛЬЗУЕТСЯ ЭТОТ ЖЕ ГУИД в дизайнере!
    /// </summary>
    [Public]
    public static readonly Guid RPOValuableGuid = Guid.Parse("C108C56D-539D-4A54-ABAE-216A5E07F839");
    #endregion
  }
}