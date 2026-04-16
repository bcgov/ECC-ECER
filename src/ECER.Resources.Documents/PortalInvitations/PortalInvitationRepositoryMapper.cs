using ECER.Utilities.DataverseSdk.Model;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.PortalInvitations;

internal interface IPortalInvitationRepositoryMapper
{
  PortalInvitation MapPortalInvitation(ecer_PortalInvitation source);
}

[Mapper]
internal sealed partial class PortalInvitationMapper : IPortalInvitationRepositoryMapper
{
  public PortalInvitation MapPortalInvitation(ecer_PortalInvitation source) => new(
    source.ecer_PortalInvitationId.ToString(),
    source.ecer_Name ?? string.Empty,
    source.ecer_FirstName ?? string.Empty,
    source.ecer_LastName ?? string.Empty,
    source.ecer_EmailAddress ?? string.Empty)
  {
    WorkexperienceReferenceId = source.ecer_WorkExperienceReferenceId?.Id.ToString(),
    CharacterReferenceId = source.ecer_CharacterReferenceId?.Id.ToString(),
    ApplicantId = source.ecer_ApplicantId?.Id.ToString(),
    ApplicationId = source.ecer_ApplicationId?.Id.ToString(),
    PspProgramRepresentativeId = source.ecer_psiprogramrepresentativeid?.Id.ToString(),
    InviteType = MapInviteType(source.ecer_Type),
    StatusCode = MapPortalInvitationStatusCode(source.StatusCode),
  };

  private InviteType? MapInviteType(ecer_PortalInvitationTypes? source) => source.HasValue ? MapInviteType(source.Value) : null;

  private PortalInvitationStatusCode? MapPortalInvitationStatusCode(ecer_PortalInvitation_StatusCode? source) => source.HasValue ? MapPortalInvitationStatusCode(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial InviteType MapInviteType(ecer_PortalInvitationTypes source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PortalInvitationStatusCode MapPortalInvitationStatusCode(ecer_PortalInvitation_StatusCode source);
}
