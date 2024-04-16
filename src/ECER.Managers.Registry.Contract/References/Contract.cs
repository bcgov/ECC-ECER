using MediatR;

namespace ECER.Managers.Registry.Contract.References;

public record ReferenceSubmissionRequest(string Token, ReferenceContactInformation ReferenceContactInformation, ReferenceEvaluation ReferenceEvaluation, bool ResponseAccuracyConfirmation) : IRequest<ReferenceSubmissionResult>;
public record ReferenceContactInformation(string LastName, string FirstName, string Email, string PhoneNumber, string CertificateNumber, string CertificateProvince);
public record ReferenceEvaluation(string Relationship, string LengthOfAcquaintance, bool WorkedWithChildren, string ChildInteractionObservations, string ApplicantTemperamentAssessment, bool Confirmed);

public class ReferenceSubmissionResult
{
  public bool IsSuccess { get; set; }
  public string? ErrorMessage { get; set; }

  public static ReferenceSubmissionResult Success() => new ReferenceSubmissionResult { IsSuccess = true };

  public static ReferenceSubmissionResult Failure(string message) => new ReferenceSubmissionResult { IsSuccess = false, ErrorMessage = message };
}
