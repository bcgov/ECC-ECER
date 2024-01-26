using ECER.Utilities.Security;

namespace ECER.Managers.Registry.Contract;

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
