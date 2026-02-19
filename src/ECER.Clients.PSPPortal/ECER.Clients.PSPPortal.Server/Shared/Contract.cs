using System.ComponentModel.DataAnnotations;

namespace ECER.Clients.PSPPortal.Server.Shared;

public record ClaimCacheSettings
{
  public double CacheTimeInSeconds { get; set; } = 500;
}

public record UploaderSettings
{
  public string TempFolderName { get; set; } = string.Empty;
  public IEnumerable<string> AllowedFileTypes { get; set; } = Array.Empty<string>();
}

public record PaginationSettings
{
  public int DefaultPageSize { get; set; }
  public int DefaultPageNumber { get; set; }
  public string PageProperty { get; set; } = string.Empty;
  public string PageSizeProperty { get; set; } = string.Empty;
}

public record CourseAreaOfInstruction
{
  public string CourseAreaOfInstructionId { get; set; } = null!;
  public string? NewHours { get; set; }
  public string AreaOfInstructionId { get; set; } = null!;
}

public record Course
{
  [Required]
  public string CourseId { get; set; } = null!;
  [Required]
  public string CourseNumber { get; set; } = null!;
  [Required]
  public string CourseTitle { get; set; } = null!;
  public string? NewCourseNumber { get; set; }
  public string? NewCourseTitle { get; set; }
  public IEnumerable<CourseAreaOfInstruction>? CourseAreaOfInstruction { get; set; }
  [Required]
  public string ProgramType { get; set; } = null!;
}
