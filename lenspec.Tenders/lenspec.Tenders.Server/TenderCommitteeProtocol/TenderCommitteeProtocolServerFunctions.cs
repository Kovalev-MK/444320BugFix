using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.TenderCommitteeProtocol;

namespace lenspec.Tenders.Server
{
  partial class TenderCommitteeProtocolFunctions
  {

    //��������� Avis Expert
    /// <summary>
    /// �������� ������ ������������ ��� ��� ������� ��������� ���������� ���������.
    /// </summary>
    /// <param name="document">�������� ���������� ���������.</param>
    /// <returns>������ ������������ ���.</returns>
    [Public, Sungero.Core.Converter("GetOurCFNames")]
    public static string GetOurCFNames(Tenders.ITenderCommitteeProtocol document)
    {
      var result = string.Empty;
      if (document.OurCF.Any(x => x.OurCF != null))
      {
        result = string.Join(", ", document.OurCF.Where(x => x.OurCF != null).Select(x => x.OurCF.CommercialName).ToList());
      }
      return result.Trim();
    }
    
    /// <summary>
    /// �������� ������ ������������ �������� ������������� ��� ������� ��������� ���������� ���������.
    /// </summary>
    /// <param name="document">�������� ���������� ���������.</param>
    /// <returns>������ ������������ �������������.</returns>
    [Public, Sungero.Core.Converter("GetObjectAnProjectNames")]
    public static string GetObjectAnProjectNames(lenspec.Tenders.ITenderCommitteeProtocol document)
    {
      var result = string.Empty;
      if (document.ObjectAnProjects.Any(x => x.ObjectAnProject != null))
      {
        result = string.Join(", ", document.ObjectAnProjects.Where(x => x.ObjectAnProject != null).Select(x => x.ObjectAnProject.Name).ToList());
      }
      return result.Trim();
    }
    
    /// <summary>
    /// �������� ������ ������ ���������� ��������� ��� ������� ��������� ���������� ���������.
    /// </summary>
    /// <param name="document">�������� ���������� ���������.</param>
    /// <returns>������ ������ ���������� ���������.</returns>
    [Public, Sungero.Core.Converter("GetTenderCommitteeMembersNames")]
    public static string GetTenderCommitteeMembersNames(lenspec.Tenders.ITenderCommitteeProtocol document)
    {
      var result = string.Empty;
      if (document.TenderCommittee != null && document.TenderCommittee.Members.Any(x => x.Member != null))
      {
        result = string.Join(", ", document.TenderCommittee.Members.Where(x => x.Member != null).Select(x => x.Member.Name).ToList());
      }
      return result.Trim();
    }
    //����� ��������� Avis Expert
  }
}