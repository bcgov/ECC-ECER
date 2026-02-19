using AutoMapper;
using ECER.Managers.Registry.Contract.Courses;
using ECER.Managers.Registry.Contract.Shared;

namespace ECER.Managers.Registry;

internal class CoursesMapper : Profile
{
  public CoursesMapper()
  {
    CreateMap<Course, Resources.Documents.Shared.Course>()
      .ReverseMap();
    
    CreateMap<CourseAreaOfInstruction, Resources.Documents.Shared.CourseAreaOfInstruction>()
      .ReverseMap();
  }
}
