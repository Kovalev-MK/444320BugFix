using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OrderBase;

namespace lenspec.Etalon.Client
{

  internal static class OrderBaseAddresseeslenspecStaticActions
  {
    public static void ClearAddresseeslenspec(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = Etalon.OrderBases.As(e.Entity);
      obj.Addresseeslenspec.Clear();
    }

    public static bool CanClearAddresseeslenspec(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = Etalon.OrderBases.As(e.Entity);
      return obj.Addresseeslenspec.Any();
    }

    public static void SaveToAcquaintanceListlenspec(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = OrderBases.As(e.Entity);
      var acquaintanceList = Sungero.RecordManagement.PublicFunctions.Module.Remote.CreateAcquaintanceList();
      
      foreach (var addressee in obj.Addresseeslenspec)
      {
        var newParticipantRow = acquaintanceList.Participants.AddNew();
        newParticipantRow.Participant = addressee.Addressee;
      }
      acquaintanceList.Show();
    }

    public static bool CanSaveToAcquaintanceListlenspec(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }
    
    public static bool CanFillFromAcquaintanceListlenspec(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = OrderBases.As(e.Entity);
      return obj.State.Properties.Addresseeslenspec.IsEnabled;
    }

    public static void FillFromAcquaintanceListlenspec(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = OrderBases.As(e.Entity);
      var acquaintanceLists = Sungero.RecordManagement.PublicFunctions.Module.Remote.GetAcquaintanceLists();
      var acquaintanceList = acquaintanceLists.ShowSelect();
      var errorMessage = Functions.OrderBase.TryFillFromAcquaintanceList(obj, acquaintanceList);
      if (!string.IsNullOrWhiteSpace(errorMessage))
        Dialogs.NotifyMessage(errorMessage);
    }

    public static bool CanFillInDivisionlenspec(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = OrderBases.As(e.Entity);
      if (obj.FillEmpDeplenspec == true && obj.Departmenslenspec.Count >= 1 && obj.FillOptlenspec != null)
        return true;
      else
        return false;
    }

    public static void FillInDivisionlenspec(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = OrderBases.As(e.Entity);
      var participants = new List<IRecipient>();
      var departments = obj.Departmenslenspec;
      var employees = new List<Sungero.Company.IEmployee>();
      if (obj.FillOptlenspec == FillOptlenspec.AllEmployess)
      {
        foreach(var department in departments)
        {
          participants.AddRange(department.Department.RecipientLinks.Select(x => x.Member).ToList());
          var subDeps = lenspec.Etalon.Module.Company.PublicFunctions.Module.GetSubDepartments(department.Department);
          foreach(var subDep in subDeps)
          {
            participants.AddRange(subDep.RecipientLinks.Select(x => x.Member).ToList());
          }
        }
        if (participants.Count > 0)
        {
          foreach (var participant in participants)
          {
            if(participant.Status == Sungero.Company.Employee.Status.Active)
            {
              employees.Add(Sungero.Company.Employees.As(participant));
            }
          }
        }
        foreach (var employee in employees)
        {
          var newParticipantRow = obj.Addresseeslenspec.AddNew();
          newParticipantRow.Addressee = employee;
        }
      }
      if (obj.FillOptlenspec == FillOptlenspec.OnlyManager)
      {
        foreach(var departmentLine in departments)
        {
          //Основное подразделение
          var employeesByJobTitle = lenspec.Etalon.PublicFunctions.Employee.Remote.GetEmployeesFromDepartmentByJobTitleKind(departmentLine.Department, lenspec.Etalon.OrderBases.Resources.JobTitleKindAndRoleName);
          employees.AddRange(employeesByJobTitle);
          if (departmentLine.Department.Manager != null && departmentLine.Department.Manager.Status == Sungero.Company.Employee.Status.Active)
            employees.Add(departmentLine.Department.Manager);
          //Сотрудники из нижестоящих подразделений
          var subDeps = lenspec.Etalon.Module.Company.PublicFunctions.Module.GetSubDepartments(departmentLine.Department);
          foreach(var subDep in subDeps)
          {
            if (subDep.Manager != null)
              employees.Add(subDep.Manager);
            var employeesByJobTitleFromSubDepartment = lenspec.Etalon.PublicFunctions.Employee.Remote.GetEmployeesFromDepartmentByJobTitleKind(subDep, lenspec.Etalon.OrderBases.Resources.JobTitleKindAndRoleName);
            employees.AddRange(employeesByJobTitleFromSubDepartment);
          }
        }
        if (employees.Count > 0)
        {
          foreach (var employee in employees)
          {
            if(employee.Status == Sungero.Company.Employee.Status.Active)
            {
              var newParticipantRow = obj.Addresseeslenspec.AddNew();
              newParticipantRow.Addressee = employee;
            }
          }
        }
      }
      Functions.OrderBase.DeleteDublicatesAddressees(obj);
    }
  }

