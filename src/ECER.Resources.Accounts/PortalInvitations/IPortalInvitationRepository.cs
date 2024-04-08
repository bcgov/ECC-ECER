using ECER.Resources.Accounts.Registrants;

namespace ECER.Resources.Accounts.PortalInvitations;

public interface IPortalInvitationRepository
{
  Task<IEnumerable<PortalInvitation>> Query(PortalInvitationQuery query);
}

public record PortalInvitationQuery(Guid portalInvitation);

public record PortalInvitation(string? Id, string name, string firstName, string lastName, string emailAddress)
{
  public Registrant? Registrant { get; set; }
}
