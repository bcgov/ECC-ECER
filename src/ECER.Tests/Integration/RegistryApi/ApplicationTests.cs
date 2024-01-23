using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Applications;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class ApplicationTests : RegistryPortalWebAppScenarioBase
{
    public ApplicationTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
    {
    }

    [Fact]
    public async Task SaveDraftApplication_NewDraft_Saved()
    {
        var application = CreateDraftApplication();
        await Host.Scenario(_ =>
        {
            _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
            _.Put.Json(application).ToUrl("/api/draftapplications");
            _.StatusCodeShouldBeOk();
        });
    }

    private DraftApplication CreateDraftApplication()
    {
        return new Faker<DraftApplication>("en_CA")
            .RuleFor(f => f.CertificationTypes, f => f.Make(f.Random.Number(2), () => f.PickRandom<CertificationType>()))
            .Generate();
    }
}