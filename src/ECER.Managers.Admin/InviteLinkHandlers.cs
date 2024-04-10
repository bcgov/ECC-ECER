using AutoMapper;
using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Managers.Admin.Contract.PortalInvitations;
using ECER.Resources.Accounts.PortalInvitations;
using MediatR;
using Microsoft.Extensions.Options;

namespace ECER.Managers.Admin;

public class InviteLinkHandlers(IPortalInvitationTransformationEngine transformationEngine, IOptions<PortalAppSettings> portalAppSettings, IPortalInvitationRepository portalInvitationRepository, IMapper mapper)
  : IRequestHandler<GenerateInviteLinkCommand, GenerateInviteLinkCommandResponse>, IRequestHandler<InviteLinkQuery, InviteLinkQueryResults>
{
  public async Task<GenerateInviteLinkCommandResponse> Handle(GenerateInviteLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    var response = await transformationEngine.Transform(new EncryptInviteTokenRequest(request.portalInvitation, request.inviteType, request.validDays))! as EncryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    string verificationLink = $"{portalAppSettings.Value.BaseUrl}/{portalAppSettings.Value.ReferenceVerificationRoute}/{response.verificationToken}";
    return new GenerateInviteLinkCommandResponse(response.portalInvitation, verificationLink);
  }

  public async Task<InviteLinkQueryResults> Handle(InviteLinkQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    var response = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.verificationToken))! as DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    if (response.portalInvitation == Guid.Empty) throw new InvalidOperationException("Invalid token");

    var portalInvitations = await portalInvitationRepository.Query(new PortalInvitationQuery(response.portalInvitation));
    return new InviteLinkQueryResults(mapper.Map<IEnumerable<Contract.PortalInvitations.PortalInvitation>>(portalInvitations)!);
  }
}
