using AutoMapper;
using ECER.Resources.Documents.Certifications;

namespace ECER.Managers.Admin;

internal class CertificationMapper : Profile
{
  public CertificationMapper()
  {
    CreateMap<CertificationSummary, Contract.Certifications.CertificationSummary>();
  }
}
