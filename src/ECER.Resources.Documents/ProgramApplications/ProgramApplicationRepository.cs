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

  public async Task<string> Create(ProgramApplication programApplication, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    if (string.IsNullOrWhiteSpace(programApplication.PostSecondaryInstituteId))
    {
      throw new InvalidOperationException("Post secondary institute id is required");
    }

    var instituteId = Guid.Parse(programApplication.PostSecondaryInstituteId);
    var institute = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(i => i.ecer_PostSecondaryInstituteId == instituteId);
    if (institute == null)
    {
      throw new InvalidOperationException($"Post secondary institute '{programApplication.PostSecondaryInstituteId}' not found");
    }

    var entity = mapper.Map<ecer_PostSecondaryInstituteProgramApplicaiton>(programApplication)!;
    entity.ecer_PostSecondaryInstituteProgramApplicaitonId = Guid.NewGuid();
    entity.StatusCode = ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode.Draft;
    entity.StateCode = ecer_postsecondaryinstituteprogramapplicaiton_statecode.Active;

    if (string.IsNullOrWhiteSpace(entity.ecer_Name))
    {
      entity.ecer_Name = "Draft Program Application";
    }

    context.AddObject(entity);
    context.AddLink(entity, ecer_PostSecondaryInstituteProgramApplicaiton.Fields.ecer_postsecondaryinstituteprogramapplicaiton_, institute);

    context.SaveChanges();
    return entity.ecer_PostSecondaryInstituteProgramApplicaitonId!.Value.ToString();
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
