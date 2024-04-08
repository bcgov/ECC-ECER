using ECER.Managers.Registry.Contract.Registrants;

namespace ECER.Managers.Registry.Contract.PortalInvitations;

public record PortalInvitationQuery(Guid portalInvitation);

public record PortalInvitation(string? Id, string name, string firstName, string lastName, string emailAddress)
{
  public Registrant? Registrant { get; set; }
}
