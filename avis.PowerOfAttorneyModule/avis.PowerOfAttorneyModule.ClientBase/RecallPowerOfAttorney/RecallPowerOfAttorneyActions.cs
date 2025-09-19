using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.PowerOfAttorneyModule.RecallPowerOfAttorney;

namespace avis.PowerOfAttorneyModule.Client
{
  partial class RecallPowerOfAttorneyActions
  {
    // Заполнить поля "Номер" и "Дата" в заявлении на отзыв доверенностей.
    public virtual void UpdateApplicationNumberAndDate(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var document = _obj.MainAttachment.ApplicationRelinquishmentAuthorities.SingleOrDefault();
      
      // Заполнить поле номер и дата в заявлении на отзыв доверенностей.
      var error = Functions.RecallPowerOfAttorney.Remote.FillNumberAndDate(_obj, document);
      // При ошибке вывести предупреждение и обновить статус
      // ошибки обновления полей заявления и шаблона.
      var isError = error != string.Empty;
      // TODO: На показ формы задач в статусе "Черновик" желательно добавить обновление параметра.
      e.Params.AddOrUpdate(Constants.RecallPowerOfAttorney.Params.DocumentTemplateFieldsUpdated, !isError);
      if (isError)
        Dialogs.NotifyMessage(error);
      else 
        // Скрываем сообщение об ошибке.
        _obj.Save();
    }

    public virtual bool CanUpdateApplicationNumberAndDate(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void Resume(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (Functions.RecallPowerOfAttorney.CheckDuplicates(_obj))
        return;
      base.Resume(e);
    }

    public override bool CanResume(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanResume(e);
    }

    public override void Restart(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (Functions.RecallPowerOfAttorney.CheckDuplicates(_obj))
        return;
      base.Restart(e);
    }

    public override bool CanRestart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanRestart(e);
    }

    public override void Start(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (Functions.RecallPowerOfAttorney.CheckDuplicates(_obj))
        return;
      base.Start(e);
    }

    public override bool CanStart(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanStart(e);
    }

    public virtual void AddRevocablePowerOfAttorneys(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Вид "Нотариальная доверенность".
      var documentNotarialKindGuid = avis.PowerOfAttorneyModule.Constants.Module.DocumentNotarialKindGuid;
      var documentNotarialKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(documentNotarialKindGuid);
      // Вид "Доверенность".
      var powerOfAttorneyKindGuid = Sungero.Docflow.PublicConstants.Module.Initialize.PowerOfAttorneyKind;
      var powerOfAttorneyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(powerOfAttorneyKindGuid);
      
      // Фильтрация отзываемых доверенностей.
      var powerOfAttorneys = lenspec.Etalon.PowerOfAttorneys
        .GetAll(x =>
                (Equals(x.DocumentKind, documentNotarialKind) || Equals(x.DocumentKind, powerOfAttorneyKind)) &&
                _obj.Attorney.Equals(x.IssuedTo) &&
                x.DateAbortPOAavis == null &&
                x.IsProjectPOAavis == false &&
                x.LifeCycleState == lenspec.Etalon.PowerOfAttorney.LifeCycleState.Active &&
                !_obj.PowerOfAttorney.PowerOfAttorneys.Contains(x));
      
      var dialog = Dialogs.CreateInputDialog(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.MainTitleDialog);
      var selectedDocuments = dialog.AddSelectMany(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.ChildeTitleDialog, true, lenspec.Etalon.PowerOfAttorneys.Null).From(powerOfAttorneys);
      
      if (dialog.Show() == DialogButtons.Ok)
        foreach (var poa in selectedDocuments.Value)
          _obj.PowerOfAttorney.PowerOfAttorneys.Add(poa);
      
      Functions.RecallPowerOfAttorney.Remote.UpdateApplicationRelinquishmentAuthority(_obj, false);
      UpdateApplicationNumberAndDate(e);
    }

    public virtual bool CanAddRevocablePowerOfAttorneys(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Sungero.CoreEntities.Users.Current.Equals(_obj.Author) && _obj.Attorney != null;
    }

    public virtual void FormApplicationRelinquishmentAuthority(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверка наличия отзываемых полномочий.
      if (!_obj.PowerOfAttorney.PowerOfAttorneys.Any())
      {
        e.AddError(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.ErrorMessageAbsentPoas);
        return;
      }
      
      // Проверка наличия заявления.
      if (_obj.MainAttachment.ApplicationRelinquishmentAuthorities.Any())
      {
        e.AddInformation(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.ApplicationRelinquishmentAuthorityExists);
        return;
      }
      
      // Создать заявление об отказе от полномочий.
      var document = Functions.RecallPowerOfAttorney.Remote.UpdateApplicationRelinquishmentAuthority(_obj, true);
      document.ShowModal();
      
      if (document.State.IsInserted)
      {
        e.AddError(avis.PowerOfAttorneyModule.RecallPowerOfAttorneys.Resources.ErrorMessageEmptyApplication);
        return;
      }
      _obj.MainAttachment.ApplicationRelinquishmentAuthorities.Add(document);
      
      UpdateApplicationNumberAndDate(e);
    }

    public virtual bool CanFormApplicationRelinquishmentAuthority(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Sungero.CoreEntities.Users.Current.Equals(_obj.Author) && _obj.Attorney != null;
    }

  }
}