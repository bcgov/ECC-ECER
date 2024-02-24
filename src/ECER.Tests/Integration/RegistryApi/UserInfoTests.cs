using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Users;
using ECER.Utilities.Security;
using Shouldly;
using System.Net;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.Api;

public class UserInfoTests : RegistryPortalWebAppScenarioBase
{
  public UserInfoTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task PostUserInfo_NewBceidUser_Ok()
  {
    await Host.Scenario(_ =>
    {
      _.WithNewUser(new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bceidbasic"));
      _.Post.Json(CreateNewUser()).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task PostUserInfo_NewBcscUser_Ok()
  {
    await Host.Scenario(_ =>
    {
      _.WithNewUser(new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc"));
      _.Post.Json(CreateNewUser()).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task GetUserInfo_ExistingBCSCUser_UserProfile()
  {
    var identity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc");
    var request = CreateNewUser();
    await Host.Scenario(_ =>
    {
      _.WithNewUser(identity);
      _.Post.Json(request).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var response = await Host.Scenario(_ =>
    {
      _.WithNewUser(identity);
      _.Get.Url("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var userProfile = (await response.ReadAsJsonAsync<UserInfo>()).ShouldNotBeNull();
    userProfile.ShouldBe(request);
  }

  [Fact]
  public async Task GetUserInfo_NonExistingBCSCUser_Empty()
  {
    var identity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc");

    var response = await Host.Scenario(_ =>
    {
      _.WithNewUser(identity);
      _.Get.Url("/api/userinfo");
      _.StatusCodeShouldBe(HttpStatusCode.NotFound);
    });

    response.Context.Response.ContentLength.ShouldBeNull();
  }

  private UserInfo CreateNewUser()
  {
    var userProfile = new Faker<UserInfo>("en_CA")
    .CustomInstantiator(f => new UserInfo(
        f.Person.FirstName,
        f.Person.LastName,
        DateOnly.FromDateTime(f.Person.DateOfBirth),
        f.Person.Email,
        f.Person.Phone
        ));

    return userProfile.Generate();
  }
}
