using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.EditComponentRXRequestTask;

namespace lenspec.AutomatedSupportTickets
{
  partial class EditComponentRXRequestTaskAuthorPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> AuthorFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Получаем список не автоматизированных сотрудников, и убираем их их списка.
      var users = query.Where(r => (r.Login == null || r.Login.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed) && Sungero.Company.Employees.Is(r));
      if (users == null || !users.Any())
        return query;
      
      var employees = new List<Sungero.Company.IEmployee>();
      foreach (var user in users)
      {
        employees.Add(Sungero.Company.Employees.As(user));
      }
      
      var notAutomatedIds = Sungero.Company.PublicFunctions.Module.Remote.GetNotAutomatedEmployees(employees).ToList().Select(x => x.Id);
      if (notAutomatedIds == null || !notAutomatedIds.Any())
        return query;
      
      query = query.Where(q => !notAutomatedIds.Contains(q.Id));
      return query;
    }
  }

  // Добавлено avis.
  partial class EditComponentRXRequestTaskServerHandlers
  {
    /// <summary>
    /// До сохранения.
    /// </summary>
    /// <param name="e"></param>
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var subject = string.Empty;
      if (_obj.TypeRequest != null)
      {
        subject += _obj.Info.Properties.TypeRequest.GetLocalizedValue(_obj.TypeRequest);
      }
      if (!string.IsNullOrEmpty(_obj.Number))
      {
        subject += " " + _obj.Number;
      }
      if (_obj.DateCreated.HasValue)
      {
        subject += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.DateCreated.Value.ToString("d");
      }
      
      _obj.Subject = subject;
    }
  }
  // Конец добавлено avis.
}