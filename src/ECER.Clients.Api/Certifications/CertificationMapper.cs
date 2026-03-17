using AutoMapper;
using ECER.Infrastructure.Common;

namespace ECER.Clients.Api.Certifications;

public class CertificationMapper : SecureProfile
{
  public CertificationMapper()
  {
    CreateMap<Managers.Admin.Contract.Certifications.CertificationSummary, CertificationSummary>()
      .ReverseMap();
  }
}
