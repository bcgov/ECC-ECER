using Alba;
using ECER.Clients.PSPPortal.Server.Users;
using ECER.Managers.Admin.Contract.PortalInvitations;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

public class RegistrationTests : PspPortalWebAppScenarioBase
{
  public RegistrationTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  private RegisterPspUserRequest BuildRequest(string token, string programRepresentativeId, string bceidBusinessId, string bceidBusinessName) =>
    new RegisterPspUserRequest(
      Token: token,
      ProgramRepresentativeId: programRepresentativeId,
      BceidBusinessId: bceidBusinessId,
      BceidBusinessName: bceidBusinessName)
    {
      Profile = new PspUserProfile
      {
        FirstName = Faker.Name.FirstName(),
        LastName = Faker.Name.LastName(),
        Email = $"test_{Faker.Internet.Email()}",
        JobTitle = Faker.Company.Bs()
      }
    };

  private UserIdentity UniqueIdentity() => new UserIdentity($"{Guid.NewGuid():N}", "bceidbusiness");

  [Fact]
  public async Task RegisterPspUser_WithEmptyBceidBusinessId_ReturnsBadRequest()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(Fixture.RegistrationTestPortalInvitationId, 7), CancellationToken.None);
    var token = packingResponse.VerificationLink.Split('/')[2];

    await Host.Scenario(_ =>
    {
      _.WithPspUser(UniqueIdentity());
      _.Post.Json(BuildRequest(token, Fixture.RegistrationTestProgramRepId, "", "Test Business Name")).ToUrl("/api/users/register");
      _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task RegisterPspUser_WithNonMatchingBceidBusinessId_ReturnsBadRequest()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(UniqueIdentity());
      _.Post.Json(BuildRequest("any-token", Fixture.AuthenticatedPspUserId, "wrong-bceid-id", "Some Business")).ToUrl("/api/users/register");
      _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
    });

    var body = await response.ReadAsJsonAsync<PspRegistrationErrorResponse>();
    body!.ErrorCode.ShouldBe(PspRegistrationError.BceidBusinessIdDoesNotMatch);
  }

  [Fact]
  public async Task RegisterPspUser_WithConsumedPortalInvitation_ReturnsBadRequest()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(Fixture.RegistrationCompletedPortalInvitationId, 7), CancellationToken.None);
    var token = packingResponse.VerificationLink.Split('/')[2];

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(UniqueIdentity());
      _.Post.Json(BuildRequest(token, Fixture.RegistrationTestProgramRepId, PspPortalWebAppFixture.RegistrationTestBceidBusinessId, "Test Business Name")).ToUrl("/api/users/register");
      _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
    });

    var body = await response.ReadAsJsonAsync<PspRegistrationErrorResponse>();
    body!.ErrorCode.ShouldBe(PspRegistrationError.PortalInvitationWrongStatus);
  }

  [Fact]
  public async Task RegisterPspUser_WithValidRequest_ReturnsOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(Fixture.RegistrationSuccessPortalInvitationId, 7), CancellationToken.None);
    var token = packingResponse.VerificationLink.Split('/')[2];

    await Host.Scenario(_ =>
    {
      _.WithPspUser(UniqueIdentity());
      _.Post.Json(BuildRequest(token, Fixture.RegistrationSuccessProgramRepId, PspPortalWebAppFixture.RegistrationTestBceidBusinessId, "Test Business Name")).ToUrl("/api/users/register");
      _.StatusCodeShouldBeOk();
    });
  }
}
