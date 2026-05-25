using ECER.Managers.Registry.Contract.PortalInvitations;

namespace ECER.Managers.Registry;

public class PspPortalInvitationVerificationHandler : IPortalInvitationVerificationHandler
{
  public bool CanHandle(InviteType? inviteType)
  {
    return inviteType == InviteType.PSIProgramRepresentative;
  }

  public Task<PortalInvitationVerificationQueryResult> Verify(PortalInvitation portalInvitation, CancellationToken cancellationToken)
  {
    return Task.FromResult(PortalInvitationVerificationQueryResult.Success(portalInvitation));
  }
}
