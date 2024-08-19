using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Engines.Validation.Applications;

internal sealed class ApplicationSubmissionValidationEngine : IApplicationValidationEngine
{
  private static readonly IEnumerable<CertificationType> fiveYearNestedCertificationTypes = new[] { CertificationType.Ite, CertificationType.Sne };

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

    // if the application contains SNE and ITE certification types, the application should contain the Five Years certification type
    if (fiveYearNestedCertificationTypes.Intersect(application.CertificationTypes).Any() && !application.CertificationTypes.Any(ct => ct == CertificationType.FiveYears))
    {
      validationErrors.Add("Sub five year certification type selected but without five year certification");
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

    // if the application contains the SNE certification type, the application should contain at least 2 education
    if (application.CertificationTypes.Any(ct => ct == CertificationType.Sne) && application.Transcripts.Count() < 2)
    {
      validationErrors.Add("applicant does not have enough education for SNE");
    }

    // if the application contains the ITE certification type, the application should contain at least 2 education
    if (application.CertificationTypes.Any(ct => ct == CertificationType.Ite) && application.Transcripts.Count() < 2)
    {
      validationErrors.Add("applicant does not have enough education for ITE");
    }

    // if the application contains both SNE and ITE certification types, the application should contain at least 3 education
    if (application.CertificationTypes.Any(ct => ct == CertificationType.Ite) && application.CertificationTypes.Any(ct => ct == CertificationType.Sne) && application.Transcripts.Count() < 3)
    {
      validationErrors.Add("applicant does not have enough education for both ITE and SNE");
    }

    return new ValidationResults(validationErrors);
  }
}
