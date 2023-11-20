namespace ECER.Engines.Validation.Applications;

/// <summary>
/// Validates registry applications
/// </summary>
public interface IApplicationValidationEngine
{
    /// <summary>
    /// Validates if an application is completed
    /// </summary>
    /// <param name="application">a instance of an application</param>
    /// <returns>validation results</returns>
    Task<ValidationResults> Validate(Application application);
}

/// <summary>
/// validation results
/// </summary>
public record ValidationResults(bool IsValid);

/// <summary>
/// application to validate
/// </summary>
public record Application();
