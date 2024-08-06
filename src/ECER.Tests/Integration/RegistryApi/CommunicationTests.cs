using Alba;
using Bogus;
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
  public async Task GetCommunications_ReturnsParentCommunications_byPage()
  {
    var communicationsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url("/api/messages?page=2&&pageSize=20");
      _.StatusCodeShouldBeOk();
    });

    var communications = await communicationsResponse.ReadAsJsonAsync<GetMessagesResponse>();
    communications!.Communications.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetCommunications_ReturnsChildCommunications_byPage()
  {
    var communicationsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url($"/api/messages/{this.Fixture.communicationOneId}?page=2&&pageSize=20");
      _.StatusCodeShouldBeOk();
    });

    var communications = await communicationsResponse.ReadAsJsonAsync<GetMessagesResponse>();
    communications!.Communications.ShouldNotBeNull();
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

  [Fact(Skip = "Adding registrant reference to communication causes duplicate key in all tests, without that this test fails")]
  public async Task SeenCommunication_ReturnId()
  {
    var communicationSeenResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new CommunicationSeenRequest(Fixture.communicationTwoId)).ToUrl($"/api/messages/{Fixture.communicationTwoId}/seen");
      _.StatusCodeShouldBeOk();
    });

    (await communicationSeenResponse.ReadAsJsonAsync<CommunicationResponse>()).ShouldNotBeNull().CommunicationId.ShouldBe(Fixture.communicationTwoId);
  }

  [Fact]
  public async Task SendMessage_ReturnsCommunicationId()
  {
    var communication = CreateNewReply();

    var sendMessageResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Post.Json(new SendMessageRequest(communication)).ToUrl($"/api/messages");
      _.StatusCodeShouldBeOk();
    });

    var sendMessageResponseId = await sendMessageResponse.ReadAsJsonAsync<SendMessageResponse>();
    sendMessageResponseId!.CommunicationId.ShouldNotBeEmpty();
  }

  private Clients.RegistryPortal.Server.Communications.Communication CreateNewReply()
  {
    var faker = new Faker("en_CA");
    var communication = new Clients.RegistryPortal.Server.Communications.Communication()
    {
      Text = faker.Lorem.Paragraph()
    };

    communication.Id = this.Fixture.communicationOneId;
    return communication;
  }
}
