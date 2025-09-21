using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters
{
  partial class CheckLetterDataReportServerHandlers
  {

    //Добавлено Avis Expert
    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.CheckLetterDataReport.SourceTableName, CheckLetterDataReport.ReportSessionId);
    }

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      var reportSessionId = System.Guid.NewGuid().ToString();
      CheckLetterDataReport.ReportSessionId = reportSessionId;
      var dataTable = new List<Structures.CheckLetterDataReport.TableLine>();
      
      var massMailingApplication = CheckLetterDataReport.Entity;
      // Система находит запись справочника «Объекты проекта», указанную в карточке заявки на массовую рассылку из которой был вызван отчет.
      var objectAnProject = massMailingApplication.ObjectAnProject;
      if (objectAnProject != null)
      {
        // Находит все документы типа «Клиентский договор», в полях которых указан «Объект» с карточки Заявки на массовую рассылку.
        var clientContracts = SalesDepartmentArchive.SDAClientContracts
          .GetAll(x => x.LifeCycleState == lenspec.SalesDepartmentArchive.SDAClientContract.LifeCycleState.Active && x.ObjectAnProject != null && x.ObjectAnProject.Equals(objectAnProject))
          .AsEnumerable();
        
        // Если заполнены клиентские договора, то берем их оттуда.
        if (massMailingApplication.CollectionClientContract.Count() > 0)
        {
          var sdaClientContracts = new List<lenspec.SalesDepartmentArchive.ISDAClientContract>();
          foreach (var clientContract in massMailingApplication.CollectionClientContract)
            sdaClientContracts.Add(clientContract.ClientContract);
          
          clientContracts = sdaClientContracts.AsEnumerable();
        }

        if (massMailingApplication.MailingType.ChangeOfMeasurements.HasValue && massMailingApplication.MailingType.ChangeOfMeasurements.Value == true)
        {
          clientContracts = clientContracts
            .Where(x => x.Premises != null && (x.Premises.EditSquere.HasValue && !x.Premises.EditSquere.Value.Equals(0.00) &&
                                               x.Premises.EditPrice.HasValue && !x.Premises.EditPrice.Value.Equals(0.00)))
            .AsEnumerable();
        }
        foreach(var clientContract in clientContracts)
        {
          // Вычисляет записи справочника «Персоны», связанных с этими договорами (по полю «Клиент»).
          var people = clientContract.CounterpartyClient.Where(x => Sungero.Parties.People.Is(x.ClientItem)).Select(x => Sungero.Parties.People.As(x.ClientItem)).ToList<Sungero.Parties.IPerson>();
          foreach(var person in people)
          {
            // На каждую найденную запись справочника «Персоны» + «Клиентский договор» формирует 1 строчку в отчете.
            var tableLine = Structures.CheckLetterDataReport.TableLine.Create();
            tableLine.ReportSessionId = reportSessionId;
            tableLine.AddresseeFIO = person.ShortName ?? person.Name;
            tableLine.ContractNumber = clientContract.ClientDocumentNumber;
            tableLine.ContractDate = clientContract.ClientDocumentDate.HasValue ? clientContract.ClientDocumentDate.Value.ToString("d") : string.Empty;
            tableLine.ObjectAnProject = objectAnProject.Name;
            tableLine.NameRNV = objectAnProject.NameRNV;
            tableLine.AddressRNV = objectAnProject.AddressRNV;
            var objectAnSale = clientContract.Premises;
            if (objectAnSale != null)
            {
              tableLine.ReductionSizeS = objectAnSale.ReductionSizeS;
              tableLine.ZoomSizeS = objectAnSale.ZoomSizeS;
              tableLine.AmountOfSurchargeInWords = objectAnSale.SurchargeAmount != null ? objectAnSale.AmountInWords : string.Empty;
              tableLine.AmountOfRefundInWords = objectAnSale.RefundAmount != null ? objectAnSale.AmountInWords : string.Empty;
            }
            tableLine.DateRNV = objectAnProject.EnterAnObjectPermit != null && objectAnProject.EnterAnObjectPermit.DateRNV.HasValue ? objectAnProject.EnterAnObjectPermit.DateRNV.Value.ToString("d") : string.Empty;
            tableLine.NumberRNV = objectAnProject.EnterAnObjectPermit != null ? objectAnProject.EnterAnObjectPermit.NumberRNV : string.Empty;
            tableLine.RepresentativeByPowerOfAttorneyNumber = massMailingApplication.RepresentativeByPowerOfAttorneyNumber;
            tableLine.PowerOfAttorneyDate = massMailingApplication.PowerOfAttorneyDate.HasValue ? massMailingApplication.PowerOfAttorneyDate.Value.ToString("d") : string.Empty;
            tableLine.DeliveryMethod = massMailingApplication.DeliveryMethod != null ? massMailingApplication.DeliveryMethod.Name : string.Empty;
            tableLine.Email = person.Email;
            tableLine.CorrespondentPostalAddress = person.PostalAddress;
            tableLine.RegistrationDate = string.Empty;
            tableLine.RegistrationNumber = string.Empty;
            var businessUnit = massMailingApplication.BusinessUnit;
            if (businessUnit != null)
            {
              tableLine.BusinessUnit = businessUnit.Name;
              tableLine.LegalAddress = businessUnit.LegalAddress;
              tableLine.PostalAddress = businessUnit.PostalAddress;
              tableLine.PSRN = businessUnit.PSRN;
              tableLine.TIN = businessUnit.TIN;
              tableLine.TRRC = businessUnit.TRRC;
            }
            tableLine.SettlementAccount = massMailingApplication.SettlementAccount;
            tableLine.Bank = massMailingApplication.Bank != null ? massMailingApplication.Bank.Name : string.Empty;
            tableLine.CorrespondentAccount = massMailingApplication.CorrespondentAccount;
            tableLine.BIC = massMailingApplication.BIC;
            if (massMailingApplication.OurSignatory != null)
            {
              tableLine.OurSignatoryFIO = massMailingApplication.OurSignatory.Name;
              tableLine.OurSignatoryJobTitle = massMailingApplication.OurSignatory.JobTitle != null
                ? massMailingApplication.OurSignatory.JobTitle.Name
                : string.Empty;
            }
            tableLine.InitiatorFIO = massMailingApplication.Author.Name;
            tableLine.Phone = massMailingApplication.PreparedBy != null ? massMailingApplication.PreparedBy.Phone : string.Empty;
            
            dataTable.Add(tableLine);
          }
        }
      }
      Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.CheckLetterDataReport.SourceTableName, dataTable);
    }
    //конец Добавлено Avis Expert

  }
}