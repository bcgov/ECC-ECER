using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Engines.Validation.Applications;

internal sealed class ApplicationSubmissionValidationEngine : IApplicationSubmissionValidationEngine
{
  private static readonly IEnumerable<CertificationType> fiveYearNestedCertificationTypes = new[] { CertificationType.Ite, CertificationType.Sne };

  public async Task<ValidationResults> Validate(Application application)
  {
    await Task.CompletedTask;

    var validationErrors = new List<string>();
    if (fiveYearNestedCertificationTypes.Intersect(application.CertificationTypes).Any() && !application.CertificationTypes.Any(ct => ct == CertificationType.FiveYears))
      validationErrors.Add("Sub five year certification type selected but without five year certification");

    return new ValidationResults(validationErrors);
  }
}
