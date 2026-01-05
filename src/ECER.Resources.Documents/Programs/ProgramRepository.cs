using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Programs;

internal sealed class ProgramRepository : IProgramRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public ProgramRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<IEnumerable<Program>> Query(ProgramQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var programs = context.ecer_ProgramSet.AsQueryable();

    if (query.ById != null) programs = programs.Where(p => p.ecer_ProgramId == Guid.Parse(query.ById));

    programs = programs.OrderByDescending(p => p.CreatedOn);

    var results = context.From(programs)
      .Join()
      .Include(c => c.ecer_course_Programid)
      .Execute()
      .ToList();

    if (query.ByPostSecondaryInstituteId != null)
    {
      var instituteId = Guid.Parse(query.ByPostSecondaryInstituteId);
      results = results.Where(p => p.ecer_PostSecondaryInstitution != null && p.ecer_PostSecondaryInstitution.Id == instituteId).ToList();
    }

    if (query.ByStatus != null && query.ByStatus.Any())
    {
      var statuses = mapper.Map<IEnumerable<ecer_Program_StatusCode>>(query.ByStatus)!.ToList();
      results = results.Where(p => p.StatusCode != null && statuses.Contains(p.StatusCode.Value)).ToList();
    }

    return mapper.Map<IEnumerable<Program>>(results)!;
  }

  public async Task<string> Save(Program program, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    if (string.IsNullOrWhiteSpace(program.PostSecondaryInstituteId))
    {
      throw new InvalidOperationException("Post secondary institute id is required");
    }

    var instituteId = Guid.Parse(program.PostSecondaryInstituteId);
    var institute = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(i => i.ecer_PostSecondaryInstituteId == instituteId);
    if (institute == null) throw new InvalidOperationException($"Post secondary institute '{program.PostSecondaryInstituteId}' not found");

    var ecerProgram = mapper.Map<ecer_Program>(program)!;
    var defaultStatus = ecer_Program_StatusCode.RequiresReview;

    if (!ecerProgram.ecer_ProgramId.HasValue)
    {
      ecerProgram.StatusCode = defaultStatus;
      ecerProgram.ecer_ProgramId = Guid.NewGuid();
      if (string.IsNullOrWhiteSpace(ecerProgram.ecer_Name))
      {
        ecerProgram.ecer_Name = "Draft Program";
      }

      context.AddObject(ecerProgram);
      context.AddLink(ecerProgram, ecer_Program.Fields.ecer_program_PostSecondaryInstitution_ecer_pos, institute);
    }
    else
    {
      var existing = context.ecer_ProgramSet.SingleOrDefault(p => p.ecer_ProgramId == ecerProgram.ecer_ProgramId);
      if (existing == null) throw new InvalidOperationException($"ecer_Program '{ecerProgram.ecer_ProgramId}' not found");

      ecerProgram.StatusCode = existing.StatusCode ?? defaultStatus;
      if (string.IsNullOrWhiteSpace(ecerProgram.ecer_Name))
      {
        ecerProgram.ecer_Name = existing.ecer_Name;
      }

      context.Detach(existing);
      context.Attach(ecerProgram);
      context.UpdateObject(ecerProgram);
      context.AddLink(ecerProgram, ecer_Program.Fields.ecer_program_PostSecondaryInstitution_ecer_pos, institute);
    }

    context.SaveChanges();
    return ecerProgram.ecer_ProgramId.Value.ToString();
  }
}
