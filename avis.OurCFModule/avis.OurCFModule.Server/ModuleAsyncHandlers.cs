using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.OurCFModule.Server
{
  public class ModuleAsyncHandlers
  {

    public virtual void AsyncChangingApprovalsInISP(avis.OurCFModule.Server.AsyncHandlerInvokeArgs.AsyncChangingApprovalsInISPInvokeArgs args)
    {
      var employeeCurrent = args.employeeCurrentId;
      //Поменять
      var employeeNew     = Sungero.Company.Employees.GetAll(x => x.Id == long.Parse(args.employeeNewId)).FirstOrDefault();
      var roles           = lenspec.EtalonDatabooks.RoleKinds.GetAll();
      var rolesIds        = args.roleIds.Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToArray();
      roles = roles.Where(x => rolesIds.Contains(x.Id));
      var ourCFs          = lenspec.EtalonDatabooks.OurCFs.GetAll();
      if (!string.IsNullOrEmpty(args.ourCFIds))
      {
        var ids = args.ourCFIds.Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(x => long.Parse(x)).ToArray();
        ourCFs = ourCFs.Where(x => ids.Contains(x.Id));
      }
      
      foreach (var role in roles)
      {
        if (!string.IsNullOrEmpty(employeeCurrent))
        {
          foreach (var ourCF in ourCFs)
          {
            var employee    = Sungero.Company.Employees.GetAll(x => x.Id == long.Parse(employeeCurrent)).FirstOrDefault();
            var currentRoles = ourCF.CollectionCoordinators.Where(x => x.Role == role);
            //Если в найденных ИСП нет Роли = Роль из диалогового окна, то создаем новую строку и заполняем Сотрудника из поля Новый исполнитель и Роль.
            //            if (!currentRoles.Any())
            //            {
            //              var newRole       = ourCF.CollectionCoordinators.AddNew();
            //              newRole.Role      = role;
            //              newRole.Employee  = avis.EtalonIntergation.Employees.As(employeeNew);
            //              ourCF.Save();
            //            }
            //            else
            //            {
            foreach (var currentRole in currentRoles)
            {
              //Если в найденных ИСП есть Роль, но не заполнен Сотрудник, то указываем Нового Сотрудника.
              //              if (currentRole.Employee == null || currentRole.Employee == avis.EtalonIntergation.Employees.As(employee))
              //              {
              //Если в найденных ИСП есть Роль и Сотрудник = Текущий исполнитель, то заменить Сотрудника на Нового исполнителя.
              currentRole.Employee = avis.EtalonIntergation.Employees.As(employeeNew);
              //              }
              //              }
              ourCF.Save();
            }
          }
        }
        else
        {
          foreach (var ourCF in ourCFs)
          {
            var currentRoles  = ourCF.CollectionCoordinators.Where(x => x.Role == role);
            //Если в найденных ИСП нет Роли = Роль из диалогового окна, то создаем новую строку и заполняем Сотрудника из поля Новый исполнитель и Роль.
            if (currentRoles.Count() == 0)
            {
              var newRole       = ourCF.CollectionCoordinators.AddNew();
              newRole.Role      = role;
              newRole.Employee  = avis.EtalonIntergation.Employees.GetAll(x => x.Id == employeeNew.Id).FirstOrDefault();
              ourCF.Save();
            }
            else
            {
              foreach (var currentRole in currentRoles)
              {
                //Если в найденных ИСП есть Роль, но не заполнен Сотрудник, то указываем Нового Сотрудника.
                currentRole.Employee = avis.EtalonIntergation.Employees.GetAll(x => x.Id == employeeNew.Id).FirstOrDefault();
              }
              ourCF.Save();
            }
          }
        }
      }
    }
  }
}