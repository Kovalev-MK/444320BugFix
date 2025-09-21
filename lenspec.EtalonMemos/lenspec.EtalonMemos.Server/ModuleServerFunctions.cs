using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow;
using lenspec.Etalon;

namespace lenspec.EtalonMemos.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Получить мои Служебные записки
    /// </summary>
    [Remote(IsPure = true), Public]
    public static System.Linq.IQueryable<lenspec.Etalon.IMemo> GetAllMemos(DateTime? dateFrom, DateTime? dateTo, IUser userEmployee)
    {
      return Etalon.Memos.GetAll()
        .Where(l => dateFrom == null || l.Created >= dateFrom)
        .Where(l => dateTo == null || l.Created < dateTo)
        .Where(l => userEmployee == null || l.Author.Equals(userEmployee));
    }
    
    /// <summary>
    /// Создать новую служебную записку.
    /// </summary>
    [Remote]
    public static lenspec.Etalon.IMemo CreateMemos()
    {     
      return Etalon.Memos.Create();
    }
  }
}