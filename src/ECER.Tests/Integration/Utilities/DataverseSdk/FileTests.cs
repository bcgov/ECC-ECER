using Bogus;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Shouldly;
using Xunit.Categories;

namespace ECER.Tests.Integration.Utilities.DataverseSdk;

[IntegrationTest]
public class FileTests : IAsyncLifetime
{
  private EcerContext dataverseContext = null!;
  private ServiceClient serviceClient = null!;
  private readonly Faker faker = new();
  private readonly HttpClient httpClient = new();
  private readonly List<ecer_File> testFiles = [];
  private readonly IConfigurationRoot configuration;

  public FileTests()
  {
    var configBuilder = new ConfigurationBuilder().AddUserSecrets(typeof(Clients.RegistryPortal.Server.Program).Assembly);
    configuration = configBuilder.Build();
  }

  [Theory(Skip = "Replacing with finalized solution")]
  [InlineData(1 * 1024 * 1024)]
  [InlineData(4 * 1024 * 1024)]
  public async Task CanStoreDocumentInDataverse(int documentSize)
  {
    var file = CreateNewFile();
    var content = await GenerateTestFile(documentSize);
    var fileId = await dataverseContext.UploadFileAsync(file, ecer_File.Fields.ecer_DocumentFile, content, default);
    fileId.ShouldNotBeNullOrEmpty();
  }

  [Fact(Skip = "Replacing with finalized solution")]
  public async Task CanRetrieveDocumentFromDataverse()
  {
    var fileWithDocument = dataverseContext.ecer_FileSet.First(f => f.ecer_DocumentFile != null);
    var content = await dataverseContext.DownloadFileAsync(fileWithDocument, ecer_File.Fields.ecer_DocumentFile, default);
    content.ShouldNotBeNull();
  }

  [Fact(Skip = "Replacing with finalized solution")]
  public async Task CanDeleteDocumentFromDataverse()
  {
    var fileWithDocumentPreDelete = dataverseContext.ecer_FileSet.First(f => f.ecer_DocumentFile != null);
    await dataverseContext.DeleteFileAsync(fileWithDocumentPreDelete, ecer_File.Fields.ecer_DocumentFile, default);
    dataverseContext.ClearChanges();
    var fileWithDocumentPostDelete = dataverseContext.ecer_FileSet.SingleOrDefault(f => f.ecer_FileId == fileWithDocumentPreDelete.ecer_FileId);
    fileWithDocumentPostDelete.ShouldNotBeNull().ecer_DocumentFile.ShouldBeNull();
  }

  [Fact(Skip = "Replacing with finalized solution")]
  public async Task CanStoreImageInDataverse()
  {
    var file = CreateNewFile();
    var content = await GenerateTestImage();
    var fileId = await dataverseContext.UploadFileAsync(file, ecer_File.Fields.ecer_ImageFile, content, default);
    fileId.ShouldNotBeNullOrEmpty();
  }

  [Fact(Skip = "Replacing with finalized solution")]
  public async Task CanRetrieveImageFromDataverse()
  {
    var fileWithDocument = dataverseContext.ecer_FileSet.First(f => f.ecer_ImageFile != null);
    var content = await dataverseContext.DownloadFileAsync(fileWithDocument, ecer_File.Fields.ecer_ImageFile, default);
    content.ShouldNotBeNull();
  }

  [Fact(Skip = "Replacing with finalized solution")]
  public async Task CanDeleteImageFromDataverse()
  {
    var fileWithDocumentPreDelete = dataverseContext.ecer_FileSet.First(f => f.ecer_ImageFile != null);
    await dataverseContext.DeleteFileAsync(fileWithDocumentPreDelete, ecer_File.Fields.ecer_ImageFile, default);
    dataverseContext.ClearChanges();
    var fileWithDocumentPostDelete = dataverseContext.ecer_FileSet.SingleOrDefault(f => f.ecer_FileId == fileWithDocumentPreDelete.ecer_FileId);
    fileWithDocumentPostDelete.ShouldNotBeNull().ecer_ImageFile.ShouldBeNull();
  }

  public async Task InitializeAsync()
  {
    await Task.CompletedTask;
    serviceClient = new ServiceClient(configuration.GetValue<string>("Dataverse:ConnectionString"));
    dataverseContext = new EcerContext(serviceClient);
  }

  public async Task DisposeAsync()
  {
    await Task.CompletedTask;
    foreach (var file in testFiles)
    {
      dataverseContext.DeleteObject(file);
    }
    dataverseContext.Dispose();
    serviceClient.Dispose();
  }

  private ecer_File CreateNewFile()
  {
    var file = new ecer_File
    {
      ecer_FileId = Guid.NewGuid(),
    };
    dataverseContext.AddObject(file);
    dataverseContext.SaveChanges();
    dataverseContext.ClearChanges();

    return file;
  }

  private async Task<FileContainer> GenerateTestFile(int size)
  {
    var fileName = faker.System.FileName(".txt");
    var content = await faker.Lorem.ByteArray(size);

    return new FileContainer(fileName, "plain/text", content);
  }

  private async Task<FileContainer> GenerateTestImage()
  {
    var fileName = faker.System.FileName(".jpg");
    var content = await (await httpClient.GetAsync(new Uri(faker.Image.LoremFlickrUrl()))).Content.ReadAsByteArrayAsync();

    return new FileContainer(fileName, "image/jpg", content);
  }
}
