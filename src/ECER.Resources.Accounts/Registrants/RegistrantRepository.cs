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
      context.AddObject(contact);
    }
    else
    {
      var oldClientId = contact.ecer_ClientID;
      var oldTempClientId = contact.ecer_TempClientID;
      context.Detach(contact);
      contact = mapper.Map<Contact>(registrant.Profile)!;
      contact.Id = Guid.Parse(registrant.Id);
      contact.ecer_ClientID = oldClientId;
      contact.ecer_TempClientID = oldTempClientId;
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
    var statusCode = contact.StatusCode;
    var firstname = contact.FirstName;
    var middlename = contact.MiddleName;
    var lastname = contact.LastName;
    var idVerificationDecision = contact.ecer_idverificationdecision;

    context.Detach(contact);
    var clientId = contact.ecer_ClientID;
    var tempClientId = contact.ecer_TempClientID;
    contact = mapper.Map<Contact>(registrant.Profile)!;
    contact.ContactId = contactId;
    contact.FirstName = firstname;
    contact.MiddleName = middlename;
    contact.LastName = lastname;
    contact.ecer_IsVerified = verified;
    contact.ecer_ClientID = clientId;
    contact.ecer_TempClientID = tempClientId;
    contact.StatusCode = statusCode;
    contact.ecer_idverificationdecision = idVerificationDecision;
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

  public async Task SaveIdentityIds(Registrant registrant, ProfileIdentification profileIdentification, CancellationToken ct)
  {
    await Task.CompletedTask;

    if (!Guid.TryParse(registrant.Id, out var contactId)) throw new InvalidOperationException($"registrant id {registrant.Id} is not a guid");
    var contact = context.ContactSet.SingleOrDefault(c => c.ContactId == contactId);

    if (contact == null) throw new InvalidOperationException($"Registrant {registrant.Id} not found");

    if (contact.StatusCode != Contact_StatusCode.Unverified && contact.StatusCode != Contact_StatusCode.PendingforDocuments) throw new InvalidOperationException($"SaveIdentityIds :: Registrant {registrant.Id} is not in Unverified state. State is :: {contact.StatusCode}");

    var primaryId = context.ecer_identificationtypeSet.SingleOrDefault(i => i.ecer_identificationtypeId == Guid.Parse(profileIdentification.PrimaryIdTypeObjectId));
    var secondaryId = context.ecer_identificationtypeSet.SingleOrDefault(i => i.ecer_identificationtypeId == Guid.Parse(profileIdentification.SecondaryIdTypeObjectId));
    contact.ecer_HasProvidedID = true;
    contact.StatusCode = Contact_StatusCode.ReadyforIDVerification;
    context.UpdateObject(contact);

    context.AddLink(contact, Contact.Fields.ecer_contact_primaryidtype, primaryId!);
    context.AddLink(contact, Contact.Fields.ecer_contact_secondaryidtype, secondaryId!);

    await HandleFilesForIdentification(profileIdentification.PrimaryIds, contact, "Primary ID", ct);
    await HandleFilesForIdentification(profileIdentification.SecondaryIds, contact, "Secondary ID", ct);

    context.SaveChanges();
  }

  private async Task HandleFilesForIdentification(IEnumerable<IdentityDocument> inputList, Contact contact, string tagname, CancellationToken ct)
  {
    await Task.CompletedTask;
    var list = mapper.Map<IEnumerable<bcgov_DocumentUrl>>(inputList);
    var sourceFolder = "tempfolder";
    var destinationFolder = "ecer_contact/" + contact.ContactId;
    foreach (var document in list)
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
      context.AddLink(document, bcgov_DocumentUrl.Fields.bcgov_contact_bcgov_documenturl, contact);
      var tag = context.bcgov_tagSet.SingleOrDefault(t => t.bcgov_name.Contains(tagname));
      if (tag == null) throw new InvalidOperationException($"Tag {tagname} not found");

      context.AddLink(document, bcgov_DocumentUrl.Fields.bcgov_tag1_bcgov_documenturl, tag);
    }
  }
}
