using AutoMapper;
using ECER.Resources.Documents.Certifications;

namespace ECER.Managers.Registry;

internal class CertificationMapper : Profile
{
  public CertificationMapper()
  {
    CreateMap<Certification, Contract.Certifications.Certification>()
        .ReverseMap();
  }
}
