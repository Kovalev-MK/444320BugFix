using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustomerRequest;

namespace avis.CustomerRequests
{
  partial class CustomerRequestClientHandlers
  {

    //Добавлено Avis Expert
    public virtual void ClaimAmtValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue.Value < 0)
      {
        e.AddError(avis.CustomerRequests.CustomerRequests.Resources.AmountMustBePositive);
        return;
      }
    }

    public override void PreparedByValueInput(Sungero.Docflow.Client.OfficialDocumentPreparedByValueInputEventArgs e)
    {
      if (e.NewValue != null && !lenspec.EtalonDatabooks.PublicFunctions.Module.Remote.IsAutomatedEmployee(e.NewValue))
      {
        e.AddError(lenspec.EtalonDatabooks.Resources.NeedSpecifyAuthomatedEmployee);
      }
      base.PreparedByValueInput(e);
    }

    /// <summary>
    /// Показ формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      _obj.State.Properties.Created.IsVisible = _obj.State.Properties.RegistrationNumber.IsVisible;
      
      if (!_obj.State.IsInserted)
      {
        if (_obj.ReqCategory == null || _obj.ReqCategory.IsClaim == false || 
            (_obj.ReqCategory.IsClaim == true && _obj.ReqCategory.ClaimType == avis.CustomerRequests.CategoryRequest.ClaimType.NonMat))
        {
          _obj.State.Properties.ClaimAmt.IsEnabled = false;
          _obj.State.Properties.ClaimAmt.IsRequired = false;
        }
        else
        {
          _obj.State.Properties.ClaimAmt.IsEnabled = true;
          _obj.State.Properties.ClaimAmt.IsRequired = true;
        }
      }
    }

    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.Name.IsEnabled = false;
      _obj.State.Properties.DeliveryMethod.IsEnabled = Users.Current.IncludedIn(Roles.Administrators) ||
        Users.Current.IncludedIn(Sungero.Docflow.Constants.Module.RoleGuid.ClerksRole);
      
      // В зависимости от установленных значений в категории обращения, устанавливаем обязательность полей.
      _obj.State.Properties.ObjectOfProject.IsRequired = _obj.ReqCategory != null && _obj.ReqCategory.IsObject == true;
      
      _obj.State.Properties.SDAContracts.IsRequired = !_obj.ManagementContractsMKD.Any();
      _obj.State.Properties.ManagementContractsMKD.IsRequired = !_obj.SDAContracts.Any();
    }

    public virtual void ClientValueInput(avis.CustomerRequests.Client.CustomerRequestClientValueInputEventArgs e)
    {
    }

    public virtual void ReqCategoryValueInput(avis.CustomerRequests.Client.CustomerRequestReqCategoryValueInputEventArgs e)
    {
      if (e.NewValue == null)
      {
        _obj.IsClaim = false;
        _obj.ClaimType = null;
        _obj.ClaimAmt = null;
        return;
      }
      if (!Equals(e.NewValue, e.OldValue))
      {
        _obj.IsClaim = e.NewValue.IsClaim;
        _obj.ClaimType = e.NewValue.ClaimType;
        if (e.NewValue.IsClaim == false || (e.NewValue.IsClaim == true && e.NewValue.ClaimType == avis.CustomerRequests.CategoryRequest.ClaimType.NonMat))
        {
          _obj.State.Properties.ClaimAmt.IsEnabled = false;
          _obj.State.Properties.ClaimAmt.IsRequired = false;
          _obj.ClaimAmt = null;
        }
        else
        {
          _obj.State.Properties.ClaimAmt.IsEnabled = true;
          _obj.State.Properties.ClaimAmt.IsRequired = true;
        }
      }
    }
  }
}