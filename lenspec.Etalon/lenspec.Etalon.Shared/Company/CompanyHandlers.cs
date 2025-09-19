using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Company;

namespace lenspec.Etalon
{


  partial class CompanyWorkKindsAvisavisSharedHandlers
  {

    public virtual void WorkKindsAvisavisRegionChanged(lenspec.Etalon.Shared.CompanyWorkKindsAvisavisRegionChangedEventArgs e)
    {
      if (e.NewValue == null || _obj.City != null && _obj.City.Region != e.NewValue)
      {
        _obj.City = null;
      }
    }

    public virtual void WorkKindsAvisavisCityChanged(lenspec.Etalon.Shared.CompanyWorkKindsAvisavisCityChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.Region = e.NewValue.Region;
      }
    }
    /// <summary>
    /// Изменение значения свойства "Виды работ" коллекции "Виды работ".
    /// </summary>
    /// <param name="e"></param>
    public virtual void WorkKindsAvisavisWorkKindChanged(lenspec.Etalon.Shared.CompanyWorkKindsAvisavisWorkKindChangedEventArgs e)
    {
      // Заполняем "Группы работ".
      _obj.WorkGroup = _obj.WorkKind?.Group;
    }
  }

  partial class CompanyMaterialsavisSharedHandlers
  {

    public virtual void MaterialsavisRegionChanged(lenspec.Etalon.Shared.CompanyMaterialsavisRegionChangedEventArgs e)
    {
      if (e.NewValue == null || _obj.City != null && _obj.City.Region != e.NewValue)
      {
        _obj.City = null;
      }
    }

    public virtual void MaterialsavisCityChanged(lenspec.Etalon.Shared.CompanyMaterialsavisCityChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.Region = e.NewValue.Region;
      }
    }
    /// <summary>
    /// Изменение значения свойства "Материалы" колленкции "Материалы"
    /// </summary>
    /// <param name="e"></param>
    public virtual void MaterialsavisMaterialChanged(lenspec.Etalon.Shared.CompanyMaterialsavisMaterialChangedEventArgs e)
    {
      // Заполняем группу материалов.
      _obj.MaterialGroup = e.NewValue?.Group;
    }
  }

  partial class CompanyMaterialsavisSharedCollectionHandlers
  {
    /// <summary>
    /// Удаление из коллекции Материалы.
    /// </summary>
    /// <param name="e"></param>
    public virtual void MaterialsavisDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextItems = _obj.Materialsavis.Where(x => x.Id > _deleted.Id);
      foreach(var item in nextItems)
      {
        item.Number = Convert.ToInt32(item.Number) - 1;
      }
    }

    /// <summary>
    /// Добавление в колленкцию Материалы.
    /// </summary>
    /// <param name="e"></param>
    public virtual void MaterialsavisAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = (_obj.Materialsavis.Max(a => a.Number) ?? 0) + 1;
    }
  }

  partial class CompanyWorkKindsAvisavisSharedCollectionHandlers
  {

    /// <summary>
    /// Удаление из коллекции Виды работ.
    /// </summary>
    /// <param name="e"></param>
    public virtual void WorkKindsAvisavisDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextItems = _obj.WorkKindsAvisavis.Where(x => x.Id > _deleted.Id);
      foreach(var item in nextItems)
      {
        item.Number = Convert.ToInt32(item.Number) - 1;
      }
    }

    /// <summary>
    /// Добавление в колленкцию Виды работ.
    /// </summary>
    /// <param name="e"></param>
    public virtual void WorkKindsAvisavisAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = (_obj.WorkKindsAvisavis.Max(a => a.Number) ?? 0) + 1;
    }
  }

  // Добавлено avis
  partial class CompanySharedHandlers
  {

    public virtual void RegionOfPresencesavisChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      _obj.Citieslenspec.Clear();
    }

    public virtual void MaterialsavisChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      _obj.MaterialKindsDetailinglenspec = string.Empty;
      _obj.MaterialKindsForAccreditationlenspec = string.Empty;
      _obj.RegionsMaterialsKindslenspec = string.Empty;
      if (_obj.Materialsavis != null && _obj.Materialsavis.Any())
      {
        _obj.MaterialKindsForAccreditationlenspec = string.Join(", ", _obj.Materialsavis.Where(x => x.MaterialGroup != null).Select(x => x.MaterialGroup.Name).ToArray<string>());
        _obj.MaterialKindsDetailinglenspec = string.Join(", ", _obj.Materialsavis.Where(x => x.Material != null).Select(x => x.Material.Name).ToArray<string>());
        _obj.RegionsMaterialsKindslenspec = string.Join(", ", _obj.Materialsavis.Where(x => x.Region != null).Select(x => x.Region.Name).ToArray<string>());
      }
    }

    public virtual void WorkKindsAvisavisChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      _obj.WorkKindsDetailinglenspec = string.Empty;
      _obj.WorkKindsForAccreditationlenspec = string.Empty;
      _obj.RegionsWorkKindslenspec = string.Empty;
      if (_obj.WorkKindsAvisavis != null && _obj.WorkKindsAvisavis.Any())
      {
        _obj.WorkKindsForAccreditationlenspec = string.Join(", ", _obj.WorkKindsAvisavis.Where(x => x.WorkGroup != null).Select(x => x.WorkGroup.Name).ToArray<string>());
        _obj.WorkKindsDetailinglenspec = string.Join(", ", _obj.WorkKindsAvisavis.Where(x => x.WorkKind != null).Select(x => x.WorkKind.Name).ToArray<string>());
        _obj.RegionsWorkKindslenspec = string.Join(", ", _obj.WorkKindsAvisavis.Where(x => x.Region != null).Select(x => x.Region.Name).ToArray<string>());
      }
    }

    public override void NonresidentChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      base.NonresidentChanged(e);
      
      // Если компания нерезидент
      if (e.NewValue != true)
      {
        _obj.ForeignCompanyCodeavis = string.Empty;
        _obj.State.Properties.ForeignCompanyCodeavis.IsRequired = false;
        _obj.State.Properties.ForeignCompanyCodeavis.IsEnabled = false;
      }
      else
      {
        _obj.State.Properties.TIN.IsRequired = false;
        _obj.State.Properties.TRRC.IsRequired = false;
        
        _obj.State.Properties.ForeignCompanyCodeavis.IsRequired = true;
        _obj.State.Properties.ForeignCompanyCodeavis.IsEnabled = true;
      }
    }

    /// <summary>
    /// Изменение значения свойства "Группы контрагентов".
    /// </summary>
    /// <param name="e"></param>
    public virtual void GroupCounterpartyavisChanged(lenspec.Etalon.Shared.CompanyGroupCounterpartyavisChangedEventArgs e)
    {
      _obj.CategoryCounterpartyavis = null;
      
      if (e.NewValue != e.OldValue)
      {
        _obj.ResultApprovalDEBavis = lenspec.Etalon.Company.ResultApprovalDEBavis.NotAssessed;
        _obj.Nonresident = false;
        _obj.State.Properties.Nonresident.IsVisible = false;
      }
      
      if (e.NewValue == null)
        return;
      // Если Федеральные органы госвласти, Региональный органы госвласти, Компания Группы Эталон,
      // то поле Результат согласования ДБ автоматически заполняется значением «Не требует согласования».
      if (e.NewValue != null &&
          e.NewValue.IdDirectum5 == 17896396 ||
          e.NewValue.IdDirectum5 == 17896402 ||
          e.NewValue.IdDirectum5 == 6 )
        _obj.ResultApprovalDEBavis = lenspec.Etalon.Company.ResultApprovalDEBavis.DoesNotReqAppr;
    }
  }
  
  // Конец добавлено avis
}