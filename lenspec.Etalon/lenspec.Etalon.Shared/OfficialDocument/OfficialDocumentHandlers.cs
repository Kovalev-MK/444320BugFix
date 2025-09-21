using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OfficialDocument;

namespace lenspec.Etalon
{
  partial class OfficialDocumentVersionsSharedHandlers
  {

    //Добавлено Avis Expert
    public override void VersionsBodyChanged(Sungero.Domain.Shared.BinaryDataPropertyChangedEventArgs e)
    {
      base.VersionsBodyChanged(e);
      
      var entity = Etalon.OfficialDocuments.As(_obj.RootEntity);
      if (entity != null && entity.Archiveavis.HasValue && entity.Archiveavis.Value == true)
      {
        foreach(var property in _obj.State.Properties)
        {
          property.IsRequired = false;
        }
        foreach(var property in entity.State.Properties)
        {
          property.IsRequired = false;
        }
      }
    }
    //конец Добавлено Avis Expert
  }

  partial class OfficialDocumentVersionsSharedCollectionHandlers
  {

    //Добавлено Avis Expert
    public override void VersionsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      var entity = Etalon.OfficialDocuments.As(_added.RootEntity);
      if (entity != null && entity.Archiveavis.HasValue && entity.Archiveavis.Value == true)
      {
        foreach(var property in entity.State.Properties)
        {
          property.IsRequired = false;
        }
        foreach(var property in _added.State.Properties)
        {
          property.IsRequired = false;
        }
      }
      base.VersionsAdded(e);
    }
    //конец Добавлено Avis Expert
  }

  partial class OfficialDocumentSharedHandlers
  {

    //Добавлено Avis Expert
    public virtual void ArchiveavisChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue.Value == true)
      {
        foreach(var property in _obj.State.Properties)
        {
          property.IsRequired = false;
        }
      }
    }
    //конец Добавлено Avis Expert

  }
}