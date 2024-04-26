using Alba;
using ECER.Clients.RegistryPortal.Server.Communications;
using ECER.Managers.Registry.Contract.Communications;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class CommunicationsTests : RegistryPortalWebAppScenarioBase
{
  public CommunicationsTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetCommunications_ReturnsCommunications()
  {
    var communicationsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url("/api/messages");
      _.StatusCodeShouldBeOk();
    });

    var communications = await communicationsResponse.ReadAsJsonAsync<Clients.RegistryPortal.Server.Communications.Communication[]>();
    communications.ShouldNotBeNull();
  }


  [Fact]
  public async Task GetMessageStatus_ReturnsStatus()
  {
    var communicationsStatusResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url("/api/messages/status");
      _.StatusCodeShouldBeOk();
    });

    var communicationsStatus = await communicationsStatusResponse.ReadAsJsonAsync<CommunicationsStatusResults>();
    communicationsStatus.ShouldNotBeNull();
  }
  
  
  [Fact]
  public async Task SeenCommunication_ReturnId()
  {
    var communicationSeenResponse =  await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new CommunicationSeenRequest(Fixture.communicationThreeId)).ToUrl($"/api/messages/{Fixture.communicationThreeId}/seen");
      _.StatusCodeShouldBeOk();
    });
    
    (await communicationSeenResponse.ReadAsJsonAsync<CommunicationResponse>()).ShouldNotBeNull().CommunicationId.ShouldBe(Fixture.communicationThreeId);
  }
}
