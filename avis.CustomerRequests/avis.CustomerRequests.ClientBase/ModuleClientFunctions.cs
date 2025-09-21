using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.CustomerRequests.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Поиск обращений клиентов.
    /// </summary>
    [LocalizeFunction("SearchCustReq_ResourceKey", "SearchCustReq_DescriptionResourceKey")]
    public virtual IQueryable<ICustomerRequest> SearchCustReq()
    {
      // Создать диалог данных клиентского договора.
      var dialog = Dialogs.CreateInputDialog("Поиск обращений клиентов");
      
      var regNumber = dialog.AddString("Рег. №", false);
      var created = dialog.AddDate("Дата документа", false);
      var businessUnit = dialog.AddSelect("Наша орг.", false, Sungero.Company.BusinessUnits.Null);
      var client = dialog.AddSelect("Клиент", false, lenspec.Etalon.People.Null);
      var contract = dialog.AddSelect("Клиентский договор", false, lenspec.SalesDepartmentArchive.SDAClientContracts.Null);
      var contractMKD = dialog.AddSelect("Договор МКД", false, avis.ManagementCompanyJKHArhive.ManagementContractMKDs.Null);
      
      dialog.Buttons.AddOkCancel();
      dialog.Buttons.Default = DialogButtons.Ok;
      dialog.SetOnButtonClick((args) =>
                              {
                                if (string.IsNullOrEmpty(regNumber.Value) && created.Value == null && client.Value == null &&
                                    contract.Value == null && businessUnit.Value == null && contractMKD.Value == null)
                                {
                                  args.AddError(avis.CustomerRequests.Resources.NeedFillAnyField);
                                }
                              });
      if (dialog.Show() == DialogButtons.Ok)
      {
        return Functions.Module.Remote.GetFilteredCustumRequests(regNumber.Value, created.Value, client.Value, contract.Value, businessUnit.Value, contractMKD.Value);
      }
      return null;
    }
    
    
    
  }

  
}