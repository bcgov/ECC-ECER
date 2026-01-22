using ECER.Managers.Registry.Contract.Programs;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Engines.Validation.Programs;

internal sealed class NewProgramSubmissionValidationEngine : IProgramValidationEngine
{
  public async Task<ValidationResults> Validate(Program program, IReadOnlyCollection<AreaOfInstruction> instructions)
  {
    await Task.CompletedTask;
    var validationErrors = new List<string>();

    if (program.ProgramTypes == null || !program.ProgramTypes.Any())
    {
      validationErrors.Add("No program types provided");
      return new ValidationResults(validationErrors);
    }
   
    var basicCourses = program.Courses?.Where(c => c.ProgramType == nameof(ProgramTypes.Basic)).ToList();
    var iteCourses = program.Courses?.Where(c => c.ProgramType == nameof(ProgramTypes.ITE)).ToList();
    var sneCourses = program.Courses?.Where(c => c.ProgramType == nameof(ProgramTypes.SNE)).ToList();

    if (program.ProgramTypes.Contains(nameof(ProgramTypes.Basic)) && (basicCourses == null || basicCourses.Count == 0))
    {
      validationErrors.Add("Must have courses for BASIC program type");
    }
    
    if (program.ProgramTypes.Contains(nameof(ProgramTypes.ITE)) && (iteCourses == null || iteCourses.Count == 0))
    {
      validationErrors.Add("Must have courses for ITE program type");
    }
    
    if (program.ProgramTypes.Contains(nameof(ProgramTypes.SNE)) && (sneCourses == null || sneCourses.Count == 0))
    {
      validationErrors.Add("Must have courses for SNE program type");
    }
    
    if (program.ProgramTypes.Contains(nameof(ProgramTypes.Basic)) && basicCourses != null && basicCourses.Count > 0)
    {
      validationErrors.AddRange(CheckForMinimumHours(basicCourses, instructions));
    }

    if (program.ProgramTypes.Contains(nameof(ProgramTypes.ITE)) && iteCourses != null && iteCourses.Count > 0)
    {
      validationErrors.AddRange(CheckForMinimumHours(iteCourses, instructions));
      validationErrors.AddRange(CheckTotalCourseHours(iteCourses, nameof(ProgramTypes.ITE)));
    }
    
    if (program.ProgramTypes.Contains(nameof(ProgramTypes.SNE)) && sneCourses != null && sneCourses.Count > 0)
    {
      validationErrors.AddRange(CheckForMinimumHours(sneCourses, instructions));
      validationErrors.AddRange(CheckTotalCourseHours(sneCourses, nameof(ProgramTypes.SNE)));
    }
    return new ValidationResults(validationErrors);
  }

  public static List<string> CheckForMinimumHours(IReadOnlyCollection<Course> courses, IReadOnlyCollection<AreaOfInstruction> instructions)
  {
    var minHourErrors = new List<string>();
    
    foreach (var instruction in instructions)
    {
      var filteredInstructions = courses.Where(c => c.CourseAreaOfInstruction != null)
        .SelectMany(c => c.CourseAreaOfInstruction!)
        .Where(ins => ins.AreaOfInstructionId == instruction.Id)
        .ToList();

      if (filteredInstructions.Count != 0)
      {
        var totalHours = filteredInstructions
          .Where(a => !string.IsNullOrWhiteSpace(a.NewHours))
          .Sum(a => decimal.Parse(a.NewHours!));
      
        if (instruction.MinimumHours != decimal.Zero && totalHours < instruction.MinimumHours)
        {
          minHourErrors.Add("Minimum hours are required for instruction: " + instruction.Name);
        } else if(instruction.MinimumHours == decimal.Zero && totalHours <= decimal.Zero)
        {
          minHourErrors.Add("Total hours must be greater than zero: " + instruction.Name);
        }
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

    if (allInstructionsForCourse.Count != 0)
    {
      var totalHours = allInstructionsForCourse?
        .Where(a => !string.IsNullOrWhiteSpace(a.NewHours))
        .Sum(a => decimal.Parse(a.NewHours!)) ?? 0;
      
      if (totalHours < 450)
      {
        totalHourErrors.Add("Total course hours must hit the minimum total required hours for program: " + programType);
      }
    }
    
    return totalHourErrors;
  }
}
