using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.EtalonContracts.AttachmentContractDocument;

namespace avis.EtalonContracts.Shared
{
  partial class AttachmentContractDocumentFunctions
  {
    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var name = "";
      if (_obj.DocumentKind != null)
        name = $"{_obj.DocumentKind.Name} ";
      
      if (!string.IsNullOrEmpty(_obj.RegistrationNumber))
        name += $"№{_obj.RegistrationNumber} ";
      
      if (_obj.RegistrationDate != null)
        name += $"от {_obj.RegistrationDate.Value.ToString("dd.MM.yyyy")} ";

      if (_obj.LeadingDocument != null)
        name += $"к {_obj.LeadingDocument.Name}";
      
      _obj.Name = name;
      _obj.Nameavis = name;
    }
    
    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();

      _obj.State.Properties.Subject.IsRequired = false;
      _obj.State.Properties.Department.IsRequired = false;
    }
  }
}