using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.DocumentTemplate;

namespace lenspec.Etalon
{
  partial class DocumentTemplateClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      if (!e.Params.Contains(lenspec.Etalon.DocumentTemplates.Resources.DocumentTypesRestrictionEnabled))
        e.Params.Add(
          lenspec.Etalon.DocumentTemplates.Resources.DocumentTypesRestrictionEnabled,
          IsDocumentTypeRestrictionEnabled()
         );
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      e.Params.Add(
        lenspec.Etalon.DocumentTemplates.Resources.DocumentTypesRestrictionEnabled,
        IsDocumentTypeRestrictionEnabled()
       );
      
      // Установка доступности полей.
      _obj.State.Properties.IsSalesAgentlenspec.IsEnabled = Users.Current.IncludedIn(Roles.Administrators);
    }
    
    /// <summary>
    /// Проверить, необходимо ли ограничить доступные виды документов.
    /// </summary>
    /// <returns>true, если необходимо ограничить доступность; Иначе – false.</returns>
    private bool IsDocumentTypeRestrictionEnabled()
    {
      return Users.Current.IncludedIn(EtalonDatabooks.PublicConstants.Module.ResponsibleContractualTemplates);
    }

    public override void DocumentTypeValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      base.DocumentTypeValueInput(e);

      bool isDocumentTypeRestrictionEnabled;
      e.Params.TryGetValue(
        lenspec.Etalon.DocumentTemplates.Resources.DocumentTypesRestrictionEnabled,
        out isDocumentTypeRestrictionEnabled
       );
      
      if (Functions.DocumentTemplate.Remote.IsDocumentTypeRestrictionError(e.NewValue, isDocumentTypeRestrictionEnabled))
        e.AddError(lenspec.Etalon.DocumentTemplates.Resources.DocumentTypesRestrictionError);
    }

  }
}