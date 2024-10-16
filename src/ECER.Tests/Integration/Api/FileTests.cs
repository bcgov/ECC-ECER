using Alba;
using Bogus;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.Api;

public class FileTests : ApiWebAppScenarioBase
{
  private readonly Faker faker = new Faker("en_CA");

  public FileTests(ITestOutputHelper output, ApiWebAppFixture fixture) : base(output, fixture)
  {
  }

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

    var uploadResponse = await Host.Scenario(_ =>
    {
      _.WithRequestHeader("file-classification", testClassification);
      _.WithRequestHeader("file-tag", testTags);
      _.WithRequestHeader("file-folder", testFolder);
      _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });

    uploadResponse.Context.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);

    var response = await Host.Scenario(_ =>
    {
      _.WithRequestHeader("file-folder", testFolder);
      _.Get.Url($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });

    response.Context.Response.Headers["Content-Type"].ToString().ShouldBe(testFile.ContentType);
    response.Context.Response.Headers["Content-Disposition"].ToString().Split(';').Select(s => s.Trim()).ShouldContain($"filename={testFile.FileName}");
    response.Context.Response.Headers["file-folder"].ToString().ShouldBe(testFolder);
    response.Context.Response.Headers["file-tag"].ToString().ShouldBe(testTags);
    response.Context.Response.Headers["file-classification"].ToString().ShouldBe(testClassification);

    var returnedFile = await response.ReadAsTextAsync();
    returnedFile.Length.ShouldBe(fileLength);
    testFile.Content.Position = 0;
    using var sw = new StreamReader(testFile.Content);
    returnedFile.ShouldBe(await sw.ReadToEndAsync());
  }
}
