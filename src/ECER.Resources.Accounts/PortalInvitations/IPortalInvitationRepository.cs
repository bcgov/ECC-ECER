using ECER.Resources.Accounts.Registrants;

namespace ECER.Resources.Accounts.PortalInvitations;

public interface IPortalInvitationRepository
{
  Task<IEnumerable<PortalInvitation>> Query(PortalInvitationQuery query);
}

public record PortalInvitationQuery(Guid portalInvitationId);

public record PortalInvitation(string? Id, string name, string firstName, string lastName, string emailAddress, InviteType inviteType)
{
  public Registrant? Registrant { get; set; }
}

public enum InviteType
{
  CharacterReference,
  WorkExperienceReference
}
