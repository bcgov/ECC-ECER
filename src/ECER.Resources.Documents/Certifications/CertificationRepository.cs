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

    // Initial join without filtering
    var queryWithJoin = from cert in Certifications
                        join reg in Registrants on cert.ecer_Registrantid.Id equals reg.Id
                        select new { cert, reg };

    // Filtering by ID
    if (query.ById != null) queryWithJoin = queryWithJoin.Where(r => r.cert.ecer_CertificateId == Guid.Parse(query.ById));

    // Filtering by applicant ID
    if (query.ByApplicantId != null) queryWithJoin = queryWithJoin.Where(r => r.cert.ecer_Registrantid.Id == Guid.Parse(query.ByApplicantId));

    // Filtering by First Name
    if (!string.IsNullOrEmpty(query.ByFirstName))
    {
      queryWithJoin = queryWithJoin.Where(x => x.reg.FirstName == query.ByFirstName);
    }

    // Filtering by Last Name
    if (!string.IsNullOrEmpty(query.ByLastName))
    {
      queryWithJoin = queryWithJoin.Where(x => x.reg.LastName == query.ByLastName);
    }

    // Filtering by certificate number
    if (!string.IsNullOrEmpty(query.ByCertificateNumber))
      queryWithJoin = queryWithJoin.Where(r => r.cert.ecer_CertificateNumber.Equals(query.ByCertificateNumber));

    //Order by latest first (based on expiry date), then apply pagination
    queryWithJoin = queryWithJoin.OrderByDescending(r => r.cert.ecer_ExpiryDate).Skip(query.PageNumber).Take(query.PageSize);

    var results = context.From(queryWithJoin.Select(c => c.cert))
      .Join()
      .Include(a => a.ecer_certifiedlevel_CertificateId)
      .Include(a => a.ecer_documenturl_CertificateId)
      .Include(a => a.ecer_certificate_Registrantid)
      .IncludeNested(a => a.ecer_certificateconditions_Registrantid)
      .Execute().GroupBy(r => r.ecer_Registrantid.Id).Select(g => g.FirstOrDefault());

    return mapper.Map<IEnumerable<Certification>>(results)!.ToList();
  }
}
