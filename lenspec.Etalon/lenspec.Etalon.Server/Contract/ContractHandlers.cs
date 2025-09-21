using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Contract;

namespace lenspec.Etalon
{

  partial class ContractServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      //lenspec.Etalon.PublicFunctions.Contract.CalculateRemainingAmount(_obj);
    }

    /// <summary>
    /// ��������.
    /// </summary>
    /// <param name="e"></param>
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsFrameworkavis = false;
      _obj.IsDeferredPaymentlenspec = false;
    }
  }

}