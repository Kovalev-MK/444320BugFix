using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Counterparty;

namespace lenspec.Etalon.Client
{
  partial class CounterpartyActions
  {
    public virtual void ShowTenderDocumentslenspec(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var documents = Functions.Counterparty.Remote.GetTenderDocuments(_obj);
      documents.Show();
    }

    public virtual bool CanShowTenderDocumentslenspec(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowApprovalCounterpartyBaseavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var documents = avis.ApprovingCounterpartyDEB.ApprovalCounterpartyBases.GetAll(x => x.Counterparty == _obj);
      documents.Show(lenspec.Etalon.Counterparties.Resources.ShowApprovalCounterpartyName);
    }

    public virtual bool CanShowApprovalCounterpartyBaseavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void Export1Cavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
    }

    public virtual bool CanExport1Cavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (Employees.Current.IncludedIn(Roles.Administrators))
        return true;
      
      return false;
    }

    public virtual void AllContractsYearavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Sungero.Contracts.ContractualDocuments.GetAll(c => c.Counterparty == _obj && c.ValidFrom != null && c.ValidFrom.Value.Year == Calendar.Now.Year).Show(lenspec.Etalon.Counterparties.Resources.AllContractsYear);
    }

    public virtual bool CanAllContractsYearavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void AllContractsavis(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Sungero.Contracts.ContractualDocuments.GetAll(c => c.Counterparty == _obj).Show(lenspec.Etalon.Counterparties.Resources.AllContracts);
    }

    public virtual bool CanAllContractsavis(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }
  }
}