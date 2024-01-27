using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Users;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class ProfileTests : RegistryPortalWebAppScenarioBase
{
  public ProfileTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetProfile_Existing_Returned()
  {
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url("/api/profile");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task PutProfile_Existing_Returned()
  {
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(CreateNewUser()).ToUrl("/api/profile");
      _.StatusCodeShouldBeOk();
    });
  }

  private UserProfile CreateNewUser()
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

    return userProfile.Generate();
  }
}
