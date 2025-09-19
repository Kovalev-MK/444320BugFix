using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ContractualDocument;

namespace lenspec.Etalon
{
  partial class ContractualDocumentConstructionObjectsavisClientHandlers
  {

    public virtual void ConstructionObjectsavisNumberValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue < 1)
        e.AddError(Sungero.Docflow.OfficialDocuments.Resources.NumberAddresseeListIsNotPositive);
    }
  }

  partial class ContractualDocumentClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      _obj.State.Properties.ConstructionObjectsavis.Properties.NumberInContract.IsVisible = false;
      if (lenspec.Etalon.SupAgreements.Is(_obj))
      {
        _obj.State.Properties.ConstructionObjectsavis.Properties.NumberInContract.IsVisible = true;
      }
      base.Showing(e);
    }

    public override void TotalAmountValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      //base.TotalAmountValueInput(e);
      // TODO: Удален ресурс после обновления!!!
      if (e.NewValue < 0) 
        e.AddError(Sungero.Docflow.Resources.TotalAmountMustBePositive);
      this._obj.State.Properties.TotalAmount.HighlightColor = Sungero.Core.Colors.Empty;
    }

    /// <summary>
    /// Обновление формы.
    /// </summary>
    /// <param name="e"></param>
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.Subject.IsRequired = false;
      _obj.State.Properties.DeliveryMethod.IsVisible = true;
      _obj.State.Properties.DeliveryMethod.IsEnabled = true;
      
      // ������ ������������ ���� "���" � ����������� �� "������ ����".
      PublicFunctions.ContractualDocument.IsRequiredPropertyOurCF(_obj);
      
      // Установка обязательности полей в зависимости от выбранного контрагента.
      PublicFunctions.ContractualDocument.CounterpartyIsRequired(_obj);
      // Блокируем поля при изменении "Тип договора".
      PublicFunctions.ContractualDocument.EditContractType(_obj);
      // Блокируем поле сумма на вкладке свойство. 
      PublicFunctions.ContractualDocument.BlockTotalAmmount(_obj);
      // Видимость и обязательность поля Объект
      var isComputeApprovaler = _obj.OurCFavis != null && _obj.OurCFavis.IsComputeApprovalers == true;
      _obj.State.Properties.Objectlenspec.IsVisible = isComputeApprovaler;
      _obj.State.Properties.Objectlenspec.IsRequired = isComputeApprovaler;
      
      // Делаем доступным поле только роли Администратор.
      _obj.State.Properties.ExternalApprovalState.IsEnabled = Employees.Current.IncludedIn(Roles.Administrators);
      
      if (e.Params.Contains(Constants.Contracts.ContractualDocument.Params.IsCreatedFromIntragroupDocument))
        _obj.State.Properties.Department.IsEnabled = true;
    }

  }
}