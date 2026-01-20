using ECER.Managers.Registry.Contract.Programs;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Engines.Validation.Programs;

internal sealed class NewProgramSubmissionValidationEngine : IProgramValidationEngine
{
  public async Task<ValidationResults> Validate(Program program, IReadOnlyCollection<AreaOfInstruction> instructions)
  {
    await Task.CompletedTask;
    var validationErrors = new List<string>();

    var basicCourses = program.Courses?.Where(c => c.ProgramType == nameof(ProgramTypes.Basic)).ToList();
    var iteCourses = program.Courses?.Where(c => c.ProgramType == nameof(ProgramTypes.ITE)).ToList();
    var sneCourses = program.Courses?.Where(c => c.ProgramType == nameof(ProgramTypes.SNE)).ToList();

    if (basicCourses != null)
    {
      validationErrors.AddRange(CheckForMinimumHours(basicCourses, instructions));
    }

    if (iteCourses != null)
    {
      validationErrors.AddRange(CheckForMinimumHours(iteCourses, instructions));
      validationErrors.AddRange(CheckTotalCourseHours(iteCourses));
      validationErrors.AddRange(CheckForMinimumAreaOfInstruction(iteCourses, instructions, nameof(ProgramTypes.ITE)));
    }
    
    if (sneCourses != null)
    {
      validationErrors.AddRange(CheckForMinimumHours(sneCourses, instructions));
      validationErrors.AddRange(CheckTotalCourseHours(sneCourses));
      validationErrors.AddRange(CheckForMinimumAreaOfInstruction(sneCourses, instructions, nameof(ProgramTypes.SNE)));
    }
    return new ValidationResults(validationErrors);
  }

  public static List<string> CheckForMinimumHours(IEnumerable<Course> courses, IReadOnlyCollection<AreaOfInstruction> instructions)
  {
    var minHourErrors = new List<string>();
    
    foreach (var course in courses)
    {
      var areaOfInstructions = course.CourseAreaOfInstruction;
      if (areaOfInstructions != null)
      {
        foreach (var areaOfInstruction in areaOfInstructions)
        {
          var provInstructions =
            instructions.FirstOrDefault(ins => Guid.Parse(ins.Id) == Guid.Parse(areaOfInstruction.AreaOfInstructionId));

          if (provInstructions == null)
          {
            minHourErrors.Add("Area of instruction not found: " + areaOfInstruction.AreaOfInstructionId);
          }

          if (areaOfInstruction.NewHours != null && provInstructions != null && provInstructions.MinimumHours != decimal.Zero &&
              decimal.Parse(areaOfInstruction.NewHours) < provInstructions.MinimumHours)
          {
            minHourErrors.Add("Minimum hours are required for course: " + course.NewCourseTitle);
          }
        }
      }
    }
    return minHourErrors;
  }
  
  public static List<string> CheckTotalCourseHours(IEnumerable<Course> courses)
  {
    var totalHourErrors = new List<string>();
    
    foreach (var course in courses)
    {
      var areaOfInstructions = course.CourseAreaOfInstruction;
      if (areaOfInstructions != null)
      {
        var totalHours = course.CourseAreaOfInstruction?
          .Where(a => !string.IsNullOrWhiteSpace(a.NewHours))
          .Sum(a => decimal.Parse(a.NewHours!)) ?? 0;

        if (totalHours < 450)
        {
          totalHourErrors.Add("Total course hours must hit the minimum total required hours for course: " + course.NewCourseTitle);
        }
      }
    }
    return totalHourErrors;
  }
  
  public static List<string> CheckForMinimumAreaOfInstruction(IEnumerable<Course> courses, IReadOnlyCollection<AreaOfInstruction> instructions, string programType)
  {
    var minHourErrors = new List<string>();

    var areaOfInstructionsByType = instructions.Where(ins => ins.ProgramTypes.SingleOrDefault(program => program == programType) != null);
    
    foreach (var course in courses)
    {
      var areaOfInstructions = course.CourseAreaOfInstruction;
      if (areaOfInstructions != null)
      {
        bool hasInstructionWithHours = areaOfInstructions.Any(area => areaOfInstructionsByType.Any(typeA => Guid.Parse(typeA.Id) == Guid.Parse(area.AreaOfInstructionId)) 
                                                  && area.NewHours != null
                                                  && decimal.Parse(area.NewHours) > 0);
        if (!hasInstructionWithHours)
        {
          minHourErrors.Add("For each Area of Instruction, the total must be greater than 0: " + course.NewCourseTitle);
        }
      }
    }
    return minHourErrors;
  }
  
}
