using ECER.Managers.Admin.Contract.PortalInvitations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.Managers.Admin;

public class InviteLinkHandlerTests : ApiWebAppScenarioBase
{
  public InviteLinkHandlerTests(ITestOutputHelper output, ApiWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task CanTransformPortalInvitationToLink()
  {
    var portalInvitationId = Fixture.testPortalInvitationForLinkGeneration.Id;
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var response = await bus.Send(new GenerateInviteLinkCommand(portalInvitationId, 7), CancellationToken.None);
    response.ShouldNotBeNull();
  }
}
