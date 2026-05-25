using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Resources.Documents.PortalInvitations;
using Mediator;

namespace ECER.Managers.Registry.PortalInvitations;

public class PortalInvitationHandlers(
  IPortalInvitationTransformationEngine transformationEngine,
  IPortalInvitationRepository portalInvitationRepository,
  IPortalInvitationMapper portalInvitationMapper,
  IEnumerable<IPortalInvitationVerificationHandler> verificationHandlers)
  : IRequestHandler<PortalInvitationVerificationQuery, PortalInvitationVerificationQueryResult>
{
  public async ValueTask<PortalInvitationVerificationQueryResult> Handle(PortalInvitationVerificationQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var response = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.VerificationToken))! as DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    if (response.PortalInvitation == Guid.Empty)
    {
      return PortalInvitationVerificationQueryResult.Failure("Invalid Token");
    }

    var portalInvitation = await portalInvitationRepository.Query(new PortalInvitationQuery(response.PortalInvitation), cancellationToken);
    if (portalInvitation == null)
    {
      return PortalInvitationVerificationQueryResult.Failure("Portal Invitation not found");
    }

    if (portalInvitation.StatusCode != Resources.Documents.PortalInvitations.PortalInvitationStatusCode.Sent)
    {
      return PortalInvitationVerificationQueryResult.Failure("Portal Invitation Wrong Status");
    }

    var mappedPortalInvitation = portalInvitationMapper.MapPortalInvitation(portalInvitation);
    var handler = verificationHandlers.FirstOrDefault(h => h.CanHandle(mappedPortalInvitation.InviteType));
    if (handler == null)
    {
      return PortalInvitationVerificationQueryResult.Failure("Unsupported invite type.");
    }

    return await handler.Verify(mappedPortalInvitation, cancellationToken);
  }
}
