using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using lenspec.Etalon.OutgoingLetter;

namespace lenspec.Etalon
{
  partial class OutgoingLetterInResponseToOffDocumentslenspecDocumentPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> InResponseToOffDocumentslenspecDocumentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => Sungero.Docflow.IncomingDocumentBases.Is(x) || avis.CustomerRequests.CustomerRequests.Is(x));
      
      var selectedDocuments = _obj.OutgoingLetter.InResponseToOffDocumentslenspec
        .Where(d => d.Document != null && !Equals(d, _obj))
        .Select(d => d.Document);
      query = query.Where(x => !selectedDocuments.Contains(x));
      
      if (_obj.OutgoingLetter.Addressees.Any(a => a.Correspondent != null))
      {
        var correspondents = _obj.OutgoingLetter.Addressees.Where(a => a.Correspondent != null).Select(a => a.Correspondent).ToList();
        query = query.Where(l => Sungero.Docflow.IncomingDocumentBases.Is(l) && correspondents.Contains(Sungero.Docflow.IncomingDocumentBases.As(l).Correspondent) ||
                            avis.CustomerRequests.CustomerRequests.Is(l) && correspondents.Contains(avis.CustomerRequests.CustomerRequests.As(l).Client));
      }
      
      if (_obj.OutgoingLetter.BusinessUnit != null)
        query = query.Where(l => Equals(_obj.OutgoingLetter.BusinessUnit, l.BusinessUnit));
      
      return query;
    }
  }


  partial class OutgoingLetterPreparedByPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public override IQueryable<T> PreparedByFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.PreparedByFiltering(query, e);
      
      var substituted = Sungero.Company.PublicFunctions.Module.GetUsersSubstitutedBy(Users.Current).Select(x => x.Id).ToList();
      query = query.Where(x => substituted.Contains(x.Id));
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class OutgoingLetterOurSignatoryPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public override IQueryable<T> OurSignatoryFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.OurSignatoryFiltering(query, e);
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class OutgoingLetterInResponseToCustomavisPropertyFilteringServerHandler<T>
  {

    //Добавлено Avis Expert
    public virtual IQueryable<T> InResponseToCustomavisFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.BusinessUnit != null)
        query = query.Where(l => Equals(_obj.BusinessUnit, l.BusinessUnit));
      
      query = query.Where(x => Sungero.Docflow.IncomingDocumentBases.Is(x) || avis.CustomerRequests.CustomerRequests.Is(x));
      
      if (_obj.Addressees.Any(a => a.Correspondent != null))
      {
        var correspondents = _obj.Addressees.Where(a => a.Correspondent != null).Select(a => a.Correspondent).ToList();
        var incomingDocs = query.Where(l => correspondents.Contains(Sungero.Docflow.IncomingDocumentBases.As(l).Correspondent)).Select(x => x.Id).ToList();
        var costomerRequests = query.Where(l => correspondents.Contains(avis.CustomerRequests.CustomerRequests.As(l).Client)).Select(x => x.Id).ToList();
        query = query.Where(x => (incomingDocs.Any() && incomingDocs.Contains(x.Id)) || (costomerRequests.Any() && costomerRequests.Contains(x.Id)));
      }
      
      return query;
    }
    //конец Добавлено Avis Expert
  }

  partial class OutgoingLetterServerHandlers
  {

    //Добавлено Avis Expert
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.Archiveavis != true)
      {
        if (_obj.IsManyAddressees == true && _obj.DeliveryMethod != null &&
            (_obj.DeliveryMethod.Name.Trim().Equals(Sungero.Docflow.MailDeliveryMethods.Resources.EmailMethod) || _obj.DeliveryMethod.Name.Trim().Equals("Электронная почта")))
        {
          var hasEmptyEmail = false;
          var addressees = _obj.GetChildCollectionPropertyValue(_obj.Info.Properties.Addressees.Name);
          foreach (Etalon.IOutgoingLetterAddressees addressee in addressees)
          {
            if (string.IsNullOrEmpty(addressee.AddresseeEmailavis))
            {
              hasEmptyEmail = true;
              break;
            }
          }
          if (hasEmptyEmail)
          {
            e.AddError(lenspec.Etalon.OutgoingLetters.Resources.FillEmailForAddressees);
            return;
          }
        }
      }
      base.BeforeSave(e);
      
      if (_obj.IsManyResponses == false)
        Functions.OutgoingLetter.ClearAndFillFirstResponseDocument(_obj);
      else if (_obj.IsManyResponses == true)
        _obj.InResponseToCustomavis = null;
      
      var emptyItems = _obj.InResponseToOffDocumentslenspec.Where(x => x.Document == null).ToList();
      foreach (var item in emptyItems)
        _obj.InResponseToOffDocumentslenspec.Remove(item);
      
      foreach (var responseItem in _obj.InResponseToOffDocumentslenspec)
      {
        if (!_obj.Relations.GetRelatedFromDocuments(Sungero.Docflow.PublicConstants.Module.ResponseRelationName).Any(x => Equals(x, responseItem.Document)) && responseItem.Document.AccessRights.CanRead())
          _obj.Relations.AddFrom(Sungero.Docflow.PublicConstants.Module.ResponseRelationName, responseItem.Document);
      }
    }
    //конец Добавлено Avis Expert
  }

}