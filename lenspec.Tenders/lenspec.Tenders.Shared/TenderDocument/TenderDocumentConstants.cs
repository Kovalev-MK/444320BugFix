using System;
using Sungero.Core;

namespace lenspec.Tenders.Constants
{
  public static class TenderDocument
  {
    public static class Params
    {
      /// GUID вида документа.
      public const string DocumentKindGuid = "DocumentKindGuid";
      
      /// Признак доступности действия "Создать PDF с установкой штампа".
      public const string CanCreatePDFWithStamp = "CanCreatePDFWithStamp";
      
      /// Признак вхождения пользователя в роль "Администраторы".
      public const string IsAdministrator = "IsAdministrator";
    }
  }
}