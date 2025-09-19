using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectBuildingPermitDocument;

namespace lenspec.EtalonDatabooks.Client
{
  // Добавлено avis.
  
  partial class ProjectBuildingPermitDocumentActions
  {
    public override void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      lenspec.EtalonDatabooks.ProjectBuildingPermitDocuments.GetAll(p => p.Id != _obj.Id && p.NumberRNS == _obj.NumberRNS && p.DateRNS == _obj.DateRNS).Show();
    }

    public override bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanShowDuplicates(e);
    }

    public override void FillObjectAnProject(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.State.IsChanged)
      {
        e.AddError(lenspec.EtalonDatabooks.ProjectObjectPermitDocuments.Resources.NeedSaveDocumentCard);
        return;
      }
      //TODO: добавить обработку на сохранение карточек.
      foreach(var objectAnProject in _obj.CollectionObjectAnProject)
      {
        objectAnProject.ObjectAnProject.BuildingPermit = _obj;
        objectAnProject.ObjectAnProject.NumberRNS = _obj.NumberRNS;
        objectAnProject.ObjectAnProject.DateRNS = _obj.DateRNS;
        objectAnProject.ObjectAnProject.AddressRNS = _obj.AddressRNS;
        objectAnProject.ObjectAnProject.Save();
      }
      
      foreach (var accountingObject in _obj.CollectionAccountingObject)
      {
        accountingObject.AccountingObject.BuildingPermit = _obj;
        accountingObject.AccountingObject.NumberRNS = _obj.NumberRNS;
        accountingObject.AccountingObject.DateRNS = _obj.DateRNS;
        accountingObject.AccountingObject.AddressRNS = _obj.AddressRNS;
        accountingObject.AccountingObject.Save();
      }
    }

    public override bool CanFillObjectAnProject(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanFillObjectAnProject(e);
    }

    /// <summary>
    /// Кнопка Объекты проекта по разрешению.
    /// </summary>
    /// <param name="e"></param>
    public override void ObjectAnProjectPermitShow(Sungero.Domain.Client.ExecuteActionArgs e)
    {     
      EtalonDatabooks.ObjectAnProjects.GetAll(o => o.BuildingPermit == _obj).Show();
    }

    public override bool CanObjectAnProjectPermitShow(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanObjectAnProjectPermitShow(e);
    }
  }
  
  // Конец добавлено avis.
}