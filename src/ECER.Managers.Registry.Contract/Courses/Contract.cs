using ECER.Managers.Registry.Contract.Shared;
using MediatR;

namespace ECER.Managers.Registry.Contract.Courses;

public record UpdateCourseCommand(Course Course, string Id, string Type, string PostSecondaryInstituteId) : IRequest<string>;

public enum FunctionType
{
  ProgramProfile,
  ProgramApplication
}
