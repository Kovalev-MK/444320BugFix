using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderCommitteeProtocol;

namespace lenspec.Tenders.Shared
{
  partial class TenderCommitteeProtocolFunctions
  {

    /// <summary>
    /// Убрать дубли адресатов.
    /// </summary>
    public virtual void DeleteDublicatesAddressees()
    {
      var distinctAdresseesList = _obj.Addressees.Select(r => r.Addressee).ToList().Distinct();
      if (distinctAdresseesList.Count() != _obj.Addressees.Count())
      {
        _obj.Addressees.Clear();
        foreach (var employee in distinctAdresseesList)
        {
          var newParticipantRow = _obj.Addressees.AddNew();
          newParticipantRow.Addressee = employee;
        }
      }
    }
    
    /// <summary>
    /// Заполнить адресатов из списка ознакомления/рассмотрения.
    /// </summary>
    /// <param name="acquaintanceList">Список ознакомления/рассмотрения.</param>
    /// <returns>Сообщение об ошибке или пустая строка.</returns>
    public virtual string TryFillFromAcquaintanceList(Sungero.RecordManagement.IAcquaintanceList acquaintanceList)
    {
      if (acquaintanceList == null)
        return string.Empty;
      
      var participants = Sungero.RecordManagement.PublicFunctions.AcquaintanceList.GetParticipants(acquaintanceList);
      foreach (var participant in participants)
      {
        var newParticipantRow = _obj.Addressees.AddNew();
        newParticipantRow.Addressee = participant;
      }
      _obj.Save();
      return string.Empty;
    }
    
    /// <summary>
    /// Убрать дубли адресатов.
    /// </summary>
    public virtual void DeleteDublicatesAddresses()
    {
      var distinctAdresseesList = _obj.Addressees.Select(r => r.Addressee).ToList().Distinct();
      if (distinctAdresseesList.Count() != _obj.Addressees.Count())
      {
        _obj.Addressees.Clear();
        foreach (var employee in distinctAdresseesList)
        {
          var newParticipantRow = _obj.Addressees.AddNew();
          newParticipantRow.Addressee = employee;
        }
      }
    }
    
    /// <summary>
    /// Получить документы, связанные типом связи "Приложение".
    /// </summary>
    /// <returns>Документы, связанные типом связи "Приложение".</returns>
    [Public]
    public override List<Sungero.Docflow.IOfficialDocument> GetAddenda()
    {
      var addenda = base.GetAddenda();
      if (_obj.BasisDocument != null && !Sungero.Docflow.PublicFunctions.OfficialDocument.IsObsolete(_obj.BasisDocument))
        addenda.Add(_obj.BasisDocument);
      return addenda;
    }
    
    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var documentKind = _obj.DocumentKind;
      
      if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      
      if (documentKind == null || !documentKind.GenerateDocumentName.Value)
        return;
      
      var sB = new System.Text.StringBuilder();
      
      // Имя в формате: <Вид документа> № <Рег. №> от <Дата документа> <Наша орг.> для <Контрагенты> <ИСП> <ОП>
      using (TenantInfo.Culture.SwitchTo())
      {
        if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
        {
          sB.Append(lenspec.Tenders.TenderDocumentBases.Resources.Number);
          sB.Append(_obj.RegistrationNumber);
        }
        if (_obj.RegistrationDate != null)
        {
          sB.Append(lenspec.Tenders.TenderDocumentBases.Resources.FromPartOfDocName);
          sB.Append(_obj.RegistrationDate.Value.ToString("d"));
        }
        var counterpartyNames = _obj.Counterparties
          .Where(x => x.Counterparty != null)
          .Select(x => x.Counterparty.Name);
        if (_obj.BusinessUnit != null && counterpartyNames.Any())
        {
          sB.Append(lenspec.Tenders.TenderDocumentBases.Resources.WhitespacePartOfDocName);
          sB.Append(_obj.BusinessUnit.Name);
          sB.Append(lenspec.Tenders.TenderDocumentBases.Resources.ForPartOfDocName);
          sB.Append(string.Join(
            lenspec.Tenders.TenderDocumentBases.Resources.CommaPartOfDocName,
            counterpartyNames
           ));
        }
        // ИСП.
        var ourCFNames = _obj.OurCF
          .Where(x => x.OurCF != null)
          .Select(x => x.OurCF.CommercialName);
        if (_obj.OurCF.Where(x => x.OurCF != null).Any())
        {
          sB.Append(lenspec.Tenders.TenderDocumentBases.Resources.WhitespacePartOfDocName);
          sB.Append(string.Join(
            lenspec.Tenders.TenderDocumentBases.Resources.CommaPartOfDocName,
            ourCFNames
           ));
        }
        // Объекты проектов.
        var objectAnProjectNames = _obj.ObjectAnProjects
          .Where(x => x.ObjectAnProject != null)
          .Select(x => x.ObjectAnProject.Name);
        if (objectAnProjectNames.Any())
        {
          sB.Append(lenspec.Tenders.TenderDocumentBases.Resources.WhitespacePartOfDocName);
          sB.Append(string.Join(
            lenspec.Tenders.TenderDocumentBases.Resources.CommaPartOfDocName,
            objectAnProjectNames
           ));
        }
      }
      
      var name = sB.ToString();
      if (string.IsNullOrWhiteSpace(name))
      {
        name = _obj.VerificationState == null ?
          Sungero.Docflow.Resources.DocumentNameAutotext :
          _obj.DocumentKind.ShortName;
      }
      else if (documentKind != null)
      {
        name = documentKind.ShortName + name;
      }
      
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      _obj.Name = name.Substring(0, Math.Min(_obj.Info.Properties.Name.Length, name.Length));
    }
  }
}