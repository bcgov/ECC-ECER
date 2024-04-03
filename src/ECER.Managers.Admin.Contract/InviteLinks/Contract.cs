using MediatR;

namespace ECER.Managers.Admin.Contract.InviteLinks;

public record GenerateInviteLinkCommand(Guid portalInvitation, InviteType inviteType, int validDays) : IRequest<GenerateInviteLinkCommandResponse>;
public record VerifyInviteLinkCommand(Guid portalInvitation, string encryptedVerificationToken) : IRequest<VerifyInviteLinkCommandResponse>;
public record GenerateInviteLinkCommandResponse(Guid portalInvitation, string encryptedVerificationToken);
public record VerifyInviteLinkCommandResponse(Guid portalInvitation, InviteType inviteType);

public enum InviteType
{
  CharacterReference,
  WorkExperienceReference
}
