using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Managers.Admin.Contract.PortalInvitations;
using MediatR;
using Microsoft.Extensions.Options;

namespace ECER.Managers.Admin;

public class InviteLinkHandlers(IPortalInvitationTransformationEngine transformationEngine, IOptions<PortalAppSettings> portalAppSettings)
  : IRequestHandler<GenerateInviteLinkCommand, GenerateInviteLinkCommandResponse>
{
  public async Task<GenerateInviteLinkCommandResponse> Handle(GenerateInviteLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    var response = await transformationEngine.Transform(new EncryptInviteTokenRequest(request.PortalInvitation, request.InviteType, request.ValidDays))! as EncryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    string verificationLink = $"{portalAppSettings.Value.BaseUrl}/{portalAppSettings.Value.ReferenceVerificationRoute}/{response.VerificationToken}";
    return new GenerateInviteLinkCommandResponse(response.PortalInvitation, verificationLink);
  }
}
