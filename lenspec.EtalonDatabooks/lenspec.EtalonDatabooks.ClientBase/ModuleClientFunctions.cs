using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.EtalonDatabooks.Client
{
  public class ModuleFunctions
  {

    #region [Настраиваемые гиперссылки]
    
    /// <summary>
    /// Открыть настраиваемую гиперссылку 1.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink01_ResourceKey", "OpenEditableHyperlink01_DescriptionResourceKey")]
    public void OpenEditableHyperlink01()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant01).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(1), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 2.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink02_ResourceKey", "OpenEditableHyperlink02_DescriptionResourceKey")]
    public void OpenEditableHyperlink02()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant02).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(2), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 3.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink03_ResourceKey", "OpenEditableHyperlink03_DescriptionResourceKey")]
    public void OpenEditableHyperlink03()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant03).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(3), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 4.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink04_ResourceKey", "OpenEditableHyperlink04_DescriptionResourceKey")]
    public void OpenEditableHyperlink04()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant04).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(4), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 5.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink05_ResourceKey", "OpenEditableHyperlink05_DescriptionResourceKey")]
    public void OpenEditableHyperlink05()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant05).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(5), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 6.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink06_ResourceKey", "OpenEditableHyperlink06_DescriptionResourceKey")]
    public void OpenEditableHyperlink06()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant06).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(6), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 7.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink07_ResourceKey", "OpenEditableHyperlink07_DescriptionResourceKey")]
    public void OpenEditableHyperlink07()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant07).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(7), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 8.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink08_ResourceKey", "OpenEditableHyperlink08_DescriptionResourceKey")]
    public void OpenEditableHyperlink08()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant08).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(8), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 9.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink09_ResourceKey", "OpenEditableHyperlink09_DescriptionResourceKey")]
    public void OpenEditableHyperlink09()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant09).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(9), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 10.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink10_ResourceKey", "OpenEditableHyperlink10_DescriptionResourceKey")]
    public void OpenEditableHyperlink10()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant10).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(10), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 11.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink11_ResourceKey", "OpenEditableHyperlink11_DescriptionResourceKey")]
    public void OpenEditableHyperlink11()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant11).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(11), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 12.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink12_ResourceKey", "OpenEditableHyperlink12_DescriptionResourceKey")]
    public void OpenEditableHyperlink12()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant12).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(12), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 13.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink13_ResourceKey", "OpenEditableHyperlink13_DescriptionResourceKey")]
    public void OpenEditableHyperlink13()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant13).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(13), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 14.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink14_ResourceKey", "OpenEditableHyperlink14_DescriptionResourceKey")]
    public void OpenEditableHyperlink14()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant14).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(14), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 15.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink15_ResourceKey", "OpenEditableHyperlink15_DescriptionResourceKey")]
    public void OpenEditableHyperlink15()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant15).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(15), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 16.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink16_ResourceKey", "OpenEditableHyperlink16_DescriptionResourceKey")]
    public void OpenEditableHyperlink16()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant16).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(16), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 17.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink17_ResourceKey", "OpenEditableHyperlink17_DescriptionResourceKey")]
    public void OpenEditableHyperlink17()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant17).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(17), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 18.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink18_ResourceKey", "OpenEditableHyperlink18_DescriptionResourceKey")]
    public void OpenEditableHyperlink18()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant18).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(18), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 19.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink19_ResourceKey", "OpenEditableHyperlink19_DescriptionResourceKey")]
    public void OpenEditableHyperlink19()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant19).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(19), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 20.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink20_ResourceKey", "OpenEditableHyperlink20_DescriptionResourceKey")]
    public void OpenEditableHyperlink20()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant20).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(20), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 21.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink21_ResourceKey", "OpenEditableHyperlink21_DescriptionResourceKey")]
    public void OpenEditableHyperlink21()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant21).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(21), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 22.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink22_ResourceKey", "OpenEditableHyperlink22_DescriptionResourceKey")]
    public void OpenEditableHyperlink22()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant22).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(22), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 23.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink23_ResourceKey", "OpenEditableHyperlink23_DescriptionResourceKey")]
    public void OpenEditableHyperlink23()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant23).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(23), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 24.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink24_ResourceKey", "OpenEditableHyperlink24_DescriptionResourceKey")]
    public void OpenEditableHyperlink24()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant24).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(24), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 25.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink25_ResourceKey", "OpenEditableHyperlink25_DescriptionResourceKey")]
    public void OpenEditableHyperlink25()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant25).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(25), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 26.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink26_ResourceKey", "OpenEditableHyperlink26_DescriptionResourceKey")]
    public void OpenEditableHyperlink26()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant26).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(26), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 27.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink27_ResourceKey", "OpenEditableHyperlink27_DescriptionResourceKey")]
    public void OpenEditableHyperlink27()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant27).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(27), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 28.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink28_ResourceKey", "OpenEditableHyperlink28_DescriptionResourceKey")]
    public void OpenEditableHyperlink28()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant28).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(28), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 29.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink29_ResourceKey", "OpenEditableHyperlink29_DescriptionResourceKey")]
    public void OpenEditableHyperlink29()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant29).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(29), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }

    /// <summary>
    /// Открыть настраиваемую гиперссылку 30.
    /// </summary>
    [LocalizeFunction("OpenEditableHyperlink30_ResourceKey", "OpenEditableHyperlink30_DescriptionResourceKey")]
    public void OpenEditableHyperlink30()
    {
      var url = ConstantDatabooks.GetAll(c => c.Code == PublicConstants.ConstantDatabook.EditableHyperlinkConstant30).First().Value;
      if (string.IsNullOrEmpty(url))
        Dialogs.ShowMessage(lenspec.EtalonDatabooks.Resources.EmptyHyperlinkErrorFormat(30), MessageType.Error);
      else
        Hyperlinks.Open(url);
    }
    
    #endregion [Настраиваемые гиперссылки]

  }
}