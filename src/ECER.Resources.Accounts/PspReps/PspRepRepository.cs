using AutoMapper;
using ECER.Resources.Accounts.PspReps;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Extensions.Configuration;

namespace ECER.Resources.Accounts.PSPReps;

internal sealed class PspRepRepository() : IPspRepRepository
{
  public Task<IEnumerable<PspRep>> Query(PspRepQuery query, CancellationToken ct)
  {
    throw new NotImplementedException();
  }

  public Task Save(PspRep pspRep, CancellationToken ct)
  {
    throw new NotImplementedException();
  }
}
