using Alba;
using ECER.Clients.RegistryPortal.Server.References;
using ECER.Managers.Admin.Contract.PortalInvitations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class PortalInvitationTests : RegistryPortalWebAppScenarioBase
{
  public PortalInvitationTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task CanGetPortalInvitationData()
  {
    var bus = Host.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationId;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.WorkExperienceReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.verificationLink.Split('/')[2];
    var verifyResponse = await bus.Send(new VerifyInviteTokenCommand(token), CancellationToken.None);
    verifyResponse!.portalInvitation.ShouldBe(portalInvitation);
    verifyResponse.inviteType.ShouldBe(InviteType.WorkExperienceReference);

    var inviteLinkResponse = await Host.Scenario(_ =>
    {
      _.Get.Url($"/api/PortalInvitations/{token}");
      _.StatusCodeShouldBeOk();
    });

    var queryResult = await inviteLinkResponse.ReadAsJsonAsync<ReferenceQueryResult>();
    queryResult.ShouldNotBeNull();
  }
}
