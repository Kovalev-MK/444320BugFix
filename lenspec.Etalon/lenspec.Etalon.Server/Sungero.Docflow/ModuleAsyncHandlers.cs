using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Docflow.Server
{
  partial class ModuleAsyncHandlers
  {

    #region [Документооборот. Массовое назначение прав на задачи]

    /// <summary>
    /// Выдача прав на задачи по документам.
    /// </summary>
    public virtual void GrantAccessRightsToTasksByDocumentsAsynclenspec(lenspec.Etalon.Module.Docflow.Server.AsyncHandlerInvokeArgs.GrantAccessRightsToTasksByDocumentsAsynclenspecInvokeArgs args)
    {
      var prefix = "avis - GrantAccessRightsToTasksByDocumentsAsync -";
      args.Retry = Functions.Module.GrantAccessRightsToTasksByDocuments(args.DocumentIDs, args.RuleID, prefix);
    }
    
    /// <summary>
    /// Выборка документов и выдача прав на связанные задачи.
    /// </summary>
    [Obsolete("Устаревший АО. Используйте GrantAccessRightsToTasksByDocumentsAsynclenspec.", true)]
    public virtual void GrantAccessRightsToTasksavis(lenspec.Etalon.Module.Docflow.Server.AsyncHandlerInvokeArgs.GrantAccessRightsToTasksavisInvokeArgs args)
    {
      // Deprecated.
    }

    /// <summary>
    /// Выдача прав на задачи в рамках одного документа
    /// </summary>
    [Obsolete("Устаревший АО. Используйте GrantAccessRightsToTasksByDocumentsAsynclenspec.", true)]
    public virtual void GrantAccessRightsToTaskByDocumentavis(lenspec.Etalon.Module.Docflow.Server.AsyncHandlerInvokeArgs.GrantAccessRightsToTaskByDocumentavisInvokeArgs args)
    {
      // Deprecated.
    }
    
    #endregion [Документооборот. Массовое назначение прав на задачи]
  }
}