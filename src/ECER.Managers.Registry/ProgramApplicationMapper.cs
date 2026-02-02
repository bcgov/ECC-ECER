using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ApplicationStatus = ECER.Resources.Documents.ProgramApplications.ApplicationStatus;

namespace ECER.Managers.Registry;

internal class ProgramApplicationMapper: Profile
{
  public ProgramApplicationMapper()
  {
    CreateMap<ProgramApplication, Resources.Documents.ProgramApplications.ProgramApplication>()
      .ForCtorParam(nameof(ProgramApplication.Id), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(ProgramApplication.PostSecondaryInstituteId), opts => opts.MapFrom(s => s.PostSecondaryInstituteId))
      .ForMember(d => d.ProgramApplicationName, opts => opts.MapFrom(s => s.ProgramApplicationName))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.Status))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
    
    CreateMap<Contract.ProgramApplications.ApplicationStatus, ApplicationStatus>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Contract.ProgramApplications.ApplicationType, ApplicationType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Contract.ProgramApplications.DeliveryType, DeliveryType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Contract.ProgramApplications.ProvincialCertificationTypeOffered, ProvincialCertificationTypeOffered>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
  }
}
