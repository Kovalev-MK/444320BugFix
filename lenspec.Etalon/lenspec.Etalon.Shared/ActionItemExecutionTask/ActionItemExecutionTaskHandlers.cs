using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ActionItemExecutionTask;

namespace lenspec.Etalon
{
  partial class ActionItemExecutionTaskSharedHandlers
  {
    public override void AssignedByChanged(Sungero.RecordManagement.Shared.ActionItemExecutionTaskAssignedByChangedEventArgs e)
    {
      base.AssignedByChanged(e);
      
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (Users.Current.IsSystem == true || Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
        return;
      
      // Если изменился выдал, очищаем поле исполнитель.
      if (!Equals(e.NewValue, e.OldValue))
        _obj.Assignee = null;
    }

    public override void DocumentsGroupAdded(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      base.DocumentsGroupAdded(e);
      
      #region Обращение клиента
      
      var custReq = avis.CustomerRequests.CustomerRequests.As(_obj.DocumentsGroup.OfficialDocuments.FirstOrDefault());
      if (custReq != null)
      {
        _obj.IsCompoundActionItem = false;
        _obj.CoAssignees.Clear();
        _obj.ActiveText = string.Empty;
        if (custReq.ReqCategory != null)
        {
          _obj.ActiveText += custReq.ReqCategory.Name + ".";
        }
        if (!string.IsNullOrEmpty(custReq.Subject))
        {
          _obj.ActiveText += " " + custReq.Subject;
        }
        if (custReq.ObjectOfProject != null && custReq.ObjectOfProject.OurCF != null)
        {
          _obj.OurCFlenspec = custReq.ObjectOfProject.OurCF;
        }
        else
        {
          _obj.OurCFlenspec = null;
        }
        
        var setups = avis.CustomerRequests.CustReqSetups.GetAll(x => x.Status == avis.CustomerRequests.CustReqSetup.Status.Active)
          .Where(x => x.BUCollection.Any(b => b.BusinessUnit.Id == custReq.BusinessUnit.Id) && x.CustReqCategory.Equals(custReq.ReqCategory));
        if (custReq.ObjectOfProject != null && custReq.ObjectOfProject.OurCF != null)
        {
          if (setups.Where(x => custReq.ObjectOfProject.OurCF.Equals(x.OurCF)).Any())
          {
            setups = setups.Where(x => custReq.ObjectOfProject.OurCF.Equals(x.OurCF));
          }
          else
          {
            setups = setups.Where(x => x.OurCF == null);
          }
        }
        
        var categRow = setups.FirstOrDefault();
        if (categRow != null)
        {
          _obj.IsUnderControl = true;
          if (categRow.Controller != null)
            _obj.Supervisor = categRow.Controller;
          if (categRow.Executor != null)
            _obj.Assignee = categRow.Executor;
          if (categRow.DaysForReview != null)
            _obj.Deadline = Calendar.Now.AddWorkingDays((int)categRow.DaysForReview);
        }
      }
      
      #endregion
    }
  }
}