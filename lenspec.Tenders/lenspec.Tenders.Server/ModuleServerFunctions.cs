using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Tenders.Server
{
  public class ModuleFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Отфильтровать по дате создания с учетом пользовательского часового пояса.
    /// </summary>
    /// <param name="query">Исходный запрос.</param>
    /// <param name="periodBegin">Дата начала периода.</param>
    /// <param name="periodEnd">Дата окончания периода.</param>
    /// <returns>Отфильтрованный по дате запрос.</returns>
    [Public]
    public virtual IQueryable<ICounterpartyRegisterBase> ApplyFilterByCreatedDate(IQueryable<ICounterpartyRegisterBase> query,
                                                                                  DateTime periodBegin, DateTime periodEnd)
    {
      var serverPeriodBegin = Equals(Calendar.SqlMinValue, periodBegin) ? periodBegin : Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(periodBegin);
      var serverPeriodEnd = Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd : periodEnd.EndOfDay().FromUserTime();
      
      // Если временные оффсеты клиента и сервера не отличаются, то отфильтровать документы по периоду.
      if (TenantInfo.UtcOffset == Users.UtcOffsetOfCurrent)
      {
        query = query.Where(j => j.Created.Between(serverPeriodBegin, serverPeriodEnd));
      }
      else
      {
        // Если временные оффсеты клиента и сервера отличаются.
        var clientPeriodEnd = !Equals(Calendar.SqlMaxValue, periodEnd) ? periodEnd.AddDays(1) : Calendar.SqlMaxValue;
        query = query.Where(j => (j.Created.Between(serverPeriodBegin, serverPeriodEnd) ||
                                  j.Created == periodBegin) && j.Created != clientPeriodEnd);
      }
      
      return query;
    }
    
    /// <summary>
    /// Перевести список параметров строку для отчета
    /// </summary>
    /// <param name="workKindGroups">Виды работ</param>
    /// <param name="workKinds">Детализация видов работ</param>
    /// <param name="materialGroups">Виды материалов</param>
    /// <param name="materials">Детализация материалов</param>
    /// <returns></returns>
    [Public]
    public static string ConvertCollectionParams(System.Collections.Generic.List<avis.EtalonParties.IWorkGroup> workKindGroups, System.Collections.Generic.List<avis.EtalonParties.IWorkKind> workKinds,
                                                 System.Collections.Generic.List<avis.EtalonParties.IMaterialGroup> materialGroups, System.Collections.Generic.List<avis.EtalonParties.IMaterial> materials,
                                                 System.Collections.Generic.List<Sungero.Commons.IRegion> regions, System.Collections.Generic.List<Sungero.Commons.ICity> cities)
    {
      var result = string.Empty;
      if (workKindGroups != null && workKindGroups.Any())
      {
        var workKindGroupNames = string.Join(", ", workKindGroups.Select(x => x.Name));
        result += string.Format("Виды работ: {0}", workKindGroupNames);
      }
      if (workKinds != null && workKinds.Any())
      {
        var workKindNames = string.Join(", ", workKinds.Select(x => x.Name));
        result += string.Format("{0}Детализация видов работ: {1}", '\n', workKindNames);
      }
      if (materialGroups != null && materialGroups.Any())
      {
        var materialGroupNames = string.Join(", ", materialGroups.Select(x => x.Name));
        result += string.Format("{0}Виды материалов: {1}", '\n', materialGroupNames);
      }
      if (materials != null && materials.Any())
      {
        var materialNames = string.Join(", ", materials.Select(x => x.Name));
        result += string.Format("{0}Детализация материалов: {1}", '\n', materialNames);
      }
      if (regions != null && regions.Any())
      {
        var regionNames = string.Join(", ", regions.Select(x => x.Name));
        result += string.Format("{0}Регионы: {1}", '\n', regionNames);
      }
      if (cities != null && cities.Any())
      {
        var cityNames= string.Join(", ", cities.Select(x => x.Name));
        result += string.Format("{0}Населенные пункты: {1}", '\n', cityNames);
      }
      return result;
    }
    
    /// <summary>
    /// Создать Протокол комитета аккредитации.
    /// </summary>
    /// <returns>Протокол комитета аккредитации.</returns>
    [Remote]
    public static Tenders.IAccreditationCommitteeProtocol CreateAccreditationCommitteeProtocol()
    {
      return Tenders.AccreditationCommitteeProtocols.Create();
    }
    
    /// <summary>
    /// Создать Анкету квалификации.
    /// </summary>
    /// <returns>Анкета квалификации.</returns>
    [Remote]
    public static Tenders.ITenderAccreditationForm CreateTenderAccreditationForm()
    {
      return Tenders.TenderAccreditationForms.Create();
    }
    
    /// <summary>
    /// Создать Протокол тендерного комитета.
    /// </summary>
    /// <returns>Протокол тендерного комитета.</returns>
    [Remote]
    public static Tenders.ITenderCommitteeProtocol CreateTenderCommitteeProtocol()
    {
      return Tenders.TenderCommitteeProtocols.Create();
    }
    //конец Добавлено Avis Expert
    
    /// <summary>
    /// Создать документ Тендерная документация
    /// </summary>
    /// <returns>Тендерная документация.</returns>
    [Remote]
    public static Tenders.ITenderDocument CreateTenderDocument()
    {
      return Tenders.TenderDocuments.Create();
    }
  }
}