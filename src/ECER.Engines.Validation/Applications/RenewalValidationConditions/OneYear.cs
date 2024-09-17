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

        if (application.OneYearRenewalExplanationChoice == null)
        {
          validationErrors.Add("one year explanation choice cannot be null");
        }

        if (application.OneYearRenewalExplanationChoice == OneYearRenewalexplanations.Other && string.IsNullOrEmpty(application.RenewalExplanationOther))
        {
          validationErrors.Add("renewal explanation other required if one year renewal explanation choice is other");
        }
        // each application should contain at least one character reference
        if (!application.CharacterReferences.Any())
        {
          validationErrors.Add("the application does not have any character references");
        }
        break;

      case CertificateStatus.ExpiredLessThanFiveYearsAgo:
        // each application should contain at least one professional development
        if (!application.ProfessionalDevelopments.Any())
        {
          validationErrors.Add("the application does not have any professional development");
        }

        if (application.OneYearRenewalExplanationChoice == null)
        {
          validationErrors.Add("one year explanation choice cannot be null");
        }

        // each application should contain explanation letter
        if (application.OneYearRenewalExplanationChoice == OneYearRenewalexplanations.Other && string.IsNullOrEmpty(application.RenewalExplanationOther))
        {
          validationErrors.Add("the application does not have explanation letter");
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
          validationErrors.Add("Work experience does not meet 400 hours");
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