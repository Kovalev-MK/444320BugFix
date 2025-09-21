using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.OutgoingLetters.MassMailingApplication;

namespace lenspec.OutgoingLetters
{
  partial class MassMailingApplicationServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.LettersStatus = MassMailingApplication.LettersStatus.No;
    }
  }

  partial class MassMailingApplicationOurSignatoryPropertyFilteringServerHandler<T>
  {
    //Добавлено Avis Expert
    public override IQueryable<T> OurSignatoryFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.OurSignatoryFiltering(query, e);
      if (_obj.DeliveryMethod != null && _obj.DeliveryMethod.Name.Equals(lenspec.Etalon.MailDeliveryMethods.Resources.FullEmailMethod))
      {
        var signatureSettings = Sungero.Docflow.SignatureSettings.GetAll(x => x.Certificate != null);
        query = query.Where(x => signatureSettings.Any(s => s.Recipient.Equals(x)));
      }
      return query;
    }
  }


  partial class MassMailingApplicationObjectAnProjectPropertyFilteringServerHandler<T>
  {

    
    public virtual IQueryable<T> ObjectAnProjectFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => x.IsLinkToInvest == true);
      return query;
    }
    //конец Добавлено Avis Expert
  }


  partial class MassMailingApplicationClientContractsClientContractPropertyFilteringServerHandler<T>
  {
  }

}