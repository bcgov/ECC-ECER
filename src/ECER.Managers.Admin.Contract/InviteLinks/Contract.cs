using MediatR;

namespace ECER.Managers.Admin.Contract.InviteLinks;

public record GenerateInviteLinkCommand(Guid portalInvitation, InviteType inviteType) : IRequest<PortalInvitationToLinkResponse>;
public record VerifyInviteLinkCommand(Guid portalInvitation, string encryptedVerificationToken) : IRequest<LinkToPortalInvitationResponse>;
public record PortalInvitationToLinkRequest(Guid portalInvitation, InviteType inviteType);
public record PortalInvitationToLinkResponse(Guid portalInvitation, string encryptedVerificationToken);
public record LinkToPortalInvitationRequest(Guid portalInvitation, string encryptedVerificationToken);
public record LinkToPortalInvitationResponse(Guid portalInvitation, InviteType inviteType);

public enum InviteType
{
  CharacterReference,
  WorkExperienceReference
}
