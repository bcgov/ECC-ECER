using MediatR;

namespace ECER.Managers.Admin.Contract.InviteLinks;

public record GenerateInviteLinkCommand(Guid portalInvitation, InviteType inviteType, int validDays) : IRequest<GenerateInviteLinkCommandResponse>;
public record GenerateInviteLinkCommandResponse(Guid portalInvitation, string verificationLink);

public record VerifyInviteTokenCommand(Guid portalInvitation, string verificationToken) : IRequest<VerifyInviteTokenCommandResponse>;
public record VerifyInviteTokenCommandResponse(Guid portalInvitation, InviteType inviteType);

public enum InviteType
{
  CharacterReference,
  WorkExperienceReference
}
