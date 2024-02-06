using ECER.Resources.Accounts.Communications;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.Resources.Communications;

[IntegrationTest]
public class CommunicationRepositoryTests : RegistryPortalWebAppScenarioBase
{
  private readonly ICommunicationRepository repository;

  public CommunicationRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Host.Services.GetRequiredService<ICommunicationRepository>();
  }

  [Fact]
  public async Task QueryCommunications_ById_Found()
  {
    // Arrange
    var communicationId = "d2cef005-33b6-ee11-a569-000d3af4fd74"; // Example ID

    // Act
    var communications = await repository.Query(new CommunicationQuery { ById = communicationId });

    // Assert
    communications.ShouldHaveSingleItem();
  }
}
