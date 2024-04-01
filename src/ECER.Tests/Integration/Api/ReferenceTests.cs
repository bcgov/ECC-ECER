using ECER.Managers.Admin.Contract.References;
using Xunit.Abstractions;
using ReferenceType = ECER.Managers.Admin.Contract.References.ReferenceType;

namespace ECER.Tests.Integration.Api;

public class ReferenceTests : ApiWebAppScenarioBase
{
  public ReferenceTests(ITestOutputHelper output, ApiWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GenerateReferenceLink_WithValidInfo_ReturnsOk()
  {
    var validRequest = new GenerateReferenceLinkRequest(Guid.NewGuid(), ReferenceType.CharacterReference);
    await Host.Scenario(_ =>
    {
      _.Post.Json(validRequest).ToUrl("/api/references");
      _.StatusCodeShouldBe(200);
    });
  }
}
