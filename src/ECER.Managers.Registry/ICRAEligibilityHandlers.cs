﻿using AutoMapper;
using ECER.Engines.Validation.ICRA;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.ICRA;
using ECER.Resources.Documents.ICRA;
using MediatR;

namespace ECER.Managers.Registry;

/// <summary>
/// Message handlers
/// </summary>
public class ICRAEligibilityHandlers(
     IICRARepository iCRARepository,
     IMapper mapper,
     IICRAValidationEngine icraValidationEngine)
  : IRequestHandler<SaveICRAEligibilityCommand, Contract.ICRA.ICRAEligibility?>,
    IRequestHandler<ICRAEligibilitiesQuery, ICRAEligibilitiesQueryResults>,
    IRequestHandler<SubmitICRAEligibilityCommand, SubmitICRAEligibilityResult>
{
  public async Task<Contract.ICRA.ICRAEligibility?> Handle(SaveICRAEligibilityCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    if (request.eligibility.Id == null)
    {
      var icraEligibilities = await iCRARepository.Query(new ICRAQuery
      {
        ByApplicantId = request.eligibility.ApplicantId,
        ByStatus = new Resources.Documents.ICRA.ICRAStatus[] {
          Resources.Documents.ICRA.ICRAStatus.Draft,
          Resources.Documents.ICRA.ICRAStatus.Submitted,
          Resources.Documents.ICRA.ICRAStatus.InReview,
          Resources.Documents.ICRA.ICRAStatus.ReadyforReview,
          Resources.Documents.ICRA.ICRAStatus.ReadyforAssessment
        }
      }, cancellationToken);

      //this checks for any submitted Icra eligibilities
      if (icraEligibilities.Any(e => e.Status != Resources.Documents.ICRA.ICRAStatus.Draft))
      {
        throw new InvalidOperationException($"Applicant id: {request.eligibility.ApplicantId} has a submitted Icra eligibility assessment in progress. A new draft cannot be created.");
      }

      if (icraEligibilities.Any(e => e.Status == Resources.Documents.ICRA.ICRAStatus.Draft))
      {
        // user already has a draft icra eligibility
        throw new InvalidOperationException($"Applicant id: {request.eligibility.ApplicantId} has a draft ICRA in progress. A new draft cannot be created");
      }
    }
    var iCRAEligibilityId = await iCRARepository.Save(mapper.Map<Resources.Documents.ICRA.ICRAEligibility>(request.eligibility)!, cancellationToken);

    var freshIcraEligibilities = await iCRARepository.Query(new ICRAQuery
    {
      ById = iCRAEligibilityId,
    }, cancellationToken);

    return mapper.Map<Contract.ICRA.ICRAEligibility>(freshIcraEligibilities.SingleOrDefault())!;
  }

  public async Task<SubmitICRAEligibilityResult> Handle(SubmitICRAEligibilityCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var eligibilities = await iCRARepository.Query(new ICRAQuery
    {
      ByApplicantId = request.userId,
      ByStatus = new Resources.Documents.ICRA.ICRAStatus[]
      {
        Resources.Documents.ICRA.ICRAStatus.Draft,
        Resources.Documents.ICRA.ICRAStatus.Submitted,
        Resources.Documents.ICRA.ICRAStatus.InReview,
        Resources.Documents.ICRA.ICRAStatus.ReadyforReview,
        Resources.Documents.ICRA.ICRAStatus.ReadyforAssessment
      }
    }, cancellationToken);

    if (eligibilities.Any(e => e.Status == Resources.Documents.ICRA.ICRAStatus.Submitted || e.Status == Resources.Documents.ICRA.ICRAStatus.InReview || e.Status == Resources.Documents.ICRA.ICRAStatus.ReadyforReview))
    {
      return new SubmitICRAEligibilityResult { Eligibility = null, Error = Contract.ICRA.SubmissionError.DraftIcraEligibilityValidationFailed, ValidationErrors = new List<string> { "submitted icra eligibility already exists" } };
    }

    var draftResource = eligibilities.FirstOrDefault(e => e.Id == request.icraEligibilityId && e.Status == Resources.Documents.ICRA.ICRAStatus.Draft);
    var draft = mapper.Map<Contract.ICRA.ICRAEligibility>(draftResource!);
    if (draft == null)
    {
      return new SubmitICRAEligibilityResult { Eligibility = null, Error = Contract.ICRA.SubmissionError.DraftIcraEligibilityNotFound, ValidationErrors = new List<string> { "draft icra eligibility does not exist" } };
    }

    var validation = await icraValidationEngine.Validate(draft!);
    if (!validation.IsValid)
    {
      return new SubmitICRAEligibilityResult { Eligibility = null, Error = Contract.ICRA.SubmissionError.DraftIcraEligibilityValidationFailed, ValidationErrors = validation.ValidationErrors };
    }

    var id = await iCRARepository.Submit(draft.Id!, cancellationToken);

    var fresh = await iCRARepository.Query(new ICRAQuery { ById = id }, cancellationToken);
    return new SubmitICRAEligibilityResult { Eligibility = mapper.Map<Contract.ICRA.ICRAEligibility>(fresh.SingleOrDefault()) };
  }

  public async Task<ICRAEligibilitiesQueryResults> Handle(ICRAEligibilitiesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var eligibilities = await iCRARepository.Query(new ICRAQuery
    {
      ById = request.ById,
      ByApplicantId = request.ByApplicantId,
      ByStatus = request.ByStatus?.Convert<Contract.ICRA.ICRAStatus, Resources.Documents.ICRA.ICRAStatus>(),
    }, cancellationToken);
    return new ICRAEligibilitiesQueryResults(mapper.Map<IEnumerable<Contract.ICRA.ICRAEligibility>>(eligibilities)!);
  }
}
