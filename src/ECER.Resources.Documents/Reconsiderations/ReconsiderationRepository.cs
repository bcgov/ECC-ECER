using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Reconsiderations;

internal sealed class ReconsiderationRepository : IReconsiderationRepository
{
  private readonly EcerContext context;
  private readonly IReconsiderationRepositoryMapper mapper;
  private readonly IObjectStorageProviderResolver objectStorageProviderResolver;

  public ReconsiderationRepository(EcerContext context, IReconsiderationRepositoryMapper mapper, IObjectStorageProviderResolver objectStorageProviderResolver)
  {
    this.context = context;
    this.mapper = mapper;
    this.objectStorageProviderResolver = objectStorageProviderResolver;
  }

  public async Task<IEnumerable<Reconsideration>> Query(ReconsiderationQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var reconsiderations = context.ecer_ReconsiderationRequestSet;

    if (query.ByApplicantId != null)
    {
      reconsiderations = reconsiderations.Where(r => r.ecer_ApplicantId.Id == Guid.Parse(query.ByApplicantId));
    }

    if (query.ById != null)
    {
      reconsiderations = reconsiderations.Where(r => r.Id == Guid.Parse(query.ById));
    }

    if (query.ByStatusCodes != null)
    {
      var statuses = mapper.MapReconsiderationStatusCodes(query.ByStatusCodes);
      reconsiderations = reconsiderations.WhereIn(r => r.StatusCode!.Value, statuses);
    }
    var results = context.From(reconsiderations)
      .Join()
      .Include(r => r.ecer_bcgov_documenturl_ReconsiderationRequestId)
      .Include(r => r.ecer_reconsiderationrequest_ApplicationId)
      .Execute();

    return mapper.MapReconsiderationRequests(results).ToList();
  }

  public async Task<string> Submit(Reconsideration reconsideration, string applicantId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    if (!Guid.TryParse(reconsideration.Id, out var reconsiderationId))
    {
      throw new ArgumentException($"Invalid or missing reconsideration request id: {reconsideration.Id}");
    }

    if (!Guid.TryParse(applicantId, out var parsedApplicantId))
    {
      throw new ArgumentException($"Invalid or missing applicant id: {applicantId}");
    }

    var existingReconsideration = context.ecer_ReconsiderationRequestSet
      .FirstOrDefault(r => r.Id == reconsiderationId && r.ecer_ApplicantId.Id == parsedApplicantId);

    if (existingReconsideration == null)
    {
      throw new InvalidOperationException($"Reconsideration request id: {reconsideration.Id} for applicant id: {applicantId} not found");
    }

    if (existingReconsideration.StatusCode != ecer_ReconsiderationRequest_StatusCode.New)
    {
      throw new InvalidOperationException($"Reconsideration request id: {reconsideration.Id} status is not in status code New for submission it is {reconsideration.Status}");
    }

    var existingApplication = context.ecer_ApplicationSet.FirstOrDefault(r => r.Id == existingReconsideration.ecer_ApplicationId.Id);
    if (existingApplication == null)
    {
      throw new InvalidOperationException($"Reconsideration request id: {reconsideration.Id} is not associated to an application");
    }
    context.BeginTransaction();

    var updatedReconsideration = mapper.MapReconsiderationRequest(reconsideration);
    updatedReconsideration.StatusCode = ecer_ReconsiderationRequest_StatusCode.InReview;
    updatedReconsideration.ecer_DeclarationandSubmitteddate = DateTime.Now;

    existingApplication.StatusCode = ecer_Application_StatusCode.Dispute;
    context.UpdateObject(existingApplication);

    context.Detach(existingReconsideration);
    context.Attach(updatedReconsideration);
    context.UpdateObject(updatedReconsideration);

    await HandleAddReconsiderationFiles(updatedReconsideration, reconsideration, reconsideration.Files, existingReconsideration.ecer_ApplicationId?.Id.ToString() ?? string.Empty, applicantId, cancellationToken);

    context.CommitTransaction();
    return reconsiderationId.ToString();
  }

  private async Task HandleAddReconsiderationFiles(ecer_ReconsiderationRequest ecer_reconsideration, Reconsideration reconsideration, IEnumerable<FileInfo> tobeAddedFiles, string applicationId, string applicantId, CancellationToken ct)
  {
    await Task.CompletedTask;

    var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(applicantId));
    if (applicant == null) throw new InvalidOperationException($"Applicant id '{applicantId}' for reconsideration id {reconsideration.Id} not found");

    var application = context.ecer_ApplicationSet.SingleOrDefault(a => a.Id == Guid.Parse(applicationId));
    if (application == null) throw new InvalidOperationException($"Application id '{reconsideration.ApplicationId}' for reconsideration id {reconsideration.Id} not found");

    foreach (var document in tobeAddedFiles)
    {
      if (string.IsNullOrEmpty(document.Id))
      {
        throw new InvalidOperationException($"Document '{document.Id}' is not valid");
      }

      var sourceFolder = "tempfolder";
      var destinationFolder = "ecer_reconsiderationrequest/" + reconsideration.Id;
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
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.bcgov_contact_bcgov_documenturl, applicant);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_ReconsiderationRequestId, ecer_reconsideration);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_documenturl_ApplicationId, application);
    }
  }
}
