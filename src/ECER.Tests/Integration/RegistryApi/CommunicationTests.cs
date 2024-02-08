using Alba;
using Xunit;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Newtonsoft.Json;
using Wolverine;

namespace ECER.Tests.Integration.RegistryApi;

public class CommunicationsTests : RegistryPortalWebAppScenarioBase
{
  public CommunicationsTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetCommunications_ReturnsCommunications()
  {
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      var response = _.Get.Url("/api/messages");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task GetMessageStatus_ReturnsStatus()
  {
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url("/api/messages/status");
      _.StatusCodeShouldBeOk();
    });
  }
}
