using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.ActsOfManagementCompanyBase;

namespace lenspec.SalesDepartmentArchive.Client
{
  partial class ActsOfManagementCompanyBaseActions
  {
    public virtual void StarmToActs(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверки возможности преобразования и наложения отметки.
      if (_obj.ActDate == null)
        e.AddError("Не заполнено поле, \"Дата акта\".");
      
      if (string.IsNullOrEmpty(_obj.RegistrationNumber))
        e.AddError("Не заполнено поле, \"Рег номер\".");
      
      if (_obj.BusinessUnit == null)
        e.AddError("Не заполнено поле, \"Наша организация\".");
      //
      var position = Functions.ActsOfManagementCompanyBase.ShowAddRegistrationStampDialog(_obj);
      if (position == null)
        return;
      
      var registrationDate = _obj?.RegistrationDate.Value.ToString("dd.MM.yyyy");
      var actName = $"Акт № {_obj.RegistrationNumber} от {registrationDate}";
      string stampHtml = Resources.ActsOfManagementCompanyStampHtml; //(_obj?.BusinessUnit?.Name, actName);
      stampHtml = stampHtml.Replace("{0}", _obj?.BusinessUnit?.Name);
      stampHtml = stampHtml.Replace("{1}", actName);
      
      var result = PublicFunctions.ActsOfManagementCompanyBase.StarmpAvis(_obj, position.RightIndent, position.BottomIndent, stampHtml);
       
      // Успешная интерактивная конвертация.
      if (string.IsNullOrEmpty(result))
      {
        Dialogs.NotifyMessage(Sungero.Docflow.OfficialDocuments.Resources.ConvertionDone);
        return;
      }
      else
      {
        Dialogs.NotifyMessage("Ошибка простановки штампа");
        return;
      }
    }

    public virtual bool CanStarmToActs(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Если входим в роль "Полные права на акты управляющих компаний", делаем доступной кнопку.. 
      if (Sungero.Company.Employees.Current.IncludedIn(SalesDepartmentArchive.Constants.Module.FullRightsActsOfManagementCompany) || Sungero.Company.Employees.Current.IncludedIn(Roles.Administrators))
        return true;
      
      return false;
    }
  }
}