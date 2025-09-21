using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.AccreditationCommitteeProtocol;

namespace lenspec.Tenders.Client
{
  internal static class AccreditationCommitteeProtocolAddresseesStaticActions
  {

    public static bool CanClearAddressees(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = AccreditationCommitteeProtocols.As(e.Entity);
      return obj.Addressees.Any();
    }

    public static void ClearAddressees(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = AccreditationCommitteeProtocols.As(e.Entity);
      obj.Addressees.Clear();
    }

    public static bool CanFillFromDepartment(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = AccreditationCommitteeProtocols.As(e.Entity);
      return obj.IsFillingByDepartmentEmployees == true &&
        obj.DepartmentsForAddressees.Count >= 1 && obj.AddresseesFilingOption != null;
    }

    public static void FillFromDepartment(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = AccreditationCommitteeProtocols.As(e.Entity);
      var participants = new List<IRecipient>();
      var departments = obj.DepartmentsForAddressees;
      var employees = new List<Sungero.Company.IEmployee>();
      #region Все сотрудники
      
      if (obj.AddresseesFilingOption == AccreditationCommitteeProtocol.AddresseesFilingOption.AllEmployees)
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
          var newParticipantRow = obj.Addressees.AddNew();
          newParticipantRow.Addressee = employee;
        }
      }
      
      #endregion
      
      #region Только руководители
      
      if (obj.AddresseesFilingOption == AccreditationCommitteeProtocol.AddresseesFilingOption.OnlyManagers)
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
              var newParticipantRow = obj.Addressees.AddNew();
              newParticipantRow.Addressee = employee;
            }
          }
        }
      }
      
      #endregion
      
      Functions.AccreditationCommitteeProtocol.DeleteDublicatesAddressees(obj);
    }

    public static bool CanFillFromAcquaintanceList(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = AccreditationCommitteeProtocols.As(e.Entity);
      return obj.State.Properties.Addressees.IsEnabled;
    }

    public static void FillFromAcquaintanceList(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = AccreditationCommitteeProtocols.As(e.Entity);
      var acquaintanceLists = Sungero.RecordManagement.PublicFunctions.Module.Remote.GetAcquaintanceLists();
      var acquaintanceList = acquaintanceLists.ShowSelect();
      var errorMessage = Functions.AccreditationCommitteeProtocol.TryFillFromAcquaintanceList(obj, acquaintanceList);
      if (!string.IsNullOrWhiteSpace(errorMessage))
        Dialogs.NotifyMessage(errorMessage);
    }

    public static bool CanSaveToAcquaintanceList(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }

    public static void SaveToAcquaintanceList(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = AccreditationCommitteeProtocols.As(e.Entity);
      var acquaintanceList = Sungero.RecordManagement.PublicFunctions.Module.Remote.CreateAcquaintanceList();
      
      foreach (var addressee in obj.Addressees)
      {
        var newParticipantRow = acquaintanceList.Participants.AddNew();
        newParticipantRow.Participant = addressee.Addressee;
      }
      acquaintanceList.Show();
    }
  }

  partial class AccreditationCommitteeProtocolActions
  {
    public override void SaveAndClose(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.SaveAndClose(e);
    }

    public override bool CanSaveAndClose(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSaveAndClose(e);
    }

    public override void Save(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Save(e);
    }

    public override bool CanSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSave(e);
    }


    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.HasVersions)
      {
        e.AddError(lenspec.Tenders.AccreditationCommitteeProtocols.Resources.NeedCreateVersion);
        return;
      }
      
      if (lenspec.Etalon.PublicFunctions.ApprovalTask.CheckDuplicates(_obj, false))
        return;
      
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

  }

}