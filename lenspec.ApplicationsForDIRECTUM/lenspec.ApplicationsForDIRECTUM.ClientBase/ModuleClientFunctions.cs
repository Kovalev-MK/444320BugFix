using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.ApplicationsForDIRECTUM.Client
{
  public class ModuleFunctions
  {
    //Добавлено Avis Expert
    
    /// <summary>
    /// Другие заявки.
    /// </summary>
    [LocalizeFunction("OtherShow_ResourceKey", "OtherShow_DescriptionResourceKey")]
    public virtual void OtherShow()
    {
      var task = AutomatedSupportTickets.PublicFunctions.Module.Remote.CreateEditComponentRXRequestTask();
      task.TypeRequest = AutomatedSupportTickets.EditComponentRXRequestTask.TypeRequest.Other;
      task.Show();
    }
    
    /// <summary>
    /// Запрос прав на просмотр документов.
    /// </summary>
    [LocalizeFunction("EditDocShow_ResourceKey", "EditDocShow_DescriptionResourceKey")]
    public virtual void EditDocShow()
    {
      var task = AutomatedSupportTickets.PublicFunctions.Module.Remote.CreateEditComponentRXRequestTask();
      task.TypeRequest = AutomatedSupportTickets.EditComponentRXRequestTask.TypeRequest.EditDoc;
      task.Show();
    }
    
    /// <summary>
    /// Заявки на изменение процессов.
    /// </summary>
    [LocalizeFunction("EditProcessShow_ResourceKey", "EditProcessShow_DescriptionResourceKey")]
    public virtual void EditProcessShow()
    {
      var task = AutomatedSupportTickets.PublicFunctions.Module.Remote.CreateEditComponentRXRequestTask();
      task.TypeRequest = AutomatedSupportTickets.EditComponentRXRequestTask.TypeRequest.EditProcess;
      task.Show();
    }
    
    /// <summary>
    /// Создать заявку на формирование замещения.
    /// </summary>
    [LocalizeFunction("CreateSubstitutionRequest_ResourceKey", "CreateSubstitutionRequest_DescriptionResourceKey")]
    public virtual void CreateSubstitutionRequest()
    {
      AutomatedSupportTickets.PublicFunctions.Module.Remote.CreateSubstitutionRequest().Show();
    }

    /// <summary>
    /// Создать документ
    /// </summary>
    [LocalizeFunction("CreateDoc_ResourceKey", "CreateDoc_DescriptionResourceKey")]
    public virtual void CreateDoc()
    {
      ApplicationsForDIRECTUM.PublicFunctions.Module.Remote.CreateDoc().Show();
    }
    //конец Добавлено Avis Expert

  }
}