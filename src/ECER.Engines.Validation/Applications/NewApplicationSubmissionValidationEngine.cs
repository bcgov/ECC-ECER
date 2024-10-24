using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Engines.Validation.Applications;

internal sealed class NewApplicationSubmissionValidationEngine : IApplicationValidationEngine
{
  public async Task<ValidationResults> Validate(Application application)
  {
    await Task.CompletedTask;

    var validationErrors = new List<string>();

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

    // if the application contains the ECE Assistant certification type, the most recent education should be within 5 years
    if (application.CertificationTypes.Any(ct => ct == CertificationType.EceAssistant) && application.Transcripts.Any(t => t.EndDate.AddYears(5) <= DateTime.Today))
    {
      // The end date is more than 5 years from today
      validationErrors.Add("Education was completed more than 5 years ago");
    }

    // if the application contains the Five Years certification type, the total work experience should be at least 500 hours
    if (application.CertificationTypes.Any(ct => ct == CertificationType.FiveYears))
    {
      var totalHours = application.WorkExperienceReferences
          .Sum(we => we.Hours);

      if (totalHours < 500)
      {
        validationErrors.Add("Work experience does not meet 500 hours");
      }
    }

    return new ValidationResults(validationErrors);
  }
}
