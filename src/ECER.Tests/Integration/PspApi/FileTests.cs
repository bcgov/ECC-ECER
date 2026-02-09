using Alba;
using Bogus;
using System.Net.Http.Headers;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.PspApi;

public class FileTests : PspPortalWebAppScenarioBase
{
  private readonly Faker faker = new Faker("en_CA");

  public FileTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  { }

  [Fact]
  [Category("Internal")]
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
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.WithRequestHeader("file-classification", testClassification);
      _.WithRequestHeader("file-tag", testTags);
      _.WithRequestHeader("file-folder", testFolder);
      _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  [Category("Internal")]
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
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.WithRequestHeader("file-classification", testClassification);
      _.WithRequestHeader("file-tag", testTags);
      _.WithRequestHeader("file-folder", testFolder);
      _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });

    // Now, delete the file
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Delete.Url($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });
  }
}
