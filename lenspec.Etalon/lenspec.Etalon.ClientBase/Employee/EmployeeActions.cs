using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Client
{
  partial class EmployeeActions
  {
    //Добавлено Avis Expert
  
    /// <summary>
    /// Кнопка получить пользователя из АД
    /// </summary>
    /// <param name="e"></param>
    public virtual void GetEmployeeADavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.Employee.DownloadEmployeeADFromIntegraDB(_obj);
    }

    public virtual bool CanGetEmployeeADavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
    
    //конец Добавлено Avis Expert
  }
}