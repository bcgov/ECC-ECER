using ECER.Managers.Registry.Contract.Communications;
using ECER.Resources.Accounts.Communications;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;
using CommunicationStatus = ECER.Resources.Accounts.Communications.CommunicationStatus;
using UserCommunicationQuery = ECER.Resources.Accounts.Communications.UserCommunicationQuery;

namespace ECER.Tests.Integration.Resources.Accounts.Communications;

[IntegrationTest]
public class CertificationsRepositoryTests : RegistryPortalWebAppScenarioBase
{
  private readonly ICommunicationRepository repository;

  public CertificationsRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<ICommunicationRepository>();
  }

  [Fact]
  public async Task QueryCommunications_ById_Found()
  {
    // Arrange
    var communicationId = Fixture.communicationOneId;


    // Act
    var communications = await repository.Query(new UserCommunicationQuery { ById = communicationId });

    // Assert
    communications.ShouldHaveSingleItem();
  }

  [Fact]
  public async Task SeenCommunications_MarkAsSeen()
  {
    var communicationId = Fixture.communicationOneId;

    // Ensure test message is not "seen"
    var communications = await repository.Query(new UserCommunicationQuery { ById = communicationId });
    communications.ShouldHaveSingleItem();
    communications.First().Acknowledged.ShouldBeFalse();
    communications.First().Status.ShouldBe(CommunicationStatus.NotifiedRecipient);

    // Act
    await repository.MarkAsSeen(communicationId, default);
    var seenCommunications = await repository.Query(new UserCommunicationQuery { ById = communicationId });

    // Assert communication has been marked "seen"
    seenCommunications.ShouldHaveSingleItem();
    seenCommunications.First().Acknowledged.ShouldBeTrue();
    seenCommunications.First().Status.ShouldBe(CommunicationStatus.Acknowledged);
  }
}
