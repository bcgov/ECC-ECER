using AutoMapper;
using ECER.Utilities.Hosting;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECER.Clients.RegistryPortal.Server.References;

public class ReferencesEndpoints : IRegisterEndpoints
{
  public void Register(IEndpointRouteBuilder endpointRouteBuilder)
  {
    endpointRouteBuilder.MapPost("/api/References", async Task<Results<Ok, BadRequest<string>>> (ReferenceSubmissionRequest request, IMediator messageBus, HttpContext httpContext, IMapper mapper, CancellationToken ct) =>
    {
      if (request.Token == null) return TypedResults.BadRequest("Token is required");
      var result = await messageBus.Send(mapper.Map<Managers.Registry.Contract.Applications.ReferenceSubmissionRequest>(request), ct);
      if (!result.IsSuccess)
      {
        return TypedResults.BadRequest(result.ErrorMessage);
      }
      return TypedResults.Ok();
    }).WithOpenApi("Handles reference submission", string.Empty, "references_post").WithParameterValidation();
  }
}

public record ReferenceSubmissionRequest(string Token, ReferenceContactInformation ReferenceContactInformation, ReferenceEvaluation ReferenceEvaluation, bool ResponseAccuracyConfirmation);
public record ReferenceContactInformation(string LastName, string FirstName, string Email, string PhoneNumber, string CertificateNumber, string CertificateProvince);
public record ReferenceEvaluation(string Relationship, string LengthOfAcquaintance, bool WorkedWithChildren, string ChildInteractionObservations, string ApplicantTemperamentAssessment, bool Confirmed);
