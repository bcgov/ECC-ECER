//using Bogus;
//using ECER.Utilities.DataverseSdk.Model;
//using Microsoft.Extensions.Configuration;
//using Microsoft.PowerPlatform.Dataverse.Client;
//using Shouldly;
//using Xunit.Categories;

//namespace ECER.Tests.Integration.Utilities.ObjectStorage;

//[IntegrationTest]
//public class DataverseTests : IAsyncLifetime
//{
//  private EcerContext dataverseContext = null!;
//  private ServiceClient serviceClient = null!;
//  private readonly Faker faker = new();
//  private readonly List<Annotation> testEntities = [];
//  private readonly IConfigurationRoot configuration;

//  public DataverseTests()
//  {
//    var configBuilder = new ConfigurationBuilder().AddUserSecrets(typeof(Clients.RegistryPortal.Server.Program).Assembly);
//    configuration = configBuilder.Build();
//  }

//  [Theory(Skip = "Replacing with finalized solution")]
//  [InlineData(1 * 1024 * 1024)]
//  [InlineData(4 * 1024 * 1024)]
//  public async Task CanStoreDocumentInDataverse(int documentSize)
//  {
//    var entity = CreateContainerEntity();
//    var content = await faker.GenerateTestFile(documentSize);
//    var fileId = await dataverseContext.UploadFileAsync(entity, Annotation.Fields., new Memory<byte>(content.Content., default);
//    fileId.ShouldNotBeNullOrEmpty();
//  }

//  [Fact(Skip = "Replacing with finalized solution")]
//  public async Task CanRetrieveDocumentFromDataverse()
//  {
//    var entityWithDocument = dataverseContext.ecer_FileSet.First(f => f.ecer_DocumentFile != null);
//    var content = await dataverseContext.DownloadFileAsync(entityWithDocument, ecer_File.Fields.ecer_DocumentFile, default);
//    content.ShouldNotBeNull();
//  }

//  [Fact(Skip = "Replacing with finalized solution")]
//  public async Task CanDeleteDocumentFromDataverse()
//  {
//    var fileWithDocumentPreDelete = dataverseContext.ecer_FileSet.First(f => f.ecer_DocumentFile != null);
//    await dataverseContext.DeleteFileAsync(fileWithDocumentPreDelete, ecer_File.Fields.ecer_DocumentFile, default);
//    dataverseContext.ClearChanges();
//    var fileWithDocumentPostDelete = dataverseContext.ecer_FileSet.SingleOrDefault(f => f.ecer_FileId == fileWithDocumentPreDelete.ecer_FileId);
//    fileWithDocumentPostDelete.ShouldNotBeNull().ecer_DocumentFile.ShouldBeNull();
//  }

//  [Fact(Skip = "Replacing with finalized solution")]
//  public async Task CanStoreImageInDataverse()
//  {
//    var file = CreateContainerEntity();
//    var content = await faker.GenerateTestImage();
//    var fileId = await dataverseContext.UploadFileAsync(file, ecer_File.Fields.ecer_ImageFile, content, default);
//    fileId.ShouldNotBeNullOrEmpty();
//  }

//  [Fact(Skip = "Replacing with finalized solution")]
//  public async Task CanRetrieveImageFromDataverse()
//  {
//    var fileWithDocument = dataverseContext.AnnotationSet.First(f => f.ecer_ImageFile != null);
//    var content = await dataverseContext.DownloadFileAsync(fileWithDocument, Annotation.Fields.ecer_ImageFile, default);
//    content.ShouldNotBeNull();
//  }

//  [Fact(Skip = "Replacing with finalized solution")]
//  public async Task CanDeleteImageFromDataverse()
//  {
//    var fileWithDocumentPreDelete = dataverseContext.aa.First(f => f.ecer_ImageFile != null);
//    await dataverseContext.DeleteFileAsync(fileWithDocumentPreDelete, ecer_File.Fields.ecer_ImageFile, default);
//    dataverseContext.ClearChanges();
//    var fileWithDocumentPostDelete = dataverseContext.ecer_FileSet.SingleOrDefault(f => f.ecer_FileId == fileWithDocumentPreDelete.ecer_FileId);
//    fileWithDocumentPostDelete.ShouldNotBeNull().ecer_ImageFile.ShouldBeNull();
//  }

//  public async Task InitializeAsync()
//  {
//    await Task.CompletedTask;
//    serviceClient = new ServiceClient(configuration.GetValue<string>("Dataverse:ConnectionString"));
//    dataverseContext = new EcerContext(serviceClient);
//  }

//  public async Task DisposeAsync()
//  {
//    await Task.CompletedTask;
//    foreach (var file in testEntities)
//    {
//      dataverseContext.DeleteObject(file);
//    }
//    dataverseContext.Dispose();
//    serviceClient.Dispose();
//  }

//  private Annotation CreateContainerEntity()
//  {
//    var entity = new Annotation
//    {
//      AnnotationId = Guid.NewGuid(),
//    };

//    dataverseContext.AddObject(entity);
//    dataverseContext.SaveChanges();
//    dataverseContext.ClearChanges();

//    return entity;
//  }
//}
