using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Applications;
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
        "Reference_Contact@test.gov.bc.ca",
        faker.Phone.PhoneNumber(),
        faker.Address.City()
    )
    {
      FirstName = faker.Person.FirstName,
      CertificateProvinceId = "98fbb5c5-68da-ee11-904c-000d3af4645f" // Random Canadian province abbreviation
    };

    // Generating random data for ReferenceEvaluation
    var referenceEvaluation = new CharacterReferenceEvaluation(
        ReferenceRelationship.Supervisor, // Relationship
        string.Empty,        // RelationshipOther
        ReferenceKnownTime.From1to2years, // LengthOfAcquaintance
        faker.Random.Bool(), // WorkedWithChildren
        faker.Lorem.Paragraph(), // ChildInteractionObservations
        faker.Lorem.Paragraph() // ApplicantTemperamentAssessment
    );

    // Creating the ReferenceSubmissionRequest record
    var referenceSubmissionRequest = new CharacterReferenceSubmissionRequest(
        token,
        true,
        referenceContactInfo,
        referenceEvaluation,
        true,
        faker.Random.Word() //recaptcha token
    );

    return referenceSubmissionRequest;
  }

  private WorkExperienceReferenceSubmissionRequest Create500HoursTypeWorkExperienceReferenceSubmissionRequest(string token)
  {
    var faker = new Faker("en_CA");

    // Generating random data for ReferenceContactInformation
    var referenceContactInfo = new ReferenceContactInformation(
        faker.Person.LastName,
        "Reference_Contact@test.gov.bc.ca",
        faker.Phone.PhoneNumber(),
        faker.Address.City()
    )
    {
      FirstName = faker.Person.FirstName,
      CertificateProvinceId = "98fbb5c5-68da-ee11-904c-000d3af4645f" // Random Canadian province abbreviation
    };

    // Generating random data for WorkExperienceReferenceDetails
    var workExperienceReferenceDetails = new WorkExperienceReferenceDetails()
    {
      Hours = faker.Random.Number(1, 100), // Hours
      WorkHoursType = faker.PickRandom<WorkHoursType>(), // WorkHoursType
      ChildrenProgramName = faker.Random.Word(), // ChildrenProgramName
      ChildrenProgramType = faker.PickRandom<ChildrenProgramType>(), // ChildrenProgramType
      ChildrenProgramTypeOther = faker.Random.Word(), // ChildrenProgramTypeOther
      ChildcareAgeRanges = new List<ChildcareAgeRanges>() { ChildcareAgeRanges._35years }, // Child care Age Ranges
      Role = null, //Role - skipped for 500 hours type work experience reference
      AgeofChildrenCaredFor = null, // age of children cared for - skipped for 500 hours type work experience reference
      StartDate = faker.Date.Between(DateTime.Now.AddYears(-10), DateTime.Now), // StartDate
      EndDate = faker.Date.Between(DateTime.Now, DateTime.Now.AddYears(10)), // EndDate
      ReferenceRelationship = faker.PickRandom<ReferenceRelationship>(), // ReferenceRelationship
      ReferenceRelationshipOther = faker.Random.Word(), // ReferenceRelationshipOther
      AdditionalComments = null, // additional comments - skipped for 500 hours type work experience reference
      WorkExperienceType = WorkExperienceTypes.Is500Hours // Set 500 hours Type Work Experience for Validations
    };

    // Generating random data for WorkExperienceReferenceCompetenciesAssessment
    var workExperienceReferenceCompetenciesAssessment = new WorkExperienceReferenceCompetenciesAssessment()
    {
      ChildDevelopment = faker.PickRandom<LikertScale>(), // ChildDevelopment
      ChildDevelopmentReason = faker.Lorem.Paragraph(), // ChildDevelopmentReason
      ChildGuidance = faker.PickRandom<LikertScale>(), // ChildGuidance
      ChildGuidanceReason = faker.Lorem.Paragraph(), // ChildGuidanceReason
      HealthSafetyAndNutrition = faker.PickRandom<LikertScale>(), // HealthSafetyAndNutrition
      HealthSafetyAndNutritionReason = faker.Lorem.Paragraph(), // HealthSafetyAndNutritionReason
      DevelopAnEceCurriculum = faker.PickRandom<LikertScale>(), // DevelopAnEceCurriculum
      DevelopAnEceCurriculumReason = faker.Lorem.Paragraph(), // DevelopAnEceCurriculumReason
      ImplementAnEceCurriculum = faker.PickRandom<LikertScale>(), // ImplementAnEceCurriculum
      ImplementAnEceCurriculumReason = faker.Lorem.Paragraph(), // ImplementAnEceCurriculumReason
      FosteringPositiveRelationChild = faker.PickRandom<LikertScale>(), // FosteringPositiveRelationChild
      FosteringPositiveRelationChildReason = faker.Lorem.Paragraph(), // FosteringPositiveRelationChildReason
      FosteringPositiveRelationFamily = faker.PickRandom<LikertScale>(), // FosteringPositiveRelationFamily
      FosteringPositiveRelationFamilyReason = faker.Lorem.Paragraph(), // FosteringPositiveRelationFamilyReason
      FosteringPositiveRelationCoworker = faker.PickRandom<LikertScale>(), // FosteringPositiveRelationCoworker
      FosteringPositiveRelationCoworkerReason = faker.Lorem.Paragraph() // FosteringPositiveRelationCoworkerReason
    };

    // Creating the WorkExperienceReferenceSubmissionRequest record
    var workExperienceReferenceSubmissionRequest = new WorkExperienceReferenceSubmissionRequest(
        token,
        true,
        referenceContactInfo,
        workExperienceReferenceDetails,
        workExperienceReferenceCompetenciesAssessment,
        faker.Random.Bool(), // ConfirmProvidedInformationIsRight
        faker.Random.Word() //recaptcha token
    )
    { WorkExperienceType = WorkExperienceTypes.Is500Hours }; // Set 500 hours Type Work Experience for Validations

    return workExperienceReferenceSubmissionRequest;
  }

  private WorkExperienceReferenceSubmissionRequest Create400HoursTypeWorkExperienceReferenceSubmissionRequest(string token)
  {
    var faker = new Faker("en_CA");

    // Generating random data for ReferenceContactInformation
    var referenceContactInfo = new ReferenceContactInformation(
        faker.Person.LastName,
        "Reference_Contact@test.gov.bc.ca",
        faker.Phone.PhoneNumber(),
        faker.Address.City()
    )
    {
      FirstName = faker.Person.FirstName,
      CertificateProvinceId = "98fbb5c5-68da-ee11-904c-000d3af4645f" // Random Canadian province abbreviation
    };

    // Generating random data for WorkExperienceReferenceDetails
    var workExperienceReferenceDetails = new WorkExperienceReferenceDetails()
    {
      Hours = faker.Random.Number(1, 100), // Hours
      WorkHoursType = faker.PickRandom<WorkHoursType>(), // WorkHoursType
      ChildrenProgramName = faker.Random.Word(), // ChildrenProgramName
      ChildrenProgramType = null, // ChildrenProgramType - skipped for 400 hours type work experience reference
      ChildrenProgramTypeOther = null, // ChildrenProgramTypeOther - skipped for 400 hours type work experience reference
      ChildcareAgeRanges = null, // Child care Age Ranges - skipped for 400 hours type work experience reference
      Role = "Child Care Provider", //Role - needed for 400 hours type work experience reference
      AgeofChildrenCaredFor = faker.PickRandom<ChildcareAgeRanges>().ToString(), // age of children cared for - for 400 hours type work experience reference
      StartDate = faker.Date.Between(DateTime.Now.AddYears(-10), DateTime.Now), // StartDate
      EndDate = faker.Date.Between(DateTime.Now, DateTime.Now.AddYears(10)), // EndDate
      ReferenceRelationship = faker.PickRandom<ReferenceRelationship>(), // ReferenceRelationship
      ReferenceRelationshipOther = null, // ReferenceRelationshipOther - skipped for 400 hours type work experience reference
      AdditionalComments = faker.Lorem.Paragraph(), // additional comments - for 400 hours type work experience reference
      WorkExperienceType = WorkExperienceTypes.Is400Hours // Set 400 hours Type Work Experience for Validations
    };

    // Creating the WorkExperienceReferenceSubmissionRequest record
    var workExperienceReferenceSubmissionRequest = new WorkExperienceReferenceSubmissionRequest(
        token,
        true, // will provide reference
        referenceContactInfo,
        workExperienceReferenceDetails,
        null, // Competencies Assessment - skipped for 400 hours type work experience reference
        faker.Random.Bool(), // ConfirmProvidedInformationIsRight
        faker.Random.Word() //recaptcha token
    )
    { WorkExperienceType = WorkExperienceTypes.Is400Hours };// Set 400 hours Type Work Experience for Validations

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
  public async Task Submit500HoursTypeWorkExperienceReference_ShouldReturnOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationWorkExperienceReferenceIdSubmit;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.WorkExperienceReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var referenceSubmissionRequest = Create500HoursTypeWorkExperienceReferenceSubmissionRequest(token);
    await Host.Scenario(_ =>
    {
      _.Post.Json(referenceSubmissionRequest).ToUrl($"/api/References/WorkExperience");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task Submit400HoursTypeWorkExperienceReference_ShouldReturnOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitation400HoursTypeWorkExperienceReferenceIdSubmit;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.WorkExperienceReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var referenceSubmissionRequest = Create400HoursTypeWorkExperienceReferenceSubmissionRequest(token);
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
    var optOutReferenceRequest = new OptOutReferenceRequest(token, UnabletoProvideReferenceReasons.Idonotknowthisperson, Faker.Random.Word());
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
    var optOutReferenceRequest = new OptOutReferenceRequest(token, UnabletoProvideReferenceReasons.Idonotknowthisperson, Faker.Random.Word());
    await Host.Scenario(_ =>
    {
      _.Post.Json(optOutReferenceRequest).ToUrl($"/api/References/OptOut");
      _.StatusCodeShouldBeOk();
    });
  }
}
