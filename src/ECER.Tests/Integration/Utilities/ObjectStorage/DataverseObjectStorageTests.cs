using Bogus;
using ECER.Infrastructure.Common;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.Dataverse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Shouldly;
using Xunit.Categories;

namespace ECER.Tests.Integration.Utilities.ObjectStorage;

[IntegrationTest]
public class DataverseObjectStorageTests : IAsyncLifetime
{
  private readonly Faker faker = new Faker();

  private IObjecStorageProvider storageProvider => scope.ServiceProvider.GetRequiredService<IObjecStorageProvider>();
  private IOrganizationServiceAsync organizationService => scope.ServiceProvider.GetRequiredService<IOrganizationServiceAsync>();
  private IServiceScope scope = null!;

  [Theory(Skip = "not ready")]
  [InlineData(1 * 1024 * 1024)]
  [InlineData(4 * 1024 * 1024)]
  public async Task CanStoreObject(int documentSize)
  {
    var file = await faker.GenerateTestFile(documentSize);
    var entity = await CreateObjectEntity();
    await Should.NotThrowAsync(async () => await storageProvider.StoreAsync(new DataverseDescriptor(entity, Annotation.Fields.DocumentBody), new FileObject(file.FileName, file.ContentType, file.Content), CancellationToken.None));
  }

  [Fact(Skip = "not ready")]
  public async Task CanGetObject()
  {
    var file = await faker.GenerateTestFile(1000);
    var content = await file.Content.CloneAsync();
    var entity = await CreateObjectEntity();
    var descriptor = new DataverseDescriptor(entity);
    await storageProvider.StoreAsync(descriptor, new FileObject(file.FileName, file.ContentType, file.Content), CancellationToken.None);
    var storedFile = (await storageProvider.GetAsync(descriptor, CancellationToken.None)).ShouldNotBeNull();
    storedFile.FileName.ShouldBe(file.FileName);
    storedFile.ContentType.ShouldBe(file.ContentType);

    var storedContent = await storedFile.Content.CloneAsync();
    storedContent.ShouldBe(content);
  }

  public async Task InitializeAsync()
  {
    await Task.CompletedTask;

    var configBuilder = new ConfigurationBuilder().AddUserSecrets(typeof(Clients.RegistryPortal.Server.Program).Assembly);
    var configuration = configBuilder.Build();
    var configurationWithoutObjectStorage = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
    {
      ["dataverse:ConnectionString"] = configuration.GetValue<string>("dataverse:ConnectionString")!,
    }).Build();
    var services = new ServiceCollection().AddLogging();
    var dataverseConfigurer = new ECER.Utilities.DataverseSdk.Configurer();
    var objectStorageConfigurer = new ECER.Utilities.ObjectStorage.Providers.Configurer();
    var cfgContext = new ConfigurationContext(services, configurationWithoutObjectStorage);

    dataverseConfigurer.Configure(cfgContext);
    objectStorageConfigurer.Configure(cfgContext);
    scope = services.BuildServiceProvider().CreateScope();
  }

  public async Task DisposeAsync()
  {
    await Task.CompletedTask;
    scope.Dispose();
  }

  private async Task<Entity> CreateObjectEntity()
  {
    var note = new Annotation { Id = Guid.NewGuid() };
    await organizationService.CreateAsync(note);

    return note;
  }
}
