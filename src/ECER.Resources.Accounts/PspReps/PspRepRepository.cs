using AutoMapper;
using ECER.Resources.Accounts.PspReps;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Extensions.Configuration;

namespace ECER.Resources.Accounts.PSPReps;

internal sealed class PspRepRepository(EcerContext context, IMapper mapper, IConfiguration configuration) : IPspRepRepository
{
  public async Task<IEnumerable<PspRep>> Query(PspRepQuery query, CancellationToken ct)
  {
    await Task.CompletedTask;
    var pspReps = context.ecer_ECEProgramRepresentativeSet;

    if (query.ById != null)
    {
      pspReps = from pspRep in context.ecer_ECEProgramRepresentativeSet
                 join authentication in context.ecer_AuthenticationSet on pspRep.ecer_ContactId.Id equals authentication.ecer_Customerid.Id
                 where authentication.ecer_IdentityProvider == query.ByIdentity.IdentityProvider && authentication.ecer_ExternalID == query.ByIdentity.UserId
                 select pspRep;
    }

    var results = context.From(pspReps).Execute();

    return mapper.Map<IEnumerable<PspRep>>(results)!.ToList();
  }

  public async Task Save(PspRep pspRep, CancellationToken ct)
  {
    if (!Guid.TryParse(pspRep.Id, out var pspRepId)) throw new InvalidOperationException($"Psp rep id {pspRep.Id} is not a guid");
    var representative = context.ecer_ECEProgramRepresentativeSet.SingleOrDefault(c => c.Id == pspRepId);

    if (representative == null) throw new InvalidOperationException($"Psp rep {pspRepId} not found");

    context.Detach(representative);
    representative = mapper.Map<PspRep>(pspRep)!;
    context.Attach(representative);
    context.UpdateObject(representative);

    context.SaveChanges();
    await Task.CompletedTask;
  }
}
