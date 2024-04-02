using ECER.Engines.Transformation.InviteLinks;
using ECER.Managers.Admin.Contract.InviteLinks;
using MediatR;

namespace ECER.Managers.Admin;

public class InviteLinkHandlers(IInviteLinkTransformationEngine transformationEngine)
  : IRequestHandler<GenerateInviteLinkCommand, PortalInvitationToLinkResponse>, IRequestHandler<VerifyInviteLinkCommand, LinkToPortalInvitationResponse>
{
  public async Task<PortalInvitationToLinkResponse> Handle(GenerateInviteLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    return await transformationEngine.Transform(new PortalInvitationToLinkRequest(request.portalInvitation, request.inviteType))!;
  }

  public async Task<LinkToPortalInvitationResponse> Handle(VerifyInviteLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    return await transformationEngine.Transform(new LinkToPortalInvitationRequest(request.portalInvitation, request.encryptedVerificationToken))!;
  }
}
