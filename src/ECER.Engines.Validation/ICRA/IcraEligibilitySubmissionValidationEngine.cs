using ECER.Managers.Registry.Contract.ICRA;

namespace ECER.Engines.Validation.ICRA;

internal sealed class IcraEligibilitySubmissionValidationEngine : IICRAValidationEngine
{
  public async Task<Applications.ValidationResults> Validate(ICRAEligibility eligibility)
  {
    await Task.CompletedTask;

    var errors = new List<string>();

    // Minimal draft submission rules: must have ApplicantId and Draft status
    if (string.IsNullOrWhiteSpace(eligibility.ApplicantId))
    {
      errors.Add("ApplicantId is required");
    }
    if (!eligibility.InternationalCertifications.Any(d=>d.CertificateStatus==CertificateStatus.Valid))
    {
      errors.Add("Valid International certification is missing");
    }


    if ((eligibility.InternationalCertifications ?? Enumerable.Empty<InternationalCertification>())
            .Any(ic => string.IsNullOrWhiteSpace(ic.CertificateTitle)))
    {
      errors.Add("International certification missing CertificateTitle");
    }

    if (!eligibility.EmploymentReferences.Any())
    {
      errors.Add("Employment Reference is missing");
    }

    // If any EmploymentReferences exist, ensure minimally required fields
    if ((eligibility.EmploymentReferences ?? Enumerable.Empty<EmploymentReference>())
            .Any(r =>
                string.IsNullOrWhiteSpace(r.FirstName) ||
                string.IsNullOrWhiteSpace(r.LastName) ||
                string.IsNullOrWhiteSpace(r.EmailAddress)))
    {
      errors.Add("Employment reference requires FirstName, LastName, and EmailAddress");
    }

    return new Applications.ValidationResults(errors);
  }
}


