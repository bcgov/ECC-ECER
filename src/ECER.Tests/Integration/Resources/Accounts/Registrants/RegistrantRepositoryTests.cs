using Bogus;
using ECER.Resources.Accounts.Registrants;
using ECER.Utilities.Security;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.Resources.Accounts.Registrants;

[IntegrationTest]
public class RegistrantRepositoryTests : RegistryPortalWebAppScenarioBase

{
    private readonly IRegistrantRepository registrantRepository;

    public RegistrantRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
    {
        registrantRepository = Host.Services.GetRequiredService<IRegistrantRepository>();
    }

    [Fact]
    public async Task Query_NonExistentId_NoResults()
    {
        var result = await registrantRepository.Query(new RegistrantQuery { WithId = Guid.NewGuid().ToString() });
        result.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task Query_NonExistentIdentity_NoResults()
    {
        var result = await registrantRepository.Query(new RegistrantQuery { WithIdentity = new UserIdentity("noop", Guid.NewGuid().ToString()) });
        result.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task Create_NewRegistrant_Success()
    {
        var userProfile = CreateUserProfile();
        var newIdentity = new UserIdentity(Fixture.TestRunId, Guid.NewGuid().ToString());
        var id = await registrantRepository.RegisterNew(new NewRegistrantRequest(userProfile, newIdentity));

        id.ShouldNotBeNullOrEmpty();
        var user = (await registrantRepository.Query(new RegistrantQuery { WithId = id })).Items.ShouldHaveSingleItem();
        user.Id.ShouldBe(id);
        user.Profile.FirstName.ShouldBe(userProfile.FirstName);
        user.Profile.LastName.ShouldBe(userProfile.LastName);
        user.Profile.DateOfBirth.ShouldBe(userProfile.DateOfBirth);
        user.Profile.Email.ShouldBe(userProfile.Email);
        user.Profile.Phone.ShouldBe(userProfile.Phone);
        user.Profile.HomeAddress.ShouldBe(userProfile.HomeAddress);
        user.Profile.MailingAddress.ShouldBe(userProfile.MailingAddress);
        var identity = user.Identities.ShouldHaveSingleItem();
        newIdentity.IdentityProvider.ShouldBe(identity.IdentityProvider);
        newIdentity.UserId.ShouldBe(identity.UserId);
    }

    private UserProfile CreateUserProfile()
    {
        return new UserProfile
        {
            FirstName = Faker.Person.FirstName,
            LastName = $"{Fixture.TestRunId}_{Faker.Person.LastName}",
            DateOfBirth = DateOnly.FromDateTime(Faker.Person.DateOfBirth),
            Email = Faker.Person.Email,
            Phone = Faker.Phone.PhoneNumber(),
            HomeAddress = new Address(Faker.Address.StreetAddress(), null, Faker.Address.City(), Faker.Address.ZipCode(), Faker.Address.State(), Faker.Address.Country())
        };
    }
}