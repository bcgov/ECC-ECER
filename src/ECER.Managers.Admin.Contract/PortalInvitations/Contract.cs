using MediatR;

namespace ECER.Managers.Admin.Contract.PortalInvitations;

public record GenerateInviteLinkCommand(Guid PortalInvitation, InviteType InviteType, int ValidDays) : IRequest<GenerateInviteLinkCommandResponse>;
public record GenerateInviteLinkCommandResponse(Guid PortalInvitation, string VerificationLink);

public enum InviteType
{
  CharacterReference,
  WorkExperienceReference
}
