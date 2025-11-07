using AutoMapper;
using ECER.Managers.Registry.Contract.PortalInvitations;

namespace ECER.Managers.Registry;

public class PspPortalInvitationVerificationHandler(IMapper mapper) : IPortalInvitationVerificationHandler
{
  public bool CanHandle(InviteType? inviteType)
  {
    return inviteType == InviteType.PSIProgramRepresentative;
  }

  public Task<PortalInvitationVerificationQueryResult> Verify(PortalInvitation portalInvitation, CancellationToken cancellationToken)
  {
    var result = mapper.Map<Contract.PortalInvitations.PortalInvitation>(portalInvitation);
    return Task.FromResult(PortalInvitationVerificationQueryResult.Success(result));
  }
}


