namespace ECER.Resources.Accounts.Registrants;

/// <summary>
/// Manages registrants
/// </summary>
public interface IRegistrantRepository
{
    /// <summary>
    /// Create a new registrant
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<string> Create(NewRegistrantRequest request);
}

public record NewRegistrantRequest();
