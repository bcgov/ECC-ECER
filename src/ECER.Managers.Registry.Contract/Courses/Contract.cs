using ECER.Managers.Registry.Contract.Programs;
using ECER.Managers.Registry.Contract.Shared;
using MediatR;

namespace ECER.Managers.Registry.Contract.Courses;

public record SaveCourseCommand(Course Course, string Id, string PostSecondaryInstituteId) : IRequest<string>;
public record UpdateCourseCommand(Course Course, string Id, string Type, string PostSecondaryInstituteId) : IRequest<string>;
public record DeleteCourseCommand(string CourseId, string PostSecondaryInstituteId) : IRequest<string>;
public record GetCoursesCommand(string Id, string PostSecondaryInstituteId, FunctionType Type) : IRequest<IEnumerable<Course>>
{
  public IEnumerable<ProgramTypes>? ProgramTypes { get; set; }
};

public enum FunctionType
{
  ProgramProfile,
  ProgramApplication
}
