using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingLetter;

namespace lenspec.Etalon.Server
{
  partial class IncomingLetterFunctions
  {
    /// <summary>
    /// Создание исходящего письма, на основе входящего.
    /// </summary>
    /// <returns></returns>
    public override Sungero.Docflow.IOfficialDocument CreateReplyDocument()
    {
      var outgoingDocument = lenspec.Etalon.OutgoingLetters.As(base.CreateReplyDocument());
      //outgoingDocument.InResponseTo = _obj;
      outgoingDocument.DeliveryMethod = _obj.DeliveryMethod;
      outgoingDocument.Subject = _obj.Subject;
      outgoingDocument.Correspondent = _obj.Correspondent;
      outgoingDocument.OurCFlenspec = _obj.OurCFlenspec;
      outgoingDocument.Addressee = _obj.Contact;
      outgoingDocument.PreparedBy = Employees.Current;
      
      outgoingDocument.ClientContractlenspec = _obj.ClientContractslenspec.FirstOrDefault()?.ClientContract;
      outgoingDocument.ManagementContractMKDavis = _obj.ManagementContractsMKDlenspec.FirstOrDefault()?.ManagementContractMKD;
      return outgoingDocument;
    }
  }
}