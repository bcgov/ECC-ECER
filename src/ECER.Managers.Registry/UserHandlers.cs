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

        return new UserProfileQueryResponse(mapper.Map<UserProfile?>(registrant?.Profile));
    }

    public static async Task<string> Handle(RegisterNewUserCommand cmd, IRegistrantRepository registrantRepository, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(registrantRepository);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(cmd);

        var results = await registrantRepository.Query(new RegistrantQuery
        {
            WithIdentity = new UserIdentity(cmd.Login.IdentityProvider, cmd.Login.id)
        });

        if (results.Items.Any()) throw new InvalidOperationException($"Registrant with login {cmd.Login.id}@{cmd.Login.IdentityProvider} already exists");

        return await registrantRepository.RegisterNew(mapper.Map<NewRegistrantRequest>(cmd));
    }
}

public record UserProfileQuery(string IdentityProvider, string Id);

public record RegisterNewUserCommand(UserProfile UserProfile, Login Login);

public record UserProfileQueryResponse(UserProfile? UserProfile);

public record UserProfile(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string Phone,
    Address HomeAddress,
    Address? MailingAddress
    );

public record Address(
    string Line1,
    string Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );

public record Login(string IdentityProvider, string id);