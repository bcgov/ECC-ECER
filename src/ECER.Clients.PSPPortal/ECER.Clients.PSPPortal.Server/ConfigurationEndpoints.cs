using ECER.Clients.PSPPortal.Server.EducationInstitutions;
using ECER.Clients.PSPPortal.Server.Programs;
using ECER.Managers.Admin.Contract.Metadatas;
using ECER.Utilities.Hosting;
using Mediator;

namespace ECER.Clients.PSPPortal.Server;

public class ConfigurationEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/configuration", async (HttpContext ctx, IMediator messageBus, CancellationToken ct) =>
    {
      await Task.CompletedTask;
      var configuration = ctx.RequestServices.GetRequiredService<IConfiguration>();
      var appConfig = configuration.Get<ApplicationConfiguration>()!;
      return TypedResults.Ok(appConfig);
    }).WithOpenApi("Returns the UI initial configuration", string.Empty, "configuration_get")
      .CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5)));

    endpointRouteBuilder.MapGet("/api/provincelist", async (HttpContext ctx, IMediator messageBus, IConfigurationMapper mapper, CancellationToken ct) =>
    {
      var results = await messageBus.Send(new ProvincesQuery(), ct);
      return TypedResults.Ok(mapper.MapProvinces(results.Items));
    }).WithOpenApi("Handles province queries", string.Empty, "province_get")
      .CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5)));

    endpointRouteBuilder.MapGet("/api/countrylist", async (HttpContext ctx, IMediator messageBus, IConfigurationMapper mapper, CancellationToken ct) =>
    {
      var results = await messageBus.Send(new CountriesQuery(), ct);
      return TypedResults.Ok(mapper.MapCountries(results.Items));
    }).WithOpenApi("Handles country queries", string.Empty, "country_get")
      .CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5)));

    endpointRouteBuilder.MapGet("/api/areaofinstructionlist", async (HttpContext ctx, IMediator messageBus, IConfigurationMapper mapper, CancellationToken ct) =>
    {
      var results = await messageBus.Send(new AreaOfInstructionsQuery(), ct);
      var instructions = mapper.MapAreaOfInstructions(results.Items);
      return TypedResults.Ok(new AreaOfInstructionListResponse(instructions));
    }).WithOpenApi("Handles area of instruction queries", string.Empty, "area_of_instruction_get")
      .CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5)));

    endpointRouteBuilder.MapGet("/api/systemMessages", async (HttpContext ctx, IMediator messageBus, IConfigurationMapper mapper, CancellationToken ct) =>
    {
      var results = await messageBus.Send(new SystemMessagesQuery(), ct);
      return TypedResults.Ok(mapper.MapSystemMessages(results.Items));
    })
     .WithOpenApi("Handles system messages queries", string.Empty, "systemMessage_get")
     .CacheOutput(p => p.Expire(TimeSpan.FromMinutes(5)));
  }
}

#pragma warning disable CA2227

public record ApplicationConfiguration
{
  public Dictionary<string, OidcAuthenticationSettings> ClientAuthenticationMethods { get; set; } = [];
}

#pragma warning restore CA2227

public record OidcAuthenticationSettings
{
  public string Authority { get; set; } = null!;
  public string ClientId { get; set; } = null!;
  public string Scope { get; set; } = null!;
  public string? Idp { get; set; }
}

public record Province(string ProvinceId, string ProvinceName, string ProvinceCode);
public record Country(string CountryId, string CountryName, string CountryCode, bool IsICRA);
public record AreaOfInstructionListResponse(IEnumerable<AreaOfInstruction> AreaOfInstruction);
public record AreaOfInstruction(string Id, string Name, IEnumerable<ProgramTypes> ProgramTypes, int? MinimumHours, string? DisplayOrder, string? ParentAreaOfInstructionId);

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
  REFERENCES,
  PSPPortal,
  CertificationsPortal
}
