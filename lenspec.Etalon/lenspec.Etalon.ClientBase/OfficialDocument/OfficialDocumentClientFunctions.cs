using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.OfficialDocument;
using Sungero.Domain.Client;

namespace lenspec.Etalon.Client
{
  partial class OfficialDocumentFunctions
  {
    //Добавлено Avis Expert
    
    /// <summary>
    /// Выбор связанных документов для отправки и создания письма.
    /// </summary>
    /// <param name="relatedDocuments">Связанные документы.</param>
    [Public]
    public override void SelectRelatedDocumentsAndCreateEmail(List<Sungero.Docflow.IOfficialDocument> relatedDocuments)
    {
      base.SelectRelatedDocumentsAndCreateEmail(relatedDocuments);
    }
    
    /// <summary>
    /// Блокировать поля в Жизненном цикле.
    /// </summary>
    [Public]
    public void BlockLifecycle()
    {
      if (Users.Current.IncludedIn(Roles.Administrators))
        return;
      
      _obj.State.Properties.LifeCycleState.IsEnabled = false;
      _obj.State.Properties.RegistrationState.IsEnabled = false;
      _obj.State.Properties.InternalApprovalState.IsEnabled = false;
      _obj.State.Properties.ExecutionState.IsEnabled = false;
      _obj.State.Properties.ControlExecutionState.IsEnabled = false;
    }
    
    /// <summary>
    /// Сохранение штрихкода в папку - клиентский код.
    /// </summary>
    /// <param name="document"></param>
    [Public]
    public static void SaveBarcode(Sungero.Docflow.IOfficialDocument document, string printerName)
    {
      // Формируем отчёт.
      var report = avis.PrinterSettings.Reports.GetBarcodePageReportAvis();
      var tenantId = Sungero.Docflow.PublicFunctions.Module.Remote.GetCurrentTenantId();
      var formattedTenantId = Sungero.Docflow.PublicFunctions.Module.FormatTenantIdForBarcode(tenantId);
      // ШК с кодом компании.
      //report.barcode = string.Format("{0} - {1}", formattedTenantId, document.Id);
      //report.barcodeName = string.Format("{0} - {1}", SystemInfo.GetBrandName(), document.Id);
      // ШК без кода компании.
      report.barcode = $"{document.Id}";
      report.barcodeName = $"{document.Id}";
      
      // Сохраняем отчёт в файл.
      avis.PrinterSettings.PublicFunctions.Module.SaveFile($"{printerName};{document.Id}.pdf", report);
    }
    
    /// <summary>
    /// Зарегистрировать документ с зарезервированным номером.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public]
    public static string RegisterWithReservedNumberAvis(Etalon.IOfficialDocument document)
    {
      var error = string.Empty;
      // Валидация зарезервированных номеров, кроме тех, что добавлены в исключения.
      var numberValidationDisabled = Functions.OfficialDocument.Remote.IsNumberValidationDisabled(document);
      if (!numberValidationDisabled)
      {
        var validationNumber = Etalon.PublicFunctions.DocumentRegister.CheckRegistrationNumberFormat(lenspec.Etalon.DocumentRegisters.As(document.DocumentRegister),
                                                                                                     Sungero.Docflow.OfficialDocuments.As(document));
        if (!string.IsNullOrEmpty(validationNumber))
        {
          error = validationNumber;
          return error;
        }
      }
      
      // Список ИД доступных журналов.
      var documentRegistersIds = Sungero.Docflow.PublicFunctions.OfficialDocument.GetDocumentRegistersIdsByDocument(document, Sungero.Docflow.RegistrationSetting.SettingType.Registration);
      
      // Проверить возможность выполнения действия.
      if (document.DocumentRegister != null && !documentRegistersIds.Contains(document.DocumentRegister.Id))
      {
        error = Sungero.Docflow.Resources.NoRightToRegistrationInDocumentRegister;
        return error;
      }

      var registrationData = string.Format("{0}:\n{1} - {2}\n{3} - {4}\n{5} - {6}",
                                           Sungero.Docflow.Resources.ConfirmRegistrationWithFollowingData,
                                           Sungero.Docflow.Resources.RegistrationNumber, document.RegistrationNumber,
                                           Sungero.Docflow.Resources.RegistrationDate, document.RegistrationDate.Value.ToUserTime().ToShortDateString(),
                                           Sungero.Docflow.Resources.DocumentRegister, document.DocumentRegister);

      // Диалог регистрации с зарезервированным номером.
      var reservedRegistration = Dialogs.CreateTaskDialog(Sungero.Docflow.Resources.DocumentRegistration, registrationData);
      var reservedRegister = reservedRegistration.Buttons.AddCustom(Sungero.Docflow.Resources.Register);
      reservedRegistration.Buttons.Default = reservedRegister;
      reservedRegistration.Buttons.AddCancel();

      if (reservedRegistration.Show() == reservedRegister)
      {
        Sungero.Docflow.PublicFunctions.OfficialDocument.RegisterDocument(document, document.DocumentRegister, document.RegistrationDate,
                                                                          document.RegistrationNumber, false, true);
        Dialogs.NotifyMessage(Sungero.Docflow.Resources.SuccessRegisterNotice);
      }
      
      return error;
    }
    
