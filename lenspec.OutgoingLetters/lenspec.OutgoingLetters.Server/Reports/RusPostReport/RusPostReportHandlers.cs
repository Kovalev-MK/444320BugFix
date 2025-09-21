using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters
{
  partial class RusPostReportServerHandlers
  {
    
    //Добавлено Avis Expert
    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.RusPostReport.SourceTableName, RusPostReport.ReportSessionId);
    }

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      var reportSessionId = System.Guid.NewGuid().ToString();
      RusPostReport.ReportSessionId = reportSessionId;
      var documents = Enumerable.Empty<Etalon.IOutgoingLetter>().AsQueryable();
      AccessRights.AllowRead(() => { documents = this.GetOutgoingLetter(); });
      this.WriteToOutgoingLettersTable(documents, reportSessionId);
    }

    /// <summary>
    /// Получить исходящие письма, удовлетворяющие заданным параметрам отчета.
    /// </summary>
    /// <returns>Исходящие письма, подходящие под параметры отчета.</returns>
    public virtual IQueryable<Etalon.IOutgoingLetter> GetOutgoingLetter()
    {
      try
      {
        var correspondentIds = RusPostReport.Addressee.Select(x => x.Id).ToList();
        var businessUnitIds = RusPostReport.BusinessUnit.Select(x => x.Id).ToList();
        
        var letters = Etalon.OutgoingLetters.GetAll()
          .Where(l => l.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered)
          .Where(l => l.RegistrationDate >= RusPostReport.BeginDate)
          .Where(l => l.RegistrationDate <= RusPostReport.EndDate)
          .Where(l => l.DeliveryMethod != null &&
                 Etalon.MailDeliveryMethods.As(l.DeliveryMethod).SendingByMailavis.HasValue &&
                 Etalon.MailDeliveryMethods.As(l.DeliveryMethod).SendingByMailavis.Value);
        if (businessUnitIds.Any())
        {
          letters = letters.Where(l => businessUnitIds.Contains(l.BusinessUnit.Id));
        }
        if (correspondentIds.Any())
        {
          letters = letters.Where(l => l.IsManyAddressees != true && correspondentIds.Contains(l.Correspondent.Id) ||
                                  (l.IsManyAddressees == true && l.Addressees.Any(x => correspondentIds.Contains(x.Correspondent.Id))));
        }
        
        return letters.OrderBy(l => l.RegistrationNumber);
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - RusPostReport - GetOutgoingLetter - ", ex);
        return null;
      }
    }
    
    /// <summary>
    /// Заполнить отчет данными об исходящих письмах.
    /// </summary>
    /// <param name="documents">Исходящие письма.</param>
    /// <param name="reportSessionId">ИД сессии отчета.</param>
    public virtual void WriteToOutgoingLettersTable(IQueryable<Etalon.IOutgoingLetter> documents, string reportSessionId)
    {
      if (documents == null)
        return;
      
      try
      {
        var outgoingLetters = new List<Structures.RusPostReport.TableLine>();
        var lineNumber = 0;
        var deliveryMethod = string.Empty;
        foreach (var document in documents.ToList())
        {
          if (document.IsManyAddressees == true)
          {
            foreach(var addressee in document.Addressees)
            {
              lineNumber++;
              deliveryMethod = addressee.DeliveryMethod != null ? addressee.DeliveryMethod.Name : string.Empty;
              if (Etalon.People.Is(addressee.Correspondent))
              {
                outgoingLetters.Add(this.CreateTabliLine(reportSessionId, lineNumber, Etalon.People.As(addressee.Correspondent), document, deliveryMethod));
                continue;
              }
              else if (Etalon.CompanyBases.Is(addressee.Correspondent))
              {
                outgoingLetters.Add(this.CreateTabliLine(reportSessionId, lineNumber, Etalon.CompanyBases.As(addressee.Correspondent), document, deliveryMethod));
              }
            }
          }
          else
          {
            lineNumber++;
            deliveryMethod = document.DeliveryMethod != null ? document.DeliveryMethod.Name : string.Empty;
            if (Etalon.People.Is(document.Correspondent))
            {
              outgoingLetters.Add(this.CreateTabliLine(reportSessionId, lineNumber, Etalon.People.As(document.Correspondent), document, deliveryMethod));
            }
            else if (Etalon.CompanyBases.Is(document.Correspondent))
            {
              outgoingLetters.Add(this.CreateTabliLine(reportSessionId, lineNumber, Etalon.CompanyBases.As(document.Correspondent), document, deliveryMethod));
            }
          }
        }
        
        Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.RusPostReport.SourceTableName, outgoingLetters);
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Avis - RusPostReport - WriteToOutgoingLettersTable - ", ex);
      }
    }
    
    private Structures.RusPostReport.TableLine CreateTabliLine(string reportSessionId, int lineNumber, Etalon.IPerson person, Etalon.IOutgoingLetter document, string deliveryMethod)
    {
      // ФИО, № договора
      var correspondentAndContractNumber = person.Name;
      if (document.ClientContractlenspec != null)
      {
        correspondentAndContractNumber += ", " + document.ClientContractlenspec.ClientDocumentNumber;
      }
      
      // Рег. номер от Рег. дата, Содержание
      var registrationDataAndSubject = string.Empty;
      if (!string.IsNullOrEmpty(document.RegistrationNumber))
      {
        registrationDataAndSubject += document.RegistrationNumber;
      }
      if (document.RegistrationDate != null)
      {
        registrationDataAndSubject += " от " + document.RegistrationDate.Value.ToString("d");
      }
      if (!string.IsNullOrEmpty(document.Subject))
      {
        registrationDataAndSubject += ", " + document.Subject;
      }
      
      var index = person.Indexavis ?? string.Empty;
      var region = person.Regionavis ?? string.Empty;
      var city = person.Cityavis ?? string.Empty;
      var address = person.HomeAddressavis ?? string.Empty;
      var phones = person.Phones ?? string.Empty;
      
      var bussinesUnit = string.Empty;
      var returnAddress = string.Empty;
      if (document.BusinessUnit != null)
      {
        bussinesUnit = document.BusinessUnit.LegalName ?? string.Empty;
        returnAddress = document.BusinessUnit.PostalAddress ?? string.Empty;
      }
      
      return Structures.RusPostReport.TableLine.Create(reportSessionId,
                                                       lineNumber,
                                                       correspondentAndContractNumber,
                                                       registrationDataAndSubject,
                                                       index,
                                                       region,
                                                       city,
                                                       address,
                                                       bussinesUnit,
                                                       deliveryMethod,
                                                       returnAddress,
                                                       string.Empty,
                                                       string.Empty,
                                                       phones);
    }
    
    private Structures.RusPostReport.TableLine CreateTabliLine(string reportSessionId, int lineNumber, Etalon.ICompanyBase counterparty, Etalon.IOutgoingLetter document, string deliveryMethod)
    {
      // ФИО, № договора
      var correspondentAndContractNumber = counterparty.Name;
      
      // Рег. номер от Рег. дата, Содержание
      var registrationDataAndSubject = string.Empty;
      if (!string.IsNullOrEmpty(document.RegistrationNumber))
      {
        registrationDataAndSubject += document.RegistrationNumber;
      }
      if (document.RegistrationDate != null)
      {
        registrationDataAndSubject += " от " + document.RegistrationDate.Value.ToString("d");
      }
      if (!string.IsNullOrEmpty(document.Subject))
      {
        registrationDataAndSubject += ", " + document.Subject;
      }
      
      var index = counterparty.Indexavis ?? string.Empty;
      var region = counterparty.Region != null ? counterparty.Region.Name : string.Empty;
      var city = counterparty.City != null ? counterparty.City.Name : string.Empty;
      var address = counterparty.Streetavis ?? string.Empty;
      var phones = counterparty.Phones ?? string.Empty;
      
      var bussinesUnit = string.Empty;
      var returnAddress = string.Empty;
      if (document.BusinessUnit != null)
      {
        bussinesUnit = document.BusinessUnit.LegalName ?? string.Empty;
        returnAddress = document.BusinessUnit.PostalAddress ?? string.Empty;
      }
      
      return Structures.RusPostReport.TableLine.Create(reportSessionId,
                                                       lineNumber,
                                                       correspondentAndContractNumber,
                                                       registrationDataAndSubject,
                                                       index,
                                                       region,
                                                       city,
                                                       address,
                                                       bussinesUnit,
                                                       deliveryMethod,
                                                       returnAddress,
                                                       string.Empty,
                                                       string.Empty,
                                                       phones);
    }
    //конец Добавлено Avis Expert

  }
}