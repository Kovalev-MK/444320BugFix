using System;
using Sungero.Core;

namespace avis.CustomerRequests.Constants
{
  
  public static class Module
  {
    
    /// <summary>
    ///  GUID  Тип документа Обращение клиентов".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid CustReqType = Guid.Parse("29b9e735-ecfb-49ac-8150-b53238439f38");
    
    
    /// <summary>
    ///  GUID  Вид документа Обращение клиентов".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid CustReqDocKind = Guid.Parse("1CA8572F-DCB4-4BF1-9794-55037AD9CD64");

    
    /// <summary>
    ///  GUID  Роль доступа к справочникам".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid RoleAccessToDatabook = Guid.Parse("CA345458-6529-4192-9F39-52DB3D2AF224");
    
    /// <summary>
    ///  GUID  Роль доступа к документу Обращение клиентов".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid RoleAccessToRequests = Guid.Parse("ED4575A3-D141-4F5B-8994-41184868C9FF");
    
  }
}