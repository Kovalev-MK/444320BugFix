using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ActionItemExecutionTask;

namespace lenspec.Etalon.Shared
{
  partial class ActionItemExecutionTaskFunctions
  {
    /// <summary>
    /// ��������� ������������ ���������� �������� �����.
    /// </summary>
    /// <param name="e">��������� ��������.</param>
    /// <returns>True - ���� �������� ����� ��������� ���������, ����� - false.</returns>
    public override bool ValidateActionItemAssignedBy(Sungero.Core.IValidationArgs e)
    {
      if (Users.Current.IsSystem != true && _obj?.AssignedBy?.Person == Employees.Current.Person)
        return true;
      
      return base.ValidateActionItemAssignedBy(e);
    }
  }
}