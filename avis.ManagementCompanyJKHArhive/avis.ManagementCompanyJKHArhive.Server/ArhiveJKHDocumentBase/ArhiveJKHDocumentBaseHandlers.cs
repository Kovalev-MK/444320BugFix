using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ManagementCompanyJKHArhive.ArhiveJKHDocumentBase;

namespace avis.ManagementCompanyJKHArhive
{
  partial class ArhiveJKHDocumentBaseObjectAnSalePropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация выбора из списка "Помещение".
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual IQueryable<T> ObjectAnSaleFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.OurCF == null)
        return query;
      
      query = query.Where(q => q.OurCF == _obj.OurCF);
      return query;
    }
  }

  partial class ArhiveJKHDocumentBaseOurCFPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public virtual IQueryable<T> OurCFFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.ObjectAnSale != null && _obj.ObjectAnSale.OurCF != null)
      {
        query = query.Where(x => x.Equals(_obj.ObjectAnSale.OurCF));
      }
      
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class ArhiveJKHDocumentBaseServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      _obj.Subject = _obj.Name;
      
      base.BeforeSave(e);
    }
    // Добавлено avis.
    
    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.Name = "<Имя будет сформировано автоматически>";
      _obj.Subject = "<Имя будет сформировано автоматически>";
      _obj.BusinessUnit = Sungero.Company.Employees.Current.Department.BusinessUnit;
    }
    
    // Конец добавлено avis.
  }
}