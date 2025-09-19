using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Company;

namespace lenspec.Etalon
{
  partial class CompanyMaterialsavisCityPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MaterialsavisCityFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Region != null)
        query = query.Where(x => x.Region == _obj.Region);
      
      return query;
    }
  }

  partial class CompanyWorkKindsAvisavisCityPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WorkKindsAvisavisCityFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Region != null)
        query = query.Where(x => x.Region == _obj.Region);
      
      return query;
    }
  }

  partial class CompanyCitieslenspecCityPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CitieslenspecCityFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_root.RegionOfPresencesavis.Any())
      {
        var regions = _root.RegionOfPresencesavis.Select(x => x.Region);
        query = query.Where(x => regions.Contains(x.Region));
      }
      return query;
    }
  }

  partial class CompanyFilteringServerHandler<T>
  {
    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = base.Filtering(query, e);
      if (_filter == null)
        return query;
      
      // Набор флажков Тип контрагента.
      if (_filter.Provider || _filter.Contractor)
        query = query.Where(c => _filter.Provider && c.IsProvideravis == true ||
                            _filter.Contractor && c.IsContractoravis == true);
      
      return query;
    }
  }

  partial class CompanyMaterialsavisMaterialGroupPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MaterialsavisMaterialGroupFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q=> q.Status == avis.EtalonParties.MaterialGroup.Status.Active);
      
      return query;
    }
  }

  partial class CompanyMaterialsavisMaterialPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MaterialsavisMaterialFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q => q.Status == avis.EtalonParties.Material.Status.Active);
      
      return query;
    }
  }

  partial class CompanyWorkKindsAvisavisWorkGroupPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WorkKindsAvisavisWorkGroupFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q => q.Status == avis.EtalonParties.WorkGroup.Status.Active);
      
      return query;
    }
  }

  partial class CompanyWorkKindsAvisavisWorkKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> WorkKindsAvisavisWorkKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q => q.Status == avis.EtalonParties.WorkKind.Status.Active);
      
      return query;
    }
  }

  partial class CompanyServerHandlers
  {

    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      // Копировать Согласование ДЭБ с головной организации
      if (_obj.HeadCompany != null)
      {
        _obj.ResultApprovalDEBavis = _obj.HeadCompany.ResultApprovalDEBavis;
        _obj.InspectionDateDEBavis = _obj.HeadCompany.InspectionDateDEBavis;
        _obj.LimitPerCounterpartyavis = _obj.HeadCompany.LimitPerCounterpartyavis;
        _obj.ResponsibleDEBavis = _obj.HeadCompany.ResponsibleDEBavis;
        _obj.ApprovalPeriodavis = _obj.HeadCompany.ApprovalPeriodavis;
      }
      
      // Вызов АО для обработки реестра поставщиков и подрядчиков
      Functions.Company.UpdateRegistries(_obj);
    }

    /// <summary>
    /// Создание.
    /// </summary>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsProvideravis = false;
      _obj.IsContractoravis = false;
      _obj.IsMonopolistlenspec = false;
    }
    
    /// <summary>
    /// После сохранения
    /// </summary>
    /// <param name="e"></param>
    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      
      // Проверяем контакт.
      var contact = lenspec.Etalon.Contacts.GetAll(c => c.Name == _obj.FIOContactavis && c.Company == _obj).FirstOrDefault();
      if (contact != null)
        return;
      
      if (string.IsNullOrEmpty(_obj.FIOContactavis))
        return;
      
      // Создаём контакт.
      var newContact = lenspec.Etalon.Contacts.Create();
      newContact.Name = _obj.FIOContactavis;
      newContact.JobTitle = _obj.JobTitleContactavis;
      newContact.Company = _obj;
      newContact.Save();
    }
  }

  partial class CompanyGroupCounterpartyavisPropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация выбора из списка
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual IQueryable<T> GroupCounterpartyavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q => q.IdDirectum5 != 6 || q.IdDirectum5 == null);
      
      return query;
    }
  }

  partial class CompanyCategoryCounterpartyavisPropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация доступных значение.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual IQueryable<T> CategoryCounterpartyavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Выводим только значения которые включены в выбранную группу контрагента.
      query = query.Where(q => q.GroupCounterparties.Where(g => g.GroupCounterparty == _obj.GroupCounterpartyavis) != null);
      
      return query;
    }
  }
}