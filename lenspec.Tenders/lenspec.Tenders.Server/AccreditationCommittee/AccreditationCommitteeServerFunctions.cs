using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.AccreditationCommittee;

namespace lenspec.Tenders.Server
{
  partial class AccreditationCommitteeFunctions
  {

    /// <summary>
    /// Получить дубли записей.
    /// </summary>
    /// <param name="name">Наименование.</param>
    /// <param name="index">Индекс.</param>
    /// <param name="excludedId">ИД для исключения.</param>
    /// <returns>Список дублей.</returns>
    [Remote(IsPure = true)]
    public static IQueryable<IAccreditationCommittee> GetDuplicateAccreditationCommittees(string name, string index, long? excludedId)
    {
      var duplicates = AccreditationCommittees.GetAll(x => x.Name.Trim() == name.Trim() && x.Index.Trim() == index.Trim());
      if (excludedId.HasValue)
        duplicates = duplicates.Where(x => x.Id != excludedId.Value);
      
      return duplicates;
    }
  }
}