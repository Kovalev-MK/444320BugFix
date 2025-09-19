using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Memo;

namespace lenspec.Etalon
{
  partial class MemoSharedHandlers
  {

    //Добавлено Avis Expert
    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      
      if (e.NewValue == null)
      {
        _obj.Department = null;
        _obj.Addressee = null;
        _obj.OurSignatory = null;
      }
      else
      {
        if (_obj.Department != null && !_obj.Department.BusinessUnit.Equals(e.NewValue))
        {
          _obj.Department = null;
        }
        if (_obj.Addressee != null && _obj.Addressee.Department != null && !_obj.Addressee.Department.BusinessUnit.Equals(e.NewValue))
        {
          _obj.Addressee = null;
        }
        if (_obj.OurSignatory != null && _obj.OurSignatory.Department != null && !_obj.OurSignatory.Department.BusinessUnit.Equals(e.NewValue))
        {
          _obj.OurSignatory = null;
        }
      }
    }
    //конец Добавлено Avis Expert
  }

}