using ECER.Engines.Transformation;
using ECER.Engines.Transformation.InviteLinks;
using ECER.Managers.Admin.Contract.InviteLinks;
using MediatR;
using Microsoft.Extensions.Options;

namespace ECER.Managers.Admin;

public class InviteLinkHandlers(IInviteLinkTransformationEngine transformationEngine, IOptions<PortalAppSettings> portalAppSettings)
  : IRequestHandler<GenerateInviteLinkCommand, GenerateInviteLinkCommandResponse>, IRequestHandler<VerifyInviteTokenCommand, VerifyInviteTokenCommandResponse>
{
  public async Task<GenerateInviteLinkCommandResponse> Handle(GenerateInviteLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    var response = await transformationEngine.Transform(new EncryptInviteTokenRequest(request.portalInvitation, request.inviteType, request.validDays))! as EncryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    string verificationLink = $"{portalAppSettings.Value.BaseUrl}/{portalAppSettings.Value.ReferenceVerificationRoute}/{response.verificationToken}";

    return new GenerateInviteLinkCommandResponse(response.portalInvitation, verificationLink);
  }

  public async Task<VerifyInviteTokenCommandResponse> Handle(VerifyInviteTokenCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    var response = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.verificationToken))! as DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    return new VerifyInviteTokenCommandResponse(response.portalInvitation, response.inviteType);
  }
}
