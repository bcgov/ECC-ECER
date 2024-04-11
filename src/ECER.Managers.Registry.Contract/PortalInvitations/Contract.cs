using MediatR;

namespace ECER.Managers.Registry.Contract.PortalInvitations;

public record PortalInvitationVerificationQuery(string VerificationToken) : IRequest<PortalInvitationVerificationQueryResult>;
public record PortalInvitationVerificationQueryResult(PortalInvitation portalInvitation);

public record PortalInvitation(string? Id, string Name, string ReferenceFirstName, string ReferenceLastName, string ReferenceEmailAddress)
{
  public string? ApplicantId { get; set; }
  public string? ApplicantFirstName { get; set; }
  public string? ApplicantLastName { get; set; }
  public string? ApplicationId { get; set; }
  public string? WorkexperienceReferenceId { get; set; }
  public string? CharacterReferenceId { get; set; }
  public InviteType? InviteType { get; set; }
}

public enum InviteType
{
  CharacterReference,
  WorkExperienceReference
}
