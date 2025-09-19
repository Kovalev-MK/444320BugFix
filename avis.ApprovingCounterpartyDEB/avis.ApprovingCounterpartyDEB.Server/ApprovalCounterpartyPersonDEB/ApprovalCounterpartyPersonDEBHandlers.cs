using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using avis.ApprovingCounterpartyDEB.ApprovalCounterpartyPersonDEB;

namespace avis.ApprovingCounterpartyDEB
{
  partial class ApprovalCounterpartyPersonDEBManagerInitiatorPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ManagerInitiatorFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = query.Where(x => !Equals(x, Sungero.Company.Employees.Current));
      return query;
    }
  }

  partial class ApprovalCounterpartyPersonDEBFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      if (_filter == null)
        return query;
      query = query.Where(x => (_filter.Initiator == null || x.Author == _filter.Initiator) &&
                          (_filter.Counterparty == null || x.Counterparty == _filter.Counterparty) &&
                          (_filter.PeriodFrom == null || x.Created == _filter.PeriodFrom || x.Created > _filter.PeriodFrom) &&
                          (_filter.PeriodTo == null || x.Created == _filter.PeriodTo || x.Created < _filter.PeriodTo));
      return query;
    }
  }

  partial class ApprovalCounterpartyPersonDEBServerHandlers
  {

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      //�������� �������� �����������
      var databook = _obj.ApprovalDatabook.Counterparties.SingleOrDefault();
      var companyBase = lenspec.Etalon.CompanyBases.As(databook);
      if (companyBase != null)
      {
        var headCompany = lenspec.Etalon.Banks.Is(companyBase) ? lenspec.Etalon.CompanyBases.As(companyBase.HeadBankavis) : lenspec.Etalon.CompanyBases.As(companyBase.HeadCompany);
        if (headCompany != null && (headCompany.ResultApprovalDEBavis == null ||
                                    headCompany.ResultApprovalDEBavis == lenspec.Etalon.CompanyBase.ResultApprovalDEBavis.CoopNotRecomend ||
                                    headCompany.ResultApprovalDEBavis == lenspec.Etalon.CompanyBase.ResultApprovalDEBavis.NotAssessed))
        {
          var headLink = Hyperlinks.Get(headCompany);
          e.AddError($"Необходимо направить на проверку в ДБ головную организацию {headLink}. В случае одобрения, филиалы также будут автоматически согласованы.");
          return;
        }
      }
      
      //����� �� ������� ��������
      var document = _obj.ApprovalDocument.ApprovalCounterpartyBases.SingleOrDefault();
      document.AccessRights.Revoke(document.Author, DefaultAccessRightsTypes.FullAccess);
      document.AccessRights.Grant(document.Author, DefaultAccessRightsTypes.Read);
      document.AccessRights.Save();
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      //��������� ������������ �� �������� ������
      var debManagmentRole = Roles.GetAll(x => x.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.DEBManagementGuid).Single();
      var debCoordinationRole = Roles.GetAll(x => x.Sid == lenspec.Etalon.Module.Parties.PublicConstants.Module.DEBCoordinationgGuid).Single();
      foreach (var user in Roles.GetAllUsersInGroup(debManagmentRole))
      {
        if (!_obj.Mandatories.Any(x => Equals(x.Employee, user)))
        {
          var debManagmentLine = _obj.Mandatories.AddNew();
          debManagmentLine.Employee = lenspec.Etalon.Employees.As(user);
        }
      }
      foreach (var user in Roles.GetAllUsersInGroup(debCoordinationRole))
      {
        if (!_obj.Mandatories.Any(x => Equals(x.Employee, user)))
        {
          var debCoordinationLine = _obj.Mandatories.AddNew();
          debCoordinationLine.Employee = lenspec.Etalon.Employees.As(user);
        }
      }
    }
  }

}