using ECER.Managers.Registry.Contract.Shared;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Engines.Validation;

/// <summary>Evaluates per-type course completion progress for a program application.</summary>
public interface ICourseProgressEvaluator
{
  /// <summary>Returns "To-Do", "InProgress", or "Completed" for the given program type based on its courses.</summary>
  string EvaluateProgress(
    IEnumerable<Course> courses,
    string programType,
    IReadOnlyCollection<AreaOfInstruction> areasOfInstruction,
    bool checkTotalHours);
}
