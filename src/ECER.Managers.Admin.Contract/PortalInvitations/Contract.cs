using ECER.Resources.Accounts.Registrants;
using MediatR;

namespace ECER.Managers.Admin.Contract.PortalInvitations;

public record GenerateInviteLinkCommand(Guid portalInvitation, InviteType inviteType, int validDays) : IRequest<GenerateInviteLinkCommandResponse>;
public record GenerateInviteLinkCommandResponse(Guid portalInvitation, string verificationLink);

public record InviteLinkQuery(string verificationToken) : IRequest<InviteLinkQueryResults>;
public record PortalInvitation(string? Id, string name, string firstName, string lastName, string emailAddress, InviteType inviteType)
{
  public Registrant? Registrant { get; set; }
}

public record InviteLinkQueryResults(IEnumerable<PortalInvitation> Items);

public enum InviteType
{
  CharacterReference,
  WorkExperienceReference
}
