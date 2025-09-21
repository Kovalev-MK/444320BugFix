using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderCommitteeProtocol;

namespace lenspec.Tenders.Client
{
  internal static class TenderCommitteeProtocolAddresseesStaticActions
  {

    public static void SaveToAcquaintanceList(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = TenderCommitteeProtocols.As(e.Entity);
      var acquaintanceList = Sungero.RecordManagement.PublicFunctions.Module.Remote.CreateAcquaintanceList();
      
      foreach (var addressee in obj.Addressees)
      {
        var newParticipantRow = acquaintanceList.Participants.AddNew();
        newParticipantRow.Participant = addressee.Addressee;
      }
      acquaintanceList.Show();
    }
    
    public static bool CanSaveToAcquaintanceList(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }

    public static void FillFromAcquaintanceList(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = TenderCommitteeProtocols.As(e.Entity);
      var acquaintanceLists = Sungero.RecordManagement.PublicFunctions.Module.Remote.GetAcquaintanceLists();
      var acquaintanceList = acquaintanceLists.ShowSelect();
      var errorMessage = Functions.TenderCommitteeProtocol.TryFillFromAcquaintanceList(obj, acquaintanceList);
      if (!string.IsNullOrWhiteSpace(errorMessage))
        Dialogs.NotifyMessage(errorMessage);
    }

    public static bool CanFillFromAcquaintanceList(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = TenderCommitteeProtocols.As(e.Entity);
      return obj.State.Properties.Addressees.IsEnabled;
    }

    public static void ClearAddressees(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = TenderCommitteeProtocols.As(e.Entity);
      obj.Addressees.Clear();
    }

    public static bool CanClearAddressees(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = TenderCommitteeProtocols.As(e.Entity);
      return obj.Addressees.Any();
    }


    public static bool CanFillInDivision(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      var obj = TenderCommitteeProtocols.As(e.Entity);
      if (obj.FillEmpDepartment == true && obj.AddressessDepartments.Count >= 1 && obj.FillOption != null)
        return true;
      else
        return false;
    }

    public static void FillInDivision(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = TenderCommitteeProtocols.As(e.Entity);
      var participants = new List<IRecipient>();
      var departments = obj.AddressessDepartments;
      var employees = new List<Sungero.Company.IEmployee>();
      if (obj.FillOption == FillOption.AllEmployess)
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
      if (obj.FillOption == FillOption.OnlyManagers)
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
      Functions.TenderCommitteeProtocol.DeleteDublicatesAddressees(obj);
    }
  }

  partial class TenderCommitteeProtocolActions
  {
    public override void SaveAndClose(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.Addressees == null || !_obj.Addressees.Any())
      {
        e.AddError(lenspec.Tenders.TenderCommitteeProtocols.Resources.IsEmptyAddressees);
        return;
      }
      else
        Functions.TenderCommitteeProtocol.DeleteDublicatesAddresses(_obj);
      base.SaveAndClose(e);
    }

    public override bool CanSaveAndClose(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSaveAndClose(e);
    }

    public override void Save(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
      if (_obj.Addressees == null || !_obj.Addressees.Any())
      {
        e.AddError(lenspec.Tenders.TenderCommitteeProtocols.Resources.IsEmptyAddressees);
        return;
      }
      else
        Functions.TenderCommitteeProtocol.DeleteDublicatesAddresses(_obj);
      base.Save(e);
    }

    public override bool CanSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSave(e);
    }





    
    //Добавлено Avis Expert
    public override void ImportInLastVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ImportInLastVersion(e);
    }

    public override bool CanImportInLastVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanImportInLastVersion(e) && Users.Current.IncludedIn(Constants.Module.ScanOfTenderCommitteeProtocolCreatingRole);
    }

    public override void ImportInNewVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ImportInNewVersion(e);
    }

    public override bool CanImportInNewVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanImportInNewVersion(e) && Users.Current.IncludedIn(Constants.Module.ScanOfTenderCommitteeProtocolCreatingRole);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e) && Users.Current.IncludedIn(Constants.Module.TenderCommitteeProtocolCreatingRole);
    }

    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.HasVersions)
      {
        Dialogs.ShowMessage(lenspec.Tenders.TenderCommitteeProtocols.Resources.NeedCreateDocumentVersion);
        return;
      }
      
      if (lenspec.Etalon.PublicFunctions.ApprovalTask.CheckDuplicates(_obj, false))
        return;
      
      base.SendForApproval(e);
    }

    public override void CreateFromFile(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateFromFile(e);
    }

    public override bool CanCreateFromFile(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCreateFromFile(e) && Users.Current.IncludedIn(Constants.Module.ScanOfTenderCommitteeProtocolCreatingRole);
    }
    //конец Добавлено Avis Expert

  }

}