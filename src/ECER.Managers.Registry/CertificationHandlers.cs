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
    var Certifications = await GetCertificationsPaginated(request);

    return new CertificationsQueryResults(mapper.Map<IEnumerable<Contract.Certifications.Certification>>(Certifications)!);
  }

  public async Task<CertificationsQueryResults> Handle(UserCertificationQueryLookup request, CancellationToken cancellationToken)
  {
    var Certifications = await GetCertificationsPaginated(request);

    //additional logic for query lookup which only wants 1 certification returned for each registrant
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

  private async Task<IEnumerable<Resources.Documents.Certifications.Certification>> GetCertificationsPaginated(UserCertificationQueryBase request)
  {
    ArgumentNullException.ThrowIfNull(CertificationRepository);
    ArgumentNullException.ThrowIfNull(mapper);
    ArgumentNullException.ThrowIfNull(request);

    request.PageSize = (request.PageSize == 0) ? 100 : request.PageSize; // By Default get Max. 100 records.

    return await CertificationRepository.Query(new Resources.Documents.Certifications.UserCertificationQuery
    {
      ById = request.ById,
      ByApplicantId = request.ByApplicantId,
      ByCertificateNumber = request.ByCertificateNumber,
      ByFirstName = request.ByFirstName,
      ByLastName = request.ByLastName,
      PageNumber = request.PageNumber,
      PageSize = request.PageSize,
    });
  }
}
