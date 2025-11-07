using AutoMapper;
using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Resources.Documents.PortalInvitations;
using MediatR;

namespace ECER.Managers.Registry.PortalInvitations;

public class PortalInvitationHandlers(
  IPortalInvitationTransformationEngine transformationEngine,
  IPortalInvitationRepository portalInvitationRepository,
  IMapper mapper,
  IEnumerable<IPortalInvitationVerificationHandler> verificationHandlers)
  : IRequestHandler<PortalInvitationVerificationQuery, PortalInvitationVerificationQueryResult>
{
  public async Task<PortalInvitationVerificationQueryResult> Handle(PortalInvitationVerificationQuery request, CancellationToken cancellationToken)
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
    var portalInvitation_Manager = mapper.Map<Contract.PortalInvitations.PortalInvitation>(portalInvitation);
    var handler = verificationHandlers.FirstOrDefault(h => h.CanHandle(portalInvitation_Manager.InviteType));
    if (handler == null)
    {
      return PortalInvitationVerificationQueryResult.Failure("Unsupported invite type.");
    }

    return await handler.Verify(portalInvitation_Manager, cancellationToken);
  }
}
