using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Managers.Registry.Contract.ICRA;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.ICRA;
using MediatR;


namespace ECER.Managers.Registry;

/// <summary>
/// Message handlers
/// </summary>
public class ICRAEligibilityHandlers(
     IICRARepository iCRARepository,
     IMapper mapper)
  : IRequestHandler<SaveICRAEligibilityCommand, Contract.ICRA.ICRAEligibility?>,
    IRequestHandler<ICRAEligibilitiesQuery, ICRAEligibilitiesQueryResults>
{

  public async Task<Contract.ICRA.ICRAEligibility?> Handle(SaveICRAEligibilityCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    if (request.eligibility.Id == null)
    {
      var icraEligibilities = await iCRARepository.Query(new ICRAQuery
      {
        ByApplicantId = request.eligibility.ApplicantId,
        ByStatus = [Resources.Documents.ICRA.ICRAStatus.Draft]
      }, cancellationToken);

      var existingDraftICRA = icraEligibilities.FirstOrDefault();
      if (existingDraftICRA != null)
      {
        // user already has a draft icra eligibility
        throw new InvalidOperationException($"User already has a draft ICRA with id '{existingDraftICRA.Id}'");
      }
    }
    var iCRAEligibilityId = await iCRARepository.Save(mapper.Map<Resources.Documents.ICRA.ICRAEligibility>(request.eligibility)!, cancellationToken);

    var freshIcraEligibilities = await iCRARepository.Query(new ICRAQuery
    {
      ById = iCRAEligibilityId,
    }, cancellationToken);

    return mapper.Map<Contract.ICRA.ICRAEligibility>(freshIcraEligibilities.SingleOrDefault())!;
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
