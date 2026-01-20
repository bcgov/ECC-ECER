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

  public async Task<IEnumerable<Program>> Query(ProgramQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var programs = context.ecer_ProgramSet.AsQueryable();

    if (query.ById != null) programs = programs.Where(p => p.ecer_ProgramId == Guid.Parse(query.ById));

    programs = programs.OrderByDescending(p => p.CreatedOn);

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

  public async Task<string> UpdateCourse(IEnumerable<Course> incomingCourse, string id, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var program = context.ecer_ProgramSet.SingleOrDefault(p => p.ecer_ProgramId == Guid.Parse(id));
    if (program == null) throw new InvalidOperationException($"ecer_Program '{id}' not found");
    
    foreach (var course in incomingCourse)
    {
      var courses = context.ecer_CourseSet.AsQueryable().Where(p => p.ecer_CourseId == Guid.Parse(course.CourseId));
      
      var courseExists = context.From(courses)
        .Join()
        .Include(c => c.ecer_courseprovincialrequirement_CourseId)
        .Execute()
        .SingleOrDefault();
      
      //.ecer_CourseSet.SingleOrDefault(r => r.ecer_CourseId == Guid.Parse(course.CourseId));
      
      if (courseExists != null)
      {
        if (!context.IsAttached(courseExists))
        {
          context.Attach(courseExists);
        }
        
        courseExists.ecer_NewCode = !string.IsNullOrWhiteSpace(course.NewCourseNumber)
          ? course.NewCourseNumber
          : course.CourseNumber;
        courseExists.ecer_NewCourseName = !string.IsNullOrWhiteSpace(course.NewCourseTitle) 
          ? course.NewCourseTitle 
          : course.CourseTitle;
        context.UpdateObject(courseExists);

        if (course.CourseAreaOfInstruction != null)
        {
          foreach (var areaOfInstruction in course.CourseAreaOfInstruction)
          {
            if (courseExists.ecer_courseprovincialrequirement_CourseId != null)
            {
              var existingAreaOfInstruction =
                courseExists.ecer_courseprovincialrequirement_CourseId.SingleOrDefault(a =>
                  a.Id == Guid.Parse(areaOfInstruction.CourseAreaOfInstructionId));
              
              if (existingAreaOfInstruction != null)
              {
                UpdateAreaOfInstruction(areaOfInstruction,  existingAreaOfInstruction);
              }
              else
              {
                CreateNewAreaOfInstruction(areaOfInstruction, courseExists);
              }
            }
          }
        }
      }
      else
      {
        throw new InvalidOperationException($"Course not found");
      }
    }
    context.SaveChanges();
    return program.Id.ToString();
  }

  private void CreateNewAreaOfInstruction(CourseAreaOfInstruction areaOfInstruction, ecer_Course courseExists)
  {
    var instructionArea =
      context.ecer_ProvincialRequirementSet.SingleOrDefault(r =>
        r.Id == Guid.Parse(areaOfInstruction.AreaOfInstructionId));

    if (instructionArea != null)
    {
      var newAreaOfInstruction = new ecer_CourseProvincialRequirement
      {
        Id = Guid.NewGuid(),
        ecer_Hours = 0,
        ecer_NewHours = Convert.ToDecimal(areaOfInstruction.NewHours),
        ecer_CourseId = new EntityReference(ecer_Course.EntityLogicalName, courseExists.Id),
        ecer_ProgramAreaId = new EntityReference(ecer_ProvincialRequirement.EntityLogicalName, instructionArea.Id)
      };
      context.AddObject(newAreaOfInstruction);
    }
  }

  private void UpdateAreaOfInstruction(CourseAreaOfInstruction areaOfInstruction, ecer_CourseProvincialRequirement existingAreaOfInstruction)
  {
    var instructionArea =
      context.ecer_ProvincialRequirementSet.SingleOrDefault(r =>
        r.Id == Guid.Parse(areaOfInstruction.AreaOfInstructionId));

    if (instructionArea != null)
    {
      existingAreaOfInstruction.ecer_NewHours = Convert.ToDecimal(areaOfInstruction.NewHours);
      context.UpdateObject(existingAreaOfInstruction);
    }
  }

  public async Task<string> UpdateProgram(Program program, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var existingProgram = context.ecer_ProgramSet.SingleOrDefault(p => p.ecer_ProgramId == Guid.Parse(program.Id!));
    if (existingProgram == null) throw new InvalidOperationException($"ecer_Program '{program.Id}' not found");

    if (existingProgram.ecer_Type == ecer_ProgramProfileType.ChangeRequest &&
        existingProgram.StatusCode == ecer_Program_StatusCode.RequiresReview)
    {
      if (program.Status == ProgramStatus.Withdrawn)
      {
        existingProgram.StatusCode = ecer_Program_StatusCode.Withdrawn;
        existingProgram.StateCode = ecer_program_statecode.Inactive;
        
        context.UpdateObject(existingProgram);
      }
    }
    context.SaveChanges();
    return program.Id!;
  }
}
