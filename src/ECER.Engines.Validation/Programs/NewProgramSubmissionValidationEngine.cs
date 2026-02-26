using ECER.Managers.Registry.Contract.Programs;
using ECER.Managers.Registry.Contract.Shared;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Engines.Validation.Programs;

internal sealed class NewProgramSubmissionValidationEngine : IProgramValidationEngine
{
  public async Task<ValidationResults> Validate(Program program, IReadOnlyCollection<AreaOfInstruction> instructions)
  {
    await Task.CompletedTask;
    var validationErrors = new List<string>();

    ArgumentNullException.ThrowIfNull(program.OfferedProgramTypes);

    if (program.ProgramTypes == null || !program.ProgramTypes.Any())
    {
      validationErrors.Add("No program types authorized");
    }

    foreach (var offeredProgramType in program.OfferedProgramTypes)
    {
      if (program.ProgramTypes != null && !program.ProgramTypes.Contains(offeredProgramType))
      {
        validationErrors.Add($"Not authorized to provide {offeredProgramType} program type");
      }
    }

    var basicCourses = program.Courses?.Where(c => c.ProgramType == nameof(ProgramTypes.Basic)).ToList();
    var iteCourses = program.Courses?.Where(c => c.ProgramType == nameof(ProgramTypes.ITE)).ToList();
    var sneCourses = program.Courses?.Where(c => c.ProgramType == nameof(ProgramTypes.SNE)).ToList();

    if (program.OfferedProgramTypes.Contains(nameof(ProgramTypes.Basic)) && (basicCourses == null || basicCourses.Count == 0))
    {
      validationErrors.Add("Must have courses for BASIC program type");
    }

    if (program.OfferedProgramTypes.Contains(nameof(ProgramTypes.ITE)) && (iteCourses == null || iteCourses.Count == 0))
    {
      validationErrors.Add("Must have courses for ITE program type");
    }

    if (program.OfferedProgramTypes.Contains(nameof(ProgramTypes.SNE)) && (sneCourses == null || sneCourses.Count == 0))
    {
      validationErrors.Add("Must have courses for SNE program type");
    }

    if (program.OfferedProgramTypes.Contains(nameof(ProgramTypes.Basic)) && basicCourses != null && basicCourses.Count > 0)
    {
      validationErrors.AddRange(CheckForMinimumHours(basicCourses, instructions, nameof(ProgramTypes.Basic)));
    }

    if (program.OfferedProgramTypes.Contains(nameof(ProgramTypes.ITE)) && iteCourses != null && iteCourses.Count > 0)
    {
      validationErrors.AddRange(CheckForMinimumHours(iteCourses, instructions, nameof(ProgramTypes.ITE)));
      validationErrors.AddRange(CheckTotalCourseHours(iteCourses, nameof(ProgramTypes.ITE)));
    }

    if (program.OfferedProgramTypes.Contains(nameof(ProgramTypes.SNE)) && sneCourses != null && sneCourses.Count > 0)
    {
      validationErrors.AddRange(CheckForMinimumHours(sneCourses, instructions, nameof(ProgramTypes.SNE)));
      validationErrors.AddRange(CheckTotalCourseHours(sneCourses, nameof(ProgramTypes.SNE)));
    }
    return new ValidationResults(validationErrors);
  }

  public static List<string> CheckForMinimumHours(IReadOnlyCollection<Course> courses, IReadOnlyCollection<AreaOfInstruction> instructions, string programType)
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
      else if (instruction.MinimumHours == decimal.Zero && totalHours <= decimal.Zero && instruction.ParentAreaOfInstructionId == null)
      {
        minHourErrors.Add("Total hours must be greater than zero: " + instruction.Name);
      }
    }
    return minHourErrors;
  }

  public static List<string> CheckTotalCourseHours(IReadOnlyCollection<Course> courses, string programType)
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
