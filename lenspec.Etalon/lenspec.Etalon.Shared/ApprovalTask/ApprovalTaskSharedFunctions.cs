using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalTask;

namespace lenspec.Etalon.Shared
{
  partial class ApprovalTaskFunctions
  {

    /// <summary>
    /// Проверить наличие согласуемого документа в задаче и наличие хоть каких-то прав на него.
    /// </summary>
    /// <returns>True, если с документом можно работать.</returns>
    public override bool HasDocumentAndCanRead()
    {
      return base.HasDocumentAndCanRead();
    }
    
    //Добавлено Avis Expert
    
    private Sungero.Company.IEmployee GetManager(Sungero.Company.IEmployee employee)
    {
      return lenspec.Etalon.PublicFunctions.Employee.GetManagerOrEmployee(employee);
    }
    
    /// <summary>
    /// Установить видимость и значение поля Руководитель инициатора.
    /// </summary>
    public void SetHeadOfTheInitiator()
    {
      try
      {
        var stages = _obj.ApprovalRule.Stages.Select(x => x.Stage).Where(x => x != null).ToList();
        var containsHeadOfTheInitiator = stages.Any(s => s.ApprovalRole != null && (s.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ManagerTaskCard ||
                                                                                    s.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.InitManager) ||
                                                    s.ApprovalRoles.Any(r => r.ApprovalRole != null && r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ManagerTaskCard) ||
                                                    s.ApprovalRoles.Any(r => r.ApprovalRole != null && r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.InitManager));
        
        _obj.State.Properties.HeadOfTheInitiatorlenspec.IsVisible = _obj.State.Properties.HeadOfTheInitiatorlenspec.IsRequired =
          stages.Any(s => s.ApprovalRole != null && (s.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ManagerTaskCard ||
                                                     s.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.InitManager) ||
                     s.ApprovalRoles.Any(r => r.ApprovalRole != null && r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.ManagerTaskCard) ||
                     s.ApprovalRoles.Any(r => r.ApprovalRole != null && r.ApprovalRole.Type == EtalonDatabooks.ComputedRole.Type.InitManager));
        
        if (containsHeadOfTheInitiator && avis.EtalonIntergation.Employees.Is(_obj.Author))
        {
          var manager = GetManager(Sungero.Company.Employees.As(_obj.Author));
          _obj.HeadOfTheInitiatorlenspec = avis.EtalonIntergation.Employees.As(manager);
        }
        else
        {
          _obj.HeadOfTheInitiatorlenspec = null;
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - SetHeadOfTheInitiator - task {0} - ", ex, _obj.Id);
      }
    }
    //конец Добавлено Avis Expert
  }
}