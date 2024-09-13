using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Users;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Utilities.Security;
using Shouldly;
using System.Net;
using System.Reflection.Metadata;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

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
    userProfile.FirstName.ShouldBe(request.FirstName);
    userProfile.LastName.ShouldBe(request.LastName);
    userProfile.DateOfBirth.ShouldBe(request.DateOfBirth);
    userProfile.Phone.ShouldBe(request.Phone);
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

  // Integration test to simulate creating a new registrant with no registration number
  [Fact]
  public async Task Handle_ShouldCreateNewRegistrant_WhenNoRegistrationNumberProvided()
  {
    var userIdentity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc");
    var newUser = CreateNewUser();

    var result = await Host.Scenario(_ =>
    {
      _.WithNewUser(userIdentity);
      _.Post.Json(newUser).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var response = await Host.Scenario(_ =>
    {
      _.WithNewUser(userIdentity);
      _.Get.Url("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var registeredUser = (await response.ReadAsJsonAsync<UserInfo>()).ShouldNotBeNull();
    registeredUser.Email.ShouldBe(newUser.Email);
  }

  [Fact]
  public async Task Handle_ShouldUpdateExistingRegistrant_WhenRegistrationNumberProvided()
  {
    var userIdentity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc");
    var newUser = CreateNewUser();
    newUser.RegistrationNumber = "1234";

    await Host.Scenario(_ =>
    {
      _.WithNewUser(userIdentity);
      _.Post.Json(newUser).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    newUser = new UserInfo(
    newUser.FirstName,
    newUser.LastName,
    newUser.FirstName,
    newUser.DateOfBirth,
    "updatedemail@test.com",
    newUser.Phone
    );
    newUser.RegistrationNumber = "1234";

    var newUserIdentity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc");
    await Host.Scenario(_ =>
        {
          _.WithNewUser(newUserIdentity);
          _.Post.Json(newUser).ToUrl("/api/userinfo");
          _.StatusCodeShouldBeOk();
        });

    var response = await Host.Scenario(_ =>
    {
      _.WithNewUser(userIdentity);
      _.Get.Url("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var updatedUser = (await response.ReadAsJsonAsync<UserInfo>()).ShouldNotBeNull();
    updatedUser.Email.ShouldBe("updatedemail@test.com");
  }

  [Fact]
  public async Task Handle_ShouldCreateNewRegistrant_WhenNoMatchingContact()
  {
    var userIdentity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc");
    var newUser = CreateNewUser();

    await Host.Scenario(_ =>
    {
      _.WithNewUser(userIdentity);
      _.Post.Json(newUser).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var response = await Host.Scenario(_ =>
    {
      _.WithNewUser(userIdentity);
      _.Get.Url("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var registeredUser = (await response.ReadAsJsonAsync<UserInfo>()).ShouldNotBeNull();
    registeredUser.Email.ShouldBe(newUser.Email);
  }

  private UserInfo CreateNewUser()
  {
    var userProfile = new Faker<UserInfo>("en_CA")
    .CustomInstantiator(f => new UserInfo(
        f.Person.FirstName,
        f.Person.LastName,
        f.Person.FirstName,
        DateOnly.FromDateTime(f.Person.DateOfBirth),
        f.Person.Email,
        f.Person.Phone
        ));

    return userProfile.Generate();
  }
}
