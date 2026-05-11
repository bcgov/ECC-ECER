using Riok.Mapperly.Abstractions;
using ContractCertifications = ECER.Managers.Registry.Contract.Certifications;
using ResourceCertifications = ECER.Resources.Documents.Certifications;

namespace ECER.Managers.Registry;

public interface ICertificationMapper
{
  ContractCertifications.Certification? MapCertification(ResourceCertifications.Certification? source);
  IEnumerable<ContractCertifications.Certification> MapCertifications(IEnumerable<ResourceCertifications.Certification> source);
}

[Mapper]
internal partial class CertificationMapper : ICertificationMapper
{
  public ContractCertifications.Certification? MapCertification(ResourceCertifications.Certification? source) => source == null ? null : new(source.Id)
  {
    Name = source.Name,
    Number = source.Number,
    ExpiryDate = source.ExpiryDate,
    EffectiveDate = source.EffectiveDate,
    Date = source.Date,
    PrintDate = source.PrintDate,
    HasConditions = source.HasConditions,
    LevelName = source.LevelName,
    CertificatePDFGeneration = MapCertificatePDFGeneration(source.CertificatePDFGeneration),
    StatusCode = MapCertificateStatusCode(source.StatusCode),
    IneligibleReference = MapYesNoNull(source.IneligibleReference),
    Levels = source.Levels.Select(MapCertificationLevel).ToList(),
    Files = source.Files.Select(MapCertificationFile).ToList(),
    CertificateConditions = source.CertificateConditions.Select(MapCertificateCondition).ToList(),
  };

  public IEnumerable<ContractCertifications.Certification> MapCertifications(IEnumerable<ResourceCertifications.Certification> source) => source.Select(certification => MapCertification(certification)!).ToList();

  private static ContractCertifications.CertificationLevel MapCertificationLevel(ResourceCertifications.CertificationLevel source) => new(source.Id)
  {
    Type = source.Type,
  };

  private static ContractCertifications.CertificationFile MapCertificationFile(ResourceCertifications.CertificationFile source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Size = source.Size,
    Name = source.Name,
    CreatedOn = source.CreatedOn,
    Tag1Name = source.Tag1Name,
  };

  private static ContractCertifications.CertificateCondition MapCertificateCondition(ResourceCertifications.CertificateCondition source) => new()
  {
    Id = source.Id,
    Name = source.Name,
    Details = source.Details,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
    DisplayOrder = source.DisplayOrder,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractCertifications.CertificateStatusCode MapCertificateStatusCode(ResourceCertifications.CertificateStatusCode source);

  private ContractCertifications.CertificateStatusCode? MapCertificateStatusCode(ResourceCertifications.CertificateStatusCode? source) => source.HasValue ? MapCertificateStatusCode(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractCertifications.CertificatePDFGeneration MapCertificatePDFGeneration(ResourceCertifications.CertificatePDFGeneration source);

  private ContractCertifications.CertificatePDFGeneration? MapCertificatePDFGeneration(ResourceCertifications.CertificatePDFGeneration? source) => source.HasValue ? MapCertificatePDFGeneration(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractCertifications.YesNoNull MapYesNoNull(ResourceCertifications.YesNoNull source);

  private ContractCertifications.YesNoNull? MapYesNoNull(ResourceCertifications.YesNoNull? source) => source.HasValue ? MapYesNoNull(source.Value) : null;
}
