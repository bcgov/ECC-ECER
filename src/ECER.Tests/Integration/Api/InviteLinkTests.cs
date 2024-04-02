using ECER.Managers.Admin.Contract.InviteLinks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.Api;

public class InviteLinkTests : ApiWebAppScenarioBase
{
  public InviteLinkTests(ITestOutputHelper output, ApiWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GenerateReferenceLink_WithValidInfo_ReturnsOk()
  {
    var validRequest = new PortalInvitationToLinkRequest(Guid.NewGuid(), InviteType.CharacterReference);
    await Host.Scenario(_ =>
    {
      _.Post.Json(validRequest).ToUrl("/api/invitelinks");
      _.StatusCodeShouldBe(200);
    });
  }

  [Fact]
  public async Task CanTransform()
  {
    var bus = Host.Services.GetRequiredService<IMediator>();
    var response = await bus.Send(new GenerateInviteLinkCommand(Guid.NewGuid(), InviteType.CharacterReference), CancellationToken.None);
    response.ShouldNotBeNull();
  }
}
