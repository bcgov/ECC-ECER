using Bogus;
using ECER.Resources.Accounts.Registrants;
using ECER.Utilities.Security;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.Resources.Accounts.Registrants;

public class RegistrantRepositoryTests : RegistryPortalWebAppScenarioBase

{
  private readonly IRegistrantRepository registrantRepository;

  public RegistrantRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    registrantRepository = Fixture.Services.GetRequiredService<IRegistrantRepository>();
  }

  [Fact]
  public async Task Query_NonExistentId_NoResults()
  {
    var registrants = await registrantRepository.Query(new RegistrantQuery { ByUserId = Guid.NewGuid().ToString() }, default);
    registrants.ShouldBeEmpty();
  }

  [Fact]
  public async Task Query_NonExistentIdentity_NoResults()
  {
    var registrants = await registrantRepository.Query(new RegistrantQuery { ByIdentity = new UserIdentity("noop", Guid.NewGuid().ToString()) }, default);
    registrants.ShouldBeEmpty();
  }

  [Fact]
  public async Task Create_NewRegistrant_Success()
  {
    var userProfile = CreateUserProfile();
    var newIdentity = new UserIdentity(Fixture.TestRunId, Guid.NewGuid().ToString());
    var id = await registrantRepository.Create(new Registrant { Profile = userProfile, Identities = new[] { newIdentity } }, default);

    id.ShouldNotBeNullOrEmpty();
    var user = (await registrantRepository.Query(new RegistrantQuery { ByUserId = id }, default)).ShouldHaveSingleItem();
    user.Id.ShouldBe(id);
    user.Profile.FirstName.ShouldBe(userProfile.FirstName);
    user.Profile.LastName.ShouldBe(userProfile.LastName);
    user.Profile.DateOfBirth.ShouldBe(userProfile.DateOfBirth);
    user.Profile.Email.ShouldBe(userProfile.Email);
    user.Profile.Phone.ShouldBe(userProfile.Phone);
    user.Profile.ResidentialAddress.ShouldBe(userProfile.ResidentialAddress);
    user.Profile.MailingAddress.ShouldBe(userProfile.MailingAddress);
    user.Profile.AlternateContactPhone.ShouldBe(userProfile.AlternateContactPhone);
    user.Profile.MiddleName.ShouldBe(userProfile.MiddleName);
    user.Profile.PreferredName.ShouldBe(userProfile.PreferredName);
    var identity = user.Identities.ShouldHaveSingleItem();
    newIdentity.IdentityProvider.ShouldBe(identity.IdentityProvider);
    newIdentity.UserId.ShouldBe(identity.UserId);
  }

  [Fact]
  public async Task Create_NewRegistrant_PartialUserProfile_Success()
  {
    var userProfile = CreatePartialUserProfile();
    var newIdentity = new UserIdentity(Fixture.TestRunId, Guid.NewGuid().ToString());
    var id = await registrantRepository.Create(new Registrant { Profile = userProfile, Identities = new[] { newIdentity } }, default);

    id.ShouldNotBeNullOrEmpty();
    var user = (await registrantRepository.Query(new RegistrantQuery { ByUserId = id }, default)).ShouldHaveSingleItem();
    user.Id.ShouldBe(id);
    user.Profile.FirstName.ShouldBe(userProfile.FirstName);
    user.Profile.LastName.ShouldBe(userProfile.LastName);
    user.Profile.Email.ShouldBe(userProfile.Email);
    user.Profile.Phone.ShouldBe(userProfile.Phone);
    var identity = user.Identities.ShouldHaveSingleItem();
    newIdentity.IdentityProvider.ShouldBe(identity.IdentityProvider);
    newIdentity.ShouldBe(identity);
  }

  [Fact]
  public async Task Save_ExistingRegistrant_Updated()
  {
    var userProfile = CreatePartialUserProfile();
    var newIdentity = new UserIdentity(Fixture.TestRunId, Guid.NewGuid().ToString());
    var registrantId = await registrantRepository.Create(new Registrant { Profile = userProfile, Identities = new[] { newIdentity } }, default);

    var updatedUserProfile = CreateUserProfile();
    await registrantRepository.Save(new Registrant { Profile = updatedUserProfile, Id = registrantId }, default);
    var user = (await registrantRepository.Query(new RegistrantQuery { ByUserId = registrantId }, default)).ShouldHaveSingleItem();
    user.Profile.PreviousNames = user.Profile.PreviousNames.ToArray();
    user.Profile.ShouldBe(updatedUserProfile);
  }

  [Fact]
  public async Task Save_NonExistantRegistrant_Updated()
  {
    var userProfile = CreateUserProfile();
    await Should.ThrowAsync<InvalidOperationException>(async () => await registrantRepository.Save(new Registrant { Profile = userProfile, Id = Guid.NewGuid().ToString() }, default));
  }

  private UserProfile CreateUserProfile()
  {
    return new UserProfile
    {
      FirstName = Faker.Person.FirstName,
      MiddleName = Faker.Person.FirstName,
      PreferredName = Faker.Person.FirstName,
      LastName = $"{Fixture.TestRunId}_{Faker.Person.LastName}",
      DateOfBirth = DateOnly.FromDateTime(Faker.Person.DateOfBirth),
      Email = Faker.Person.Email,
      Phone = Faker.Phone.PhoneNumber(),
      AlternateContactPhone = Faker.Phone.PhoneNumber(),
      ResidentialAddress = new Address(Faker.Address.StreetAddress(), null, Faker.Address.City(), Faker.Address.ZipCode(), Faker.Address.State(), Faker.Address.Country())
    };
  }

  private UserProfile CreatePartialUserProfile()
  {
    return new UserProfile
    {
      FirstName = Faker.Person.FirstName,
      LastName = $"{Fixture.TestRunId}_{Faker.Person.LastName}",
      Email = Faker.Person.Email,
      Phone = Faker.Phone.PhoneNumber(),
    };
  }
}
