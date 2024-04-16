namespace ECER.Resources.Documents.References;

public interface IReferenceRepository
{
  Task<bool> SubmitReferenceResponse(ReferenceSubmissionRequest request, CancellationToken ct);
}

public record ReferenceSubmissionRequest(string Token, ReferenceContactInformation ReferenceContactInformation, ReferenceEvaluation ReferenceEvaluation, bool ResponseAccuracyConfirmation);
public record ReferenceContactInformation(string LastName, string FirstName, string Email, string PhoneNumber, string CertificateNumber, string CertificateProvince);
public record ReferenceEvaluation(string Relationship, string LengthOfAcquaintance, bool WorkedWithChildren, string ChildInteractionObservations, string ApplicantTemperamentAssessment, bool Confirmed);
