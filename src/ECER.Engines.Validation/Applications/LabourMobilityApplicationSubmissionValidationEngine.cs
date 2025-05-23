﻿using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Engines.Validation.Applications;

internal sealed class LabourMobilityApplicationSubmissionValidationEngine : IApplicationValidationEngine
{
  public async Task<ValidationResults> Validate(Application application)
  {
    await Task.CompletedTask;

    var validationErrors = new List<string>();

    if (!application.CertificationTypes.Any())
    {
      validationErrors.Add("Application is not associated with a certification type");
    }

    // each application should contain at least one character reference
    if (!application.CharacterReferences.Any())
    {
      validationErrors.Add("the application does not have any character references");
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
