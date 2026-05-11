using ECER.Clients.RegistryPortal.Server.Certifications;
using Riok.Mapperly.Abstractions;
using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractPortalInvitations = ECER.Managers.Registry.Contract.PortalInvitations;

namespace ECER.Clients.RegistryPortal.Server.References;

internal interface IPortalInvitationMapper
{
  PortalInvitation MapPortalInvitation(ContractPortalInvitations.PortalInvitation source);
}

[Mapper]
internal partial class PortalInvitationMapper(ICertificationMapper certificationMapper) : IPortalInvitationMapper
{
  public PortalInvitation MapPortalInvitation(ContractPortalInvitations.PortalInvitation source) => new(
    source.Id,
    source.Name,
    source.ReferenceFirstName,
    source.ReferenceLastName,
    source.ReferenceEmailAddress)
  {
    ApplicantFirstName = source.ApplicantFirstName,
    ApplicantLastName = source.ApplicantLastName,
    ApplicationId = source.ApplicationId,
    ApplicationSubmittedOn = source.ApplicationSubmittedOn,
    CertificationTypes = source.CertificationTypes.Select(MapCertificationType).ToList(),
    WorkexperienceReferenceId = source.WorkexperienceReferenceId,
    CharacterReferenceId = source.CharacterReferenceId,
    InviteType = MapInviteType(source.InviteType),
    WorkExperienceReferenceHours = source.WorkExperienceReferenceHours,
    WorkExperienceType = MapWorkExperienceType(source.WorkExperienceType),
    FromCertificate = source.FromCertificate == null ? null : certificationMapper.MapCertification(source.FromCertificate),
  };

  private static InviteType? MapInviteType(ContractPortalInvitations.InviteType? source) => source switch
  {
    null => null,
    ContractPortalInvitations.InviteType.CharacterReference => InviteType.CharacterReference,
    ContractPortalInvitations.InviteType.WorkExperienceReferenceforApplication => InviteType.WorkExperienceReferenceforApplication,
    ContractPortalInvitations.InviteType.WorkExperienceReferenceforICRA => InviteType.WorkExperienceReferenceforICRA,
    _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial Applications.CertificationType MapCertificationType(ContractApplications.CertificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial Applications.WorkExperienceTypes MapWorkExperienceType(ContractApplications.WorkExperienceTypes source);

  private Applications.WorkExperienceTypes? MapWorkExperienceType(ContractApplications.WorkExperienceTypes? source) => source.HasValue ? MapWorkExperienceType(source.Value) : null;
}
