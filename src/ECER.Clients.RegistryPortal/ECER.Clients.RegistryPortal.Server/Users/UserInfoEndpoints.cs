using AutoMapper;
using ECER.Managers.Registry;
using ECER.Utilities.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Users;

public class UserInfoEndpoints : IRegisterEndpoints
{
    public void Register(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("api/userinfo", async Task<Results<Ok<UserInfoResponse>, NotFound>> (CancellationToken ct, IMessageBus bus, IMapper mapper) =>
        {
            var result = await bus.InvokeAsync<UserProfileQueryResponse>(new UserProfileQuery(), ct);
            if (result == null) return TypedResults.NotFound();
            return TypedResults.Ok(new UserInfoResponse(mapper.Map<UserProfile>(result.UserProfile)));
        }).WithOpenApi(op =>
        {
            op.OperationId = "GetUserInfo";
            op.Summary = "Get user profile information";
            op.Description = "Gets the current user profile information";
            return op;
        }).RequireAuthorization();
    }
}

/// <summary>
/// User profile information response
/// </summary>
/// <param name="UserInfo">Optional user information payload, only available if the user exists</param>
public record UserInfoResponse(UserProfile UserInfo);

/// <summary>
/// User profile information
/// </summary>
/// <param name="FirstName">First name</param>
/// <param name="LastName">Last name</param>
/// <param name="DateOfBirth">Date of birth in the form of yyyy-MM-dd</param>
/// <param name="Email">Email address</param>
/// <param name="Phone">Phone number</param>
/// <param name="HomeAddress">The home address</param>
public record UserProfile(
    string FirstName,
    string LastName,
    string DateOfBirth,
    string? Email,
    string? Phone,
    string? HomeAddress
    );