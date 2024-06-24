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

    
    return new Faker<UserProfile>("en_CA")
      .RuleFor(f => f.FirstName, f => f.Name.FirstName())
      .RuleFor(f => f.LastName, f => f.Name.LastName())
      .RuleFor(f => f.MiddleName, f => f.Name.FirstName())
      .RuleFor(f => f.PreferredName, f => f.Name.FirstName())
      .RuleFor(f => f.AlternateContactPhone, f => f.Phone.PhoneNumber())
      .RuleFor(f => f.DateOfBirth, f => f.Date.PastDateOnly())
      .RuleFor(f => f.Email, f => f.Internet.Email())
      .RuleFor(f => f.Phone, f => f.Phone.PhoneNumber())
      .RuleFor(f => f.ResidentialAddress, address)
      .RuleFor(f => f.MailingAddress, address)
      .RuleFor(f => f.PreviousNames, Array.Empty<PreviousName>())
      .Generate();
  }
}
