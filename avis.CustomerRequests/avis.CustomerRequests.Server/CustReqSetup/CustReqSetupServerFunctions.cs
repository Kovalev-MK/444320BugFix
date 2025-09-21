using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustReqSetup;

namespace avis.CustomerRequests.Server
{
  partial class CustReqSetupFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Получить дубли настройки.
    /// </summary>
    /// <returns>Настройки исполнения, дублирующие по категории обращения, контролеру, исполнителю, ИСП и нашим организациям.</returns>
    [Remote(IsPure = true)]
    public IQueryable<avis.CustomerRequests.ICustReqSetup> GetDuplicates()
    {
      var duplicates = new List<avis.CustomerRequests.ICustReqSetup>();
      var businessUnits = _obj.BUCollection.Select(x => x.BusinessUnit).ToList().OrderBy(x => x.Id);
      
      var setups = avis.CustomerRequests.CustReqSetups.GetAll(x => x.Status == avis.CustomerRequests.CustReqSetup.Status.Active)
        .Where(x => !Equals(_obj, x) && Equals(_obj.CustReqCategory, x.CustReqCategory) && Equals(_obj.Controller, x.Controller) &&
               Equals(_obj.Executor, x.Executor) && Equals(_obj.OurCF, x.OurCF))
        .ToList();
      foreach(var setup in setups)
      {
        var bu = setup.BUCollection.Select(x => x.BusinessUnit).ToList().OrderBy(x => x.Id);
        if (businessUnits.SequenceEqual(bu))
        {
          duplicates.Add(setup);
        }
      }
      return duplicates.AsQueryable();
    }
    //конец Добавлено Avis Expert
  }
}