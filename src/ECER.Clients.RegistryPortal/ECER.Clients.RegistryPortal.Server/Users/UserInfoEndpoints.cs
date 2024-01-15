﻿using System.ComponentModel.DataAnnotations;
using AutoMapper;
using ECER.Managers.Registry;
using ECER.Utilities.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server.Users;

public class UserInfoEndpoints : IRegisterEndpoints
{
    public void Register(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("api/userinfo", async Task<Results<Ok<UserInfoResponse>, NotFound, ForbidHttpResult>> (HttpContext ctx, CancellationToken ct, IMessageBus bus, IMapper mapper) =>
        {
            var login = AuthenticationService.GetUserLogin(ctx.User);
            if (login == null) return TypedResults.Forbid();
            var result = await bus.InvokeAsync<UserProfileQueryResponse>(new UserProfileQuery(login.Value.identityProvider, login.Value.id), ct);
            if (result.UserProfile == null) return TypedResults.NotFound();
            return TypedResults.Ok(new UserInfoResponse(mapper.Map<UserProfile>(result.UserProfile)));
        }).WithOpenApi(op =>
        {
            op.OperationId = "GetUserInfo";
            op.Summary = "Get user profile information";
            op.Description = "Gets the current user profile information";
            return op;
        }).RequireAuthorization();

        endpointRouteBuilder.MapPost("api/userinfo/profile", async Task<Results<Ok, ForbidHttpResult>> (NewUserRequest request, HttpContext ctx, CancellationToken ct, IMessageBus bus, IMapper mapper) =>
        {
            var login = AuthenticationService.GetUserLogin(ctx.User);
            if (login == null) return TypedResults.Forbid();

            await bus.InvokeAsync<string>(new RegisterNewUserCommand(mapper.Map<Managers.Registry.UserProfile>(request.Profile), new Login(login.Value.identityProvider, login.Value.id)));

            return TypedResults.Ok();
        }).WithOpenApi(op =>
        {
                op.OperationId = "CreateUserInfo";
                op.Summary = "Create user profile information";
                op.Description = "Create new user";
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
/// <param name="MailingAddress">The mailing addess</param>
public record UserProfile(
    string? FirstName,
    string? LastName,
    DateOnly? DateOfBirth,
    string Email,
    string Phone,
    Address? HomeAddress,
    Address? MailingAddress
    );

/// <summary>
/// Address
/// </summary>
/// <param name="Line1"></param>
/// <param name="Line2"></param>
/// <param name="City"></param>
/// <param name="PostalCode"></param>
/// <param name="Province"></param>
/// <param name="Country"></param>
public record Address(
    string Line1,
    string? Line2,
    string City,
    string PostalCode,
    string? Province,
    string Country
    );

/// <summary>
/// New user request
/// </summary>
public record NewUserRequest
{
    [Required]
    public UserProfile Profile { get; set; } = null!;
}