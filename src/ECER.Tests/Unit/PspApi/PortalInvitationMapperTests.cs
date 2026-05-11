using ContractApplications = ECER.Managers.Registry.Contract.Applications;
using ContractPortalInvitations = ECER.Managers.Registry.Contract.PortalInvitations;
using ECER.Managers.Registry.PortalInvitations;
using ResourceApplications = ECER.Resources.Documents.Applications;
using ResourcePortalInvitations = ECER.Resources.Documents.PortalInvitations;
using Shouldly;

namespace ECER.Tests.Unit.PspApi;

public class PortalInvitationMapperTests
{
  [Fact]
  public void MapPortalInvitation_MapsAllSharedFields()
  {
    var mapper = new PortalInvitationMapper();
    var invitationId = Guid.NewGuid().ToString();

    var source = new ResourcePortalInvitations.PortalInvitation(invitationId, "Invite", "Ref", "Person", "ref@example.org")
    {
      ApplicantId = Guid.NewGuid().ToString(),
      ApplicationId = Guid.NewGuid().ToString(),
      WorkexperienceReferenceId = Guid.NewGuid().ToString(),
      CharacterReferenceId = Guid.NewGuid().ToString(),
      PspProgramRepresentativeId = Guid.NewGuid().ToString(),
      InviteType = ResourcePortalInvitations.InviteType.WorkExperienceReferenceforApplication,
      StatusCode = ResourcePortalInvitations.PortalInvitationStatusCode.Sent,
      BceidBusinessName = "Business",
      PostSecondaryInstitutionName = "Institute",
      IsLinked = true,
    };

    var result = mapper.MapPortalInvitation(source);

    result.Id.ShouldBe(source.Id);
    result.Name.ShouldBe(source.Name);
    result.ReferenceFirstName.ShouldBe(source.ReferenceFirstName);
    result.ReferenceLastName.ShouldBe(source.ReferenceLastName);
    result.ReferenceEmailAddress.ShouldBe(source.ReferenceEmailAddress);
    result.ApplicantId.ShouldBe(source.ApplicantId);
    result.ApplicationId.ShouldBe(source.ApplicationId);
    result.WorkexperienceReferenceId.ShouldBe(source.WorkexperienceReferenceId);
    result.CharacterReferenceId.ShouldBe(source.CharacterReferenceId);
    result.PspProgramRepresentativeId.ShouldBe(source.PspProgramRepresentativeId);
    result.InviteType.ShouldBe(ContractPortalInvitations.InviteType.WorkExperienceReferenceforApplication);
    result.StatusCode.ShouldBe(ContractPortalInvitations.PortalInvitationStatusCode.Sent);
    result.BceidBusinessName.ShouldBe(source.BceidBusinessName);
    result.PostSecondaryInstitutionName.ShouldBe(source.PostSecondaryInstitutionName);
    result.IsLinked.ShouldBeTrue();
  }

  [Fact]
  public void MapCertificationTypes_MapsByName()
  {
    var mapper = new PortalInvitationMapper();

    var result = mapper.MapCertificationTypes(
    [
      ResourceApplications.CertificationType.OneYear,
      ResourceApplications.CertificationType.Sne,
    ]);

    result.ShouldBe(
    [
      ContractApplications.CertificationType.OneYear,
      ContractApplications.CertificationType.Sne,
    ]);
  }

  [Fact]
  public void MapWorkExperienceType_MapsNullableType()
  {
    var mapper = new PortalInvitationMapper();

    mapper.MapWorkExperienceType(ResourceApplications.WorkExperienceTypes.Is500Hours)
      .ShouldBe(ContractApplications.WorkExperienceTypes.Is500Hours);

    mapper.MapWorkExperienceType(null).ShouldBeNull();
  }
}
