using System;
using Sungero.Core;

namespace lenspec.Etalon.Module.Parties.Constants
{
  public static class Module
  {
    #region GUID Ролей.
    
    /// <summary>
    /// GUID роли "Полные права на управление архивом для управляющих компаний (ЖКХ)".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid FullPermitArhivJKH = Guid.Parse("15408225-08D2-45A5-9CCB-B702C0B88676");
    
    /// <summary>
    /// GUID роли "Согласующий сотрудник ДБ".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid DEBCoordinationgGuid = Guid.Parse("4A7F6602-B526-4915-A4FD-EF6F68A25BC9");
    
    /// <summary>
    /// GUID роли "Руководство ДБ".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid DEBManagementGuid = Guid.Parse("345FAADF-56E2-4678-9E76-F8EE8A9BEA88");
    
    /// <summary>
    /// GUID роли "Согласующий по тендерам".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid TenderCoordinatorGuid = Guid.Parse("293B0B30-F000-4868-A032-042423A444B5");
    
    /// <summary>
    /// GUID роли "Ответственные за тендеры".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid TenderResponsibleGuid = Guid.Parse("A131D5FF-F6BB-4630-A6E5-5787BC1482BD");
    
    /// <summary>
    /// GUID роли "Ответственные за справочник "Ответственные по контрагентам"".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ResponsibleByCounterpartiesDiadocGuid = Guid.Parse("1C25CCD9-744C-4A87-997D-E40EDC1AE6AF");
    
    /// <summary>
    /// GUID роли "Полные права на Клиенты (собственники) для Канцелярий УК".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid IsClientOwnerAccessGuid = Guid.Parse("7B06FF6A-9DCA-4C5E-8E8C-5770EFC295AF");
    
    /// <summary>
    /// GUID вида документа "Учредительный документ".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ConstituentDocumentKind = Guid.Parse("460923D2-7F24-4F7D-B517-A73357551771");
    
    /// <summary>
    /// GUID вида документа "Устав и изменения".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid CharterAndChangesKind = Guid.Parse("9FD4F947-79AC-43A9-9F56-73FB8EBE838D");
    
    /// <summary>
    /// GUID вида документа "Выписка из ЕГРЮЛ/ЕГРИП".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ExtractFromEGRULKind = Guid.Parse("16AC127C-CB00-4BC3-9287-A649D00207C8");

    /// <summary>
    /// GUID роли "Ответственные за выгрузку контрагентов в 1С".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid ResponsibleUnloadingContractors1CGuid = Guid.Parse("f7095f41-d937-4e4b-a3b8-7f6ab5118920");     
    
    /// <summary>
    /// GUID роли "Полные права на справочник Банковские реквизиты".
    /// </summary>
    [Sungero.Core.Public]
    public static readonly Guid BankDetailsRoleGuid = Guid.Parse("AF6D04B7-3B27-423E-B532-E6C805356120");
    
    #endregion
    }
}