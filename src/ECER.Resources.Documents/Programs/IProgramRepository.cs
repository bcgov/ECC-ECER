namespace ECER.Resources.Documents.Programs;

public interface IProgramRepository
{
  Task<IEnumerable<Program>> Query(ProgramQuery query, CancellationToken cancellationToken);

  Task<string> Save(Program program, CancellationToken cancellationToken);
  
  Task<string> UpdateCourse(IEnumerable<Course> incomingCourse, string id, CancellationToken cancellationToken);
}

public record ProgramQuery
{
  public string? ById { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
  public IEnumerable<ProgramStatus>? ByStatus { get; set; }
}

public record Course
{
  public string CourseId { get; set; } = null!;
  public string CourseNumber { get; set; } = null!;
  public string CourseTitle { get; set; } = null!;
  public string? NewCourseNumber { get; set; } 
  public string? NewCourseTitle { get; set; } 
  public IEnumerable<CourseAreaOfInstruction>? CourseAreaOfInstruction { get; set; }
  public string ProgramType { get; set; } = null!;
}

public record CourseAreaOfInstruction()
{
  public string CourseAreaOfInstructionId { get; set; } = null!;
  public string? NewHours { get; set; }
  public string AreaOfInstructionId { get; set; } = null!;
}

public record Program(string? Id, string PostSecondaryInstituteId)
{
  public string? PortalStage { get; set; }
  public ProgramStatus Status { get; set; }
  public DateTime? CreatedOn { get; set; }
  public string? Name { get; set; }
  public string? PostSecondaryInstituteName { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public IEnumerable<string>? ProgramTypes { get; set; }
  public IEnumerable<Course>? Courses { get; set; }
}

public enum ProgramStatus
{
  Draft,
  UnderReview,
  Approved,
  Denied,
  Inactive,
  ChangeRequestInProgress
}
