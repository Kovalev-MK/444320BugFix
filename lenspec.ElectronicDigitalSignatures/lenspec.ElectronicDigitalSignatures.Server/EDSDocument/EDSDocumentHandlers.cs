using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.EDSDocument;

namespace lenspec.ElectronicDigitalSignatures
{
  partial class EDSDocumentServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      var edsApplications = _obj.Relations.GetRelatedFromDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName)
        .Where(x => lenspec.ElectronicDigitalSignatures.EDSApplications.Is(x))
        .Select(x => lenspec.ElectronicDigitalSignatures.EDSApplications.As(x));
      
      // Выдать получателю УКЭП права на просмотр документа.
      foreach (var edsApplication in edsApplications)
      {
        var preparedBy = edsApplication.PreparedBy;
        if (preparedBy != null && !_obj.AccessRights.IsGrantedDirectly(DefaultAccessRightsTypes.Read, preparedBy) &&
            _obj.AccessRights.StrictMode != AccessRightsStrictMode.Enhanced)
          _obj.AccessRights.Grant(preparedBy, DefaultAccessRightsTypes.Read);
      }
    }
  }

}