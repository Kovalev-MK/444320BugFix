using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Tenders.Server
{

  partial class SystemicQualificationOfCounterpartiesFolderHandlers
  {

    public virtual IQueryable<Sungero.Workflow.IAssignmentBase> SystemicQualificationOfCounterpartiesDataQuery(IQueryable<Sungero.Workflow.IAssignmentBase> query)
    {
      var result = EtalonDatabooks.PublicFunctions.Module.GetAssignmentsByFolderSettings(query, EtalonDatabooks.FlowFolderSetting.FolderName.SysQualOfCntprt);
      return result;
    }
    
    public virtual bool IsSystemicQualificationOfCounterpartiesVisible()
    {
      return true;
    }
  }

  partial class QualificationDocumentsFolderHandlers
  {
    public virtual IQueryable<Sungero.Docflow.IDocumentKind> QualificationDocumentsDocumentKindFiltering(IQueryable<Sungero.Docflow.IDocumentKind> query)
    {
      /* Если необходимо фильтровать доступные виды документов по видам для следующих типов:
       * Тендерная документация, Анкета квалификации, Сведения о контрагенте,
       * раскомментировать.
      var availableDocumentTypeGuids = new List<string>(){
        lenspec.Tenders.PublicConstants.Module.TenderDocumentTypeGuid,
        lenspec.Tenders.PublicConstants.Module.TenderCommitteeProtocolTypeGuid,
        lenspec.Etalon.PublicConstants.Docflow.CounterpartyDocument.CouterpartyDocumentTypeGuid
      };
      
      return query.Where(x => availableDocumentTypeGuids.Contains(x.DocumentType.DocumentTypeGuid));
       */
      
      return query;
    }

    public virtual IQueryable<lenspec.Etalon.IOfficialDocument> QualificationDocumentsDataQuery(IQueryable<lenspec.Etalon.IOfficialDocument> query)
    {
      
      // Скрытая фильтрация по КА из контекста вызова.
      var counterpartyPrefiltered = Sungero.Parties.Counterparties.Null;
      // Папка открыта из реестра подрядчиков/поставщиков?
      if (CallContext.CalledFrom(Tenders.CounterpartyRegisterBases.Info))
      {
        var id = CallContext.GetCallerEntityId(Tenders.CounterpartyRegisterBases.Info);
        var counterpartyRegisterBase = Tenders.CounterpartyRegisterBases.Get(id);
        // Выбираем КА из реестра.
        counterpartyPrefiltered = counterpartyRegisterBase.Counterparty;
        query = FilterByCounterparty(query, counterpartyPrefiltered);
      }
      
      var approvalCounterpartyRegisterTasks = new List<Tenders.IApprovalCounterpartyRegisterTask>().AsQueryable();
      AccessRights.AllowRead(
        () =>
        {
          // Задачи с типом "Задача на согласование включения в реестр квалифицированных контрагентов/список дисквалифицированных контрагентов".
          approvalCounterpartyRegisterTasks = Tenders.ApprovalCounterpartyRegisterTasks.GetAll();
        });
      query = query.Where(
        document =>
        document.IsQualificationDocumentlenspec == true ||
        approvalCounterpartyRegisterTasks.Any(task => task.AttachmentDetails.Any(d => d.EntityId == document.Id))
       );
      
      // Фильтрация.
      if (_filter == null)
        return query;
      
      // Фильтр по виду документа.
      if (_filter.DocumentKind != null)
        query = query.Where(d => Equals(d.DocumentKind, _filter.DocumentKind));
      
      // Фильтр по автору.
      if (_filter.Author != null)
        query = query.Where(d => Equals(d.Author, _filter.Author));
      
      // Фильтр по НОР.
      if (_filter.BusinessUnit != null)
        query = query.Where(d => Equals(d.BusinessUnit, _filter.BusinessUnit));
      
      //      // Фильтр по контрагенту (превалирует над контекстом).
      //      if (_filter.Counterparty != null)
      //        query = FilterByCounterparty(query, _filter.Counterparty);
      //      // Фильтрация по контексту (если имеется).
      //      else
      //        // Фильтрация по контексту (если имеется).
      //        query = FilterByCounterparty(query, counterpartyPrefiltered);
      
      // Фильтр по интервалу времени
      var periodBegin = _filter.DateRangeFrom ?? Calendar.SqlMinValue;
      var periodEnd = _filter.DateRangeTo ?? Calendar.SqlMaxValue;
      
      if (_filter.LastWeek)
        periodBegin = Calendar.UserToday.AddDays(-7);
      
      if (_filter.LastMonth)
        periodBegin = Calendar.UserToday.AddDays(-30);
      
      if (_filter.Last90Days)
        periodBegin = Calendar.UserToday.AddDays(-90);
      
      if (_filter.ManualPeriod)
      {
        periodBegin = _filter.DateRangeFrom ?? Calendar.SqlMinValue;
        periodEnd = _filter.DateRangeTo ?? Calendar.SqlMaxValue;
      }
      
      var serverPeriodBegin = Equals(Calendar.SqlMinValue, periodBegin) ? periodBegin : Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(periodBegin);
      var serverPeriodEnd = Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd : periodEnd.EndOfDay().FromUserTime();
      var clientPeriodEnd = !Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd.AddDays(1) : Calendar.SqlMaxValue;
      query = query.Where(d => (d.DocumentDate.Between(serverPeriodBegin, serverPeriodEnd) ||
                                d.DocumentDate == periodBegin) && d.DocumentDate != clientPeriodEnd);
      
      return query;
    }
    
    /// <summary>
    /// Получить квалификационные документы по КА.
    /// </summary>
    /// <param name="query">Квалификационные документы.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <returns>Отфильтрованные по КА документы.</returns>
    private static IQueryable<lenspec.Etalon.IOfficialDocument> FilterByCounterparty(IQueryable<lenspec.Etalon.IOfficialDocument> query, Sungero.Parties.ICounterparty counterparty)
    {
      if (counterparty == null)
        return query;
      
      return query.Where(
        d =>
        //        (
        //          !Tenders.TenderDocuments.Is(d) &&             // Тендерная документация.
        //          !Tenders.TenderCommitteeProtocols.Is(d) &&    // Тендерная документация.
        //          !Tenders.TenderAccreditationForms.Is(d) &&    // Анкета квалификации.
        //          !Sungero.Docflow.CounterpartyDocuments.Is(d)  // Сведения о КА.
        //         ) ||
        //        (
        Tenders.TenderDocuments.Is(d)               && Tenders.TenderDocuments.As(d).Counterparties.Any(c => Equals(c.Counterparty, counterparty)) ||
        Tenders.TenderCommitteeProtocols.Is(d)      && Tenders.TenderCommitteeProtocols.As(d).Counterparties.Any(c => Equals(c.Counterparty, counterparty)) ||
        Tenders.TenderAccreditationForms.Is(d)      && Equals(Tenders.TenderAccreditationForms.As(d).Counterparty, counterparty) ||
        Sungero.Docflow.CounterpartyDocuments.Is(d) && Equals(Sungero.Docflow.CounterpartyDocuments.As(d).Counterparty, counterparty)
        //         )
       );
    }
    
  }

  partial class TendersHandlers
  {
  }
}