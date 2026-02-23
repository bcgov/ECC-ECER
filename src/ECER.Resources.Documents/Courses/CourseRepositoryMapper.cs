using AutoMapper;
using ECER.Resources.Documents.Shared;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.Courses;

internal class CourseRepositoryMapper : Profile
{
  public CourseRepositoryMapper()
  {
    CreateMap<ecer_Course, Course>(MemberList.Destination)
      .ForMember(d => d.CourseId, opts => opts.MapFrom(s => s.ecer_CourseId))
      .ForMember(d => d.CourseNumber, opts => opts.MapFrom(s => s.ecer_Code))
      .ForMember(d => d.CourseTitle, opts => opts.MapFrom(s => s.ecer_CourseName))
      .ForMember(d => d.NewCourseNumber, opts => opts.MapFrom(s => s.ecer_NewCode))
      .ForMember(d => d.NewCourseTitle, opts => opts.MapFrom(s => s.ecer_NewCourseName))
      .ForMember(d => d.CourseAreaOfInstruction, opts => opts.MapFrom(s => s.ecer_courseprovincialrequirement_CourseId))
      .ForMember(d => d.ProgramType, opts => opts.MapFrom(s => s.ecer_ProgramType))
      .ReverseMap();

    CreateMap<ecer_CourseProvincialRequirement, CourseAreaOfInstruction>(MemberList.Destination)
      .ForMember(d => d.NewHours, opts => opts.MapFrom(s => s.ecer_NewHours))
      .ForMember(d => d.CourseAreaOfInstructionId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.AreaOfInstructionId, opts => opts.MapFrom(s => s.ecer_ProgramAreaId.Id))
      .ReverseMap();
  }
}
