using AutoMapper;
using ECER.Resources.Documents.PortalInvitations;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using ECER.Utilities.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal sealed partial class ApplicationRepository : IApplicationRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;
  private readonly IObjecStorageProvider objectStorageProvider;
  private readonly IConfiguration configuration;

  public ApplicationRepository(
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

  public async Task<IEnumerable<Application>> Query(ApplicationQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var applications = context.ecer_ApplicationSet.Where(
      a => a.StatusCode!.Value != ecer_Application_StatusCode.Cancelled && 
      a.StatusCode!.Value != ecer_Application_StatusCode.Closed && 
      a.StatusCode!.Value != ecer_Application_StatusCode.Withdrawn);

    if (query.ByStatus != null && query.ByStatus.Any())
    {
      var statuses = mapper.Map<IEnumerable<ecer_Application_StatusCode>>(query.ByStatus)!.ToList();
      applications = applications.WhereIn(a => a.StatusCode!.Value, statuses);
    }

    if (query.ById != null) applications = applications.Where(r => r.ecer_ApplicationId == Guid.Parse(query.ById));
    if (query.ByApplicantId != null) applications = applications.Where(r => r.ecer_Applicantid.Id == Guid.Parse(query.ByApplicantId));

    // Order by CreatedOn descending
    applications = applications.OrderByDescending(a => a.CreatedOn);

    var results = context.From(applications)
      .Join()
      .Include(a => a.ecer_workexperienceref_Applicationid_ecer)
      .Include(a => a.ecer_application_lmprovinceid)
      .Include(a => a.ecer_application_certificationcomparisonid)
      .Include(a => a.ecer_characterreference_Applicationid)
      .Include(a => a.ecer_ecer_professionaldevelopment_Applicationi)
      .IncludeNested(a => a.ecer_bcgov_documenturl_ProfessionalDevelopmentId)
      .Include(a => a.ecer_transcript_Applicationid)
      .IncludeNested(a => a.ecer_transcript_InstituteCountryId)
      .IncludeNested(a => a.ecer_transcript_ProvinceId)
      .IncludeNested(a => a.ecer_transcript_postsecondaryinstitutionid)
      .IncludeNested(a => a.ecer_bcgov_documenturl_TranscriptId)
      .Execute();

    return mapper.Map<IEnumerable<Application>>(results)!.ToList();
  }

  public async Task<string> SaveApplication(Application application, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(application.ApplicantId));
    if (applicant == null) throw new InvalidOperationException($"Applicant '{application.ApplicantId}' not found");

    var ecerApplication = mapper.Map<ecer_Application>(application)!;
    if (application.Origin != null)
    {
      ecerApplication.ecer_Origin = mapper.Map<ecer_Origin>(application.Origin);
    }

    var ecerTranscripts = mapper.Map<IEnumerable<ecer_Transcript>>(application.Transcripts)!.ToList();
    var ecerWorkExperienceReferences = mapper.Map<IEnumerable<ecer_WorkExperienceRef>>(application.WorkExperienceReferences)!.ToList();
    var ecerCharacterReferences = mapper.Map<IEnumerable<ecer_CharacterReference>>(application.CharacterReferences)!.ToList();

    if (!ecerApplication.ecer_ApplicationId.HasValue)
    {
      ecerApplication.ecer_ApplicationId = Guid.NewGuid();
      context.AddObject(ecerApplication);
      context.AddLink(ecerApplication, ecer_Application.Fields.ecer_application_Applicantid_contact, applicant);
    }
    else
    {
      var existingApplication = context.ecer_ApplicationSet.SingleOrDefault(c => c.ecer_ApplicationId == ecerApplication.ecer_ApplicationId);
      if (existingApplication == null) throw new InvalidOperationException($"ecer_Application '{ecerApplication.ecer_ApplicationId}' not found");

      if (ecerApplication.ecer_DateSigned.HasValue && existingApplication.ecer_DateSigned.HasValue) ecerApplication.ecer_DateSigned = existingApplication.ecer_DateSigned;

      context.Detach(existingApplication);
      context.Attach(ecerApplication);
      context.UpdateObject(ecerApplication);
    }

    await UpdateProfessionalDevelopments(ecerApplication, applicant, application.ApplicantId, application.ProfessionalDevelopments.ToList(), cancellationToken);
    await UpdateWorkExperienceReferences(ecerApplication, ecerWorkExperienceReferences);
    await UpdateCharacterReferences(ecerApplication, ecerCharacterReferences);
    await UpdateTranscripts(ecerApplication, ecerTranscripts);

    if (application.ApplicationType == ApplicationTypes.LaborMobility)
    {
      var comparisonRecord = context.ecer_certificationcomparisonSet.SingleOrDefault(c => c.ecer_certificationcomparisonId == Guid.Parse(application.LabourMobilityCertificateInformation!.CertificateComparisonId!));
      if(comparisonRecord == null) throw new InvalidOperationException($"Save application '{ecerApplication.ecer_ApplicationId}' failed. Certification comparison '{application.LabourMobilityCertificateInformation!.CertificateComparisonId}' not found");
      context.AddLink(ecerApplication, ecer_Application.Fields.ecer_application_certificationcomparisonid, comparisonRecord);

      var province =context.ecer_ProvinceSet.SingleOrDefault(c => c.ecer_ProvinceId == Guid.Parse(application.LabourMobilityCertificateInformation!.LabourMobilityProvince!.ProvinceId));
      if(province == null) throw new InvalidOperationException($"Save application '{ecerApplication.ecer_ApplicationId}' failed. Province '{application.LabourMobilityCertificateInformation!.LabourMobilityProvince!.ProvinceId}' not found");
      context.AddLink(ecerApplication, ecer_Application.Fields.ecer_application_lmprovinceid, province);
    }

    context.SaveChanges();
    return ecerApplication.ecer_ApplicationId.Value.ToString();
  }

  public async Task<string> Submit(string applicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var application = context.ecer_ApplicationSet.FirstOrDefault(d => d.ecer_ApplicationId == Guid.Parse(applicationId) && d.StatusCode == ecer_Application_StatusCode.Draft);
    if (application == null) throw new InvalidOperationException($"Application '{applicationId}' not found");

    application.StatusCode = ecer_Application_StatusCode.Submitted;
    context.UpdateObject(application);

    context.SaveChanges();
    return applicationId;
  }

  public async Task<string> Cancel(string applicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var application = context.ecer_ApplicationSet.FirstOrDefault(
      d => d.ecer_ApplicationId == Guid.Parse(applicationId) && d.StatusCode == ecer_Application_StatusCode.Draft
      );
    if (application == null) throw new InvalidOperationException($"Application '{applicationId}' not found");
    application.StatusCode = ecer_Application_StatusCode.Cancelled;
    application.StateCode = ecer_application_statecode.Inactive;
    context.UpdateObject(application);
    context.SaveChanges();
    return applicationId;
  }

  public async Task<string> SubmitReference(SubmitReferenceRequest request, CancellationToken cancellationToken)
  {
    return request switch
    {
      CharacterReferenceSubmissionRequest req => await SubmitCharacterReference(request.PortalInvitation!.CharacterReferenceId!, req),
      WorkExperienceReferenceSubmissionRequest req => await SubmitWorkexperienceReference(request.PortalInvitation!.WorkexperienceReferenceId!, req),
      _ => throw new NotSupportedException($"{request.GetType().Name} is not supported")
    };
  }

  public async Task<string> OptOutReference(OptOutReferenceRequest request, CancellationToken cancellationToken) => request.PortalInvitation!.InviteType switch
  {
    InviteType.CharacterReference => await OptOutCharacterReference(request),
    InviteType.WorkExperienceReference => await OptOutWorkExperienceReference(request),
    _ => throw new NotSupportedException($"{request.GetType().Name} is not supported")
  };
}
