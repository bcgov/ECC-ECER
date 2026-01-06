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

/// <summary>
/// Container for <see cref="ProgramsQuery"/> results
/// </summary>
/// <param name="Items"></param>
public record ProgramsQueryResults(IEnumerable<Program> Items);

public record ProgramDetailCommand(string ProgramId, string PostSecondaryInstituteId) : IRequest<ProgramDetail>;

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
}
public record ProgramDetail()
{
  public string? PostSecondaryInstituteName { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public IEnumerable<string>? ProgramTypes { get; set; }
  public IEnumerable<Course>? Courses { get; set; }
};

public record Course()
{
  public float Hours { get; set; }
  public string? Title { get; set; }
  public string? CourseNumber { get; set; }
  public CourseProgramType programType { get; set; }
  public IEnumerable<AreaOfInstruction>? AreaOfInstructions { get; set; }
}
public record AreaOfInstruction()
{
  public string? AreaOfInstructionName { get; set; }
  public float Hours { get; set; }
}

public enum ProgramStatus
{
  Draft,
  UnderReview,
  Approved,
  Denied,
  Inactive
}

public enum CourseProgramType
{
  ITE,
  SNE,
  Basic
}
