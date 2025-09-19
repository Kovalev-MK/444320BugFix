using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.AutomatedSupportTickets.ApprovalSubstitutionAssignment;

namespace lenspec.AutomatedSupportTickets.Server
{
  partial class ApprovalSubstitutionAssignmentFunctions
  {
    //Добавлено Avis Expert
    /// <summary>
    /// Получить дубли замещения.
    /// </summary>
    /// <returns>Замещения, дублирующие текущее.</returns>
    [Remote(IsPure = true)]
    public IQueryable<Sungero.CoreEntities.ISubstitution> GetDublicatesInSubstitution()
    {
      var dublicate = Sungero.CoreEntities.Substitutions.GetAll().Where(x => _obj.Substitute.Equals(x.Substitute) && 
                                                                        _obj.SubstitutedUser.Equals(x.User) &&
                                                                        x.StartDate != null && _obj.StartDate.Value >= x.StartDate.Value && 
                                                                        x.EndDate != null && _obj.EndDate.Value <= x.EndDate.Value);
      
      return dublicate;
    }
    //конец Добавлено Avis Expert
  }
}