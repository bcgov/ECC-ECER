using Riok.Mapperly.Abstractions;
using ContractCertifications = ECER.Managers.Registry.Contract.Certifications;

namespace ECER.Clients.RegistryPortal.Server.Certifications;

internal interface ICertificationMapper
{
  Certification MapCertification(ContractCertifications.Certification source);
  IEnumerable<Certification> MapCertifications(IEnumerable<ContractCertifications.Certification> source);
  IEnumerable<CertificationLookupResponse> MapLookupResponses(IEnumerable<ContractCertifications.Certification> source);
}

[Mapper]
internal partial class CertificationMapper : ICertificationMapper
{
  public IEnumerable<Certification> MapCertifications(IEnumerable<ContractCertifications.Certification> source) => source.Select(MapCertification).ToList();

  public IEnumerable<CertificationLookupResponse> MapLookupResponses(IEnumerable<ContractCertifications.Certification> source) => source.Select(MapLookupResponse).ToList();

  public Certification MapCertification(ContractCertifications.Certification source) => new(source.Id)
  {
    Name = source.Name,
    Number = source.Number,
    ExpiryDate = source.ExpiryDate,
    EffectiveDate = source.EffectiveDate,
    Date = source.Date,
    PrintDate = source.PrintDate,
    HasConditions = source.HasConditions,
    LevelName = source.LevelName,
    StatusCode = MapCertificateStatusCode(source.StatusCode),
    CertificatePDFGeneration = MapCertificatePdfGeneration(source.CertificatePDFGeneration),
    IneligibleReference = MapYesNoNull(source.IneligibleReference),
    Levels = source.Levels.Select(MapCertificationLevel).ToList(),
    Files = source.Files.Select(MapCertificationFile).ToList(),
    CertificateConditions = source.CertificateConditions.Select(MapCertificateCondition).ToList(),
  };

  private CertificationLookupResponse MapLookupResponse(ContractCertifications.Certification source) => new(source.Id)
  {
    Name = source.Name,
    RegistrationNumber = source.Number,
    StatusCode = MapCertificateStatusCode(source.StatusCode),
    LevelName = source.LevelName,
    ExpiryDate = source.ExpiryDate,
    HasConditions = source.HasConditions,
    Levels = source.Levels.Select(MapCertificationLevel).ToList(),
    CertificateConditions = source.CertificateConditions.Select(MapCertificateCondition).ToList(),
  };

  private partial CertificationLevel MapCertificationLevel(ContractCertifications.CertificationLevel source);

  private partial CertificateCondition MapCertificateCondition(ContractCertifications.CertificateCondition source);

  private static CertificationFile MapCertificationFile(ContractCertifications.CertificationFile source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Size = source.Size,
    Name = source.Name,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CertificateStatusCode MapCertificateStatusCode(ContractCertifications.CertificateStatusCode source);

  private CertificateStatusCode? MapCertificateStatusCode(ContractCertifications.CertificateStatusCode? source) => source.HasValue ? MapCertificateStatusCode(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CertificatePDFGeneration MapCertificatePdfGeneration(ContractCertifications.CertificatePDFGeneration source);

  private CertificatePDFGeneration? MapCertificatePdfGeneration(ContractCertifications.CertificatePDFGeneration? source) => source.HasValue ? MapCertificatePdfGeneration(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial YesNoNull MapYesNoNull(ContractCertifications.YesNoNull source);

  private YesNoNull? MapYesNoNull(ContractCertifications.YesNoNull? source) => source.HasValue ? MapYesNoNull(source.Value) : null;
}
