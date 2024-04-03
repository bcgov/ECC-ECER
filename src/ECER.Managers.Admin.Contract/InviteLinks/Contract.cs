using MediatR;

namespace ECER.Managers.Admin.Contract.InviteLinks;

public record ManageInviteLinkCommand();
public record ManageInviteLinkCommandResponse();

public record GenerateInviteLinkCommand(Guid portalInvitation, InviteType inviteType, int validDays) : ManageInviteLinkCommand, IRequest<GenerateInviteLinkCommandResponse>;
public record GenerateInviteLinkCommandResponse(Guid portalInvitation, string encryptedVerificationToken) : ManageInviteLinkCommandResponse;

public record VerifyInviteLinkCommand(Guid portalInvitation, string encryptedVerificationToken) : ManageInviteLinkCommand, IRequest<VerifyInviteLinkCommandResponse>;
public record VerifyInviteLinkCommandResponse(Guid portalInvitation, InviteType inviteType) : ManageInviteLinkCommandResponse;

public enum InviteType
{
  CharacterReference,
  WorkExperienceReference
}
