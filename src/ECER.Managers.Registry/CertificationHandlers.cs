using AutoMapper;
using ECER.Managers.Registry.Contract.Certifications;
using ECER.Resources.Documents.Certifications;
using MediatR;
using CertificateStatusCode = ECER.Resources.Documents.Certifications.CertificateStatusCode;

namespace ECER.Managers.Registry;

public class CertificationHandlers(ICertificationRepository CertificationRepository, IMapper mapper)
    : IRequestHandler<Contract.Certifications.UserCertificationQuery, CertificationsQueryResults>,
  IRequestHandler<UserCertificationQueryLookup, CertificationsQueryResults>

{
  public async Task<CertificationsQueryResults> Handle(Contract.Certifications.UserCertificationQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(CertificationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    request.PageSize = (request.PageSize == 0) ? 100 : request.PageSize; // By Default get Max. 100 records.

    var Certifications = await CertificationRepository.Query(new Resources.Documents.Certifications.UserCertificationQuery
    {
      ById = request.ById,
      ByApplicantId = request.ByApplicantId,
      ByCertificateNumber = request.ByCertificateNumber,
      ByFirstName = request.ByFirstName,
      ByLastName = request.ByLastName,
      PageNumber = request.PageNumber,
      PageSize = request.PageSize,
    });

    return new CertificationsQueryResults(mapper.Map<IEnumerable<Contract.Certifications.Certification>>(Certifications)!);
  }

  public async Task<CertificationsQueryResults> Handle(UserCertificationQueryLookup request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(CertificationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    request.PageSize = (request.PageSize == 0) ? 100 : request.PageSize; // By Default get Max. 100 records.

    var Certifications = await CertificationRepository.Query(new Resources.Documents.Certifications.UserCertificationQuery
    {
      ById = request.ById,
      ByApplicantId = request.ByApplicantId,
      ByCertificateNumber = request.ByCertificateNumber,
      ByFirstName = request.ByFirstName,
      ByLastName = request.ByLastName,
      PageNumber = request.PageNumber,
      PageSize = request.PageSize,
    });

    var sortedCertifications = Certifications.GroupBy(c => c.RegistrantId).Select(g => g.OrderBy(c =>
    {
      switch (c.StatusCode)
      {
        case CertificateStatusCode.Active:
          return 1;

        case CertificateStatusCode.Cancelled:
          return 2;

        case CertificateStatusCode.Suspended:
          return 3;

        case CertificateStatusCode.Expired:
          return 4;

        default:
          return 5;
      }
    }).ThenByDescending(c => c.ExpiryDate)
    .ThenBy(r => r.BaseCertificateTypeId) //accounts for certificates with the same expiry date we rank them by type
    .FirstOrDefault());

    return new CertificationsQueryResults(mapper.Map<IEnumerable<Contract.Certifications.Certification>>(sortedCertifications)!);
  }
}
