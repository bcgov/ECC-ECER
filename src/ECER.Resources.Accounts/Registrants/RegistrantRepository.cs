using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;
using ECER.Utilities.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using ECER.Utilities.ObjectStorage.Providers.S3;

namespace ECER.Resources.Accounts.Registrants;

internal sealed class RegistrantRepository(EcerContext context, IMapper mapper, IObjecStorageProvider objectStorageProvider, IConfiguration configuration) : IRegistrantRepository
{
  public async Task<string> Create(Registrant registrant, CancellationToken ct)
  {
    await Task.CompletedTask;

    var contact = mapper.Map<Contact>(registrant.Profile)!;
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
      context.LoadProperty(contact, nameof(Contact.ecer_previousname_Contactid));
    }
    return mapper.Map<IEnumerable<Registrant>>(contacts)!;
  }

  public async Task Save(Registrant registrant, CancellationToken ct)
  {
    if (!Guid.TryParse(registrant.Id, out var contactId)) throw new InvalidOperationException($"registrant id {registrant.Id} is not a guid");
    var contact = context.ContactSet.SingleOrDefault(c => c.ContactId == contactId);

    if (contact == null) throw new InvalidOperationException($"Registrant {registrant.Id} not found");

    context.Detach(contact);

    contact = mapper.Map<Contact>(registrant.Profile)!;
    contact.ContactId = contactId;

    context.Attach(contact);
    context.UpdateObject(contact);

    var ecerPreviousNames = mapper.Map<IEnumerable<ecer_PreviousName>>(registrant.Profile.PreviousNames)!;
    foreach (var previousName in ecerPreviousNames)
    {
      if (previousName.Id == Guid.Empty)
      {
        previousName.ecer_PreviousNameId = Guid.NewGuid();
        context.AddObject(previousName);
        context.AddLink(previousName, ecer_PreviousName.Fields.ecer_previousname_Contactid, contact);
      }
      else
      {
        var sourceFolder = "tempfolder";
        var destinationFolder = "ecer_previousname/" + previousName.Id;
        var existingPreviousName = context.ecer_PreviousNameSet.SingleOrDefault(c => c.Id == previousName.Id);
        if (existingPreviousName == null) throw new InvalidOperationException($"Previous name {previousName.Id} not found");
        foreach (var document in previousName.ecer_documenturl_PreviousNameId)
        {
          if (document.Id == Guid.Empty)
          {
            throw new InvalidOperationException($"Document '{document.Id}' is not valid");
          }

          var fileId = document.Id.ToString();
          await objectStorageProvider.MoveAsync(new S3Descriptor(GetBucketName(configuration), fileId, sourceFolder), new S3Descriptor(GetBucketName(configuration), fileId, destinationFolder), ct);

          document.bcgov_Url = destinationFolder;
          document.StatusCode = bcgov_DocumentUrl_StatusCode.Active;
          document.StateCode = bcgov_documenturl_statecode.Active;

          context.AddObject(document);
          context.AddLink(document, bcgov_DocumentUrl.Fields.ecer_documenturl_PreviousNameId, existingPreviousName);
        }

        existingPreviousName.ecer_LinktoIDDocument = destinationFolder;
        existingPreviousName.StatusCode = ecer_PreviousName_StatusCode.ReadyforVerification;
        context.UpdateObject(existingPreviousName);
      }
    }

    context.SaveChanges();
    await Task.CompletedTask;
  }

  private static string GetBucketName(IConfiguration configuration) =>
 configuration.GetValue<string>("objectStorage:bucketName") ?? throw new InvalidOperationException("objectStorage:bucketName is not set");
}
