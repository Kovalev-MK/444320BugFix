using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.ExecutionPowerOfAttorney;

namespace avis.PowerOfAttorneyModule.Shared
{
  partial class ExecutionPowerOfAttorneyFunctions
  {

    /// <summary>
    /// Добавить документы поверенных во вложения
    /// </summary>       
    public void SetAttorneyDocumentsInAttachments(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      var powerOfAttorney = lenspec.Etalon.PowerOfAttorneys.As(e.Attachment);
      var attachments = powerOfAttorney.Relations.GetRelatedDocuments();
      foreach(var attachment in attachments)
      {
        var simpleDoc = Sungero.Docflow.SimpleDocuments.As(attachment);
        if(simpleDoc != null && simpleDoc.DocumentKind.Equals(Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(PublicConstants.Module.DocumentAttorneyKindGuid)))
        {
          _obj.AttorneyDocuments.SimpleDocuments.Add(simpleDoc);
        }
        else
        {
          _obj.Addendums.All.Add(attachment);
        }
      }
    }

    /// <summary>
    /// Заполнить руководите инициатора
    /// </summary>
    public void SetManagerInitiator(Sungero.Workflow.Shared.TaskAuthorChangedEventArgs e)
    {
      if(e.NewValue != null)
      {
        var department = Sungero.Company.Employees.As(e.NewValue).Department;
        if(department != null)
        {
          var manager = avis.PowerOfAttorneyModule.PublicFunctions.Module.FoundManager(department);
          _obj.ManagerInitiator = manager;
        }
      }
    }
  }
}