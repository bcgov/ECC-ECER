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
    /// <returns>Query results</returns>
    public static async Task<UserProfileQueryResponse> Handle(UserProfileQuery query)
    {
        return await Task.FromResult(new UserProfileQueryResponse(null));
    }
}

public record UserProfileQuery();

public record UserProfileQueryResponse(UserProfile? UserProfile);

public record UserProfile();