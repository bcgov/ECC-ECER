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

        var authentication = new ecer_Authentication(Guid.NewGuid())
        {
            ecer_ExternalID = request.UserIdentity.Id,
            ecer_IdentityProvider = request.UserIdentity.IdentityProvider
        };

        context.AddObject(authentication);
        context.AddLink(contact, ecer_Authentication.Fields.ecer_contact_ecer_authentication_455, authentication);

        context.SaveChanges();

        return contact.Id.ToString();
    }

    public async Task<RegistrantQueryResults> Query(RegistrantQuery query)
    {
        await Task.CompletedTask;

        var qry = from contact in context.ContactSet
                  join authentication in context.ecer_AuthenticationSet on contact.ContactId equals authentication.ecer_Customerid.Id
                  select new { contact, authentication };

        if (query.WithIdentity != null) qry = qry.Where(r => r.authentication.ecer_IdentityProvider == query.WithIdentity.IdentityProvider && r.authentication.ecer_ExternalID == query.WithIdentity.Id);
        if (query.WithId != null) qry = qry.Where(r => r.contact.ContactId.Equals(Guid.Parse(query.WithId)));

        var contacts = qry.Select(r => r.contact).ToList();
        foreach (var contact in contacts)
        {
            context.LoadProperty(contact, nameof(Contact.ecer_contact_ecer_authentication_455));
        }
        return new RegistrantQueryResults(Items: mapper.Map<IEnumerable<Registrant>>(contacts));
    }
}