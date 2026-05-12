using ECER.Clients.PSPPortal.Server.Shared;
using Riok.Mapperly.Abstractions;
using PortalPrograms = ECER.Clients.PSPPortal.Server.Programs;
using ContractPrograms = ECER.Managers.Registry.Contract.Programs;
using ContractShared = ECER.Managers.Registry.Contract.Shared;

namespace ECER.Clients.PSPPortal.Server.Courses;

internal interface ICoursesMapper
{
  ContractShared.Course MapCourse(Course? source);
  IEnumerable<Course> MapCourses(IEnumerable<ContractShared.Course> source);
  ContractPrograms.ProgramTypes[]? MapProgramTypes(PortalPrograms.ProgramTypes[]? source);
}

[Mapper]
internal partial class CoursesMapper : ICoursesMapper
{
  public ContractShared.Course MapCourse(Course? source)
  {
    if (source == null)
    {
      return null!;
    }

    return new ContractShared.Course()
    {
      CourseId = source.CourseId!,
      CourseNumber = source.CourseNumber,
      CourseTitle = source.CourseTitle,
      NewCourseNumber = source.NewCourseNumber,
      NewCourseTitle = source.NewCourseTitle,
      CourseAreaOfInstruction = source.CourseAreaOfInstruction?.Select(MapCourseAreaOfInstruction).ToList(),
      ProgramType = source.ProgramType,
    };
  }

  public IEnumerable<Course> MapCourses(IEnumerable<ContractShared.Course> source) => source.Select(MapCourse).ToList();

  public ContractPrograms.ProgramTypes[]? MapProgramTypes(PortalPrograms.ProgramTypes[]? source) => source?.Select(MapProgramType).ToArray();

  private Course MapCourse(ContractShared.Course source) => new()
  {
    CourseId = source.CourseId,
    CourseNumber = source.CourseNumber,
    CourseTitle = source.CourseTitle,
    NewCourseNumber = source.NewCourseNumber,
    NewCourseTitle = source.NewCourseTitle,
    CourseAreaOfInstruction = source.CourseAreaOfInstruction?.Select(MapCourseAreaOfInstruction).ToList(),
    ProgramType = source.ProgramType,
  };

  private static ContractShared.CourseAreaOfInstruction MapCourseAreaOfInstruction(CourseAreaOfInstruction source) => new()
  {
    CourseAreaOfInstructionId = source.CourseAreaOfInstructionId,
    NewHours = source.NewHours,
    AreaOfInstructionId = source.AreaOfInstructionId,
  };

  private static CourseAreaOfInstruction MapCourseAreaOfInstruction(ContractShared.CourseAreaOfInstruction source) => new()
  {
    CourseAreaOfInstructionId = source.CourseAreaOfInstructionId,
    NewHours = source.NewHours,
    AreaOfInstructionId = source.AreaOfInstructionId,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPrograms.ProgramTypes MapProgramType(PortalPrograms.ProgramTypes source);
}
