using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;

namespace ECER.Resources.Documents.ProgramApplications;

internal sealed class ProgramApplicationRepository : IProgramApplicationRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;
  
  public ProgramApplicationRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<ProgramApplicationQueryResults> Query(ProgramApplicationQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var applications = context.ecer_PostSecondaryInstituteProgramApplicaitonSet.AsQueryable();
    
    if (query.ById != null) applications = applications.Where(p => p.ecer_PostSecondaryInstituteProgramApplicaitonId == Guid.Parse(query.ById));

    applications = applications.OrderByDescending(p => p.CreatedOn);

    int paginatedTotalProgramCount = 0;
    if (query.PageNumber > 0)
    {
      paginatedTotalProgramCount = context.From(applications).Aggregate().Count();
      applications = applications.OrderByDescending(item => item.CreatedOn).Skip(query.PageNumber).Take(query.PageSize);
    }
    else
    {
      applications = applications.OrderByDescending(item => item.CreatedOn);
    }

    var results = context.From(applications)
      .Execute()
      .ToList();
    
    if (query.ByPostSecondaryInstituteId != null)
    {
      var instituteId = Guid.Parse(query.ByPostSecondaryInstituteId);
      results = results.Where(p => p.ecer_PostSecondaryInstitute?.Id == instituteId).ToList();
    }

    if (query.ByStatus != null && query.ByStatus.Any())
    {
      var statuses = mapper.Map<IEnumerable<ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode>>(query.ByStatus)!.ToList();
      results = results.Where(p => p.StatusCode != null && statuses.Contains(p.StatusCode.Value)).ToList();
    }

    return new ProgramApplicationQueryResults(mapper.Map<IEnumerable<ProgramApplication>>(results)!, query.PageNumber > 0 ? paginatedTotalProgramCount : results.Count);
  }
}
