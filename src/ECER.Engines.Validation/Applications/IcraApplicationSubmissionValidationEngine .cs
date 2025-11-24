using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Engines.Validation.Applications;

internal sealed class IcraApplicationSubmissionValidationEngine : IApplicationValidationEngine
{
  public async Task<ValidationResults> Validate(Application application)
  {
    await Task.CompletedTask;

    var validationErrors = new List<string>();

    if (!application.CertificationTypes.Contains(CertificationType.FiveYears))
    {
      validationErrors.Add("Application is not associated with a five year certification type");
    }

    // each application should contain at least one education
    if (!application.Transcripts.Any())
    {
      validationErrors.Add("the application does not have any education");
    }

    // each application should contain at least one character reference
    if (!application.CharacterReferences.Any())
    {
      validationErrors.Add("the application does not have any character references");
    }

    return new ValidationResults(validationErrors);
  }
}
