using System;
using Sungero.Core;

namespace lenspec.EtalonDatabooks.Constants
{
  public static class ObjectAnSale
  {
    /// <summary>
    /// Ячейки данных Помещения в файле Excel.
    /// </summary>
    public static class ExcelCells
    {
      /// Номер помещения.
      [Public]
      public const int NumberRoomMail = 8;
      
      /// ИСП (ИД проекта в RX).
      [Public]
      public const int OurCFId = 9;
      
      /// Объект (ИД объекта в Dir RX).
      [Public]
      public const int ObjectAnProjectId = 10;
      
      /// Назначение помещения (ИД назначения в Dir RX).
      [Public]
      public const int PurposeOfPremisesId = 11; 
    }
  }
}