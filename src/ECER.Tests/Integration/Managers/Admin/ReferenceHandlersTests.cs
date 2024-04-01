using ECER.Managers.Admin.Contract.References;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using ReferenceType = ECER.Managers.Admin.Contract.References.ReferenceType;

namespace ECER.Tests.Integration.Managers.Admin;

public class ReferenceHandlersTests : ApiWebAppScenarioBase
{
  public ReferenceHandlersTests(ITestOutputHelper output, ApiWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task CanTransform()
  {
    var bus = Host.Services.GetRequiredService<IMediator>();
    var response = await bus.Send(new GenerateReferenceLinkCommand(Guid.NewGuid(), ReferenceType.CharacterReference), CancellationToken.None);
    response.ShouldNotBeNull();
  }

  [Fact]
  public async Task CanUnTransform()
  {
    var bus = Host.Services.GetRequiredService<IMediator>();
    var guid = Guid.NewGuid();
    var packingResponse = await bus.Send(new GenerateReferenceLinkCommand(guid, ReferenceType.WorkExperienceReference), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var unpackingResponse = await bus.Send(new UnpackReferenceLinkCommand(guid, packingResponse.referenceLink), CancellationToken.None);
    unpackingResponse.portalInvitation.ShouldBe(guid);
    unpackingResponse.referenceType.ShouldBe(ReferenceType.WorkExperienceReference);
  }
}
