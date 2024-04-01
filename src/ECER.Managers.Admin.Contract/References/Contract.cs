using MediatR;

namespace ECER.Managers.Admin.Contract.References;

public record GenerateReferenceLinkCommand(Guid portalInvitation, ReferenceType referenceType) : IRequest<PortalInvitationToLinkResponse>;
public record UnpackReferenceLinkCommand(Guid portalInvitation, string referenceLink) : IRequest<LinkToPortalInvitationResponse>;
public record PortalInvitationToLinkRequest(Guid portalInvitation, ReferenceType referenceType);
public record PortalInvitationToLinkResponse(Guid portalInvitation, string referenceLink);
public record LinkToPortalInvitationRequest(Guid portalInvitation, string referenceLink);
public record LinkToPortalInvitationResponse(Guid portalInvitation, ReferenceType referenceType);

public enum ReferenceType
{
  CharacterReference,
  WorkExperienceReference
}
