using ECER.Managers.Registry.Contract.Programs;
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

    if (program.OfferedProgramTypes.Contains(nameof(ProgramTypes.Basic)))
      validationErrors.AddRange(ProgramCourseValidator.ValidateProgramTypeCourses(program.Courses ?? [], nameof(ProgramTypes.Basic), instructions));

    if (program.OfferedProgramTypes.Contains(nameof(ProgramTypes.ITE)))
      validationErrors.AddRange(ProgramCourseValidator.ValidateProgramTypeCourses(program.Courses ?? [], nameof(ProgramTypes.ITE), instructions, checkTotalHours: true));

    if (program.OfferedProgramTypes.Contains(nameof(ProgramTypes.SNE)))
      validationErrors.AddRange(ProgramCourseValidator.ValidateProgramTypeCourses(program.Courses ?? [], nameof(ProgramTypes.SNE), instructions, checkTotalHours: true));

    return new ValidationResults(validationErrors);
  }
}
