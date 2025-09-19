using System;
using Sungero.Core;

namespace lenspec.Etalon.Constants.EtalonIntergation
{
  public static class Company
  {
    
    public static class Params
    {
      /// Признак вхождения в роль "Администраторы".
      public const string isAdmin = "isAdmin";
      
      /// Признак вхождения в роль "Делопроизводители".
      public const string isClerk = "isClerk";
      
      /// Признак вхождения в роль "Права на создание всех видов контрагентов".
      public const string isCreateCounterpartyRole = "isCreateCounterpartyRole";
      
      /// Признак вхождения в роль "Ответственные за квалификацию и дисквалификацию контрагентов".
      public const string isResponsibleForCounterpartyQualification = "isResponsibleForCounterpartyQualification";
      
      /// Признак вхождения в роль "Ответственный за признак монополистов КА".
      public const string isResponsibleForMonopolists = "isResponsibleForMonopolists";
    }
  }
}