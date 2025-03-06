using ECER.Managers.Registry.Contract.Applications;
using ECER.Resources.Documents.Certifications;

namespace ECER.Engines.Validation.Applications;

internal sealed partial class ApplicationRenewalValidationEngine : IApplicationValidationEngine
{
  private ICertificationRepository _certificateRepository;

  public ApplicationRenewalValidationEngine(ICertificationRepository certificationRepository)
  {
    _certificateRepository = certificationRepository;
  }

  public async Task<ValidationResults> Validate(Application application)
  {
    await Task.CompletedTask;
    var validationErrors = new List<string>();

    if (!application.CertificationTypes.Any())
    {
      validationErrors.Add("Application is not associated with a certification type");
    }

    if (application.CertificationTypes.Contains(CertificationType.EceAssistant))
    {
      validationErrors = await EceAssistant(application);
    }
    else if (application.CertificationTypes.Contains(CertificationType.OneYear))
    {
      validationErrors = await OneYear(application);
    }
    else if (application.CertificationTypes.Contains(CertificationType.FiveYears))
    {
      validationErrors = await FiveYears(application);
    }
    return new ValidationResults(validationErrors);
  }

  private enum CertificateStatus
  {
    Active,
    ExpiredLessThanFiveYearsAgo,
    ExpiredMoreThanFiveYearsAgo,
    NoCertificateFound
  }

  private async Task<CertificateStatus> GetCertificateStatus(string applicantId)
  {
    try
    {
      var expiryDate = await getLastCertificateExpiryDate(applicantId);
      var now = DateTime.Now.Date; //sets time stamp to 00:00:00

      if (expiryDate >= now)
      {
        return CertificateStatus.Active;
      }
      else if (expiryDate < now && expiryDate >= now.AddYears(-5))
      {
        return CertificateStatus.ExpiredLessThanFiveYearsAgo;
      }
      else
      {
        return CertificateStatus.ExpiredMoreThanFiveYearsAgo;
      }
    }
    catch (InvalidOperationException)
    {
      return CertificateStatus.NoCertificateFound;
    }
  }

  private async Task<DateTime> getLastCertificateExpiryDate(string applicantId)
  {
    var certificates = await _certificateRepository.Query(new UserCertificationQuery() { ByApplicantId = applicantId });
    var lastCertificate = certificates.OrderByDescending(d => d.ExpiryDate).ThenBy(e => e.BaseCertificateTypeId).FirstOrDefault();
    if (lastCertificate == null || lastCertificate.ExpiryDate == null)
    {
      throw new InvalidOperationException("Certificate or datetime is null");
    }
    return (DateTime)lastCertificate.ExpiryDate;
  }
}
