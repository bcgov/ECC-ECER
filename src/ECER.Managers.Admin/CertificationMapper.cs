using Riok.Mapperly.Abstractions;
using ContractCertifications = ECER.Managers.Admin.Contract.Certifications;
using ResourceCertifications = ECER.Resources.Documents.Certifications;

namespace ECER.Managers.Admin;

public interface ICertificationMapper
{
  IEnumerable<ContractCertifications.CertificationSummary> MapCertificationSummaries(IEnumerable<ResourceCertifications.CertificationSummary> source);
}

[Mapper]
internal partial class CertificationMapper : ICertificationMapper
{
  public IEnumerable<ContractCertifications.CertificationSummary> MapCertificationSummaries(IEnumerable<ResourceCertifications.CertificationSummary> source) => source.Select(MapCertificationSummary).ToList();

  private partial ContractCertifications.CertificationSummary MapCertificationSummary(ResourceCertifications.CertificationSummary source);
}
