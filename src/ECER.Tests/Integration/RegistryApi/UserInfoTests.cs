using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Users;
using ECER.Utilities.Security;
using Shouldly;
using System;
using System.Net;
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
  public async Task PostUserInfo_NewBceidUser_WithInvalidCountryCode_ShouldReturnOkAndCountryNameNull()
  {
    var identity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bceidbasic");
    var newUser = CreateNewUser();
    var addressWithInvalidCountry = new Faker<Address>("en_CA")
     .CustomInstantiator(f => new Address(
    f.Address.StreetAddress(),
    null,
    f.Address.City(),
    f.Address.ZipCode(),
    f.Address.State(), "invalid country code"
    ));
    newUser.ResidentialAddress = addressWithInvalidCountry;

    await Host.Scenario(_ =>
    {
      _.WithNewUser(identity);
      _.Post.Json(CreateNewUser()).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var response = await Host.Scenario(_ =>
    {
      _.WithNewUser(identity);
      _.Get.Url("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var userProfile = (await response.ReadAsJsonAsync<UserInfo>()).ShouldNotBeNull();
    userProfile.ResidentialAddress!.Country.ShouldBe(null);
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
    registeredUser.MailingAddress!.City.ShouldBe(registeredUser.MailingAddress.City);
    registeredUser.ResidentialAddress!.City.ShouldBe(registeredUser.ResidentialAddress.City);
  }

  [Fact]
  public async Task Handle_ShouldUpdateExistingRegistrant_WhenRegistrationNumberProvided()
  {
    var userIdentity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc");
    var newUser = CreateNewUser();
    newUser.RegistrationNumber = "1234567";
    await Host.Scenario(_ =>
   {
     _.WithNewUser(userIdentity);
     _.Post.Json(newUser).ToUrl("/api/userinfo");
     _.StatusCodeShouldBeOk();
   });
    var response = await Host.Scenario(_ =>
    {
      _.WithNewUser(userIdentity);
      _.Get.Json(newUser).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var registeredUser = (await response.ReadAsJsonAsync<UserInfo>()).ShouldNotBeNull();

    newUser = new UserInfo(
    newUser.LastName,
    newUser.DateOfBirth,
    "updatedemail@test.com",
    newUser.Phone
    )
    { FirstName = newUser.FirstName, GivenName = newUser.FirstName, ResidentialAddress = new Address("1007 Glen Dr", "", "Coquitlam", "V3B0B4", "BC", "CA") };
    newUser.RegistrationNumber = registeredUser.RegistrationNumber;

    var newUserIdentity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc");
    await Host.Scenario(_ =>
    {
      _.WithNewUser(newUserIdentity);
      _.Post.Json(newUser).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var updatedResponse = await Host.Scenario(_ =>
    {
      _.WithNewUser(userIdentity);
      _.Get.Url("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var updatedUser = (await updatedResponse.ReadAsJsonAsync<UserInfo>()).ShouldNotBeNull();
    updatedUser.Email.ShouldBe("updatedemail@test.com");
  }

  [Fact]
  public async Task Handle_ShouldCreateNewRegistrant_WhenNoMatchingContact()
  {
    var userIdentity = Fixture.AuthenticatedBcscUserIdentity;

    var existingUserProfile = await (await Host.Scenario(_ =>
    {
      _.WithNewUser(userIdentity);
      _.Get.Url("/api/userinfo");
      _.StatusCodeShouldBeOk();
    })).ReadAsJsonAsync<UserInfo>().ShouldNotBeNull();

    var userProfile = existingUserProfile with { LastName = Fixture.TestRunId, GivenName = Fixture.TestRunId, ResidentialAddress = new Address("1007 Glen Dr", "", "Coquitlam", "V3B0B4", "BC", "CA") };
    var newUserIdentity = new UserIdentity(Guid.NewGuid().ToString("N").ToUpperInvariant(), "bcsc");

    await Host.Scenario(_ =>
    {
      _.WithNewUser(newUserIdentity);
      _.Post.Json(userProfile).ToUrl("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var response = await Host.Scenario(_ =>
    {
      _.WithNewUser(newUserIdentity);
      _.Get.Url("/api/userinfo");
      _.StatusCodeShouldBeOk();
    });

    var registeredUser = (await response.ReadAsJsonAsync<UserInfo>()).ShouldNotBeNull();
    registeredUser.LastName.ShouldBe(userProfile.LastName);
  }

  private static UserInfo CreateNewUser()
  {
    var address = new Faker<Address>("en_CA")
    .CustomInstantiator(f => new Address(
        f.Address.StreetAddress(),
        null,
        f.Address.City(),
        f.Address.ZipCode(),
        f.Address.State(), "CA"
        ));

    var userProfile = new Faker<UserInfo>("en_CA")
    .CustomInstantiator(f => new UserInfo(
        f.Person.LastName,
        DateOnly.FromDateTime(f.Person.DateOfBirth),
        "fake@email.com",
        f.Person.Phone
        )
    { FirstName = f.Person.FirstName, GivenName = f.Person.FirstName, MailingAddress = address, ResidentialAddress = address });

    return userProfile.Generate();
  }
}
