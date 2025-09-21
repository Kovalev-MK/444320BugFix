using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAActBase;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAActBaseSharedHandlers
  {

    public virtual void ActDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      FillName();
    }

    public virtual void ClientChanged(lenspec.SalesDepartmentArchive.Shared.SDAActBaseClientChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue)
        return;
      
      FillName();
      
      if (e.NewValue == null)
      {
        _obj.State.Properties.ClientContract.IsEnabled = false;
        _obj.ClientContract = null;
        return;
      }
      
      // Обнуляем поля, в случае если изменилось значение клиента.
      _obj.State.Properties.ClientContract.IsEnabled = true;
      _obj.BusinessUnit = null;
      
      // Заполняем клиентский договор.
      var clientConsracts = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(c => c.CounterpartyClient.Any(cl => Equals(e.NewValue, cl.ClientItem)));
      if (clientConsracts.Count() == 1)
        _obj.ClientContract = clientConsracts.FirstOrDefault();
      else
        _obj.ClientContract = null;
    }

    public virtual void ClientContractChanged(lenspec.SalesDepartmentArchive.Shared.SDAActBaseClientContractChangedEventArgs e)
    {
      // Привязываем акт, к клиентскому договору.
      _obj.Relations.AddFromOrUpdate(Sungero.Docflow.Constants.Module.SimpleRelationName, e.OldValue, e.NewValue);
      
      // Устанавливаем нашу организацию, из клиентского договора.
      _obj.BusinessUnit = e.NewValue?.BusinessUnit;
      // Устанавливаем помещение, из клиентского договора.
      _obj.Room = e.NewValue?.Premises;
      // Устанавливаем проект, их клиентского договора.
      _obj.OurCF = e.NewValue?.ObjectAnProject?.OurCF;
      
      FillName();
    }
    
    /// <summary>
    /// Изменение значение "Вид документа".
    /// </summary>
    /// <param name="e"></param>
    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      // base.DocumentKindChanged(e);
      FillName();
    }
  }
}