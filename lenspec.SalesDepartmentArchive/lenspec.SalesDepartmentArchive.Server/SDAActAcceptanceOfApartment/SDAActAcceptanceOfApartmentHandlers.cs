using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDAActAcceptanceOfApartment;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDAActAcceptanceOfApartmentClientContractPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> ClientContractFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // query = base.ClientContractFiltering(query, e);
      // Если клиент не указан, то выдаём все договора.
      if (_obj.ClientNew == null || _obj.ClientNew.Count == 0)
        return query;
      
      // Получаем список клиентов
      var clients = new List<Sungero.Parties.ICounterparty>();
      
      foreach (var client in _obj.ClientNew)
      {
        if (client.Client != null)
          clients.Add(client.Client);
      }
      
      // Отображаем клиентские договоры, только с данным клиентом.
      query = query.Where(c => c.CounterpartyClient.Where(cl => cl != null).FirstOrDefault() != null);
      
      query = query.Where(c => c.CounterpartyClient.All(x => clients.Contains(x.ClientItem)));
      
      query = query.Where(c => c.CounterpartyClient.Count() > 0);
      
      query = query.Where(c => c.CounterpartyClient.Count() == clients.Count());
      
      return query;
    }
  }

  partial class SDAActAcceptanceOfApartmentDocumentKindPropertyFilteringServerHandler<T>
  {
    /// <summary>
    /// Фильтрация выбора из спика "Вид документа."
    /// </summary>
    /// <param name="query"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override IQueryable<T> DocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var actAcceptanceOfApartmentTypeGuid = lenspec.SalesDepartmentArchive.PublicConstants.Module.ActAcceptanceOfApartmentTypeGuid;
      var actAcceptanceOfApartment = Sungero.Docflow.DocumentTypes.GetAll(t => t.DocumentTypeGuid == actAcceptanceOfApartmentTypeGuid.ToString()).FirstOrDefault();
      query = query.Where(q => q.DocumentType == actAcceptanceOfApartment);
      
      return query;
    }
  }
}