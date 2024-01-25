using System.Net;
using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Users;
using ECER.Utilities.Security;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.Api;

public class UserInfoTests : RegistryPortalWebAppScenarioBase
{
  public UserInfoTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task PostUserProfile_NewBceidUser_Ok()
  {
    await Host.Scenario(_ =>
    {
      _.WithNewUser(new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bceidbasic"));
      _.Post.Json(CreateNewUserRequest()).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task PostUserProfile_NewBcscUser_Ok()
  {
    await Host.Scenario(_ =>
    {
      _.WithNewUser(new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc"));
      _.Post.Json(CreateNewUserRequest()).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task GetUserProfile_ExistingBCSCUser_UserProfile()
  {
    var identity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc");
    var request = CreateNewUserRequest();
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

    var userProfile = (await response.ReadAsJsonAsync<UserInfoResponse>()).ShouldNotBeNull().UserInfo;
    userProfile.ShouldBe(request.Profile);
  }

  [Fact]
  public async Task GetUserProfile_NonExistingBCSCUser_Empty()
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

  private NewUserRequest CreateNewUserRequest()
  {
    var address = new Faker<Address>("en_CA")
        .CustomInstantiator(f => new Address(
            f.Address.StreetAddress(),
            null,
            f.Address.City(),
            f.Address.ZipCode(),
            f.Address.State(), f.Address.Country()
            ));

    var userProfile = new Faker<UserProfile>("en_CA")
        .CustomInstantiator(f => new UserProfile(
            f.Person.FirstName,
            f.Person.LastName,
            DateOnly.FromDateTime(f.Person.DateOfBirth),
            f.Person.Email,
            f.Person.Phone,
            address.Generate(),
            address.Generate().OrNull(f)
            ));

    return new Faker<NewUserRequest>("en_CA")
        .RuleFor(req => req.Profile, f => userProfile.Generate());
  }
}
