using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Docflow.Shared
{
  partial class ModuleFunctions
  {

    //��������� Avis Expert
    /// <summary>
    /// �������� ���������, ��������� ����� ����� "����������".
    /// </summary>
    /// <param name="document">��������.</param>
    /// <returns>���������, ��������� ����� ����� "����������".</returns>
    /// <remarks>���������� ������ �� ���������� ���������.</remarks>
    [Public]
    public override List<Sungero.Content.IElectronicDocument> GetAddenda(Sungero.Content.IElectronicDocument document)
    {
      if (Sungero.RecordManagement.Orders.Is(document))
      {
        return lenspec.LocalRegulations.DocumentApprovedByOrders.GetAll(x => Equals(document, x.LeadingDocument)).ToList<Sungero.Content.IElectronicDocument>();
      }
      else
      {
        return base.GetAddenda(document);
      }
    }
    //����� ��������� Avis Expert
  }
}