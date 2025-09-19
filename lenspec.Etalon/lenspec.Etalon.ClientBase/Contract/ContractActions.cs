using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Contract;

namespace lenspec.Etalon.Client
{
  partial class ContractActions
  {
    public override void SaveAndClose(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!CheckVersionExists(e))
        return;
      
      base.SaveAndClose(e);
    }

    private bool CheckVersionExists(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!_obj.HasVersions)
      {
        e.AddError(lenspec.Etalon.Contracts.Resources.CreateVersion);
        return false;
      }
      return true;
    }
    
    public override bool CanSaveAndClose(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSaveAndClose(e);
    }

    public override void Save(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (!CheckVersionExists(e))
        return;
      
      base.Save(e);
    }

    public override bool CanSave(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSave(e);
    }

    public override void ScanInNewVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.ScanInNewVersion(e);
    }

    public override bool CanScanInNewVersion(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !IsSalesAgent(e) && base.CanScanInNewVersion(e);
    }
    
    private bool IsSalesAgent(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      var isSalesAgent = false;
      e.Params.TryGetValue(lenspec.Etalon.Contracts.Resources.IsCounterpartySalesAgent, out isSalesAgent);
      
      return isSalesAgent;
    }

    public override void CreateFromScanner(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateFromScanner(e);
    }

    public override bool CanCreateFromScanner(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !IsSalesAgent(e) && base.CanCreateFromScanner(e);
    }

    public override void CreateFromFile(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateFromFile(e);
    }

    public override bool CanCreateFromFile(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !IsSalesAgent(e) && base.CanCreateFromFile(e);
    }

    public override void CreateFromTemplate(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (_obj.Counterparty == null)
      {
        e.AddError(lenspec.Etalon.Contracts.Resources.FillCounterparty);
        return;
      }
      base.CreateFromTemplate(e);
    }

    public override bool CanCreateFromTemplate(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanCreateFromTemplate(e);
    }

    public override void CreateSupAgreement(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.CreateSupAgreement(e);
    }

    public override bool CanCreateSupAgreement(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (_obj.InternalApprovalState != Contract.InternalApprovalState.Signed)
        return false;
      
      return base.CanCreateSupAgreement(e);
    }

  }

}