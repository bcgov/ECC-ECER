using AutoMapper;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Managers.Registry.Contract.Certifications;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.RegistryPortal.Server.Certifications;

public class CertificationsEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapGet("/api/certifications/{id?}", async (string? id, HttpContext httpContext, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      var userId = httpContext.User.GetUserContext()?.UserId;
      if (string.IsNullOrEmpty(id))
      {
        id == null;
      }
      else
      {
        bool IdIsNotGuid = !Guid.TryParse(id, out _); 
        if (IdIsNotGuid) 
        { return TypedResults.BadRequest("Id is not a valid guid"); }
      }

      var query = new UserCertificationQuery
      {
        ById = id,
        ByApplicantId = userId,
      };
      var results = await messageBus.Send<CertificationsQueryResults>(query, ct);
      return TypedResults.Ok(mapper.Map<IEnumerable<Certification>>(results.Items));
    })
     .WithOpenApi("Handles certification queries", string.Empty, "certification_get")
    .RequireAuthorization()
    .WithParameterValidation();

    endpointRouteBuilder.MapPut("/api/certifications/RequestPdf/{id?}", async Task<Results<Ok<string>, BadRequest<ProblemDetails>>> (string? id, HttpContext httpContext, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
    {
      var userId = httpContext.User.GetUserContext()?.UserId;
      bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid) { id = null; }
      if (IdIsNotGuid)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Detail = "Invalid certificate Id",
          Extensions = { ["errors"] = "Invalid certificate Id" }
        };
        return TypedResults.BadRequest(problemDetails);
      }

      var query = new RequestCertificationPdfCommand(id!, userId!);
      var result = await messageBus.Send(query, ct);

      return TypedResults.Ok(result.CertificationId);
    })
    .WithOpenApi("Handles certification queries", string.Empty, "certification_requestpdf_put")
    .RequireAuthorization()
    .WithParameterValidation();

    endpointRouteBuilder.MapPost("/api/certifications/lookup", async Task<Results<Ok<IEnumerable<CertificationLookupResponse>>, BadRequest<ProblemDetails>, NotFound>> (CertificationLookupRequest request, HttpContext httpContext, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
    {
      var recaptchaResult = await messageBus.Send(new Managers.Registry.Contract.Recaptcha.VerifyRecaptchaCommand(request.RecaptchaToken), ct);

      if (!recaptchaResult.Success)
      {
        var problemDetails = new ProblemDetails
        {
          Status = StatusCodes.Status400BadRequest,
          Detail = "Invalid recaptcha token",
          Extensions = { ["errors"] = recaptchaResult.ErrorCodes }
        };
        return TypedResults.BadRequest(problemDetails);
      }

      var query = new UserCertificationQueryLookup
      {
        ByCertificateNumber = request.RegistrationNumber,
        ByFirstName = request.FirstName,
        ByLastName = request.LastName,
        PageNumber = request.PageNumber,
        PageSize = request.PageSize
      };
      var results = await messageBus.Send(query, ct);
      return TypedResults.Ok(mapper.Map<IEnumerable<CertificationLookupResponse>>(results.Items));
    })
     .WithOpenApi("Handles certifications lookup queries", string.Empty, "certifications_lookup_post")
     .WithParameterValidation();
  }
}

public record Certification(string Id)
{
  public string? Name { get; set; }
  public string? Number { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public DateTime? EffectiveDate { get; set; }
  public DateTime? Date { get; set; }
  public DateTime? PrintDate { get; set; }
  public bool? HasConditions { get; set; }
  public string? LevelName { get; set; }
  public CertificateStatusCode? StatusCode { get; set; }
  public CertificatePDFGeneration? CertificatePDFGeneration { get; set; }
  public YesNoNull? IneligibleReference { get; set; }
  public IEnumerable<CertificationLevel> Levels { get; set; } = Array.Empty<CertificationLevel>();
  public IEnumerable<CertificationFile> Files { get; set; } = Array.Empty<CertificationFile>();
  public IEnumerable<CertificateCondition> CertificateConditions { get; set; } = Array.Empty<CertificateCondition>();
}

public record CertificationLookupResponse(string Id)
{
  public string? Name { get; set; }
  public string? RegistrationNumber { get; set; }
  public CertificateStatusCode? StatusCode { get; set; }
  public string? LevelName { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public bool? HasConditions { get; set; }
  public IEnumerable<CertificationLevel> Levels { get; set; } = Array.Empty<CertificationLevel>();
  public IEnumerable<CertificateCondition> CertificateConditions { get; set; } = Array.Empty<CertificateCondition>();
}

public record CertificationLookupRequest(string RecaptchaToken)
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? RegistrationNumber { get; set; }
  public int PageSize { get; set; }
  public int PageNumber { get; set; }
  public string? SortField { get; set; }
  public string? SortDirection { get; set; }
}

public record CertificateCondition
{
  public string? Id { get; set; }
  public string? Name { get; set; }
  public string? Details { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public int DisplayOrder { get; set; }
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

public enum CertificatePDFGeneration
{
  No,
  Requested,
  Yes,
}

public enum YesNoNull
{
  No,
  Yes,
}
