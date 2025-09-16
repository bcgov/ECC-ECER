using AutoMapper;
using System.Diagnostics.Contracts;

namespace ECER.Clients.RegistryPortal.Server.ICRA;

public class ICRAEligibilityMapper : Profile
{
  public ICRAEligibilityMapper()
  {
    CreateMap<ICRAEligibility, Managers.Registry.Contract.ICRA.ICRAEligibility>()
      .ReverseMap();

    CreateMap<InternationalCertification, Managers.Registry.Contract.ICRA.InternationalCertification>()
      .ReverseMap();
  }
}
