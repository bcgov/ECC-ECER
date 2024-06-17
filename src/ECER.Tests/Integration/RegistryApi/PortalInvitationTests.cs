using Alba;
using ECER.Clients.RegistryPortal.Server.References;
using ECER.Managers.Admin.Contract.PortalInvitations;
using ECER.Managers.Registry.Contract.PortalInvitations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using InviteType = ECER.Managers.Admin.Contract.PortalInvitations.InviteType;

namespace ECER.Tests.Integration.RegistryApi;

public class PortalInvitationTests : RegistryPortalWebAppScenarioBase
{
  public PortalInvitationTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task CanGetPortalInvitationData()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationWorkExperienceReferenceIdSubmit;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.WorkExperienceReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var verifyResponse = await bus.Send(new PortalInvitationVerificationQuery(token), CancellationToken.None);

    verifyResponse.Invitation!.Id.ShouldBe(portalInvitation.ToString());

    var inviteLinkResponse = await Host.Scenario(_ =>
    {
      _.Get.Url($"/api/PortalInvitations/{token}");
      _.StatusCodeShouldBeOk();
    });

    var queryResult = await inviteLinkResponse.ReadAsJsonAsync<PortalInvitationQueryResult>();
    queryResult.ShouldNotBeNull();
  }

  [Fact]
  public async Task DoNotReturnCompletedPortalInvitationData()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationWorkExperienceReferenceIdCompleted;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.WorkExperienceReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var verifyResponse = await bus.Send(new PortalInvitationVerificationQuery(token), CancellationToken.None);

    verifyResponse.Invitation.ShouldBeNull();
    verifyResponse.ErrorMessage.ShouldBe("Reference has already been submitted.");
    verifyResponse.IsSuccess.ShouldBeFalse();

    await Host.Scenario(_ =>
    {
      _.Get.Url($"/api/PortalInvitations/{token}");
      _.StatusCodeShouldBe(400);
    });
  }
}
