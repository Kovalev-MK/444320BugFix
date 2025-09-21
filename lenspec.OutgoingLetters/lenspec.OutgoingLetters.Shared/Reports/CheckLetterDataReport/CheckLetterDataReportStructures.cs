using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.OutgoingLetters.Structures.CheckLetterDataReport
{
  //Добавлено Avis Expert
  /// <summary>
  /// Строка отчета.
  /// </summary>
  partial class TableLine
  {
    public string ReportSessionId { get; set; }
    
    public string AddresseeFIO { get; set; }
    
    public string ContractNumber { get; set; }
    
    public string ContractDate { get; set; }
    
    public string ObjectAnProject { get; set; }
    
    public string NameRNV { get; set; }
    
    public string AddressRNV { get; set; }
    
    public double? ReductionSizeS { get; set; }
    
    public double? ZoomSizeS { get; set; }
    
    public string AmountOfSurchargeInWords { get; set; }
    
    public string AmountOfRefundInWords { get; set; }
    
    public string DateRNV { get; set; }
    
    public string NumberRNV { get; set; }
    
    public string BusinessUnit { get; set; }
    
    public string RepresentativeByPowerOfAttorneyNumber { get; set; }
    
    public string PowerOfAttorneyDate { get; set; }
    
    public string DeliveryMethod { get; set; }
    
    public string Email { get; set; }
    
    public string RegistrationDate { get; set; }
    
    public string RegistrationNumber { get; set; }
    
    public string LegalAddress { get; set; }
    
    public string PostalAddress { get; set; }
    
    public string PSRN { get; set; }
    
    public string TIN { get; set; }
    
    public string TRRC { get; set; }
    
    public string SettlementAccount { get; set; }
    
    public string Bank { get; set; }
    
    public string CorrespondentAccount { get; set; }
    
    public string BIC { get; set; }
    
    public string OurSignatoryJobTitle { get; set; }
    
    public string OurSignatoryFIO { get; set; }
    
    public string InitiatorFIO { get; set; }
    
    public string Phone { get; set; }
    
    public string CorrespondentPostalAddress { get; set; }
  }
    //конец Добавлено Avis Expert
}