using Bogus;
using ECER.Infrastructure.Common;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Categories;

namespace ECER.Tests.Integration.Utilities.ObjectStorage;

[IntegrationTest, Category("requires_vpn")]
public class S3ObjectStorageTests : IAsyncLifetime
{
  private readonly Faker faker = new Faker();

  private IObjecStorageProvider storageProvider => scope.ServiceProvider.GetRequiredService<IObjecStorageProvider>();
  private string bucket = null!;
  private IServiceScope scope = null!;

  [Theory]
  [InlineData(1 * 1024 * 1024)]
  [InlineData(4 * 1024 * 1024)]
  public async Task CanStoreObject(int documentSize)
  {
    var file = await faker.GenerateTestFile(documentSize);
    await Should.NotThrowAsync(async () => await storageProvider.StoreAsync(new S3Descriptor(bucket, file.FileName, "test"), new FileObject(file.FileName, file.ContentType, file.Content, null), CancellationToken.None));
  }

  [Fact]
  public async Task CanGetObject()
  {
    var file = await faker.GenerateTestFile(1000);
    var content = await file.Content.CloneAsync();
    var descriptor = new S3Descriptor(bucket, file.FileName, "test");
    await storageProvider.StoreAsync(descriptor, new FileObject(file.FileName, file.ContentType, file.Content, null), CancellationToken.None);
    var storedFile = (await storageProvider.GetAsync(descriptor, CancellationToken.None)).ShouldNotBeNull();
    storedFile.FileName.ShouldBe(file.FileName);
    storedFile.ContentType.ShouldBe(file.ContentType);

    var storedContent = await storedFile.Content.CloneAsync();
    storedContent.ShouldBe(content);
  }

  [Fact]
  public async Task CanDeleteFile()
  {
    var file = await faker.GenerateTestFile(1000);
    var descriptor = new S3Descriptor(bucket, file.FileName, "test");
    await storageProvider.StoreAsync(descriptor, new FileObject(file.FileName, file.ContentType, file.Content, null), CancellationToken.None);
    await Should.NotThrowAsync(async () => await storageProvider.DeleteAsync(descriptor, CancellationToken.None));
    (await storageProvider.GetAsync(descriptor, CancellationToken.None)).ShouldBeNull();
  }

  [Fact]
  public async Task CanCopyFile()
  {
    var file = await faker.GenerateTestFile(1000);
    var originalContent = await file.Content.CloneAsync();
    var fromDescriptor = new S3Descriptor(bucket, file.FileName, "test/source");
    var toDescriptor = new S3Descriptor(bucket, file.FileName, "test/destination");
    await storageProvider.StoreAsync(fromDescriptor, new FileObject(file.FileName, file.ContentType, file.Content, null), CancellationToken.None);

    (await storageProvider.GetAsync(toDescriptor, CancellationToken.None)).ShouldBeNull();

    await storageProvider.CopyAsync(fromDescriptor, toDescriptor, CancellationToken.None);

    (await storageProvider.GetAsync(fromDescriptor, CancellationToken.None)).ShouldNotBeNull();
    var copiedFile = (await storageProvider.GetAsync(toDescriptor, CancellationToken.None)).ShouldNotBeNull();

    var copiedContent = await copiedFile.Content.CloneAsync();
    copiedContent.ShouldBe(originalContent);
  }

  [Fact]
  public async Task CanMoveFile()
  {
    var file = await faker.GenerateTestFile(1000);
    var originalContent = await file.Content.CloneAsync();
    var fromDescriptor = new S3Descriptor(bucket, file.FileName, "test/source");
    var toDescriptor = new S3Descriptor(bucket, file.FileName, "test/destination");
    await storageProvider.StoreAsync(fromDescriptor, new FileObject(file.FileName, file.ContentType, file.Content, null), CancellationToken.None);

    (await storageProvider.GetAsync(toDescriptor, CancellationToken.None)).ShouldBeNull();

    await storageProvider.MoveAsync(fromDescriptor, toDescriptor, CancellationToken.None);

    (await storageProvider.GetAsync(fromDescriptor, CancellationToken.None)).ShouldBeNull();
    var movedFile = (await storageProvider.GetAsync(toDescriptor, CancellationToken.None)).ShouldNotBeNull();

    var copiedContent = await movedFile.Content.CloneAsync();
    copiedContent.ShouldBe(originalContent);
  }

  public async Task InitializeAsync()
  {
    await Task.CompletedTask;

    var configBuilder = new ConfigurationBuilder().AddUserSecrets(typeof(Clients.RegistryPortal.Server.Program).Assembly);
    var configuration = configBuilder.Build();
    var services = new ServiceCollection();
    var configurer = new Configurer();

    configurer.Configure(new ConfigurationContext(services, configuration));
    scope = services.BuildServiceProvider().CreateScope();
    bucket = configuration.GetValue<string>("objectStorage:bucketName")!;
  }

  public async Task DisposeAsync()
  {
    await Task.CompletedTask;
    scope.Dispose();
  }
}
