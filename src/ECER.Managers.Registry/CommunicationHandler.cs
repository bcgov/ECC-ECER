using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Communications;
using ECER.Resources.Accounts.Communications;
using MediatR;

namespace ECER.Managers.Registry
{
  public class CommunicationHandler(ICommunicationRepository communicationRepository, IMapper mapper)
    : IRequestHandler<Contract.Communications.UserCommunicationsStatusQuery, CommunicationsStatusResults>,
      IRequestHandler<Contract.Communications.UserCommunicationQuery, CommunicationsQueryResults>
  {
    public async Task<CommunicationsStatusResults> Handle(UserCommunicationsStatusQuery request, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(communicationRepository);
      ArgumentNullException.ThrowIfNull(mapper);
      ArgumentNullException.ThrowIfNull(request);

      var statuses = new List<Resources.Accounts.Communications.CommunicationStatus>();
      statuses.Add(Resources.Accounts.Communications.CommunicationStatus.NotifiedRecipient);
      var communications = await communicationRepository.Query(new Resources.Accounts.Communications.UserCommunicationQuery
      {
        ByRegistrantId = request.ByRegistrantId,
        ByStatus = statuses,
      });
      
      var unreadCount = communications.Where(c => !c.Acknowledged).ToList().Count; // it does not support Any
      var hasUnread = unreadCount > 0;

      var communicationsStatus = new Contract.Communications.CommunicationsStatus() { HasUnread = hasUnread, Count = unreadCount };
      return new CommunicationsStatusResults(communicationsStatus!);
    }

    public async Task<CommunicationsQueryResults> Handle(Contract.Communications.UserCommunicationQuery request, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(communicationRepository);
      ArgumentNullException.ThrowIfNull(mapper);
      ArgumentNullException.ThrowIfNull(request);

      var communications = await communicationRepository.Query(new Resources.Accounts.Communications.UserCommunicationQuery
      {
        ById = request.ById,
        ByRegistrantId = request.ByRegistrantId,
        ByStatus = request.ByStatus?.Convert<Contract.Communications.CommunicationStatus, Resources.Accounts.Communications.CommunicationStatus>(),
      });
      return new CommunicationsQueryResults(mapper.Map<IEnumerable<Contract.Communications.Communication>>(communications)!);
    }
  }
}
