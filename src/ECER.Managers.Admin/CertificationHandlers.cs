using AutoMapper;
using ECER.Managers.Admin.Contract.Certifications;
using ECER.Resources.Documents.Certifications;
using MediatR;

namespace ECER.Managers.Admin;

public class CertificationHandlers(ICertificationRepository certificationRepository,
    IMapper mapper)
  : IRequestHandler<GetCertificationsCommand, GetCertificationsCommandResponse>
{
  public async Task<GetCertificationsCommandResponse> Handle(GetCertificationsCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var certifications = await certificationRepository.QueryCertificateSummary(new UserCertificationSummaryQuery() { ById = request.Id });
    return new GetCertificationsCommandResponse(mapper.Map<IEnumerable<ECER.Managers.Admin.Contract.Certifications.CertificationSummary>>(certifications)!);
  }
}
