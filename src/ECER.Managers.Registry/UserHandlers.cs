using AutoMapper;
using ECER.Resources.Accounts.Registrants;
using ECER.Utilities.Security;

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
            WithIdentity = query.UserIdentity
        });

        var registrant = results.Items.SingleOrDefault();

        return new UserProfileQueryResponse(registrant?.Id, mapper.Map<UserProfile?>(registrant?.Profile));
    }

    public static async Task<string> Handle(RegisterNewUserCommand cmd, IRegistrantRepository registrantRepository, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(registrantRepository);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(cmd);

        var results = await registrantRepository.Query(new RegistrantQuery
        {
            WithIdentity = cmd.Identity
        });

        if (results.Items.Any()) throw new InvalidOperationException($"Registrant with identity {cmd.Identity} already exists");

        return await registrantRepository.RegisterNew(mapper.Map<NewRegistrantRequest>(cmd));
    }
}

public record UserProfileQuery(UserIdentity UserIdentity);

public record RegisterNewUserCommand(UserProfile UserProfile, UserIdentity Identity);

public record UserProfileQueryResponse(string? UserId, UserProfile? UserProfile);

public record UserProfile(
    string FirstName,
    string LastName,
    DateOnly? DateOfBirth,
    string Email,
    string Phone,
    Address? HomeAddress,
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