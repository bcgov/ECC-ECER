using ECER.Resources.Documents.Programs;
using ECER.Resources.Documents.Shared;
using ECER.Utilities.DataverseSdk.Model;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.Courses;

internal interface ICourseRepositoryMapper
{
  List<Course> MapCourses(IEnumerable<ecer_Course> source);
  ecer_Course MapCourse(Course source);
  List<ecer_PSIProgramType> MapProgramTypes(IEnumerable<ProgramType> source);
}

[Mapper]
internal partial class CourseRepositoryMapper : ICourseRepositoryMapper
{
  public List<Course> MapCourses(IEnumerable<ecer_Course> source) => source.Select(MapCourse).ToList();

  public ecer_Course MapCourse(Course source) => new()
  {
    ecer_CourseId = Guid.Parse(source.CourseId),
    ecer_Code = source.CourseNumber,
    ecer_CourseName = source.CourseTitle,
    ecer_NewCode = source.NewCourseNumber,
    ecer_NewCourseName = source.NewCourseTitle,
    ecer_ProgramType = MapProgramType(source.ProgramType),
  };

  public List<ecer_PSIProgramType> MapProgramTypes(IEnumerable<ProgramType> source) => source.Select(MapProgramType).ToList();

  private Course MapCourse(ecer_Course source) => new()
  {
    CourseId = source.ecer_CourseId?.ToString() ?? string.Empty,
    CourseNumber = source.ecer_Code ?? string.Empty,
    CourseTitle = source.ecer_CourseName ?? string.Empty,
    NewCourseNumber = source.ecer_NewCode,
    NewCourseTitle = source.ecer_NewCourseName,
    CourseAreaOfInstruction = (source.ecer_courseprovincialrequirement_CourseId ?? Array.Empty<ecer_CourseProvincialRequirement>())
      .Select(MapCourseAreaOfInstruction)
      .ToList(),
    ProgramType = source.ecer_ProgramType?.ToString() ?? string.Empty,
  };

  private CourseAreaOfInstruction MapCourseAreaOfInstruction(ecer_CourseProvincialRequirement source) => new()
  {
    NewHours = source.ecer_NewHours.HasValue ? Convert.ToSingle(source.ecer_NewHours.Value) : 0,
    CourseAreaOfInstructionId = source.Id.ToString(),
    AreaOfInstructionId = source.ecer_ProgramAreaId?.Id.ToString() ?? string.Empty,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PSIProgramType MapProgramType(ProgramType source);

  private static ecer_PSIProgramType MapProgramType(string source) => Enum.Parse<ecer_PSIProgramType>(source);
}
