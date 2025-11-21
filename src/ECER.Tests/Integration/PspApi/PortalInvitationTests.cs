using Alba;
using ECER.Clients.RegistryPortal.Server.References;
using ECER.Managers.Admin.Contract.PortalInvitations;
using ECER.Managers.Registry.Contract.PortalInvitations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

public class PortalInvitationTests : PspPortalWebAppScenarioBase
{
  public PortalInvitationTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task CanGetPortalInvitationData()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationOneId;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, 7), CancellationToken.None);
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
}

