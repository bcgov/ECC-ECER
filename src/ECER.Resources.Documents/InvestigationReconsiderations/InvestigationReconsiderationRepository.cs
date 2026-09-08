using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.InvestigationReconsiderations;

internal sealed class InvestigationReconsiderationRepository : IInvestigationReconsiderationRepository
{
  private readonly EcerContext context;
  private readonly IInvestigationReconsiderationRepositoryMapper mapper;
  private readonly IObjectStorageProviderResolver objectStorageProviderResolver;

  public InvestigationReconsiderationRepository(EcerContext context, IInvestigationReconsiderationRepositoryMapper mapper, IObjectStorageProviderResolver objectStorageProviderResolver)
  {
    this.context = context;
    this.mapper = mapper;
    this.objectStorageProviderResolver = objectStorageProviderResolver;
  }

  public async Task<IEnumerable<InvestigationReconsideration>> Query(InvestigationReconsiderationQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var investigationReconsiderations = context.ecer_ReconsiderationInvestigationOutcomeSet;

    if (query.ByApplicantId != null)
    {
      investigationReconsiderations = investigationReconsiderations.Where(r => r.ecer_RegistrantId.Id == Guid.Parse(query.ByApplicantId));
    }

    if (query.ById != null)
    {
      investigationReconsiderations = investigationReconsiderations.Where(r => r.Id == Guid.Parse(query.ById));
    }

    if (query.ByStatusCodes != null)
    {
      var statuses = mapper.MapInvestigationReconsiderationStatusCodes(query.ByStatusCodes);
      investigationReconsiderations = investigationReconsiderations.WhereIn(r => r.StatusCode!.Value, statuses);
    }
    var results = context.From(investigationReconsiderations)
      .Join()
      .Include(r => r.ecer_bcgov_documenturl_ReconsiderationInvestigationOutcomeId)
      .Include(r => r.ecer_reconsiderationinvestigationoutcome_InvestigationId)
      .Include(r => r.ecer_reconsiderationinvestigationoutcome_ImmediateActionId)
      .Execute();

    return mapper.MapInvestigationReconsiderationRequests(results).ToList();
  }

  public async Task<string> Submit(InvestigationReconsideration investigationReconsideration, string applicantId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    if (!Guid.TryParse(investigationReconsideration.Id, out var reconsiderationId))
    {
      throw new ArgumentException($"Invalid or missing investigation reconsideration request id: {investigationReconsideration.Id}");
    }

    if (!Guid.TryParse(applicantId, out var parsedApplicantId))
    {
      throw new ArgumentException($"Invalid or missing applicant id: {applicantId}");
    }

    var existingReconsideration = context.ecer_ReconsiderationInvestigationOutcomeSet
      .FirstOrDefault(r => r.Id == reconsiderationId && r.ecer_RegistrantId.Id == parsedApplicantId);

    if (existingReconsideration == null)
    {
      throw new InvalidOperationException($"Investigation reconsideration request id: {investigationReconsideration.Id} for applicant id: {applicantId} not found");
    }

    if (existingReconsideration.StatusCode != ecer_ReconsiderationInvestigationOutcome_StatusCode.New)
    {
      throw new InvalidOperationException($"Investigation reconsideration request id: {investigationReconsideration.Id} status is not in status code New for submission it is {investigationReconsideration.Status}");
    }

    var updatedInvestigationReconsideration = mapper.MapInvestigationReconsiderationRequest(investigationReconsideration);
    updatedInvestigationReconsideration.StatusCode = ecer_ReconsiderationInvestigationOutcome_StatusCode.InReview;
    updatedInvestigationReconsideration.ecer_SubmittedDate = DateTime.Now;

    context.Detach(existingReconsideration);
    context.Attach(updatedInvestigationReconsideration);
    context.UpdateObject(updatedInvestigationReconsideration);

    await HandleAddInvestigationReconsiderationFiles(updatedInvestigationReconsideration, investigationReconsideration, investigationReconsideration.Files, cancellationToken);

    context.SaveChanges();
    return reconsiderationId.ToString();
  }

  private async Task HandleAddInvestigationReconsiderationFiles(ecer_ReconsiderationInvestigationOutcome ecer_investigationReconsideration, InvestigationReconsideration investigationReconsideration, IEnumerable<FileInfo> tobeAddedFiles, CancellationToken ct)
  {
    await Task.CompletedTask;

    foreach (var document in tobeAddedFiles)
    {
      if (string.IsNullOrEmpty(document.Id))
      {
        throw new InvalidOperationException($"Document '{document.Id}' is not valid");
      }

      var sourceFolder = "tempfolder";
      var destinationFolder = "ecer_reconsiderationinvestigationoutcome/" + investigationReconsideration.Id;
      var fileId = document.Id;
      var objectStorageProvider = objectStorageProviderResolver.resolve(document.EcerWebApplicationType);
      var file = await objectStorageProvider.GetAsync(new S3Descriptor(objectStorageProvider.BucketName, fileId, sourceFolder), ct);

      await objectStorageProvider.MoveAsync(new S3Descriptor(objectStorageProvider.BucketName, fileId, sourceFolder), new S3Descriptor(objectStorageProvider.BucketName, fileId, destinationFolder), ct);

      var documenturl = new bcgov_DocumentUrl()
      {
        bcgov_DocumentUrlId = Guid.Parse(fileId),
        bcgov_Url = destinationFolder,
        bcgov_FileName = file!.FileName,
        bcgov_FileSize = Infrastructure.Common.UtilityFunctions.HumanFileSize(file.Content.Length),
        bcgov_FileExtension = document.Extention,
        StatusCode = bcgov_DocumentUrl_StatusCode.Active,
        StateCode = bcgov_documenturl_statecode.Active,
        bcgov_OriginCode = bcgov_OriginCode.Web,
        ecer_DocumentInternallyReviewed = ecer_YesNoNull.No,
        ecer_ApplicationName = document.EcerWebApplicationType.ToString()
      };

      context.AddObject(documenturl);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_ReconsiderationInvestigationOutcomeId, ecer_investigationReconsideration);
    }
  }
}
