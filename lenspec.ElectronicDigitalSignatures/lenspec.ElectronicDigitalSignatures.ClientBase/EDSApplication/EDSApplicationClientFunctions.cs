using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.ElectronicDigitalSignatures.EDSApplication;

namespace lenspec.ElectronicDigitalSignatures.Client
{
  partial class EDSApplicationFunctions
  {

    /// <summary>
    /// Показать Инструкцию для продления УКЭП.
    /// </summary>
    [Public]
    public static void ShowRenewalInstruction()
    {
      var url = lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.RenewalInstruction).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.NeedFillRenewalInstructionConstant, MessageType.Error);
      else
        Hyperlinks.Open(url);
    }
    
    /// <summary>
    /// Показать Инструкцию для самостоятельного аннулирования УКЭП.
    /// </summary>
    [Public]
    public static void ShowCancellationInstruction()
    {
      var url = lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.CancellationInstruction).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.NeedFillCancellationInstructionConstant, MessageType.Error);
      else
        Hyperlinks.Open(url);
    }
    
    /// <summary>
    /// Показать Инструкцию для настройки УКЭП на рабочем ПК.
    /// </summary>
    [Public]
    public static void ShowSettingInstruction()
    {
      var url = lenspec.EtalonDatabooks.ConstantDatabooks.GetAll(c => c.Code == lenspec.EtalonDatabooks.PublicConstants.ConstantDatabook.SettingInstruction).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.ElectronicDigitalSignatures.EDSApplications.Resources.NeedFillSettingInstructionConstant, MessageType.Error);
      else
        Hyperlinks.Open(url);
    }
  }
}