using System;
using Sungero.Core;

namespace lenspec.CourierShipments.Constants
{
  public static class Module
  {

    //Добавлено Avis Expert
    #region Типы документов
    
    #endregion
    
    
    #region Виды документов
    
    // Уникальный идентификатор для вида «Заявка на курьерское отправление».
    public static readonly Guid CourierShipmentsApplicationKind = Guid.Parse("D1C434E2-837B-452E-802B-251E816C839C");
    
    #endregion
    
    
    #region Роли
    
    // GUID роли "Ответственные за справочник курьерские службы".
    [Sungero.Core.Public]
    public static readonly Guid CourierServiceResponsible = Guid.Parse("6F303E87-2B68-410F-A0EA-EB2A9931BA5C");
    
    #endregion
    
    //конец Добавлено Avis Expert
  }
}