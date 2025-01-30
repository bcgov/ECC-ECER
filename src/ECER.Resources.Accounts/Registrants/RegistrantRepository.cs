using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Accounts.Registrants;

internal sealed class RegistrantRepository(EcerContext context, IMapper mapper, IObjecStorageProvider objectStorageProvider, IConfiguration configuration) : IRegistrantRepository
{
  public async Task<string> Create(Registrant registrant, CancellationToken ct)
  {
    await Task.CompletedTask;

    var contact = string.IsNullOrEmpty(registrant.Id) ? null : context.ContactSet.SingleOrDefault(c => c.ContactId.Equals(Guid.Parse(registrant.Id)));

    if (contact == null)
    {
      contact = mapper.Map<Contact>(registrant.Profile)!;
      contact.Id = Guid.NewGuid();
      contact.ecer_ClientID = null;
      contact.ecer_TempClientID = null;
      context.AddObject(contact);
    }
    else
    {
      var oldClientId = contact.ecer_ClientID;
      var oldTemClientId = contact.ecer_TempClientID;
      context.Detach(contact);
      contact = mapper.Map<Contact>(registrant.Profile)!;
      contact.Id = Guid.Parse(registrant.Id);
      contact.ecer_ClientID = oldClientId;
      contact.ecer_TempClientID = oldTemClientId;
      context.Attach(contact);
      context.UpdateObject(contact);
    }

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
    var contacts = context.ContactSet;

    if (query.ByIdentity != null)
    {
      contacts = from contact in context.ContactSet
                 join authentication in context.ecer_AuthenticationSet on contact.ContactId equals authentication.ecer_Customerid.Id
                 where authentication.ecer_IdentityProvider == query.ByIdentity.IdentityProvider && authentication.ecer_ExternalID == query.ByIdentity.UserId
                 select contact;
    }

    if (query.ByUserId != null) contacts = contacts.Where(r => r.ContactId.Equals(Guid.Parse(query.ByUserId)));

    if (query.ByLastName != null) contacts = contacts.Where(r => r.LastName.Equals(query.ByLastName));
    if (query.ByRegistrationNumber != null) contacts = contacts.Where(r => r.ecer_ClientID.Equals(query.ByRegistrationNumber));
    if (query.ByDateOfBirth != null) contacts = contacts.Where(r => r.BirthDate == query.ByDateOfBirth.Value.ToDateTime(TimeOnly.MinValue).Date);

    var results = context.From(contacts)
      .Join()
      .Include(a => a.ecer_contact_ecer_authentication_455)
      .Include(a => a.ecer_previousname_Contactid)
      .Execute();

    // Filter out previous names where the Source is "Name Log"
    foreach (var contact in results)
    {
      if (contact.ecer_previousname_Contactid != null && contact.ecer_previousname_Contactid.Any())
      {
        var previousNames = contact.ecer_previousname_Contactid
            .Where(pn => pn.ecer_Source != ecer_PreviousNameSources.NameLog)
            .ToList();
        contact.ecer_previousname_Contactid = previousNames.Count != 0 ? previousNames : null;
      }
    }

    return mapper.Map<IEnumerable<Registrant>>(results)!.ToList();
  }

  public async Task Save(Registrant registrant, CancellationToken ct)
  {
    if (!Guid.TryParse(registrant.Id, out var contactId)) throw new InvalidOperationException($"registrant id {registrant.Id} is not a guid");
    var contact = context.ContactSet.SingleOrDefault(c => c.ContactId == contactId);

    if (contact == null) throw new InvalidOperationException($"Registrant {registrant.Id} not found");

    var verified = contact.ecer_IsVerified;

    context.Detach(contact);
    var clientId = contact.ecer_ClientID;
    var tempClientId = contact.ecer_TempClientID;
    var bceidVerificationStatus = contact.ecer_BCeIDVerificationStatus;
    contact = mapper.Map<Contact>(registrant.Profile)!;
    contact.ContactId = contactId;
    contact.ecer_IsVerified = verified;
    contact.ecer_ClientID = clientId;
    contact.ecer_TempClientID = tempClientId;
    contact.ecer_BCeIDVerificationStatus = bceidVerificationStatus;
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
