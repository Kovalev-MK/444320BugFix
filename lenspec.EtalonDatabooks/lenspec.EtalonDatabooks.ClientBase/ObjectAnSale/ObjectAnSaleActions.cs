using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.ObjectAnSale;

namespace lenspec.EtalonDatabooks.Client
{
  partial class ObjectAnSaleActions
  {
    public virtual void ShowClients(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var clients = lenspec.SalesDepartmentArchive.PublicFunctions.SDAClientContract.Remote.GetClients(_obj, false);     
      clients.Show(lenspec.EtalonDatabooks.Resources.Clients);          
    }

    public virtual bool CanShowClients(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowOwners(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var clients = lenspec.SalesDepartmentArchive.PublicFunctions.SDAClientContract.Remote.GetClients(_obj, true);
      var owners = avis.ManagementCompanyJKHArhive.PublicFunctions.ManagementContractMKD.Remote.GetOwners(_obj);
      var selection = clients.Concat(owners).ToList().Distinct();
      
      selection.Show(lenspec.EtalonDatabooks.Resources.Owners);
    }

    public virtual bool CanShowOwners(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }


    //Добавлено Avis Expert
    /// <summary>
    /// Действие "Договоры управления МКД".
    /// </summary>
    /// <param name="e"></param>
    public virtual void MKDsShow(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      avis.ManagementCompanyJKHArhive.ManagementContractMKDs.GetAll(m => m.ObjectAnSale == _obj).Show();
    }

    public virtual bool CanMKDsShow(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }
    
    public virtual void ImportFromExcel(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Загрузка Собственников и Помещений");
      var fileSelector = dialog.AddFileSelect("Файл", true);
      fileSelector.WithFilter(string.Empty, "xlsx", "xls");
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
                                        Dialogs.NotifyMessage(lenspec.EtalonDatabooks.ObjectAnSales.Resources.ImportFromExcelResultMessage);
                                      
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

    public virtual bool CanImportFromExcel(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Users.Current.IncludedIn(Roles.Administrators) || Users.Current.IncludedIn(avis.EtalonParties.PublicConstants.Module.RoleCreateCounterpartyGuid);
    }

    public virtual bool CanRequestPIBAndBTI(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return CallContext.CalledFrom(lenspec.OutgoingLetters.MassMailingApplications.Info);
    }
    
    public virtual void RequestPIBAndBTI(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.ObjectAnProject != null)
      {
        var asyncHandler = lenspec.Etalon.Module.Integration.AsyncHandlers.AsyncUpdateObjectAnProjectavis.Create();
        asyncHandler.IdInvest = _obj.ObjectAnProject.IdInvest;
        asyncHandler.EmployeeId = Sungero.Company.Employees.Current.Id.ToString();
        asyncHandler.ExecuteAsync();
        
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.ObjectAnSales.Resources.RequestPIBAndBTIResultMessage, MessageType.Information);
      }
    }
    //конец Добавлено Avis Expert
  }
}