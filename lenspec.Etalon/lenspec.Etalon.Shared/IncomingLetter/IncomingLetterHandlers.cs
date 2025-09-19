using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.IncomingLetter;

namespace lenspec.Etalon
{
  partial class IncomingLetterManagementContractsMKDlenspecSharedCollectionHandlers
  {

    public virtual void ManagementContractsMKDlenspecDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      _obj.Relations.RemoveFrom(Sungero.Docflow.PublicConstants.Module.CorrespondenceRelationName, _deleted.ManagementContractMKD);
    }

    public virtual void ManagementContractsMKDlenspecAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
    }
  }

  partial class IncomingLetterClientContractslenspecSharedCollectionHandlers
  {

    public virtual void ClientContractslenspecDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      _obj.Relations.RemoveFrom(Sungero.Docflow.PublicConstants.Module.CorrespondenceRelationName, _deleted.ClientContract);
    }

    public virtual void ClientContractslenspecAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
    }
  }

  partial class IncomingLetterManagementContractsMKDlenspecSharedHandlers
  {

    public virtual void ManagementContractsMKDlenspecManagementContractMKDChanged(lenspec.Etalon.Shared.IncomingLetterManagementContractsMKDlenspecManagementContractMKDChangedEventArgs e)
    {
      _obj.IncomingLetter.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.CorrespondenceRelationName, e.OldValue, e.NewValue);
    }
  }

  partial class IncomingLetterClientContractslenspecSharedHandlers
  {

    public virtual void ClientContractslenspecClientContractChanged(lenspec.Etalon.Shared.IncomingLetterClientContractslenspecClientContractChangedEventArgs e)
    {
      _obj.IncomingLetter.Relations.AddFromOrUpdate(Sungero.Docflow.PublicConstants.Module.CorrespondenceRelationName, e.OldValue, e.NewValue);
    }
  }

  partial class IncomingLetterSharedHandlers
  {

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
    }

    public virtual void ClientContractslenspecChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      if (_obj.ClientContractslenspec.Count() == 1 && _obj.ClientContractslenspec.Single().ClientContract != null)
      {
        var clientContract = _obj.ClientContractslenspec.Single().ClientContract;
        if (clientContract.ObjectAnProject != null)
          _obj.OurCFlenspec = clientContract.ObjectAnProject.OurCF;
        
        if (_obj.Archiveavis == true)
          _obj.BusinessUnit = clientContract.BusinessUnit;
      }
      else
        _obj.OurCFlenspec = null;
    }

    //Добавлено Avis Expert
    public override void CorrespondentChanged(Sungero.Docflow.Shared.IncomingDocumentBaseCorrespondentChangedEventArgs e)
    {
      if (_obj.IsCorrespondenceWithinGrouplenspec == true)
      {
        FillName();
        return;
      }
      
      base.CorrespondentChanged(e);
    }

    public virtual void ManagementContractMKDavisChanged(lenspec.Etalon.Shared.IncomingLetterManagementContractMKDavisChangedEventArgs e)
    {
    }

    public virtual void ClientContractlenspecChanged(lenspec.Etalon.Shared.IncomingLetterClientContractlenspecChangedEventArgs e)
    {
    }
    //конец Добавлено Avis Expert

    public override void DatedChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      base.DatedChanged(e);
      FillName();
    }
  }

}