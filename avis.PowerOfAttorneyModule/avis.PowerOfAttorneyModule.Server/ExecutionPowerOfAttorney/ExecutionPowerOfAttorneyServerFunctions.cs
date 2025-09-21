using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ExecutionPowerOfAttorney;

namespace avis.PowerOfAttorneyModule.Server
{
  partial class ExecutionPowerOfAttorneyFunctions
  {

    /// <summary>
    /// Контрол состояния для вкладок 'Задачи' в документах
    /// </summary>
    public StateView GetStateView(Sungero.Docflow.IOfficialDocument document)
    {
      var stateView = StateView.Create();
      var block = stateView.AddBlock();
      var link = Hyperlinks.Get(_obj);
      block.AddHyperlink(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.HyperlinkText, link);
      block.AddLineBreak();
      block.AssignIcon(ExecutionPowerOfAttorneys.Resources.task2, StateBlockIconSize.Large);
      block.AddLabel(_obj.Subject);
      return stateView;
    }

//    /// <summary>
//    /// Получить сотрудников, для фильтрации выбора в диалоге выдачи оригинала
//    /// <returns>Поверенные персоны</returns>
//    /// </summary>
//    [Remote(IsPure = true)]
//    public List<lenspec.Etalon.IEmployee> GetAttorneyPerformers()
//    {
//      var document = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
//      var attorneysPeople = document.Attorneyavis.Where(x => Sungero.Parties.People.Is(x.Attorn) && Sungero.Parties.People.As(x.Attorn).Status == Sungero.Parties.Person.Status.Active).Select(p => p.Attorn);
//      if(attorneysPeople == null || !attorneysPeople.Any())
//        return new List<lenspec.Etalon.IEmployee>();
//      else
//      {
//        var employees = lenspec.Etalon.Employees.GetAll(x => x.Person != null && attorneysPeople.Contains(x.Person) && x.Status == Sungero.Company.Employee.Status.Active);
//        if(employees == null)
//          return new List<lenspec.Etalon.IEmployee>();
//        else
//          return employees.ToList();
//      }
//    }
    
    /// <summary>
    /// Конвертировать доверенности в PDF
    /// </summary>
    public void ConvertToPdfPowerOfAttorneys(List<lenspec.Etalon.IPowerOfAttorney> documents)
    {
      foreach(var poa in documents)
      {
        var isSupported = Sungero.AsposeExtensions.Converter.CheckIfExtensionIsSupported(poa.AssociatedApplication.Extension);
        if(isSupported == false)
        {
          Logger.DebugFormat(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.DebugLogMessageUnsupportedConvertation, poa.Id);
          continue;
        }
        var lockStatus = Locks.GetLockInfo(poa);
        if(lockStatus.IsLocked)
        {
          Logger.DebugFormat(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.DebugLogMessageLockedDocument, poa.Id, lockStatus.OwnerName);
          continue;
        }
        using(var stream = poa.LastVersion.Body.Read())
        {
          var pdfStream = Sungero.Docflow.IsolatedFunctions.PdfConverter.GeneratePdf(stream, poa.LastVersion.AssociatedApplication.Extension);
          var newVerion = poa.CreateVersionFrom(pdfStream, "pdf"); //poa.LastVersion.AssociatedApplication.Extension
          newVerion.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension("pdf");
          poa.Save();
        }
      }
    }

    /// <summary>
    /// Контрол состояния
    /// </summary>
    [Remote]
    public StateView GetExecutionPowerOfAttorneyState()
    {
      var stateView = StateView.Create();
      var block = stateView.AddBlock();
      var mainDocument = _obj.ProjectPOA.PowerOfAttorneys.FirstOrDefault();
      var kind = string.Empty;
      var countCopies = string.Empty;
      var ourBusinessUnit = string.Empty;
      if(mainDocument != null)
      {
        kind = mainDocument.DocumentKind.Name;
        countCopies = mainDocument.CountCopiesavis.Value.ToString();
        if(mainDocument.OurBusinessUavis.Any())
        {
          var companyNames = mainDocument.OurBusinessUavis.Select(i => i.Company.Name);
          foreach(string name in companyNames)
          {
            ourBusinessUnit += name == companyNames.Last() ? name : name + ", ";
          }
        }
      }
      block.AddLabel(string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.ControlStateKIndPOA, kind));
      block.AddLineBreak();
      block.AddLabel(string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.ControlStateCountCopies, countCopies));
      block.AddLineBreak();
      block.AddLabel(string.Format(avis.PowerOfAttorneyModule.ExecutionPowerOfAttorneys.Resources.ControlStateBusinessUnits, ourBusinessUnit));
      return stateView;
    }

  }
}