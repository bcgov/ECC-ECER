using ECER.Resources.Documents.PortalInvitations;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using ECER.Utilities.ObjectStorage.Providers;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal sealed partial class ApplicationRepository : IApplicationRepository
{
  private readonly EcerContext context;
  private readonly IApplicationRepositoryMapper mapper;
  private readonly IObjectStorageProviderResolver objectStorageProviderResolver;

  public ApplicationRepository(
       EcerContext context,
       IObjectStorageProviderResolver objectStorageProviderResolver,
       IApplicationRepositoryMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
    this.objectStorageProviderResolver = objectStorageProviderResolver;
  }

  public async Task<IEnumerable<Application>> Query(ApplicationQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var applications = context.ecer_ApplicationSet.Where(
      a => a.StatusCode!.Value != ecer_Application_StatusCode.Withdrawn &&
      a.StatusCode!.Value != ecer_Application_StatusCode.Closed &&
      a.StatusCode!.Value != ecer_Application_StatusCode.NotSubmitted);

    if (query.ByStatus != null && query.ByStatus.Any())
    {
      var statuses = mapper.MapApplicationStatuses(query.ByStatus);
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

    HydrateTranscriptLookups(results);
    return mapper.MapApplications(results);
  }

  public async Task<string> SaveApplication(Application application, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(application.ApplicantId));
    if (applicant == null) throw new InvalidOperationException($"Applicant '{application.ApplicantId}' not found");

    var ecerApplication = mapper.MapApplication(application);
    if (application.Origin != null)
    {
      ecerApplication.ecer_Origin = mapper.MapOrigin(application.Origin);
    }

    var ecerTranscripts = mapper.MapTranscripts(application.Transcripts);
    var ecerWorkExperienceReferences = mapper.MapWorkExperienceReferences(application.WorkExperienceReferences);
    var ecerCharacterReferences = mapper.MapCharacterReferences(application.CharacterReferences);

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

    if (application.ApplicationType == ApplicationTypes.LabourMobility)
    {
      if (application.LabourMobilityCertificateInformation != null && application.LabourMobilityCertificateInformation.CertificateComparisonId != null)
      {
        var comparisonRecord = context.ecer_certificationcomparisonSet.SingleOrDefault(c => c.ecer_certificationcomparisonId == Guid.Parse(application.LabourMobilityCertificateInformation.CertificateComparisonId));
        if (comparisonRecord == null) throw new InvalidOperationException($"Save application '{ecerApplication.ecer_ApplicationId}' failed. Certification comparison '{application.LabourMobilityCertificateInformation.CertificateComparisonId}' not found");
        context.AddLink(ecerApplication, ecer_Application.Fields.ecer_application_certificationcomparisonid, comparisonRecord);
      }

      if (application.LabourMobilityCertificateInformation != null && application.LabourMobilityCertificateInformation.LabourMobilityProvince != null)
      {
        var province = context.ecer_ProvinceSet.SingleOrDefault(c => c.ecer_ProvinceId == Guid.Parse(application.LabourMobilityCertificateInformation.LabourMobilityProvince.ProvinceId));
        if (province == null) throw new InvalidOperationException($"Save application '{ecerApplication.ecer_ApplicationId}' failed. Province '{application.LabourMobilityCertificateInformation.LabourMobilityProvince.ProvinceId}' not found");
        context.AddLink(ecerApplication, ecer_Application.Fields.ecer_application_lmprovinceid, province);
      }
    }

    if (!string.IsNullOrEmpty(application.FromCertificate))
    {
      var certification = context.ecer_CertificateSet.SingleOrDefault(c => c.ecer_CertificateId == Guid.Parse(application.FromCertificate) && c.ecer_Registrantid.Id == Guid.Parse(application.ApplicantId));
      if (certification == null) throw new InvalidOperationException($"Save application '{ecerApplication.ecer_ApplicationId}' failed. Certification '{application.FromCertificate}' not found");
      context.AddLink(ecerApplication, ecer_Application.Fields.ecer_application_FromCertificateId, certification);
    }

    context.SaveChanges();
    return ecerApplication.ecer_ApplicationId.Value.ToString();
  }

  private void HydrateTranscriptLookups(IEnumerable<ecer_Application> applications)
  {
    var transcripts = applications
      .SelectMany(application => application.ecer_transcript_Applicationid ?? Array.Empty<ecer_Transcript>())
      .ToList();

    if (transcripts.Count == 0)
    {
      return;
    }

    var countriesById = LoadCountriesById(GetMissingCountryIds(transcripts));
    var provincesById = LoadProvincesById(GetMissingProvinceIds(transcripts));
    var institutionsById = LoadInstitutionsById(GetMissingInstitutionIds(transcripts));

    foreach (var transcript in transcripts)
    {
      HydrateTranscriptLookup(transcript, countriesById, provincesById, institutionsById);
    }
  }

  private static List<Guid> GetMissingCountryIds(IEnumerable<ecer_Transcript> transcripts) =>
    transcripts
      .Where(transcript => transcript.ecer_transcript_InstituteCountryId == null && transcript.ecer_InstituteCountryId != null)
      .Select(transcript => transcript.ecer_InstituteCountryId.Id)
      .Distinct()
      .ToList();

  private static List<Guid> GetMissingProvinceIds(IEnumerable<ecer_Transcript> transcripts) =>
    transcripts
      .Where(transcript => transcript.ecer_transcript_ProvinceId == null && transcript.ecer_ProvinceId != null)
      .Select(transcript => transcript.ecer_ProvinceId.Id)
      .Distinct()
      .ToList();

  private static List<Guid> GetMissingInstitutionIds(IEnumerable<ecer_Transcript> transcripts) =>
    transcripts
      .Where(transcript => transcript.ecer_transcript_postsecondaryinstitutionid == null && transcript.ecer_postsecondaryinstitutionid != null)
      .Select(transcript => transcript.ecer_postsecondaryinstitutionid.Id)
      .Distinct()
      .ToList();

  private Dictionary<Guid, ecer_Country> LoadCountriesById(List<Guid> countryIds) =>
    countryIds.Count == 0
      ? new Dictionary<Guid, ecer_Country>()
      : context.ecer_CountrySet
        .WhereIn(country => country.ecer_CountryId!.Value, countryIds)
        .ToDictionary(country => country.ecer_CountryId!.Value);

  private Dictionary<Guid, ecer_Province> LoadProvincesById(List<Guid> provinceIds) =>
    provinceIds.Count == 0
      ? new Dictionary<Guid, ecer_Province>()
      : context.ecer_ProvinceSet
        .WhereIn(province => province.ecer_ProvinceId!.Value, provinceIds)
        .ToDictionary(province => province.ecer_ProvinceId!.Value);

  private Dictionary<Guid, ecer_PostSecondaryInstitute> LoadInstitutionsById(List<Guid> institutionIds) =>
    institutionIds.Count == 0
      ? new Dictionary<Guid, ecer_PostSecondaryInstitute>()
      : context.ecer_PostSecondaryInstituteSet
        .WhereIn(institution => institution.ecer_PostSecondaryInstituteId!.Value, institutionIds)
        .ToDictionary(institution => institution.ecer_PostSecondaryInstituteId!.Value);

  private static void HydrateTranscriptLookup(
    ecer_Transcript transcript,
    Dictionary<Guid, ecer_Country> countriesById,
    Dictionary<Guid, ecer_Province> provincesById,
    Dictionary<Guid, ecer_PostSecondaryInstitute> institutionsById)
  {
    if (transcript.ecer_transcript_InstituteCountryId == null &&
        transcript.ecer_InstituteCountryId != null &&
        countriesById.TryGetValue(transcript.ecer_InstituteCountryId.Id, out var country))
    {
      transcript.ecer_transcript_InstituteCountryId = country;
    }

    if (transcript.ecer_transcript_ProvinceId == null &&
        transcript.ecer_ProvinceId != null &&
        provincesById.TryGetValue(transcript.ecer_ProvinceId.Id, out var province))
    {
      transcript.ecer_transcript_ProvinceId = province;
    }

    if (transcript.ecer_transcript_postsecondaryinstitutionid == null &&
        transcript.ecer_postsecondaryinstitutionid != null &&
        institutionsById.TryGetValue(transcript.ecer_postsecondaryinstitutionid.Id, out var institution))
    {
      transcript.ecer_transcript_postsecondaryinstitutionid = institution;
    }
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
    application.StatusCode = ecer_Application_StatusCode.Withdrawn;
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
      IcraWorkExperienceReferenceSubmissionRequest req => await SubmitIcraWorkExperienceReference(request.PortalInvitation!.WorkexperienceReferenceId!, req),
      _ => throw new NotSupportedException($"{request.GetType().Name} is not supported")
    };
  }

  public async Task<string> OptOutReference(OptOutReferenceRequest request, CancellationToken cancellationToken) => request.PortalInvitation!.InviteType switch
  {
    InviteType.CharacterReference => await OptOutCharacterReference(request),
    InviteType.WorkExperienceReferenceforApplication => await OptOutWorkExperienceReference(request),
    InviteType.WorkExperienceReferenceforICRA => await OptOutWorkExperienceReference(request),
    _ => throw new NotSupportedException($"{request.GetType().Name} is not supported")
  };
}
