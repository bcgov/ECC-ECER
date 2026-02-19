namespace ECER.Managers.Registry.Contract.Shared;

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
