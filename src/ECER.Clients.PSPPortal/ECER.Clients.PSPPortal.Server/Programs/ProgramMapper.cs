using AutoMapper;
using AutoMapper.Extensions.EnumMapping;

namespace ECER.Clients.PSPPortal.Server.Programs;

internal sealed class ProgramMapper : Profile
{
  public ProgramMapper()
  {
    CreateMap<Program, Managers.Registry.Contract.Programs.Program>()
      .ForCtorParam(nameof(Managers.Registry.Contract.Programs.Program.Id), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(Managers.Registry.Contract.Programs.Program.PostSecondaryInstituteId), opts => opts.MapFrom(_ => string.Empty))
      .ForMember(d => d.PortalStage, opts => opts.MapFrom(s => s.PortalStage))
      .ForMember(d => d.Status, opts => opts.MapFrom(_ => Managers.Registry.Contract.Programs.ProgramStatus.Draft))
      .ForMember(d => d.CreatedOn, opts => opts.MapFrom(s => s.CreatedOn))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.Name))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);

    CreateMap<ProgramStatus, Managers.Registry.Contract.Programs.ProgramStatus>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Course, Managers.Registry.Contract.Programs.Course>()
      .ReverseMap();
    
    CreateMap<AreaOfInstruction, Managers.Registry.Contract.Programs.AreaOfInstruction>()
      .ReverseMap();
  }
}
