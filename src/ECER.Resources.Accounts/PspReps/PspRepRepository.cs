using AutoMapper;
using ECER.Resources.Accounts.PspReps;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;

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

    // On registration, enable portal access and persist with the authentication link
    pspUser.ecer_AccessToPortal = ecer_AccessToPortal.Active;
    context.UpdateObject(pspUser);
    context.AddObject(authentication);
    context.AddLink(pspUser, ecer_Authentication.Fields.ecer_authentication_eceprogramrepresentative, authentication);
    context.SaveChanges();

    await Task.CompletedTask;
    return authentication.Id.ToString();
  }
  
  public async Task<IEnumerable<PspUser>> Query(PspRepQuery query, CancellationToken ct)
  {
    await Task.CompletedTask;
    IQueryable<ecer_ECEProgramRepresentative> pspUsers = context.ecer_ECEProgramRepresentativeSet;

    if (query.ById != null)
    {
      pspUsers = pspUsers.Where(r => r.Id.Equals(Guid.Parse(query.ById)));
    }

    if (query.ByIdentity != null)
    {
      pspUsers = from pspUser in context.ecer_ECEProgramRepresentativeSet
                join authentication in context.ecer_AuthenticationSet on pspUser.Id equals authentication.ecer_eceprogramrepresentative.Id
                where authentication.ecer_IdentityProvider == query.ByIdentity.IdentityProvider && authentication.ecer_ExternalID == query.ByIdentity.UserId
                select pspUser;
    }

    if (query.ByPostSecondaryInstituteId != null)
    {
      if (!Guid.TryParse(query.ByPostSecondaryInstituteId, out var instituteId))
      {
        return Enumerable.Empty<PspUser>();
      }

      pspUsers = pspUsers.Where(r => r.ecer_PostSecondaryInstitute != null && r.ecer_PostSecondaryInstitute.Id == instituteId);
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
    var representativeRole = pspUser.ecer_RepresentativeRole;
    var hasAcceptedTermsOfUse = pspUser.ecer_HasAcceptedTermsofUse;

    context.Detach(pspUser);

    pspUser = mapper.Map<ecer_ECEProgramRepresentative>(user.Profile);
    pspUser.Id = userId;
    pspUser.ecer_FirstName = firstName;
    pspUser.ecer_LastName = lastName;
    pspUser.ecer_RepresentativeRole = representativeRole;
    
    // Only accept terms of use once
    if (hasAcceptedTermsOfUse == true)
    {
      pspUser.ecer_HasAcceptedTermsofUse = hasAcceptedTermsOfUse;
    }
    
    context.Attach(pspUser);
    context.UpdateObject(pspUser);
    context.SaveChanges();
    await Task.CompletedTask;
  }

  public async Task Deactivate(string pspUserId, CancellationToken ct)
  {
    await Task.CompletedTask;
    if (!Guid.TryParse(pspUserId, out var userId)) throw new InvalidOperationException($"PSP Program Rep id {pspUserId} is not a valid GUID");

    var pspUser = context.ecer_ECEProgramRepresentativeSet.SingleOrDefault(r => r.Id == userId);
    if (pspUser == null) throw new InvalidOperationException($"Psp Program Rep with id {pspUserId} not found");

    pspUser.ecer_AccessToPortal = ecer_AccessToPortal.Disabled;
    context.UpdateObject(pspUser);

    var authentications = context.ecer_AuthenticationSet.Where(authentication =>
      authentication.ecer_eceprogramrepresentative != null && authentication.ecer_eceprogramrepresentative.Id == userId).ToList();

    foreach (var authentication in authentications)
    {
      context.DeleteObject(authentication);
    }

    context.SaveChanges();
  }

  public async Task SetPrimary(string pspUserId, CancellationToken ct)
  {
    await Task.CompletedTask;
    if (!Guid.TryParse(pspUserId, out var userId)) throw new InvalidOperationException($"PSP Program Rep id {pspUserId} is not a valid GUID");

    var targetRep = context.ecer_ECEProgramRepresentativeSet.SingleOrDefault(r => r.Id == userId);
    if (targetRep == null) throw new InvalidOperationException($"Psp Program Rep with id {pspUserId} not found");

    var institutionId = targetRep.ecer_PostSecondaryInstitute?.Id;
    if (institutionId == null) throw new InvalidOperationException($"Psp Program Rep with id {pspUserId} is not linked to a post secondary institute");

    var reps = context.ecer_ECEProgramRepresentativeSet
      .Where(r => r.ecer_PostSecondaryInstitute != null && r.ecer_PostSecondaryInstitute.Id == institutionId)
      .ToList();

    foreach (var rep in reps)
    {
      rep.ecer_RepresentativeRole = rep.Id == userId ? ecer_RepresentativeRole.Primary : ecer_RepresentativeRole.Secondary;
      context.UpdateObject(rep);
    }

    context.SaveChanges();
  }

  public async Task Add(PspUserProfile profile, string postSecondaryInstitutionId, CancellationToken ct)
  {
    await Task.CompletedTask;
    
    if (!Guid.TryParse(postSecondaryInstitutionId, out var institutionId)) throw new InvalidOperationException($"Post Secondary Institution ID {postSecondaryInstitutionId} is not a valid GUID");
    
    var institution = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(i => i.Id == institutionId);
    if(institution == null) throw new InvalidOperationException($"Post Secondary Institution with ID {postSecondaryInstitutionId} not found");
    
    var pspUser = mapper.Map<ecer_ECEProgramRepresentative>(profile);
    pspUser.ecer_InvitetoPortal = true;
    pspUser.ecer_AccessToPortal = ecer_AccessToPortal.Invited;
    pspUser.ecer_RepresentativeRole = ecer_RepresentativeRole.Secondary;
    
    context.AddObject(pspUser);
    context.AddLink(institution, ecer_PostSecondaryInstitute.Fields.ecer_eceprogramrepresentative_PostSecondaryIns, pspUser);
    context.SaveChanges();
  }
}
