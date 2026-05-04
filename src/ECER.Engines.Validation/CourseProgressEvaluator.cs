using ECER.Managers.Registry.Contract.Shared;
using ECER.Resources.Documents.MetadataResources;

namespace ECER.Engines.Validation;

internal sealed class CourseProgressEvaluator : ICourseProgressEvaluator
{
  public string EvaluateProgress(
    IEnumerable<Course> courses,
    string programType,
    IReadOnlyCollection<AreaOfInstruction> areasOfInstruction,
    bool checkTotalHours)
  {
    var courseList = courses.ToList();
    var hasAny = courseList.Any(c => string.Equals(c.ProgramType, programType, StringComparison.OrdinalIgnoreCase));
    if (!hasAny) return "ToDo";

    var errors = ProgramCourseValidator.ValidateProgramTypeCourses(courseList, programType, areasOfInstruction, checkTotalHours);
    return errors.Count == 0 ? "Completed" : "InProgress";
  }
}
