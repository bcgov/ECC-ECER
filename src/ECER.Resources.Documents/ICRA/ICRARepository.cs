using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using ECER.Utilities.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.ICRA;

internal sealed partial class ICRARepository : IICRARepository
{

  private readonly EcerContext context;
  private readonly IMapper mapper;
  private readonly IObjecStorageProvider objectStorageProvider;
  private readonly IConfiguration configuration;

  public ICRARepository(
       EcerContext context,
       IObjecStorageProvider objectStorageProvider,
       IMapper mapper,
       IConfiguration configuration)
  {
    this.context = context;
    this.mapper = mapper;
    this.objectStorageProvider = objectStorageProvider;
    this.configuration = configuration;
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
      .Include(a => a.ecer_WorkExperienceRef_ecer_ICRAEligibilityAssessment_ecer_ICRAEligibilityAssessment)
      .Include(a => a.ecer_internationalcertification_EligibilityAssessment_ecer_icraeligibilityassessment)
      .IncludeNested(a => a.ecer_bcgov_documenturl_internationalcertificationid)
      .IncludeNested(a => a.ecer_internationalcertification_CountryId)
      .Execute();

    return mapper.Map<IEnumerable<ICRAEligibility>>(results)!.ToList();
  }

  public async Task<string> Save(ICRAEligibility iCRAEligibility, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(iCRAEligibility.ApplicantId));
    if (applicant == null) throw new InvalidOperationException($"Applicant '{iCRAEligibility.ApplicantId}' not found");

    var icraEligibility = mapper.Map<ecer_ICRAEligibilityAssessment>(iCRAEligibility)!;
    if (!icraEligibility.ecer_ICRAEligibilityAssessmentId.HasValue)
    {
      icraEligibility.ecer_ICRAEligibilityAssessmentId = Guid.NewGuid();
      icraEligibility.StatusCode = ecer_ICRAEligibilityAssessment_StatusCode.Draft;
      context.AddObject(icraEligibility);
      context.AddLink(icraEligibility, ecer_ICRAEligibilityAssessment.Fields.ecer_icraeligibilityassessment_ApplicantId, applicant);
    }
    else
    {
      var existingIcraEligibility = context.ecer_ICRAEligibilityAssessmentSet.SingleOrDefault(c => c.ecer_ICRAEligibilityAssessmentId == icraEligibility.ecer_ICRAEligibilityAssessmentId);
      if (existingIcraEligibility == null || existingIcraEligibility.StatusCode!=ecer_ICRAEligibilityAssessment_StatusCode.Draft) throw new InvalidOperationException($"ecer_ICRAEligibilityAssessmentId '{icraEligibility.ecer_ICRAEligibilityAssessmentId}' not found or is not draft!");

      if (icraEligibility.ecer_DateSigned.HasValue && existingIcraEligibility.ecer_DateSigned.HasValue) icraEligibility.ecer_DateSigned = existingIcraEligibility.ecer_DateSigned;

      icraEligibility.ecer_ICRAEligibilityAssessmentId = existingIcraEligibility.ecer_ICRAEligibilityAssessmentId;
      context.Detach(existingIcraEligibility);
      context.Attach(icraEligibility);
      context.UpdateObject(icraEligibility);
    }
    // Update international certifications and their files
    await UpdateInternationalCertifications(icraEligibility, applicant, iCRAEligibility.ApplicantId, iCRAEligibility.InternationalCertifications.ToList(), cancellationToken);
    // Update employment references
    await UpdateEmploymentReferences(icraEligibility, applicant, mapper.Map<List<ecer_WorkExperienceRef>>(iCRAEligibility.EmploymentReferences)!, cancellationToken);
    context.SaveChanges();
    return icraEligibility.ecer_ICRAEligibilityAssessmentId!.Value.ToString();
  }

  public async Task<string> Submit(string icraEligibilityId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var icra = context.ecer_ICRAEligibilityAssessmentSet.FirstOrDefault(d => d.ecer_ICRAEligibilityAssessmentId == Guid.Parse(icraEligibilityId) && d.StatusCode == ecer_ICRAEligibilityAssessment_StatusCode.Draft);
    if (icra == null) throw new InvalidOperationException($"ICRA Eligibility '{icraEligibilityId}' not found");

    icra.StatusCode = ecer_ICRAEligibilityAssessment_StatusCode.Submitted;
    context.UpdateObject(icra);
    context.SaveChanges();
    return icraEligibilityId;
  }
}
