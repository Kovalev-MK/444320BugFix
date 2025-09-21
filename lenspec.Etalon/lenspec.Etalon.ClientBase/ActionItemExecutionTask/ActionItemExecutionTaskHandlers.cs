using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ActionItemExecutionTask;

namespace lenspec.Etalon
{
  partial class ActionItemExecutionTaskActionItemPartsClientHandlers
  {
    public override void ActionItemPartsAssigneeValueInput(Sungero.RecordManagement.Client.ActionItemExecutionTaskActionItemPartsAssigneeValueInputEventArgs e)
    {
      base.ActionItemPartsAssigneeValueInput(e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (e.NewValue != null && !Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
      {
        var document = lenspec.Etalon.ActionItemExecutionTasks.As(_obj.RootEntity).DocumentsGroup.OfficialDocuments.SingleOrDefault();
        if ((document == null || !avis.CustomerRequests.CustomerRequests.Is(document)) &&
            !EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
        {
          e.AddError(lenspec.Etalon.ActionItemExecutionTasks.Resources.NeedSpecifyAutomatedEmployee);
        }
      }
    }

    public override void ActionItemPartsSupervisorValueInput(Sungero.RecordManagement.Client.ActionItemExecutionTaskActionItemPartsSupervisorValueInputEventArgs e)
    {
      base.ActionItemPartsSupervisorValueInput(e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (e.NewValue != null && !Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
      {
        var document = lenspec.Etalon.ActionItemExecutionTasks.As(_obj.RootEntity).DocumentsGroup.OfficialDocuments.SingleOrDefault();
        if ((document == null || !avis.CustomerRequests.CustomerRequests.Is(document)) &&
            !EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
        {
          e.AddError(lenspec.Etalon.ActionItemExecutionTasks.Resources.NeedSpecifyAutomatedEmployee);
        }
      }
    }
  }

  partial class ActionItemExecutionTaskClientHandlers
  {
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
        return;
      
      // Делаем доступным поле, если в поле "Выдал" персона совпадает с пользователем.
      _obj.State.Properties.AssignedBy.IsEnabled = Users.Current.IsSystem != true && (_obj.AssignedBy == null || _obj.AssignedBy?.Person == Employees.Current.Person);
    }
  }
}