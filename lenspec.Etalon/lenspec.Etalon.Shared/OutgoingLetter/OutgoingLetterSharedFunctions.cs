using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OutgoingLetter;
using Sungero.Domain.Shared;

namespace lenspec.Etalon.Shared
{
  partial class OutgoingLetterFunctions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Удалить из коллекции ответных документов не от текущих корреспондентов.
    /// </summary>
    public override void RemoveDocumentsOfDeletedCorrespondents()
    {
      var documentsToRemove = _obj.InResponseToOffDocumentslenspec
        .Where(d => d.Document != null &&
               !_obj.Addressees.Any(a => Sungero.Docflow.IncomingDocumentBases.Is(d.Document) && Equals(Sungero.Docflow.IncomingDocumentBases.As(d.Document).Correspondent, a.Correspondent) ||
                                    avis.CustomerRequests.CustomerRequests.Is(d.Document) && Equals(avis.CustomerRequests.CustomerRequests.As(d.Document).Client, a.Correspondent)))
        .ToList();
      
      foreach (var document in documentsToRemove)
        _obj.InResponseToDocuments.Remove(document);
    }
    
    #region Несколько ответных писем
    
    /// <summary>
    /// Очистить ответные документы и заполнить из одиночного свойства "В ответ на".
    /// </summary>
    public override void ClearAndFillFirstResponseDocument()
    {
      _obj.InResponseToOffDocumentslenspec.Clear();
      if (_obj.InResponseToCustomavis != null)
      {
        var newItem = _obj.InResponseToOffDocumentslenspec.AddNew();
        newItem.Document = _obj.InResponseToCustomavis;
      }
    }
    
    /// <summary>
    /// Заполнить "В ответ на" из коллекции ответных документов.
    /// </summary>
    public override void FillInResponseToFromInResponseToDocuments()
    {
      var firstItem = _obj.InResponseToOffDocumentslenspec.FirstOrDefault(d => d.Document != null);
      if (firstItem == null)
        return;
      
      if (_obj.InResponseToCustomavis == null)
        _obj.InResponseToCustomavis = firstItem.Document;
      else if (!_obj.InResponseToOffDocumentslenspec.Any(d => Equals(_obj.InResponseToCustomavis, d.Document)))
               _obj.InResponseToCustomavis = _obj.InResponseToOffDocumentslenspec.First().Document;
      }
    
    /// <summary>
    /// Заполнить первый элемент коллекции ответных документов.
    /// </summary>
    public override void FillFirstResponseDocument()
    {
      if (_obj.InResponseToCustomavis != null && !_obj.InResponseToOffDocumentslenspec.Any())
      {
        var newItem = _obj.InResponseToOffDocumentslenspec.AddNew();
        newItem.Document = _obj.InResponseToCustomavis;
      }
    }
    
    #endregion
    
    /// <summary>
    /// Добавить в группу вложений входящее письмо, в ответ на которое было создано исходящее.
    /// </summary>
    /// <param name="group">Группа вложений.</param>
    public override void AddRelatedDocumentsToAttachmentGroup(Sungero.Workflow.Interfaces.IWorkflowEntityAttachmentGroup group)
    {
      foreach (var document in _obj.InResponseToOffDocumentslenspec.Select(d => d.Document).Distinct())
        if (document != null && !group.All.Contains(document))
          group.All.Add(document);
    }
    
    /// <summary>
    /// Получить список адресатов с электронной почтой для отправки многоадресного письма.
    /// </summary>
    /// <returns>Список адресатов.</returns>
    public override List<Sungero.Docflow.Structures.OfficialDocument.IEmailAddressee> GetEmailAddresseesFromManyAddresseesLetter()
    {
      var result = new List<Sungero.Docflow.Structures.OfficialDocument.IEmailAddressee>();
      
      var allEmails = new StringBuilder();
      var addressees = _obj.GetChildCollectionPropertyValue(_obj.Info.Properties.Addressees.Name);
      var businessUnits = Sungero.Company.BusinessUnits.GetAll();
      foreach (Etalon.IOutgoingLetterAddressees addresseeRow in addressees)
      {
        #region Зеркалам НОР не отправлять эл. письма - временно закомментировано
        /*
        if (Sungero.Parties.Companies.Is(addresseeRow.Correspondent))
        {
          var company = Sungero.Parties.Companies.As(addresseeRow.Correspondent);
          if (businessUnits.Any(x => company.Equals(x.Company)))
            continue;
        }
        */
        #endregion
        
        var hasEmail = !string.IsNullOrWhiteSpace(addresseeRow.AddresseeEmailavis);
        if (hasEmail)
        {
          allEmails.Append(string.Format("{0}; ", addresseeRow.AddresseeEmailavis));
        }
      }
      var emailAddressee = Sungero.Docflow.Structures.OfficialDocument.EmailAddressee
        .Create(Sungero.RecordManagement.OutgoingLetters.Resources.Correspondents, allEmails.ToString());
      result.Add(emailAddressee);
      return result;
    }
    
    /// <summary>
    /// Получить список адресатов с электронной почтой для отправки одноадресного письма.
    /// </summary>
    /// <returns>Список адресатов.</returns>
    public override List<Sungero.Docflow.Structures.OfficialDocument.IEmailAddressee> GetEmailAddresseesFromSingleAddresseeLetter()
    {
      var result = new List<Sungero.Docflow.Structures.OfficialDocument.IEmailAddressee>();
      
      #region Зеркалам НОР не отправлять эл. письма - временно закомментировано
      /*
      if (_obj.Correspondent != null && Sungero.Parties.Companies.Is(_obj.Correspondent))
      {
        var company = Sungero.Parties.Companies.As(_obj.Correspondent);
        if (Sungero.Company.BusinessUnits.GetAll().Any(x => company.Equals(x.Company)))
          return result;
      }
      */
      #endregion
      
      var addresseeName = _obj.Addressee != null ? _obj.Addressee.Name : _obj.Correspondent.Name;
      var emailAddressee = Sungero.Docflow.Structures.OfficialDocument.EmailAddressee
        .Create(Sungero.Docflow.OfficialDocuments.Resources.AddresseeLabelFormat(addresseeName, _obj.Emaillenspec), _obj.Emaillenspec);
      result.Add(emailAddressee);
      return result;
    }
    //конец Добавлено Avis Expert
  }
}