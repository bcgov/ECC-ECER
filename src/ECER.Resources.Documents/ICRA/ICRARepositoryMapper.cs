using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;

namespace ECER.Resources.Documents.ICRA;

internal class ICRARepositoryMapper : Profile
{
  public ICRARepositoryMapper()
  {
    CreateMap<ICRAEligibility, ecer_ICRAEligibilityAssessment>(MemberList.Source);

    CreateMap<ICRAStatus, ecer_ICRAEligibilityAssessment_StatusCode>()
         .ConvertUsingEnumMapping(opts => opts.MapByName(true))
         .ReverseMap();
  }

  public static string IdOrEmpty(EntityReference? reference) =>
      reference != null && reference.Id != Guid.Empty
          ? reference.Id.ToString()  
          : string.Empty;              
}
