using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ActionItemExecutionAssignment;

namespace lenspec.Etalon.Client
{
  partial class ActionItemExecutionAssignmentActions
  {
    public virtual void CreateOutLetteravis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var custReq = avis.CustomerRequests.CustomerRequests.As(_obj.DocumentsGroup.OfficialDocuments.FirstOrDefault());
      SalesDepartmentArchive.ISDAClientContract selectedContract = null;
      if (custReq.SDAContracts.Count() > 1)
      {
        var reqContracts = custReq.SDAContracts.Select(x => x.Contract).ToArray();
        var dialog = Dialogs.CreateInputDialog("Выбор клиентского договора для исходящего письма");
        var contract = dialog.AddSelect("Клиентский договор", true, SalesDepartmentArchive.SDAClientContracts.Null).From(reqContracts);
        dialog.Buttons.AddOkCancel();
        dialog.Buttons.Default = DialogButtons.Ok;
        if (dialog.Show() == DialogButtons.Ok)
        {
          selectedContract = contract.Value;
        }
      }
      else if (custReq.SDAContracts.Any())
      {
        selectedContract = custReq.SDAContracts.FirstOrDefault().Contract;
      }
      var outLetter = lenspec.Etalon.OutgoingLetters.Create();
      outLetter.Subject = custReq.Subject;
      outLetter.Correspondent = custReq.Client;
      outLetter.DeliveryMethod = custReq.DeliveryMethod;
      outLetter.BusinessUnit = custReq.BusinessUnit;
      outLetter.ClientContractlenspec = selectedContract;
      outLetter.ManagementContractMKDavis = custReq.ManagementContractsMKD.FirstOrDefault()?.ManagementContractMKD;
      Etalon.OutgoingLetters.As(outLetter).InResponseToCustRequestavis = custReq;
      Etalon.OutgoingLetters.As(outLetter).InResponseToCustomavis = custReq;
      outLetter.Show();
    }

    public virtual bool CanCreateOutLetteravis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var document = _obj.DocumentsGroup.OfficialDocuments.FirstOrDefault();
      return document != null && avis.CustomerRequests.CustomerRequests.Is(document);
    }

    // Добавлено avis.
    
    public override void PrintActionItem(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.PrintActionItem(e);
    }
    
    /// <summary>
    /// Возможность выполнения действия "Печать поручения".
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public override bool CanPrintActionItem(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Для роли "Права на поручения без фильтрации по субординации" - коробка.
      if (!Users.Current.IncludedIn(lenspec.EtalonDatabooks.PublicConstants.Module.OfficeAssignment))
      {
        return false;
      }
      return base.CanPrintActionItem(e);
    }
    
    // Конец добавлено avis.
  }
}