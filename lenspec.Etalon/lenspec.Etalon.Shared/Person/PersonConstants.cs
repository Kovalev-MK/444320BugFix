using System;
using Sungero.Core;

namespace lenspec.Etalon.Constants.EtalonIntergation
{
  public static class Person
  {
    /// <summary>
    /// Ячейки данных Персоны в файле Excel.
    /// </summary>
    public static class ExcelCells
    {
      /// Фамилия.
      [Public]
      public const int LastName = 0;
      
      /// Имя.
      [Public]
      public const int FirstName = 1;
      
      /// Отчество.
      [Public]
      public const int MiddleName = 2;
      
      /// Пол.
      [Public]
      public const int Sex = 3;
      
      /// Дата рождения.
      [Public]
      public const int DateOfBirth = 4;
      
      /// Номер телефона.
      [Public]
      public const int Phone = 5;
      
      /// Электронная почта.
      [Public]
      public const int Email = 6;
      
      /// Лицевой счет.
      [Public]
      public const int PersonalAccount = 7;
    }
  }
}