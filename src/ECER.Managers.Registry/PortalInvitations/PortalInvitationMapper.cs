using Riok.Mapperly.Abstractions;
using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractPortalInvitations = ECER.Managers.Registry.Contract.PortalInvitations;
using ResourceApplications = ECER.Resources.Documents.Applications;
using ResourcePortalInvitations = ECER.Resources.Documents.PortalInvitations;

namespace ECER.Managers.Registry.PortalInvitations;

public interface IPortalInvitationMapper
{
  ContractPortalInvitations.PortalInvitation MapPortalInvitation(ResourcePortalInvitations.PortalInvitation source);
  IEnumerable<ContractApplications.CertificationType> MapCertificationTypes(IEnumerable<ResourceApplications.CertificationType> source);
  ContractApplications.WorkExperienceTypes? MapWorkExperienceType(ResourceApplications.WorkExperienceTypes? source);
}

[Mapper]
internal partial class PortalInvitationMapper : IPortalInvitationMapper
{
  public ContractPortalInvitations.PortalInvitation MapPortalInvitation(ResourcePortalInvitations.PortalInvitation source) => new(
    source.Id,
    source.Name,
    source.ReferenceFirstName,
    source.ReferenceLastName,
    source.ReferenceEmailAddress)
  {
    ApplicantId = source.ApplicantId,
    ApplicationId = source.ApplicationId,
    WorkexperienceReferenceId = source.WorkexperienceReferenceId,
    CharacterReferenceId = source.CharacterReferenceId,
    PspProgramRepresentativeId = source.PspProgramRepresentativeId,
    InviteType = MapInviteType(source.InviteType),
    StatusCode = MapPortalInvitationStatusCode(source.StatusCode),
    BceidBusinessName = source.BceidBusinessName,
    PostSecondaryInstitutionName = source.PostSecondaryInstitutionName,
    IsLinked = source.IsLinked,
  };

  public IEnumerable<ContractApplications.CertificationType> MapCertificationTypes(IEnumerable<ResourceApplications.CertificationType> source) => source.Select(MapCertificationType).ToList();

  public ContractApplications.WorkExperienceTypes? MapWorkExperienceType(ResourceApplications.WorkExperienceTypes? source) => source.HasValue ? MapWorkExperienceType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPortalInvitations.InviteType MapInviteType(ResourcePortalInvitations.InviteType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPortalInvitations.PortalInvitationStatusCode MapPortalInvitationStatusCode(ResourcePortalInvitations.PortalInvitationStatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.CertificationType MapCertificationType(ResourceApplications.CertificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractApplications.WorkExperienceTypes MapWorkExperienceType(ResourceApplications.WorkExperienceTypes source);

  private ContractPortalInvitations.InviteType? MapInviteType(ResourcePortalInvitations.InviteType? source) => source.HasValue ? MapInviteType(source.Value) : null;

  private ContractPortalInvitations.PortalInvitationStatusCode? MapPortalInvitationStatusCode(ResourcePortalInvitations.PortalInvitationStatusCode? source) => source.HasValue ? MapPortalInvitationStatusCode(source.Value) : null;
}
