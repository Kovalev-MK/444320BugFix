using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OrderBase;

namespace lenspec.Etalon
{
  partial class OrderBaseAddresseeslenspecSharedHandlers
  {

    public virtual void AddresseeslenspecAddresseeChanged(lenspec.Etalon.Shared.OrderBaseAddresseeslenspecAddresseeChangedEventArgs e)
    {
      if (e.NewValue != null && e.NewValue.Department != null)
        _obj.Department = e.NewValue.Department;
    }
  }

  partial class OrderBaseAddresseeslenspecSharedCollectionHandlers
  {

    public virtual void AddresseeslenspecDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextItems = _obj.Addresseeslenspec.Where(x => x.Id > _deleted.Id);
      foreach(var item in nextItems)
      {
        item.Number = Convert.ToInt32(item.Number) - 1;
      }
    }

    public virtual void AddresseeslenspecAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = (_obj.Addresseeslenspec.Max(a => a.Number) ?? 0) + 1;
    }
  }

  partial class OrderBaseSharedHandlers
  {

    public override void PreparedByChanged(Sungero.Docflow.Shared.OfficialDocumentPreparedByChangedEventArgs e)
    {
      base.PreparedByChanged(e);
      
      _obj.Assignee = e.NewValue;
    }

    public override void NameChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.NameChanged(e);
    }

    public virtual void FillEmpDeplenspecChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == true)
      {  _obj.State.Properties.Departmenslenspec.IsEnabled = true;
        _obj.State.Properties.Departmenslenspec.IsRequired = true;
        _obj.State.Properties.FillOptlenspec.IsEnabled = true;
        _obj.State.Properties.FillOptlenspec.IsRequired = true;
      }
      else
      {
        _obj.State.Properties.Departmenslenspec.IsEnabled = false;
        _obj.State.Properties.Departmenslenspec.IsRequired = false;
        _obj.Departmenslenspec.Clear();
        _obj.State.Properties.FillOptlenspec.IsEnabled = false;
        _obj.State.Properties.FillOptlenspec.IsRequired = false;
        _obj.FillOptlenspec = null;
        if (_obj.Addresseeslenspec.Count() != 0)
          _obj.Addresseeslenspec.Clear();
      }
    }

    public virtual void IsManyAddresseeslenspecChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == true)
      {
        Functions.OrderBase.ClearAndFillFirstAddressee(_obj);
        Functions.OrderBase.SetManyAddresseesPlaceholder(_obj);
      }
      else if (e.NewValue == false)
      {
        Functions.OrderBase.FillAddresseeFromAddressees(_obj);
        Functions.OrderBase.ClearAndFillFirstAddressee(_obj);
      }

    }
  }
}