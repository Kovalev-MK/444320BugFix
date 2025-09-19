using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OutgoingLetter;

namespace lenspec.Etalon.Client
{
  partial class OutgoingLetterFunctions
  {

    //Добавлено Avis Expert
    /// <summary>
    /// Создание письма с вложенными документами.
    /// </summary>
    /// <param name="email">Почта для отправки письма.</param>
    /// <param name="attachments">Список вложений.</param>
    public override void CreateEmail(string email, List<Sungero.Docflow.IOfficialDocument> attachments)
    {
      var mail = MailClient.CreateMail();
      mail.To.Add(email);
      mail.Subject = _obj.Name;
      mail.AddAttachment(_obj.LastVersion);
      if (attachments != null)
      {
        foreach (var relation in attachments)
          if (relation.HasVersions)
            mail.AddAttachment(relation.LastVersion);
      }
      mail.Show();
    }
    
    /// <summary>
    /// Получить связанные документы, имеющие версии.
    /// </summary>
    /// <returns>Список связанных документов.</returns>
    public override List<Sungero.Docflow.IOfficialDocument> GetRelatedDocumentsWithVersions()
    {
      var addendumRelatedDocuments = Functions.OutgoingLetter.Remote.GetRelatedDocumentsByRelationType(_obj, Sungero.Docflow.Constants.Module.AddendumRelationName, true);
      addendumRelatedDocuments = addendumRelatedDocuments.Where(x => !Sungero.RecordManagement.IncomingLetters.Is(x)).OrderBy(x => x.Name).ToList();
      
      var relatedDocuments = new List<Sungero.Docflow.IOfficialDocument>();
      relatedDocuments.AddRange(addendumRelatedDocuments);
      
      // TODO Dmitirev_IA: Опасно для более 2000 документов.
      relatedDocuments = relatedDocuments.Distinct().ToList();
      return relatedDocuments;
    }
    
    /// <summary>
    /// Выбор связанных документов для отправки и создания письма.
    /// </summary>
    /// <param name="relatedDocuments">Связанные документы.</param>
    public override void SelectRelatedDocumentsAndCreateEmail(List<Sungero.Docflow.IOfficialDocument> relatedDocuments)
    {
      var addressees = Sungero.Docflow.PublicFunctions.OfficialDocument.GetEmailAddressees(_obj);
      if ((addressees == null || !addressees.Any()) && (relatedDocuments == null || !relatedDocuments.Any()))
      {
        if (Sungero.Docflow.PublicFunctions.Module.AllowCreatingEmailWithLockedVersions(new List<Sungero.Content.IElectronicDocument>() { _obj }))
          this.CreateEmail(string.Empty, relatedDocuments);
        return;
      }
      relatedDocuments.Add(_obj);
      var castedRelatedDocuments = relatedDocuments.Select(x => Sungero.Content.ElectronicDocuments.As(x)).ToList<Sungero.Content.IElectronicDocument>();
      if (Sungero.Docflow.PublicFunctions.Module.AllowCreatingEmailWithLockedVersions(castedRelatedDocuments))
      {
        var email = string.Empty;
        var addresses = addressees.FirstOrDefault();
        if (addresses != null)
        {
          email = addresses.Email;
        }
        this.CreateEmail(email, relatedDocuments);
      }
    }
    //конец Добавлено Avis Expert
  }
}