using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.FlowFolderSetting;

namespace lenspec.EtalonDatabooks
{
  partial class FlowFolderSettingAssignmentTypeByTaskAssignmentDeliveryMethodPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public virtual IQueryable<T> AssignmentTypeByTaskAssignmentDeliveryMethodFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var allowedItems = new List<long>();
      if (_obj.AssignmentType != null && _obj.AssignmentType.AssignmentTypeGuid.Equals(EtalonDatabooks.Constants.Module.FlowFoldersSetting.ApprovalSendingAssignmentGuid))
      {
        return query;
      }
      else
      {
        return query.Where(x => allowedItems.Contains(x.Id));
      }
    }
    //конец Добавлено Avis Expert
  }

  partial class FlowFolderSettingAssignmentTypeByTaskAssignmentTypePropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public virtual IQueryable<T> AssignmentTypeByTaskAssignmentTypeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_root.TaskType != null)
      {
        query = query.Where(x => x.TaskType.Equals(_root.TaskType));
      }
      return query;
    }
    //конец Добавлено Avis Expert
  }

}