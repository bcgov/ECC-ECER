using AutoMapper;
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

        var authentication = new ECER_Authentication(Guid.NewGuid())
        {
            ECER_ExternalId = request.UserIdentity.Id,
            ECER_IdentityProvider = request.UserIdentity.IdentityProvider
        };

        context.AddObject(authentication);
        context.AddLink(contact, ECER_Authentication.Fields.ECER_Contact_ECER_Authentication_455, authentication);

        context.SaveChanges();

        return contact.Id.ToString();
    }

    public async Task<RegistrantQueryResults> Query(RegistrantQuery query)
    {
        await Task.CompletedTask;

        var qry = from contact in context.ContactSet
                  join authentication in context.ECER_AuthenticationSet on contact.ContactId equals authentication.ECER_CustomerId.Id
                  select new { contact, authentication };

        if (query.WithIdentity != null) qry = qry.Where(r => r.authentication.ECER_IdentityProvider == query.WithIdentity.IdentityProvider && r.authentication.ECER_ExternalId == query.WithIdentity.Id);
        if (query.WithId != null) qry = qry.Where(r => r.contact.ContactId.Equals(Guid.Parse(query.WithId)));

        var contacts = qry.Select(r => r.contact).ToList();
        foreach (var contact in contacts)
        {
            context.LoadProperty(contact, nameof(Contact.ECER_Contact_ECER_Authentication_455));
        }
        return new RegistrantQueryResults(Items: mapper.Map<IEnumerable<Registrant>>(contacts));
    }
}