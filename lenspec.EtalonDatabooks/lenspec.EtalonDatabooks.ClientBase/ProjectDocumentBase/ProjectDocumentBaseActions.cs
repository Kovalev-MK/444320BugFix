using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ProjectDocumentBase;

namespace lenspec.EtalonDatabooks.Client
{
  // Добавлено avis.
  
  partial class ProjectDocumentBaseActions
  {
    public virtual void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    /// <summary>
    /// Действие "Заполнить Объекты проектов".
    /// </summary>
    /// <param name="e"></param>
    public virtual void FillObjectAnProject(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      return;
    }

    public virtual bool CanFillObjectAnProject(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Проверяем что есть права на изменение документа и карточка сохранена.
      return _obj.AccessRights.CanUpdate() && !_obj.State.IsChanged;
    }

    /// <summary>
    /// Кнопка "Объекты проекта по разрешению".
    /// </summary>
    /// <param name="e"></param>
    public virtual void ObjectAnProjectPermitShow(Sungero.Domain.Client.ExecuteActionArgs e)
    {
    }

    public virtual bool CanObjectAnProjectPermitShow(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }
  }
  
  // Конец добавлено avis.
}