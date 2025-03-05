using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Engines.Validation.Applications;

internal sealed partial class ApplicationRenewalValidationEngine
{
  private async Task<List<string>> FiveYears(Application application)
  {
    await Task.CompletedTask;
    var validationErrors = new List<string>();
    switch (await GetCertificateStatus(application.RegistrantId))
    {
      case CertificateStatus.Active:
        // each application should contain at least one professional development
        if (!application.ProfessionalDevelopments.Any())
        {
          validationErrors.Add("the application does not have any professional development");
        }
        // each application should contain at least one character reference
        if (!application.CharacterReferences.Any())
        {
          validationErrors.Add("the application does not have any character references");
        }

        // todal 400 hours work experience references needed
        if (application.WorkExperienceReferences
       .Sum(we => we.Hours) < 400)
        {
          validationErrors.Add("You must provide 400 hours of work experience");
        }
        break;

      case CertificateStatus.ExpiredLessThanFiveYearsAgo:
        // each application should contain at least one professional development
        if (!application.ProfessionalDevelopments.Any())
        {
          validationErrors.Add("the application does not have any professional development");
        }

        if (application.FiveYearRenewalExplanationChoice == null)
        {
          validationErrors.Add("You must provide a reason for late renewal");
        }
        // each application should contain explanation letter
        if (string.IsNullOrEmpty(application.RenewalExplanationOther) && application.FiveYearRenewalExplanationChoice == FiveYearRenewalExplanations.Other)
        {
          validationErrors.Add("renewal explanation other required if explanation choice is other");
        }
        // each application should contain at least one character reference
        if (!application.CharacterReferences.Any())
        {
          validationErrors.Add("the application does not have any character references");
        }
        // todal 400 hours work experience references needed
        if (application.WorkExperienceReferences
       .Sum(we => we.Hours) < 400)
        {
          validationErrors.Add("You must provide 400 hours of work experience");
        }
        break;

      case CertificateStatus.ExpiredMoreThanFiveYearsAgo:
        // each application should contain at least one professional development
        if (!application.ProfessionalDevelopments.Any())
        {
          validationErrors.Add("the application does not have any professional development");
        }
        // each application should contain at least one character reference
        if (!application.CharacterReferences.Any())
        {
          validationErrors.Add("the application does not have any character references");
        }
        // total 500 hours work experience references needed
        if (application.WorkExperienceReferences
       .Sum(we => we.Hours) < 500)
        {
          validationErrors.Add("You must provide 500 hours of work experience");
        }
        break;

      case CertificateStatus.NoCertificateFound:
        break;
    }
    return validationErrors;
  }
}
