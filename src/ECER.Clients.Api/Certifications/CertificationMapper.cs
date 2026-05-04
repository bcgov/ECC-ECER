using Riok.Mapperly.Abstractions;
using ContractCertifications = ECER.Managers.Admin.Contract.Certifications;

namespace ECER.Clients.Api.Certifications;

internal interface ICertificationMapper
{
  IEnumerable<CertificationSummary> MapCertificationSummaries(IEnumerable<ContractCertifications.CertificationSummary> source);
}

[Mapper]
internal partial class CertificationMapper : ICertificationMapper
{
  public IEnumerable<CertificationSummary> MapCertificationSummaries(IEnumerable<ContractCertifications.CertificationSummary> source) => source.Select(MapCertificationSummary).ToList();

  private CertificationSummary MapCertificationSummary(ContractCertifications.CertificationSummary source) => new(source.Id)
  {
    FileName = source.FileName,
    FileId = source.FileId,
    CreatedOn = source.CreatedOn,
  };
}
