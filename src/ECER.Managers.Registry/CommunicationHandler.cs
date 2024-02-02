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

namespace ECER.Managers.Registry
{
  public static class CommunicationHandler
  {
    public static async Task<CommunicationsStatusResults> Handle(string UserId, ICommunicationRepository communicationRepository, IMapper mapper)
    {
      ArgumentNullException.ThrowIfNull(communicationRepository);
      ArgumentNullException.ThrowIfNull(mapper);
      ArgumentNullException.ThrowIfNull(UserId);

      var status = await communicationRepository.NotificationStatus(UserId);

      return new CommunicationsStatusResults(mapper.Map<Contract.Communications.CommunicationsStatus>(status));
    }

    public static async Task<CommunicationsQueryResults> Handle(CommunicationsQuery query, ICommunicationRepository communicationRepository, IMapper mapper)
    {
      ArgumentNullException.ThrowIfNull(communicationRepository);
      ArgumentNullException.ThrowIfNull(mapper);
      ArgumentNullException.ThrowIfNull(query);

      var commuminications = await communicationRepository.Query(new CommunicationQuery
      {
        ById = query.ById,
        ByStatus = query.ByStatus?.Convert<Contract.Communications.CommunicationStatus, Resources.Accounts.Communications.CommunicationStatus>(),
      });
      return new CommunicationsQueryResults(mapper.Map<IEnumerable<Contract.Communications.Communication>>(commuminications));
    }
  }
}
