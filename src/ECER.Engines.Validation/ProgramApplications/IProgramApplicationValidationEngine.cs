using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Managers.Registry.Contract.Shared;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Engines.Validation.ProgramApplications;

public interface IProgramApplicationValidationEngine
{
  Task<ProgramApplicationValidationResults> Validate(ProgramApplicationValidationContext context);
}

public record ProgramApplicationValidationResults(IEnumerable<string> ValidationErrors)
{
  public bool IsValid => !ValidationErrors.Any();
}

public record ProgramApplicationValidationContext(
  IEnumerable<ComponentGroupValidationStatus> ComponentGroupStatuses,
  IEnumerable<Course> Courses,
  ProgramApplication ProgramApplication,
  IReadOnlyCollection<AreaOfInstruction> AreasOfInstruction,
  bool DeclarationAccepted);

public record ComponentGroupValidationStatus(string Id, string Name, string Status);
