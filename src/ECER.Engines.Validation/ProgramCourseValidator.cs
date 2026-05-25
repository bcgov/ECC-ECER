using ECER.Managers.Registry.Contract.Shared;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Engines.Validation;

internal static class ProgramCourseValidator
{
  public static List<string> ValidateProgramTypeCourses(
    IEnumerable<Course> allCourses,
    string programType,
    IReadOnlyCollection<AreaOfInstruction> instructions,
    bool checkTotalHours = false)
  {
    var errors = new List<string>();
    var courses = allCourses
      .Where(c => string.Equals(c.ProgramType, programType, StringComparison.OrdinalIgnoreCase))
      .ToList();

    if (courses.Count == 0)
    {
      errors.Add($"Must have courses for {programType} program type");
    }
    else
    {
      errors.AddRange(CheckForMinimumHours(courses, instructions, programType));
      if (checkTotalHours)
        errors.AddRange(CheckTotalCourseHours(courses, programType));
    }

    return errors;
  }

  public static List<string> CheckForMinimumHours(IEnumerable<Course> courses, IReadOnlyCollection<AreaOfInstruction> instructions, string programType)
  {
    var minHourErrors = new List<string>();
    var instructionsByType = instructions.Where(ins => ins.ProgramTypes.Contains(programType)).ToList();

    var hoursByAreaId = courses
      .Where(c => c.CourseAreaOfInstruction != null)
      .SelectMany(c => c.CourseAreaOfInstruction!)
      .Where(a => !string.IsNullOrWhiteSpace(a.NewHours))
      .GroupBy(a => a.AreaOfInstructionId)
      .ToDictionary(g => g.Key, g => g.Sum(a => decimal.Parse(a.NewHours!)));

    var childrenByParentId = instructionsByType
      .Where(ins => ins.ParentAreaOfInstructionId != null)
      .GroupBy(ins => ins.ParentAreaOfInstructionId!)
      .ToDictionary(g => g.Key, g => g.Select(ins => ins.Id).ToList());

    foreach (var instruction in instructionsByType)
    {
      var totalHours = hoursByAreaId.GetValueOrDefault(instruction.Id);

      if (childrenByParentId.TryGetValue(instruction.Id, out var childIds))
      {
        totalHours += childIds.Sum(childId => hoursByAreaId.GetValueOrDefault(childId));
      }

      if (instruction.MinimumHours != decimal.Zero && totalHours < instruction.MinimumHours)
      {
        minHourErrors.Add("Minimum hours are required for instruction: " + instruction.Name);
      }
    }
    return minHourErrors;
  }

  public static List<string> CheckTotalCourseHours(IEnumerable<Course> courses, string programType)
  {
    var totalHourErrors = new List<string>();

    var allInstructionsForCourse = courses.Where(c => c.CourseAreaOfInstruction != null)
      .SelectMany(c => c.CourseAreaOfInstruction!)
      .ToList();

    var totalHours = allInstructionsForCourse.Count == 0 ? 0
        : allInstructionsForCourse
        .Where(a => !string.IsNullOrWhiteSpace(a.NewHours))
        .Sum(a => decimal.Parse(a.NewHours!));

    if (totalHours < 450)
    {
      totalHourErrors.Add("Total course hours must hit the minimum total required hours for program: " + programType);
    }

    return totalHourErrors;
  }
}
