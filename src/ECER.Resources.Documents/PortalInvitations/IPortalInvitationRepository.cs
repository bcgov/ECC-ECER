﻿namespace ECER.Resources.Documents.PortalInvitations;

public interface IPortalInvitationRepository
{
  Task<PortalInvitation> Query(PortalInvitationQuery query, CancellationToken ct);

  Task<bool> Expire(ExpirePortalInvitationCommand command, CancellationToken ct);
}

public record PortalInvitationQuery(Guid portalInvitationId);
public record ExpirePortalInvitationCommand(Guid portalInvitationId);

public record PortalInvitation(string? Id, string Name, string ReferenceFirstName, string ReferenceLastName, string ReferenceEmailAddress)
{
  public string? ApplicantId { get; set; }
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
