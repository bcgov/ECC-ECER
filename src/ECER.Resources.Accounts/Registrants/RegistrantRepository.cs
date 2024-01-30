using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Accounts.Registrants;

internal sealed class RegistrantRepository(EcerContext context, IMapper mapper) : IRegistrantRepository
{
  public async Task<string> Create(Registrant registrant, CancellationToken ct)
  {
    await Task.CompletedTask;

    var contact = mapper.Map<Contact>(registrant.Profile);
    contact.Id = Guid.NewGuid();

    context.AddObject(contact);

    foreach (var identity in registrant.Identities)
    {
      var authentication = new ecer_Authentication(Guid.NewGuid())
      {
        ecer_ExternalID = identity.UserId,
        ecer_IdentityProvider = identity.IdentityProvider
      };

      context.AddObject(authentication);
      context.AddLink(contact, ecer_Authentication.Fields.ecer_contact_ecer_authentication_455, authentication);
    }
    context.SaveChanges();

    return contact.Id.ToString();
  }

  public async Task<IEnumerable<Registrant>> Query(RegistrantQuery query, CancellationToken ct)
  {
    await Task.CompletedTask;

    var qry = from contact in context.ContactSet
              join authentication in context.ecer_AuthenticationSet on contact.ContactId equals authentication.ecer_Customerid.Id
              select new { contact, authentication };

    if (query.ByIdentity != null) qry = qry.Where(r => r.authentication.ecer_IdentityProvider == query.ByIdentity.IdentityProvider && r.authentication.ecer_ExternalID == query.ByIdentity.UserId);
    if (query.ByUserId != null) qry = qry.Where(r => r.contact.ContactId.Equals(Guid.Parse(query.ByUserId)));

    var contacts = qry.Select(r => r.contact).ToList();
    foreach (var contact in contacts)
    {
      context.LoadProperty(contact, nameof(Contact.ecer_contact_ecer_authentication_455));
    }
    return mapper.Map<IEnumerable<Registrant>>(contacts);
  }

  public async Task Save(Registrant registrant, CancellationToken ct)
  {
    if (!Guid.TryParse(registrant.Id, out var contactId)) throw new InvalidOperationException($"registrant id {registrant.Id} is not a guid");
    var contact = context.ContactSet.SingleOrDefault(c => c.ContactId == contactId);

    if (contact == null) throw new InvalidOperationException($"Registrant {registrant.Id} not found");

    context.Detach(contact);

    contact = mapper.Map<Contact>(registrant.Profile);
    contact.ContactId = contactId;

    context.Attach(contact);
    context.UpdateObject(contact);

    context.SaveChanges();
    await Task.CompletedTask;
  }
}
