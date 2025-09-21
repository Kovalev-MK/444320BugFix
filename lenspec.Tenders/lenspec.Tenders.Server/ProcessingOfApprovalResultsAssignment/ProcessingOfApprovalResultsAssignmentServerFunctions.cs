using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.ProcessingOfApprovalResultsAssignment;

namespace lenspec.Tenders.Server
{
  partial class ProcessingOfApprovalResultsAssignmentFunctions
  {

    /// <summary>
    /// Закрыть записи из реестра поставщиков.
    /// </summary>
    /// <param name="providers">Поставщики.</param>
    [Remote(IsPure = false)]
    public void CloseProviders(List<Tenders.IProviderRegister> providers, Tenders.ITenderDocument decision)
    {
      foreach (var provider in providers)
      {
        var closedStatus = Sungero.CoreEntities.DatabookEntry.Status.Closed;
        if (provider.Status == closedStatus)
          continue;
        
        provider.Status = closedStatus;
        provider.Closed = Calendar.Now;
        provider.QCDecision = decision;
        provider.Save();
      }
    }
    
    /// <summary>
    /// Закрыть записи из реестра подрядчиков.
    /// </summary>
    /// <param name="contractors">Подрядчики.</param>
    /// <returns></returns>
    [Remote(IsPure = false)]
    public void CloseContractors(List<Tenders.IContractorRegister> contractors, Tenders.ITenderDocument decision)
    {
      foreach (var contractor in contractors)
      {
        var closedStatus = Sungero.CoreEntities.DatabookEntry.Status.Closed;
        if (contractor.Status == closedStatus)
          continue;
        
        contractor.Status = closedStatus;
        contractor.Closed = Calendar.Now;
        contractor.QCDecision = decision;
        contractor.Save();
      }
    }
    
    /// <summary>
    /// Получить анкету квалификации из группы вложений "Анкета квалификации".
    /// </summary>
    /// <returns>Анкета квалификации.</returns>
    [Public, Remote(IsPure = true)]
    public ITenderAccreditationForm GetTenderAccreditationForm()
    {
      var tenderAccreditationFormKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Tenders.Constants.Module.TenderAccreditationFormKind);
      var document = _obj.QualificationFormGroup.TenderAccreditationForms.FirstOrDefault(x => Equals(x.DocumentKind, tenderAccreditationFormKind));
      return Tenders.TenderAccreditationForms.As(document);
    }

  }
}