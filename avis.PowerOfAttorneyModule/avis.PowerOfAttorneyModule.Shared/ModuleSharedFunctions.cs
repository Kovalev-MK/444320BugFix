using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.PowerOfAttorneyModule.Shared
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Найти руководителя департамента. Первый найденный автоматизированный руководитель
    /// </summary>
    /// <param name="department">Департамент</param>
    /// <returns>Руководитель</returns>
    /// <remarks>Если руководитель не указан в подразделении, рекурсивно ищет руководителя в головном подразделении</remarks>
    [Public]
    public Sungero.Company.IEmployee FoundManager(Sungero.Company.IDepartment department)
    {
      if(department.Manager != null &&
         (lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(department.Manager)))
      {
        return department.Manager;
      }
      if(department.HeadOffice == null)
      {
        return null;
      }
      return FoundManager(department.HeadOffice);
    }

    /// <summary>
    /// Найти руководителя департамента. Первый найденный автоматизированный руководитель, не являющийся ГД и не являющийся указанным сотрудником
    /// </summary>
    /// <param name="employee">Сотрудник</param>
    /// <param name="department">Подразделение</param>
    /// <returns></returns>
    [Public]
    public Sungero.Company.IEmployee FoundManagerNotGD(Sungero.Company.IEmployee employee, Sungero.Company.IDepartment department)
    {
      var roleGD = Roles.GetAll(x => x.Sid == Sungero.Docflow.PublicConstants.Module.RoleGuid.BusinessUnitHeadsRole).SingleOrDefault();
      if (department.Manager != null &&
          department.Manager != employee &&
          !department.Manager.IncludedIn(roleGD) &&
          lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(department.Manager))
        return department.Manager;
      
      if (department.HeadOffice == null)
        return null;
      
      return FoundManagerNotGD(employee, department.HeadOffice);
    }
  }
}