using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.SalesDepartmentArchive.SDARequestSubmissionToArchive;

namespace lenspec.SalesDepartmentArchive
{
  partial class SDARequestSubmissionToArchiveListOfDocumentSharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void ListOfDocumentRealEstateDocumentKindChanged(lenspec.SalesDepartmentArchive.Shared.SDARequestSubmissionToArchiveListOfDocumentRealEstateDocumentKindChangedEventArgs e)
    {
      var clientContractDocumentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.SDAClientContractKind);
      if (e.NewValue != null && e.NewValue.DocumentKind != null && e.NewValue.DocumentKind.Equals(clientContractDocumentKind) && _obj.ClientContract != null)
      {
        _obj.DocumentDate = _obj.ClientContract.ClientDocumentDate;
      }
    }

    public virtual void ListOfDocumentClientContractChanged(lenspec.SalesDepartmentArchive.Shared.SDARequestSubmissionToArchiveListOfDocumentClientContractChangedEventArgs e)
    {
      var clientContractDocumentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.SDAClientContractKind);
      if (e.NewValue != null && _obj.RealEstateDocumentKind != null &&_obj.RealEstateDocumentKind.DocumentKind != null && _obj.RealEstateDocumentKind.DocumentKind.Equals(clientContractDocumentKind))
      {
        _obj.DocumentDate = e.NewValue.ClientDocumentDate;
      }
    }
    //конец Добавлено Avis Expert
  }

  partial class SDARequestSubmissionToArchiveListOfDocumentSharedCollectionHandlers
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
      _added.Stitched = SalesDepartmentArchive.SDARequestSubmissionToArchiveListOfDocument.Stitched.No;
    }
    //конец Добавлено Avis Expert
  }

  partial class SDARequestSubmissionToArchiveSharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void StorageAddressChanged(lenspec.SalesDepartmentArchive.Shared.SDARequestSubmissionToArchiveStorageAddressChangedEventArgs e)
    {
      FillName();
    }

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      FillName();
    }

    public override void CreatedChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.CreatedChanged(e);
      FillName();
    }

    public override void DepartmentChanged(Sungero.Docflow.Shared.OfficialDocumentDepartmentChangedEventArgs e)
    {
      base.DepartmentChanged(e);
      FillName();
    }

    public override void RegistrationNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.RegistrationNumberChanged(e);
      FillName();
    }
    //конец Добавлено Avis Expert

  }
}