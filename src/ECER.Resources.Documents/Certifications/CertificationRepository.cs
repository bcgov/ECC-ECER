using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.Certifications;

internal class CertificationRepository : ICertificationRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public CertificationRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<IEnumerable<Certification>> Query(UserCertificationQuery query)
  {
    await Task.CompletedTask;
    var Certifications = from c in context.ecer_CertificateSet
                         select c;

    // Filtering by ID
    if (query.ById != null) Certifications = Certifications.Where(r => r.ecer_CertificateId == Guid.Parse(query.ById));

    // Filtering by applicant ID
    if (query.ByApplicantId != null) Certifications = Certifications.Where(r => r.ecer_Registrantid.Id == Guid.Parse(query.ByApplicantId));

    Certifications = Certifications.OrderByDescending(r => r.ecer_ExpiryDate);
    return mapper.Map<IEnumerable<Certification>>(Certifications.ToList());
  }
}
