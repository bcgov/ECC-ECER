using MediatR;

namespace ECER.Managers.Registry.Contract.Programs;

/// <summary>
/// Invokes draft program saving use case
/// </summary>
public record SaveDraftProgramCommand(Program Program) : IRequest<Program?>;

/// <summary>
/// Invokes program query use case
/// </summary>
public record ProgramsQuery : IRequest<ProgramsQueryResults>
{
  public string? ById { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
  public IEnumerable<ProgramStatus>? ByStatus { get; set; }
}

public record UpdateCourseCommand(IEnumerable<Course> Course, string Id) : IRequest<string>;
public record UpdateProgramCommand(Program Program) : IRequest<string>;
public record SubmitProgramCommand(string ProgramId, string UserId) : IRequest<SubmitProgramResult>;

public record SubmitProgramResult
{
  public string? ProgramId { get; set; }
  public ProgramSubmissionError? Error { get; set; }
  public IEnumerable<string>? ValidationErrors { get; set; }
}

public enum ProgramSubmissionError
{
  DraftApplicationNotFound,
  DraftApplicationValidationFailed
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

public record CourseAreaOfInstruction
{
  public string CourseAreaOfInstructionId { get; set; } = null!;
  public string? NewHours { get; set; }
  public string AreaOfInstructionId { get; set; } = null!;
}

/// <summary>
/// Container for <see cref="ProgramsQuery"/> results
/// </summary>
/// <param name="Items"></param>
public record ProgramsQueryResults(IEnumerable<Program> Items);

public record Program(string? Id, string PostSecondaryInstituteId)
{
  public string? PortalStage { get; set; }
  public ProgramStatus Status { get; set; }
  public DateTime? CreatedOn { get; set; }
  public string? Name { get; set; }
  public string? ProgramName { get; set; }
  public string? PostSecondaryInstituteName { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public string? NewBasicTotalHours { get; set; }
  public string? NewSneTotalHours { get; set; }
  public string? NewIteTotalHours { get; set; }
  public string? DeclarationDate { get; set; }
  public string? DeclarationUserName { get; set; }
  public ProgramProfileType ProgramProfileType { get; set; }
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
  ChangeRequestInProgress,
  Withdrawn
}

public enum ProgramProfileType
{
  ChangeRequest,
  AnnualReview
}

public enum ProgramTypes
{
  Basic,
  SNE,
  ITE
}
