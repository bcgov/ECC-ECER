using AutoMapper;
using ECER.Managers.Registry.Contract.Certifications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;

namespace ECER.Clients.RegistryPortal.Server.Certifications;

public class CertificationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/certifications/{id?}", async (string? id, HttpContext httpContext, IMediator messageBus, IMapper mapper) =>
    {
      var userId = httpContext.User.GetUserContext()?.UserId;
      bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid) { id = null; }

      var query = new UserCertificationQuery
      {
        ById = id,
        ByApplicantId = userId,
      };
      var results = await messageBus.Send<CertificationsQueryResults>(query);
      return TypedResults.Ok(mapper.Map<IEnumerable<Certification>>(results.Items));
    })
     .WithOpenApi("Handles certification queries", string.Empty, "certification_get")
     .RequireAuthorization()
     .WithParameterValidation();
  }
}

public record Certification(string Id)
{
  public string? Number { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public DateTime? EffectiveDate { get; set; }
  public DateTime? Date { get; set; }
  public DateTime? PrintDate { get; set; }
  public bool? HasConditions { get; set; }
  public string? LevelName { get; set; }
  public CertificateStatusCode? StatusCode { get; set; }
  public YesNoNull? IneligibleReference { get; set; }
  public IEnumerable<CertificationLevel> Levels { get; set; } = Array.Empty<CertificationLevel>();
  public IEnumerable<CertificationFile> Files { get; set; } = Array.Empty<CertificationFile>();
}

public record CertificationLevel(string Id)
{
  public string? Type { get; set; }
}
public record CertificationFile(string Id)
{
  public string? Url { get; set; }
  public string? Extention { get; set; }
  public string? Size { get; set; }
  public string? Name { get; set; }
}

public enum CertificateStatusCode
{
  Active,
  Cancelled,
  Expired,
  Inactive,
  Renewed,
  Reprinted,
  Suspended
}

public enum YesNoNull
{
  No,
  Yes,
}
