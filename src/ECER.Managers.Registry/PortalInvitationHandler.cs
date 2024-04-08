using AutoMapper;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Resources.Accounts.PortalInvitations;

namespace ECER.Managers.Registry;

public class PortalInvitationHandler(IPortalInvitationRepository portalInvitationRepository, IMapper mapper)
{
  public async Task<PortalInvitationQueryResults> Handle(Contract.PortalInvitations.PortalInvitationQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(portalInvitationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    var portalInvitations = await portalInvitationRepository.Query(new Resources.Accounts.PortalInvitations.PortalInvitationQuery(request.portalInvitationId));
    return new PortalInvitationQueryResults(mapper.Map<IEnumerable<Contract.PortalInvitations.PortalInvitation>>(portalInvitations)!);
  }
}
