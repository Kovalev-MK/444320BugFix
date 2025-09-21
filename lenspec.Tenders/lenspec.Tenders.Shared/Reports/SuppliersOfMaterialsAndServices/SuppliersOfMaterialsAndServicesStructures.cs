using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Tenders.Structures.SuppliersOfMaterialsAndServices
{

  /// <summary>
  /// 
  /// </summary>
  partial class WorkKindsAndMaterials
  {
    public string ReportSessionId { get; set; } 
    public string Name { get; set; } 
    public string TIN { get; set; } 
    public string TRRC { get; set; } 
    public string PSRN { get; set; } 
    public string Accreditation { get; set; } 
    public string TargetEntity { get; set; } 
    public string EntityGroup { get; set; } 
    public string EntityDetailing { get; set; } 
    public string Region { get; set; } 
    public string City { get; set; }  
  }



}