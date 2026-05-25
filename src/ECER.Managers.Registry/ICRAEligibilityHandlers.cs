using ECER.Engines.Validation.ICRA;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.ICRA;
using ECER.Resources.Documents.ICRA;
using Mediator;
using ICRAStatus = ECER.Resources.Documents.ICRA.ICRAStatus;

namespace ECER.Managers.Registry;

/// <summary>
/// Message handlers
/// </summary>
public class ICRAEligibilityHandlers(
     IICRARepository iCRARepository,
     IICRAEligibilityMapper icraEligibilityMapper,
     IICRAValidationEngine icraValidationEngine)
  : IRequestHandler<SaveICRAEligibilityCommand, Contract.ICRA.ICRAEligibility?>,
    IRequestHandler<ICRAEligibilitiesQuery, ICRAEligibilitiesQueryResults>,
    IRequestHandler<SubmitICRAEligibilityCommand, SubmitICRAEligibilityResult>,
    IRequestHandler<ResendIcraWorkExperienceReferenceInviteCommand, string>,
    IRequestHandler<AddIcraWorkExperienceReferenceCommand, AddOrReplaceIcraWorkExperienceReferenceResult>,
    IRequestHandler<ReplaceIcraWorkExperienceReferenceCommand, AddOrReplaceIcraWorkExperienceReferenceResult>,
    IRequestHandler<GetIcraWorkExperienceReferenceByIdCommand, Contract.ICRA.EmploymentReference>
{
  public async ValueTask<Contract.ICRA.ICRAEligibility?> Handle(SaveICRAEligibilityCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    if (request.eligibility.Id == null)
    {
      var icraEligibilities = await iCRARepository.Query(new ICRAQuery
      {
        ByApplicantId = request.eligibility.ApplicantId,
        ByStatus =
        [
          Resources.Documents.ICRA.ICRAStatus.Draft,
          Resources.Documents.ICRA.ICRAStatus.Submitted,
          Resources.Documents.ICRA.ICRAStatus.InReview,
          Resources.Documents.ICRA.ICRAStatus.ReadyforReview,
          Resources.Documents.ICRA.ICRAStatus.ReadyforAssessment
        ]
      }, cancellationToken);

      if (icraEligibilities.Any(e => e.Status != Resources.Documents.ICRA.ICRAStatus.Draft))
      {
        throw new InvalidOperationException($"Applicant id: {request.eligibility.ApplicantId} has a submitted Icra eligibility assessment in progress. A new draft cannot be created.");
      }

      if (icraEligibilities.Any(e => e.Status == Resources.Documents.ICRA.ICRAStatus.Draft))
      {
        throw new InvalidOperationException($"Applicant id: {request.eligibility.ApplicantId} has a draft ICRA in progress. A new draft cannot be created");
      }
    }

    request.eligibility.Origin = Contract.ICRA.IcraEligibilityOrigin.Portal;

    foreach (var reference in request.eligibility.EmploymentReferences)
    {
      reference.Type = Contract.ICRA.WorkExperienceTypesIcra.ICRA;
    }

    var iCRAEligibilityId = await iCRARepository.Save(icraEligibilityMapper.MapEligibility(request.eligibility), cancellationToken);

    var freshIcraEligibilities = await iCRARepository.Query(new ICRAQuery
    {
      ById = iCRAEligibilityId,
    }, cancellationToken);

    return icraEligibilityMapper.MapEligibility(freshIcraEligibilities.SingleOrDefault());
  }

  public async ValueTask<SubmitICRAEligibilityResult> Handle(SubmitICRAEligibilityCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var eligibilities = await iCRARepository.Query(new ICRAQuery
    {
      ByApplicantId = request.userId,
      ByStatus =
      [
        Resources.Documents.ICRA.ICRAStatus.Draft,
        Resources.Documents.ICRA.ICRAStatus.Submitted,
        Resources.Documents.ICRA.ICRAStatus.InReview,
        Resources.Documents.ICRA.ICRAStatus.ReadyforReview,
        Resources.Documents.ICRA.ICRAStatus.ReadyforAssessment
      ]
    }, cancellationToken);

    if (eligibilities.Any(e => e.Status == Resources.Documents.ICRA.ICRAStatus.Submitted || e.Status == Resources.Documents.ICRA.ICRAStatus.InReview || e.Status == Resources.Documents.ICRA.ICRAStatus.ReadyforReview))
    {
      return new SubmitICRAEligibilityResult { Eligibility = null, Error = Contract.ICRA.SubmissionError.DraftIcraEligibilityValidationFailed, ValidationErrors = new List<string> { "submitted icra eligibility already exists" } };
    }

    var draft = icraEligibilityMapper.MapEligibility(eligibilities.FirstOrDefault(dst =>
      dst.Id == request.icraEligibilityId && dst.Status == Resources.Documents.ICRA.ICRAStatus.Draft));
    if (draft == null)
    {
      return new SubmitICRAEligibilityResult { Eligibility = null, Error = Contract.ICRA.SubmissionError.DraftIcraEligibilityNotFound, ValidationErrors = new List<string> { "draft icra eligibility does not exist" } };
    }

    var validation = await icraValidationEngine.Validate(draft);
    if (!validation.IsValid)
    {
      return new SubmitICRAEligibilityResult { Eligibility = null, Error = Contract.ICRA.SubmissionError.DraftIcraEligibilityValidationFailed, ValidationErrors = validation.ValidationErrors };
    }

    var id = await iCRARepository.Submit(draft.Id!, cancellationToken);

    var fresh = await iCRARepository.Query(new ICRAQuery { ById = id }, cancellationToken);
    return new SubmitICRAEligibilityResult { Eligibility = icraEligibilityMapper.MapEligibility(fresh.SingleOrDefault()) };
  }

  public async ValueTask<ICRAEligibilitiesQueryResults> Handle(ICRAEligibilitiesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var eligibilities = await iCRARepository.Query(new ICRAQuery
    {
      ById = request.ById,
      ByApplicantId = request.ByApplicantId,
      ByStatus = request.ByStatus?.Convert<Contract.ICRA.ICRAStatus, Resources.Documents.ICRA.ICRAStatus>(),
    }, cancellationToken);
    return new ICRAEligibilitiesQueryResults(icraEligibilityMapper.MapEligibilities(eligibilities));
  }

  public async ValueTask<string> Handle(ResendIcraWorkExperienceReferenceInviteCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var eligibilities = await iCRARepository.Query(new ICRAQuery
    {
      ById = request.IcraEligibilityId,
      ByApplicantId = request.UserId,
      ByStatus = [ICRAStatus.Submitted],
    }, cancellationToken);

    if (!eligibilities.Any())
    {
      throw new InvalidOperationException($"ICRA eligibility application not found id '{request.IcraEligibilityId}' or ICRA application is past submitted stage");
    }
    ArgumentNullException.ThrowIfNull(request);

    bool foundReference = eligibilities.Any(eligibility =>
      eligibility.EmploymentReferences.Any(reference => reference.Id == request.ReferenceId)
    );

    if (!foundReference)
    {
      throw new InvalidOperationException($"reference id '{request.ReferenceId}' does not belong to user '{request.UserId}' or Icra application '{request.IcraEligibilityId}'");
    }

    var icraWorkExperienceReferenceId = await iCRARepository.ResendIcraWorkExperienceReferenceInvite(new ResendIcraReferenceInviteRequest(request.ReferenceId), cancellationToken);
    return icraWorkExperienceReferenceId;
  }

  public async ValueTask<AddOrReplaceIcraWorkExperienceReferenceResult> Handle(AddIcraWorkExperienceReferenceCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var eligibilities = await iCRARepository.Query(new ICRAQuery
    {
      ById = request.IcraEligibilityId,
      ByApplicantId = request.UserId,
      ByStatus = [ICRAStatus.Submitted],
    }, cancellationToken);

    if (!eligibilities.Any())
    {
      return new AddOrReplaceIcraWorkExperienceReferenceResult
      {
        IsSuccess = false,
        ErrorMessage = $"ICRA eligibility application not found id '{request.IcraEligibilityId}' or ICRA application is past submitted stage"
      };
    }

    var icraWorkExperienceReference = await iCRARepository.AddIcraWorkExperienceReference(
      new AddIcraWorkExperienceReferenceRequest(
        icraEligibilityMapper.MapEmploymentReference(request.EmploymentReference),
        request.IcraEligibilityId,
        request.UserId),
      cancellationToken);

    return new AddOrReplaceIcraWorkExperienceReferenceResult { IsSuccess = true, EmploymentReference = icraEligibilityMapper.MapEmploymentReference(icraWorkExperienceReference) };
  }

  public async ValueTask<AddOrReplaceIcraWorkExperienceReferenceResult> Handle(ReplaceIcraWorkExperienceReferenceCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var eligibilities = await iCRARepository.Query(new ICRAQuery
    {
      ById = request.IcraEligibilityId,
      ByApplicantId = request.UserId,
      ByStatus = [ICRAStatus.Submitted],
    }, cancellationToken);

    if (!eligibilities.Any())
    {
      return new AddOrReplaceIcraWorkExperienceReferenceResult
      {
        IsSuccess = false,
        ErrorMessage = $"ICRA eligibility application not found id '{request.IcraEligibilityId}' or ICRA application is past submitted stage"
      };
    }

    bool foundReference = eligibilities.Any(eligibility =>
      eligibility.EmploymentReferences.Any(reference => reference.Id == request.ReferenceId)
    );

    if (!foundReference)
    {
      return new AddOrReplaceIcraWorkExperienceReferenceResult
      {
        IsSuccess = false,
        ErrorMessage = $"reference id '{request.ReferenceId}' does not belong to user '{request.UserId}' or Icra application '{request.IcraEligibilityId}'"
      };
    }

    var icraWorkExperienceReference = await iCRARepository.ReplaceIcraWorkExperienceReference(
      new ReplaceIcraWorkExperienceReferenceRequest(
        icraEligibilityMapper.MapEmploymentReference(request.EmploymentReference),
        request.IcraEligibilityId,
        request.ReferenceId,
        request.UserId),
      cancellationToken);

    return new AddOrReplaceIcraWorkExperienceReferenceResult { IsSuccess = true, EmploymentReference = icraEligibilityMapper.MapEmploymentReference(icraWorkExperienceReference) };
  }

  public async ValueTask<Contract.ICRA.EmploymentReference> Handle(GetIcraWorkExperienceReferenceByIdCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    try
    {
      var icraWorkExperienceReference = await iCRARepository.GetIcraWorkExperienceReferenceById(request.ReferenceId, request.ApplicantId, cancellationToken);
      return icraEligibilityMapper.MapEmploymentReference(icraWorkExperienceReference);
    }
    catch (InvalidOperationException)
    {
      return null!;
    }
  }
}
