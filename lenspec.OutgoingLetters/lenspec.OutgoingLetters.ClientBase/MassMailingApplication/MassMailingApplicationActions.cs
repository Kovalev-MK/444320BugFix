using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.OutgoingLetters.MassMailingApplication;

namespace lenspec.OutgoingLetters.Client
{
  partial class MassMailingApplicationActions
  {
    
    //Добавлено Avis Expert
    /// <summary>
    /// Действие "Удалить клиентские договора".
    /// </summary>
    /// <param name="e"></param>
    public virtual void DeleteClientContract(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Получить список уже добавленных клиентских договоров.
      var clientContracts = new List<lenspec.SalesDepartmentArchive.ISDAClientContract>();
      foreach (var clientContract in _obj.CollectionClientContract)
        clientContracts.Add(clientContract.ClientContract);
      
      // В списке для выбора оставляем только те договоры что еще не добавлены.
      var query = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(c => clientContracts.Contains(c)).ShowSelectMany();
      var selectedClientContracts = query.ToList();
      
      // Удаляем из списка клиентские договоры.
      foreach (var clientContract in selectedClientContracts)
      {
        var deleteClientContract = _obj.CollectionClientContract.Where(c => c.ClientContract == clientContract).FirstOrDefault();
        _obj.CollectionClientContract.Remove(deleteClientContract);
      }
    }

    public virtual bool CanDeleteClientContract(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (_obj.ObjectAnProject != null && _obj.MailingType != null)
        return true;
      
      return false;
    }

    /// <summary>
    ///  Действие "Добавить клиентский договор."
    /// </summary>
    /// <param name="e"></param>
    public virtual void AddClientContract(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Получить список уже добавленных клиентских договоров.
      var clientContracts = new List<lenspec.SalesDepartmentArchive.ISDAClientContract>();
      foreach (var clientContract in _obj.CollectionClientContract)
        clientContracts.Add(clientContract.ClientContract);

      // В списке для выбора оставляем только те договоры, что еще не добавлены.
      var query = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(c => c.ObjectAnProject == _obj.ObjectAnProject && !clientContracts.Contains(c));
      
      if (_obj.MailingType.ChangeOfMeasurements.HasValue && _obj.MailingType.ChangeOfMeasurements.Value == true)
      {
        query = query
           .Where(x => x.Premises != null && ( 
                                             (x.Premises.EditSquere.HasValue && !x.Premises.EditSquere.Value.Equals(0.00)) &&
                                             (x.Premises.EditPrice.HasValue && !x.Premises.EditPrice.Value.Equals(0.00))
                                            ));
      }
      
      query = query.ShowSelectMany();
      var selectedClientContracts = query.ToList();
      
      // Добавляем в список клиентские договоры.
      foreach (var clientContract in selectedClientContracts)
        _obj.CollectionClientContract.AddNew().ClientContract = clientContract;
    }

    public virtual bool CanAddClientContract(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (_obj.ObjectAnProject != null && _obj.MailingType != null)
        return true;
      
      return false;
    }

    public virtual void CheckLetterData(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      var report = Reports.GetCheckLetterDataReport();
      report.Entity = _obj;
      report.Open();
    }

    public virtual bool CanCheckLetterData(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

    public virtual void GenerateLettersForMailing(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.Save();
      
      if (_obj.LettersStatus == MassMailingApplication.LettersStatus.Formed)
      {
        Dialogs.ShowMessage(lenspec.OutgoingLetters.MassMailingApplications.Resources.OutgoingLettersAlreadyFormed, MessageType.Warning);
        return;
      }
      
      
      if (_obj.MailingType.ChangeOfMeasurements.HasValue && _obj.MailingType.ChangeOfMeasurements.Value == true)
      {
        var dialog = Dialogs.CreateInputDialog("Проверьте список помещений");
        var premisesHyperlink = dialog.AddHyperlink("Показать список помещений");
        var ok = dialog.Buttons.AddCustom("Все верно");
        var cancel = dialog.Buttons.AddCustom("Назад");
        
        premisesHyperlink.SetOnExecute(
          () => {
            var objectAnSales = lenspec.EtalonDatabooks.ObjectAnSales.GetAll(x => x.ObjectAnProject.Equals(_obj.ObjectAnProject));
            if (_obj.MailingType.ChangeOfMeasurements.HasValue && _obj.MailingType.ChangeOfMeasurements.Value == true)
            {
              objectAnSales = objectAnSales.Where(x => x.EditSquere.HasValue && !x.EditSquere.Value.Equals(0.00) &&
                                                  x.EditPrice.HasValue && !x.EditPrice.Value.Equals(0.00));
              var clientContracts = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll()
                .Where(x => x.LifeCycleState == lenspec.SalesDepartmentArchive.SDAClientContract.LifeCycleState.Active &&
                       x.CounterpartyClient.Any(c => Sungero.Parties.People.Is(c.ClientItem)));
              if (clientContracts.Any())
              {
                objectAnSales = objectAnSales.Where(x => clientContracts.Any(c => c.Premises.Equals(x)));
                objectAnSales.ShowModal();
              }
              else
              {
                Dialogs.ShowMessage(lenspec.OutgoingLetters.MassMailingApplications.Resources.ClientContractsByPersonNotFound, MessageType.Warning);
              }
            }
            else
            {
              objectAnSales.ShowModal();
            }
          });
        if (dialog.Show() == ok)
        {
          RunAsyncHandlerCreateOutgoingLettersForMassMailing(_obj.Id);
        }
      }
      else
      {
        RunAsyncHandlerCreateOutgoingLettersForMassMailing(_obj.Id);
      }
      e.CloseFormAfterAction = true;
    }

    public virtual bool CanGenerateLettersForMailing(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }
    
    private void RunAsyncHandlerCreateOutgoingLettersForMassMailing(long massMailingApplicationId)
    {
      var asyncHandler = lenspec.OutgoingLetters.AsyncHandlers.CreateOutgoingLettersForMassMailing.Create();
      asyncHandler.MassMailingApplicationId = massMailingApplicationId;
      asyncHandler.UserId = Sungero.CoreEntities.Users.Current.Id;
      asyncHandler.ExecuteAsync(lenspec.OutgoingLetters.MassMailingApplications.Resources.CreateOutgoingLettersStarted, string.Empty);
    }
    //конец Добавлено Avis Expert

  }

}