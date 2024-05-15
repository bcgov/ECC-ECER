using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.References;
using ECER.Managers.Admin.Contract.PortalInvitations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using InviteType = ECER.Managers.Admin.Contract.PortalInvitations.InviteType;

namespace ECER.Tests.Integration.RegistryApi;

public class ReferenceTests : RegistryPortalWebAppScenarioBase
{
  public ReferenceTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  private CharacterReferenceSubmissionRequest CreateCharacterReferenceSubmissionRequest(string token)
  {
    var faker = new Faker("en_CA");

    // Generating random data for ReferenceContactInformation
    var referenceContactInfo = new ReferenceContactInformation(
        faker.Person.LastName,
        faker.Person.FirstName,
        faker.Person.Email,
        faker.Phone.PhoneNumber(),
        faker.Address.City()
    )
    {
      CertificateProvinceId = "98fbb5c5-68da-ee11-904c-000d3af4645f" // Random Canadian province abbreviation
    };

    // Generating random data for ReferenceEvaluation
    var referenceEvaluation = new CharacterReferenceEvaluation(
        ReferenceRelationship.Supervisor, // Relationship
        string.Empty,        // RelationshipOther
        ReferenceKnownTime.From1to2years, // LengthOfAcquaintance
        faker.Random.Bool(), // WorkedWithChildren
        faker.Lorem.Paragraph(), // ChildInteractionObservations
        faker.Lorem.Paragraph(), // ApplicantTemperamentAssessment
        faker.Random.Bool(), // Confirmed
        faker.Lorem.Paragraph()
    );

    // Creating the ReferenceSubmissionRequest record
    var referenceSubmissionRequest = new CharacterReferenceSubmissionRequest(
        token,
        true,
        referenceContactInfo,
        referenceEvaluation,
        true
    );

    return referenceSubmissionRequest;
  }

  private WorkExperienceReferenceSubmissionRequest CreateWorkExperienceReferenceSubmissionRequest(string token)
  {
    var faker = new Faker("en_CA");

    // Generating random data for ReferenceContactInformation
    var referenceContactInfo = new ReferenceContactInformation(
        faker.Person.LastName,
        faker.Person.FirstName,
        faker.Person.Email,
        faker.Phone.PhoneNumber(),
        faker.Address.City()
    )
    {
      CertificateProvinceId = "98fbb5c5-68da-ee11-904c-000d3af4645f" // Random Canadian province abbreviation
    };

    // Generating random data for WorkExperienceReferenceDetails
    var workExperienceReferenceDetails = new WorkExperienceReferenceDetails(
        faker.Random.Number(1, 100), // Hours
        faker.PickRandom<WorkHoursType>(), // WorkHoursType
        faker.Random.Word(), // ChildrenProgramName
        faker.PickRandom<ChildrenProgramType>(), // ChildrenProgramType
        faker.Random.Word(), // ChildrenProgramTypeOther
        new List<ChildcareAgeRanges>() { ChildcareAgeRanges.Grade1 }, // AgeOfChildrenCaredFor
        faker.Date.Between(DateTime.Now.AddYears(-10), DateTime.Now), // StartDate
        faker.Date.Between(DateTime.Now, DateTime.Now.AddYears(10)), // EndDate
        faker.PickRandom<ReferenceRelationship>(), // ReferenceRelationship
        faker.Random.Word() // ReferenceRelationshipOther
    );

    // Generating random data for WorkExperienceReferenceCompetenciesAssessment
    var workExperienceReferenceCompetenciesAssessment = new WorkExperienceReferenceCompetenciesAssessment(
        faker.PickRandom<LikertScale>(), // ChildDevelopment
        faker.Lorem.Paragraph(), // ChildDevelopmentReason
        faker.PickRandom<LikertScale>(), // ChildGuidance
        faker.Lorem.Paragraph(), // ChildGuidanceReason
        faker.PickRandom<LikertScale>(), // HealthSafetyAndNutrition
        faker.Lorem.Paragraph(), // HealthSafetyAndNutritionReason
        faker.PickRandom<LikertScale>(), // DevelopAnEceCurriculum
        faker.Lorem.Paragraph(), // DevelopAnEceCurriculumReason
        faker.PickRandom<LikertScale>(), // ImplementAnEceCurriculum
        faker.Lorem.Paragraph(), // ImplementAnEceCurriculumReason
        faker.PickRandom<LikertScale>(), // FosteringPositiveRelationChild
        faker.Lorem.Paragraph(), // FosteringPositiveRelationChildReason
        faker.PickRandom<LikertScale>(), // FosteringPositiveRelationFamily
        faker.Lorem.Paragraph(), // FosteringPositiveRelationFamilyReason
        faker.PickRandom<LikertScale>(), // FosteringPositiveRelationCoworker
        faker.Lorem.Paragraph() // FosteringPositiveRelationCoworkerReason
    );

    // Creating the WorkExperienceReferenceSubmissionRequest record
    var workExperienceReferenceSubmissionRequest = new WorkExperienceReferenceSubmissionRequest(
        token,
        true,
        referenceContactInfo,
        workExperienceReferenceDetails,
        workExperienceReferenceCompetenciesAssessment,
        faker.Random.Bool(), // ApplicantShouldNotBeECE
        faker.Lorem.Paragraph(), // ApplicantNotQualifiedReason
        faker.Random.Bool() // ConfirmProvidedInformationIsRight
    );

    return workExperienceReferenceSubmissionRequest;
  }

  [Fact]
  public async Task SubmitCharacterReference_ShouldReturnOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationCharacterReferenceIdSubmit;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.CharacterReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var referenceSubmissionRequest = CreateCharacterReferenceSubmissionRequest(token);
    await Host.Scenario(_ =>
    {
      _.Post.Json(referenceSubmissionRequest).ToUrl($"/api/References/Character");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task SubmitWorkExperienceReference_ShouldReturnOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationWorkExperienceReferenceIdSubmit;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.WorkExperienceReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var referenceSubmissionRequest = CreateWorkExperienceReferenceSubmissionRequest(token);
    await Host.Scenario(_ =>
    {
      _.Post.Json(referenceSubmissionRequest).ToUrl($"/api/References/WorkExperience");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task OptOutCharacterReference_ShouldReturnOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationCharacterReferenceIdOptout;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.CharacterReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var optOutReferenceRequest = new OptOutReferenceRequest(token, UnabletoProvideReferenceReasons.Idonotknowthisperson);
    await Host.Scenario(_ =>
    {
      _.Post.Json(optOutReferenceRequest).ToUrl($"/api/References/OptOut");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task OptOutWorkExperienceReference_ShouldReturnOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationWorkExperienceReferenceIdOptout;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.WorkExperienceReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var optOutReferenceRequest = new OptOutReferenceRequest(token, UnabletoProvideReferenceReasons.Idonotknowthisperson);
    await Host.Scenario(_ =>
    {
      _.Post.Json(optOutReferenceRequest).ToUrl($"/api/References/OptOut");
      _.StatusCodeShouldBeOk();
    });
  }
}
