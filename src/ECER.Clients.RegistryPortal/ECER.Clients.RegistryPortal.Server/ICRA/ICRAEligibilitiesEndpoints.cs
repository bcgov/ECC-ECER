using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.ICRA;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.RegistryPortal.Server.ICRA;

public class ICRAEligibilitiesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPut("/api/icra/{id?}", async Task<Results<Ok<DraftICRAEligibilityResponse>, BadRequest<string>, NotFound>> (string? id, SaveDraftICRAEligibilityRequest request, HttpContext ctx, CancellationToken ct, IMediator messageBus, IMapper mapper) =>
        {
          bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid && id != null) { id = null; }
          bool PayloadIdIsNotGuid = !Guid.TryParse(request.Eligibility.Id, out _); if (PayloadIdIsNotGuid && request.Eligibility.Id != null) { request.Eligibility.Id = null; }

          if (request.Eligibility.Id != id) return TypedResults.BadRequest("resource id and payload id do not match");
          var userContext = ctx.User.GetUserContext();
          request.Eligibility.ApplicantId = userContext!.UserId;
          var eligibility = mapper.Map<Managers.Registry.Contract.ICRA.ICRAEligibility>(request.Eligibility)!;

          if (id != null)
          {
            var query = new ICRAEligibilitiesQuery
            {
              ById = id,
              ByApplicantId = userContext!.UserId,
              ByStatus = [Managers.Registry.Contract.ICRA.ICRAStatus.Draft]
            };
            var results = await messageBus.Send(query, ct);
            if (!results.Items.Any()) return TypedResults.NotFound();
          }

          var freshEligibility = await messageBus.Send(new SaveICRAEligibilityCommand(eligibility), ct);
          return TypedResults.Ok(new DraftICRAEligibilityResponse(mapper.Map<ICRAEligibility>(freshEligibility)));
        })
        .WithOpenApi("Save a draft icra eligibility for the current user", string.Empty, "icra_put")
        .RequireAuthorization()
        .WithParameterValidation();

    endpointRouteBuilder.MapGet("/api/icra/{id?}", async (string? id, ICRAStatus[]? byStatus, HttpContext ctx, IMediator messageBus, IMapper mapper, CancellationToken ct) =>
        {
          var userId = ctx.User.GetUserContext()?.UserId;

          bool IdIsNotGuid = !Guid.TryParse(id, out _); if (IdIsNotGuid) { id = null; }
          var query = new ICRAEligibilitiesQuery
          {
            ById = id,
            ByApplicantId = userId,
            ByStatus = byStatus?.Convert<ICRAStatus, Managers.Registry.Contract.ICRA.ICRAStatus>()
          };
          var results = await messageBus.Send(query, ct);
          return TypedResults.Ok(mapper.Map<IEnumerable<ICRAEligibility>>(results.Items));
        })
        .WithOpenApi("Handles icra queries", string.Empty, "icra_get")
        .RequireAuthorization()
        .WithParameterValidation();
  }
}

public record SaveDraftICRAEligibilityRequest(ICRAEligibility Eligibility);

public record DraftICRAEligibilityResponse(ICRAEligibility Eligibility);

public record ICRAEligibilityQueryResponse(IEnumerable<ICRAEligibility> Items);

public record ICRAEligibility()
{
  public string? Id { get; set; }
  public string ApplicantId { get; set; } = string.Empty;
  public string? PortalStage { get; set; }

  public DateTime? SignedDate { get; set; }
  public DateTime? CreatedOn { get; set; }

  public ICRAStatus Status { get; set; }
  public IEnumerable<InternationalCertification> InternationalCertifications { get; set; } = Array.Empty<InternationalCertification>();
}
public record InternationalCertification
{
  public string? Id { get; set; }
  public string? CountryId { get; set; }
  public string? NameOfRegulatoryAuthority { get; set; }
  public string? EmailOfRegulatoryAuthority { get; set; }
  public string? PhoneOfRegulatoryAuthority { get; set; }
  public string? WebsiteOfRegulatoryAuthority { get; set; }
  public string? OnlineCertificateValidationToolOfRegulatoryAuthority { get; set; }
  public CertificateStatus CertificateStatus { get; set; }
  public string? CertificateTitle { get; set; }
  public DateTime? IssueDate { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public IEnumerable<Applications.FileInfo> Files { get; set; } = Array.Empty<Applications.FileInfo>();
  public IEnumerable<string> DeletedFiles { get; set; } = Array.Empty<string>();
  public IEnumerable<string> NewFiles { get; set; } = Array.Empty<string>();
}

public enum CertificateStatus
{
  Valid,
  Expired
}

public enum ICRAStatus
{
  Active,
  Draft,
  Eligible,
  Inactive,
  Ineligible,
  InReview,
  ReadyforReview,
  Submitted
}
