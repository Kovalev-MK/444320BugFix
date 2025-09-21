using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Order;

namespace lenspec.Etalon.Shared
{
  partial class OrderFunctions
  {
    
    /// <summary>
    /// �������� ���������, ��������� ����� ����� "����������".
    /// </summary>
    /// <returns>���������, ��������� ����� ����� "����������".</returns>
    [Public]
    public override List<Sungero.Docflow.IOfficialDocument> GetAddenda()
    {
      return lenspec.LocalRegulations.DocumentApprovedByOrders.GetAll(x => Equals(_obj, x.LeadingDocument)).ToList<Sungero.Docflow.IOfficialDocument>();
    }

  }
}