using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalSendingAssignment;

namespace lenspec.Etalon.Client
{
  partial class ApprovalSendingAssignmentFunctions
  {

    #region Отправка по email
    
    /// <summary>
    /// Создать и отправить письмо по почте.
    /// </summary>
    /// <param name="task">Задача.</param>
    public static void SendByMailAvis(Sungero.Docflow.IApprovalTask task)
    {
      var doc = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var addenda = task.AddendaGroup.OfficialDocuments.Where(x => !Equals(x, OfficialDocuments.Null) && x.HasVersions).ToList();
      var relatedDocuments = new List<Sungero.Docflow.IOfficialDocument>();
      relatedDocuments.AddRange(addenda);
      lenspec.Etalon.PublicFunctions.OfficialDocument.SelectRelatedDocumentsAndCreateEmail(Etalon.OfficialDocuments.As(doc), relatedDocuments);
    }
    
    #endregion
  }
}