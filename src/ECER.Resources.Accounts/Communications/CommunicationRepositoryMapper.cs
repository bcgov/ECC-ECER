using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;
using Ganss.Xss;

namespace ECER.Resources.Accounts.Communications;

internal class CommunicationRepositoryMapper : Profile
{
  private static readonly HtmlSanitizer htmlSanitizer = new();

  public CommunicationRepositoryMapper()
  {
    CreateMap<ecer_Communication, Communication>(MemberList.Destination)
     .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_CommunicationId))
     .ForMember(d => d.Body, opts => opts.MapFrom(s => htmlSanitizer.Sanitize(s.ecer_Message, "", null)))
     .ForMember(d => d.Subject, opts => opts.MapFrom(s => s.ecer_Name))
     .ForMember(d => d.From, opts => opts.MapFrom(s => s.ecer_InitiatedFrom))
     .ForMember(d => d.Acknowledged, opts => opts.MapFrom(s => s.ecer_Acknowledged))
     .ForMember(d => d.NotifiedOn, opts => opts.MapFrom(s => s.ecer_DateNotified))
     .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
     .ForMember(d => d.DoNotReply, opts => opts.MapFrom(s => s.ecer_DoNotReply));

    CreateMap<Communication, ecer_Communication>(MemberList.Source)
     .ForSourceMember(s => s.Subject, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.From, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.NotifiedOn, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.Acknowledged, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.Status, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.Body, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.DoNotReply, opts => opts.DoNotValidate())
     .ForMember(d => d.ecer_Message, opts => opts.MapFrom(s => htmlSanitizer.Sanitize(s.Body, "", null)));

    CreateMap<ecer_Communication_StatusCode, CommunicationStatus>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true));

    CreateMap<ecer_InitiatedFrom, InitiatedFrom>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true));
  }
}
