using ECER.Managers.Registry.Contract.Registrants;
using MediatR;

namespace ECER.Managers.Registry.Contract.PortalInvitations;

public record PortalInvitationQuery(Guid portalInvitationId) : IRequest<PortalInvitationQueryResults>;

public record PortalInvitation(string? Id, string name, string firstName, string lastName, string emailAddress)
{
  public Registrant? Registrant { get; set; }
}

public record PortalInvitationQueryResults(IEnumerable<PortalInvitation> Items);
