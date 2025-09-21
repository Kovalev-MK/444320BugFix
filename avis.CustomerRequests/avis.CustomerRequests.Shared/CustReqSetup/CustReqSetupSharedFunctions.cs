using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.CustomerRequests.CustReqSetup;

namespace avis.CustomerRequests.Shared
{
  partial class CustReqSetupFunctions
  {

    //��������� Avis Expert
    /// <summary>
    /// ��������� ����� ���������.
    /// </summary>
    /// <returns>True, ���� ��������� �������, ����� - false.</returns>
    public bool HaveDuplicates()
    {
      return Functions.CustReqSetup.Remote.GetDuplicates(_obj).Any();
    }
    //����� ��������� Avis Expert
  }
}