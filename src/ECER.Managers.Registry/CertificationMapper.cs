using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Resources.Documents.Certifications;

namespace ECER.Managers.Registry;

internal class CertificationMapper : SecureProfile
{
  public CertificationMapper()
  {
    CreateMap<Certification, Contract.Certifications.Certification>()
        .ReverseMap();

    CreateMap<CertificationLevel, Contract.Certifications.CertificationLevel>()
      .ReverseMap();

    CreateMap<CertificationFile, Contract.Certifications.CertificationFile>()
      .ReverseMap();

    CreateMap<CertificateCondition, Contract.Certifications.CertificateCondition>();
  }
}
