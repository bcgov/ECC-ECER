using AngleSharp.Dom;
using AutoMapper;
using ECER.Managers.Registry.Contract.Certifications;
using ECER.Resources.Documents.Certifications;
using MediatR;
using System.Collections.Generic;
using CertificateStatusCode = ECER.Resources.Documents.Certifications.CertificateStatusCode;

namespace ECER.Managers.Registry;

public class CertificationHandlers(ICertificationRepository CertificationRepository, IMapper mapper)
    : IRequestHandler<Contract.Certifications.UserCertificationQuery, CertificationsQueryResults>,
  IRequestHandler<UserCertificationQueryLookup, CertificationsQueryResults>,
  IRequestHandler<RequestCertificationPdfCommand, CertificationRequestPdfResult>

{
  public async Task<CertificationsQueryResults> Handle(Contract.Certifications.UserCertificationQuery request, CancellationToken cancellationToken)
  {
    var Certifications = await GetCertificationsPaginated(request);

    return new CertificationsQueryResults(mapper.Map<IEnumerable<Contract.Certifications.Certification>>(Certifications)!);
  }

  public async Task<CertificationRequestPdfResult> Handle(RequestCertificationPdfCommand request, CancellationToken cancellationToken)
  {
    var certificates = await CertificationRepository.Query(new Resources.Documents.Certifications.UserCertificationQuery
    {
      ById = request?.certificationId,
      ByApplicantId = request?.userId
    });
    var certificate = certificates.FirstOrDefault();
    await CertificationRepository.RequestPdf(certificate!.Id, cancellationToken);
    return new CertificationRequestPdfResult(certificate!.Id);
  }

  public async Task<CertificationsQueryResults> Handle(
    UserCertificationQueryLookup request,
    CancellationToken cancellationToken)
  {
    var certifications = await GetCertificationsPaginated(request);

    var results = new List<Resources.Documents.Certifications.Certification>();

    foreach (var group in certifications.GroupBy(c => c.RegistrantId))
    {
      // 1) Collect and sort all active certs
      var certs = group
          .Where(c => (c.StatusCode == CertificateStatusCode.Active || c.StatusCode == CertificateStatusCode.Suspended || c.StatusCode == CertificateStatusCode.Cancelled) && c.ExpiryDate >= DateTime.UtcNow.Date)
          .OrderByDescending(c => c.ExpiryDate)
          .ThenBy(c => c.BaseCertificateTypeId)
          .ToList();
      results.AddRange(certs);

    }

    // map and return results
    var dtos = mapper.Map<IEnumerable<Contract.Certifications.Certification>>(results)!;
    return new CertificationsQueryResults(dtos);
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
