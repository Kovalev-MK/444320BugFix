using System;
using Sungero.Core;

namespace lenspec.Etalon.Constants.Contracts
{
  public static class ContractualDocument
  {
    public static class Params
    {
      // Имя параметра: версия создана из действия.
      public const string CreateFromAction = "CreateFromAction";
      
      // Имя параметра: версия создана из действия Создать на основе.
      public const string CreateFromDocument = "CreateFromDocument";
      
      // Имя параметра: документ создан из действия Создать из внутригруппового документа.
      public const string IsCreatedFromIntragroupDocument = "IsCreatedFromIntragroupDocument";
      
    }
  }
}