using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAActAcceptanceOfApartment;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAActAcceptanceOfApartmentClientNewSharedCollectionHandlers
  {

    public virtual void ClientNewDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      _obj.ClientContract = null;
    }
  }


  partial class SDAActAcceptanceOfApartmentClientNewSharedHandlers
  {

    public virtual void ClientNewClientChanged(lenspec.SalesDepartmentArchive.Shared.SDAActAcceptanceOfApartmentClientNewClientChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue)
        return;
      
      var act = lenspec.SalesDepartmentArchive.SDAActAcceptanceOfApartments.As(_obj.RootEntity);
      
      if (e.NewValue == null)
      {
        act.State.Properties.ClientContract.IsEnabled = false;
        act.ClientContract = null;
        return;
      }
      
      // Обнуляем поля, в случае если изменилось значение клиента.
      act.State.Properties.ClientContract.IsEnabled = true;
      act.BusinessUnit = null;
      
      // Получаем список клиентов
      var clients = new List<Sungero.Parties.ICounterparty>();
      
      foreach (var client in act.ClientNew)
      {
        if (client.Client != null)
          clients.Add(client.Client);
      }
      
      // Заполняем клиентский договор.
      var clientConsracts = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll();
      
      if (clients != null || clients.Count > 0)
      {
        clientConsracts = clientConsracts.Where(c => c.CounterpartyClient.Where(cl => cl != null).FirstOrDefault() != null);
        
        clientConsracts = clientConsracts.Where(c => c.CounterpartyClient.All(x => clients.Contains(x.ClientItem)));
        
        clientConsracts = clientConsracts.Where(c => c.CounterpartyClient.Count() > 0);
        
        clientConsracts = clientConsracts.Where(c => c.CounterpartyClient.Count() == clients.Count());
      }
      
      if (clientConsracts.Count() == 1)
        act.ClientContract = clientConsracts.FirstOrDefault();
      else
        act.ClientContract = null;
    }
  }

  partial class SDAActAcceptanceOfApartmentSharedHandlers
  {

    public virtual void ClientNewChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      FillName();
    }

    public override void ClientChanged(lenspec.SalesDepartmentArchive.Shared.SDAActBaseClientChangedEventArgs e)
    {
      // base.ClientChanged(e);
    }

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      
      // Если Акт осмотра, то отображаем поле "Без дефектов".
      var inspectionAct = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(lenspec.SalesDepartmentArchive.Constants.Module.InspectionAct);
      if (_obj.DocumentKind == inspectionAct)
      {
        _obj.State.Properties.IsNotDefect.IsVisible = true;
        return;
      }
      
      // Если не Акт осмотра, то скрываем поле и ставим false.
      _obj.State.Properties.IsNotDefect.IsVisible = false;
      _obj.IsNotDefect = false;
    }
  }
}