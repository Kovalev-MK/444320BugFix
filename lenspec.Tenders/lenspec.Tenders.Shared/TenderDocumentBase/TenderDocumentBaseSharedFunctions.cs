using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderDocumentBase;

namespace lenspec.Tenders.Shared
{
  partial class TenderDocumentBaseFunctions
  {

    // добавлено Avis Expert
    
    /// <summary>
    /// Обновить поля оргструктуры.
    /// </summary>
    public void UpdateOrganizationStructure()
    {
      _obj.PreparedBy = null;
      _obj.Department = null;
      
      if (!TenderDocuments.Is(_obj))
      {
        _obj.BusinessUnit = null;
        return;
      }
      
      var decisionOnInclusionOfCounterpartyKindGuid = Constants.Module.DecisionOnInclusionOfCounterpartyKind;
      var decisionOnExclusionOfCounterpartyKindGuid = Constants.Module.DecisionOnExclusionOfCounterpartyKind;
      var memoOnInclusionOfCounterpartyKindGuid =     Constants.Module.MemoOnInclusionOfCounterpartyKind;
      var memoOnExclusionOfCounterpartyKindGuid =     Constants.Module.MemoOnExclusionOfCounterpartyKind;
      
      // Для следующих видов тендерной документации не очищаем поле НОР.
      var decisionOnInclusionOfCounterpartyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(decisionOnInclusionOfCounterpartyKindGuid);
      var decisionOnExclusionOfCounterpartyKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(decisionOnExclusionOfCounterpartyKindGuid);
      var memoOnInclusionOfCounterpartyKind =     Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(memoOnInclusionOfCounterpartyKindGuid);
      var memoOnExclusionOfCounterpartyKind =     Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(memoOnExclusionOfCounterpartyKindGuid);
      
      var keepBusinessUnit =
        Equals(_obj.DocumentKind, decisionOnInclusionOfCounterpartyKind) ||
        Equals(_obj.DocumentKind, decisionOnExclusionOfCounterpartyKind) ||
        Equals(_obj.DocumentKind, memoOnInclusionOfCounterpartyKind) ||
        Equals(_obj.DocumentKind, memoOnExclusionOfCounterpartyKind);
      
      if (!keepBusinessUnit)
        _obj.BusinessUnit = null;
    }

    /// <summary>
    /// Заполнить имя документа.
    /// </summary>
    [Public]
    public override void FillName()
    {
      var documentKind = _obj.DocumentKind;
      
      if (documentKind != null && !documentKind.GenerateDocumentName.Value && _obj.Name == Sungero.Docflow.Resources.DocumentNameAutotext)
        _obj.Name = string.Empty;
      
      if (documentKind == null || !documentKind.GenerateDocumentName.Value)
        return;
      
      var name = string.Empty;
      
      /* ��� � �������:
        <��� ���������> �<���.�����> �� <���� ���������> ����� <���� ���.> � <�����������, ����� �������> �� <���, ����� �������>. <������� ���������� ������>
       */
      using (TenantInfo.Culture.SwitchTo())
      {
        if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
          name += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
        if (_obj.RegistrationDate != null)
          name += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.RegistrationDate.Value.ToString("d");
        if (_obj.BusinessUnit != null && _obj.Counterparties.Any(x => x.Counterparty != null))
        {
          name += lenspec.Tenders.TenderDocumentBases.Resources.BetweenPartOfDocName + _obj.BusinessUnit.Name +
            lenspec.Tenders.TenderDocumentBases.Resources.AndPartOfDocName + string.Join(", ", _obj.Counterparties.Where(x => x.Counterparty != null).Select(x => x.Counterparty.Name).ToList());
        }
        if (_obj.OurCF.Any(x => x.OurCF != null))
          name += lenspec.Tenders.TenderDocumentBases.Resources.ByPartOfDocName + string.Join(", ", _obj.OurCF.Where(x => x.OurCF != null).Select(x => x.OurCF.CommercialName).ToList());
        if (!string.IsNullOrEmpty(_obj.TenderSelectionSubject))
          name += lenspec.Tenders.TenderDocumentBases.Resources.DotPartOfDocName + _obj.TenderSelectionSubject;
      }
      
      if (string.IsNullOrWhiteSpace(name))
      {
        if (_obj.VerificationState == null)
          name = Sungero.Docflow.Resources.DocumentNameAutotext;
        else
          name = _obj.DocumentKind.ShortName;
      }
      else if (documentKind != null)
      {
        name = documentKind.ShortName + name;
      }
      name = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(name);
      if (name.Length > _obj.Info.Properties.Name.Length)
        _obj.Name = name.Substring(0, _obj.Info.Properties.Name.Length);
      else
        _obj.Name = name;
    }
    
    /// <summary>
    /// ���������� �������������� ������� � ����������� �� ����������� ������.
    /// </summary>
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      _obj.State.Properties.Department.IsRequired = false;
      _obj.State.Properties.Subject.IsRequired = false;
    }
    //����� ��������� Avis Expert
  }
}