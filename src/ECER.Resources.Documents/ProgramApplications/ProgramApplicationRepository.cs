using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Xrm.Sdk.Client;

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
    
    //Filter by Id
    if (query.ById != null) applications = applications.Where(p => p.ecer_PostSecondaryInstituteProgramApplicaitonId == Guid.Parse(query.ById));
    
    //By status
    if (query.ByStatus != null && query.ByStatus.Any())
    {
      var statuses = mapper.Map<IEnumerable<ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode>>(query.ByStatus)!.ToList();
      applications = applications.WhereIn(p => p.StatusCode!.Value, statuses);
    }
    
    //By post secondary
    if (query.ByPostSecondaryInstituteId != null)
    {
      var instituteId = Guid.Parse(query.ByPostSecondaryInstituteId);
      applications = applications.Where(p => p.ecer_PostSecondaryInstitute.Id == instituteId);
    }
    
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
    
    return new ProgramApplicationQueryResults(mapper.Map<IEnumerable<ProgramApplication>>(results)!, query.PageNumber > 0 ? paginatedTotalProgramCount : results.Count);
  }
}
