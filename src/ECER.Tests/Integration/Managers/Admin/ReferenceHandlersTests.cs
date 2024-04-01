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
    var response = await bus.Send(new ReferenceLinkQuery(Guid.NewGuid(), ReferenceType.CharacterReference), CancellationToken.None);
    response.ShouldNotBeNull();
  }
}
