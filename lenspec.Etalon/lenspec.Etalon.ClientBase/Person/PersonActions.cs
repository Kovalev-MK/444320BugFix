using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Person;
using CommonLibrary;

namespace lenspec.Etalon.Client
{
  // Добавлено avis.
  partial class PersonActions
  {
    public virtual void ShowBankDetailslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var bankDetails = avis.EtalonParties.PublicFunctions.BankDetail.Remote.GetBankDetails(_obj);
      bankDetails.Show();
    }

    public virtual bool CanShowBankDetailslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void Export1Cavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Export1Cavis(e);
//      var check           = string.Empty;
//      var checkList       = new List<string>();
      
      var user = Sungero.Company.Employees.Current;
      if (user == null)
        return;
      var dialog          = Dialogs.CreateInputDialog("Экспорт в 1С");
      //      var businesUnit     = dialog.AddSelect("Наша организация", true, Sungero.Company.BusinessUnits.Null);
      //      businesUnit.Value   = user.Department.BusinessUnit;
      var employee        = dialog.AddSelect("Инициатор", true, Sungero.Company.Employees.Null);
      employee.Value = user;
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        if(_obj.Status != Sungero.Parties.Counterparty.Status.Active)
        {
          e.AddError("Выгрузить в 1С можно только действующие записи справочника" );
          return;
        }
        if(_obj.ResultApprovalDEBavis.Value != Etalon.Counterparty.ResultApprovalDEBavis.CoopPossible   &&
           _obj.ResultApprovalDEBavis.Value != Etalon.Counterparty.ResultApprovalDEBavis.CoopWithRisks  &&
           _obj.ResultApprovalDEBavis.Value != Etalon.Counterparty.ResultApprovalDEBavis.DoesNotReqAppr)
        {
          e.AddError(lenspec.Etalon.Companies.Resources.SbError);
          return;
        }
        if (_obj.IsLawyeravis != true && _obj.IsClientBuyersavis != true &&  _obj.IsClientOwnersavis != true && _obj.IsEmployeeGKavis != true && _obj.IsOtheravis != true)
        {
          e.AddError(lenspec.Etalon.People.Resources.RequiredFillIn);
          return;
        }
//        if(_obj.Sex == null)
//        {
//          checkList.Add("Пол");
//        }
//        if(_obj.DateOfBirth == null)
//        {
//          checkList.Add("Дата рождения");
//        }
//        if(string.IsNullOrEmpty(_obj.LegalAddress))
//        {
//          checkList.Add("Адрес регистрации");
//        }
//        if(string.IsNullOrEmpty(_obj.PostalAddress))
//        {
//          checkList.Add("Почтовый адрес");
//        }
//        if (string.IsNullOrEmpty(_obj.TIN))
//        {
//          checkList.Add("ИНН");
//        }
//        if (checkList.Count() > 0)
//        {
//          check = string.Join(", ", checkList);
//          e.AddError("Для выгрузки в 1С необходимо заполнить: " + check + ". Для внесения необходимых данных обратитесь в службу развития и администрирования СЭД.");
//          return;
//        }
        Dialogs.NotifyMessage("Выгрузка в интеграционную базу началась");
        //        var busineUnitCode = lenspec.Etalon.BusinessUnits.As(businesUnit.Value).ExternalCodeavis;
        var busineUnitCode = "000";
        lenspec.Etalon.Module.Parties.PublicFunctions.Module.UnloadingKAFromReference(_obj.Id, busineUnitCode);
      }
    }

    public override bool CanExport1Cavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Роль Ответственные за выгрузку контрагентов в 1С
      var responsibleUnloadingContractors1CGuid = Etalon.Module.Parties.Constants.Module.ResponsibleUnloadingContractors1CGuid;
      var responsibleUnloadingContractorsRole = Roles.GetAll(r => Equals(r.Sid, responsibleUnloadingContractors1CGuid)).FirstOrDefault();
      
      // Отображаем кнопку если сотрудник входит в роль Ответственные за выгрузку контрагентов в 1С
      if (Employees.Current.IncludedIn(responsibleUnloadingContractorsRole))
        return true;
      return base.CanExport1Cavis(e);
    }

    public virtual void ImportFromExcelavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Загрузка Собственников и Помещений");
      var fileSelector = dialog.AddFileSelect("Файл", true);
      fileSelector.WithFilter(string.Empty, "xlsx");
      var import = dialog.Buttons.AddCustom("Загрузить");
      dialog.Buttons.AddCancel();
      dialog.SetOnButtonClick(b =>
                              {
                                if (b.Button == import && b.IsValid)
                                {
                                  try
                                  {
                                    using (var memory = new System.IO.MemoryStream())
                                    {
                                      
                                      memory.Write(fileSelector.Value.Content, 0, fileSelector.Value.Content.Length);
                                      
                                      var errors = Etalon.PublicFunctions.Person.ImportPersonsAndObjectAnSale(memory);
                                      if (!string.IsNullOrEmpty(errors))
                                        e.AddError(errors);
                                      else
                                        Dialogs.NotifyMessage("Персоны и Помещения обновлены. Результат сохранен в Загрузки.");
                                      
                                      var zip = lenspec.EtalonDatabooks.PublicFunctions.Module.ExportBinaryDataAsZip(memory.ToArray(), fileSelector.Value.Name);
                                      zip.Export();
                                    }
                                  }
                                  catch (Exception ex)
                                  {
                                    Logger.Error("Avis - ImportFromExcelavis - ", ex);
                                    b.AddError(ex.Message, fileSelector);
                                  }
                                }
                              });
      dialog.Show();
    }

    public virtual bool CanImportFromExcelavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(Roles.Administrators) || Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid);
    }

    public virtual void ShowEmployeeslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var employees = lenspec.Etalon.Employees.GetAll().Where(x => x.Person == _obj && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active);
      employees.Show();
    }

    public virtual bool CanShowEmployeeslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public override void SaveAndClose(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed)
      {
        if (lenspec.Etalon.Employees.GetAll().Where(x => x.Person == _obj && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).Count() > 0)
        {
          e.AddError("Персона выбрана в действующем сотруднике", _obj.Info.Actions.ShowEmployeeslenspec);
          return;
        }
      }
      
      base.SaveAndClose(e);
    }

    public override bool CanSaveAndClose(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSaveAndClose(e);
    }

    public override void Save(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed)
      {
        if (lenspec.Etalon.Employees.GetAll().Where(x => x.Person == _obj && x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active).Count() > 0)
        {
          e.AddError("Персона выбрана в действующем сотруднике", _obj.Info.Actions.ShowEmployeeslenspec);
          return;
        }
      }
      
      base.Save(e); 
    }

    public override bool CanSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSave(e);
    }

    public override void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ShowDuplicates(e);
    }

    public override bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanShowDuplicates(e);
    }

    /// <summary>
    /// Кнопка "Договоры ЖКХ".
    /// </summary>
    /// <param name="e"></param>
    public virtual void AgreementsJKHavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if(Users.Current.IncludedIn(lenspec.Etalon.Module.Parties.Constants.Module.IsClientOwnerAccessGuid))
      {
        var documents = avis.ManagementCompanyJKHArhive.ManagementContractMKDs.GetAll(c => c.Client == _obj);
        documents.Show($"Документы МКД {_obj.LastName} {_obj.FirstName} {_obj.MiddleName}");
      }
    }

    public virtual bool CanAgreementsJKHavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(lenspec.Etalon.Module.Parties.Constants.Module.IsClientOwnerAccessGuid);
    }

    public virtual void ClientAgreementsavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Получаем документы с тип документа "Клиентский договор" и с указанием в клиентах данной персоны.
      if(Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid))
      {
        var documents = lenspec.SalesDepartmentArchive.SDAClientContracts.GetAll(d => d.CounterpartyClient.Where(c => c.ClientItem == _obj).FirstOrDefault() != null);
        documents.Show($"Клиентские договоры {_obj.LastName} {_obj.FirstName} {_obj.MiddleName}");
      }
    }

    /// <summary>
    /// Возможность выполнения кнопки "Клиентские договоры".
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual bool CanClientAgreementsavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid);
    }

    public virtual void UpdateInvestavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!string.IsNullOrEmpty(_obj.CodeInvestavis) && Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid))
      {
        // Отправляем в инвест запрос на обновление персоны.
        var asyncHandler = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncUpdateClientavis.Create();

        // Заполнить параметры асинхронного обработчика.
        asyncHandler.IdInvest = _obj.CodeInvestavis;
        
        // Вызвать асинхронный обработчик.
        asyncHandler.ExecuteAsync();
      }
    }

    /// <summary>
    /// Возможность выполнения кнопки "Обновить инф. из инвест".
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual bool CanUpdateInvestavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return (_obj.IsClientBuyersavis == true &&
              Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid));
    }
  }
  // Конец добавлено avis.
}