using ECER.Managers.Registry.Contract.Programs;
using ECER.Managers.Registry.Contract.Shared;
using Mediator;

namespace ECER.Managers.Registry.Contract.Courses;

public record SaveCourseCommand(Course Course, string Id, string PostSecondaryInstituteId) : IRequest<SaveCourseCommandResult>;
public record UpdateCourseCommand(Course Course, string Id, string Type, string PostSecondaryInstituteId) : IRequest<string>;
public record DeleteCourseCommand(string CourseId, string PostSecondaryInstituteId, string? ApplicationId = null) : IRequest<string>;
public record GetCoursesCommand(string Id, string PostSecondaryInstituteId, FunctionType Type) : IRequest<IEnumerable<Course>>
{
  public IEnumerable<ProgramTypes>? ProgramTypes { get; set; }
};

public enum FunctionType
{
  ProgramProfile,
  ProgramApplication
}
public record SaveCourseCommandResult()
{
  public string? CourseId { get; set; }
  public SaveCourseError? Error { get; set; }
}

public enum SaveCourseError
{
  ProgramApplicationNotFound,
  IncorrectProgramApplicationTypeToSaveCourse
}
