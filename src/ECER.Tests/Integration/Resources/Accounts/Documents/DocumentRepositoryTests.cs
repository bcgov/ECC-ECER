using ECER.Resources.Accounts.Documents;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.Resources.Accounts.Documents;

[IntegrationTest]
public class DocumentRepositoryTests : RegistryPortalWebAppScenarioBase
{
  private readonly IDocumentRepository repository;

  public DocumentRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<IDocumentRepository>();
  }

  [Fact]
  public async Task QueryDocuments_ById_Found()
  {
    // Arrange
    var documentId = Fixture.documentOneId;

    // Act
    var documents = await repository.Query(new UserDocumentQuery { ById = documentId });

    // Assert
    documents.ShouldHaveSingleItem();
    documents.First().Id.ShouldBe(documentId);
  }

  [Fact]
  public async Task QueryDocuments_ByRegistrantId_Found()
  {
    // Arrange
    var registrantId = Fixture.AuthenticatedBcscUserId;

    // Act
    var documents = await repository.Query(new UserDocumentQuery { ByRegistrantId = registrantId });

    // Assert
    documents.ShouldNotBeEmpty();
  }
}
