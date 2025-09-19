using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OrderBase;

namespace lenspec.Etalon.Server
{
  partial class OrderBaseFunctions
  {
    
    /// <summary>
    /// Создать задачу по процессу "Согласование официального документа".
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Задача по процессу "Согласование официального документа".</returns>
    [Remote(PackResultEntityEagerly = true), Public]
    public static IApprovalTask CreateApprovalTask(IOrderBase document)
    {
      var task = ApprovalTasks.Create();
      //task.DocumentGroup.All.Add(document);
      task.DocumentGroup.OfficialDocuments.Add(document);
      
      return task;
    }
    
    /// <summary>
    /// Получить наши организации для фильтрации подходящих прав подписи.
    /// </summary>
    /// <returns>Наши организации.</returns>
    public override List<Sungero.Company.IBusinessUnit> GetBusinessUnits()
    {
      // Проверка вхождения в роль "Пользователи с правами на указание в документах сотрудников из любых НОР".
      var roleSid = lenspec.EtalonDatabooks.PublicConstants.Module.RightsToSelectAnyEmployees;
      var hasRightsToSelectAnyEmployees = Users.Current.IncludedIn(roleSid);
      
      // Если сотрудник входит в роль, отправляем пустой список НОР в GetSignatureSettingsQuery().
      // Доступные для выбора сотрудники по НОР не фильтруются (см. Functions.SignatureSetting.GetSignatureSettings()).
      if (hasRightsToSelectAnyEmployees)
        return new List<Sungero.Company.IBusinessUnit>() { };
      else
        return base.GetBusinessUnits();
    }
  }
}