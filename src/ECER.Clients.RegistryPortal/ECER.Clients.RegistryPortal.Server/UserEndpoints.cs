using ECER.Utilities.Hosting;

namespace ECER.Clients.RegistryPortal.Server;

public class UserEndpoints : IRegisterEndpoints
{
    public void Register(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("api/userinfo", async (HttpContext ctx) =>
        {
            await Task.CompletedTask;
            return TypedResults.Ok(new UserInfoResponse(true, null));
        }).WithOpenApi(op =>
        {
            op.OperationId = "GetUserInfo";
            op.Summary = "Get user profile information";
            op.Description = "Gets the current user profile information";
            return op;
        });
    }
}

/// <summary>
/// User profile information response
/// </summary>
/// <param name="IsNew">True if the user is new, false if returning user</param>
/// <param name="UserInfo">Optional user information payload, only available if the user exists</param>
public record UserInfoResponse(bool IsNew, UserInfo? UserInfo);

/// <summary>
/// User profile information
/// </summary>
/// <param name="FirstName">First name</param>
/// <param name="LastName">Last name</param>
/// <param name="DateOfBirth">Date of birth in the form of yyyy-MM-dd</param>
/// <param name="Email">Email address</param>
/// <param name="Phone">Phone number</param>
/// <param name="HomeAddress">The home address</param>
public record UserInfo(
    string FirstName,
    string LastName,
    string DateOfBirth,
    string? Email,
    string? Phone,
    string? HomeAddress
    );