using ECER.Managers.Registry.Contract.Reconsiderations;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.Reconsiderations;
using Mediator;

namespace ECER.Managers.Registry;

public class ReconsiderationHandlers(IReconsiderationMapper reconsiderationMapper, IReconsiderationRepository reconsiderationRepository, IApplicationRepository applicationRepository)
  : IRequestHandler<ReconsiderationQueryCommand, ReconsiderationQueryResults>,
    IRequestHandler<ReconsiderationSubmitCommand, ReconsiderationSubmitResult>
{
  public async ValueTask<ReconsiderationQueryResults> Handle(ReconsiderationQueryCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var query = new ReconsiderationQuery
    {
      ById = request.ById,
      ByApplicantId = request.ByApplicantId,
      ByApplicationId = request.ByApplicationId,
      ByStatusCodes = request.ByStatusCodes != null ? reconsiderationMapper.MapReconsiderationStatusCodes(request.ByStatusCodes) : null
    };
    var reconsiderations = await reconsiderationRepository.Query(query, cancellationToken);
    return new ReconsiderationQueryResults(reconsiderationMapper.MapReconsiderationRequests(reconsiderations));
  }

  public async ValueTask<ReconsiderationSubmitResult> Handle(ReconsiderationSubmitCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var query = new ReconsiderationQuery
    {
      ById = request.Reconsideration.Id,
      ByApplicantId = request.ApplicantId,
    };

    var reconsiderations = await reconsiderationRepository.Query(query, cancellationToken);

    var reconsideration = reconsiderations.FirstOrDefault();

    if (reconsideration == null)
    {
      return new ReconsiderationSubmitResult() { IsSuccess = false, Id = request.Reconsideration.Id, ErrorCode = ReconsiderationSubmitErrorCode.ReconsiderationNotFound };
    }

    if (reconsideration.Status != Resources.Documents.Reconsiderations.ReconsiderationStatusCode.New)
    {
      return new ReconsiderationSubmitResult() { IsSuccess = false, Id = request.Reconsideration.Id, ErrorCode = ReconsiderationSubmitErrorCode.ReconsiderationWrongStatus };
    }

    var applicationQuery = new ApplicationQuery
    {
      ById = reconsideration.ApplicationId,
      ByApplicantId = request.ApplicantId,
    };
    var applications = await applicationRepository.Query(applicationQuery, cancellationToken);
    var application = applications.FirstOrDefault();

    if (application == null)
    {
      return new ReconsiderationSubmitResult() { IsSuccess = false, Id = request.Reconsideration.Id, ErrorCode = ReconsiderationSubmitErrorCode.ApplicationNotFound };
    }

    var submittedReconsiderationId = await reconsiderationRepository.Submit(reconsiderationMapper.MapReconsiderationRequest(request.Reconsideration), request.ApplicantId, cancellationToken);

    return new ReconsiderationSubmitResult() { IsSuccess = true, Id = submittedReconsiderationId };
  }
}
