using ECER.Managers.Registry.Contract.ProgramApplications;

namespace ECER.Engines.Validation.ProgramApplications;

internal sealed class ProgramApplicationSubmissionValidationEngine : IProgramApplicationValidationEngine
{
  public async Task<ProgramApplicationValidationResults> Validate(ProgramApplicationValidationContext context)
  {
    await Task.CompletedTask;
    var errors = new List<string>();

    // a) All component groups must be completed
    var incompleteGroups = context.ComponentGroupStatuses
      .Where(g => !string.Equals(g.Status, "Completed", StringComparison.OrdinalIgnoreCase))
      .ToList();

    foreach (var group in incompleteGroups)
    {
      errors.Add($"{group.Name} has not been completed");
    }

    // b) Course validation
    if (context.ProgramTypes.Contains(ProgramCertificationType.Basic))
      errors.AddRange(ProgramCourseValidator.ValidateProgramTypeCourses(context.Courses, nameof(ProgramCertificationType.Basic), context.AreasOfInstruction));

    if (context.ProgramTypes.Contains(ProgramCertificationType.ITE))
      errors.AddRange(ProgramCourseValidator.ValidateProgramTypeCourses(context.Courses, nameof(ProgramCertificationType.ITE), context.AreasOfInstruction, checkTotalHours: true));

    if (context.ProgramTypes.Contains(ProgramCertificationType.SNE))
      errors.AddRange(ProgramCourseValidator.ValidateProgramTypeCourses(context.Courses, nameof(ProgramCertificationType.SNE), context.AreasOfInstruction, checkTotalHours: true));

    // c) Declaration must be accepted
    if (!context.DeclarationAccepted)
    {
      errors.Add("Declaration must be accepted to submit the program application");
    }

    return new ProgramApplicationValidationResults(errors);
  }
}