  partial class OrderBaseActions
  {
    public override bool CanSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSave(e);
    }


    public override void SaveAndClose(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.Addresseeslenspec == null || !_obj.Addresseeslenspec.Any())
      {
        e.AddError("Заполните хотя бы одного адресата!");
        return;
      }
      else
        Functions.OrderBase.DeleteDublicatesAddressees(_obj);
      
      base.SaveAndClose(e);
    }

    public override bool CanSaveAndClose(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSaveAndClose(e);
    }

    public override void Save(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.Addresseeslenspec == null || !_obj.Addresseeslenspec.Any())
      {
        e.AddError("Заполните хотя бы одного адресата!");
        return;
      }
      else
        Functions.OrderBase.DeleteDublicatesAddressees(_obj);
      
      base.Save(e);
    }

    public virtual void ChangeManyAddresseeslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.IsManyAddresseeslenspec == false)
      {
        Dialogs.NotifyMessage(Sungero.Docflow.OfficialDocuments.Resources.FillAddresseesOnAddresseesTab);
        _obj.IsManyAddresseeslenspec = true;
      }
      else if (_obj.IsManyAddresseeslenspec == true)
      {
        if (_obj.Addresseeslenspec.Count(a => a.Addressee != null) > 1)
        {
          var addresseeRaw = _obj.Addresseeslenspec.OrderBy(a => a.Number).FirstOrDefault(a => a.Addressee != null);
          var addresseeName = addresseeRaw.Addressee.Person.ShortName;
          var dialog = Dialogs.CreateTaskDialog(Sungero.Docflow.OfficialDocuments.Resources.ChangeManyAddresseesQuestion,
                                                Sungero.Docflow.OfficialDocuments.Resources.ChangeManyAddresseesDescriptionFormat(addresseeName), MessageType.Question);
          dialog.Buttons.AddYesNo();
          if (dialog.Show() == DialogButtons.Yes)
            _obj.IsManyAddresseeslenspec = false;
        }
        else
        {
          _obj.IsManyAddresseeslenspec = false;
        }
      }
    }

    public virtual bool CanChangeManyAddresseeslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.Addresseeslenspec.Any())
      {
        e.AddError("Заполните хотя бы одного адресата!");
        return;
      }
      
      // Если по документу ранее были запущены задачи, то вывести соответствующий диалог.
      if (lenspec.Etalon.PublicFunctions.ApprovalTask.CheckDuplicates(_obj, false))
        return;
      
      // Принудительно сохранить документ, чтобы сохранились связи. Иначе они не попадут в задачу.
      _obj.Save();
      
      // Если у документа нет тела документа, то выведем диалог
      if (!Functions.OrderBase.NeedSendForApproval(_obj))
        return;
      
      this.CreateApprovalTask(e);
    }
    
    public void CreateApprovalTask(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var task = Functions.OrderBase.Remote.CreateApprovalTask(_obj);
      if (task.ApprovalRule != null)
      {
        task.Show();
        e.CloseFormAfterAction = true;
      }
      else
      {
        // Если по документу нет регламента, вывести сообщение.
        Dialogs.ShowMessage(Sungero.Docflow.OfficialDocuments.Resources.NoApprovalRuleWarning, MessageType.Warning);
        throw new OperationCanceledException();
      }
    }
    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }
  }
}