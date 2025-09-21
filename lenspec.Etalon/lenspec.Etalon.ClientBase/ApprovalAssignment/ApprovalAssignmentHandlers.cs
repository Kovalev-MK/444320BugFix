using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.ApprovalAssignment;

namespace lenspec.Etalon
{
  partial class ApprovalAssignmentClientHandlers
  {

    //Добавлено Avis Expert
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        //TODO: без учета коробки?
        // Если согласуемый документ - СЗ, скрыты результат выполнения Переадресовать и поле Переадресовать сотруднику.
        _obj.State.Properties.Addressee.IsVisible = !Sungero.Docflow.Memos.Is(document);
        
        // Если согл. документ - Электронная доверенность, скрыты группы вложений приложения и дополнительно
        _obj.State.Attachments.AddendaGroup.IsVisible = !Sungero.Docflow.FormalizedPowerOfAttorneys.Is(document);
        _obj.State.Attachments.OtherGroup.IsVisible = !Sungero.Docflow.FormalizedPowerOfAttorneys.Is(document);
      }
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      base.Showing(e);
      
      #region [Запись параметров]
      
      // Признак вхождения в роль "Ответственный юрист для доверенностей."
      var roleLawyer = Roles.GetAll(x => x.Sid == avis.PowerOfAttorneyModule.PublicConstants.Module.RoleGuidResponsibleLawyer).SingleOrDefault();
      var isInRoleLawyer = _obj.Performer.IncludedIn(roleLawyer);
      e.Params.AddOrUpdate(Constants.Docflow.ApprovalAssignment.Params.IsInRoleLawyer, isInRoleLawyer);
      
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null)
      {
        // Признак вида документа "Заявка на оформление доверенности".
        var requestToCreatePowerOfAttorneyKind = Sungero.Docflow.PublicFunctions.DocumentKind
          .GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreatePowerOfAttorneyKindGuid);
        e.Params.AddOrUpdate(
          Constants.Docflow.ApprovalAssignment.Params.IsRequestToCreatePowerOfAttorneyKind,
          Equals(requestToCreatePowerOfAttorneyKind, document.DocumentKind)
         );
        
        // Признак вида документа "Заявка на оформление нотариальной доверенности".
        var requestToCreateNotarialPowerOfAttorneyKind = Sungero.Docflow.PublicFunctions.DocumentKind
          .GetNativeDocumentKind(avis.PowerOfAttorneyModule.PublicConstants.Module.RequestToCreateNotarialPowerOfAttorneyKindGuid);
        e.Params.AddOrUpdate(
          Constants.Docflow.ApprovalAssignment.Params.IsRequestToCreateNotarialPowerOfAttorneyKind,
          Equals(requestToCreateNotarialPowerOfAttorneyKind, document.DocumentKind)
         );
      }
      
      #endregion [Запись параметров]
      
      e.Instruction = EtalonDatabooks.PublicFunctions.Module.GetAssignmentInstruction(_obj.Stage);
    }
    //конец Добавлено Avis Expert

  }
}