using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Contract;

namespace lenspec.Etalon.Shared
{
  partial class ContractFunctions
  {
    
    /// <summary>
    /// Обновление в зависимости от галочки "Отсрочка платежа".
    /// </summary>
    [Public]
    public void IsDeferredPayment()
    {
      var isDeferredPayment = _obj.IsDeferredPaymentlenspec == true;
      _obj.State.Properties.DaysOfDefermentlenspec.IsVisible = isDeferredPayment;
      _obj.State.Properties.DaysOfDefermentlenspec.IsRequired = isDeferredPayment;
    }
    
    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var name = "";
      if (_obj.DocumentKind != null)
        name = $"{_obj.DocumentKind?.Name} ";
      
      if (!string.IsNullOrEmpty(_obj.RegistrationNumber))
        name += $"№{_obj.RegistrationNumber} ";
      
      if (_obj.RegistrationDate != null)
        name += $"от {_obj.RegistrationDate.Value.ToString("dd.MM.yyyy")} ";
      
      if (_obj.TotalAmount != null)
        name += $"на сумму {_obj.TotalAmount} ";
      
      if (_obj.Currency != null)
        name += $"{_obj.Currency.ShortName} ";
      
      if (_obj.Counterparty != null)
        name += $"между {_obj.Counterparty.Name} ";
      
      if (_obj.BusinessUnit != null)
        name += $"и {_obj.BusinessUnit.Name} ";
      
      if (_obj.DocumentGroup != null)
        name += $"категория {_obj.DocumentGroup.Name} ";
      
      if (_obj.ContractKindavis != null)
        name += $"вид {_obj?.ContractKindavis?.Name} ";
      
      if (!string.IsNullOrEmpty(_obj.SubjectContractavis))
        name += $"предмет договора {_obj?.SubjectContractavis}";

      if (_obj.OurCFavis != null)
        name += $", объект {_obj.OurCFavis.Name}";
      
      _obj.Name = name;
      _obj.Nameavis = name;
    }
    
    /// <summary>
    /// Обновление в зависимости от галочки "Рамочный".
    /// </summary>
    [Public]
    public void IsFramework()
    {
      if (_obj.IsFrameworkavis == true)
      {
        PublicFunctions.ContractualDocument.Edit0Nds(_obj);
        
        _obj.State.Properties.VatRate.IsEnabled = false;
        _obj.State.Properties.TotalAmount.IsEnabled = false;
        _obj.State.Properties.ConstructionObjectsavis.Properties.Summ.IsEnabled = false;
        
        if (_obj.TotalAmount == null || _obj.TotalAmount > 0)
          _obj.TotalAmount = 0;
        
        foreach (var constructionObject in _obj.ConstructionObjectsavis)
        {
          if (constructionObject.Summ == null || constructionObject.Summ > 0)
            constructionObject.Summ = 0;
        }
        
        return;
      }
      
      if (_obj?.ContractTypeavis?.Name == lenspec.Etalon.ContractualDocuments.Resources.Gratuitous)
        return;
      
      _obj.State.Properties.VatRate.IsEnabled = true;
      _obj.State.Properties.TotalAmount.IsEnabled = true;
      _obj.State.Properties.ConstructionObjectsavis.Properties.Summ.IsEnabled = true;
      
      PublicFunctions.ContractualDocument.BlockTotalAmmount(_obj);
    }
    
    /// <summary>
    /// Установить обязательность свойств в зависимости от заполненных данных.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();

      _obj.State.Properties.Subject.IsRequired = false;
      _obj.State.Properties.DocumentGroup.IsRequired = false;
    }
  }
}