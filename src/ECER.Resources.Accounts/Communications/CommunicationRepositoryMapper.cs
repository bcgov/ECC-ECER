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
     .ForMember(d => d.ApplicationId, opts => opts.MapFrom(s => GetApplicationId(s)))
     .ForMember(d => d.IcraEligibilityId, opts => opts.MapFrom(s => GetIcraEligibilityId(s)))
     .ForMember(d => d.DoNotReply, opts => opts.MapFrom(s => s.ecer_DoNotReply))
     .ForMember(d => d.Documents, opts => opts.MapFrom(s => s.ecer_bcgov_documenturl_CommunicationId_ecer_communication))
     .ForMember(d => d.LatestMessageNotifiedOn, opts => opts.MapFrom(s => s.ecer_IsRoot != null && s.ecer_IsRoot == true ? (s.ecer_LatestMessageNotifiedDate ?? s.ecer_DateNotified) : s.ecer_DateNotified))
     .ForMember(d => d.IsRead, opts => opts.MapFrom(s => s.ecer_IsRoot != null && s.ecer_IsRoot == true ? s.ecer_AreAllRead : s.ecer_Acknowledged))
     .ForMember(d => d.ProgramRepresentativeId, opts => opts.MapFrom(s => s.ecer_ProgramRepresentativeId));

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
     .ForSourceMember(s => s.IcraEligibilityId, opts => opts.DoNotValidate())
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

  private static Guid? GetIcraEligibilityId(ecer_Communication src)
  {
    var allowedStatusCodes = new List<ecer_ICRAEligibilityAssessment_StatusCode>
    {
      ecer_ICRAEligibilityAssessment_StatusCode.Submitted,
      ecer_ICRAEligibilityAssessment_StatusCode.Active,
      ecer_ICRAEligibilityAssessment_StatusCode.ReadyforReview,
      ecer_ICRAEligibilityAssessment_StatusCode.InReview,
      ecer_ICRAEligibilityAssessment_StatusCode.ReadyforAssessment,
    };

    if (src.ecer_communication_ICRAEligibilityAssessmentId == null || src.ecer_communication_ICRAEligibilityAssessmentId?.StatusCode == null)
    {
      return null;
    }

    if (!allowedStatusCodes.Contains(src.ecer_communication_ICRAEligibilityAssessmentId.StatusCode.Value))
    {
      return null;
    }

    return src.ecer_communication_ICRAEligibilityAssessmentId.ecer_ICRAEligibilityAssessmentId;
  }

  private static Guid? GetApplicationId(ecer_Communication src)
  {
    var allowedStatusCodes = new List<ecer_Application_StatusCode>
    {
      ecer_Application_StatusCode.Closed,
      ecer_Application_StatusCode.Escalated,
      ecer_Application_StatusCode.InProgress,
      ecer_Application_StatusCode.Pending,
      ecer_Application_StatusCode.PendingPSPConsultationNeeded,
      ecer_Application_StatusCode.PendingQueue,
      ecer_Application_StatusCode.Ready,
      ecer_Application_StatusCode.Submitted
    };

    if (src.ecer_communication_Applicationid == null || src.ecer_communication_Applicationid?.StatusCode == null)
    {
      return null;
    }

    if (!allowedStatusCodes.Contains(src.ecer_communication_Applicationid.StatusCode.Value))
    {
      return null;
    }

    return src.ecer_communication_Applicationid.ecer_ApplicationId;
  }
}