    /// <summary>
    /// Зарезервировать номер.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public]
    public static string RegisterDocumentAvis(Etalon.IOfficialDocument document)
    {
      var error = string.Empty;
      // Регистрация документа с зарезервированным номером.
      if (document.RegistrationState == RegistrationState.Reserved)
      {
        Functions.OfficialDocument.RegisterWithReservedNumberAvis(document);
        return error;
      }
      
      // Список доступных журналов.
      var dialogParams = Functions.OfficialDocument.Remote.GetRegistrationDialogParamsAvis(document, Sungero.Docflow.RegistrationSetting.SettingType.Registration);

      // Проверить возможность выполнения действия.
      if (dialogParams.RegistersIds == null || !dialogParams.RegistersIds.Any())
      {
        error = Sungero.Docflow.Resources.NoDocumentRegistersAvailable;
        return error;
      }

      // Вызвать диалог.
      var result = Sungero.Docflow.Client.OfficialDocumentFunctions.RunRegistrationDialog(document, dialogParams);
      if (result != null)
      {
        Sungero.Docflow.PublicFunctions.OfficialDocument.RegisterDocument(document, result.Register, result.Date, result.Number, false, true);
        Dialogs.NotifyMessage(Sungero.Docflow.Resources.SuccessRegisterNotice);
      }
      return error;
    }
    
    /// <summary>
    /// Зарезервировать номер.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public]
    public static string ReserveNumberAvis(Sungero.Docflow.IOfficialDocument document)
    {
      // Проверка блокировки документа.
      if (Locks.GetLockInfo(document).IsLocked)
      {
        return lenspec.Etalon.OfficialDocuments.Resources.ReservedErrorMessage;
      }
      
      var error = string.Empty;
      // Список доступных журналов.
      var dialogParams = Functions.OfficialDocument.Remote.GetRegistrationDialogParamsAvis(document, Sungero.Docflow.RegistrationSetting.SettingType.Reservation);

      // Проверить возможность выполнения действия.
      if (dialogParams.RegistersIds == null || !dialogParams.RegistersIds.Any())
      {
        error = Sungero.Docflow.Resources.NoDocumentRegistersAvailableForReserve;
        return error;
      }

      if (dialogParams.RegistersIds.Count > 1 && !dialogParams.IsClerk)
      {
        error = Sungero.Docflow.Resources.ReserveSettingsRequired;
        return error;
      }
      
      // Начало блокировки, если возможно; сообщение попробуйте позже, если нет.
      var isForcedLocked = false;
      try
      {
        isForcedLocked = Locks.TryLock(document);
        if (isForcedLocked)
        {
          // Вызвать диалог.
          var result = Sungero.Docflow.Client.OfficialDocumentFunctions.RunRegistrationDialog(document, dialogParams);

          if (result != null)
          {
            Sungero.Docflow.PublicFunctions.OfficialDocument.RegisterDocument(document, result.Register, result.Date, result.Number, true, true);
          }
        }
        else
        {
          return lenspec.Etalon.OfficialDocuments.Resources.BlockDocumentErrorMessage;
        }
      }
      catch (Exception ex)
      {
        return lenspec.Etalon.OfficialDocuments.Resources.ReserveNumberErrorMessage;
      }
      finally
        // Конец блокировки.
      {
        if (isForcedLocked)
          Locks.Unlock(document);
      }

      return error;
    }
    
    /// <summary>
    /// Присвоить номер.
    /// </summary>
    /// <param name="document">Документ.</param>
    [Public]
    public static string AssignNumberAvis(Sungero.Docflow.IOfficialDocument document)
    {
      // Проверка блокировки документа.
      if (Locks.GetLockInfo(document).IsLocked)
      {
        return lenspec.Etalon.OfficialDocuments.Resources.ReservedErrorMessage;
      }

      var error = string.Empty;
      // Список доступных журналов.
      var dialogParams = Functions.OfficialDocument.Remote.GetRegistrationDialogParamsAvis(document, Sungero.Docflow.RegistrationSetting.SettingType.Numeration);

      // Проверить возможность выполнения действия.
      if (dialogParams.RegistersIds == null || !dialogParams.RegistersIds.Any())
      {
        error = Sungero.Docflow.Resources.NumberingSettingsRequired;
        return error;
      }

      if (dialogParams.RegistersIds.Count > 1)
      {
        error = Sungero.Docflow.Resources.NumberingSettingsRequired;
        return error;
      }

      // Начало блокировки, если возможно; сообщение попробуйте позже, если нет.
      var isForcedLocked = false;
      try
      {
        isForcedLocked = Locks.TryLock(document);
        if (isForcedLocked)
        {
          // Вызвать диалог.
          var result = Sungero.Docflow.Client.OfficialDocumentFunctions.RunRegistrationDialog(document, dialogParams);
          
          if (result != null)
          {
            Sungero.Docflow.PublicFunctions.OfficialDocument.RegisterDocument(document, result.Register, result.Date, result.Number, false, true);
            
            Dialogs.NotifyMessage(Sungero.Docflow.Resources.SuccessNumerationNotice);
          }
        }
        else
        {
          return lenspec.Etalon.OfficialDocuments.Resources.BlockDocumentErrorMessage;
        }
      }
      catch (Exception ex)
      {
        return lenspec.Etalon.OfficialDocuments.Resources.ReserveNumberErrorMessage;
      }
      finally
        // Конец блокировки.
      {
        if (isForcedLocked)
          Locks.Unlock(document);
      }

      return error;
    }
    //конец Добавлено Avis Expert
  }
}