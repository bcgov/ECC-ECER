using System.Net;
using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Users;
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
            _.WithBceidUser(Guid.NewGuid().ToString("N").ToUpperInvariant());
            _.Post.Json(CreateNewUserRequest()).ToUrl("/api/userinfo/profile");
            _.StatusCodeShouldBeOk();
        });
    }

    [Fact]
    public async Task PostUserProfile_NewBcscUser_Ok()
    {
        await Host.Scenario(_ =>
        {
            _.WithBcscUser(Guid.NewGuid().ToString("N").ToUpperInvariant());
            _.Post.Json(CreateNewUserRequest()).ToUrl("/api/userinfo/profile");
            _.StatusCodeShouldBeOk();
        });
    }

    [Fact]
    public async Task GetUserProfile_ExistingBCSCUser_UserProfile()
    {
        var userId = Guid.NewGuid().ToString("N").ToUpperInvariant();
        var request = CreateNewUserRequest();
        await Host.Scenario(_ =>
        {
            _.WithBceidUser(userId);
            _.Post.Json(request).ToUrl("/api/userinfo/profile");
            _.StatusCodeShouldBeOk();
        });

        var response = await Host.Scenario(_ =>
        {
            _.WithBceidUser(userId.ToUpperInvariant());
            _.Get.Url("/api/userinfo");
            _.StatusCodeShouldBeOk();
        });

        var userProfile = (await response.ReadAsJsonAsync<UserInfoResponse>()).ShouldNotBeNull().UserInfo;
        userProfile.ShouldBe(request.Profile);
    }

    [Fact]
    public async Task GetUserProfile_NonExistingBCSCUser_Empty()
    {
        var userId = Guid.NewGuid().ToString("N").ToUpperInvariant();

        var response = await Host.Scenario(_ =>
        {
            _.WithBceidUser(userId.ToUpperInvariant());
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