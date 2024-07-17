using System.Net.Http.Headers;
using Alba;
using Bogus;
using Xunit.Abstractions;
using Xunit.Categories;
using Xunit.Sdk;

namespace ECER.Tests.Integration.RegistryApi;

public class FileTests : RegistryPortalWebAppScenarioBase
{
  private readonly Faker faker = new Faker("en_CA");

  public FileTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  { }

  [Fact]
  [Category("VPN")]
  public async Task CanUploadFile()
  {
    var fileLength = 1041;
    var testFile = await faker.GenerateTestFile(fileLength);
    var testFileId = Guid.NewGuid().ToString();
    var testFolder = "integrationtests";
    var testTags = "tag1=1,tag2=2";
    var testClassification = "test-classification";
    using var content = new StreamContent(testFile.Content);
    content.Headers.ContentType = new MediaTypeHeaderValue(testFile.ContentType);

    using var formData = new MultipartFormDataContent
        {
          { content, "file", testFile.FileName }
        };

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.WithRequestHeader("file-classification", testClassification);
      _.WithRequestHeader("file-tag", testTags);
      _.WithRequestHeader("file-folder", testFolder);
      _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  [Category("VPN")]
  public async Task CanDeleteFile()
  {
    var testFileId = Guid.NewGuid().ToString();

    // Assuming the file upload step is successful
    var fileLength = 1041;
    var testFile = await faker.GenerateTestFile(fileLength);
    var testFolder = "integrationtests";
    var testTags = "tag1=1,tag2=2";
    var testClassification = "test-classification";
    using var content = new StreamContent(testFile.Content);
    content.Headers.ContentType = new MediaTypeHeaderValue(testFile.ContentType);

    using var formData = new MultipartFormDataContent
        {
          { content, "file", testFile.FileName }
        };

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.WithRequestHeader("file-classification", testClassification);
      _.WithRequestHeader("file-tag", testTags);
      _.WithRequestHeader("file-folder", testFolder);
      _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });

    // Now, delete the file
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Delete.Url($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  [Category("VPN")]
  public async Task CanDownloadCertificateFile()
  {
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url($"/api/files/certificate/854c5086-c542-ef11-a316-000d3af45c14");
      _.StatusCodeShouldBeOk();
    });
  }
}
