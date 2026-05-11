using Riok.Mapperly.Abstractions;
using ContractPrograms = ECER.Managers.Registry.Contract.Programs;
using ContractShared = ECER.Managers.Registry.Contract.Shared;
using ResourcePrograms = ECER.Resources.Documents.Programs;
using ResourceShared = ECER.Resources.Documents.Shared;

namespace ECER.Managers.Registry;

public interface ICoursesMapper
{
  ResourceShared.Course MapCourse(ContractShared.Course source);
  IEnumerable<ContractShared.Course> MapCourses(IEnumerable<ResourceShared.Course> source);
  IEnumerable<ResourcePrograms.ProgramType> MapProgramTypes(IEnumerable<ContractPrograms.ProgramTypes> source);
}

[Mapper]
internal partial class CoursesMapper : ICoursesMapper
{
  public ResourceShared.Course MapCourse(ContractShared.Course source) => new()
  {
    CourseId = source.CourseId,
    CourseNumber = source.CourseNumber,
    CourseTitle = source.CourseTitle,
    NewCourseNumber = source.NewCourseNumber,
    NewCourseTitle = source.NewCourseTitle,
    CourseAreaOfInstruction = source.CourseAreaOfInstruction?.Select(MapCourseAreaOfInstruction).ToList(),
    ProgramType = source.ProgramType,
  };

  public IEnumerable<ContractShared.Course> MapCourses(IEnumerable<ResourceShared.Course> source) => source.Select(MapCourse).ToList();

  public IEnumerable<ResourcePrograms.ProgramType> MapProgramTypes(IEnumerable<ContractPrograms.ProgramTypes> source) => source.Select(MapProgramType).ToList();

  private static ContractShared.Course MapCourse(ResourceShared.Course source) => new()
  {
    CourseId = source.CourseId,
    CourseNumber = source.CourseNumber,
    CourseTitle = source.CourseTitle,
    NewCourseNumber = source.NewCourseNumber,
    NewCourseTitle = source.NewCourseTitle,
    CourseAreaOfInstruction = source.CourseAreaOfInstruction?.Select(MapCourseAreaOfInstruction).ToList(),
    ProgramType = source.ProgramType,
  };

  private static ResourceShared.CourseAreaOfInstruction MapCourseAreaOfInstruction(ContractShared.CourseAreaOfInstruction source) => new()
  {
    CourseAreaOfInstructionId = source.CourseAreaOfInstructionId,
    NewHours = string.IsNullOrWhiteSpace(source.NewHours) ? null : Convert.ToSingle(source.NewHours),
    AreaOfInstructionId = source.AreaOfInstructionId,
  };

  private static ContractShared.CourseAreaOfInstruction MapCourseAreaOfInstruction(ResourceShared.CourseAreaOfInstruction source) => new()
  {
    CourseAreaOfInstructionId = source.CourseAreaOfInstructionId ?? string.Empty,
    NewHours = source.NewHours?.ToString(),
    AreaOfInstructionId = source.AreaOfInstructionId,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourcePrograms.ProgramType MapProgramType(ContractPrograms.ProgramTypes source);
}
