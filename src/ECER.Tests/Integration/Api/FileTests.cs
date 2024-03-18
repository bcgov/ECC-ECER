using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using Alba;
using Bogus;
using ECER.Tests.Integration;
using JasperFx.Core;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests;

public class FileTests : ApiWebAppScenarioBase
{
    private readonly Faker faker = new Faker("en_CA");

    public FileTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
    {
    }

    [Fact]
    public async Task CanUploadFile()
    {
        var testFile = await faker.GenerateTestFile(1000);
        var testFileId = Guid.NewGuid().ToString();
        using var content = new StreamContent(testFile.Content);
        content.Headers.ContentType = new MediaTypeHeaderValue(testFile.ContentType);

        using var formData = new MultipartFormDataContent
        {
          { content, "files", testFile.FileName }
        };
        
        var _ = await Host.Scenario(_ =>
        {
            _.WithRequestHeader("file-classification", "test");
            _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
            _.StatusCodeShouldBeOk();
        });

        var response = await Host.Scenario(_ => {
            _.Get.Url($"/api/files/{testFileId}");
            _.StatusCodeShouldBeOk();
        });

        response.ShouldNotBeNull();
    }
}
