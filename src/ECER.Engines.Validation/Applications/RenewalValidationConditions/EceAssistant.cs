using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Engines.Validation.Applications;

internal sealed partial class ApplicationRenewalValidationEngine
{
  private async Task<List<string>> EceAssistant(Application application)
  {
    await Task.CompletedTask;
    var validationErrors = new List<string>();
    if (string.IsNullOrEmpty(application.FromCertificate))
    {
      validationErrors.Add("certification id in the field FromCertificate should not be null");
      return validationErrors;
    }
    switch (await GetCertificateStatus(application.FromCertificate,application.RegistrantId))
    {
      case CertificateStatus.Active:
        break;

      case CertificateStatus.ExpiredLessThanFiveYearsAgo:
        // each application should contain at least one education
        if (application.Transcripts.Count() != 1)
        {
          validationErrors.Add("the application does not have one education");
        }
        // each application should contain at least one character reference
        if (!application.CharacterReferences.Any())
        {
          validationErrors.Add("the application does not have any character references");
        }
        // total 400 hours work experience references needed
        if (application.WorkExperienceReferences
       .Sum(we => we.Hours) < 400)
        {
          validationErrors.Add("You must provide 400 hours of work experience");
        }
        break;

      case CertificateStatus.ExpiredMoreThanFiveYearsAgo:
        break;

      case CertificateStatus.NoCertificateFound:
        break;
    }
    return validationErrors;
  }
}
