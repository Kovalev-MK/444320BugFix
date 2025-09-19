using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Memo;

namespace lenspec.Etalon
{
  partial class MemoAddresseesAddresseePropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> AddresseesAddresseeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.AddresseesAddresseeFiltering(query, e);
      
      var hasRightsToSelectAnyEmployees = false;
      e.Params.TryGetValue(lenspec.Etalon.Constants.Docflow.Memo.Params.HasRightsToSelectAnyEmployees, out hasRightsToSelectAnyEmployees);
      
      var businessUnit = Sungero.Docflow.Memos.As(_obj.RootEntity).BusinessUnit;
      if (businessUnit != null && !hasRightsToSelectAnyEmployees)
        query = query.Where(x => x.Department != null && x.Department.BusinessUnit != null && x.Department.BusinessUnit.Equals(businessUnit));
      
      return query;
    }
  }

  partial class MemoServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (Users.Current.IsSystem != true && _obj.InternalApprovalState != Sungero.Docflow.Memo.InternalApprovalState.Aborted)
      {
        var incorrectBU = new List<string>();
        var notAuthomatedEmployees = new List<string>();
        var hasRightsToSelectAnyEmployees = false;
        if (!e.Params.TryGetValue(Constants.Docflow.Memo.Params.HasRightsToSelectAnyEmployees, out hasRightsToSelectAnyEmployees))
        {
          var roleSid = lenspec.EtalonDatabooks.PublicConstants.Module.RightsToSelectAnyEmployees;
          hasRightsToSelectAnyEmployees = Users.Current.IncludedIn(roleSid);
          e.Params.Add(Constants.Docflow.Memo.Params.HasRightsToSelectAnyEmployees, hasRightsToSelectAnyEmployees);
        }

        foreach(var addressee in _obj.Addressees)
        {
          // Если НОР сотрудника не совпадает с НОР СЗ.
          if (
            !hasRightsToSelectAnyEmployees &&   // Роль с правом выбора адресатов вне НОР.
            _obj.BusinessUnit != null &&
            !Equals(_obj.BusinessUnit, addressee.Addressee.Department.BusinessUnit)
           )
            incorrectBU.Add($"{addressee.Addressee.Name}, ИД {addressee.Addressee.Id}");
          
          // Если сотрудник неавтоматизированный.
          if (!lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(addressee.Addressee))
            notAuthomatedEmployees.Add($"{addressee.Addressee.Name}, ИД {addressee.Addressee.Id}");
        }
        var errors = new List<string>();
        if (incorrectBU.Any())
          errors.Add(lenspec.Etalon.Memos.Resources.EmployeeHasIncorrectBusinessUnitFormat(string.Join(",\r\n", incorrectBU)));
        
        if (notAuthomatedEmployees.Any())
          errors.Add(lenspec.Etalon.Memos.Resources.ContainsNotAutomatedEmployeesFormat(string.Join(",\r\n", notAuthomatedEmployees)));
        
        if (errors.Any())
        {
          e.AddError(string.Join("\r\n", errors));
          return;
        }
      }
      base.BeforeSave(e);
    }
  }

  partial class MemoOurSignatoryPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public override IQueryable<T> OurSignatoryFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.OurSignatoryFiltering(query, e);
      
      var hasRightsToSelectAnyEmployees = false;
      e.Params.TryGetValue(lenspec.Etalon.Constants.Docflow.Memo.Params.HasRightsToSelectAnyEmployees, out hasRightsToSelectAnyEmployees);
      
      if (_obj.BusinessUnit != null && !hasRightsToSelectAnyEmployees)
        query = query.Where(x => x.Department != null && x.Department.BusinessUnit != null && x.Department.BusinessUnit.Equals(_obj.BusinessUnit));
      
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class MemoAddresseePropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public override IQueryable<T> AddresseeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.AddresseeFiltering(query, e);
      
      var hasRightsToSelectAnyEmployees = false;
      e.Params.TryGetValue(lenspec.Etalon.Constants.Docflow.Memo.Params.HasRightsToSelectAnyEmployees, out hasRightsToSelectAnyEmployees);
      
      if (_obj.BusinessUnit != null && !hasRightsToSelectAnyEmployees)
        query = query.Where(x => x.Department != null && x.Department.BusinessUnit != null && x.Department.BusinessUnit.Equals(_obj.BusinessUnit));
      
      return query;
    }
    //конец Добавлено Avis Expert
  }


  partial class MemoDepartmentPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public override IQueryable<T> DepartmentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.DepartmentFiltering(query, e);
      
      var hasRightsToSelectAnyEmployees = false;
      e.Params.TryGetValue(lenspec.Etalon.Constants.Docflow.Memo.Params.HasRightsToSelectAnyEmployees, out hasRightsToSelectAnyEmployees);
      
      if (_obj.BusinessUnit != null && !hasRightsToSelectAnyEmployees)
      {
        query = query.Where(x => x.BusinessUnit != null && x.BusinessUnit.Equals(_obj.BusinessUnit));
      }
      return query;
    }
    //конец Добавлено Avis Expert
  }


}