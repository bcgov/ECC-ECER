using Microsoft.AspNetCore.Http.HttpResults;
using ECER.Utilities.Hosting;
using MediatR;
using ECER.Managers.E2ETest.Contract.E2ETestsContacts;
using Microsoft.AspNetCore.Mvc;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Security;
using AutoMapper;
using ECER.Clients.RegistryPortal.Server.Applications;
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

      if (!ctx.Request.Headers.TryGetValue("APPLICATION-TYPE", out var applicationType))
      {
        return TypedResults.BadRequest(new ProblemDetails { Title = "Missing header: APPLICATION-TYPE" });
      }

      RegistryPortal.Server.Applications.CertificationType certificationType;
      switch (applicationType.ToString())
      {
        case "Assistant":
          certificationType = RegistryPortal.Server.Applications.CertificationType.EceAssistant;
          break;

        case "OneYear":
          certificationType = RegistryPortal.Server.Applications.CertificationType.OneYear;
          break;

        case "5Year":
          certificationType = RegistryPortal.Server.Applications.CertificationType.FiveYears;
          break;

        default:
          return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid header: APPLICATION-TYPE." });
      }

      var userIdentity = new UserIdentity(externalUserId.ToString(), "bcsc"); // Convert StringValues to UserIdentity
      var profile = (await messageBus.Send(new SearchRegistrantQuery { ByUserIdentity = userIdentity }, ctx.RequestAborted)).Items.SingleOrDefault();
      if (profile == null) return TypedResults.NotFound();
      var contact_id = profile.UserId;

      var draftApplicationObj = new Faker<DraftApplication>("en_CA")
            .RuleFor(f => f.CertificationTypes, f => f.Make(1, () => certificationType))
            .RuleFor(f => f.SignedDate, f => f.Date.Recent())
            .RuleFor(f => f.Transcripts, f => f.Make(f.Random.Number(2, 5), () => CreateTranscript()))
            .RuleFor(f => f.CharacterReferences, f => f.Make(1, () => CreateCharacterReference()))
            .Generate();

      var draftApplication = mapper.Map<Managers.Registry.Contract.Applications.Application>(draftApplicationObj, opts => opts.Items.Add("registrantId", contact_id))!;

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
      var result = await messageBus.Send(new E2ETestsGenerateCertificateCommand(submitAppResult.Application!.Id!), ct);
      return TypedResults.Ok(result);
    })
    .WithOpenApi("Handles seeding of Applications and certifications for Renewal workflow", string.Empty, "E2ETests_seed_post_application_certificate")
    .RequireAuthorization()
    .DisableAntiforgery()
    .WithParameterValidation();
  }

  private static RegistryPortal.Server.Applications.Transcript CreateTranscript()
  {
    // Use Faker to generate values for the required parameters
    var faker = new Faker("en_CA");
    var educationalInstitutionName = faker.Company.CompanyName();
    var programName = $"{faker.Hacker.Adjective()} Program";
    var studentLastName = faker.Name.LastName();
    var startDate = faker.Date.Past();
    var endDate = faker.Date.Past();
    var isNameUnverified = faker.Random.Bool();
    var educationRecognition = new RegistryPortal.Server.Applications.EducationRecognition(); // Initialize as needed
    var educationOrigin = new RegistryPortal.Server.Applications.EducationOrigin(); // Initialize as needed

    // Instantiate the Transcript record with the required arguments
    var transcript = new RegistryPortal.Server.Applications.Transcript(
        educationalInstitutionName,
        programName,
        studentLastName,
        startDate,
        endDate,
        isNameUnverified,
        educationRecognition,
        educationOrigin
    )
    {
      // Populate optional properties
      Id = null,
      CampusLocation = faker.Address.City(),
      StudentFirstName = faker.Name.FirstName(),
      StudentNumber = faker.Random.Number(10000000, 99999999).ToString(),
      IsECEAssistant = faker.Random.Bool(),
      TranscriptStatusOption = RegistryPortal.Server.Applications.TranscriptStatusOptions.OfficialTranscriptRequested,
    };

    return transcript;
  }

  private RegistryPortal.Server.Applications.CharacterReference CreateCharacterReference()
  {
    var faker = new Faker("en_CA");
    return new RegistryPortal.Server.Applications.CharacterReference(
      faker.Name.LastName(), faker.Phone.PhoneNumber(), "Character_Reference@test.gov.bc.ca"
    )
    { FirstName = faker.Name.FirstName() };
  }
}
