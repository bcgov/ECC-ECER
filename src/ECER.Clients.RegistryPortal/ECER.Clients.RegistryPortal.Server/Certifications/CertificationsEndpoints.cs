using ECER.Managers.Registry.Contract.Certifications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using AutoMapper;

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
  public bool? HasConditions { get; set; }
  public bool? Level { get; set; }
  public CertificateStatusCode? StatusCode { get; set; }
  public YesNoNull? IneligibleReference { get; set; }
}

public enum CertificateStatusCode
{
  Active,
  Cancelled,
  Expired,
  Inactive,
  Reprinted,
  Suspended
}

public enum YesNoNull
{
  No,
  Yes,
}
