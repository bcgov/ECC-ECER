using AutoMapper;
using ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry;

/// <summary>
/// User Manager
/// </summary>
public static class UserHandlers
{
    /// <summary>
    /// Handles user profile query use case
    /// </summary>
    /// <param name="query">The query</param>
    /// <param name="registrantRepository">DI service</param>
    /// <param name="mapper">DI service</param>
    /// <returns>Query results</returns>
    public static async Task<UserProfileQueryResponse> Handle(UserProfileQuery query, IRegistrantRepository registrantRepository, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(registrantRepository);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(query);

        var results = await registrantRepository.Query(new RegistrantQuery
        {
            WithIdentity = new UserIdentity(query.IdentityProvider, query.Id)
        });

        var registrant = results.Items.SingleOrDefault();

        return new UserProfileQueryResponse(mapper.Map<UserProfile?>(registrant));
    }
}

public record UserProfileQuery(string IdentityProvider, string Id);

public record UserProfileQueryResponse(UserProfile? UserProfile);

public record UserProfile(
    string FirstName,
    string LastName,
    string DateOfBirth,
    string? Email,
    string? Phone,
    string? HomeAddress
    );