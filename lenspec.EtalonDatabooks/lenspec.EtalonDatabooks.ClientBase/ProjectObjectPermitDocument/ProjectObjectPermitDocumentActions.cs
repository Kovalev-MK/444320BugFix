using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectObjectPermitDocument;

namespace lenspec.EtalonDatabooks.Client
{
  // Добавлено avis.
  
  partial class ProjectObjectPermitDocumentActions
  {
    public override void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      lenspec.EtalonDatabooks.ProjectObjectPermitDocuments.GetAll(p => p.Id != _obj.Id && p.NumberRNV == _obj.NumberRNV && p.DateRNV == _obj.DateRNV).Show();
    }

    public override bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanShowDuplicates(e);
    }

    public override void FillObjectAnProject(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Обновляем Объекты проектов.
      if (_obj.State.IsChanged || _obj.State.IsInserted)
      {
        e.AddError(lenspec.EtalonDatabooks.ProjectObjectPermitDocuments.Resources.NeedSaveDocumentCard);
        return;
      }
      //TODO: добавить обработку на сохранение карточек.
      foreach(var objectAnProject in _obj.CollectionObjectAnProject)
      {
        objectAnProject.ObjectAnProject.EnterAnObjectPermit = _obj;
        objectAnProject.ObjectAnProject.NumberRNV = _obj.NumberRNV;
        objectAnProject.ObjectAnProject.DateRNV = _obj.DateRNV;
        objectAnProject.ObjectAnProject.AddressRNV = _obj.AddressRNV;
        objectAnProject.ObjectAnProject.Save();
      }
      
      foreach (var accountingObject in _obj.CollectionAccountingProject)
      {
        accountingObject.AccountingProject.EnterAnObjectPermit = _obj;
        accountingObject.AccountingProject.NumberRNV = _obj.NumberRNV;
        accountingObject.AccountingProject.DateRNV = _obj.DateRNV;
        accountingObject.AccountingProject.AddressRNV = _obj.AddressRNV;
        accountingObject.AccountingProject.Save();
      }
    }

    public override bool CanFillObjectAnProject(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanFillObjectAnProject(e);
    }

    /// <summary>
    ///  Кнопка "Объекты проекта по разрешению".
    /// </summary>
    /// <param name="e"></param>
    public override void ObjectAnProjectPermitShow(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Открываем список "объекты проектов" в которых указан данный документ.
      EtalonDatabooks.ObjectAnProjects.GetAll(o => o.EnterAnObjectPermit == _obj).Show();
    }

    public override bool CanObjectAnProjectPermitShow(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanObjectAnProjectPermitShow(e);
    }
  }

  // Конец добавлено avis.
}