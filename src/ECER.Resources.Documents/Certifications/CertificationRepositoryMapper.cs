using ECER.Utilities.DataverseSdk.Model;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.Certifications;

internal interface ICertificationRepositoryMapper
{
  List<Certification> MapCertifications(IEnumerable<ecer_Certificate> source);
  List<CertificationSummary> MapCertificationSummaries(IEnumerable<ecer_CertificateSummary> source);
}

[Mapper]
internal partial class CertificationRepositoryMapper : ICertificationRepositoryMapper
{
  public List<Certification> MapCertifications(IEnumerable<ecer_Certificate> source) => source.Select(MapCertification).ToList();

  public List<CertificationSummary> MapCertificationSummaries(IEnumerable<ecer_CertificateSummary> source) => source.Select(MapCertificationSummary).ToList();

  private Certification MapCertification(ecer_Certificate source) => new(source.ecer_CertificateId?.ToString() ?? string.Empty)
  {
    Name = source.ecer_RegistrantidName,
    Number = source.ecer_CertificateNumber,
    RegistrantId = source.ecer_Registrantid?.Id.ToString(),
    ExpiryDate = source.ecer_ExpiryDate,
    EffectiveDate = source.ecer_EffectiveDate,
    Date = source.ecer_Date,
    PrintDate = source.ecer_PrintedDate,
    HasConditions = source.ecer_HasConditions,
    LevelName = source.ecer_CertificateLevel,
    BaseCertificateTypeId = MapBaseCertificateTypeId(source.ecer_BaseCertificateTypeID),
    StatusCode = MapCertificateStatusCode(source.StatusCode),
    CertificatePDFGeneration = MapCertificatePdfGeneration(source.ecer_HasCertificatePDF),
    IneligibleReference = MapYesNoNull(source.ecer_IneligibleReference),
    Levels = (source.ecer_certifiedlevel_CertificateId ?? Array.Empty<ecer_CertifiedLevel>()).Select(MapCertificationLevel).ToList(),
    Files = (source.ecer_documenturl_CertificateId ?? Array.Empty<bcgov_DocumentUrl>()).Select(MapCertificationFile).ToList(),
    CertificateConditions = (source.ecer_certificate_Registrantid?.ecer_certificateconditions_Registrantid ?? Array.Empty<ecer_CertificateConditions>())
      .Where(condition => condition.StatusCode == ecer_CertificateConditions_StatusCode.Active)
      .Select(MapCertificateCondition)
      .ToList(),
  };

  private CertificateCondition MapCertificateCondition(ecer_CertificateConditions source) => new()
  {
    Id = source.Id.ToString(),
    CertificateId = source.ecer_CertificateId?.Id.ToString(),
    Name = source.ecer_Name,
    Details = source.ecer_Details,
    StartDate = source.ecer_StartDate.GetValueOrDefault(),
    EndDate = source.ecer_EndDate.GetValueOrDefault(),
    DisplayOrder = source.ecer_DisplayOrder.GetValueOrDefault(),
  };

  private CertificationLevel MapCertificationLevel(ecer_CertifiedLevel source) => new(source.ecer_CertifiedLevelId?.ToString() ?? string.Empty)
  {
    Type = source.ecer_CertificateTypeIdName,
  };

  private CertificationFile MapCertificationFile(bcgov_DocumentUrl source) => new(source.bcgov_DocumentUrlId?.ToString() ?? string.Empty)
  {
    Url = source.bcgov_Url,
    Extention = source.bcgov_FileExtension,
    Size = source.bcgov_FileSize,
    Name = source.bcgov_FileName,
    CreatedOn = source.CreatedOn,
    Tag1Name = source.bcgov_Tag1IdName,
  };

  private CertificationSummary MapCertificationSummary(ecer_CertificateSummary source)
  {
    var latestFile = source.ecer_bcgov_documenturl_CertificateSummaryId?
      .OrderByDescending(file => file.CreatedOn)
      .FirstOrDefault();

    return new CertificationSummary(source.ecer_CertificateSummaryId?.ToString() ?? string.Empty)
    {
      FileName = latestFile?.bcgov_FileName,
      FilePath = latestFile?.bcgov_Url,
      FileExtention = latestFile?.bcgov_FileExtension,
      FileId = latestFile?.bcgov_DocumentUrlId.ToString(),
      CreatedOn = latestFile?.CreatedOn,
    };
  }

  private CertificateStatusCode? MapCertificateStatusCode(ecer_Certificate_StatusCode? source) => source.HasValue ? MapCertificateStatusCode(source.Value) : null;

  private CertificatePDFGeneration? MapCertificatePdfGeneration(ecer_CertificatePDFGeneration? source) => source.HasValue ? MapCertificatePdfGeneration(source.Value) : null;

  private YesNoNull? MapYesNoNull(ecer_YesNoNull? source) => source.HasValue ? MapYesNoNull(source.Value) : null;

  private static int? MapBaseCertificateTypeId(string? source) => int.TryParse(source, out var parsed) ? parsed : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CertificateStatusCode MapCertificateStatusCode(ecer_Certificate_StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CertificatePDFGeneration MapCertificatePdfGeneration(ecer_CertificatePDFGeneration source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial YesNoNull MapYesNoNull(ecer_YesNoNull source);
}
