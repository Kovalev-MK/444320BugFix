using System;
using Sungero.Core;

namespace lenspec.ProtocolsCollegialBodies.Constants
{
  public static class Module
  {

    /// <summary>
    /// Уникальный идентификатор для роли «Делопроизводители ГК».
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ClerksGKResponsible = Guid.Parse("b2f7dae1-009a-46ec-9298-8c776840c491");
    
    /// <summary>
    /// Уникальный идентификатор для вида «Протокол коллегиального органа».
    /// </summary>
    public static readonly Guid ProtocolCollegialBodyKind = Guid.Parse("c5adda56-5233-469a-b803-e1d4bba9fec2");
    
     /// <summary>
    /// Уникальный идентификатор для тиgа «Протокол коллегиального органа».
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ProtocolCollegialBodyTypes = Guid.Parse("34127cad-2596-4796-96cd-8f65be8b6f83");
  }
}