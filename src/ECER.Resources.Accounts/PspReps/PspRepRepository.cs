using AutoMapper;
using ECER.Resources.Accounts.PspReps;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Extensions.Configuration;

namespace ECER.Resources.Accounts.PSPReps;

internal sealed class PspRepRepository(EcerContext context, IMapper mapper, IConfiguration configuration) : IPspRepRepository
{
  public async Task<IEnumerable<PspRep>> Query(PspRepQuery query, CancellationToken ct)
  {
  }

  public async Task Save(PspRep pspRep, CancellationToken ct)
  {
  }
}
