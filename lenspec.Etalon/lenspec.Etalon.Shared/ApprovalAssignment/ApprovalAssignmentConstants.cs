using System;
using Sungero.Core;

namespace lenspec.Etalon.Constants.Docflow
{
  public static class ApprovalAssignment
  {

    public static class Params
    {
      /// Признак вхождения в роль "Ответственный юрист для доверенностей."
      public const string IsInRoleLawyer = "IsInRoleLawyer";
      
      /// Признак вида документа "Заявка на оформление доверенности".
      public const string IsRequestToCreatePowerOfAttorneyKind = "IsRequestToCreatePowerOfAttorneyKind";
      
      /// Признак вида документа "Заявка на оформление нотариальной доверенности".
      public const string IsRequestToCreateNotarialPowerOfAttorneyKind = "IsRequestToCreateNotarialPowerOfAttorneyKind";
      
      // Высота контрола заполнения НОР.
      [Sungero.Core.Public]
      public const int BusinessUnitsTextRowsCount = 3;
    }
  }
}