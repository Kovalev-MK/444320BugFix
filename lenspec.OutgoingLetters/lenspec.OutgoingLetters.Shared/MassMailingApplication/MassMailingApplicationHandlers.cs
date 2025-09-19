using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.OutgoingLetters.MassMailingApplication;

namespace lenspec.OutgoingLetters
{

  partial class MassMailingApplicationSharedHandlers
  {

    public virtual void CollectionClientContractChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if (_obj.LettersStatus == MassMailingApplication.LettersStatus.Formed)
      {
        _obj.LettersStatus = MassMailingApplication.LettersStatus.FormationRequired;
      }
    }

    public override void DeliveryMethodChanged(Sungero.Docflow.Shared.OfficialDocumentDeliveryMethodChangedEventArgs e)
    {
      base.DeliveryMethodChanged(e);
      _obj.OurSignatory = null;
    }

    //Добавлено Avis Expert
    public virtual void BankChanged(lenspec.OutgoingLetters.Shared.MassMailingApplicationBankChangedEventArgs e)
    {
      if (e.NewValue != null)
      {
        _obj.BIC = e.NewValue.BIC;
        _obj.CorrespondentAccount = e.NewValue.CorrespondentAccount;
      }
      else
      {
        _obj.BIC = null;
        _obj.CorrespondentAccount = null;
      }
    }

    public virtual void MailingTypeChanged(lenspec.OutgoingLetters.Shared.MassMailingApplicationMailingTypeChangedEventArgs e)
    {
      FillName();
      if (e.NewValue == null || !e.NewValue.Equals(e.OldValue))
      {
        _obj.Bank = null;
        // БИК и Корр. счет очистятся в событии Изменения значения свойства Банк.
        _obj.SettlementAccount = null;
      }
    }

    public virtual void ObjectAnProjectChanged(lenspec.OutgoingLetters.Shared.MassMailingApplicationObjectAnProjectChangedEventArgs e)
    {
      FillName();
      
      // Если пустое, то очищаем клиентские договора.
      if (e.NewValue != e.OldValue)
        _obj.CollectionClientContract.Clear();
    }

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      FillName();
    }
    //конец Добавлено Avis Expert

  }
}