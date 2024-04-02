using ECER.Managers.Admin.Contract.References;

namespace ECER.Engines.Transformation.References;

/// <summary>
/// transforms references links
/// </summary>
public interface IReferenceLinkTransformationEngine
{
  Task<PortalInvitationToLinkResponse> Transform(PortalInvitationToLinkRequest request);

  Task<LinkToPortalInvitationResponse> Transform(LinkToPortalInvitationRequest request);
}
