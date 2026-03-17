using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Resources.Documents.Certifications;

namespace ECER.Managers.Admin;

internal class CertificationMapper : SecureProfile
{
  public CertificationMapper()
  {
    CreateMap<CertificationSummary, Contract.Certifications.CertificationSummary>();
  }
}
