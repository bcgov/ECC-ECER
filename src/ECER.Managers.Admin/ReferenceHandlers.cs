using ECER.Engines.Transformation.References;
using ECER.Managers.Admin.Contract.References;
using MediatR;

namespace ECER.Managers.Admin;

public class ReferenceHandlers(IReferenceLinkTransformationEngine transformationEngine)
  : IRequestHandler<GenerateReferenceLinkCommand, PortalInvitationToLinkResponse>, IRequestHandler<UnpackReferenceLinkCommand, LinkToPortalInvitationResponse>
{
  public async Task<PortalInvitationToLinkResponse> Handle(GenerateReferenceLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    return await transformationEngine.Transform(new PortalInvitationToLinkRequest(request.portalInvitation, request.referenceType))!;
  }

  public async Task<LinkToPortalInvitationResponse> Handle(UnpackReferenceLinkCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(transformationEngine);
    return await transformationEngine.Transform(new LinkToPortalInvitationRequest(request.portalInvitation, request.encryptedVerificationToken))!;
  }
}
