namespace ECER.Resources.Documents.Programs;

public interface IProgramRepository
{
  Task<IEnumerable<Program>> Query(ProgramQuery query, CancellationToken cancellationToken);

  Task<string> Save(Program program, CancellationToken cancellationToken);

  Task<ProgramDetail> GetProgramById(ProgramDetailQuery programQuery, CancellationToken cancellationToken);
}

public record ProgramQuery
{
  public string? ById { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
  public IEnumerable<ProgramStatus>? ByStatus { get; set; }
}
public record ProgramDetailQuery(string ProgramId, string PostSecondaryInstituteId);

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
  public CourseProgramType ProgramType { get; set; }
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
