using System.Globalization;
using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Accounts.Registrants;

internal sealed class RegistrantRepository(EcerContext context, IMapper mapper) : IRegistrantRepository
{
    public async Task<string> Create(NewRegistrantRequest request)
    {
        await Task.CompletedTask;
        var registrantId = Guid.NewGuid();
        var contact = new Contact(registrantId)
        {
            FirstName = $"first{registrantId.ToString().Substring(0, 4)}",
            LastName = $"last{registrantId.ToString().Substring(32, 4)}",
            Birthdate = DateTime.Parse("2000-01-01", CultureInfo.InvariantCulture)
        };

        context.AddObject(contact);

        context.SaveChanges();

        return registrantId.ToString();
    }

    public async Task<RegistrantQueryResults> Query(RegistrantQuery query)
    {
        await Task.CompletedTask;

        var contacts = from contact in context.ContactSet
                       join authentication in context.ECER_AuthenticationSet on contact.ContactId equals authentication.ECER_ContactId.Id
                       select new { contact, authentication };

        if (query.WithIdentity != null) contacts = contacts.Where(r => r.authentication.ECER_IdentityProviderName == query.WithIdentity.IdentityProvider && r.authentication.ECER_ExternalId == query.WithIdentity.Id);
        if (query.WithId != null) contacts = contacts.Where(r => r.contact.Id == Guid.Parse(query.WithId));

        return new RegistrantQueryResults(Items: mapper.Map<IEnumerable<Registrant>>(contacts.ToList()));
    }
}