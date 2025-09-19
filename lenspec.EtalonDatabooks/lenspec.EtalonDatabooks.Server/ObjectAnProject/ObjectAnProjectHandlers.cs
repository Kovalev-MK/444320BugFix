using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnProject;

namespace lenspec.EtalonDatabooks
{
  partial class ObjectAnProjectApprovalersEmployeePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ApprovalersEmployeeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var substitutions = Substitutions.GetAll().Where(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      query = query.Where(x => (x.Login != null && x.Login.Status == lenspec.EtalonDatabooks.ObjectAnProject.Status.Active) || 
                          substitutions.Any(s => Equals(x, s.User) && s.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && s.IsSystem == false));
      return query;
    }
  }


  // Добавлено avis.
  partial class ObjectAnProjectServerHandlers
  {

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      bool ourCFChangedParam;
      if (e.Params.TryGetValue(Constants.Module.ObjectAnProjectOurCFHasBeenChangedParam, out ourCFChangedParam) && ourCFChangedParam)
      {
        var asyncHandler = EtalonDatabooks.AsyncHandlers.AsyncObjectAnSaleOurCFFilling.Create();
        asyncHandler.ObjectAnProjectId = _obj.Id;
        asyncHandler.ExecuteAsync();
      }
    }

    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      e.Params.AddOrUpdate(Constants.Module.ObjectAnProjectOurCFHasBeenChangedParam, !Equals(_obj.OurCF, _obj.State.Properties.OurCF.OriginalValue));
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      if (_obj.State.IsCopied)
      {
        _obj.IdInvest = null;
      }
      _obj.IsLinkToInvest = false;
    }
    
    /// <summary>
    /// До сохранения
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.IsEditProperyInvest == true && _obj.IsLinkToInvest == true)
      {
        // Создать асинхронный обработчик GrantAccessRightsToDocument.
        var asyncHandler = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncEditObjectAnProjectvisavis.Create();

        // Заполнить параметры асинхронного обработчика.
        asyncHandler.Id = _obj.Id;
        asyncHandler.IdOurCF = _obj.OurCF.Id;
        if (_obj?.SpecDeveloper?.Name != null)
        {
          asyncHandler.SpecDeveloperName = _obj.SpecDeveloper.Name;
          asyncHandler.SpecDeveloperInn = _obj.SpecDeveloper.TIN;
        }
        asyncHandler.IdInvest = _obj.IdInvest;
        asyncHandler.Name = _obj.Name;
        asyncHandler.AddressBuild = _obj.AddressBuild;
        if (_obj?.BuildingPermit?.Id != null)
          asyncHandler.IdBuildingPermit = _obj.BuildingPermit.Id;
        asyncHandler.NumberRNS = _obj.NumberRNS;
        if (_obj?.DateRNS != null)
          asyncHandler.DateRNS = _obj.DateRNS.Value;
        if (_obj?.EnterAnObjectPermit?.Id != null)
          asyncHandler.IdEnterAnObjectPermit = _obj.EnterAnObjectPermit.Id;
        asyncHandler.NumberRNV = _obj.NumberRNV;
        if (_obj?.DateRNV != null)
          asyncHandler.DateRNV = _obj.DateRNV.Value;
        asyncHandler.NameRNV = _obj.NameRNV;
        asyncHandler.AddressRNV = _obj.AddressRNV;
        
        // Вызвать асинхронный обработчик.
        asyncHandler.ExecuteAsync();
        
        _obj.IsEditProperyInvest = false;
      }
    }
  }

  partial class ObjectAnProjectBuildingPermitPropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация списка разрешений на строительство.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual IQueryable<T> BuildingPermitFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(q => q.SpecDeveloper == _obj.SpecDeveloper);
      
      return query;
    }
  }
  // Конец добавлено avis.
}