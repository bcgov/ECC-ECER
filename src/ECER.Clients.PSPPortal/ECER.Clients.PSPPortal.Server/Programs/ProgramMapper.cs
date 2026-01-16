using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using System;
using System.Linq;

namespace ECER.Clients.PSPPortal.Server.Programs;

internal sealed class ProgramMapper : Profile
{
  public ProgramMapper()
  {
    CreateMap<Program, Managers.Registry.Contract.Programs.Program>()
      .ForCtorParam(nameof(Managers.Registry.Contract.Programs.Program.Id), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(Managers.Registry.Contract.Programs.Program.PostSecondaryInstituteId), opts => opts.MapFrom(_ => string.Empty))
      .ForMember(d => d.PortalStage, opts => opts.MapFrom(s => s.PortalStage))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.CreatedOn, opts => opts.MapFrom(s => s.CreatedOn))
      .ForMember(d => d.ProgramProfileType, opts => opts.MapFrom(s => s.ProgramProfileType))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.Name))
      .ForMember(d => d.ProgramTypes, opts => opts.MapFrom(s => s.ProgramTypes != null ? s.ProgramTypes.Select(type => type.ToString()) : null))
      .ReverseMap()
      .ForMember(d => d.ProgramTypes, opts => opts.MapFrom(s => ParseProgramTypes(s.ProgramTypes)))
      .ValidateMemberList(MemberList.Destination);

    CreateMap<ProgramStatus, Managers.Registry.Contract.Programs.ProgramStatus>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<ProgramProfileType, Managers.Registry.Contract.Programs.ProgramProfileType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Course, Managers.Registry.Contract.Programs.Course>()
      .ReverseMap();
    
    CreateMap<CourseAreaOfInstruction, Managers.Registry.Contract.Programs.CourseAreaOfInstruction>()
      .ReverseMap();
  }

  private static ProgramTypes[]? ParseProgramTypes(IEnumerable<string>? programTypes)
  {
    if (programTypes == null)
    {
      return null;
    }

    return programTypes
      .Select(type => Enum.TryParse<ProgramTypes>(type, true, out var parsed) ? (ProgramTypes?)parsed : null)
      .Where(parsed => parsed.HasValue)
      .Select(parsed => parsed!.Value)
      .ToArray();
  }
}
