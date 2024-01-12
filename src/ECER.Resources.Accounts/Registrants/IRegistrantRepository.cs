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
    Task<string> RegisterNew(NewRegistrantRequest request);

    Task<RegistrantQueryResults> Query(RegistrantQuery query);
}

public record NewRegistrantRequest(UserProfile UserProfile, UserIdentity UserIdentity);

public record RegistrantQuery
{
    public string? WithId { get; set; }
    public UserIdentity? WithIdentity { get; set; }
}

public record RegistrantQueryResults(IEnumerable<Registrant> Items);

public record Registrant
{
    public string Id { get; set; } = null!;
    public IEnumerable<UserIdentity> Identities { get; set; } = Array.Empty<UserIdentity>();
    public UserProfile Profile { get; set; } = null!;
}

public record UserIdentity(string IdentityProvider, string Id)
{
    public DateTime? LastLogin { get; set; }
}

public record UserProfile
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Address? HomeAddress { get; set; }
    public Address? MailingAddress { get; set; }
}

public record Address(
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );