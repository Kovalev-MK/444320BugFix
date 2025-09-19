using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.LocalRegulations.Server
{
  public class ModuleFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Создать приказ.
    /// </summary>
    /// <returns>Приказ.</returns>
    [Remote, Public]
    public static Sungero.RecordManagement.IOrder CreateOrder()
    {
      return Sungero.RecordManagement.Orders.Create();
    }
    
    /// <summary>
    /// Создать распоряжение.
    /// </summary>
    /// <returns>Распоряжение.</returns>
    [Remote, Public]
    public static Sungero.RecordManagement.ICompanyDirective CreateCompanyDirective()
    {
      return Sungero.RecordManagement.CompanyDirectives.Create();
    }
    //конец Добавлено Avis Expert
  }
}