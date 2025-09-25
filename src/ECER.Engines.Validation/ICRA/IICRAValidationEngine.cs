using ECER.Managers.Registry.Contract.ICRA;

namespace ECER.Engines.Validation.ICRA;

public interface IICRAValidationEngine
{
  Task<Applications.ValidationResults> Validate(ICRAEligibility eligibility);
}


