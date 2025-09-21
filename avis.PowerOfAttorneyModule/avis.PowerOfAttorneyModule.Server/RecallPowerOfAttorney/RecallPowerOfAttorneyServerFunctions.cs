using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.RecallPowerOfAttorney;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class RecallPowerOfAttorneyFunctions
  {
    /// <summary>
    /// Обновить либо создать заявление об отказе от полномочий по доверенности.
    /// </summary>
    /// <param name="createNew">Признак необходимости создания нового заявления.</param>
    /// <returns>Заявление об отказе от полномочий по доверенности; Null, если обновить существующее заявление не удалось.</returns>
    [Remote]
    public IApplicationRelinquishmentAuthority UpdateApplicationRelinquishmentAuthority(bool createNew)
    {
      var document = _obj.MainAttachment.ApplicationRelinquishmentAuthorities.SingleOrDefault();
      
      // Обновить нечего, создавать нельзя.
      if (document == null && !createNew)
        return null;
      
      // Создаем новую либо очищаем старую карточку.
      if (document == null)
      {
        document = ApplicationRelinquishmentAuthorities.Create();
        document.Attorney = _obj.Attorney;
        document.NumberAndDate = GetNumberAndDate();
      }
      else
        document.BusinessUnits.Clear();
      
      // Заполняем поля в карточке заявления.
      foreach (var businessUnit in _obj.PowerOfAttorney.PowerOfAttorneys.Select(x => x.BusinessUnit).Distinct())
      {
        var newLine = document.BusinessUnits.AddNew();
        newLine.Company = businessUnit;
      }
      
      return document;
    }
    
    /// <summary>
    /// Заполнить поля "Номер" и "Дата" в заявлении на отзыв доверенности.
    /// </summary>
    [Remote]
    public string FillNumberAndDate(IApplicationRelinquishmentAuthority document)
    {
      if (document == null)
        return string.Empty;
      
      document.NumberAndDate = GetNumberAndDate();
      if (document.HasVersions)
      {
        // Проверка блокировок на документе.
        var lockInfo = Locks.GetLockInfo(document.LastVersion.Body);
        var isLocked = lockInfo == null || lockInfo.IsLocked;
        
        if (isLocked)
          return avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.VersionLockedUpdateFieldsOnStartFormat(lockInfo.OwnerName);

        document.UpdateTemplateParameters();
      }
      
      document.Save();
      return string.Empty;
    }
    
    /// <summary>
    /// Получить дату и № отзываемых доверенностей.
    /// </summary>
    /// <returns>№ и дата отзываемых доверенностей.</returns>
    public string GetNumberAndDate()
    {
      var numAndDates = new List<string>();
      var sortPOAs = _obj.PowerOfAttorney.PowerOfAttorneys.OrderBy(x => x.BusinessUnit.Name);
      foreach (var doc in sortPOAs)
      {
        var poa = lenspec.Etalon.PowerOfAttorneys.As(doc);
        var newString = string.Format("№ {0} от {1}", poa.RegistrationNumber ?? poa.RegistryNumavis, poa.ValidFrom.Value.ToShortDateString());
        numAndDates.Add(newString);
      }
      return string.Join(",\n", numAndDates.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    /// <summary>
    /// Найти дублирующие задачи на отзыв доверенности
    /// </summary>
    /// <returns>Список дубликатов</returns>
    [Remote(IsPure = true)]
    public List<PowerOfAttorneyModule.IRecallPowerOfAttorney> GetTaskDuplicates()
    {
      var tasks = new List<PowerOfAttorneyModule.IRecallPowerOfAttorney>();
      var application = _obj.MainAttachment.ApplicationRelinquishmentAuthorities.SingleOrDefault();
      if (application != null)
      {
        AccessRights.AllowRead(() =>
                               {
                                 var duplicates = RecallPowerOfAttorneys.GetAll(x => !Equals(_obj, x) &&
                                                                                x.Status != Sungero.Workflow.Task.Status.Aborted &&
                                                                                x.AttachmentDetails.Any(d => d.GroupId == Guid.Parse("6cfbf076-f132-4d0d-b5f3-46c4aede1848") &&
                                                                                                        d.AttachmentId == application.Id));
                                 tasks.AddRange(duplicates);
                               });
      }
      return tasks;
    }

  }
}