namespace ECER.Engines.Validation.Applications;

internal sealed class ApplicationValidationEngine : IApplicationValidationEngine
{
    public async Task<ValidationResults> Validate(Application application)
    {
        return await Task.FromResult(new ValidationResults(true));
    }
}
