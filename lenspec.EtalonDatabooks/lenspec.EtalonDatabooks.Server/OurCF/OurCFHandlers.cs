using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.OurCF;


namespace lenspec.EtalonDatabooks
{

  partial class OurCFServerHandlers
  {
    /// <summary>
    /// Создание.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      // Обнуляем логические поля.
      _obj.IsSale = false;
      _obj.IsComputeApprovalers = false;
      // Если создаём копирование, очищаем поля ид интеграций.
      if (_obj.State.IsCopied)
      {
        _obj.IdInvest = null;
        _obj.Code1C = null;
      }
    }
    
    /// <summary>
    /// До сохранения
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.IsEditProperyInvest == true)
      {
        // Создать асинхронный обработчик GrantAccessRightsToDocument.
        var asyncHandler = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncEditOurCFavis.Create();

        // Заполнить параметры асинхронного обработчика.
        asyncHandler.Id = _obj.Id;
        asyncHandler.IdInvest = _obj.IdInvest;
        asyncHandler.CommercialName = _obj.CommercialName;
       
        // Вызвать асинхронный обработчик.
        asyncHandler.ExecuteAsync();
        
        _obj.IsEditProperyInvest = false;
      }
      
      // Если Вычилять согласующих из объекта, то очищает т.ч. Согласующие
      if (_obj.IsComputeApprovalers == true)
      {
        _obj.CollectionCoordinators.Clear();
      }
    }
  }

  partial class OurCFAgrDevelopmentPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> AgrDevelopmentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Development != null)
        query = query.Where(x => Equals(x.BusinessUnit, _obj.Development));
      
      return query;
    }
  }
}