using AutoMapper;
using ECER.Resources.Documents.ICRA;

namespace ECER.Managers.Registry;

internal class ICRAEligibilityMapper : Profile
{
  public ICRAEligibilityMapper()
  {
    CreateMap<Contract.ICRA.ICRAEligibility, ICRAEligibility>().ReverseMap();
    CreateMap<Contract.ICRA.InternationalCertification, InternationalCertification>().ReverseMap();
    CreateMap<ECER.Resources.Documents.Applications.FileInfo, ECER.Managers.Registry.Contract.Applications.FileInfo>().ReverseMap();
  }
}
