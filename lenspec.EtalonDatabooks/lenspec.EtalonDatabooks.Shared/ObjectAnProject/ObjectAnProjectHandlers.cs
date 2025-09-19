using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnProject;

namespace lenspec.EtalonDatabooks
{
  // Добавлено avis.
  
  partial class ObjectAnProjectSharedHandlers
  {

    public virtual void DevelopmentCompanyChanged(lenspec.EtalonDatabooks.Shared.ObjectAnProjectDevelopmentCompanyChangedEventArgs e)
    {
      if (e.NewValue == null)
        _obj.DevelopmentContract = null;
    }

    public virtual void NameChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }

    public virtual void OurCFChanged(lenspec.EtalonDatabooks.Shared.ObjectAnProjectOurCFChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }

    public virtual void AddressMailChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }

    public virtual void AddressBuildChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }

    public virtual void NumberRNSChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }

    public virtual void DateRNSChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }

    public virtual void NumberRNVChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }

    public virtual void DateRNVChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }
    
    /// <summary>
    /// Изменение свойства "Наименование объекта недвижимости в соответствии с РНВ ".
    /// </summary>
    /// <param name="e"></param>
    public virtual void NameRNVChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }
    
    /// <summary>
    /// Изменение свойства "Адрес в соответствии с РНВ".
    /// </summary>
    /// <param name="e"></param>
    public virtual void AddressRNVChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
    }
    
    /// <summary>
    /// Изменение свойства "Спец застройщик".
    /// </summary>
    /// <param name="e"></param>
    public virtual void SpecDeveloperChanged(lenspec.EtalonDatabooks.Shared.ObjectAnProjectSpecDeveloperChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
      
      Functions.ObjectAnProject.Remote.PermitPropertiesIsEnable(_obj);
      
      // Заполняем регион.
      _obj.Region = e.NewValue?.Region;
      
      if (e.NewValue == null)
      {
        _obj.BuildingPermit = null;
        _obj.EnterAnObjectPermit = null;
      }
    }
    
    /// <summary>
    /// Изменение свойства "Разрешение на ввод".
    /// </summary>
    /// <param name="e"></param>
    public virtual void EnterAnObjectPermitChanged(lenspec.EtalonDatabooks.Shared.ObjectAnProjectEnterAnObjectPermitChangedEventArgs e)
    {
      if (e.NewValue == null)
        return;
      
      _obj.NameRNV = e.NewValue.NameObjectRNV;
      _obj.NumberRNV = e.NewValue.NumberRNV;
      _obj.DateRNV = e.NewValue.DateRNV;
      _obj.AddressRNV = e.NewValue.AddressRNV;
    }
    
    /// <summary>
    /// Изменение свойства "Разрешение на строительство".
    /// </summary>
    /// <param name="e"></param>
    public virtual void BuildingPermitChanged(lenspec.EtalonDatabooks.Shared.ObjectAnProjectBuildingPermitChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.IsEditProperyInvest = true;
      
      if (e.NewValue == null)
        return;
      
      _obj.NumberRNS = e.NewValue.NumberRNS;
      _obj.DateRNS = e.NewValue.DateRNS;
    }
  }
  
  // Конец добавлено avis.
}