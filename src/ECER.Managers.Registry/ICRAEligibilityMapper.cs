using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Resources.Documents.ICRA;

namespace ECER.Managers.Registry;

internal class ICRAEligibilityMapper : SecureProfile
{
  public ICRAEligibilityMapper()
  {
    CreateMap<Contract.ICRA.ICRAEligibility, ICRAEligibility>().ReverseMap();
    CreateMap<Contract.ICRA.InternationalCertification, InternationalCertification>().ReverseMap();
    CreateMap<Contract.ICRA.EmploymentReference, Resources.Documents.ICRA.EmploymentReference>().ReverseMap();
    CreateMap<Contract.ICRA.ICRAWorkExperienceReferenceSubmissionRequest, Resources.Documents.ICRA.ICRAWorkExperienceReferenceSubmissionRequest>().ReverseMap();
  }
}
