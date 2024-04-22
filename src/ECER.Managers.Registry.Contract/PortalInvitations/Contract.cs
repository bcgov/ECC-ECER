using MediatR;

namespace ECER.Managers.Registry.Contract.PortalInvitations;

public record PortalInvitationVerificationQuery(string VerificationToken) : IRequest<PortalInvitationVerificationQueryResult>;

public class PortalInvitationVerificationQueryResult
{
  public bool IsSuccess { get; set; }
  public PortalInvitation? Invitation { get; set; }
  public string? ErrorMessage { get; set; }

  public static PortalInvitationVerificationQueryResult Success(PortalInvitation invitation) => new PortalInvitationVerificationQueryResult { IsSuccess = true, Invitation = invitation };

  public static PortalInvitationVerificationQueryResult Failure(string message) => new PortalInvitationVerificationQueryResult { IsSuccess = false, ErrorMessage = message };
}

public record PortalInvitation(string? Id, string Name, string ReferenceFirstName, string ReferenceLastName, string ReferenceEmailAddress)
{
  public string? ApplicantId { get; set; }
  public string? ApplicantFirstName { get; set; }
  public string? ApplicantLastName { get; set; }
  public string? ApplicationId { get; set; }
  public string? WorkexperienceReferenceId { get; set; }
  public string? CharacterReferenceId { get; set; }
  public InviteType? InviteType { get; set; }
  public PortalInvitationStatusCode? StatusCode { get; set; }
}

public enum InviteType
{
  CharacterReference,
  WorkExperienceReference
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
