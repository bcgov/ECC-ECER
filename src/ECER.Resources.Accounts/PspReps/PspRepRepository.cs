using AutoMapper;
using ECER.Resources.Accounts.PspReps;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Accounts.PSPReps;

internal sealed class PspRepRepository(EcerContext context, IMapper mapper) : IPspRepRepository
{
  public async Task<string> AttachIdentity(PspUser user, CancellationToken ct)
  {
    if(!Guid.TryParse(user.Id, out var userId)) throw new InvalidOperationException($"PSP Program Rep id {user.Id} is not a valid GUID");

    var pspUser = context.ecer_ECEProgramRepresentativeSet.SingleOrDefault(r => r.Id == userId);
    
    if (pspUser == null) throw new InvalidOperationException($"Psp Program Rep with id {user.Id} not found");

    var identity = user.Identities.FirstOrDefault();
    
    if(identity == null) throw new InvalidOperationException("No identity provided to attach to Psp Program Rep");
    
    var authentication = new ecer_Authentication(Guid.NewGuid())
    {
      ecer_ExternalID = identity.UserId,
      ecer_IdentityProvider = identity.IdentityProvider
    };

    context.AddObject(authentication);
    context.AddLink(pspUser, ecer_Authentication.Fields.ecer_authentication_eceprogramrepresentative, authentication);

    await Task.CompletedTask;
    return authentication.Id.ToString();
  }
  
  public async Task<IEnumerable<PspUser>> Query(PspRepQuery query, CancellationToken ct)
  {
    await Task.CompletedTask;
    var pspUsers = context.ecer_ECEProgramRepresentativeSet;

    if (query.ById != null)
    {
      pspUsers = pspUsers.Where(r => r.Id.Equals(Guid.Parse(query.ById)));
    }

    if (query.ByIdentity != null)
    {
      pspUsers = from pspUser in context.ecer_ECEProgramRepresentativeSet
                join authentication in context.ecer_AuthenticationSet on pspUser.ecer_ContactId.Id equals authentication.ecer_Customerid.Id
                where authentication.ecer_IdentityProvider == query.ByIdentity.IdentityProvider && authentication.ecer_ExternalID == query.ByIdentity.UserId
                select pspUser;
    }
    
    var results = context.From(pspUsers).Execute();
    
    return mapper.Map<IEnumerable<PspUser>>(results).ToList();
  }

  public async Task Save(PspUser user, CancellationToken ct)
  {
    if(!Guid.TryParse(user.Id, out var userId)) throw new InvalidOperationException($"PSP Program Rep id {user.Id} is not a valid GUID");

    var pspUser = context.ecer_ECEProgramRepresentativeSet.SingleOrDefault(r => r.Id == userId);
    
    if (pspUser == null) throw new InvalidOperationException($"Psp Program Rep with id {user.Id} not found");

    var firstName = pspUser.ecer_FirstName;
    var lastName = pspUser.ecer_LastName;
    var email = pspUser.ecer_EmailAddress;

    context.Detach(pspUser);

    pspUser = mapper.Map<ecer_ECEProgramRepresentative>(user.Profile);
    pspUser.Id = userId;
    pspUser.ecer_FirstName = firstName;
    pspUser.ecer_LastName = lastName;
    pspUser.ecer_EmailAddress = email;
    
    context.Attach(pspUser);
    context.UpdateObject(pspUser);
    context.SaveChanges();
    await Task.CompletedTask;
  }
}
