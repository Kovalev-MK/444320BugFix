using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Structures.Docflow.PowerOfAttorney
{

  /// <summary>
  /// Обновляемые поля.
  /// </summary>
  [Public]
  partial class UpdatableFields
  {
    public Sungero.Core.Enumeration? AgentType { get; set; }
    /// Сумма.
    public double? Amount { get; set; }
    /// Наши организации.
    public List<Sungero.Company.IBusinessUnit> BusinessUnits { get; set; }
    /// Категории договора.
    public List<Sungero.Contracts.IContractCategory> ContractCategories { get; set; }
    /// Виды документов.
    public List<Sungero.Docflow.IDocumentKind> DocumentKinds { get; set; }
    /// Кому (сотрудник).
    public Sungero.Company.IEmployee IssuedTo { get; set; }
    /// Кому (контрагент).
    public Sungero.Parties.ICounterparty IssuedToParty { get; set; }
    /// Действует с.
    public DateTime ValidFrom { get; set; }
    /// Действует по.
    public DateTime ValidTill { get; set; }
  }

}