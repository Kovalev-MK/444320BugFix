using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.SupAgreement;

namespace lenspec.Etalon.Shared
{
  partial class SupAgreementFunctions
  {
    /// <summary>
    /// ��������� ��� ���������.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var name = "";
      
      if (_obj.DocumentKind != null)
        name += $"{_obj.DocumentKind.Name} ";
      
      if (!string.IsNullOrEmpty(_obj.RegistrationNumber))
        name += $"�{_obj.RegistrationNumber} ";
      
      if (_obj.RegistrationDate != null)
        name += $"�� {_obj.RegistrationDate.Value.ToString("dd.MM.yyyy")} ";
      
      if (_obj.TotalAmount != null)
        name += $"�� ����� {_obj.TotalAmount} ";
      
      if (_obj.Currency != null)
        name += $"{_obj.Currency.ShortName} ";
      
      if (_obj.LeadingDocument != null)
        name += $"� {_obj.LeadingDocument.Name}";
      
      _obj.Name = name;
      _obj.Nameavis = name;
    }
    
    /// <summary>
    /// ���������� �������������� ������� � ����������� �� ����������� ������.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();

      _obj.State.Properties.Subject.IsRequired = false;
    }
  }
}