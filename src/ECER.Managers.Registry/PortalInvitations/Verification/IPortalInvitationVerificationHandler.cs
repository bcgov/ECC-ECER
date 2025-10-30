using ECER.Managers.Registry.Contract.PortalInvitations;

namespace ECER.Managers.Registry;

public interface IPortalInvitationVerificationHandler
{
  bool CanHandle(InviteType? inviteType);
  Task<PortalInvitationVerificationQueryResult> Verify(PortalInvitation portalInvitation, CancellationToken cancellationToken);
}


