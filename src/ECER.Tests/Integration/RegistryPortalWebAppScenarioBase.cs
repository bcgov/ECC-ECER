using System.Globalization;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ECER.Tests.Integration;

[CollectionDefinition("RegistryPortalWebAppScenario")]
public class WebAppScenarioCollectionFixture : ICollectionFixture<RegistryPortalWebAppFixture>;

[Collection("RegistryPortalWebAppScenario")]
public abstract class RegistryPortalWebAppScenarioBase : WebAppScenarioBase
{
    protected new RegistryPortalWebAppFixture Fixture => (RegistryPortalWebAppFixture)base.Fixture;

    protected RegistryPortalWebAppScenarioBase(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
    {
    }
}

public class RegistryPortalWebAppFixture : WebAppFixtureBase
{
    private Contact authenticatedBcscUser = null!;

    public UserIdentity AuthenticatedBcscUserIdentity => authenticatedBcscUser.ecer_contact_ecer_authentication_455.Select(a => new UserIdentity(a.ecer_ExternalID, a.ecer_IdentityProvider)).First();
    public string AuthenticatedBcscUserId => authenticatedBcscUser.Id.ToString();

    public override async Task InitializeAsync()
    {
        Host = await CreateHost<Clients.RegistryPortal.Server.Program>();
        using var scope = Host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EcerContext>();
        await InitializeDataverseTestData(context);
    }

    private async Task InitializeDataverseTestData(EcerContext context)
    {
        await Task.CompletedTask;

        authenticatedBcscUser = GetOrAddApplicant(context, "bcsc", $"{TestRunId}_user");

        context.SaveChanges();
    }

    private Contact GetOrAddApplicant(EcerContext context, string identityProvider, string userId)
    {
        var query = (from authentication in context.ecer_AuthenticationSet
                     join contact in context.ContactSet on authentication.ecer_authentication_Contactid.ContactId equals contact.ContactId into contacts
                     from contact in contacts.DefaultIfEmpty()
                     where authentication.ecer_IdentityProvider == identityProvider && authentication.ecer_ExternalID == userId
                     select new { authentication, contact }).SingleOrDefault();

        if (query == null)
        {
            var contact = new Contact
            {
                FirstName = "test1",
                LastName = "test1",
                BirthDate = DateTime.Parse("2000-03-15", CultureInfo.InvariantCulture),
            };

            var authentication = new ecer_Authentication
            {
                ecer_IdentityProvider = identityProvider,
                ecer_ExternalID = userId
            };
            context.AddObject(authentication);
            context.AddRelatedObject(authentication, ecer_Authentication.Fields.ecer_contact_ecer_authentication_455, contact);

            context.SaveChanges();

            context.Attach(contact);

            query = new { authentication, contact };
        }

        context.LoadProperty(query.contact, Contact.Fields.ecer_contact_ecer_authentication_455);
        return query.contact;
    }
}