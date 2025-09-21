using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustomerRequestTask;

namespace avis.CustomerRequests.Server
{
  partial class CustomerRequestTaskFunctions
  {
    /// <summary>
    /// Получить ответственного ОЛК.
    /// </summary>
    /// <param name="employee">Сотрудник по которому запущена задача.</param>
    /// <returns>Ответственный отдела кадров.</returns>
    [Public]
    public Sungero.Company.IEmployee GetResponsibleOLK()
    {
      var customerRequest = avis.CustomerRequests.CustomerRequests.As(_obj.AllAttachments.FirstOrDefault());
      if (customerRequest == null)
        return null;

      // Проверяем на наличие нашей организации у сотрудника.
      var businessUnit = lenspec.Etalon.BusinessUnits.GetAll(x => x.Equals(customerRequest.BusinessUnit)).SingleOrDefault();
      if (businessUnit != null)
      {
        // Получаем роль
        var roleKindEmployeeOLK = businessUnit.RoleKindEmployeelenspec;
        if (roleKindEmployeeOLK.Any())
        {
          var performerOlk = roleKindEmployeeOLK.Where(x => x.RoleKind.Name.Equals(avis.CustomerRequests.CustomerRequestTasks.Resources.ResponsibleOLKRoleKindName)).FirstOrDefault();
          if (performerOlk != null)
          {
            return performerOlk.Employee;
          }
        }
      }
      
      return null;
    }
  }
}