using AutoMapper;
using ECER.Utilities.DataverseSdk;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Accounts.Registrants;

internal sealed class RegistrantRepository(EcerContext context, IMapper mapper) : IRegistrantRepository
{
    public async Task<string> RegisterNew(NewRegistrantRequest request)
    {
        await Task.CompletedTask;

        var contact = mapper.Map<Contact>(request.UserProfile);
        contact.Id = Guid.NewGuid();

        context.AddObject(contact);

        var authentication = mapper.Map<ECER_Authentication>(request.UserIdentity);

        context.AddObject(authentication);
        context.AddLink(contact, nameof(Contact.ECER_Authentication_ContactId), authentication);

        context.SaveChanges();

        return contact.Id.ToString();
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