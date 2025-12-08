using Alba;
using Bogus;
using ECER.Clients.PSPPortal.Server.Communications;
using ECER.Managers.Registry.Contract.Communications;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

public class CommunicationsTests : PspPortalWebAppScenarioBase
{
  public CommunicationsTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }
  
  [Fact]
  public async Task GetCommunications_ReturnsParentCommunications_byPage()
  {
    var communicationsResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url("/api/messages?page=1&&pageSize=10");
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
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/messages/{this.Fixture.communicationOneId}?page=1&&pageSize=10");
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
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url("/api/messages/status");
      _.StatusCodeShouldBeOk();
    });

    var communicationsStatus = await communicationsStatusResponse.ReadAsJsonAsync<CommunicationsStatusResults>();
    communicationsStatus.ShouldNotBeNull();
  }

  [Fact]
  public async Task SendMessage_ReturnsCommunicationId()
  {
    var communication = CreateNewReply();

    var sendMessageResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Post.Json(new SendMessageRequest(communication)).ToUrl($"/api/messages");
      _.StatusCodeShouldBeOk();
    });

    var sendMessageResponseId = await sendMessageResponse.ReadAsJsonAsync<SendMessageResponse>();
    sendMessageResponseId!.CommunicationId.ShouldNotBeEmpty();
  }

  private Clients.PSPPortal.Server.Communications.Communication CreateNewReply()
  {
    var faker = new Faker("en_CA");
    var communication = new Clients.PSPPortal.Server.Communications.Communication()
    {
      Text = faker.Lorem.Paragraph()
    };

    communication.Id = this.Fixture.communicationOneId;
    return communication;
  }
}
