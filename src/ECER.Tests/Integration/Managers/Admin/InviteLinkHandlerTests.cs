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
    var bus = Host.Services.GetRequiredService<IMediator>();
    var response = await bus.Send(new GenerateInviteLinkCommand(Guid.NewGuid(), InviteType.CharacterReference, 7), CancellationToken.None);
    response.ShouldNotBeNull();
  }

  [Fact]
  public async Task CanTransformLinkToPortalInvitation()
  {
    var bus = Host.Services.GetRequiredService<IMediator>();
    var portalInvitation = Guid.NewGuid();
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.WorkExperienceReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var verifyResponse = await bus.Send(new VerifyInviteTokenCommand(packingResponse.verificationLink.Split('/')[2]), CancellationToken.None);
    verifyResponse!.portalInvitation.ShouldBe(portalInvitation);
    verifyResponse.inviteType.ShouldBe(InviteType.WorkExperienceReference);
  }
}
