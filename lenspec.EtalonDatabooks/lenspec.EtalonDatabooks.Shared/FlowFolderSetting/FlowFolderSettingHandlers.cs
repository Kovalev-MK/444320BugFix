using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.EtalonDatabooks.FlowFolderSetting;

namespace lenspec.EtalonDatabooks
{

  partial class FlowFolderSettingSharedHandlers
  {

    public virtual void TaskTypeChanged(lenspec.EtalonDatabooks.Shared.FlowFolderSettingTaskTypeChangedEventArgs e)
    {
      Functions.FlowFolderSetting.FillName(_obj);
    }
    
    public virtual void FolderNameChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      Functions.FlowFolderSetting.FillName(_obj);
    }
    //конец Добавлено Avis Expert

  }
}