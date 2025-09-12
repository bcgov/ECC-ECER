using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;

namespace ECER.Resources.Documents.ICRA;

internal class ICRARepositoryMapper : Profile
{
  public ICRARepositoryMapper()
  {
    CreateMap<ICRAEligibility, ecer_ICRAEligibilityAssessment>(MemberList.Source)
      .ForSourceMember(s => s.ApplicantId, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.CreatedOn, opts => opts.DoNotValidate())
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.ecer_PortalStage, opts => opts.MapFrom(s => s.PortalStage))
      .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.SignedDate))
      .ReverseMap()
      .ForMember(d => d.ApplicantId, opts => opts.MapFrom(s => s.ecer_icraeligibilityassessment_ApplicantId.Id))
      .ForMember(d => d.CreatedOn, opts => opts.MapFrom(s => s.CreatedOn));


    CreateMap<ICRAStatus, ecer_ICRAEligibilityAssessment_StatusCode>()
         .ConvertUsingEnumMapping(opts => opts.MapByName(true))
         .ReverseMap();
  }

  public static string IdOrEmpty(EntityReference? reference) =>
      reference != null && reference.Id != Guid.Empty
          ? reference.Id.ToString()  
          : string.Empty;              
}
