using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Engines.Validation.Applications;

internal sealed partial class ApplicationRenewalValidationEngine
{
  private async Task<List<string>> OneYear(Application application)
  {
    await Task.CompletedTask;
    var validationErrors = new List<string>();
    switch (await GetCertificateStatus(application.RegistrantId))
    {
      case CertificateStatus.Active:

        // each application should contain explanation letter
        if (string.IsNullOrEmpty(application.ExplanationLetter))
        {
          validationErrors.Add("the application does not have explanation letter");
        }
        // each application should contain at least one character reference
        if (!application.CharacterReferences.Any())
        {
          validationErrors.Add("the application does not have any character references");
        }
        break;

      case CertificateStatus.ExpiredLessThanFiveYearsAgo:
        break;

      case CertificateStatus.ExpiredMoreThanFiveYearsAgo:
        break;

      case CertificateStatus.NoCertificateFound:
        break;
    }
    return validationErrors;
  }
}
