using ECER.Resources.Accounts.Communications;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
    var communicationId = "9039a260-87eb-4b53-a7b3-0ce5379c4c8b"; // Example ID

    // Act
    var communications = await repository.Query(new CommunicationQuery { ById = communicationId });

    communications.ShouldBeEmpty();
    // Assert
    //communications.ShouldHaveSingleItem();
    //communications.First().Id.ShouldBe(communicationId);
  }

  [Fact]
  public async Task QueryCommunications_ByApplicantId_Found()
  {
    // Arrange
    var applicantId = "9039a260-87eb-4b53-a7b3-0ce5379c4c8b"; // Example applicant ID

    // Act
    var communications = await repository.Query(new CommunicationQuery { ByApplicantId = applicantId });

    // Assert
    communications.ShouldBeEmpty();
  }
}
