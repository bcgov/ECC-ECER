using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Managers.Admin.Contract.PortalInvitations;
using MediatR;
using Microsoft.Extensions.Options;
using ECER.Resources.Documents.PortalInvitations;

namespace ECER.Managers.Admin;

public class InviteLinkHandlers(IPortalInvitationTransformationEngine transformationEngine, IOptions<PortalAppSettings> portalAppSettings, IPortalInvitationRepository portalInvitationRepository)
  : IRequestHandler<GenerateInviteLinkCommand, GenerateInviteLinkCommandResponse>
{
  public async Task<GenerateInviteLinkCommandResponse> Handle(GenerateInviteLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var response = await transformationEngine.Transform(new EncryptInviteTokenRequest(request.PortalInvitation, request.ValidDays))! as EncryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    var invitation = await portalInvitationRepository.Query(new PortalInvitationQuery(request.PortalInvitation), cancellationToken);
    var isPsp = invitation.InviteType == InviteType.PSIProgramRepresentative;
    var baseUrl = isPsp && !string.IsNullOrWhiteSpace(portalAppSettings.Value.PspBaseUrl)
      ? portalAppSettings.Value.PspBaseUrl
      : portalAppSettings.Value.BaseUrl;
    string verificationLink = $"{baseUrl}/{portalAppSettings.Value.ReferenceVerificationRoute}/{response.VerificationToken}";
    return new GenerateInviteLinkCommandResponse(response.PortalInvitation, verificationLink);
  }
}
