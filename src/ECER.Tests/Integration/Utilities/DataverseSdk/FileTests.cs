﻿using Bogus;
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
    private readonly List<ECER_File> testFiles = [];
    private readonly IConfigurationRoot configuration;

    public FileTests()
    {
        var configBuilder = new ConfigurationBuilder().AddUserSecrets(typeof(Program).Assembly);
        configuration = configBuilder.Build();
    }

    [Theory]
    [InlineData(1 * 1024 * 1024)]
    [InlineData(4 * 1024 * 1024)]
    [InlineData(8 * 1024 * 1024)]
    public async Task CanStoreDocumentInDataverse(int documentSize)
    {
        var file = CreateNewFile();
        var content = await GenerateTestFile(documentSize);
        var fileId = await dataverseContext.UploadFile(file, ECER_File.Fields.ECER_DocumentFile, content, default);
        fileId.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task CanRetrieveDocumentFromDataverse()
    {
        var fileWithDocument = dataverseContext.ECER_FileSet.First(f => f.ECER_DocumentFile != null);
        var content = await dataverseContext.DownloadFile(fileWithDocument, ECER_File.Fields.ECER_DocumentFile, default);
        content.ShouldNotBeNull();
    }

    [Fact]
    public async Task CanDeleteDocumentFromDataverse()
    {
        var fileWithDocumentPreDelete = dataverseContext.ECER_FileSet.First(f => f.ECER_DocumentFile != null);
        await dataverseContext.DeleteFile(fileWithDocumentPreDelete, ECER_File.Fields.ECER_DocumentFile, default);
        dataverseContext.ClearChanges();
        var fileWithDocumentPostDelete = dataverseContext.ECER_FileSet.SingleOrDefault(f => f.ECER_FileId == fileWithDocumentPreDelete.ECER_FileId);
        fileWithDocumentPostDelete.ShouldNotBeNull().ECER_DocumentFile.ShouldBeNull();
    }

    [Fact]
    public async Task CanStoreImageInDataverse()
    {
        var file = CreateNewFile();
        var content = await GenerateTestImage();
        var fileId = await dataverseContext.UploadFile(file, ECER_File.Fields.ECER_ImageFile, content, default);
        fileId.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task CanRetrieveImageFromDataverse()
    {
        var fileWithDocument = dataverseContext.ECER_FileSet.First(f => f.ECER_ImageFile != null);
        var content = await dataverseContext.DownloadFile(fileWithDocument, ECER_File.Fields.ECER_ImageFile, default);
        content.ShouldNotBeNull();
    }

    [Fact]
    public async Task CanDeleteImageFromDataverse()
    {
        var fileWithDocumentPreDelete = dataverseContext.ECER_FileSet.First(f => f.ECER_ImageFile != null);
        await dataverseContext.DeleteFile(fileWithDocumentPreDelete, ECER_File.Fields.ECER_ImageFile, default);
        dataverseContext.ClearChanges();
        var fileWithDocumentPostDelete = dataverseContext.ECER_FileSet.SingleOrDefault(f => f.ECER_FileId == fileWithDocumentPreDelete.ECER_FileId);
        fileWithDocumentPostDelete.ShouldNotBeNull().ECER_ImageFile.ShouldBeNull();
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

    private ECER_File CreateNewFile()
    {
        var file = new ECER_File
        {
            ECER_FileId = Guid.NewGuid(),
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