using System;
using Sungero.Core;

namespace lenspec.EtalonMemos.Constants
{
  public static class Module
  {
    /// <summary>
    /// GUID группы вложений Документ для исполнения ActionItemExecutionTask
    /// </summary>
    public static readonly Guid MainDocumentGroupID = Guid.Parse("804f50fe-f3da-411b-bb2e-e5373936e029");

    /// <summary>
    /// GUID типа документа Служебная записка.
    /// </summary>
    public static readonly Guid MemoTypeGuid = Guid.Parse("95af409b-83fe-4697-a805-5a86ceec33f5");
  }
}