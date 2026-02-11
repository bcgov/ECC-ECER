using AutoMapper;
using ECER.Resources.Documents.Shared;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Xrm.Sdk;

namespace ECER.Resources.Documents.Courses;

internal sealed class CourseRepository : ICourseRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public CourseRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<string> AddCourse(Course incomingCourse, string id, string postSecondaryInstituteId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    
    if (string.IsNullOrWhiteSpace(postSecondaryInstituteId))
    {
      throw new InvalidOperationException("Post secondary institute id is required");
    }

    var instituteId = Guid.Parse(postSecondaryInstituteId);
    var institute = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(i => i.ecer_PostSecondaryInstituteId == instituteId);
    if (institute == null) throw new InvalidOperationException($"Post secondary institute '{postSecondaryInstituteId}' not found");
    
    var applicationId = Guid.Parse(id);
    var programApplication = context.ecer_PostSecondaryInstituteProgramApplicaitonSet
      .SingleOrDefault(p => p.ecer_PostSecondaryInstituteProgramApplicaitonId == applicationId);
    
    if (!string.IsNullOrWhiteSpace(incomingCourse.CourseNumber))
    {
      var coursesWithSameNumber = 
        context.ecer_CourseSet.AsQueryable().Where(p => 
            p.ecer_Code == incomingCourse.CourseNumber
            && p.ecer_ProgramApplication.Id == applicationId
            && p.ecer_programtypeName == incomingCourse.ProgramType
            )
          .Take(1)
          .ToList();

      if (coursesWithSameNumber.Count > 0)
      {
        throw new InvalidOperationException($"This course with course number {incomingCourse.CourseNumber} already exists");
      }
    }
    
    var ecerCourse = mapper.Map<ecer_Course>(incomingCourse)!;
    ecerCourse.Id = Guid.NewGuid();
    
    context.AddObject(ecerCourse);
    context.AddLink(ecerCourse, new Relationship(ecer_Course.Fields.ecer_ecer_postsecondaryinstitute_ecer_course_postsecondaryinstitution), institute);
    context.AddLink(ecerCourse, new Relationship(ecer_Course.Fields.ecer_course_ProgramApplication_ecer_postsecond), programApplication);
    
    context.SaveChanges();
    return ecerCourse.Id.ToString();
  }

  public async Task<string> UpdateCourse(IEnumerable<Course> incomingCourse, string id, bool isProgramApplication, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    foreach (var course in incomingCourse)
    {
      var courses = context.ecer_CourseSet.AsQueryable().Where(p => p.ecer_CourseId == Guid.Parse(course.CourseId));

      var courseExists = context.From(courses)
        .Join()
        .Include(c => c.ecer_courseprovincialrequirement_CourseId)
        .Execute()
        .SingleOrDefault();

      if (courseExists != null)
      {
        if (!context.IsAttached(courseExists))
        {
          context.Attach(courseExists);
        }

        UpdateCourseMetaData(course, courseExists, id, isProgramApplication);
        if (course.CourseAreaOfInstruction != null)
        {
          foreach (var areaOfInstruction in course.CourseAreaOfInstruction)
          {
            ManageAreaOfInstruction(areaOfInstruction, courseExists);
          }
        }
      }
      else
      {
        throw new InvalidOperationException($"Course not found");
      }
    }

    context.SaveChanges();
    return id;
  }

  private void ManageAreaOfInstruction(CourseAreaOfInstruction areaOfInstruction, ecer_Course courseExists){
    if (areaOfInstruction.CourseAreaOfInstructionId != null)
    {
      var existingAreaOfInstruction =
        courseExists.ecer_courseprovincialrequirement_CourseId.SingleOrDefault(a =>
          a.Id == Guid.Parse(areaOfInstruction.CourseAreaOfInstructionId));

      if (existingAreaOfInstruction is not null)
      {
        UpdateAreaOfInstruction(areaOfInstruction, existingAreaOfInstruction);
      }
      else
      {
        throw new InvalidOperationException("Cannot update course area of instruction. It does not exist.");
      }
    }
    else
    {
      var existingAreaOfInstruction =
        courseExists.ecer_courseprovincialrequirement_CourseId.SingleOrDefault(a =>
          a.ecer_ProgramAreaId.Id == Guid.Parse(areaOfInstruction.AreaOfInstructionId));
      if (existingAreaOfInstruction is not null)
      {
        throw new InvalidOperationException(
          "Cannot add course area of instruction. One already exists for this area of instruction.");
      }

      CreateNewAreaOfInstruction(areaOfInstruction, courseExists);
    }
  }

private void UpdateCourseMetaData(Course course, ecer_Course courseExists, string id, bool isProgramApplication)
  {
    if (!string.IsNullOrWhiteSpace(course.NewCourseNumber))
    {
      var coursesWithSameNumber = isProgramApplication ?
          context.ecer_CourseSet.AsQueryable().Where(p => 
              p.ecer_Code == course.NewCourseNumber 
              && p.ecer_CourseId != Guid.Parse(course.CourseId)
              && p.ecer_ProgramApplication.Id == Guid.Parse(id)
              && p.ecer_programtypeName == course.ProgramType
            )
            .Take(1)
            .ToList()
      :
        context.ecer_CourseSet.AsQueryable().Where(p => 
            p.ecer_Code == course.NewCourseNumber 
            && p.ecer_CourseId != Guid.Parse(course.CourseId)
            && p.ecer_course_Programid.Id == Guid.Parse(id)
            && p.ecer_programtypeName == course.ProgramType
            )
          .Take(1)
          .ToList();

      if (coursesWithSameNumber.Count > 0)
      {
        throw new InvalidOperationException("This course number already exists");
      }
    }
        
    courseExists.ecer_NewCode = !string.IsNullOrWhiteSpace(course.NewCourseNumber)
      ? course.NewCourseNumber
      : course.CourseNumber;
    courseExists.ecer_NewCourseName = !string.IsNullOrWhiteSpace(course.NewCourseTitle) 
      ? course.NewCourseTitle 
      : course.CourseTitle;
    context.UpdateObject(courseExists);
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
    else
    {
      throw new InvalidOperationException("Area of instruction not found");
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
    else
    {
      throw new InvalidOperationException("Area of instruction not found");
    }
  }
}
