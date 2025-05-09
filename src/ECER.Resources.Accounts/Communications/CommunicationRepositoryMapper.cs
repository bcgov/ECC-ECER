﻿using AutoMapper;
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
     .ForMember(d => d.ApplicationId, opts => opts.MapFrom(s =>
      s.ecer_communication_Applicationid != null &&
      !new[]
      {
        ecer_Application_StatusCode.Draft,
        ecer_Application_StatusCode.Decision,
        ecer_Application_StatusCode.Reconsideration,
        ecer_Application_StatusCode.ReconsiderationDecision,
        ecer_Application_StatusCode.AppealDecision,
        ecer_Application_StatusCode.Withdrawn,
        ecer_Application_StatusCode.Complete,
        ecer_Application_StatusCode.Cancelled
      }.ToList().Contains(s.ecer_communication_Applicationid.StatusCode!.Value)
        ? s.ecer_communication_Applicationid.ecer_ApplicationId
        : null))
     .ForMember(d => d.DoNotReply, opts => opts.MapFrom(s => s.ecer_DoNotReply))
     .ForMember(d => d.Documents, opts => opts.MapFrom(s => s.ecer_bcgov_documenturl_CommunicationId_ecer_communication))
     .ForMember(d => d.LatestMessageNotifiedOn, opts => opts.MapFrom(s => s.ecer_IsRoot != null && s.ecer_IsRoot == true ? (s.ecer_LatestMessageNotifiedDate ?? s.ecer_DateNotified) : s.ecer_DateNotified))
     .ForMember(d => d.IsRead, opts => opts.MapFrom(s => s.ecer_IsRoot != null && s.ecer_IsRoot == true ? s.ecer_AreAllRead : s.ecer_Acknowledged));

    CreateMap<Communication, ecer_Communication>(MemberList.Source)
     .ForSourceMember(s => s.Subject, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.From, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.NotifiedOn, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.Acknowledged, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.Status, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.Body, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.DoNotReply, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.LatestMessageNotifiedOn, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.IsRead, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.Documents, opts => opts.DoNotValidate())
     .ForSourceMember(s => s.ApplicationId, opts => opts.DoNotValidate())
     .ForMember(d => d.ecer_Message, opts => opts.MapFrom(s => htmlSanitizer.Sanitize(s.Body, "", null)));

    CreateMap<ecer_Communication_StatusCode, CommunicationStatus>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true));

    CreateMap<ecer_InitiatedFrom, InitiatedFrom>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true));

    CreateMap<bcgov_DocumentUrl, CommunicationDocument>(MemberList.Destination)
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.bcgov_DocumentUrlId))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.bcgov_FileName))
      .ForMember(d => d.Size, opts => opts.MapFrom(s => s.bcgov_FileSize))
      .ForMember(d => d.Url, opts => opts.MapFrom(s => s.bcgov_Url))
      .ForMember(d => d.Extention, opts => opts.MapFrom(s => s.bcgov_FileExtension));
  }
}
