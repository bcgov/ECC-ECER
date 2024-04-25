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

  private CharacterReferenceSubmissionRequest CreateReferenceSubmissionRequest(string token)
  {
    var faker = new Faker("en_CA");

    // Generating random data for ReferenceContactInformation
    var referenceContactInfo = new ReferenceContactInformation(
        faker.Person.LastName,
        faker.Person.FirstName,
        faker.Person.Email,
        faker.Phone.PhoneNumber(),
        "98fbb5c5-68da-ee11-904c-000d3af4645f", // Random Canadian province abbreviation,
        faker.Address.City()
    );

    // Generating random data for ReferenceEvaluation
    var referenceEvaluation = new CharacterReferenceEvaluation(
        faker.Random.Word(), // Relationship
        faker.Random.Word(), // LengthOfAcquaintance
        faker.Random.Bool(), // WorkedWithChildren
        faker.Lorem.Paragraph(), // ChildInteractionObservations
        faker.Lorem.Paragraph() // ApplicantTemperamentAssessment
    );

    // Creating the ReferenceSubmissionRequest record
    var referenceSubmissionRequest = new CharacterReferenceSubmissionRequest(
        token,
        referenceContactInfo,
        referenceEvaluation,
        faker.Random.Bool(), // Confirmed
        faker.Lorem.Paragraph(),
        true
    );

    return referenceSubmissionRequest;
  }

  [Fact]
  public async Task SubmitCharacterReference_ShouldReturnOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationOneId;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.CharacterReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var referenceSubmissionRequest = CreateReferenceSubmissionRequest(token);
    await Host.Scenario(_ =>
    {
      _.Post.Json(referenceSubmissionRequest).ToUrl($"/api/References/Character");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task OptOutCharacterReference_ShouldReturnOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationCharacterReferenceId;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.CharacterReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var optOutReferenceRequest = new OptOutReferenceRequest(token, UnabletoProvideReferenceReasons.Idonotknowthisperson);
    await Host.Scenario(_ =>
    {
      _.Post.Json(optOutReferenceRequest).ToUrl($"/api/OptOutReference");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task OptOutWorkExperienceReference_ShouldReturnOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationWorkExperienceReferenceId;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.WorkExperienceReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var optOutReferenceRequest = new OptOutReferenceRequest(token, UnabletoProvideReferenceReasons.Idonotknowthisperson);
    await Host.Scenario(_ =>
    {
      _.Post.Json(optOutReferenceRequest).ToUrl($"/api/OptOutReference");
      _.StatusCodeShouldBeOk();
    });
  }
}
