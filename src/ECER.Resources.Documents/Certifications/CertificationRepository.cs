using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;

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
    var Certifications = context.ecer_CertificateSet.AsQueryable();
    var Registrants = context.ContactSet.AsQueryable();

    // Filtering by ID
    if (query.ById != null) Certifications = Certifications.Where(r => r.ecer_CertificateId == Guid.Parse(query.ById));

    // Filtering by applicant ID
    if (query.ByApplicantId != null) Certifications = Certifications.Where(r => r.ecer_Registrantid.Id == Guid.Parse(query.ByApplicantId));

    // Filtering by First Name
    if (!string.IsNullOrEmpty(query.ByFirstName))
    {
      Certifications = from cert in Certifications
                       join reg in Registrants on cert.ecer_Registrantid.Id equals reg.Id
                       where reg.FirstName == (query.ByFirstName)
                       select cert;
    }

    // Filtering by Last Name
    if (!string.IsNullOrEmpty(query.ByLastName))
    {
      Certifications = from cert in Certifications
                       join reg in Registrants on cert.ecer_Registrantid.Id equals reg.Id
                       where reg.FirstName == (query.ByLastName)
                       select cert;
    }

    // Filtering by certificate number
    if (!string.IsNullOrEmpty(query.ByCertificateNumber))
      Certifications = Certifications.Where(r => r.ecer_CertificateNumber.Contains(query.ByCertificateNumber));

    var results = context.From(Certifications)
      .Join()
      .Include(a => a.ecer_certifiedlevel_CertificateId)
      .Include(a => a.ecer_documenturl_CertificateId)
      .Include(a => a.ecer_certificate_Registrantid)
      .IncludeNested(a => a.ecer_certificateconditions_Registrantid)
      .Execute().GroupBy(r => r.ecer_Registrantid.Id) // Group by unique identifier (assuming RegistrantId)
             .Select(g => g.OrderByDescending(r => r.ecer_ExpiryDate).FirstOrDefault()); // Select latest by expiry date

    return mapper.Map<IEnumerable<Certification>>(results)!.ToList();
  }
}
