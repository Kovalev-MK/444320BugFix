using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Parties.Server
{
  partial class ModuleJobs
  {

    /// <summary>
    /// Контрагенты. Результат согласования ДБ.
    /// </summary>
    public virtual void CounterpartyResultApprovalDEBavis()
    {      
      var logPrefix = "Фоновый процесс Контрагенты. ";
      
      // Обрабатываем компании.
      var companies = lenspec.Etalon.Companies.GetAll(c => c.ResultApprovalDEBavis == null ||
                                                      (c.ResultApprovalDEBavis != lenspec.Etalon.Company.ResultApprovalDEBavis.CoopNotRecomend &&
                                                       c.ResultApprovalDEBavis != lenspec.Etalon.Company.ResultApprovalDEBavis.CoopPossible &&
                                                       c.ResultApprovalDEBavis != lenspec.Etalon.Company.ResultApprovalDEBavis.CoopWithRisks &&
                                                       c.ResultApprovalDEBavis != lenspec.Etalon.Company.ResultApprovalDEBavis.DoesNotReqAppr &&
                                                       c.ResultApprovalDEBavis != lenspec.Etalon.Company.ResultApprovalDEBavis.NotAssessed));

      foreach (var company in companies)
      {
        try
        {
          if (company.GroupCounterpartyavis != null && (company.GroupCounterpartyavis.IdDirectum5 == 17896396 || company.GroupCounterpartyavis.IdDirectum5 == 17896402 ||
                                                        company.GroupCounterpartyavis.IdDirectum5 == 6))
            company.ResultApprovalDEBavis = lenspec.Etalon.Company.ResultApprovalDEBavis.DoesNotReqAppr;
          else
            company.ResultApprovalDEBavis = lenspec.Etalon.Company.ResultApprovalDEBavis.NotAssessed;
          
          company.Save();
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}{1}, ИД {2}: {3}", logPrefix, lenspec.Etalon.Module.Parties.Resources.SetDBApprovalStatus, company?.Id, ex);
        }
      }
      
      // Обрабатываем Банки.
      var banks = lenspec.Etalon.Banks.GetAll(b => b.ResultApprovalDEBavis == null ||
                                              (b.ResultApprovalDEBavis != lenspec.Etalon.Bank.ResultApprovalDEBavis.CoopNotRecomend &&
                                               b.ResultApprovalDEBavis != lenspec.Etalon.Bank.ResultApprovalDEBavis.CoopPossible &&
                                               b.ResultApprovalDEBavis != lenspec.Etalon.Bank.ResultApprovalDEBavis.CoopWithRisks &&
                                               b.ResultApprovalDEBavis != lenspec.Etalon.Bank.ResultApprovalDEBavis.DoesNotReqAppr &&
                                               b.ResultApprovalDEBavis != lenspec.Etalon.Bank.ResultApprovalDEBavis.NotAssessed));

      foreach (var bank in banks)
      {
        try
        {
          bank.ResultApprovalDEBavis = lenspec.Etalon.Bank.ResultApprovalDEBavis.NotAssessed;
          bank.Save();
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}{1}, ИД {2}: {3}", logPrefix, lenspec.Etalon.Module.Parties.Resources.SetDBApprovalStatus, bank?.Id, ex);
        }
      }
      
      // Обрабатываем Персоны.
      var people = lenspec.Etalon.People.GetAll(p => p.ResultApprovalDEBavis == null ||
                                                (p.ResultApprovalDEBavis != lenspec.Etalon.Person.ResultApprovalDEBavis.CoopNotRecomend &&
                                                 p.ResultApprovalDEBavis != lenspec.Etalon.Person.ResultApprovalDEBavis.CoopPossible &&
                                                 p.ResultApprovalDEBavis != lenspec.Etalon.Person.ResultApprovalDEBavis.CoopWithRisks &&
                                                 p.ResultApprovalDEBavis != lenspec.Etalon.Person.ResultApprovalDEBavis.DoesNotReqAppr &&
                                                 p.ResultApprovalDEBavis != lenspec.Etalon.Person.ResultApprovalDEBavis.NotAssessed));

      foreach (var person in people)
      {
        try
        {
          person.ResultApprovalDEBavis = lenspec.Etalon.Person.ResultApprovalDEBavis.NotAssessed;
          person.Save();
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}{1}, ИД {2}: {3}", logPrefix, lenspec.Etalon.Module.Parties.Resources.SetDBApprovalStatus, person?.Id, ex);
        }
      }
      
      // Обработать Агентов продаж.
      var activeCompanies = Sungero.Parties.Companies.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      var salesAgents = activeCompanies.Where(x => x.SalesAgentlenspec.HasValue);
      foreach (var salesAgent in salesAgents)
      {
        var branches = activeCompanies.Where(x => x.HeadCompany == salesAgent &&
                                             (x.SalesAgentlenspec != salesAgent.SalesAgentlenspec || x.ResultApprovalDEBavis != salesAgent.ResultApprovalDEBavis));
        foreach (var company in branches)
        {
          try
          {
            company.SalesAgentlenspec = salesAgent.SalesAgentlenspec;
            company.ResultApprovalDEBavis = salesAgent.ResultApprovalDEBavis;
            company.Save();
          }
          catch (Exception ex)
          {
            Logger.ErrorFormat("{0}{1} для Агентов продаж, ИД {2}: {3}", logPrefix, lenspec.Etalon.Module.Parties.Resources.SetDBApprovalStatus, company?.Id, ex);
          }
        }
      }
    }
  }
}