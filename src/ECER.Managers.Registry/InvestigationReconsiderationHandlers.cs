using ECER.Managers.Registry.Contract.InvestigationReconsiderations;
using ECER.Resources.Documents.InvestigationReconsiderations;
using Mediator;

namespace ECER.Managers.Registry;

public class InvestigationReconsiderationHandlers(IInvestigationReconsiderationMapper investigationReconsiderationMapper, IInvestigationReconsiderationRepository investigationReconsiderationRepository)
  : IRequestHandler<InvestigationReconsiderationQueryCommand, InvestigationReconsiderationQueryResults>,
    IRequestHandler<InvestigationReconsiderationSubmitCommand, InvestigationReconsiderationSubmitResult>
{
  public async ValueTask<InvestigationReconsiderationQueryResults> Handle(InvestigationReconsiderationQueryCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var query = new InvestigationReconsiderationQuery
    {
      ById = request.ById,
      ByApplicantId = request.ByApplicantId,
      ByStatusCodes = request.ByStatusCodes != null ? investigationReconsiderationMapper.MapInvestigationReconsiderationStatusCodes(request.ByStatusCodes) : null
    };
    var investigationReconsiderations = await investigationReconsiderationRepository.Query(query, cancellationToken);
    return new InvestigationReconsiderationQueryResults(investigationReconsiderationMapper.MapInvestigationReconsiderationRequests(investigationReconsiderations));
  }

  public async ValueTask<InvestigationReconsiderationSubmitResult> Handle(InvestigationReconsiderationSubmitCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var query = new InvestigationReconsiderationQuery
    {
      ById = request.InvestigationReconsideration.Id,
      ByApplicantId = request.ApplicantId,
    };

    var investigationReconsiderations = await investigationReconsiderationRepository.Query(query, cancellationToken);

    var investigationReconsideration = investigationReconsiderations.FirstOrDefault();

    if (investigationReconsideration == null)
    {
      return new InvestigationReconsiderationSubmitResult() { IsSuccess = false, Id = request.InvestigationReconsideration.Id, ErrorCode = InvestigationReconsiderationSubmitErrorCode.InvestigationReconsiderationNotFound };
    }

    if (investigationReconsideration.Status != Resources.Documents.InvestigationReconsiderations.InvestigationReconsiderationStatusCode.New)
    {
      return new InvestigationReconsiderationSubmitResult() { IsSuccess = false, Id = request.InvestigationReconsideration.Id, ErrorCode = InvestigationReconsiderationSubmitErrorCode.InvestigationReconsiderationWrongStatus };
    }

    var submittedReconsiderationId = await investigationReconsiderationRepository.Submit(investigationReconsiderationMapper.MapInvestigationReconsiderationRequest(request.InvestigationReconsideration), request.ApplicantId, cancellationToken);

    return new InvestigationReconsiderationSubmitResult() { IsSuccess = true, Id = submittedReconsiderationId };
  }
}
