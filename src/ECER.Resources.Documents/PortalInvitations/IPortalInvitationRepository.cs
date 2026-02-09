namespace ECER.Resources.Documents.PortalInvitations;

public interface IPortalInvitationRepository
{
  Task<PortalInvitation> Query(PortalInvitationQuery query, CancellationToken ct);

  Task Complete(CompletePortalInvitationCommand command, CancellationToken ct);
}

public record PortalInvitationQuery(Guid portalInvitationId);
public record CompletePortalInvitationCommand(Guid portalInvitationId);

public record PortalInvitation(string? Id, string Name, string ReferenceFirstName, string ReferenceLastName, string ReferenceEmailAddress)
{
  public string? ApplicantId { get; set; }
  public string? ApplicationId { get; set; }
  public string? WorkexperienceReferenceId { get; set; }
  public string? CharacterReferenceId { get; set; }
  public string? PspProgramRepresentativeId { get; set; }
  public InviteType? InviteType { get; set; }
  public PortalInvitationStatusCode? StatusCode { get; set; }
  public string? BceidBusinessName { get; set; }
  public bool IsLinked { get; set; }
}

public enum InviteType
{
  CharacterReference,
  PSIProgramRepresentative,
  WorkExperienceReferenceforApplication,
  WorkExperienceReferenceforICRA
}

public enum PortalInvitationStatusCode
{
  Cancelled,
  Completed,
  Draft,
  Expired,
  Failed,
  Sent,
}
