using Microsoft.AspNetCore.Http.HttpResults;
using ECER.Utilities.Hosting;
using MediatR;
using ECER.Managers.E2ETest.Contract.E2ETestsContacts;
using Microsoft.AspNetCore.Mvc;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Security;
using AutoMapper;
using ECER.Managers.Registry.Contract.Applications;
using Bogus;

namespace ECER.Clients.E2ETestData.E2ETests;

public class E2ETestsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapDelete("/api/E2ETests/user/reset", async Task<Results<Ok<string>, BadRequest<ProblemDetails>, NotFound>> (HttpContext ctx, CancellationToken ct, IMediator messageBus) =>
    {
      if (!ctx.Request.Headers.TryGetValue("EXTERNAL-USER-ID", out var externalUserId))
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Missing header: EXTERNAL-USER-ID" });
      }

      var userIdentity = new UserIdentity(externalUserId.ToString(), "bcsc"); // Convert StringValues to UserIdentity
      var profile = (await messageBus.Send(new SearchRegistrantQuery { ByUserIdentity = userIdentity }, ctx.RequestAborted)).Items.SingleOrDefault();
      if (profile == null) return TypedResults.NotFound();
      var contact_id = profile.UserId;

      var result = await messageBus.Send(new E2ETestsDeleteContactApplicationsCommand(contact_id), ct);
      return TypedResults.Ok(result);
    })
    .WithOpenApi("Handles Deletion of all Applications and certificates of a User for E2E Tests", string.Empty, "E2ETests_delete_contact_applications")
    .RequireAuthorization()
    .DisableAntiforgery()
    .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/E2ETests/applications/seed/renewal", async Task<Results<Ok<string>, BadRequest<ProblemDetails>, NotFound>> (HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      if (!ctx.Request.Headers.TryGetValue("EXTERNAL-USER-ID", out var externalUserId))
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Missing header: EXTERNAL-USER-ID" });
      }

      if (!ctx.Request.Headers.TryGetValue("APP-STATUS", out var appStatus))
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Missing header: APP-STATUS" });
      }

      if (!bool.TryParse(appStatus, out var certIsActive))
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid boolean value for header: APP-EXPIRATION" });
      }

      if (!ctx.Request.Headers.TryGetValue("APP-EXPIRATION", out var appExpiration))
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Missing header: APP-STATUS" });
      }

      if (!bool.TryParse(appExpiration, out var isExpiredMoreThan5Years))
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid boolean value for header: APP-EXPIRATION" });
      }

      if (!ctx.Request.Headers.TryGetValue("APPLICATION-TYPE", out var applicationType))
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Missing header: APPLICATION-TYPE" });
      }

      CertificationType certificationType;
      switch (applicationType.ToString())
      {
        case "ECEAssistant":
          certificationType = CertificationType.EceAssistant;
          break;

        case "ECEOneYear":
          certificationType = CertificationType.OneYear;
          break;

        case "ECE5Years":
          certificationType = CertificationType.FiveYears;
          break;

        default:
          return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid header: APPLICATION-TYPE." });
      }

      var userIdentity = new UserIdentity(externalUserId.ToString(), "bcsc"); // Convert StringValues to UserIdentity
      var profile = (await messageBus.Send(new SearchRegistrantQuery { ByUserIdentity = userIdentity }, ctx.RequestAborted)).Items.SingleOrDefault();
      if (profile == null) return TypedResults.NotFound();
      var contact_id = profile.UserId;

      var faker = new Faker("en_CA");

      var draftApplication = new Application(null, contact_id!, ApplicationStatus.Draft);
      draftApplication.CertificationTypes = faker.Make(1, () => certificationType);
      draftApplication.SignedDate = faker.Date.Recent();
      draftApplication.Transcripts = faker.Make(faker.Random.Number(2, 5), () => CreateTranscript());
      draftApplication.CharacterReferences = faker.Make(1, () => CreateCharacterReference());
      if (certificationType == CertificationType.FiveYears)
      {
        draftApplication.WorkExperienceReferences = faker.Make(faker.Random.Number(3, 5), () => CreateWorkExperienceReference());
      }

      var application = await messageBus.Send(new SaveDraftApplicationCommand(draftApplication), ct);

      var cmd = new SubmitApplicationCommand(application!.Id!, contact_id!);
      var submitAppResult = await messageBus.Send(cmd, ct);

      if (!submitAppResult.IsSuccess && submitAppResult.Error == SubmissionError.DraftApplicationNotFound)
      {
        return TypedResults.NotFound();
      }
      if (!submitAppResult.IsSuccess && submitAppResult.Error == SubmissionError.DraftApplicationValidationFailed)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Title = "Application submission failed",
          Extensions = { ["errors"] = submitAppResult.ValidationErrors }
        };
        return TypedResults.BadRequest(problemDetails);
      }
      var result = await messageBus.Send(new E2ETestsGenerateCertificateCommand(submitAppResult.Application!.Id!, certIsActive, isExpiredMoreThan5Years), ct);
      return TypedResults.Ok(result);
    })
    .WithOpenApi("Handles seeding of Applications and certifications for Renewal workflow", string.Empty, "E2ETests_seed_post_application_certificate")
    .RequireAuthorization()
    .DisableAntiforgery()
    .WithParameterValidation();
  }

  private static Transcript CreateTranscript()
  {
    // Use Faker to generate values for the required parameters
    var faker = new Faker("en_CA");
    var educationalInstitutionName = faker.Company.CompanyName();
    var programName = $"{faker.Hacker.Adjective()} Program";
    var studentNumber = faker.Random.Number(10000000, 99999999).ToString();
    var studentLastName = faker.Name.LastName();
    var startDate = faker.Date.Past();
    var endDate = faker.Date.Past();
    var isECEAssistant = faker.Random.Bool();
    var studentFirstName = faker.Name.FirstName();
    var isNameUnverified = faker.Random.Bool();
    var educationRecognition = EducationRecognition.Recognized; // Initialize as needed
    var educationOrigin = EducationOrigin.InsideBC; // Initialize as needed

    // Instantiate the Transcript record with the required arguments
    var transcript = new Transcript(
        null,
        educationalInstitutionName,
        programName,
        studentNumber,
        startDate,
        endDate,
        isECEAssistant,
        studentFirstName,
        studentLastName,
        isNameUnverified,
        educationRecognition,
        educationOrigin
    )
    {
      // Populate optional properties
      CampusLocation = faker.Address.City(),
      TranscriptStatusOption = TranscriptStatusOptions.OfficialTranscriptRequested,
    };

    return transcript;
  }

  private CharacterReference CreateCharacterReference()
  {
    var faker = new Faker("en_CA");
    return new CharacterReference(
      faker.Name.FirstName(), faker.Name.LastName(), faker.Phone.PhoneNumber(), "Character_Reference@test.gov.bc.ca"
    )
    { Status = CharacterReferenceStage.Draft };
  }

  private WorkExperienceReference CreateWorkExperienceReference()
  {
    var faker = new Faker("en_CA");
    return new WorkExperienceReference(
      faker.Name.FirstName(), faker.Name.LastName(), "Work_Experience_Reference@test.gov.bc.ca", faker.Random.Number(200, 250)
    )
    {
      PhoneNumber = faker.Phone.PhoneNumber()
    };
  }
}
