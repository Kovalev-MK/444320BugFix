using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Contract;

namespace lenspec.Etalon
{
  partial class ContractSharedHandlers
  {

    public virtual void IsDeferredPaymentlenspecChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.Contract.IsDeferredPayment(_obj);
      if (e.NewValue != true)
        _obj.DaysOfDefermentlenspec = null;
    }

    public override void CounterpartyChanged(Sungero.Docflow.Shared.ContractualDocumentBaseCounterpartyChangedEventArgs e)
    {
      base.CounterpartyChanged(e);
      if (Equals(e.NewValue, e.OldValue))
        return;
      
      var counterparty = lenspec.Etalon.Counterparties.As(e.NewValue);
      var isSalesAgent = false;
      if (counterparty != null)
        isSalesAgent = counterparty.SalesAgentlenspec == true;
      
      e.Params.AddOrUpdate(lenspec.Etalon.Contracts.Resources.IsCounterpartySalesAgent, isSalesAgent);
      if (isSalesAgent)
        _obj.Versions.Clear();
    }

    public override void TotalAmountChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      base.TotalAmountChanged(e);
    }

    public override void ConstructionObjectsavisChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.ConstructionObjectsavisChanged(e);
      
      double summ = 0;
      
      foreach (var constructionObject in _obj.ConstructionObjectsavis)
      {
        if (constructionObject.Summ != null)
          summ += constructionObject.Summ.Value;
      }
      
      _obj.TotalAmount = summ;
    }

    /// <summary>
    /// ��������� �������� �������� "��������".
    /// </summary>
    /// <param name="e"></param>
    public virtual void IsFrameworkavisChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      PublicFunctions.Contract.IsFramework(_obj);
    }

  }
}