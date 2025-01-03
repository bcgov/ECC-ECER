﻿using AutoMapper;
using ECER.Managers.Registry.Contract.Certifications;
using ECER.Resources.Documents.Certifications;
using MediatR;

namespace ECER.Managers.Registry;

public class CertificationHandlers(ICertificationRepository CertificationRepository, IMapper mapper)
    : IRequestHandler<Contract.Certifications.UserCertificationQuery, CertificationsQueryResults>

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
}
