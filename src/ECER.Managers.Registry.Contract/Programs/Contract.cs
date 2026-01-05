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

public record Course
{
  public string CourseNumber { get; set; } = null!;
  public string? CourseTitle { get; set; }
  public string? NewHours { get; set; }
  public string? AreaOfInstructionId { get; set; }
  public string? ProgramType { get; set; }
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
  Inactive
}
