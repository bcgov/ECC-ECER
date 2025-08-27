using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.ICRA;

internal sealed partial class ICRARepository : IICRARepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public ICRARepository(
       EcerContext context,
       IMapper mapper,
       IConfiguration configuration)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<IEnumerable<ICRAEligibility>> Query(ICRAQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var icras = context.ecer_ICRAEligibilityAssessmentSet;

    if (query.ByStatus != null && query.ByStatus.Any())
    {
      var statuses = mapper.Map<IEnumerable<ecer_ICRAEligibilityAssessment_StatusCode>>(query.ByStatus)!.ToList();
      icras = icras.WhereIn(a => a.StatusCode!.Value, statuses);
    }

    if (query.ById != null) icras = icras.Where(r => r.ecer_ICRAEligibilityAssessmentId == Guid.Parse(query.ById));
    if (query.ByApplicantId != null) icras = icras.Where(r => r.ecer_ApplicantId.Id == Guid.Parse(query.ByApplicantId));

    icras = icras.OrderByDescending(a => a.CreatedOn);

    var results = context.From(icras)
      .Join()
      .Include(a => a.ecer_icraeligibilityassessment_ApplicantId)
      .Execute();

    return mapper.Map<IEnumerable<ICRAEligibility>>(results)!.ToList();
  }

  public async Task<string> Save(ICRAEligibility eligibility, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(eligibility.ApplicantId));
    if (applicant == null) throw new InvalidOperationException($"Applicant '{eligibility.ApplicantId}' not found");

    var icraEligibility = mapper.Map<ecer_ICRAEligibilityAssessment>(eligibility)!;
    if (!icraEligibility.ecer_ICRAEligibilityAssessmentId.HasValue)
    {
      icraEligibility.ecer_ICRAEligibilityAssessmentId = Guid.NewGuid();
      context.AddObject(icraEligibility);
      context.AddLink(icraEligibility, ecer_ICRAEligibilityAssessment.Fields.ecer_icraeligibilityassessment_ApplicantId, applicant);
    }
    else
    {
      var existingIcraEligibility = context.ecer_ICRAEligibilityAssessmentSet.SingleOrDefault(c => c.ecer_ICRAEligibilityAssessmentId == icraEligibility.ecer_ICRAEligibilityAssessmentId);
      if (existingIcraEligibility == null) throw new InvalidOperationException($"ecer_ICRAEligibilityAssessmentId '{icraEligibility.ecer_ICRAEligibilityAssessmentId}' not found");

      if (icraEligibility.ecer_DateSigned.HasValue && existingIcraEligibility.ecer_DateSigned.HasValue) icraEligibility.ecer_DateSigned = existingIcraEligibility.ecer_DateSigned;

      icraEligibility.ecer_ICRAEligibilityAssessmentId = existingIcraEligibility.ecer_ICRAEligibilityAssessmentId;
      context.UpdateObject(icraEligibility);
    }

    context.SaveChanges();
    return icraEligibility.ecer_ICRAEligibilityAssessmentId!.Value.ToString();
  }
}
