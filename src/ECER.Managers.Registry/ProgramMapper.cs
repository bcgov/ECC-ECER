using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Course = ECER.Resources.Documents.Shared.Course;
using CourseAreaOfInstruction = ECER.Resources.Documents.Shared.CourseAreaOfInstruction;
using Program = ECER.Resources.Documents.Programs.Program;
using ProgramProfileType = ECER.Resources.Documents.Programs.ProgramProfileType;
using ProgramStatus = ECER.Resources.Documents.Programs.ProgramStatus;

namespace ECER.Managers.Registry;

internal class ProgramMapper : Profile
{
  public ProgramMapper()
  {
    CreateMap<Contract.Programs.Program, Program>()
      .ForCtorParam(nameof(Program.Id), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(Program.PostSecondaryInstituteId), opts => opts.MapFrom(s => s.PostSecondaryInstituteId))
      .ForMember(d => d.PortalStage, opts => opts.MapFrom(s => s.PortalStage))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.ProgramProfileType, opts => opts.MapFrom(s => s.ProgramProfileType))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.Name))
      .ForMember(d => d.FromProgramProfileId, opts => opts.MapFrom(s => s.FromProgramProfileId))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination)
      .ForCtorParam(nameof(Contract.Programs.Program.PostSecondaryInstituteId), opts => opts.MapFrom(s => s.PostSecondaryInstituteId));

    CreateMap<Contract.Programs.ProgramStatus, ProgramStatus>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Contract.Programs.ProgramProfileType, ProgramProfileType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
  }
}
