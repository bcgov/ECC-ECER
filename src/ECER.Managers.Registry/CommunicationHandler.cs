using AutoMapper;
using ECER.Managers.Registry.Contract.Communications;
using ECER.Resources.Accounts.Communications;
using ECER.Resources.Documents.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECER.Infrastructure.Common;
using System.Collections;

namespace ECER.Managers.Registry
{
  public static class CommunicationHandler
  {
    public static async Task<CommunicationsStatusResults> Handle(Contract.Communications.UserCommunicationsStatusQuery query, ICommunicationRepository communicationRepository, IMapper mapper)
    {
      ArgumentNullException.ThrowIfNull(communicationRepository);
      ArgumentNullException.ThrowIfNull(mapper);
      ArgumentNullException.ThrowIfNull(query);

      var statuses = new List<Resources.Accounts.Communications.CommunicationStatus>();
      statuses.Add(Resources.Accounts.Communications.CommunicationStatus.NotifiedRecipient);
      var communications = await communicationRepository.Query(new Resources.Accounts.Communications.UserCommunicationQuery
      {
        ByRegistrantId = query.ByRegistrantId,
        ByStatus = statuses,
      });

      var unreadCount = communications.Where(c => c.Acknowledged == false).ToList().Count; // it does not support Any
      var hasUnread = unreadCount > 0;

      var communicationsStatus = new Contract.Communications.CommunicationsStatus() { HasUnread = hasUnread, Count = unreadCount };
      return new CommunicationsStatusResults(communicationsStatus!);
    }

    public static async Task<CommunicationsQueryResults> Handle(Contract.Communications.UserCommunicationQuery query, ICommunicationRepository communicationRepository, IMapper mapper)
    {
      ArgumentNullException.ThrowIfNull(communicationRepository);
      ArgumentNullException.ThrowIfNull(mapper);
      ArgumentNullException.ThrowIfNull(query);

      var communications = await communicationRepository.Query(new Resources.Accounts.Communications.UserCommunicationQuery
      {
        ById = query.ById,
        ByRegistrantId = query.ByRegistrantId,
        ByStatus = query.ByStatus?.Convert<Contract.Communications.CommunicationStatus, Resources.Accounts.Communications.CommunicationStatus>(),
      });
      return new CommunicationsQueryResults(mapper.Map<IEnumerable<Contract.Communications.Communication>>(communications)!);
    }
  }
}
