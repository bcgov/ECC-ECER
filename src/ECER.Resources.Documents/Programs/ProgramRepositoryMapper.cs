using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.Programs;

internal class ProgramRepositoryMapper : Profile
{
  public ProgramRepositoryMapper()
  {
    CreateMap<Program, ecer_Program>(MemberList.Source)
      .ForSourceMember(s => s.CreatedOn, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.PostSecondaryInstituteId, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.PostSecondaryInstituteName, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ProgramTypes, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.Courses, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_ProgramId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.ecer_Name, opts => opts.MapFrom(s => s.Name))
      .ForMember(d => d.ecer_PortalStage, opts => opts.MapFrom(s => s.PortalStage))
      .ForMember(d => d.ecer_PostSecondaryInstitution, opts => opts.Ignore())
      .ForMember(d => d.ecer_StartDate, opts => opts.MapFrom(s => s.StartDate))
      .ForMember(d => d.ecer_EndDate, opts => opts.MapFrom(s => s.EndDate))
      .ForMember(d => d.ecer_ProgramTypes, opts => opts.MapFrom(s => s.ProgramTypes != null ? s.ProgramTypes.Select(t => Enum.Parse<ecer_PSIProgramType>(t)) : null))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination)
      .ForCtorParam(nameof(Program.Id), opts => opts.MapFrom(s => s.ecer_ProgramId.HasValue ? s.ecer_ProgramId.Value.ToString() : null))
      .ForCtorParam(nameof(Program.PostSecondaryInstituteId), opts => opts.MapFrom(s => s.ecer_PostSecondaryInstitution == null ? string.Empty : s.ecer_PostSecondaryInstitution.Id.ToString()))
      .ForMember(d => d.PortalStage, opts => opts.MapFrom(s => s.ecer_PortalStage))
      .ForMember(d => d.CreatedOn, opts => opts.MapFrom(s => s.CreatedOn))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.ecer_Name))
      .ForMember(d => d.PostSecondaryInstituteName, opts => opts.MapFrom(s => s.ecer_PostSecondaryInstitutionName))
      .ForMember(d => d.StartDate, opts => opts.MapFrom(s => s.ecer_StartDate))
      .ForMember(d => d.EndDate, opts => opts.MapFrom(s => s.ecer_EndDate))
      .ForMember(d => d.Courses, opts => opts.MapFrom(s => s.ecer_course_Programid))
      .ForMember(d => d.ProgramTypes, opts => opts.MapFrom(s => s.ecer_ProgramTypes != null ? s.ecer_ProgramTypes.Select(t => t.ToString()) : null));

    CreateMap<ProgramStatus, ecer_Program_StatusCode>()
      .ConvertUsing(status =>
          status == ProgramStatus.Draft ? ecer_Program_StatusCode.RequiresReview :
          status == ProgramStatus.UnderReview ? ecer_Program_StatusCode.UnderRegistryReview :
          status == ProgramStatus.Approved ? ecer_Program_StatusCode.RegistryReviewComplete :
          status == ProgramStatus.Denied ? ecer_Program_StatusCode.Denied :
          status == ProgramStatus.Inactive ? ecer_Program_StatusCode.Inactive :
          status == ProgramStatus.ChangeRequestInProgress ? ecer_Program_StatusCode.ChangeRequestInProgress :
                                                ecer_Program_StatusCode.RequiresReview);

    CreateMap<ecer_Program_StatusCode, ProgramStatus>()
      .ConvertUsing(status =>
          status == ecer_Program_StatusCode.RequiresReview ? ProgramStatus.Draft :
          status == ecer_Program_StatusCode.UnderRegistryReview ? ProgramStatus.UnderReview :
          status == ecer_Program_StatusCode.RegistryReviewComplete ? ProgramStatus.Approved :
          status == ecer_Program_StatusCode.Denied ? ProgramStatus.Denied :
          status == ecer_Program_StatusCode.Inactive ? ProgramStatus.Inactive :
          status == ecer_Program_StatusCode.ChangeRequestInProgress ? ProgramStatus.ChangeRequestInProgress :
                                                                     ProgramStatus.Draft);

    CreateMap<ecer_Course, Course>(MemberList.Destination)
      .ForMember(d => d.CourseNumber, opts => opts.MapFrom(s => s.ecer_Code))
      .ForMember(d => d.CourseTitle, opts => opts.MapFrom(s => s.ecer_CourseName))
      .ForMember(d => d.CourseAreaOfInstruction, opts => opts.MapFrom(s => s.ecer_courseprovincialrequirement_CourseId))
      .ForMember(d => d.ProgramType, opts => opts.MapFrom(s => s.ecer_ProgramType));

    CreateMap<ecer_CourseProvincialRequirement, CourseAreaOfInstruction>(MemberList.Destination)
      .ForMember(d => d.NewHours, opts => opts.MapFrom(s => s.ecer_NewHours != null ? Convert.ToString(s.ecer_NewHours) : "0.00"))
      .ForMember(d => d.CourseAreaOfInstructionId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.AreaOfInstructionId, opts => opts.MapFrom(s => s.ecer_ProgramAreaId.Id))
      .ReverseMap();
  }
}
