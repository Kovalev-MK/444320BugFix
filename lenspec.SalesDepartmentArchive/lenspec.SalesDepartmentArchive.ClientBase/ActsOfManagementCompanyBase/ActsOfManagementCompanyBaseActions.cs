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
      // �������� ����������� �������������� � ��������� �������.
      if (_obj.ActDate == null)
        e.AddError("�� ��������� ����, \"���� ����\".");
      
      if (string.IsNullOrEmpty(_obj.RegistrationNumber))
        e.AddError("�� ��������� ����, \"��� �����\".");
      
      if (_obj.BusinessUnit == null)
        e.AddError("�� ��������� ����, \"���� �����������\".");
      //
      var position = Functions.ActsOfManagementCompanyBase.ShowAddRegistrationStampDialog(_obj);
      if (position == null)
        return;
      
      var registrationDate = _obj?.RegistrationDate.Value.ToString("dd.MM.yyyy");
      var actName = $"��� � {_obj.RegistrationNumber} �� {registrationDate}";
      string stampHtml = Resources.ActsOfManagementCompanyStampHtml; //(_obj?.BusinessUnit?.Name, actName);
      stampHtml = stampHtml.Replace("{0}", _obj?.BusinessUnit?.Name);
      stampHtml = stampHtml.Replace("{1}", actName);
      
      var result = PublicFunctions.ActsOfManagementCompanyBase.StarmpAvis(_obj, position.RightIndent, position.BottomIndent, stampHtml);
       
      // �������� ������������� �����������.
      if (string.IsNullOrEmpty(result))
      {
        Dialogs.NotifyMessage(Sungero.Docflow.OfficialDocuments.Resources.ConvertionDone);
        return;
      }
      else
      {
        Dialogs.NotifyMessage("������ ����������� ������");
        return;
      }
    }

    public virtual bool CanStarmToActs(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // ���� ������ � ���� "������ ����� �� ���� ����������� ��������", ������ ��������� ������.. 
      if (Sungero.Company.Employees.Current.IncludedIn(SalesDepartmentArchive.Constants.Module.FullRightsActsOfManagementCompany) || Sungero.Company.Employees.Current.IncludedIn(Roles.Administrators))
        return true;
      
      return false;
    }
  }
}