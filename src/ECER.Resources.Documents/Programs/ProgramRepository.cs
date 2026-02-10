using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Xrm.Sdk;
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

  public async Task<ProgramResult> Query(ProgramQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var programs = context.ecer_ProgramSet.AsQueryable();

    if (query.ById != null) programs = programs.Where(p => p.ecer_ProgramId == Guid.Parse(query.ById));

    programs = programs.OrderByDescending(p => p.CreatedOn);

    int paginatedTotalProgramCount = 0;
    if (query.PageNumber > 0)
    {
      paginatedTotalProgramCount = context.From(programs).Aggregate().Count();
      programs = programs.OrderByDescending(item => item.CreatedOn).Skip(query.PageNumber).Take(query.PageSize);
    }
    else
    {
      programs = programs.OrderByDescending(item => item.CreatedOn);
    }

    List<ecer_Program> results;

    if (query.ById != null)
    {
      results = context.From(programs)
        .Join()
        .Include(c => c.ecer_course_Programid)
        .IncludeNested(t => t.ecer_courseprovincialrequirement_CourseId)
        .Execute()
        .ToList();
    }
    else
    {
      results = context.From(programs)
        .Execute()
        .ToList();
    }

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

    return new ProgramResult
    {
      Programs = mapper.Map<IEnumerable<Program>>(results)!,
      TotalProgramsCount = query.PageNumber > 0 ? paginatedTotalProgramCount : results.Count,
    };
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

      //check for existing values, if they are the same do not map. 
      ecerProgram.ecer_NewDescriptiveProgramName = existing.ecer_DescriptiveProgramName == ecerProgram.ecer_NewDescriptiveProgramName ? string.Empty : ecerProgram.ecer_NewDescriptiveProgramName;
      ecerProgram.ecer_NewOfferingType = existing.ecer_OfferingType.SequenceEqual(ecerProgram.ecer_NewOfferingType) ? null : ecerProgram.ecer_NewOfferingType;

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

  public async Task<string> UpdateProgram(Program program, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var existingProgram = context.ecer_ProgramSet.SingleOrDefault(p => p.ecer_ProgramId == Guid.Parse(program.Id!));
    if (existingProgram == null) throw new InvalidOperationException($"ecer_Program '{program.Id}' not found");

    if (existingProgram.ecer_Type == ecer_ProgramProfileType.ChangeRequest &&
        existingProgram.StatusCode == ecer_Program_StatusCode.RequiresReview &&
        program.Status == ProgramStatus.Withdrawn)
    {
      existingProgram.StatusCode = ecer_Program_StatusCode.Withdrawn;
      existingProgram.StateCode = ecer_program_statecode.Inactive;
      context.UpdateObject(existingProgram);

      // Update the original program profile back to Registry Review Complete
      var fromProgramProfileId = existingProgram.ecer_FromProgramProfileId?.Id;
      if (fromProgramProfileId != null)
      {
        var originalProgram = context.ecer_ProgramSet
          .SingleOrDefault(p => p.ecer_ProgramId == fromProgramProfileId);
        if (originalProgram != null)
        {
          originalProgram.StatusCode = ecer_Program_StatusCode.RegistryReviewComplete;
          context.UpdateObject(originalProgram);
        }
      }
    }
    context.SaveChanges();
    return program.Id!;
  }

  public async Task<string> SubmitProgramProfile(string id, string userId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var program = context.ecer_ProgramSet.SingleOrDefault(p => p.ecer_ProgramId == Guid.Parse(id));
    var pspUser = context.ecer_ECEProgramRepresentativeSet.SingleOrDefault(r => r.Id == Guid.Parse(userId));
    if (program == null) throw new InvalidOperationException($"ecer_Program '{id}' not found");

    program.ecer_DeclarationDate = DateTime.Now;
    
    var firstName = pspUser!.ecer_FirstName?.Trim() ?? string.Empty;
    var lastName = pspUser.ecer_LastName?.Trim() ?? string.Empty;
    program.ecer_UserName = $"{firstName} {lastName}".Trim();
    
    program.StatusCode = ecer_Program_StatusCode.UnderRegistryReview;
    context.UpdateObject(program);
    context.AddLink(pspUser, ecer_Program.Fields.ecer_program_ProgramRepresentative_ecer_eceprogramrepresentative, program);
    
    context.SaveChanges();
    return id;
  }

  public async Task<string> ChangeProgram(Program program, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var existingProgram = context.ecer_ProgramSet.SingleOrDefault(p => p.ecer_ProgramId == Guid.Parse(program.Id!));
    if (existingProgram == null) throw new InvalidOperationException($"ecer_Program '{program.Id}' not found");

    /* Change the status to Change Request In Progress */
    existingProgram.StatusCode = ecer_Program_StatusCode.ChangeRequestInProgress;
    context.UpdateObject(existingProgram);
    context.SaveChanges();

    var newProgram = context.ecer_ProgramSet.SingleOrDefault(p => p.ecer_FromProgramProfileIdName == program.Name);
    if (newProgram == null) throw new InvalidOperationException($"New program for ecer_Program '{program.Id}' not found");

    var newProgramId = newProgram.ecer_ProgramId != null ? newProgram.ecer_ProgramId.ToString() : string.Empty;

    return newProgramId!;
  }

}
