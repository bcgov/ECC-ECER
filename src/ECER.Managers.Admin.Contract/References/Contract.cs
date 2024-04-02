using MediatR;

namespace ECER.Managers.Admin.Contract.References;

public record GenerateReferenceLinkCommand(Guid portalInvitation, ReferenceType referenceType) : IRequest<PortalInvitationToLinkResponse>;
public record UnpackReferenceLinkCommand(Guid portalInvitation, string encryptedVerificationToken) : IRequest<LinkToPortalInvitationResponse>;
public record PortalInvitationToLinkRequest(Guid portalInvitation, ReferenceType referenceType);
public record PortalInvitationToLinkResponse(Guid portalInvitation, string encryptedVerificationToken);
public record LinkToPortalInvitationRequest(Guid portalInvitation, string encryptedVerificationToken);
public record LinkToPortalInvitationResponse(Guid portalInvitation, ReferenceType referenceType);

public enum ReferenceType
{
  CharacterReference,
  WorkExperienceReference
}
