﻿using AutoMapper;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Clients.RegistryPortal.Server.Shared;
using ECER.Managers.Admin.Contract.Metadatas;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.Extensions.Options;

namespace ECER.Clients.RegistryPortal.Server;

public class ConfigurationEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/configuration", async (HttpContext ctx) =>
    {
      await Task.CompletedTask;
      var configuration = ctx.RequestServices.GetRequiredService<IConfiguration>();
      var appConfig = configuration.Get<ApplicationConfiguration>()!;
      return TypedResults.Ok(appConfig);
    }).WithOpenApi("Returns the UI initial configuration", string.Empty, "configuration_get");

    endpointRouteBuilder.MapGet("/api/provincelist", async (HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      var results = await messageBus.Send(new ProvincesQuery(), ct);
      return TypedResults.Ok(mapper.Map<IEnumerable<Province>>(results.Items));
    })
      .WithOpenApi("Handles province queries", string.Empty, "province_get")
      .CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5)));

    endpointRouteBuilder.MapGet("/api/systemMessages", async (HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      var results = await messageBus.Send(new SystemMessagesQuery(), ct);
      return TypedResults.Ok(mapper.Map<IEnumerable<SystemMessage>>(results.Items));
    })
     .WithOpenApi("Handles system messages queries", string.Empty, "systemMessage_get")
     .CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5)));

    endpointRouteBuilder.MapGet("/api/identificationTypes", async ([AsParameters] IdentificationTypesQuery request, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      var query = new Managers.Admin.Contract.Metadatas.IdentificationTypesQuery() { ById = request.ById, ForPrimary = request.ForPrimary, ForSecondary = request.ForSecondary };
      var results = await messageBus.Send(query, ct);
      return TypedResults.Ok(mapper.Map<IEnumerable<IdentificationType>>(results.Items));
    })
     .WithOpenApi("Handles identification types queries", string.Empty, "identificationTypes_get")
     .CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5)));

    endpointRouteBuilder.MapGet("/api/recaptchaSiteKey", async (IOptions<RecaptchaSettings> recaptchaSettings, CancellationToken ct) =>
    {
      await Task.CompletedTask;
      return TypedResults.Ok(recaptchaSettings.Value.SiteKey);
    })
      .WithOpenApi("Obtains site key for recaptcha", string.Empty, "recaptcha_site_key_get")
      .CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5)));
  }
}

#pragma warning disable CA2227 // Collection properties should be read only

public record ApplicationConfiguration
{
  public Dictionary<string, OidcAuthenticationSettings> ClientAuthenticationMethods { get; set; } = [];
}

#pragma warning restore CA2227 // Collection properties should be read only

public record OidcAuthenticationSettings
{
  public string Authority { get; set; } = null!;
  public string ClientId { get; set; } = null!;
  public string Scope { get; set; } = null!;
  public string? Idp { get; set; }
}
public record IdentificationType(string Id, string Name, bool ForPrimary, bool ForSecondary);

public record Province(string ProvinceId, string ProvinceName);
public record SystemMessage(string Name, string Subject, string Message)
{
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public IEnumerable<PortalTags> PortalTags { get; set; } = Array.Empty<PortalTags>();
}

public enum PortalTags
{
  LOGIN,
  LOOKUP,
  REFERENCES
}

public record IdentificationTypesQuery
{
  public string? ById { get; set; }
  public bool? ForPrimary { get; set; }
  public bool? ForSecondary { get; set; }
}
