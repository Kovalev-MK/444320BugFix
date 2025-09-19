using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.AccreditationCommitteeProtocol;

namespace lenspec.Tenders.Server
{
  partial class AccreditationCommitteeProtocolFunctions
  {
    
    /// <summary>
    /// Предмет аккредитации.
    /// </summary>
    /// <param name="document">Протокол комитета аккредитации.</param>
    /// <returns>Предмет аккредитации.</returns>
    [Public, Sungero.Core.Converter("GetAccreditationSubject")]
    public static string GetAccreditationSubject(IAccreditationCommitteeProtocol document)
    {
      var result = new List<string>();
      if (document.IsProvider == true)
        result.Add(document.Info.Properties.IsProvider.LocalizedName);
      
      if (document.IsContractor == true)
        result.Add(document.Info.Properties.IsContractor.LocalizedName);
      
      return string.Join(", ", result);
    }
    
    /// <summary>
    /// Детализация видов работ.
    /// </summary>
    /// <param name="document">Протокол комитета аккредитации.</param>
    /// <returns>Детализация видов работ.</returns>
    [Public, Sungero.Core.Converter("GetWorkKinds")]
    public static string GetWorkKinds(IAccreditationCommitteeProtocol document)
    {
      var result = new List<string>();
      if (document.WorkKinds.Any(x => x.WorkKind != null))
        result = document.WorkKinds.Where(x => x.WorkKind != null).Select(x => x.WorkKind.Name).ToList().Distinct().ToList();
      
      return string.Join(", ", result);
    }
    
    /// <summary>
    /// Виды работ для аккредитации.
    /// </summary>
    /// <param name="document">Протокол комитета аккредитации.</param>
    /// <returns>Виды работ для аккредитации.</returns>
    [Public, Sungero.Core.Converter("GetWorkGroups")]
    public static string GetWorkGroups(IAccreditationCommitteeProtocol document)
    {
      var result = new List<string>();
      if (document.WorkKinds.Any(x => x.WorkGroup != null))
        result = document.WorkKinds.Where(x => x.WorkGroup != null).Select(x => x.WorkGroup.Name).ToList().Distinct().ToList();
      
      return string.Join(", ", result);
    }
    
    /// <summary>
    /// Детализация видов материалов.
    /// </summary>
    /// <param name="document">Протокол комитета аккредитации.</param>
    /// <returns>Детализация видов материалов.</returns>
    [Public, Sungero.Core.Converter("GetMaterialKinds")]
    public static string GetMaterialKinds(IAccreditationCommitteeProtocol document)
    {
      var result = new List<string>();
      if (document.MaterialKinds.Any(x => x.Material != null))
        result = document.MaterialKinds.Where(x => x.Material != null).Select(x => x.Material.Name).ToList().Distinct().ToList();
      
      return string.Join(", ", result);
    }
    
    /// <summary>
    /// Виды материалов для аккредитации.
    /// </summary>
    /// <param name="document">Протокол комитета аккредитации.</param>
    /// <returns>Виды материалов для аккредитации.</returns>
    [Public, Sungero.Core.Converter("GetMaterialGroups")]
    public static string GetMaterialGroups(IAccreditationCommitteeProtocol document)
    {
      var result = new List<string>();
      if (document.MaterialKinds.Any(x => x.MaterialGroup != null))
        result = document.MaterialKinds.Where(x => x.MaterialGroup != null).Select(x => x.MaterialGroup.Name).ToList().Distinct().ToList();
      
      return string.Join(", ", result);
    }
    
    /// <summary>
    /// Члены КА.
    /// </summary>
    /// <param name="document">Протокол комитета аккредитации.</param>
    /// <returns>Члены КА.</returns>
    [Public, Sungero.Core.Converter("GetAccreditationCommitteeMembers")]
    public static string GetAccreditationCommitteeMembers(IAccreditationCommitteeProtocol document)
    {
      var result = new List<string>();
      if (document.AccreditationCommittee != null && document.AccreditationCommittee.Members.Any(x => x.Member != null))
        result = document.AccreditationCommittee.Members.Where(x => x.Member != null).Select(x => x.Member.Name).ToList().Distinct().ToList();
      
      return string.Join(", ", result);
    }
  }
}