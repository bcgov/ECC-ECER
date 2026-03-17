using AutoMapper;
using ECER.Clients.PSPPortal.Server.Shared;
using ECER.Infrastructure.Common;

namespace ECER.Clients.PSPPortal.Server.Courses;

internal sealed class CoursesMapper : SecureProfile
{
  public CoursesMapper()
  {
    CreateMap<Course, Managers.Registry.Contract.Shared.Course>()
      .ReverseMap();

    CreateMap<CourseAreaOfInstruction, Managers.Registry.Contract.Shared.CourseAreaOfInstruction>()
      .ReverseMap();
  }
}
