using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Accounts.Communications;

internal class CommunicationRepositoryMapper : Profile
{
  public CommunicationRepositoryMapper()
  {
    CreateMap<Communication, ecer_Communication>(MemberList.Source)
              .ForMember(d => d.ecer_CommunicationId, opts => opts.MapFrom(s => s.Id))
              .ForMember(d => d.ecer_Message, opts => opts.MapFrom(s => s.Body))
              .ForMember(d => d.ecer_Name, opts => opts.MapFrom(s => s.Subject))
              .ForMember(d => d.ecer_Acknowledged, opts => opts.MapFrom(s => s.Acknowledged))
              .ForMember(d => d.ecer_DateNotified, opts => opts.MapFrom(s => s.NotifiedOn))
              .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
              .ReverseMap();

    CreateMap<CommunicationStatus, ecer_Communication_StatusCode>()
            .ConvertUsingEnumMapping(opts => opts.MapByName(true))
            .ReverseMap();
  }
}


