using AutoMapper;

namespace ECER.Clients.Api.Certifications;

public class CertificationMapper : Profile
{
  public CertificationMapper()
  {
    CreateMap<Managers.Admin.Contract.Certifications.CertificationSummary, CertificationSummary>()
      .ReverseMap();
  }
}
