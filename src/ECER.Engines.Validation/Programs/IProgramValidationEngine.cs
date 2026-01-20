using ECER.Resources.Documents.MetadataResources;

namespace ECER.Engines.Validation.Programs;

public interface IProgramValidationEngine
{
  Task<ValidationResults> Validate(Managers.Registry.Contract.Programs.Program program,  IReadOnlyCollection<AreaOfInstruction> instructions);
}

public record ValidationResults(IEnumerable<string> ValidationErrors)
{
  public bool IsValid => !ValidationErrors.Any();
}
