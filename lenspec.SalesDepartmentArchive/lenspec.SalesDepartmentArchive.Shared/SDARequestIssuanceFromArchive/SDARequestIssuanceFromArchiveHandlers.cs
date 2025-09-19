using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDARequestIssuanceFromArchive;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDARequestIssuanceFromArchiveListOfDocumentSharedCollectionHandlers
  {

    //Добавлено Avis Expert
    public virtual void ListOfDocumentDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      var nextDocuments = _obj.ListOfDocument.Where(x => x.Id > _deleted.Id);
      foreach(var doc in nextDocuments)
      {
        doc.Number = doc.Number - 1;
      }
    }

    public virtual void ListOfDocumentAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Number = _obj.ListOfDocument.Count;
    }
    //конец Добавлено Avis Expert
  }

  partial class SDARequestIssuanceFromArchiveSharedHandlers
  {

    //Добавлено Avis Expert
    public override void RegistrationNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.RegistrationNumberChanged(e);
      _obj.RequestNumber = e.NewValue;
    }

    public override void DepartmentChanged(Sungero.Docflow.Shared.OfficialDocumentDepartmentChangedEventArgs e)
    {
      base.DepartmentChanged(e);
      FillName();
    }

    public override void CreatedChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.CreatedChanged(e);
      FillName();
    }

    public virtual void RequestNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      FillName();
    }
    //конец Добавлено Avis Expert

  }
}